using PenShop.Model;
using System;
using System.Collections.Generic;
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

namespace PenShop.Windows
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration() => InitializeComponent();

        #region Методы

        private (string, bool) ValidatePassword(string password)
        {
            if (password.Length < 6)
                return ("Пароль должен быть не менее 6 символов", false);
            if (!password.Any(c => Char.IsUpper(c)))
                return ("В пароле должна быть хотя бы одна прописная буква", false);
            if (!Regex.IsMatch(password, @"\d"))
                return ("В пароле должна быть хотя бы одна цифра", false);
            if (!Regex.IsMatch(password, @"[!@#$%^]"))
                return ("В пароле должен быть хотя бы одний из символов ! @ # $ % ^", false);
            return ("", true);
        }

        private User newUser() =>
            new User()
            {
                Name = NameBox.Text.Trim(),
                Login = LoginBox.Text.Trim(),
                Password = PasswordBox.Password.Trim(),
                Phone = PhoneBox.Text.Trim(),
                RoleId = 0
            };

        private bool ValidateChangesData() =>
                NameBox.Text.Trim() == "" ||
                LastnameBox.Text.Trim() == "" ||
                PhynimicBox.Text.Trim() == "" ||
                PhoneBox.Text.Trim() == "" ||
                LoginBox.Text.Trim() == "" ||
                PasswordBox.Password.Trim() == "";
        #endregion

        #region Обработчики
        private void ButtonRegistClick(object sender, RoutedEventArgs e)
        {
            if (ValidateChangesData())
            {
                MessageBox.Show("Поля не должны оставаться пустыми", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            User user = App.db.User.Local.FirstOrDefault(x => x.Login == LoginBox.Text.Trim() ||
                                                         x.Phone == PhoneBox.Text.Trim());

            if (user != null)
            {
                MessageBox.Show("Такой пользователь уже есть", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            (string message, bool flag) = ValidatePassword(PasswordBox.Password.Trim());

            if (flag == false)
            {
                MessageBox.Show(message, "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                User userNew = newUser();

                App.db.User.Local.Add(userNew);

                App.User = userNew;

                App.db.SaveChanges();
            }
            catch (Exception)
            {
                MessageBox.Show("Что то случилось, пожалуйста, проверьте введенный данные");
                return;
            }

            new MainWindow().Show();
            Close();
        }


        private void GoToLoginPagePage_Click(object sender, RoutedEventArgs e)
        {
            new Login().Show();
            Hide();
        }
        #endregion
    }
}
