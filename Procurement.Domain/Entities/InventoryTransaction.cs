using Procurement.Domain.Common;
using ProcurementApi.Domain.Enums;

namespace ProcurementApi.Domain.Entities
{
    public class InventoryTransaction : AuditableEntity
    {
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int? PurchaseOrderId { get; set; }

        public PurchaseOrder? PurchaseOrder { get; set; }

        public InventoryTransactionType TransactionType { get; set; }

        public int QuantityChange { get; set; }

        public DateTime OccurredOn { get; set; }

        public string? Notes { get; set; }
    }
}