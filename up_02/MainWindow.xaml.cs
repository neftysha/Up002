using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using up_02.Models;
using up_02.Services;

namespace up_02
{
    public partial class MainWindow : Window
    {
        private readonly Melnikov_DB_02Entities _db;
        private readonly User _currentUser;

        // Параметрless конструктор (на всякий случай)
        public MainWindow() : this(null)
        {
        }

        // Основной конструктор – получает авторизованного пользователя
        public MainWindow(User currentUser)
        {
            InitializeComponent();

            _db = new Melnikov_DB_02Entities();
            _currentUser = currentUser;

            // если пользователь не авторизован – кнопку "Мои объявления" отключаем
            if (_currentUser == null)
            {
                MyAdsButton.IsEnabled = false;
                MyAdsButton.ToolTip = "Авторизуйтесь, чтобы работать со своими объявлениями";
            }

            LoadFilters();
            LoadAds();
        }

        private void MyAdsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show(
                    "Для работы со своими объявлениями нужно войти в систему.",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // открываем окно "Мои объявления"
            UserAdsWindow wnd = new UserAdsWindow(_currentUser, this);
            wnd.Show();
            this.Hide();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void LoadFilters()
        {
            var cities = _db.City
                .OrderBy(c => c.Name)
                .ToList();

            CityComboBox.Items.Clear();
            CityComboBox.Items.Add("Все города");

            foreach (var city in cities)
            {
                CityComboBox.Items.Add(city.Name);
            }

            CityComboBox.SelectedIndex = 0;

            var categories = _db.Category
                .OrderBy(c => c.Name)
                .ToList();

            CategoryComboBox.Items.Clear();
            CategoryComboBox.Items.Add("Все категории");

            foreach (var category in categories)
            {
                CategoryComboBox.Items.Add(category.Name);
            }

            CategoryComboBox.SelectedIndex = 0;

            var types = _db.AdType
                .OrderBy(t => t.Name)
                .ToList();

            TypeComboBox.Items.Clear();
            TypeComboBox.Items.Add("Все типы");

            foreach (var type in types)
            {
                TypeComboBox.Items.Add(type.Name);
            }

            TypeComboBox.SelectedIndex = 0;

            var statuses = _db.AdStatus
                .OrderBy(s => s.Name)
                .ToList();

            StatusComboBox.Items.Clear();
            StatusComboBox.Items.Add("Все статусы");

            foreach (var status in statuses)
            {
                StatusComboBox.Items.Add(status.Name);
            }

            StatusComboBox.SelectedIndex = 0;
        }
        

        private void LoadAds()
        {
            string keyword = KeywordTextBox.Text.Trim();

            string selectedCity = CityComboBox.SelectedItem as string;
            string selectedCategory = CategoryComboBox.SelectedItem as string;
            string selectedType = TypeComboBox.SelectedItem as string;
            string selectedStatus = StatusComboBox.SelectedItem as string;

            var query = _db.Ad
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(a =>
                    a.Title.Contains(keyword) ||
                    a.Description.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(selectedCity) &&
                selectedCity != "Все города")
            {
                query = query.Where(a => a.City.Name == selectedCity);
            }

            if (!string.IsNullOrEmpty(selectedCategory) &&
                selectedCategory != "Все категории")
            {
                query = query.Where(a => a.Category.Name == selectedCategory);
            }

            if (!string.IsNullOrEmpty(selectedType) &&
                selectedType != "Все типы")
            {
                query = query.Where(a => a.AdType.Name == selectedType);
            }

            if (!string.IsNullOrEmpty(selectedStatus) &&
                selectedStatus != "Все статусы")
            {
                query = query.Where(a => a.AdStatus.Name == selectedStatus);
            }

            var ads = query
                .ToList()
                .Select(a => new AdViewModel
                {
                    AdId = a.AdId,
                    Title = a.Title,
                    Description = a.Description,
                    PriceText = $"{a.Price:0.00} ₽",
                    CityName = a.City.Name,
                    CategoryName = a.Category.Name,
                    TypeName = a.AdType.Name,
                    StatusName = a.AdStatus.Name,
                    Image = LoadImageOrPlaceholder(a.ImagePath)
                })
                .ToList();

            AdsListView.ItemsSource = ads;
        }

        private BitmapImage LoadImageOrPlaceholder(string imagePath)
        {
            // нам теперь всё равно, что пришло в imagePath – всегда заглушка
            string path = "Resources/Images/placeholder.png";

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(Path.GetFullPath(path));
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadAds();
        }
    }
}
