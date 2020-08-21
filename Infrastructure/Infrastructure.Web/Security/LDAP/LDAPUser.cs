using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infrastructure.Web.Security
{
    public class LDAPUser
    {
        private string _loginId;

        public string LoginId
        {
            get { return _loginId; }
            set { _loginId = value; }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _mail;

        public string Mail
        {
            get { return _mail; }
            set { _mail = value; }
        }

        private bool isInDB;

        public bool IsInDB
        {
            get { return isInDB; }
            set { isInDB = value; }
        }
        private bool isInLdap;

        public bool IsInLdap
        {
            get { return isInLdap; }
            set { isInLdap = value; }
        }

        private int dbId = 0;

        public int DbId
        {
            get { return dbId; }
            set { dbId = value; }
        }
        private bool isAdmin = false;

        public bool IsAdmin
        {
            get { return isAdmin; }
            set { isAdmin = value; }
        }


        public string Country
        {
            get;
            set;
        }
    }
}