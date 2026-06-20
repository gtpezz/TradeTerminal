namespace TradeTerminal.Desktop
{
    /// <summary>
    /// Модель для отображения товара
    /// </summary>
    public class ProductDisplayItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ManufacturerName { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string Article { get; set; }
        public string Photo { get; set; }

        public bool HasDiscount => Discount > 0;
    }
}