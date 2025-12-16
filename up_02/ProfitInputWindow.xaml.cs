using System;
using System.Windows;

namespace up_02
{
    public partial class ProfitInputWindow : Window
    {
        public int Amount { get; private set; }

        public ProfitInputWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(AmountTextBox.Text, out int value) || value < 0)
            {
                MessageBox.Show(
                    "Введите целое неотрицательное число.",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            Amount = value;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
