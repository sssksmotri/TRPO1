using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    
    public partial class adminprofile : Window
    {

        public int currentUserId;
        public buro_propyskovEntities2 db = new buro_propyskovEntities2();
        public Canvas tovarCanvas;
        public adminprofile(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            db = new buro_propyskovEntities2();
            LoadData();
            LoadComboBoxData();
        }
        private void LoadComboBoxData()
        {
            var uniqueNaznachenieValues = db.propysk.Select(p => p.Naznachenie).Distinct().ToList();
            naznachenieComboBox.ItemsSource = uniqueNaznachenieValues;
            var uniqueStatusValues = db.propysk.Select(p => p.Status).Distinct().ToList();
            statusComboBox.ItemsSource = uniqueStatusValues;
            var uniqueSrokValues = db.propysk.Select(p => p.Srok_ispolzovaniya).Distinct().ToList();
            SrokComboBox.ItemsSource = uniqueSrokValues;
        }
        private void LoadData()
        {
            var zayvkaData = (from z in db.zayvka
                              join p in db.propysk on z.ID_zayvki equals p.ID_zayvki into propyskGroup
                              from propyskData in propyskGroup.DefaultIfEmpty()  
                              select new
                              {
                                  z.ID_zayvki,
                                  z.ID_client,
                                  z.Lastname,
                                  z.Firstname,
                                  z.Surname,
                                  z.Date_podachi_zayvki,
                                  z.Number_zayvki,
                                  PropyskNaznachenie = propyskData != null ? propyskData.Naznachenie : null,
                                  PropyskStatus = propyskData != null ? propyskData.Status : null,
                                  PropyskSrokIspolzovaniya = propyskData != null ? propyskData.Srok_ispolzovaniya : null
                              }).ToList();

            ZayvkaDataGrid.ItemsSource = zayvkaData;
        }
        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            adminprofile adminProfile = new adminprofile(currentUserId);
            adminProfile.Show();
            this.Close();
        }

        private void sortirovat(object sender, RoutedEventArgs e)
        {
            string selectedNaznachenie = (naznachenieComboBox.SelectedItem as string);
            string selectedStatus = (statusComboBox.SelectedItem as string);
            string selectedSrok = (SrokComboBox.SelectedItem as string);

            var zayvkaData = (from z in db.zayvka
                              join p in db.propysk on z.ID_zayvki equals p.ID_zayvki into propyskGroup
                              from propyskData in propyskGroup.DefaultIfEmpty()
                              select new
                              {
                                  z.ID_zayvki,
                                  z.ID_client,
                                  z.Lastname,
                                  z.Firstname,
                                  z.Surname,
                                  z.Date_podachi_zayvki,
                                  z.Number_zayvki,
                                  ZayvkaNaznachenie = z.Naznachenie,
                                  PropyskNaznachenie = propyskData != null ? propyskData.Naznachenie : null,
                                  PropyskStatus = propyskData != null ? propyskData.Status : null,
                                  PropyskSrokIspolzovaniya = propyskData != null ? propyskData.Srok_ispolzovaniya : null
                              }).ToList();

            if (!string.IsNullOrEmpty(selectedNaznachenie))
            {
                zayvkaData = zayvkaData.Where(z => z.ZayvkaNaznachenie == selectedNaznachenie || z.PropyskNaznachenie == selectedNaznachenie).ToList();
            }
            if (!string.IsNullOrEmpty(selectedStatus))
            {
                zayvkaData = zayvkaData.Where(z => z.PropyskStatus == selectedStatus).ToList();
            }
            if (!string.IsNullOrEmpty(selectedSrok))
            {
                zayvkaData = zayvkaData.Where(z => z.PropyskSrokIspolzovaniya == selectedSrok).ToList();
            }

            zayvkaData = zayvkaData.OrderByDescending(z => z.ID_zayvki).ToList();
            ZayvkaDataGrid.ItemsSource = zayvkaData;
        }

        private void sbros(object sender, RoutedEventArgs e)
        {
            naznachenieComboBox.SelectedIndex = -1;
            statusComboBox.SelectedIndex = -1;
            SrokComboBox.SelectedIndex = -1;
            LoadData();
        }

       

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            dobav_client dobav_Client = new dobav_client(currentUserId);
            dobav_Client.Show();
            this.Close();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            izmenenie izmenenie = new izmenenie(currentUserId);
            izmenenie.Show();
            this.Close();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            delete_zayvka delete_Zayvka = new delete_zayvka(currentUserId);
            delete_Zayvka.Show();
            this.Close();
        }
    }
}