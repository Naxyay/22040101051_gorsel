using System.Linq;
using TacticalSentry.Core.Entities;
using TacticalSentry.Core.Interfaces;
using TacticalSentry.Domain.Rules;
using TacticalSentry.Infrastructure.Data;

namespace TacticalSentry.Infrastructure.Services
{
    public class DbAuthService : IAuthService
    {
        public bool Login(string username, string password)
        {
            using (var context = new TacticalDbContext())
            {
                context.Database.EnsureCreated(); 

                var user = context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

                if (user != null)
                {
                    SessionManager.Instance.StartSession(user);
                    return true;
                }
                return false;
            }
        }

        public void DeleteUser(string username)
        {
            using (var context = new TacticalDbContext())
            {
                context.Database.EnsureCreated();

                // Kullanıcıyı bul
                var userToDelete = context.Users.FirstOrDefault(u => u.Username == username);

                if (userToDelete != null)
                {
                    context.Users.Remove(userToDelete);
                    context.SaveChanges();
                }
            }
        }

        public OperatorUser GetCurrentUser() => SessionManager.Instance.CurrentOperator;

        public void RegisterUser(string username, string password, int level)
        {
            using (var context = new TacticalDbContext())
            {
                context.Users.Add(new OperatorUser
                {
                    Username = username,
                    Password = password,
                    ClearanceLevel = level,
                    ServiceNumber = "OP-" + System.Guid.NewGuid().ToString().Substring(0, 4)
                });
                context.SaveChanges();
            }
        }
    }
}