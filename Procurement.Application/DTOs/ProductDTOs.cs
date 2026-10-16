namespace ProcurementApi.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Sku { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int StockOnHand { get; set; }

        public int ReorderThreshold { get; set; }

        public bool NeedsReorder { get; set; }

        public int SupplierId { get; set; }

        public string? SupplierName { get; set; }
    }

    public class CreateProductRequest
    {
        public string Sku { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int InitialStock { get; set; }

        public int ReorderThreshold { get; set; }

        public int SupplierId { get; set; }
    }
}