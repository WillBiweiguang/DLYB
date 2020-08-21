
using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Infrastructure.Web.Security
{
    public interface ILDAPUtil : IDependency
    {
        bool Authenticate(string userName, string password);
        LDAPUser GetUserInfo(string userLoginId);
        LDAPUser GetUserInfo(string userLoginId, out bool found);
        List<LDAPUser> GetUserInfoList(List<string> LoginIdList);
        List<LDAPUser> GetUserInfoList(string key);
        List<LDAPUser> GetUserList(string key, int rowCount);
        DataTable GetUserList(string userID);
        List<LDAPUser> GetUserListByName(string name, int rowCount);
        string GetUserWorkCountry(string userAccount);
        bool IsUserExist(string userLoginId);

        string GetUserMailAddress(string userId);
    }
}