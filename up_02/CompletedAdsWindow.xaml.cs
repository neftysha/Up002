using System;
using System.Linq;
using System.Windows;

namespace up_02
{
    public partial class CompletedAdsWindow : Window
    {
        private readonly Melnikov_DB_02Entities _db;
        private readonly User _currentUser;

        public CompletedAdsWindow(User currentUser)
        {
            InitializeComponent();

            _db = new Melnikov_DB_02Entities();
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

            LoadCompletedAds();
        }

        // Загрузка всех завершённых объявлений пользователя и подсчёт прибыли
        private void LoadCompletedAds()
        {
            try
            {
                var completedStatus = _db.AdStatus
                    .FirstOrDefault(s => s.Name == "Завершено");


                if (completedStatus == null)
                {
                    MessageBox.Show(
                        "Статус \"Завершено\" не найден в базе данных.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                var ads = _db.Ad
                    .Where(a => a.UserId == _currentUser.UserId &&
                                a.AdStatusId == completedStatus.AdStatusId)
                    .OrderByDescending(a => a.PublishDate)
                    .ToList();

                CompletedAdsGrid.ItemsSource = ads;

                decimal totalProfit = ads.Any() ? ads.Sum(a => a.Price) : 0m;
                TotalProfitTextBlock.Text =
                    $"Общая прибыль: {totalProfit:0.00} ₽";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при загрузке завершённых объявлений:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
