using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Entity;
using System.ComponentModel;

namespace DLYB.CA.ModelsView
{

    public partial class ArticleInfoView : IViewModel
    {

        public ArticleInfoView()
        {
            ArticleContentViews = new List<ArticleContentView>();
            List = new List<ArticleInfoView>();
        }

        public Int32 Id { get; set; }
        [DescriptionAttribute("文章标题")]
        public String ArticleTitle { get; set; }
        public String LanguageCode { get; set; }
        public String ArticleComment { get; set; }
        public String ArticleContent { get; set; }
        public String ArticleContentEdit { get; set; }
       
        public String ArticleCateSub { get; set; }

        public List<ArticleContentView> ArticleContentViews { get; set; }
        public String[] ArticleContents { get; set; }
        public String[] VideoUrl { get; set; }
        public String[] ImgUrl { get; set; }
        public String[] ImgUrl_Old { get; set; }
        public String[] ImgID { get; set; }

        public Guid? ArticleCode { get; set; }
        public Int32? AppId { get; set; }
        public String ArticleURL { get; set; }

         [DescriptionAttribute("点赞数")]
        public Int32? ThumbsUpCount { get; set; }
        public String ImageName { get; set; }
        public String ImageUrl { get; set; }

        [DescriptionAttribute("菜单名称")]
        public String ArticleCateName { get; set; }
        public String ArticleStatus { get; set; }

        [DescriptionAttribute("阅读数")]
        public Int32? ReadCount { get; set; }

        public byte[] ImageContent { get; set; }
        public byte[] ImageContent_T { get; set; }
        public string ImageContentBase64 { get; set; }

        public Boolean? IsDeleted { get; set; }
        public Boolean? IsLike { get; set; }
        public Boolean? IsTransmit { get; set; }
        public Boolean? IsPassingLillyID { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

         [DescriptionAttribute("发布日期")]
        public DateTime? PublishDate { get; set; }

        public String CreatedUserID { get; set; }
        public String UpdatedUserID { get; set; }

        public IList<ArticleInfoView> List { get; set; }
        public Int32? ThumbImageId { get; set; }
        public String ThumbImageUrl { get; set; }
        public String Role { get; set; }
        public String Previewers { get; set; }
        public String APPName { get; set; }
        public bool IsThumbuped { get; set; }

        public IList<ArticleThumbsUpView> ArticleThumbsUpViews { get; set; }

        public List<ArticleInfoView> ArticleHistoryList { get; set; }
        public ArticleImagesView ImageHistoryList { get; set; }
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (ArticleInfo)obj;
            Id = entity.Id;
            ArticleTitle = entity.ArticleTitle;
            LanguageCode = entity.LanguageCode;
            ArticleComment = entity.ArticleComment;
            ArticleContent = entity.ArticleContent;
            ArticleCode = entity.ArticleCode;
            ArticleURL = entity.ArticleURL;
            ThumbsUpCount = entity.ThumbsUpCount;
            ThumbImageId = entity.ThumbImageId;
            ThumbImageUrl = entity.ThumbImageUrl;
            AppId = entity.AppId;
            ArticleStatus = entity.ArticleStatus;
            ReadCount = entity.ReadCount;
            ArticleCateSub = entity.ArticleCateSub;
            IsDeleted = entity.IsDeleted;
            IsLike = entity.IsLike;
            IsTransmit = entity.IsTransmit;
            IsPassingLillyID = entity.IsPassingLillyID;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            PublishDate = entity.PublishDate;
            Role = entity.Role;
            Previewers = entity.Previewers;

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
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return this;
        }


    }
}
