

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innocellence.Web.Domain.Models
{
    public class ApiResultModel
    {
        public string code { get; set; }
        public object data { get; set; }
        public string msg { get; set; }
    }
}