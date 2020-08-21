using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infrastructure.Web.Net.Mail
{
    public class EmailMessageRecord
    {
        public virtual int Id { get; set; }

        public virtual string FromAddress { get; set; }
        public virtual string ToAddress { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }
        public virtual bool IsBodyHtml { get; set; }

        public virtual int Status { get; set; }

        public virtual int OwnerId { get; set; }
        public virtual DateTime? CreatedUtc { get; set; }
        public virtual int ModifiedUserId { get; set; }
        public virtual DateTime? ModifiedUtc { get; set; }
    }
}