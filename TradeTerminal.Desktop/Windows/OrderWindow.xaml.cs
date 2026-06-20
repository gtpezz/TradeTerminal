using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace TradeTerminal.Desktop.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        private readonly ApiService _api = new();
        private readonly OrderState _order = OrderState.Instance;
        private JsonElement _orderData;
        private List<OrderItem> _items = new();

        public class OrderItem
        {
            public string? ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal Total { get; set; }
        }

        public OrderWindow(JsonElement order)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            _orderData = order;
            LoadOrder();
        }

        private void LoadOrder()
        {
            try
            {
                var orderNumber = _orderData.GetProperty("orderNumber").GetInt32();
                orderNumberLabel.Text = orderNumber.ToString();

                var userFullName = _orderData.TryGetProperty("userFullName", out var userProp)
                    ? userProp.GetString()
                    : null;
                clientLabel.Text = string.IsNullOrEmpty(userFullName) ? "Гость" : userFullName;

                var pickupCode = _orderData.GetProperty("pickupCode").GetString();
                pickupCodeLabel.Text = pickupCode;

                _items.Clear();
                decimal total = 0;

                if (_orderData.TryGetProperty("items", out var itemsProp))
                {
                    foreach (var item in itemsProp.EnumerateArray())
                    {
                        var productName = item.TryGetProperty("productName", out var nameProp)
                            ? nameProp.GetString()
                            : "Неизвестный товар";
                        var quantity = item.GetProperty("quantity").GetInt32();
                        var price = item.GetProperty("priceAtOrder").GetDecimal();
                        var itemTotal = item.GetProperty("total").GetDecimal();

                        _items.Add(new OrderItem
                        {
                            ProductName = productName,
                            Quantity = quantity,
                            Price = price,
                            Total = itemTotal
                        });
                        total += itemTotal;
                    }
                }

                itemsListView.ItemsSource = _items;
                totalLabel.Text = total.ToString("C");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveTicketButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt",
                    DefaultExt = "txt",
                    FileName = $"Талон_заказа_{orderNumberLabel.Text}_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var ticketContent = new StringBuilder();
                    ticketContent.AppendLine("=".PadRight(50, '='));
                    ticketContent.AppendLine($"          ТАЛОН ЗАКАЗА №{orderNumberLabel.Text}");
                    ticketContent.AppendLine("=".PadRight(50, '='));
                    ticketContent.AppendLine();
                    ticketContent.AppendLine($"Дата заказа: {DateTime.Now:dd.MM.yyyy HH:mm}");
                    ticketContent.AppendLine($"Клиент: {clientLabel.Text}");
                    ticketContent.AppendLine($"Код получения: {pickupCodeLabel.Text}");
                    ticketContent.AppendLine();
                    ticketContent.AppendLine("-".PadRight(50, '-'));
                    ticketContent.AppendLine("Товары:");

                    foreach (OrderItem item in itemsListView.Items)
                    {
                        ticketContent.AppendLine($"  {item.ProductName}");
                        ticketContent.AppendLine($"    {item.Quantity} шт. x {item.Price:C} = {item.Total:C}");
                    }

                    ticketContent.AppendLine("-".PadRight(50, '-'));
                    ticketContent.AppendLine($"ИТОГО: {totalLabel.Text}");
                    ticketContent.AppendLine("=".PadRight(50, '='));
                    ticketContent.AppendLine();
                    ticketContent.AppendLine("Спасибо за покупку!");

                    File.WriteAllText(saveFileDialog.FileName, ticketContent.ToString(), Encoding.UTF8);

                    MessageBox.Show("Талон успешно сохранен!", "Успешно",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Вы уверены, что хотите оформить заказ?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show($"Заказ №{orderNumberLabel.Text} успешно оформлен!\nКод получения: {pickupCodeLabel.Text}",
                        "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                    _order.ClearOrder();
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка оформления заказа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Очистить корзину
        /// </summary>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите очистить корзину?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _order.ClearOrder();
                DialogResult = true;
                Close();
            }
        }
    }
}
