using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infrastructure.Web.Domain.Entity;
namespace Infrastructure.Web.Domain.ModelsView
{
    public class ArticleContentView
    {
        /// <summary>
        /// 文章的图片
        /// </summary>
        public int? ImageID { get; set; }

        /// <summary>
        /// 文章中的视频
        /// </summary>
        public String ImageUrl { get; set; }

        [JsonIgnore]
        public ArticleImages objImage { get; set; }

        public String VideoUrl { get; set; }

        /// <summary>
        /// 文章的一段内容，一般是位于两个图片之间的内容
        /// </summary>
        public String ArticleParamContent { get; set; }

        public String[] ArticleParamContentList
        {
            get
            {
                return ArticleParamContent.Split(new[] { "\r\n" }, StringSplitOptions.None);
            }
            private set { }
        }
    }
}