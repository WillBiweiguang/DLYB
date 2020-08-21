using System;
using Infrastructure.Core.Configuration;
using Infrastructure.Core.Infrastructure.DependencyManagement;
using Autofac.Core;
using Owin;
using Infrastructure.Core;

namespace Infrastructure.Web.MVC
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the 
    /// various services composing the Nop engine. Edit functionality, modules
    /// and implementations access most Nop functionality through this 
    /// interface.
    /// </summary>
    public interface IOWinAuthorizeProvider:IDependency
    {
        void ConfigureAuth(IAppBuilder config);
       
    }
}
