using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public partial class izmenenie : Window
    {
        private int currentUserId;
        private buro_propyskovEntities2 db;
        public izmenenie(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            db = new buro_propyskovEntities2();
            LoadZayvkaIDs();
        }
        
            
        private void Otmena(object sender, RoutedEventArgs e)
        {
            adminprofile adminprofile = new adminprofile(currentUserId);
            adminprofile.Show();
            this.Close();
        }

        private void LoadZayvkaIDs()
        {
            try
            {

                List<int> zayvkaIds = db.zayvka.Select(z => z.ID_zayvki).ToList();
                IdtxtBox.ItemsSource = zayvkaIds;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ID заявок: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Otpravit(object sender, RoutedEventArgs e)
        {
            if (IdtxtBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите ID заявки.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                int selectedZayvkaId = (int)IdtxtBox.SelectedItem;
                string selectedStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

                if (string.IsNullOrEmpty(selectedStatus))
                {
                    MessageBox.Show("Пожалуйста, выберите новый статус пропуска.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                propysk propyskToUpdate = db.propysk.FirstOrDefault(p => p.ID_zayvki == selectedZayvkaId);
                if (propyskToUpdate != null)
                {
                    propyskToUpdate.Status = selectedStatus;
                    db.SaveChanges();
                    MessageBox.Show("Статус пропуска успешно обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    adminprofile adminprofile = new adminprofile(currentUserId);
                    adminprofile.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Пропуск с ID заявки {selectedZayvkaId} не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении статуса пропуска: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

}
