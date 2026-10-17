using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcurementApi.Application.DTOs;
using ProcurementApi.Application.Interfaces;
using ProcurementApi.Infrastructure.Identity;


namespace ProcurementApi.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize]
    public class ProductsController(IProductService productService, IValidator<CreateProductRequest> validator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetAll(CancellationToken cancellationToken)
            => Ok(await productService.GetAllAsync(cancellationToken));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
            => Ok(await productService.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
        public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request, CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
            var product = await productService.CreateAsync(request, User.Identity?.Name ?? "unknown", cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
    }
}