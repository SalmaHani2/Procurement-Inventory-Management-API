using ProcurementApi.Domain.Entities;

namespace ProcurementApi.Application.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task<List<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default);

        Task AddAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);

        Task AddInventoryTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}