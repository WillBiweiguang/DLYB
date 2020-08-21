using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Contracts;
using DLYB.CA.Contracts;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.ModelsView;
using DLYB.CA.Service.Common;
using DLYB.Weixin.QY.AdvancedAPIs;
using Infrastructure.Web.Net.Mail;
using System.Threading.Tasks;
using Infrastructure.Utility.Data;
using DLYB.CA.Contracts.ViewModel;
using Infrastructure.Web.Domain.Service;

namespace DLYB.CA.Service
{
    /// <summary>
    /// 业务实现——问卷调查模块
    /// </summary>
    public class FeedBackService : BaseService<FeedBackEntity>, IFeedbackService
    {
        private readonly ICategoryService _categoryService;
        public FeedBackService()
            : base("CAAdmin")
        {
            
        }

        public IList<FeedBackView> QueryList(int appId, string menuCode = null)
        {
            Expression<Func<FeedBackEntity, bool>> where;
            if (menuCode == null)
            {
                where = x => x.AppID == appId;
            }
            else
            {
                where = x => x.AppID == appId && x.MenuCode == menuCode;
            }

            var list = Repository.Entities.Where(where).ToList();

            var codes = list.Select(y => y.MenuCode).ToList();

            //var categories = _categoryService.Repository.Entities.Where(x => codes.Contains(x.CategoryCode)).Select(x => new { code = x.CategoryCode, name = x.CategoryName }).ToList();

            var viewList = list.Select(x =>
                 {
                     //var category = categories.Find(y => y.code == x.MenuCode);
                     var view = new FeedBackView();
                     view.ConvertAPIModel(x);
                     //view.MenuName = category.name;
                     return view;
                 }).ToList();


            return viewList;
        }
        //返回feedback列表
        public List<FBConfig> feedbackList()
        {
            List<FBConfig> thisList = new List<FBConfig>();
            thisList = JsonHelper.FromJson<List<FBConfig>>(CommonService.GetSysConfig("Feedback", ""));
            return thisList;
        }
        public List<Email> EmailList(string email)
        {
            List<Email> thisList = new List<Email>();
            thisList = JsonHelper.FromJson<List<Email>>(CommonService.GetSysConfig(email, ""));
            return thisList;
        }
        public bool SendMail(string LillyId,int AppId,string question, string jsonString, string request, string baseUrl, string title)
        {
            //获取收件人信息 
            string strToken = WeChatCommonService.GetWeiXinToken(AppId);
            var mobile = string.Empty;

            var obj = MailListApi.GetMember(strToken, LillyId);
            string receiver = obj != null ? obj.email : "";

            var EmailReceiver = "";
            var EmailEnableSsl = false;
            var EmailHost = "";
            var EmailPassword = "";
            var EmailUserName = "";
            var EmailPort = "";
            var EmailSender = "";
            var EmailEnable = true;
            var EmailTitle = "";
            var EmailTemplate = "";

            object jsonResult = null;
            jsonResult = JsonHelper.FromJson<Dictionary<string, object>>(jsonString); 
            var fullResult = jsonResult as Dictionary<string, object>;
            if (fullResult != null)
            {
                EmailReceiver = fullResult["EmailReceiver"] as string;
                EmailEnableSsl = bool.Parse(fullResult["EmailEnableSsl"].ToString());
                EmailHost = fullResult["EmailHost"] as string;
                EmailPassword = fullResult["EmailPassword"] as string;
                EmailUserName = fullResult["EmailUserName"] as string;
                EmailPort = fullResult["EmailPort"] as string;
                EmailSender = fullResult["EmailSender"] as string;
                EmailEnable = bool.Parse(fullResult["EmailEnable"].ToString());
                EmailTitle = fullResult["EmailTitle"] as string;
                EmailTemplate = fullResult["EmailTemplate"] as string;
            }

            if (!string.IsNullOrEmpty(EmailTitle))
            {
                //特殊处理EmailTitle
                if (!string.IsNullOrEmpty(title))
                {
                    EmailTitle = string.Format("{0} ({1},{2})", EmailTitle, LillyId, DateTime.Now.ToShortDateString());
                }
                EmailTitle = EmailTitle.Replace("\r", "").Replace("\n", "");
            }

            var emailContent = EmailTemplate.Replace("#Url#", request).Replace("#Question#", question).
                Replace("#LillyId#", LillyId).Replace("#Email#", receiver);

            var set = new EmailMessageSettingsRecord()
            {
                Host = EmailHost,
                UserName = EmailUserName,
                Password = EmailPassword,
                Port = int.Parse(EmailPort),
                EnableSsl = EmailEnableSsl,
                Enable = EmailEnable,
                DeliveryMethod = "Network",
                Address = EmailSender,
                RequireCredentials = true
            };

            Task.Run(() =>
            {
                var ser = new EmailMessageService(set);
                ser.SendMessage(EmailSender, EmailReceiver, EmailTitle,
                    emailContent.Replace("\r\n", "<br/>").Replace("\n", "<br/>").Replace("\r", "<br/>"));
            });

            return true;
        }
    }
}