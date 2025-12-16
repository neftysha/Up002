using System;
using System.Linq;
using System.Windows;

namespace up_02
{
    public partial class LoginWindow : Window
    {
        private readonly Melnikov_DB_02Entities _db;

        public LoginWindow()
        {
            InitializeComponent();
            _db = new Melnikov_DB_02Entities();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(
                    "Введите логин и пароль.",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                
                var user = _db.User
                    .FirstOrDefault(u => u.Login == login && u.PasswordHash == password);

                if (user == null)
                {
                    MessageBox.Show(
                        "Неверный логин или пароль.",
                        "Ошибка авторизации",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                
                MainWindow main = new MainWindow(user);
                main.Show();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при обращении к базе данных:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
