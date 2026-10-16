using FluentValidation;
using ProcurementApi.Application.DTOs;

namespace ProcurementApi.Application.Validators
{
    public class CreateSupplierRequestValidator : AbstractValidator<CreateSupplierRequest>
    {
        public CreateSupplierRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().MaximumLength(200);
        }
    }
}