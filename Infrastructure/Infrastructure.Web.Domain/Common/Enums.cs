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

    /// <summary>
    /// dwg 文件处理状态
    /// </summary>
    public enum BeamProcessStatus
    {
        [Description("未开始")]
        NotStart = 0,
        [Description("焊缝类别统计中")]
        InStatistics = 1,
        [Description("焊缝类别识别中")]
        Recognizing = 2
    }
}
