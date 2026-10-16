using ProcurementApi.Application.DTOs;

namespace ProcurementApi.Application.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ProductDto> CreateAsync(CreateProductRequest request, string currentUser, CancellationToken cancellationToken = default);
    }
}