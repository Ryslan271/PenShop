using Microsoft.Win32;
using PenShop.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PenShop.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditAndAddPen.xaml
    /// </summary>
    public partial class EditAndAddPen : Window
    {
        Pen Pen;

        #region Dependency Properties

        public Pen PenEdit
        {
            get { return (Pen)GetValue(PenEditProperty); }
            set { SetValue(PenEditProperty, value); }
        }
        public static readonly DependencyProperty PenEditProperty =
            DependencyProperty.Register("PenEdit", typeof(Pen), typeof(EditAndAddPen));

        public IEnumerable<PenColor> PenColors
        {
            get { return (IEnumerable<PenColor>)GetValue(PenColorsProperty); }
            set { SetValue(PenColorsProperty, value); }
        }
        public static readonly DependencyProperty PenColorsProperty =
            DependencyProperty.Register("PenColors", typeof(IEnumerable<PenColor>), typeof(EditAndAddPen));

        public IEnumerable<PenType> PenTypes
        {
            get { return (IEnumerable<PenType>)GetValue(PenTypesProperty); }
            set { SetValue(PenTypesProperty, value); }
        }
        public static readonly DependencyProperty PenTypesProperty =
            DependencyProperty.Register("PenTypes", typeof(IEnumerable<PenType>), typeof(EditAndAddPen));

        public IEnumerable<PenView> PenViews
        {
            get { return (IEnumerable<PenView>)GetValue(PenViewsProperty); }
            set { SetValue(PenViewsProperty, value); }
        }
        public static readonly DependencyProperty PenViewsProperty =
            DependencyProperty.Register("PenViews", typeof(IEnumerable<PenView>), typeof(EditAndAddPen));

        #endregion

        public byte[] Photo
        {
            get => PenEdit.Image;
            set
            {
                PenEdit.Image = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Image"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EditAndAddPen(Pen pen = null)
        {
            Pen = pen;

            PenEdit = pen ?? new Pen();

            PenColors = App.db.PenColor.Local;
            PenViews = App.db.PenView.Local;
            PenTypes = App.db.PenType.Local;

            InitializeComponent();
        }

        #region Изменение изображение

        private void EditImageProduct(object sender, RoutedEventArgs e) => ChangeImage();

        private void ChangeImage()
        {
            string filePath = OpenImage();

            if (filePath == null)
                return;

            byte[] photo = File.ReadAllBytes(filePath);

            Photo = photo;
        }

        private string OpenImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Изображения|*.png;*.jpg",
                DefaultExt = "Изображения|*.png;*.jpg",
                CheckFileExists = true,
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;
            return null;
        }
        #endregion

        #region Закрытие приложение
        private void Root_Closing(object sender, CancelEventArgs e)
        {
            if (App.db.ChangeTracker.HasChanges() == false)
                return;

            switch (AskClose())
            {
                case MessageBoxResult.Yes:
                    MainWindow.Instance.Pens.Refresh();
                    break;

                case MessageBoxResult.No:
                    foreach (var entry in App.db.ChangeTracker.Entries().Where(entry => entry.State == System.Data.Entity.EntityState.Modified))
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                    MainWindow.Instance.Pens.Refresh();
                    e.Cancel = true;
                    break;
            }
        }
        #endregion

        #region Кнопка сохранения
        private void SaveChagesInProduct(object sender, RoutedEventArgs e)
        {
            if (ValidatorProduct() == true)
            {
                MessageBox.Show("Поля не должны оставаться пустыми");
                return;
            }

            switch (Ask())
            {
                case MessageBoxResult.Yes:

                    if (Pen == null)
                        App.db.Pen.Add(PenEdit);

                    App.db.SaveChanges();

                    MainWindow.Instance.Pens.Refresh();

                    Close();
                    break;

                case MessageBoxResult.No:
                    foreach (var entry in App.db.ChangeTracker.Entries().Where(entry => entry.State == System.Data.Entity.EntityState.Modified))
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                    MainWindow.Instance.Pens.Refresh();
                    break;
            }
        }
        #endregion

        #region Проверка на заполненность полей
        private bool ValidatorProduct() => PenEditName.Text.Trim() == "" ||
                                           Price.Text.Trim() == "0";

        #endregion

        private MessageBoxResult Ask() => MessageBox.Show("Вы действительно хотите сохранить эти маленькие данные",
                                                           "Уведомление",
                                                           MessageBoxButton.YesNo,
                                                           MessageBoxImage.Warning);

        private MessageBoxResult AskClose() => MessageBox.Show("Вы действительно хотите выйти, данные не сохранятся",
                                               "Уведомление",
                                               MessageBoxButton.YesNo,
                                               MessageBoxImage.Warning);

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
