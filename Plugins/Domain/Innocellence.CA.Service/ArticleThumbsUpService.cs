using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Entity;

namespace DLYB.CA.Service
{
    public class ArticleThumbsUpService : BaseService<ArticleThumbsUp>, IArticleThumbsUpService
    {
     
        public ArticleThumbsUpService()
            : base("CAAdmin")
        {
        }

    }
}
