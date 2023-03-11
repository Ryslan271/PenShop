using PenShop.Model;
using System.Data.Entity;
using System.Windows;

namespace PenShop
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>


    public partial class App : Application
    {
        public static PenShopDBEntities db = new PenShopDBEntities();

        public static User User { get; set; }

        public App()
        {
            db.User.Load();
            db.Pen.Load();
        }
    }
}
