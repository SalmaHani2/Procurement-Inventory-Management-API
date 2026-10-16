using Microsoft.EntityFrameworkCore;
using ProcurementApi.Application.Interfaces;
using ProcurementApi.Domain.Entities;
using ProcurementApi.Infrastructure.Persistence;

namespace ProcurementApi.Infrastructure.Repositories
{
    public class SupplierRepository(ProcurementDbContext dbContext) : ISupplierRepository
    {
        public async Task<List<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
            => await dbContext.Suppliers.OrderBy(s => s.Name).ToListAsync(cancellationToken);

        public async Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        public async Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            dbContext.Suppliers.Add(supplier);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}