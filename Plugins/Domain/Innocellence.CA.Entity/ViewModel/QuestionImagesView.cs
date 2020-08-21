using Infrastructure.Core;
using DLYB.CA.Entity;
using System;

namespace DLYB.CA.ModelsView
{
    public class QuestionImagesView: IViewModel
    {
        public Int32 Id { get; set; }

        public String ImageType { get; set; }
        
        public String ImageName { get; set; }

        public Int32? QuestionID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public String CreatedUserID { get; set; }

        public Int32? AppId { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (QuestionImages)obj;
            Id = entity.Id;
            ImageType = entity.ImageType;
            ImageName = entity.ImageName;
            QuestionID = entity.QuestionID;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            AppId = entity.AppId;

            return this;
        }
    }
}
