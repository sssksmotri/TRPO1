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
    /// <summary>
    /// Логика взаимодействия для podderzhka.xaml
    /// </summary>
    public partial class podderzhka : Window
    {
        private int currentUserId;
        public podderzhka(int userId)
        {
            InitializeComponent();
            currentUserId= userId;
        }

        

        private void Otpravit_zapros(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Успешно,ваш запрос отпрвален");
        }

        private void Otmena(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(currentUserId);
            mainWindow.Show();
            this.Close();
        }
    }
}
