using NorthWindRestApi.Models;

namespace NorthWindRestApi.Services
{
    public interface IAuthenticateService
    {

        LoggedUser? Authenticate(string username, string password);
    }
}
