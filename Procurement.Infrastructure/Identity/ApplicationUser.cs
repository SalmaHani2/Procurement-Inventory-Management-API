

using Microsoft.AspNetCore.Identity;

namespace ProcurementApi.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
    }
}