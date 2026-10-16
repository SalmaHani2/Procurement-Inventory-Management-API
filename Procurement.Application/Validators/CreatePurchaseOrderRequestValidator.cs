using FluentValidation;
using ProcurementApi.Application.DTOs;

namespace ProcurementApi.Application.Validators
{
    public class CreatePurchaseOrderRequestValidator : AbstractValidator<CreatePurchaseOrderRequest>
    {
        public CreatePurchaseOrderRequestValidator()
        {
            RuleFor(x => x.SupplierId).GreaterThan(0);
            RuleFor(x => x.Lines).NotEmpty().WithMessage("A purchase order needs at least one line item.");
            RuleForEach(x => x.Lines).ChildRules(line =>
            {
                line.RuleFor(l => l.ProductId).GreaterThan(0);
                line.RuleFor(l => l.Quantity).GreaterThan(0);
            });
        }
    }

    public class RejectPurchaseOrderRequestValidator : AbstractValidator<RejectPurchaseOrderRequest>
    {
        public RejectPurchaseOrderRequestValidator()
        {
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        }
    }
}