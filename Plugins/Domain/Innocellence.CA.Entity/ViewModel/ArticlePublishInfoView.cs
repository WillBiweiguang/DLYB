using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Entity;

namespace DLYB.CA.ModelsView
{
    //[Table("ArticleInfoView")]

    public partial class ArticlePublishInfoView : IViewModel
    {
        public  Int32 Id { get; set; }

        public int ArticleID { get; set; }
        public String Title { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public String toDepartment { get; set; }
        public String toTag { get; set; }
        public String toUser { get; set; }
        public String invaildDepartment { get; set; }
        public String invaildTag { get; set; }
        public String invaildUser { get; set; }
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (ArticlePublishInfo)obj;
            Id = entity.Id;
            ArticleID = entity.ArticleID;
            Title = entity.Title;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            toDepartment = entity.toDepartment;
            toTag = entity.toTag;
            toUser = entity.toUser;
            invaildDepartment = entity.invaildDepartment;
            invaildTag = entity.invaildTag;
            invaildUser = entity.invaildUser;


            return this;
        }


    }
}
