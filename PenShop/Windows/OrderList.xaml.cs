using PenShop.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;

namespace PenShop.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrderList.xaml
    /// </summary>
    public partial class OrderList : Window
    {
        public ICollectionView Orders
        {
            get { return (ICollectionView)GetValue(OrdersProperty); }
            set { SetValue(OrdersProperty, value); }
        }
        public static readonly DependencyProperty OrdersProperty =
            DependencyProperty.Register("Orders", typeof(ICollectionView), typeof(OrderList));

        public decimal AllSum
        {
            get { return (decimal)GetValue(AllSumProperty); }
            set { SetValue(AllSumProperty, value); }
        }

        public static readonly DependencyProperty AllSumProperty =
            DependencyProperty.Register("AllSum", typeof(decimal), typeof(OrderList));

        public OrderList()
        {
            Orders = new CollectionViewSource { Source = App.db.Order.Local.Where(o => o.User == App.User)}.View;

            AllSum = App.db.Order.Local.Where(o => o.User == App.User).Select(x => x.Pen.Price * x.Count).Sum();

            InitializeComponent();

            Sorted(); // метод для сортировки сервисов
            NameDisSearchTb.TextChanged += (s, e) => { Orders.Refresh(); NameSearch(); };
            Filt.SelectionChanged += (s, e) => { Orders.Refresh(); Sorted(); };
            Sort.SelectionChanged += (s, e) => { Orders.Refresh(); Sorted(); };
        }

        #region Поиск
        public void NameSearch()
        {
            Orders.Filter += (obj) =>
            {
                var order = obj as Order;

                var search = NameDisSearchTb.Text;

                if (order.Pen.Name.ToLower().Contains(search) == false)
                    return false;

                return true;
            };
        }
        #endregion

        #region Метод сортировки
        private void Sorted()
        {
            var tag = (Sort.SelectedItem as ComboBoxItem).Tag;

            switch (tag)
            {
                case "Descending":
                    Orders.SortDescriptions.Clear();
                    Orders.SortDescriptions.Add(new SortDescription()
                    {
                        PropertyName = (Filt.SelectedItem as ComboBoxItem).Tag.ToString(),
                        Direction = ListSortDirection.Ascending,
                    });
                    break;

                case "Ascending":
                    Orders.SortDescriptions.Clear();
                    Orders.SortDescriptions.Add(new SortDescription()
                    {
                        PropertyName = (Filt.SelectedItem as ComboBoxItem).Tag.ToString(),
                        Direction = ListSortDirection.Descending,
                    });
                    break;

                case "Any":
                    Orders.SortDescriptions.Clear();
                    break;
            }

        }
        #endregion

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void DeletePenInOrderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PensList.SelectedItem == null)
                return;

            App.db.Order.Local.Remove(App.db.Order.Local.FirstOrDefault(x => x.Pen == (PensList.SelectedItem as Pen)));

            App.db.SaveChanges();

            Orders = new CollectionViewSource { Source = App.db.Order.Local.Where(o => o.User == App.User) }.View;
        }

        private void OpenPensListMouseDown(object sender, MouseButtonEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new Login().Show();
            Close();
        }
    }
}
