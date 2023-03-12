using PenShop.Model;
using PenShop.Windows;
using System.ComponentModel;
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

        public MainWindow()
        {
            Pens = new CollectionViewSource { Source = App.db.Pen.Local }.View;

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
    }
}
