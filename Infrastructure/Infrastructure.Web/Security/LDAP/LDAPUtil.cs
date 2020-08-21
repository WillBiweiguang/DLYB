using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Utilclass;

using Infrastructure.Core.Logging;

namespace Infrastructure.Web.Security
{
    public class LDAPUtil : ILDAPUtil
    {
        private static readonly string mailAttribute = "mail";
        private static readonly string nameAttribute = "cn";
        private static readonly string searchBase = "o=lilly,dc=com";
        private static readonly string countryAttribute = "EdsWorkCntryCd";

        private static readonly string adminDN = "uid=AdminIMS,ou=adminaccount,o=lilly,dc=com";
        private static readonly string adminPassword = "r@nd0m!se";
        private static readonly string ldapServer = "dsazone1.d51.lilly.com";
        private static readonly int ldapServerPort = 389;
        private static readonly string ldapUserDN = "uid=$UID,ou=account,o=lilly,dc=com";


      //  private readonly IRepository<LDAPSettingsRecord> _ldapSettingsRepository;

        public LDAPUtil()
        {
           // _ldapSettingsRepository = ldapSettingsRepository;

          //  T = NullLocalizer.Instance;
            Logger = LogManager.GetLogger(this.GetType());
        }

       // public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public LDAPSettingsRecord GetLDAPSettings()
        {
            LDAPSettingsRecord ldapSettings = null;// _ldapSettingsRepository.Table.FirstOrDefault();
            if (ldapSettings == null)
            {
                Logger.Warn("email message settings is null, default value will be used.");
                ldapSettings = new LDAPSettingsRecord
                {
                    // defaul value.        
                    Id = 1,
                    MailAttribute = mailAttribute,
                    NameAttribute = nameAttribute,
                    SearchBase = searchBase,
                    CountryAttribute = countryAttribute,

                    AdminDN = adminDN,
                    AdminPassword = adminPassword,
                    LdapServer = ldapServer,
                    LdapServerPort = ldapServerPort,
                    LdapUserDN = ldapUserDN
                };
            }
            return ldapSettings;
        }

        public LDAPUser GetUserInfo(string userLoginId)
        {
            bool found;
            return GetUserInfo(userLoginId, out found);
        }

        public LDAPUser GetUserInfo(string userLoginId, out bool found)
        {
            var ldapSettings = GetLDAPSettings();
            found = false;
            LDAPUser user = new LDAPUser();
            string userId = userLoginId.ToUpper();
            user.LoginId = userLoginId;

            user.Name = userLoginId;
            
            LdapConnection ldapConn = null;
            try
            {
                ldapConn = new LdapConnection();
                ldapConn.Connect(ldapSettings.LdapServer, ldapSettings.LdapServerPort);
                ldapConn.Bind(ldapSettings.AdminDN, ldapSettings.AdminPassword);
                if (ldapConn.Bound)
                {
                    //logger.Debug("Attributes:" + attributeName + "value:" + attributeVal);
                    LdapSearchQueue queue = ldapConn.Search(ldapSettings.SearchBase, LdapConnection.SCOPE_SUB,
                    "uid=" + userId,
                    null, false, (LdapSearchQueue)null, (LdapSearchConstraints)null);
                    LdapMessage message;

                    while ((message = queue.getResponse()) != null)
                    {
                        if (message is LdapSearchResult)
                        {
                            LdapEntry entry = ((LdapSearchResult)message).Entry;

                            LdapAttributeSet attributeSet = entry.getAttributeSet();
                            System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();
                            while (ienum.MoveNext())
                            {
                                LdapAttribute attribute = (LdapAttribute)ienum.Current;
                                string attributeName = attribute.Name;
                                string attributeVal = attribute.StringValue;
                                if (!Base64.isLDIFSafe(attributeVal))
                                {
                                    byte[] tbyte = SupportClass.ToByteArray(attributeVal);
                                    attributeVal = Base64.encode(SupportClass.ToSByteArray(tbyte));
                                }
                                Logger.Debug("Attributes:" + attributeName + "value:" + attributeVal);

                                if (ldapSettings.MailAttribute.Equals(attributeName.Trim()))
                                {
                                    user.Mail = attributeVal;
                                    found = true;
                                }
                                if (ldapSettings.NameAttribute.Equals(attributeName.Trim()))
                                {
                                    user.Name = attributeVal;
                                    found = true;
                                }
                                if (ldapSettings.CountryAttribute.Equals(attributeName.Trim()))
                                {
                                    user.Country = attributeVal;
                                    found = true;
                                }
                            }
                        }//end if
                    }//end whil                   
                }// end if
            }
            catch (Exception ex)
            {
                Logger.Error( "GetUserInfo failed.",ex);
                user = null;
                //throw ex;
            }
            finally
            {
                if (ldapConn != null)
                {
                    ldapConn.Disconnect();
                }
            }
            return user;
        }

        public bool Authenticate(string userName, string password)
        {
            var ldapSettings = GetLDAPSettings();
            Logger.Debug("Try to connect to LDAP to authenticate user with " + userName + "/***");

            string uDN = ldapSettings.LdapUserDN.Replace("$UID", userName);
            //logger.Debug("UserDN:" + uDN);

            bool flag = false;
            LdapConnection ldapConn = new LdapConnection();
            try
            {
                ldapConn.Connect(ldapSettings.LdapServer, ldapSettings.LdapServerPort);
                //Bind function will Bind the user object Credentials to the Server 
                ldapConn.Bind(uDN, password);
                flag = ldapConn.Bound;
            }
            catch (Exception ex)
            {
                Logger.Error( "Exception in Authenticate():",ex);
            }
            finally
            {
                ldapConn.Disconnect();
            }

            Logger.Debug("Authenticate result:" + flag);
            return flag;
        }


        public bool IsUserExist(string userLoginId)
        {
            var ldapSettings = GetLDAPSettings();
            Logger.Debug("Checking " + userLoginId);

            LdapConnection ldapConn = null;
            try
            {
                ldapConn = new LdapConnection();
                ldapConn.Connect(ldapSettings.LdapServer, ldapSettings.LdapServerPort);
                ldapConn.Bind(ldapSettings.AdminDN, ldapSettings.AdminPassword);
                if (ldapConn.Bound)
                {
                    LdapSearchQueue queue = ldapConn.Search(ldapSettings.SearchBase, LdapConnection.SCOPE_SUB,
                    "uid=" + userLoginId,
                    null, false, (LdapSearchQueue)null, (LdapSearchConstraints)null);
                    LdapMessage message;

                    message = queue.getResponse();
                    if (message == null || !(message is LdapSearchResult))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("IsUserExist fialed.", ex);
                return false;
            }
            finally
            {
                if (ldapConn != null)
                {
                    ldapConn.Disconnect();
                }
            }
            return false;
        }

        public DataTable GetUserList(string userID)
        {
            LdapConnection ldapConn = null;
            DataTable dt = null;
            try
            {
                ldapConn = new LdapConnection();
                ldapConn.Connect("dsazone1.d51.lilly.com", 389);
                ldapConn.Bind("uid=AdminIMS,ou=adminaccount,o=lilly,dc=com", "r@nd0m!se");

                if (ldapConn.Bound)
                {
                    dt = new DataTable();
                    dt.Columns.Add(new DataColumn("EdsSearchFirstNm", typeof(String)));
                    dt.Columns.Add(new DataColumn("EdsSearchLastNm", typeof(String)));
                    dt.Columns.Add(new DataColumn("uid", typeof(String)));
                    dt.Columns.Add(new DataColumn("EdsWorkCntryCd", typeof(String)));
                    dt.Columns.Add(new DataColumn("employeeType", typeof(String)));
                    dt.Columns.Add(new DataColumn("mail", typeof(String)));

                    string tmp = string.Empty;

                    LdapSearchQueue queue2 = ldapConn.Search("o=lilly,dc=com", LdapConnection.SCOPE_SUB, "EdsSearchFirstNm=*" + userID + "*", null, false, (LdapSearchQueue)null, (LdapSearchConstraints)null);
                    LdapMessage msg22;

                    while ((msg22 = queue2.getResponse()) != null)
                    {
                        if (msg22 is LdapSearchResult)
                        {
                            LdapEntry entry = ((LdapSearchResult)msg22).Entry;

                            LdapAttributeSet attributeSet = entry.getAttributeSet();
                            System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();

                            string attributeName = string.Empty;
                            string attributeVal = string.Empty;

                            DataRow dr = dt.NewRow();

                            while (ienum.MoveNext())
                            {
                                LdapAttribute attribute = (LdapAttribute)ienum.Current;
                                attributeName = attribute.Name;
                                attributeVal = attribute.StringValue;

                                if (attributeName.Equals("EdsSearchFirstNm") == true)
                                {
                                    tmp = attributeVal;
                                    dr[0] = tmp;
                                }

                                if (attributeName.Equals("EdsSearchLastNm") == true)
                                {
                                    tmp = attributeVal;
                                    dr[1] = tmp;
                                }

                                if (attributeName.Equals("uid") == true)
                                {
                                    tmp = attributeVal;
                                    dr[2] = tmp;
                                }
                                if ((attributeName.Equals("EdsWorkCntryCd") == true) && (attributeVal.Equals("CN") == true))
                                {
                                    tmp = attributeVal;
                                    dr[3] = tmp;
                                }

                                if (attributeName.Equals("employeeType") == true)
                                {
                                    tmp = attributeVal;
                                    dr[4] = tmp;
                                }

                                if (attributeName.Equals("mail") == true)
                                {
                                    tmp = attributeVal;
                                    dr[5] = tmp;
                                }
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                Logger.Error("GetUserList failed.", ex);
                return dt;
            }
            finally
            {
                if (ldapConn != null)
                {
                    ldapConn.Disconnect();
                }
            }
        }


        public List<LDAPUser> GetUserInfoList(List<string> LoginIdList)
        {
            var ldapSettings = GetLDAPSettings();
            bool flag = false;
            string uidAttribute = "uid";
            List<LDAPUser> userList = new List<LDAPUser>();

            LdapConnection ldapConn = null;
            try
            {
                ldapConn = new LdapConnection();
                ldapConn.Connect(ldapSettings.LdapServer, ldapSettings.LdapServerPort);
                ldapConn.Bind(ldapSettings.AdminDN, ldapSettings.AdminPassword);
                for (int i = 0; i < LoginIdList.Count; i++)
                {
                    LoginIdList[i].ToUpper();
                }
                //user.LoginId = userLoginId;
                //user.Name = userLoginId;
                StringBuilder QueryStr = new StringBuilder();
                if (LoginIdList.Count == 1)
                {
                    QueryStr.Append("uid=" + LoginIdList[0]);
                }
                else
                {
                    QueryStr.Append("(|");
                    for (int i = 0; i < LoginIdList.Count; i++)
                    {
                        QueryStr.Append("(uid=" + LoginIdList[i] + ")");
                    }
                    QueryStr.Append(")");
                }

                if (ldapConn.Bound)
                {
                    LdapSearchQueue queue = ldapConn.Search(ldapSettings.SearchBase, LdapConnection.SCOPE_SUB,
                    QueryStr.ToString(),
                    null, false, (LdapSearchQueue)null, (LdapSearchConstraints)null);
                    LdapMessage message;
                    while ((message = queue.getResponse()) != null)
                    {
                        LDAPUser user = new LDAPUser();
                        if (message is LdapSearchResult)
                        {
                            flag = false;
                            LdapEntry entry = ((LdapSearchResult)message).Entry;
                            LdapAttributeSet attributeSet = entry.getAttributeSet();
                            System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();
                            while (ienum.MoveNext())
                            {
                                LdapAttribute attribute = (LdapAttribute)ienum.Current;
                                string attributeName = attribute.Name;
                                string attributeVal = attribute.StringValue;
                                if (!Base64.isLDIFSafe(attributeVal))
                                {
                                    byte[] tbyte = SupportClass.ToByteArray(attributeVal);
                                    attributeVal = Base64.encode(SupportClass.ToSByteArray(tbyte));
                                }
                                Logger.Debug("Attributes:" + attributeName + "value:" + attributeVal);

                                if (uidAttribute.Equals(attributeName.Trim()))
                                {
                                    user.LoginId = attributeVal;
                                }
                                if (ldapSettings.MailAttribute.Equals(attributeName.Trim()))
                                {
                                    user.Mail = attributeVal;
                                    flag = true;
                                }
                                if (ldapSettings.NameAttribute.Equals(attributeName.Trim()))
                                {
                                    user.Name = attributeVal;
                                    flag = true;
                                }
                            }
                            if (flag == true)
                            {
                                if (!string.IsNullOrEmpty(user.LoginId) && !string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.Mail))
                                    userList.Add(user);
                            }

                        }//end if

                    }//end while                   
                }
            }
            catch (Exception ex)
            {
                Logger.Error( "GetUserInfoList failed.", ex);
                userList.Clear();
                //throw ex;
            }
            finally
            {
                if (ldapConn != null)
                {
                    ldapConn.Disconnect();
                }
            }
            return userList;
        }

        public List<LDAPUser> GetUserInfoList(string key)
        {
            var ldapSettings = GetLDAPSettings();
            bool flag = false;
            string uidAttribute = "uid";
            List<LDAPUser> userList = new List<LDAPUser>();



            LdapConnection ldapConn = null;
            try
            {
                ldapConn = new LdapConnection();
                ldapConn.Connect(ldapSettings.LdapServer, ldapSettings.LdapServerPort);
                ldapConn.Bind(ldapSettings.AdminDN, ldapSettings.AdminPassword);

                var queryStr = string.Format("(|(uid={0})(cn={1})(mail={2}))", key, key, key);

                if (ldapConn.Bound)
                {
                    LdapSearchQueue queue = ldapConn.Search(ldapSettings.SearchBase, LdapConnection.SCOPE_SUB,
                    queryStr,
                    null, false, (LdapSearchQueue)null, (LdapSearchConstraints)null);
                    LdapMessage message;
                    while ((message = queue.getResponse()) != null)
                    {
                        LDAPUser user = new LDAPUser();
                        if (message is LdapSearchResult)
                        {
                            flag = false;
                            LdapEntry entry = ((LdapSearchResult)message).Entry;
                            LdapAttributeSet attributeSet = entry.getAttributeSet();
                            System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();
                            while (ienum.MoveNext())
                            {
                                LdapAttribute attribute = (LdapAttribute)ienum.Current;
                                string attributeName = attribute.Name;
                                string attributeVal = attribute.StringValue;
                                if (!Base64.isLDIFSafe(attributeVal))
                                {
                                    byte[] tbyte = SupportClass.ToByteArray(attributeVal);
                                    attributeVal = Base64.encode(SupportClass.ToSByteArray(tbyte));
                                }
                                Logger.Debug("Attributes:" + attributeName + "value:" + attributeVal);

                                if (uidAttribute.Equals(attributeName.Trim()))
                                {
                                    user.LoginId = attributeVal;
                                }
                                if (ldapSettings.MailAttribute.Equals(attributeName.Trim()))
                                {
                                    user.Mail = attributeVal;
                                    flag = true;
                                }
                                if (ldapSettings.NameAttribute.Equals(attributeName.Trim()))
                                {
                                    user.Name = attributeVal;
                                    flag = true;
                                }
                            }
                            if (flag == true)
                            {
                                if (!string.IsNullOrEmpty(user.LoginId) && !string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.Mail))
                                    userList.Add(user);
                            }

                        }//end if

                    }//end while                   
                }
            }
            catch (Exception ex)
            {
                Logger.Error( "GetUserInfoList failed.", ex);
                userList.Clear();
                //throw ex;
            }
            finally
            {
                if (ldapConn != null)
                {
                    ldapConn.Disconnect();
                }
            }
            return userList;
        }

        public List<LDAPUser> GetUserList(string key, int rowCount)
        {
            var ldapSettings = GetLDAPSettings();
            bool flag = false;
            string uidAttribute = "uid";
            List<LDAPUser> userList = new List<LDAPUser>();

            LdapConnection ldapConn = null;
            try
            {
                ldapConn = new LdapConnection();

                ldapConn.Connect(ldapSettings.LdapServer, ldapSettings.LdapServerPort);
                ldapConn.Bind(ldapSettings.AdminDN, ldapSettings.AdminPassword);

                var queryStr = string.Format("(|(uid=*{0}*)(cn=*{1}*)(mail=*{2}*))", key, key, key);

                if (ldapConn.Bound)
                {
                    //specify result's count
                    var cons = ldapConn.SearchConstraints;
                    cons.MaxResults = rowCount;
                    //---

                    LdapSearchQueue queue = ldapConn.Search(ldapSettings.SearchBase, LdapConnection.SCOPE_SUB,
                    queryStr,
                    null, false, (LdapSearchQueue)null, cons);
                    LdapMessage message;
                    while ((message = queue.getResponse()) != null)
                    {
                        LDAPUser user = new LDAPUser();
                        if (message is LdapSearchResult)
                        {
                            flag = false;
                            LdapEntry entry = ((LdapSearchResult)message).Entry;
                            LdapAttributeSet attributeSet = entry.getAttributeSet();
                            System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();
                            while (ienum.MoveNext())
                            {
                                LdapAttribute attribute = (LdapAttribute)ienum.Current;
                                string attributeName = attribute.Name;
                                string attributeVal = attribute.StringValue;
                                if (!Base64.isLDIFSafe(attributeVal))
                                {
                                    byte[] tbyte = SupportClass.ToByteArray(attributeVal);
                                    attributeVal = Base64.encode(SupportClass.ToSByteArray(tbyte));
                                }
                                Logger.Debug("Attributes:" + attributeName + "value:" + attributeVal);

                                if (uidAttribute.Equals(attributeName.Trim()))
                                {
                                    user.LoginId = attributeVal;
                                }
                                if (ldapSettings.MailAttribute.Equals(attributeName.Trim()))
                                {
                                    user.Mail = attributeVal;
                                    flag = true;
                                }
                                if (ldapSettings.NameAttribute.Equals(attributeName.Trim()))
                                {
                                    user.Name = attributeVal;
                                    flag = true;
                                }
                            }
                            if (flag == true)
                            {
                                if (!string.IsNullOrEmpty(user.LoginId) && !string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.Mail))
                                    userList.Add(user);
                            }

                        }//end if

                    }//end while                   
                }
            }
            catch (Exception ex)
            {
                Logger.Error( "GetUserList failed.", ex);
                userList.Clear();
                //throw ex;
            }
            finally
            {
                if (ldapConn != null)
                {
                    ldapConn.Disconnect();
                }
            }
            return userList;
        }

        //match the key to first name or last name
        public List<LDAPUser> GetUserListByName(string name, int rowCount)
        {
            var ldapSettings = GetLDAPSettings();
            bool flag = false;
            string uidAttribute = "uid";
            List<LDAPUser> userList = new List<LDAPUser>();

            LdapConnection ldapConn = null;
            try
            {
                ldapConn = new LdapConnection();

                ldapConn.Connect(ldapSettings.LdapServer, ldapSettings.LdapServerPort);
                ldapConn.Bind(ldapSettings.AdminDN, ldapSettings.AdminPassword);

                var queryStr = string.Format("(|(EdsSearchFirstNm={0})(EdsSearchLastNm={1}))", name, name);

                if (ldapConn.Bound)
                {
                    //specify result's count
                    var cons = ldapConn.SearchConstraints;
                    cons.MaxResults = rowCount;
                    //---

                    LdapSearchQueue queue = ldapConn.Search(ldapSettings.SearchBase, LdapConnection.SCOPE_SUB,
                    queryStr,
                    null, false, (LdapSearchQueue)null, cons);
                    LdapMessage message;
                    while ((message = queue.getResponse()) != null)
                    {
                        LDAPUser user = new LDAPUser();
                        if (message is LdapSearchResult)
                        {
                            flag = false;
                            LdapEntry entry = ((LdapSearchResult)message).Entry;
                            LdapAttributeSet attributeSet = entry.getAttributeSet();
                            System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();
                            while (ienum.MoveNext())
                            {
                                LdapAttribute attribute = (LdapAttribute)ienum.Current;
                                string attributeName = attribute.Name;
                                string attributeVal = attribute.StringValue;
                                if (!Base64.isLDIFSafe(attributeVal))
                                {
                                    byte[] tbyte = SupportClass.ToByteArray(attributeVal);
                                    attributeVal = Base64.encode(SupportClass.ToSByteArray(tbyte));
                                }
                                Logger.Debug("Attributes:" + attributeName + "value:" + attributeVal);

                                if (uidAttribute.Equals(attributeName.Trim()))
                                {
                                    user.LoginId = attributeVal;
                                }
                                if (ldapSettings.MailAttribute.Equals(attributeName.Trim()))
                                {
                                    user.Mail = attributeVal;
                                    flag = true;
                                }
                                if (ldapSettings.NameAttribute.Equals(attributeName.Trim()))
                                {
                                    user.Name = attributeVal;
                                    flag = true;
                                }
                            }
                            if (flag == true)
                            {
                                if (!string.IsNullOrEmpty(user.LoginId) && !string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.Mail))
                                    userList.Add(user);
                            }

                        }//end if

                    }//end while                   
                }
            }
            catch (Exception ex)
            {
                Logger.Error( "GetUserListByName failed.", ex);
                userList.Clear();
                //throw ex;
            }
            finally
            {
                if (ldapConn != null)
                {
                    ldapConn.Disconnect();
                }
            }
            return userList;
        }


        public string GetUserWorkCountry(string userAccount)
        {
            var user = GetUserInfo(userAccount);
            if (user != null)
            {
                return user.Country;
            }
            return null;
        }

        public string GetUserMailAddress(string userId)
        {
            bool found;
            var userInfo = GetUserInfo(userId, out found);
            if (found)
            {
                return userInfo.Mail;
            }
            return null;
        }
    }
}

