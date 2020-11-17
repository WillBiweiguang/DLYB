using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Web.Domain.Common
{
   public class Utility
    {
        public static string ReplaceFileSign(string filename)
        {
            return filename.Replace("+", "");
        }
    }
}
