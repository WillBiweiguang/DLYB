using System.Collections.Generic;
using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.ModelsView;
using DLYB.CA.Contracts.ViewModel;

namespace DLYB.CA.Contracts
{
    public interface IFeedbackService : IDependency, IBaseService<FeedBackEntity>
    {
        IList<FeedBackView> QueryList(int appId, string menuCode = null);
        List<FBConfig> feedbackList();
        List<Email> EmailList(string email);
        bool SendMail(string LillyId, int AppId, string question, string jsonString, string request, string baseUrl, string title);
    }
}
