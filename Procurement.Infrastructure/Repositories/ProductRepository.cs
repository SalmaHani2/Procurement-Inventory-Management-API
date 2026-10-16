using Microsoft.EntityFrameworkCore;
using ProcurementApi.Application.Interfaces;
using ProcurementApi.Domain.Entities;
using ProcurementApi.Infrastructure.Persistence;

namespace ProcurementApi.Infrastructure.Repositories
{
    public class ProductRepository(ProcurementDbContext dbContext) : IProductRepository
    {
        public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
            => await dbContext.Products.Include(p => p.Supplier).OrderBy(p => p.Name).ToListAsync(cancellationToken);

        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await dbContext.Products.Include(p => p.Supplier).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default)
            => await dbContext.Products.AnyAsync(p => p.Sku == sku, cancellationToken);

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => dbContext.SaveChangesAsync(cancellationToken);
    }
}