using Infrastructure.Core;
using Microsoft.AspNet.Identity;
using System.Security.Principal;
using System.Threading.Tasks;
namespace Infrastructure.Web.Domain.Service
{
    public interface IAuthenticationService : IDependency {
         Task SignInAsync(IUser<int> user, bool createPersistentCookie);

         Task SignInNoDB(IUser<int> user, bool createPersistentCookie);
        void SignOut();
        void SetAuthenticatedUserForRequest(IUser<int> user);
        IUser<int> GetAuthenticatedUser(IIdentity Identity);
    }
}
