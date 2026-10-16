using ProcurementApi.Application.DTOs;
using ProcurementApi.Application.Exceptions;
using ProcurementApi.Application.Interfaces;
using ProcurementApi.Domain.Entities;

namespace ProcurementApi.Application.Services
{
    public class ProductService(IProductRepository productRepository, ISupplierRepository supplierRepository) : IProductService
    {
        public async Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var products = await productRepository.GetAllAsync(cancellationToken);
            return products.Select(p => ToDto(p, p.Supplier?.Name)).ToList();
        }

        public async Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await productRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Product {id} was not found.");
            return ToDto(product, product.Supplier?.Name);
        }

        public async Task<ProductDto> CreateAsync(CreateProductRequest request, string currentUser, CancellationToken cancellationToken = default)
        {
            var supplier = await supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken)
                ?? throw new NotFoundException($"Supplier {request.SupplierId} was not found.");

            if (await productRepository.SkuExistsAsync(request.Sku, cancellationToken))
            {
                throw new InvalidOperationException($"SKU '{request.Sku}' already exists.");
            }

            var product = new Product
            {
                Sku = request.Sku,
                Name = request.Name,
                UnitPrice = request.UnitPrice,
                StockOnHand = request.InitialStock,
                ReorderThreshold = request.ReorderThreshold,
                SupplierId = supplier.Id,
                CreatedBy = currentUser,
                CreatedOn = DateTime.UtcNow
            };

            await productRepository.AddAsync(product, cancellationToken);
            return ToDto(product, supplier.Name);
        }

        private static ProductDto ToDto(Product product, string? supplierName) => new()
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            UnitPrice = product.UnitPrice,
            StockOnHand = product.StockOnHand,
            ReorderThreshold = product.ReorderThreshold,
            NeedsReorder = product.NeedsReorder,
            SupplierId = product.SupplierId,
            SupplierName = supplierName
        };
    }
}