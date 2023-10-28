using PenShop.Model;
using PenShop.Windows;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PenShop
{
    public partial class MainWindow : Window
    {

        public static MainWindow Instance { get; set; }

        public ICollectionView Pens
        {
            get { return (ICollectionView)GetValue(PensProperty); }
            set { SetValue(PensProperty, value); }
        }
        public static readonly DependencyProperty PensProperty =
            DependencyProperty.Register("Pens", typeof(ICollectionView), typeof(MainWindow));

        public Visibility VisibilityForUser
        {
            get { return (Visibility)GetValue(VisibilityForUserProperty); }
            set { SetValue(VisibilityForUserProperty, value); }
        }

        public static readonly DependencyProperty VisibilityForUserProperty =
            DependencyProperty.Register("VisibilityForUser", typeof(Visibility), typeof(MainWindow));

        public Visibility VisibilityForClient
        {
            get { return (Visibility)GetValue(VisibilityForClientProperty); }
            set { SetValue(VisibilityForClientProperty, value); }
        }

        public static readonly DependencyProperty VisibilityForClientProperty =
            DependencyProperty.Register("VisibilityForClient", typeof(Visibility), typeof(MainWindow));

        public MainWindow()
        {
            Pens = new CollectionViewSource { Source = App.db.Pen.Local }.View;

            if (App.User.Role.Id == 1 || App.User.Role.Id == 2)
            {
                VisibilityForUser = Visibility.Visible;
                VisibilityForClient = Visibility.Collapsed;
            }
            else
            {
                VisibilityForUser = Visibility.Collapsed;
                VisibilityForClient = Visibility.Visible;
            }

            InitializeComponent();

            Instance = this;

            Sorted(); // метод для сортировки сервисов
            NameDisSearchTb.TextChanged += (s, e) => { Pens.Refresh(); NameSearch(); };
            Filt.SelectionChanged += (s, e) => { Pens.Refresh(); Sorted(); };
            Sort.SelectionChanged += (s, e) => { Pens.Refresh(); Sorted(); };
        }

        #region Поиск
        public void NameSearch()
        {
            Pens.Filter += (obj) =>
            {
                var pen = obj as Pen;

                var search = NameDisSearchTb.Text;

                if (pen.Name.ToLower().Contains(search) == false)
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
                    Pens.SortDescriptions.Clear();
                    Pens.SortDescriptions.Add(new SortDescription()
                    {
                        PropertyName = (Filt.SelectedItem as ComboBoxItem).Tag.ToString(),
                        Direction = ListSortDirection.Ascending,
                    });
                    break;

                case "Ascending":
                    Pens.SortDescriptions.Clear();
                    Pens.SortDescriptions.Add(new SortDescription()
                    {
                        PropertyName = (Filt.SelectedItem as ComboBoxItem).Tag.ToString(),
                        Direction = ListSortDirection.Descending,
                    });
                    break;

                case "Any":
                    Pens.SortDescriptions.Clear();
                    break;
            }

        }
        #endregion

        private void AddPenButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new EditAndAddPen().ShowDialog();
        }

        private void EditPenButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PensList.SelectedItem == null)
                return;

            new EditAndAddPen(PensList.SelectedItem as Pen).ShowDialog();
        }

        private void DeletePenButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PensList.SelectedItem == null)
                return;

            switch (MessageBox.Show("Вы дейсвтительно хотите удалить эту запись?", "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Stop))
            {
                case MessageBoxResult.Yes:
                    var deletedPen = PensList.SelectedItem as Pen;

                    App.db.Pen.Remove(deletedPen);
                    App.db.SaveChanges();
                    Pens.Refresh();
                    break;

                case MessageBoxResult.No:
                    return;
            }
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

        private void PensList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (App.User.Role.Id != 3)
                return;

            if (PensList.SelectedItem == null)
                return;

            if (App.db.Order.Local.FirstOrDefault(o => o.User == App.User && o.Pen == (PensList.SelectedItem as Pen)) != null)
            {
                if (MessageBox.Show("Данный товар уже добавлен в список заказов, хотите увеличить количество на +1 шт.?", "Уведомление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    EditCountOrderForUser(App.db.Order.Local.FirstOrDefault(o => o.User == App.User && o.Pen == (PensList.SelectedItem as Pen)));
                
                return;
            }

            Order newOrder = new Order()
            {
                User = App.User,
                Pen = PensList.SelectedItem as Pen,
                Count = 1
            };

            App.db.Order.Local.Add(newOrder);
            App.db.SaveChanges();

            MessageBox.Show("Товар добавлен в заказ");
        }

        private void EditCountOrderForUser(Order order)
        {
            order.Count++;
            App.db.SaveChanges();

            MessageBox.Show("Товар добавлен в заказ");
        }

        private void OpenOrderListMouseDown(object sender, MouseButtonEventArgs e)
        {
            new OrderList().Show();
            Close();
        }
    }
}
