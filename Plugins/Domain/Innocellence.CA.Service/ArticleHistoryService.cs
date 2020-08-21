using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.ModelsView;
using DLYB.CA.Entity;
using DLYB.CA.Services;
using DLYB.CA.Contracts;


namespace DLYB.CA.Service
{
    public class ArticleHistoryService : BaseService<ArticleInfo>, IArticleHistoryService
    {
        private IArticleImagesService _articleImageService;

        public ArticleHistoryService(IArticleImagesService articleImageService)
            : base("CAAdmin")
        {
            _articleImageService = articleImageService;
        }



        public List<ArticleInfoView> GetArticleHistoryList(int AppId, PageCondition condition, out int totalRow)
        {
            var where = Repository.Entities.Where(a => a.AppId == AppId && a.ArticleStatus == "Published" && a.IsDeleted == false);


            totalRow = where.Count();

            var list = where.OrderByDescending(x => x.PublishDate).Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToList();

            //return list.Select(n => (ArticleInfoView)new ArticleInfoView().ConvertAPIModel(n)).ToList();
            //////var lists123 = (from item3 in Repository.Entities where item3.AppId==AppId select item3).ToList();
            //////var item9 = (from item in lists123
            //////             join item1 in _articleImageService.Repository.Entities.Select(x => new { ArticleID = x.ArticleID, ImageName = x.ImageName }) on item.Id equals item1.ArticleID
            //////             select new ArticleInfoView()
            //////             {
            //////                 Id = item.Id,
            //////                 //ImageHistoryList = new ArticleImagesView()
            //////                 //{
            //////                 //    Id= (int)item1.ArticleID,
            //////                 ImageName = item1.ImageName,
            //////                 ArticleTitle=item.ArticleTitle,
            //////                 ArticleContent=item.ArticleContent
            //////                 //},
            //////                 //ImgUrl= new string[]{ item1.ImageName},
            //////             }).ToList();
           
            //////totalRow = item9.Count();

            //////var list = item9.OrderByDescending(x => x.CreatedDate).Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToList();

            return list.Select(n => (ArticleInfoView)new ArticleInfoView().ConvertAPIModel(n)).ToList();
            ////return list;
           
        }
        public List<ArticleInfoView> getMenuHistoryList(string MenuKey, PageCondition condition, out int totalRow) {
            var where = Repository.Entities.Where(a => a.ArticleCateSub == MenuKey && a.ArticleStatus == "Published"&&a.IsDeleted==false);


            totalRow = where.Count();

            var list = where.OrderByDescending(x => x.PublishDate).Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToList();
            return list.Select(n => (ArticleInfoView)new ArticleInfoView().ConvertAPIModel(n)).ToList();

        }
    }
   
}

