using ProcurementApi.Application.DTOs;
using ProcurementApi.Application.Exceptions;
using ProcurementApi.Application.Interfaces;
using ProcurementApi.Domain.Entities;
using ProcurementApi.Domain.Enums;

namespace ProcurementApi.Application.Services
{
    public class PurchaseOrderService(
        IPurchaseOrderRepository purchaseOrderRepository,
        ISupplierRepository supplierRepository,
        IProductRepository productRepository) : IPurchaseOrderService
    {
        public async Task<List<PurchaseOrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var orders = await purchaseOrderRepository.GetAllAsync(cancellationToken);
            return orders.Select(ToDto).ToList();
        }

        public async Task<PurchaseOrderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => ToDto(await GetOrThrowAsync(id, cancellationToken));

        public async Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderRequest request, string currentUser, CancellationToken cancellationToken = default)
        {
            var supplier = await supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken)
                ?? throw new NotFoundException($"Supplier {request.SupplierId} was not found.");

            var order = new PurchaseOrder
            {
                SupplierId = supplier.Id,
                OrderNumber = await GenerateOrderNumberAsync(cancellationToken),
                Status = PurchaseOrderStatus.Draft,
                CreatedBy = currentUser,
                CreatedOn = DateTime.UtcNow
            };

            foreach (var line in request.Lines)
            {
                var product = await productRepository.GetByIdAsync(line.ProductId, cancellationToken)
                    ?? throw new NotFoundException($"Product {line.ProductId} was not found.");

                order.Lines.Add(new PurchaseOrderLine
                {
                    ProductId = product.Id,
                    Quantity = line.Quantity,
                    UnitPrice = product.UnitPrice
                });
            }

            await purchaseOrderRepository.AddAsync(order, cancellationToken);
            return ToDto(order);
        }

        public async Task<PurchaseOrderDto> SubmitAsync(int id, string currentUser, CancellationToken cancellationToken = default)
        {
            var order = await GetOrThrowAsync(id, cancellationToken);
            order.Submit(DateTime.UtcNow);
            await TouchAndSaveAsync(order, currentUser, cancellationToken);
            return ToDto(order);
        }

        public async Task<PurchaseOrderDto> ApproveAsync(int id, string currentUser, CancellationToken cancellationToken = default)
        {
            var order = await GetOrThrowAsync(id, cancellationToken);
            order.Approve(currentUser, DateTime.UtcNow);
            await TouchAndSaveAsync(order, currentUser, cancellationToken);
            return ToDto(order);
        }

        public async Task<PurchaseOrderDto> RejectAsync(int id, string reason, string currentUser, CancellationToken cancellationToken = default)
        {
            var order = await GetOrThrowAsync(id, cancellationToken);
            order.Reject(reason, DateTime.UtcNow);
            await TouchAndSaveAsync(order, currentUser, cancellationToken);
            return ToDto(order);
        }

        public async Task<PurchaseOrderDto> ReceiveAsync(int id, string currentUser, CancellationToken cancellationToken = default)
        {
            var order = await GetOrThrowAsync(id, cancellationToken);
            order.Receive(DateTime.UtcNow);

            foreach (var line in order.Lines)
            {
                var product = line.Product ?? await productRepository.GetByIdAsync(line.ProductId, cancellationToken)
                    ?? throw new NotFoundException($"Product {line.ProductId} was not found.");

                product.StockOnHand += line.Quantity;

                await purchaseOrderRepository.AddInventoryTransactionAsync(new InventoryTransaction
                {
                    ProductId = product.Id,
                    PurchaseOrderId = order.Id,
                    TransactionType = InventoryTransactionType.Receipt,
                    QuantityChange = line.Quantity,
                    OccurredOn = DateTime.UtcNow,
                    Notes = $"Received against {order.OrderNumber}",
                    CreatedBy = currentUser,
                    CreatedOn = DateTime.UtcNow
                }, cancellationToken);
            }

            await productRepository.SaveChangesAsync(cancellationToken);
            await TouchAndSaveAsync(order, currentUser, cancellationToken);
            return ToDto(order);
        }

        public async Task<PurchaseOrderDto> CloseAsync(int id, string currentUser, CancellationToken cancellationToken = default)
        {
            var order = await GetOrThrowAsync(id, cancellationToken);
            order.Close(DateTime.UtcNow);
            await TouchAndSaveAsync(order, currentUser, cancellationToken);
            return ToDto(order);
        }

        private async Task<PurchaseOrder> GetOrThrowAsync(int id, CancellationToken cancellationToken)
            => await purchaseOrderRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Purchase order {id} was not found.");

        private Task TouchAndSaveAsync(PurchaseOrder order, string currentUser, CancellationToken cancellationToken)
        {
            order.ModifiedBy = currentUser;
            order.ModifiedOn = DateTime.UtcNow;
            return purchaseOrderRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken)
        {
            for (var attempt = 0; attempt < 5; attempt++)
            {
                var candidate = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
                if (!await purchaseOrderRepository.OrderNumberExistsAsync(candidate, cancellationToken))
                {
                    return candidate;
                }
            }

            throw new InvalidOperationException("Could not generate a unique purchase order number.");
        }

        private static PurchaseOrderDto ToDto(PurchaseOrder order) => new()
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            SupplierId = order.SupplierId,
            SupplierName = order.Supplier?.Name,
            Status = order.Status,
            Total = order.Total,
            Lines = order.Lines.Select(line => new PurchaseOrderLineDto
            {
                ProductId = line.ProductId,
                ProductName = line.Product?.Name,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                LineTotal = line.LineTotal
            }).ToList(),
            SubmittedOn = order.SubmittedOn,
            ApprovedOn = order.ApprovedOn,
            ApprovedBy = order.ApprovedBy,
            ReceivedOn = order.ReceivedOn,
            ClosedOn = order.ClosedOn,
            RejectionReason = order.RejectionReason
        };
    }
}