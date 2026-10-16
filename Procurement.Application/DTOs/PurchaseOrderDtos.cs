using ProcurementApi.Domain.Enums;

namespace ProcurementApi.Application.DTOs
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int SupplierId { get; set; }

        public string? SupplierName { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public decimal Total { get; set; }

        public List<PurchaseOrderLineDto> Lines { get; set; } = [];

        public DateTime? SubmittedOn { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ReceivedOn { get; set; }

        public DateTime? ClosedOn { get; set; }

        public string? RejectionReason { get; set; }
    }

    public class PurchaseOrderLineDto
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }
    }

    public class CreatePurchaseOrderRequest
    {
        public int SupplierId { get; set; }

        public List<CreatePurchaseOrderLineRequest> Lines { get; set; } = [];
    }

    public class CreatePurchaseOrderLineRequest
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }

    public class RejectPurchaseOrderRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}