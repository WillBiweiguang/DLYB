using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Web.Domain.Common
{
   public class Utility
    {
        public static string ReplaceWebFileName(string filename)
        {
            //处理符号 ;/?:@&=+$,#' 
            return filename.Replace("+", "-(")
                .Replace("'", "-(")
                .Replace(";", "-(")
                .Replace("/", "-(")
                .Replace("?", "-(")
                .Replace(":", "-(")
                .Replace("@", "-(")
                .Replace("&", "-(")
                .Replace("=", "-(")
                .Replace("$", "-(")
                .Replace(",", "-(")
                .Replace("#", "-(")
                .Replace(" ", "-(");
        }
    }
}
