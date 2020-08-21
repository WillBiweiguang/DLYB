using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
namespace Infrastructure.Web.Domain.Entity
{
	//[Table("NewsInfo")]
    public partial class ArticleInfo : EntityBase<int>
	{
	
		//[Id("Id",IsDbGenerated=true)]
        public override Int32 Id { get; set; }
 

        //Title of the news
		//[Column("NewsTitle",DbType=DBType.NVarChar,Length=2048,Precision=1024,IsNullable=true)]
        public String ArticleTitle { get; set; }

        //
		//[Column("NewsTitleEN",DbType=DBType.VarChar,Length=1024,Precision=1024,IsNullable=true)]
        public String LanguageCode { get; set; }

        private String articleContent = string.Empty;
        //Content of the news
		//[Column("NewsContent",DbType=DBType.NVarChar,Length=-1,Precision=-1,IsNullable=true)]
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

        //
		//[Column("NewsContentEN",DbType=DBType.VarChar,Length=-1,Precision=-1,IsNullable=true)]
        public Guid? ArticleCode { get; set; }

        //
		//[Column("NewsComment",DbType=DBType.NVarChar,Length=2048,Precision=1024,IsNullable=true)]
        public String ArticleComment { get; set; }

        //
		//[Column("NewsCommentEN",DbType=DBType.VarChar,Length=1024,Precision=1024,IsNullable=true)]
		//public String NewsCommentEN { get;set; }

        //the category of the news
		//[Column("NewsCate",DbType=DBType.Int32,Length=4,Precision=10,IsNullable=true)]
        public Int32? ArticleCate { get; set; }
        public Int32? ArticleCateSub { get; set; }
        

        //0add 1publisthed
		//[Column("NewsStatus",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
        public String ArticleStatus { get; set; }

        //the count of the read
		//[Column("ReadCount",DbType=DBType.Int32,Length=4,Precision=10,IsNullable=true)]
		public Int32? ReadCount { get;set; }
        public Int32? ThumbsUpCount { get; set; }
        

        //Course Delete flag
		//[Column("IsDeleted",DbType=DBType.Boolean,Length=1,Precision=1,IsNullable=true)]
		public Boolean? IsDeleted { get;set; }


     //   public byte[] ImageContent { get; set; }

        public byte[] ImageContent_T { get; set; }

        //
		//[Column("CreatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
		public DateTime? CreatedDate { get;set; }

        //
		//[Column("CreatedUserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
		public String CreatedUserID { get;set; }

        //
		//[Column("UpdatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
		public DateTime? UpdatedDate { get;set; }

        //
		//[Column("UpdatedUserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
		public String UpdatedUserID { get;set; }

        //
		//[Column("PublishDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
		public DateTime? PublishDate { get;set; }
 
 
		//[OneToMany(ThisKey="Id",OtherKey="NewsID")]
		//public IList<NewsImages> NewsImages { get;set; }

    }
}
