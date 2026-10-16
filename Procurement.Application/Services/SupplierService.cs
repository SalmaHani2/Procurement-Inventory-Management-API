using ProcurementApi.Application.DTOs;
using ProcurementApi.Application.Exceptions;
using ProcurementApi.Application.Interfaces;
using ProcurementApi.Domain.Entities;

namespace ProcurementApi.Application.Services
{
    public class SupplierService(ISupplierRepository repository) : ISupplierService
    {
        public async Task<List<SupplierDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var suppliers = await repository.GetAllAsync(cancellationToken);
            return suppliers.Select(ToDto).ToList();
        }

        public async Task<SupplierDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var supplier = await repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Supplier {id} was not found.");
            return ToDto(supplier);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierRequest request, string currentUser, CancellationToken cancellationToken = default)
        {
            var supplier = new Supplier
            {
                Name = request.Name,
                ContactEmail = request.ContactEmail,
                Phone = request.Phone,
                Address = request.Address,
                IsActive = true,
                CreatedBy = currentUser,
                CreatedOn = DateTime.UtcNow
            };

            await repository.AddAsync(supplier, cancellationToken);
            return ToDto(supplier);
        }

        private static SupplierDto ToDto(Supplier supplier) => new()
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactEmail = supplier.ContactEmail,
            Phone = supplier.Phone,
            Address = supplier.Address,
            IsActive = supplier.IsActive
        };
    }
}