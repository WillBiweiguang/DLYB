using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;


namespace Infrastructure.Web.Domain.ModelsView
{
    //[Table("ArticleInfoView")]

    public partial class ArticleInfoView : IViewModel
    {

        public Int32 Id { get; set; }

        public String ArticleTitle { get; set; }

        public String LanguageCode { get; set; }
        public String ArticleContent { get; set; }

        public string ArticleContentEdit { get; set; }

        public Int32? ArticleCateSub { get; set; }

        public List<ArticleContentView> ArticleContentViews { get; set; }
        public String[] ArticleContents { get; set; }

        public String[] VideoUrl { get; set; }

        public String[] ImgUrl { get; set; }

        public String[] ImgUrl_Old { get; set; }

        public String[] ImgID { get; set; }


        public Guid? ArticleCode { get; set; }
        public Int32? ArticleCate { get; set; }

        public String ArticleURL { get; set; }
        public Int32? ThumbsUpCount { get; set; }

        public String ImageName { get; set; }
        public String ImageUrl { get; set; }
        public String ArticleCateName { get; set; }
        public String ArticleStatus { get; set; }
        public Int32? ReadCount { get; set; }
        public Boolean? IsDeleted { get; set; }
        public byte[] ImageContent { get; set; }

        public byte[] ImageContent_T { get; set; }
        public string ImageContentBase64 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String UpdatedUserID { get; set; }
        public DateTime? PublishDate { get; set; }
        public IList<ArticleInfoView> List { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (ArticleInfo)obj;
            Id = entity.Id;
            ArticleTitle = entity.ArticleTitle;
            LanguageCode = entity.LanguageCode;
            ArticleContent = entity.ArticleContent;
            ArticleCode = entity.ArticleCode;
            ArticleURL = entity.ArticleURL;
            ThumbsUpCount = entity.ThumbsUpCount;
            ImageContent_T = entity.ImageContent_T;
            ArticleCate = entity.ArticleCate;
            ArticleStatus = entity.ArticleStatus;
            ReadCount = entity.ReadCount;
            ArticleCateSub = entity.ArticleCateSub;
            // ImageName = entity.ImageName;
            //IsDeleted =entity.IsDeleted;
            // ImageID =entity.ImageID;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            // UpdatedDate =entity.UpdatedDate;
            // UpdatedUserID =entity.UpdatedUserID;
            PublishDate = entity.PublishDate;

            if (!string.IsNullOrEmpty(entity.ArticleContentEdit))
            {
                ArticleContentViews = Infrastructure.Utility.Data.JsonHelper.FromJson<List<ArticleContentView>>(entity.ArticleContentEdit);
            }
            else
            {
                ArticleContentViews = new List<ArticleContentView>();
            }


            try
            {
               // ArticleContentStructures = Infrastructure.Utility.Data.JsonHelper.FromJson<List<ArticleContentStructure>>(entity.ArticleContent);
            }
            catch(Exception ex)
            {
                string s = ex.Message;
            }
            return this;
        }

    }
}
