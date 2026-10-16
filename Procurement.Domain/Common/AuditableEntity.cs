using System;
using System.Collections.Generic;
using System.Text;

namespace Procurement.Domain.Common
{
    public abstract class AuditableEntity
    {
        public int Id { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
