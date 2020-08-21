using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Contracts.ViewModel
{
    public class FBConfig
    {
        public string menuKey { get; set; }
        public string title { get; set; }
        public string pictureUrl { get; set; }
        public string content { get; set; }
        public string hint { get; set; }
        public string Email { get; set; }
    }
    public class Email
    {
        public string Appid { get; set; }
        public string menuCode { get; set; }
        public string EmailCode { get; set; }
    }
}
