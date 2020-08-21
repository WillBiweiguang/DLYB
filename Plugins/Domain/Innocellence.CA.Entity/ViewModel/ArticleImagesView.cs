using Infrastructure.Core;
using DLYB.CA.Entity;
using System;

namespace DLYB.CA.ModelsView
{
    public class ArticleImagesView : IViewModel
    {
        public Int32 Id { get; set; }

        public String ImageType { get; set; }
        
        public String ImageName { get; set; }   

        public Int32? ArticleID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public String UploadedUserId { get; set; }

        public string CreatedUserID { get; set; }

        public Int32? AppId { get; set; }

        public string CreatedUserName { get; set; }
        
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (ArticleImages)obj;
            Id = entity.Id;
            ImageType = entity.ImageType;
            ImageName = entity.ImageName;
            ArticleID = entity.ArticleID;
            CreatedDate = entity.CreatedDate;
            UploadedUserId = entity.UploadedUserId;
            CreatedUserID = entity.CreatedUserID;
            AppId = entity.AppId;
            return this;
        }
    }
}
