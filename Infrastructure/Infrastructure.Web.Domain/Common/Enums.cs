using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Web.Domain.Common
{
    public enum TaskStatus
    {
        NotRequest,
        PendingApproval,
        Approved,
        Rejected
    }
}
