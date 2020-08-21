using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infrastructure.Web.Net.Mail
{
    public class EmailMessageSettingsRecord
    {
        public virtual int Id { get; set; }
        public virtual bool Enable { get; set; }

        public virtual string Address { get; set; }
        public virtual bool RequireCredentials { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }

        public virtual string Host { get; set; }
        public virtual int Port { get; set; }
        public virtual bool EnableSsl { get; set; }
        public virtual string DeliveryMethod { get; set; }
    }
}