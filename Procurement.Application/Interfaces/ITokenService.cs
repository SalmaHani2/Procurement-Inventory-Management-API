namespace ProcurementApi.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string email, string userName, IEnumerable<string> roles);
    }
}