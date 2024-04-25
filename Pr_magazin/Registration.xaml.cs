using System;
using System.Text;
using System.Windows;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Controls;

namespace Pr_magazin
{
    
    public partial class Registration : Window
    {
        
        public Registration()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            avtorizasia avtorizasia = new avtorizasia();
            avtorizasia.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            using (buro_propyskovEntities2 db = new buro_propyskovEntities2())
            {

                if (db.client.Any(u => u.login == UserloginTextBox.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.");
                    return;
                }


                client user = new client()
                {
                    login = UserloginTextBox.Text,
                    password = PasswordBox1.Password  
                };

                db.client.Add(user);
                db.SaveChanges();

                MessageBox.Show("Вы успешно зарегистрировались в системе.");
                avtorizasia avtorizasia = new avtorizasia();
                avtorizasia.Show();
                this.Close();
            }
        }

        private bool ValidateInput()
        {
            StringBuilder errorMessage = new StringBuilder();
            bool hasError = false;

            if (!Regex.IsMatch(UserloginTextBox.Text, @"^[a-zA-Z0-9_]{4,}$"))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректный логин (только буквы и цифры).");
                hasError = true;
            }

            if (PasswordBox1.Password != PasswordBox2.Password)
            {
                errorMessage.AppendLine("Пароли не совпадают.");
                hasError = true;
            }
            if (!Regex.IsMatch(PasswordBox1.Password, @"^(?!.*\s)(?=.*[a-zA-Z])(?=.*[0-9!()-_]).{8,}$"))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректный пароль (минимум 8 символов, хотя бы 1 буква, цифра или символ !()-_).");
                hasError = true;
            }

            if (hasError)
            {
                MessageBox.Show(errorMessage.ToString(), "Ошибка");
                return false;
            }

            return true;
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            avtorizasia avtorizasia = new avtorizasia();
            avtorizasia.Show();
            Close();
        }
    }
}
