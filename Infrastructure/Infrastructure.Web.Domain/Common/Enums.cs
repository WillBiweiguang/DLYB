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
        [Description("待审核")]
        PendingApproval,
        [Description("通过")]
        Approved,
        [Description("驳回")]
        Rejected
    }

    /// <summary>
    /// dwg 文件处理状态
    /// </summary>
    public enum BeamProcessStatus
    {
        [Description("未开始")]
        NotStart = 0,
        [Description("进行中")]
        InProcessing = 1,
        [Description("已完成")]
        Complete = 2
    }

    public enum EnumMenuId
    {
        Admin = 3,
        Manager = 1,
        TaskView = 6,
        TaskApprove = 7
    }
}
