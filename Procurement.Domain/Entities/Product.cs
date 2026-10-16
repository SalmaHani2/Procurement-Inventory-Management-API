using Procurement.Domain.Common;


namespace ProcurementApi.Domain.Entities
{
    public class Product : AuditableEntity
    {
        public string Sku { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int StockOnHand { get; set; }

        public int ReorderThreshold { get; set; }

        public int SupplierId { get; set; }

        public Supplier? Supplier { get; set; }

        public bool NeedsReorder => StockOnHand <= ReorderThreshold;
    }
}