using ProcurementApi.Application.DTOs;

namespace ProcurementApi.Application.Interfaces
{
    public interface ISupplierService
    {
        Task<List<SupplierDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<SupplierDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<SupplierDto> CreateAsync(CreateSupplierRequest request, string currentUser, CancellationToken cancellationToken = default);
    }
}