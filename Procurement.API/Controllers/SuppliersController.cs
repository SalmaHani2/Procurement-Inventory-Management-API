using FluentValidation;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcurementApi.Application.DTOs;
using ProcurementApi.Application.Interfaces;

namespace ProcurementApi.Api.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    [Authorize]
    public class SuppliersController(ISupplierService supplierService, IValidator<CreateSupplierRequest> validator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<SupplierDto>>> GetAll(CancellationToken cancellationToken)
            => Ok(await supplierService.GetAllAsync(cancellationToken));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SupplierDto>> GetById(int id, CancellationToken cancellationToken)
            => Ok(await supplierService.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        [Authorize(Roles = $"{Infrastructure.Identity.Roles.Admin} , {Infrastructure.Identity.Roles.Manager}")]
        public async Task<ActionResult<SupplierDto>> Create(CreateSupplierRequest request, CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var supplier = await supplierService.CreateAsync(request, User.Identity?.Name ?? "unknown", cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
        }
    }
}