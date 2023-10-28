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

        private (List<string>, bool) ValidatePassword(string password)
        {
            List<string> mess = new List<string>();

            if (password.Length < 6)
                mess.Add("Пароль должен быть не менее 6 символов");
            if (!password.Any(c => Char.IsUpper(c)))
                mess.Add("В пароле должна быть хотя бы одна прописная буква");
            if (!Regex.IsMatch(password, @"\d"))
                mess.Add("В пароле должна быть хотя бы одна цифра");
            if (!Regex.IsMatch(password, @"[!@#$%^]"))
                mess.Add("В пароле должен быть хотя бы одний из символов ! @ # $ % ^");

            if (mess.Count >= 1)
                return (mess, false);

            return (mess, true);
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

            (List<string> message, bool flag) = ValidatePassword(PasswordBox.Password.Trim());

            if (flag == false)
            {
                MessageBox.Show(string.Join("\n", message), "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new Login().Show();
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion
    }
}
