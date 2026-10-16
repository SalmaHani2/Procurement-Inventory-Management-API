using Microsoft.EntityFrameworkCore;
using ProcurementApi.Application.Interfaces;
using ProcurementApi.Domain.Entities;
using ProcurementApi.Infrastructure.Persistence;

namespace ProcurementApi.Infrastructure.Repositories
{
    public class PurchaseOrderRepository(ProcurementDbContext dbContext) : IPurchaseOrderRepository
    {
        public async Task<List<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default)
            => await dbContext.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.Lines)
                .OrderByDescending(po => po.CreatedOn)
                .ToListAsync(cancellationToken);

        public async Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await dbContext.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.Lines)
                    .ThenInclude(line => line.Product)
                .FirstOrDefaultAsync(po => po.Id == id, cancellationToken);

        public async Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
            => await dbContext.PurchaseOrders.AnyAsync(po => po.OrderNumber == orderNumber, cancellationToken);

        public async Task AddAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
        {
            dbContext.PurchaseOrders.Add(purchaseOrder);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task AddInventoryTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default)
        {
            dbContext.InventoryTransactions.Add(transaction);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => dbContext.SaveChangesAsync(cancellationToken);
    }
}