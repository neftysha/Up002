using System;
using System.Linq;
using System.Windows;

namespace up_02
{
    public partial class UserAdsWindow : Window
    {
        private readonly Melnikov_DB_02Entities _db;
        private readonly User _currentUser;
        private readonly Window _parentWindow;

        public UserAdsWindow(User currentUser, Window parentWindow)
        {
            InitializeComponent();

            _db = new Melnikov_DB_02Entities();
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _parentWindow = parentWindow;

            HeaderTextBlock.Text = $"Мои объявления ({_currentUser.Login})";
            LoadUserAds();
        }

        private void LoadUserAds()
        {
            var ads = _db.Ad
                .Where(a => a.UserId == _currentUser.UserId)
                .OrderByDescending(a => a.PublishDate)
                .ToList();

            UserAdsGrid.ItemsSource = ads;
        }

        private Ad GetSelectedAd()
        {
            return UserAdsGrid.SelectedItem as Ad;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new AdEditWindow(_currentUser, null);
            if (wnd.ShowDialog() == true)
            {
                LoadUserAds();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var ad = GetSelectedAd();
            if (ad == null)
            {
                MessageBox.Show(
                    "Выберите объявление для редактирования.",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var wnd = new AdEditWindow(_currentUser, ad.AdId);
            if (wnd.ShowDialog() == true)
            {
                LoadUserAds();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var ad = GetSelectedAd();
            if (ad == null)
            {
                MessageBox.Show(
                    "Выберите объявление для удаления.",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Вы действительно хотите удалить объявление \"{ad.Title}\"?",
                "Удаление объявления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                _db.Ad.Remove(ad);
                _db.SaveChanges();
                LoadUserAds();

                MessageBox.Show(
                    "Объявление удалено.",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при удалении объявления:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void CompletedButton_Click(object sender, RoutedEventArgs e)
        {
            
            var wnd = new CompletedAdsWindow(_currentUser)
            {
                Owner = this
            };
            wnd.ShowDialog();
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _parentWindow.Show();
            Close();
        }
    }
}
