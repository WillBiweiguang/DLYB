
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.Web.Security
{


    public   class UserCommon
    {
        public static string GetUserName(IIdentity identity)
        {
            string identityName = identity == null ? null : identity.Name;
            if (string.IsNullOrEmpty(identityName))
                return identityName;
            int index = identityName.IndexOf('\\');
            if (index > 0 && index < (identityName.Length - 1))
            {
                identityName = identityName.Substring(index + 1);
            }
            index = identityName.IndexOf('@');
            if (index > 0 && index < (identityName.Length - 1))
            {
                identityName = identityName.Remove(index);
            }

            return identityName;
        }
    }
}
