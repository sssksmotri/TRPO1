using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Pr_magazin
{
   
    public partial class MainWindow : Window
    {
       
        private int currentUserId;
        public buro_propyskovEntities2 context;
        public MainWindow(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            context = new buro_propyskovEntities2();

            
            LoadPropyskData();
        }

        private void LoadPropyskData()
        {
            try
            {
                using (var context = new buro_propyskovEntities2())
                {
                    var propyskData = (from p in context.propysk
                                       join z in context.zayvka on p.ID_zayvki equals z.ID_zayvki
                                       where z.ID_client == currentUserId
                                       select new
                                       {
                                           p.Naznachenie,
                                           p.Status,
                                           p.Srok_ispolzovaniya
                                       }).ToList();

                    PropyskDataGrid.ItemsSource = propyskData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(currentUserId);

            mainWindow.Show();
            this.Close();
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Разработчики: Саидов Н.Н., Силюков И.А., гр.4338 ");
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            podderzhka podderzhka = new podderzhka(currentUserId);
            podderzhka.Show();
            this.Close();
        }
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Zayvka_propyska zayvka = new Zayvka_propyska(currentUserId);
            zayvka.Show();
            this.Close();
        }

        private void PropyskDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}

    

