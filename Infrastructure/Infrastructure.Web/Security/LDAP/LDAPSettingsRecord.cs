using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infrastructure.Web.Security
{
    public class LDAPSettingsRecord
    {
        public virtual int Id { get; set; }

        public virtual string MailAttribute { get; set; }
        public virtual string NameAttribute { get; set; }
        public virtual string SearchBase { get; set; }
        public virtual string CountryAttribute { get; set; }

        public virtual string AdminDN { get; set; }
        public virtual string AdminPassword { get; set; }
        public virtual string LdapServer { get; set; }
        public virtual int LdapServerPort { get; set; }
        public virtual string LdapUserDN { get; set; }
    }
}