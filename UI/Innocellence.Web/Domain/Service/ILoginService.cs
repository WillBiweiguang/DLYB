using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.Web.Entity;
using DLYB.Web.Models.Plugins;
namespace DLYB.Web.Service
{
    public interface ILoginService : IDependency
    {
        bool Login(string userName, string password,string uuid = "", string captcha = "123");
        string GetCaptcha(string uuid);
    }
}
