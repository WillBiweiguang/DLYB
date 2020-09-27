using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Web.Domain.Common
{
    public enum TaskStatus
    {
        [Description("未提交")]
        NotRequest,
        [Description("审核中")]
        PendingApproval,
        [Description("已审核")]
        Approved,
        [Description("已驳回")]
        Rejected
    }
}
