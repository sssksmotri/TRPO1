
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;


namespace Pr_magazin
{
    
    public partial class avtorizasia : Window
    {
        public avtorizasia()
        {
            InitializeComponent();
        }
        private int currentUserId;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;
            StringBuilder errorMessage = new StringBuilder();
            if (!Regex.IsMatch(UsersurnameTextBox.Text, @"^[a-zA-Z0-9_]{4,}$"))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректный логин (только буквы и цифры).");
                hasError = true;
            }
            if (!Regex.IsMatch(PasswordBox.Password, @"^(?=.*[a-zA-Z])(?=.*[0-9!()-_]).{8,}$"))
            {
                errorMessage.AppendLine("Пожалуйста, введите корректный пароль (минимум 8 символов, хотя бы 1 буква, цифра или символ !()-_).");
                hasError = true;
            }
            if (hasError)
            {
                MessageBox.Show(errorMessage.ToString());
                return;
            }
            string username1 = UsersurnameTextBox.Text;
            string password1 = PasswordBox.Password; 

           
            using (buro_propyskovEntities2 db = new buro_propyskovEntities2())
            {

                client user = db.client.FirstOrDefault(u => u.login == username1);

                if (user != null)
                {
                    
                    if (user.password == password1)
                    {
                        
                        currentUserId = user.ID_client;

                        if (username1 == "login2")
                        {
                            
                            adminprofile adminProfileWindow = new adminprofile(currentUserId);
                            adminProfileWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            
                            MainWindow mainWindow = new MainWindow(currentUserId);
                            mainWindow.Show();
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверное имя пользователя или пароль.");
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь с таким именем не существует.");
                }
            }
        }


        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Hyperlink_Click_1(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Close();
        }
    }
}
