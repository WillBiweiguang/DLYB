using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Web.Domain.Service
{
    /// <summary>
    /// Category Type, 具体数值和微信中的Application ID对应
    /// </summary>
    public enum CategoryType
    {
        [Description("News")]
        ArticleInfoCate = 6,

        [Description("News")]
        NewsCate = 6,

        [Description("Hongtu")]
        HongtuCate = 7,

        [Description("Video")]
        VideoCate = 8,

        [Description("Activity")]
        ActivityCate = 9,

        [Description("AdminApp")]
        AdminAppCate = 12,

        [Description("NSC")]
        NSCCate = 14,

        [Description("SPP")]
        SPPCate = 15,

        [Description("SalesTraining")]
        SalesTrainingCate = 16,

        [Description("HREService")]
        HREServiceCate = 17,

        [Description("LED")]
        LEDCate = 13,

        [Description("ITHelpdesk")]
        ITHelpdeskCate = 10,

        [Description("JohnLAccessChina")]
        JohnLAccessChina = 20,

        [Description("ACC")]
        ACC = 21,

        [Description("SZ")]
        SZ=22,

        [Description("Undefined")]
        Undefined = 0 //modify 1 to 0 by andrew

    }

    public enum SysConfigCode
    {
        [Description("NewsTemplate")]
        NewsTemplate = 1,
        [Description("WeixinCorpId")]
        WeixinCorpId = 2,
        [Description("WeixinCorpSecret")]
        WeixinCorpSecret = 3,
        [Description("EmailSender")]
        EmailSender = 4,
        [Description("EmailReceiver")]
        EmailReceiver = 5,
        [Description("EnableMail")]
        EnableMail = 6,
        [Description("MailTemplate")]
        MailTemplate = 7,
        [Description("EnableSsl")]
        EnableSsl = 8,
        [Description("Port")]
        Port = 9,
        [Description("Host")]
        Host = 10,
        [Description("EmailPassword")]
        EmailPassword = 11,
        [Description("EmailUserName")]
        EmailUserName = 12,
        [Description("EmailEnable")]
        EmailEnable = 13,
        [Description("EmailCCList")]
        EmailCCList = 14,
        [Description("CallBackUrl")]
        CallBackUrl=15
    }

    public enum StatusType
    {
        [Description("")]
        Other = -3,
        [Description("Saved")]
        Saved = -2,
        [Description("Rejected")]
        Rejected = -1,
        [Description("Rollbacked")]
        Rollbacked = 0,
        [Description("Submitted")]
        Submitted = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Published")]
        Published = 3
    }

}
