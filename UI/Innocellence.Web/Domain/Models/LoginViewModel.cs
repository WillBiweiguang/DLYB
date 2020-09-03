using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.Web.Domain.Models
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Captcha { get; set; }
        public string Uuid { get; set; }
    }
}