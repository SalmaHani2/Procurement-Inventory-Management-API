using FluentValidation;
using ProcurementApi.Application.DTOs;

namespace ProcurementApi.Application.Validators
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.UnitPrice).GreaterThan(0);
            RuleFor(x => x.InitialStock).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ReorderThreshold).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SupplierId).GreaterThan(0);
        }
    }
}