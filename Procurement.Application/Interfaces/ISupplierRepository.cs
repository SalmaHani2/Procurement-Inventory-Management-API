using ProcurementApi.Domain.Entities;

namespace ProcurementApi.Application.Interfaces
{
    public interface ISupplierRepository
    {
        Task<List<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);
    }
}