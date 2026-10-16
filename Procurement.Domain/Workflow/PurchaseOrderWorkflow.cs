using ProcurementApi.Domain.Enums;

namespace ProcurementApi.Domain.Workflow
{
    /// <summary>The allowed status transition table for a purchase order's lifecycle.</summary>
    public static class PurchaseOrderWorkflow
    {
        private static readonly Dictionary<PurchaseOrderStatus, PurchaseOrderStatus[]> AllowedTransitions = new()
        {
            [PurchaseOrderStatus.Draft] = [PurchaseOrderStatus.Submitted],
            [PurchaseOrderStatus.Submitted] = [PurchaseOrderStatus.Approved, PurchaseOrderStatus.Rejected],
            [PurchaseOrderStatus.Approved] = [PurchaseOrderStatus.Received],
            [PurchaseOrderStatus.Rejected] = [],
            [PurchaseOrderStatus.Received] = [PurchaseOrderStatus.Closed],
            [PurchaseOrderStatus.Closed] = []
        };

        public static bool CanTransition(PurchaseOrderStatus from, PurchaseOrderStatus to)
            => AllowedTransitions.TryGetValue(from, out var next) && next.Contains(to);

        public static void EnsureCanTransition(PurchaseOrderStatus from, PurchaseOrderStatus to)
        {
            if (!CanTransition(from, to))
            {
                throw new InvalidOperationException($"Cannot transition purchase order from '{from}' to '{to}'.");
            }
        }
    }
}