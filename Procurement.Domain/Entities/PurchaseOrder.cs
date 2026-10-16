using Procurement.Domain.Common;
using ProcurementApi.Domain.Enums;
using ProcurementApi.Domain.Workflow;

namespace ProcurementApi.Domain.Entities
{
    public class PurchaseOrder : AuditableEntity
    {
        public string OrderNumber { get; set; } = string.Empty;

        public int SupplierId { get; set; }

        public Supplier? Supplier { get; set; }

        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;

        public ICollection<PurchaseOrderLine> Lines { get; set; } = new List<PurchaseOrderLine>();

        public DateTime? SubmittedOn { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ReceivedOn { get; set; }

        public DateTime? ClosedOn { get; set; }

        public string? RejectionReason { get; set; }

        public decimal Total => Lines.Sum(line => line.LineTotal);

        public void Submit(DateTime occurredOn)
        {
            if (Lines.Count == 0)
            {
                throw new InvalidOperationException("Cannot submit a purchase order with no line items.");
            }

            PurchaseOrderWorkflow.EnsureCanTransition(Status, PurchaseOrderStatus.Submitted);
            Status = PurchaseOrderStatus.Submitted;
            SubmittedOn = occurredOn;
        }

        public void Approve(string approvedBy, DateTime occurredOn)
        {
            PurchaseOrderWorkflow.EnsureCanTransition(Status, PurchaseOrderStatus.Approved);
            Status = PurchaseOrderStatus.Approved;
            ApprovedBy = approvedBy;
            ApprovedOn = occurredOn;
        }

        public void Reject(string reason, DateTime occurredOn)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException("A rejection reason is required.", nameof(reason));
            }

            PurchaseOrderWorkflow.EnsureCanTransition(Status, PurchaseOrderStatus.Rejected);
            Status = PurchaseOrderStatus.Rejected;
            RejectionReason = reason;
            ModifiedOn = occurredOn;
        }

        public void Receive(DateTime occurredOn)
        {
            PurchaseOrderWorkflow.EnsureCanTransition(Status, PurchaseOrderStatus.Received);
            Status = PurchaseOrderStatus.Received;
            ReceivedOn = occurredOn;
        }

        public void Close(DateTime occurredOn)
        {
            PurchaseOrderWorkflow.EnsureCanTransition(Status, PurchaseOrderStatus.Closed);
            Status = PurchaseOrderStatus.Closed;
            ClosedOn = occurredOn;
        }
    }
}