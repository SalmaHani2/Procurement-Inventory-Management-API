using Procurement.Domain.Common;


namespace ProcurementApi.Domain.Entities
{
    public class Supplier : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}