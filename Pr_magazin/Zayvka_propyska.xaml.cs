using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Pr_magazin
{
    public partial class Zayvka_propyska : Window
    {
        private int currentUserId;
        
        public Zayvka_propyska(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            
        }

        private void Otpravit(object sender, RoutedEventArgs e)
        {
            StringBuilder errorMessage = new StringBuilder();
            if (string.IsNullOrWhiteSpace(FirstnameTextBox.Text) || !Regex.IsMatch(FirstnameTextBox.Text, @"^[а-яА-Яa-zA-Z]+$") || ContainsWhitespace(FirstnameTextBox.Text))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректное имя.");
            }

            if (string.IsNullOrWhiteSpace(LastnameTextBox.Text) || !Regex.IsMatch(LastnameTextBox.Text, @"^[а-яА-Яa-zA-Z]+$") || ContainsWhitespace(LastnameTextBox.Text))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректную фамилию.");
            }

            if (string.IsNullOrWhiteSpace(SurnameTextBox.Text) || !Regex.IsMatch(SurnameTextBox.Text, @"^[а-яА-Яa-zA-Z]+$") || ContainsWhitespace(SurnameTextBox.Text))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректное отчество.");
            }

            if (PurposeComboBox.SelectedItem == null)
            {
                errorMessage.AppendLine("Пожалуйста, выберите назначение.");
            }

            if (errorMessage.Length > 0)
            {
                MessageBox.Show(errorMessage.ToString(), "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (buro_propyskovEntities2 db = new buro_propyskovEntities2())
                {
                    string selectedPurpose = (PurposeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    string srokIspolzovaniya = GetSrokIspolzovaniyaForPurpose(selectedPurpose);

                    zayvka newZayvka = new zayvka
                    {
                        ID_client = currentUserId,
                        Lastname = LastnameTextBox.Text,
                        Firstname = FirstnameTextBox.Text,
                        Surname = SurnameTextBox.Text,
                        Date_podachi_zayvki = DateTime.Now,
                        Number_zayvki = GenerateZayvkaNumber(),
                        Naznachenie = selectedPurpose
                    };

                    db.zayvka.Add(newZayvka);
                    db.SaveChanges();

                    propysk newPropysk = new propysk
                    {
                        ID_zayvki = newZayvka.ID_zayvki,
                        Naznachenie = newZayvka.Naznachenie,
                        Status = "Изготавливается",
                        Srok_ispolzovaniya = srokIspolzovaniya
                    };

                    db.propysk.Add(newPropysk);
                    db.SaveChanges();

                    client clientToUpdate = db.client.FirstOrDefault(c => c.ID_client == currentUserId);
                    if (clientToUpdate != null)
                    {
                        clientToUpdate.id_propyska = newPropysk.ID_propyska;
                        db.SaveChanges();
                    }

                    MessageBox.Show("Заявка успешно отправлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow mainWindow = new MainWindow(currentUserId);
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке заявки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool ContainsWhitespace(string input)
        {
            return input.Contains(" ");
        }
        private string GetSrokIspolzovaniyaForPurpose(string purpose)
        {
            switch (purpose)
            {
                case "Доступ к рабочему месту":
                    return "1 год";
                case "Посещение склада":
                    return "6 месяцев";
                case "Посещение производственной зоны":
                    return "1 год";
                case "Временный доступ в здание":
                    return "3 месяца";
                case "Доступ на территорию":
                    return "1 год";
                default:
                    return "1 год"; 
            }
        }
        private string GenerateZayvkaNumber()
        {
            return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }



        private void Otmena(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(currentUserId);
            mainWindow.Show();
            this.Close();
        }

    }
}
