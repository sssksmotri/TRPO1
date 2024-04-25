using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace Pr_magazin
{
    public partial class dobav_client : Window
    {
        public int currentUserId; 
        public dobav_client(int userId)
        {
            InitializeComponent();
            txtNumberZayvki.Text = GenerateZayvkaNumber();
            currentUserId = userId;
            LoadClientIDs();
        }
        private void LoadClientIDs()
        {
            try
            {
                using (buro_propyskovEntities2 db = new buro_propyskovEntities2())
                {

                    List<client> clients = db.client.ToList();


                    cbClientID.Items.Clear();


                    foreach (var client in clients)
                    {
                        cbClientID.Items.Add(client.ID_client);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ID клиентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PodatZayavku_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errorMessage = new StringBuilder();

            if (string.IsNullOrWhiteSpace(txtFirstname.Text) || !Regex.IsMatch(txtFirstname.Text, @"^[а-яА-Яa-zA-Z]+$") || ContainsWhitespace(txtFirstname.Text))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректное имя без пробелов и специальных символов.");
            }

            if (string.IsNullOrWhiteSpace(txtLastname.Text) || !Regex.IsMatch(txtLastname.Text, @"^[а-яА-Яa-zA-Z]+$") || ContainsWhitespace(txtLastname.Text))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректную фамилию без пробелов и специальных символов.");
            }

            if (string.IsNullOrWhiteSpace(txtSurname.Text) || !Regex.IsMatch(txtSurname.Text, @"^[а-яА-Яa-zA-Z]+$") || ContainsWhitespace(txtSurname.Text))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректное отчество без пробелов и специальных символов.");
            }

            if (cbNaznachenie.SelectedItem == null)
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
                    DateTime datePodachi;
                    if (!DateTime.TryParse(dpDataPodachiZayvki.Text, out datePodachi))
                    {
                        MessageBox.Show("Пожалуйста, введите корректную дату подачи заявки.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string selectedNaznachenie = (cbNaznachenie.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    string srokIspolzovaniya = GetSrokIspolzovaniyaForNaznachenie(selectedNaznachenie);

                    zayvka newZayvka = new zayvka
                    {
                        ID_client = Convert.ToInt32(cbClientID.SelectedItem),
                        Lastname = txtLastname.Text,
                        Firstname = txtFirstname.Text,
                        Surname = txtSurname.Text,
                        Date_podachi_zayvki = datePodachi,
                        Number_zayvki = txtNumberZayvki.Text,
                        Naznachenie = selectedNaznachenie
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

                    MessageBox.Show("Заявка успешно отправлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    adminprofile adminProfile = new adminprofile(currentUserId);
                    adminProfile.Show();
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
        private string GetSrokIspolzovaniyaForNaznachenie(string naznachenie)
        {
            switch (naznachenie)
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
            adminprofile adminprofile = new adminprofile(currentUserId);
            adminprofile.Show();
            this.Close();
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                using (buro_propyskovEntities2 db = new buro_propyskovEntities2())
                {
                    if (db.client.Any(u => u.login == txtLogin.Text))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    client newClient = new client()
                    {
                        login = txtLogin.Text,
                        password = txtPassword.Password
                    };

                    db.client.Add(newClient);
                    db.SaveChanges();

                    MessageBox.Show("Новый клиент успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadClientIDs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            StringBuilder errorMessage = new StringBuilder();
            bool hasError = false;

            if (!Regex.IsMatch(txtLogin.Text, @"^(?!.*\s)[a-zA-Z0-9_]{4,}$"))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректный логин (только буквы и цифры).");
                hasError = true;
            }


            if (!Regex.IsMatch(txtPassword.Password, @"^(?!.*\s)(?=.*[a-zA-Z])(?=.*[0-9!()-_]).{8,}$"))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректный пароль (минимум 8 символов, хотя бы 1 буква, цифра или символ !()-_).");
                hasError = true;
            }

            if (hasError)
            {
                MessageBox.Show(errorMessage.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void Otmena_client(object sender, RoutedEventArgs e)
        {
            adminprofile adminprofile = new adminprofile(currentUserId);
            adminprofile.Show();
            this.Close();
        }

        private void cbClientID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbClientID.SelectedItem != null && cbClientID.SelectedItem is int selectedClientId)
            {
                try
                {
                    using (buro_propyskovEntities2 db = new buro_propyskovEntities2())
                    {
                        var zayvkaForClient = db.zayvka.FirstOrDefault(z => z.ID_client == selectedClientId);
                        if (zayvkaForClient != null)
                        {
                            txtLastname.Text = zayvkaForClient.Lastname;
                            txtFirstname.Text = zayvkaForClient.Firstname;
                            txtSurname.Text = zayvkaForClient.Surname;
                        }
                        else
                        {
                            MessageBox.Show("Заявка не найдена для выбранного клиента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных заявки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
    
}
