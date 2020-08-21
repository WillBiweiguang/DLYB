using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Service
{
    public class NewsPublishHistoryService : BaseService<NewsPublishHistoryEntity>, INewsPublishHistoryService
    {
    }


    public enum PublishHistoryType
    {
        Article,
        Message
    }

    public enum SendStatus
    {
        Success,
        Failed
    }
}
