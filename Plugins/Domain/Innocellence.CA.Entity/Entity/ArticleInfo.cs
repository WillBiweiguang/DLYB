using System;
using System.Collections.Generic;
using Infrastructure.Core;
namespace DLYB.CA.Entity
{
	//[Table("NewsInfo")]
    public class ArticleInfo : EntityBase<int>
	{
        public override Int32 Id { get; set; }
        public Int32? AppId { get; set; }
        public String ArticleTitle { get; set; }
        public String LanguageCode { get; set; }

        private String articleContent = string.Empty;
        public String ArticleContent
        {
            get
            {
                return System.Web.HttpUtility.UrlDecode(articleContent);
            }
            set
            {
                articleContent = System.Web.HttpUtility.UrlEncode(value);
            }
        }
        public string ArticleContentEdit { get; set; }

        public String ArticleURL { get; set; }
        public Guid? ArticleCode { get; set; }
        public String ArticleComment { get; set; }
        public String ArticleCateSub { get; set; }
        public String ArticleStatus { get; set; }

		public Int32? ReadCount { get;set; }
        public Int32? ThumbsUpCount { get; set; }
        public Int32? ThumbImageId { get; set; }
        public String ThumbImageUrl { get; set; }

        public DateTime? PublishDate { get; set; }
		public DateTime? CreatedDate { get;set; }
        public DateTime? UpdatedDate { get; set; }
		public String CreatedUserID { get;set; }
		public String UpdatedUserID { get;set; }
        public Boolean? IsDeleted { get; set; }

        public String Role { get; set; }
        public String Previewers { get; set; }
        public Boolean? IsLike { get; set; }
        public Boolean? IsTransmit { get; set; }
        public Boolean? IsPassingLillyID { get; set; }

    }
}
