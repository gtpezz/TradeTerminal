using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using TradeTerminal.Desktop.Windows;

namespace TradeTerminal.Desktop
{
    public partial class MainWindow : Window
    {
        private const string arrowUp = "\u25B2";
        private const string arrowDown = "\u25BC";
        private readonly ApiService _api = new();
        private readonly OrderState _order = OrderState.Instance;
        private JsonElement _allProducts;
        private List<JsonElement> _manufacturers = new();
        private List<JsonElement> _categories = new();
        private string _sortBy = "name";
        private bool _asc = true;

        public MainWindow()
        {
            InitializeComponent();
            _order.OrderChanged += (s, e) => Dispatcher.Invoke(UpdateOrderUI);
        }

        #region Loading

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
            UpdateUserUI();
            UpdateOrderUI();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var products = await _api.GetProductsAsync();
                var manufacturers = await _api.GetManufacturersAsync();
                var categories = await _api.GetCategoriesAsync();

                _allProducts = products;
                _manufacturers = manufacturers.EnumerateArray().ToList();
                _categories = categories.EnumerateArray().ToList();

                cmbManufacturer.Items.Clear();
                cmbManufacturer.Items.Add("Все производители");
                foreach (var m in _manufacturers)
                    cmbManufacturer.Items.Add(m.GetProperty("name").GetString());

                cmbCategory.Items.Clear();
                cmbCategory.Items.Add("Все категории");
                foreach (var c in _categories)
                    cmbCategory.Items.Add(c.GetProperty("name").GetString());

                cmbManufacturer.SelectedIndex = 0;
                cmbCategory.SelectedIndex = 0;

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Фильтры и сортировка(Filters and Sorting)

        private void ApplyFilters()
        {
            var list = _allProducts.EnumerateArray().ToList();

            if (cmbManufacturer.SelectedIndex > 0)
            {
                var name = cmbManufacturer.SelectedItem?.ToString();
                list = list.Where(p => p.GetProperty("manufacturerName").GetString() == name).ToList();
            }

            if (cmbCategory.SelectedIndex > 0)
            {
                var name = cmbCategory.SelectedItem?.ToString();
                list = list.Where(p => p.GetProperty("categoryName").GetString() == name).ToList();
            }

            if (decimal.TryParse(txtMinPrice.Text, out var min))
                list = list.Where(p => p.GetProperty("price").GetDecimal() >= min).ToList();

            if (decimal.TryParse(txtMaxPrice.Text, out var max))
                list = list.Where(p => p.GetProperty("price").GetDecimal() <= max).ToList();

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var term = txtSearch.Text.ToLower();
                list = list.Where(p => p.GetProperty("name").GetString()?.ToLower().Contains(term) == true).ToList();
            }

            if (_sortBy == "name")
                list = _asc ? list.OrderBy(p => p.GetProperty("name").GetString()).ToList()
                            : list.OrderByDescending(p => p.GetProperty("name").GetString()).ToList();
            else
                list = _asc ? list.OrderBy(p => p.GetProperty("price").GetDecimal()).ToList()
                            : list.OrderByDescending(p => p.GetProperty("price").GetDecimal()).ToList();

            var displayList = new List<ProductDisplayItem>();
            foreach (var item in list)
            {
                var photo = item.TryGetProperty("photo", out var photoProp) ? photoProp.GetString() : null;

                displayList.Add(new ProductDisplayItem
                {
                    Id = item.GetProperty("id").GetInt32(),
                    Name = item.GetProperty("name").GetString()!,
                    ManufacturerName = item.GetProperty("manufacturerName").GetString()!,
                    Price = item.GetProperty("price").GetDecimal(),
                    Discount = item.GetProperty("discount").GetDecimal(),
                    Article = item.GetProperty("article").GetString()!,
                    Photo = photo
                });
            }

            itemsControl.ItemsSource = displayList;
            lblCount.Text = $"{list.Count} из {_allProducts.GetArrayLength()}";
        }

        #endregion

        #region Обновления пользовательского интерфейса(UI Updates)

        private void UpdateUserUI()
        {
            var settings = Properties.Settings.Default;

            if (settings.IsAuthenticated)
            {
                userLabel.Text = $"\uD83D\uDC64 {settings.CurrentUserFullName}";
                loginButton.Content = "Выйти";
                var role = settings.CurrentUserRole;
                manageOrdersButton.Visibility = (role == "Администратор" || role == "Менеджер")
                    ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                userLabel.Text = "\uD83D\uDC64 Гость";
                loginButton.Content = "Войти";
                manageOrdersButton.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateOrderUI()
        {
            if (_order.HasItems)
            {
                buttonViewOrder.Visibility = Visibility.Visible;
                buttonViewOrder.Content = $"Заказ №{_order.OrderNumber} ({_order.ItemsCount} шт.) - {_order.TotalAmount:C}";
            }
            else
            {
                buttonViewOrder.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Обработчики событий(Event Handlers)

        private async void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var productId = (int)button.Tag;

            var product = _allProducts.EnumerateArray()
                .FirstOrDefault(p => p.GetProperty("id").GetInt32() == productId);
            var name = product.GetProperty("name").GetString();

            try
            {
                int orderId;
                if (!_order.HasItems)
                {
                    var userId = Properties.Settings.Default.IsAuthenticated
                        ? Properties.Settings.Default.CurrentUserId
                        : (int?)null;
                    orderId = await _api.CreateOrderAsync(userId);
                    var order = await _api.GetOrderByIdAsync(orderId);
                    _order.SetOrder(order);
                }

                orderId = _order.OrderId;
                var updated = await _api.AddItemToOrderAsync(orderId, productId);
                _order.SetOrder(updated);

                MessageBox.Show($"Товар \"{name}\" добавлен в заказ!", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.IsAuthenticated)
            {
                Properties.Settings.Default.IsAuthenticated = false;
                Properties.Settings.Default.CurrentUserId = 0;
                Properties.Settings.Default.CurrentUserFullName = "";
                Properties.Settings.Default.CurrentUserRole = "";
                Properties.Settings.Default.Save();
                _order.ClearOrder();
                UpdateUserUI();
                UpdateOrderUI();
            }
            else
            {
                var login = new LoginWindow();
                if (login.ShowDialog() == true)
                {
                    UpdateUserUI();
                }
            }
        }

        private void ButtonViewOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_order.CurrentOrder.HasValue)
            {
                var orderWindow = new OrderWindow(_order.CurrentOrder.Value);
                orderWindow.Closed += (s, args) => UpdateOrderUI();
                orderWindow.ShowDialog();
            }
        }

        private void ManageOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new OrderManagementWindow();
            window.ShowDialog();
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void SortNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sortBy == "name") _asc = !_asc;
            else { _sortBy = "name"; _asc = true; }
            UpdateSortButtons();
            ApplyFilters();
        }

        private void SortPriceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sortBy == "price") _asc = !_asc;
            else { _sortBy = "price"; _asc = true; }
            UpdateSortButtons();
            ApplyFilters();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button) button.IsEnabled = false;
            await LoadDataAsync();
            if (sender is Button enabledButton) enabledButton.IsEnabled = true;
        }

        private void UpdateSortButtons()
        {
            sortNameButton.Content = $"По имени {(_sortBy == "name" ? (_asc ? arrowUp : arrowDown) : "")}";
            sortPriceButton.Content = $"По цене {(_sortBy == "price" ? (_asc ? arrowUp : arrowDown) : "")}";

            sortNameButton.Background = _sortBy == "name"
                ? (System.Windows.Media.Brush)FindResource("AccentBrush")
                : System.Windows.Media.Brushes.LightGray;
            sortNameButton.Foreground = _sortBy == "name"
                ? System.Windows.Media.Brushes.White
                : System.Windows.Media.Brushes.Black;

            sortPriceButton.Background = _sortBy == "price"
                ? (System.Windows.Media.Brush)FindResource("AccentBrush")
                : System.Windows.Media.Brushes.LightGray;
            sortPriceButton.Foreground = _sortBy == "price"
                ? System.Windows.Media.Brushes.White
                : System.Windows.Media.Brushes.Black;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _order.OrderChanged -= (s, e) => Dispatcher.Invoke(UpdateOrderUI);
        }

        #endregion
    }
}