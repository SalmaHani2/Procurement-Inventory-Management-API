using FluentValidation;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcurementApi.Application.DTOs;
using ProcurementApi.Application.Interfaces;
//using Procurement.Infrastructure.Identity;
using ProcurementApi.Infrastructure.Identity;


namespace ProcurementApi.Api.Controllers
{
    [ApiController]
    [Route("api/purchase-orders")]
    [Authorize]
    public class PurchaseOrdersController(
        IPurchaseOrderService purchaseOrderService,
        IValidator<CreatePurchaseOrderRequest> createValidator,
        IValidator<RejectPurchaseOrderRequest> rejectValidator) : ControllerBase
    {
        private string CurrentUser => User.Identity?.Name ?? "unknown";

        [HttpGet]
        public async Task<ActionResult<List<PurchaseOrderDto>>> GetAll(CancellationToken cancellationToken)
            => Ok(await purchaseOrderService.GetAllAsync(cancellationToken));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PurchaseOrderDto>> GetById(int id, CancellationToken cancellationToken)
            => Ok(await purchaseOrderService.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        public async Task<ActionResult<PurchaseOrderDto>> Create(CreatePurchaseOrderRequest request, CancellationToken cancellationToken)
        {
            await createValidator.ValidateAndThrowAsync(request, cancellationToken);
            var order = await purchaseOrderService.CreateAsync(request, CurrentUser, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpPost("{id:int}/submit")]
        public async Task<ActionResult<PurchaseOrderDto>> Submit(int id, CancellationToken cancellationToken)
            => Ok(await purchaseOrderService.SubmitAsync(id, CurrentUser, cancellationToken));

        [HttpPost("{id:int}/approve")]
        [Authorize(Roles = $"{Infrastructure.Identity.Roles.Admin},{Infrastructure.Identity.Roles.Manager}")]
        public async Task<ActionResult<PurchaseOrderDto>> Approve(int id, CancellationToken cancellationToken)
            => Ok(await purchaseOrderService.ApproveAsync(id, CurrentUser, cancellationToken));

        [HttpPost("{id:int}/reject")]
        [Authorize(Roles = $"{Infrastructure.Identity.Roles.Admin},{Infrastructure.Identity.Roles.Manager}")]
        public async Task<ActionResult<PurchaseOrderDto>> Reject(int id, RejectPurchaseOrderRequest request, CancellationToken cancellationToken)
        {
            await rejectValidator.ValidateAndThrowAsync(request, cancellationToken);
            return Ok(await purchaseOrderService.RejectAsync(id, request.Reason, CurrentUser, cancellationToken));
        }

        [HttpPost("{id:int}/receive")]
        public async Task<ActionResult<PurchaseOrderDto>> Receive(int id, CancellationToken cancellationToken)
            => Ok(await purchaseOrderService.ReceiveAsync(id, CurrentUser, cancellationToken));

        [HttpPost("{id:int}/close")]
        public async Task<ActionResult<PurchaseOrderDto>> Close(int id, CancellationToken cancellationToken)
            => Ok(await purchaseOrderService.CloseAsync(id, CurrentUser, cancellationToken));
    }
}