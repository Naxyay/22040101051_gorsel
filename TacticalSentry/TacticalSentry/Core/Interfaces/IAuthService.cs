using TacticalSentry.Core.Entities;

namespace TacticalSentry.Core.Interfaces
{
    public interface IAuthService
    {
        bool Login(string username, string password);
        OperatorUser GetCurrentUser();
    }
}