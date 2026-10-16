using ProcurementApi.Application.DTOs;

namespace ProcurementApi.Application.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrderDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PurchaseOrderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderRequest request, string currentUser, CancellationToken cancellationToken = default);

        Task<PurchaseOrderDto> SubmitAsync(int id, string currentUser, CancellationToken cancellationToken = default);

        Task<PurchaseOrderDto> ApproveAsync(int id, string currentUser, CancellationToken cancellationToken = default);

        Task<PurchaseOrderDto> RejectAsync(int id, string reason, string currentUser, CancellationToken cancellationToken = default);

        Task<PurchaseOrderDto> ReceiveAsync(int id, string currentUser, CancellationToken cancellationToken = default);

        Task<PurchaseOrderDto> CloseAsync(int id, string currentUser, CancellationToken cancellationToken = default);
    }
}