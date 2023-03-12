using PenShop.Properties;
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

namespace PenShop.Windows
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        #region Обработчики

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text.Trim() == "" || PasswordBox.Password.Trim() == "")
                return;

            var user = App.db.User.Local.FirstOrDefault(x => x.Login == LoginBox.Text.Trim() && x.Password == PasswordBox.Password.Trim());

            if (user == null)
            {
                MessageBox.Show("Такой пользователь не зарегистрирован");
                return;
            }

            App.User = user;

            new MainWindow().Show();
            Close();
        }

        private void GoToRegistrationPage_Click(object sender, RoutedEventArgs e)
        {
            new Registration().Show();
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(MessageBox.Show("Вы уверены что хотите выйти из приложения?", "Уведомление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion
    }
}
