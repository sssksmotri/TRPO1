using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
    public partial class delete_zayvka : Window
    {
        private int currentUserId;
        private buro_propyskovEntities2 db;

        public delete_zayvka(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            db = new buro_propyskovEntities2();

            LoadZayvkaIDs();
        }
        private void LoadZayvkaIDs()
        {
            try
            {
                List<int> zayvkaIds = db.zayvka.Select(z => z.ID_zayvki).ToList();
                ZayvkaIdComBox.ItemsSource = zayvkaIds;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ID заявок: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ZayvkaIdComBox.SelectedItem is int zayvkaId && zayvkaId > 0)
            {
                try
                {
                    var zayvkaToDelete = db.zayvka.FirstOrDefault(z => z.ID_zayvki == zayvkaId);
                    if (zayvkaToDelete != null)
                    {
                        
                        var propyskRecord = db.propysk.FirstOrDefault(p => p.ID_zayvki == zayvkaId);
                        {
                           
                            db.zayvka.Remove(zayvkaToDelete);
                            db.SaveChanges();
                            MessageBox.Show("Заявка успешно удалена.", "Удаление заявки", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadZayvkaIDs();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Заявка с указанным ID не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении заявки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите ID заявки для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            adminprofile adminprofile = new adminprofile(currentUserId);
            adminprofile.Show();
            this.Close();
        }



    }
}