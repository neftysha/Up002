using System;
using System.Linq;
using System.Windows;

namespace up_02
{
    public partial class AdEditWindow : Window
    {
        private readonly Melnikov_DB_02Entities _db;
        private readonly User _currentUser;
        private readonly Ad _ad;
        private readonly bool _isNew;
        private int? _oldStatusId;

        public AdEditWindow(User currentUser, int? adId)
        {
            InitializeComponent();

            _db = new Melnikov_DB_02Entities();
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

            // Загружаем справочники
            CityComboBox.ItemsSource = _db.City.OrderBy(c => c.Name).ToList();
            CategoryComboBox.ItemsSource = _db.Category.OrderBy(c => c.Name).ToList();
            TypeComboBox.ItemsSource = _db.AdType.OrderBy(t => t.Name).ToList();
            StatusComboBox.ItemsSource = _db.AdStatus.OrderBy(s => s.Name).ToList();

            if (adId == null)
            {
                _isNew = true;
                HeaderTextBlock.Text = "Добавление объявления";
                _ad = new Ad
                {
                    PublishDate = DateTime.Today,
                    UserId = _currentUser.UserId
                };
                // Статус по умолчанию "Активно", если есть
                var active = _db.AdStatus.FirstOrDefault(s => s.Name == "Активно");
                if (active != null)
                    StatusComboBox.SelectedItem = active;
            }
            else
            {
                _isNew = false;
                HeaderTextBlock.Text = "Редактирование объявления";

                _ad = _db.Ad.First(a => a.AdId == adId.Value);

                TitleTextBox.Text = _ad.Title;
                DescriptionTextBox.Text = _ad.Description;
                PriceTextBox.Text = _ad.Price.ToString("0");

                CityComboBox.SelectedItem = _ad.City;
                CategoryComboBox.SelectedItem = _ad.Category;
                TypeComboBox.SelectedItem = _ad.AdType;
                StatusComboBox.SelectedItem = _ad.AdStatus;

                _oldStatusId = _ad.AdStatusId;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // простая проверка полей
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите название объявления.",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Некорректная цена. Введите неотрицательное число.",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (CityComboBox.SelectedItem == null ||
                CategoryComboBox.SelectedItem == null ||
                TypeComboBox.SelectedItem == null ||
                StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля (город, категория, тип, статус).",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                _ad.Title = TitleTextBox.Text.Trim();
                _ad.Description = DescriptionTextBox.Text.Trim();
                _ad.Price = price;
                _ad.City = (City)CityComboBox.SelectedItem;
                _ad.Category = (Category)CategoryComboBox.SelectedItem;
                _ad.AdType = (AdType)TypeComboBox.SelectedItem;

                var newStatus = (AdStatus)StatusComboBox.SelectedItem;
                _ad.AdStatus = newStatus;

                // проверка смены статуса на "Завершено"
                bool statusChangedToCompleted =
                    newStatus.Name == "Завершено" &&
                    (_oldStatusId == null ||
                     _oldStatusId != newStatus.AdStatusId);

                if (statusChangedToCompleted)
                {
                    ProfitInputWindow profitWindow = new ProfitInputWindow();
                    if (profitWindow.ShowDialog() == true)
                    {
                        int sum = profitWindow.Amount;
                        MessageBox.Show(
                            $"Публикация объявления завершена.\n" +
                            $"Полученная сумма: {sum} ₽",
                            "Информация",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        // пользователь отменил ввод суммы
                        MessageBox.Show(
                            "Завершение объявления отменено.",
                            "Информация",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        return;
                    }
                }

                if (_isNew)
                {
                    _ad.PublishDate = DateTime.Today;
                    _ad.UserId = _currentUser.UserId;
                    _db.Ad.Add(_ad);
                }

                _db.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при сохранении объявления:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
