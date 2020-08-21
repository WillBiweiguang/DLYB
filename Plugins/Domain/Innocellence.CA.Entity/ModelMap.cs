using AutoMapper;
using Infrastructure.Web.Domain.Entity;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Contracts.ViewModel;
using DLYB.CA.ModelsView;

namespace DLYB.CA.Entity
{

    public class ModelMappers
    {
        public static void MapperRegister()
        {
            //Identity
            Mapper.CreateMap<ArticleInfoView, ArticleInfo>();
            Mapper.CreateMap<QuestionManageView, QuestionManage>();
            Mapper.CreateMap<ArticleThumbsUpView, ArticleThumbsUp>();
            Mapper.CreateMap<MessageView, Message>();
            Mapper.CreateMap<MessageTextView, MessageText>();
            Mapper.CreateMap<WechatUserView, WechatUser>();
            Mapper.CreateMap<ThumbsUpCountView, ThumbsUp>();
            Mapper.CreateMap<SearchKeywordView, SearchKeyword>();
            Mapper.CreateMap<FaqInfoView, FaqInfo>();
            Mapper.CreateMap<ArticleImagesView, ArticleImages>();

            Mapper.CreateMap<AppMenuView, Category>();
            Mapper.CreateMap<FeedBackView, FeedBackEntity>();
            Mapper.CreateMap<PageReportGroupView, PageReportGroup>();
            Mapper.CreateMap<AccessDashboardView, AccessDashboard>();
            Mapper.CreateMap<FileManageView, FileManage>();
            Mapper.CreateMap<PerformanceLoggingView, PerformanceLogging>();
            Mapper.CreateMap<Finance3rdQueryEntityView, Finance3rdQueryEntity>();
            Mapper.CreateMap<FinanceEntityView, FinanceQueryEntity>();
            Mapper.CreateMap<QuestionSubView, QuestionSub>();
            Mapper.CreateMap<SubmitInfoView,SubmitInfo>();
        }

    }
}
