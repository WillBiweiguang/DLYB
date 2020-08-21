using Infrastructure.Core;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;


namespace DLYB.CA.ModelsView
{
    public class QuestionSubView : IViewModel
    {
        public Int32 Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public Int32 QuestionId { get; set; }
        public string CreatedDate { get; set; }
     
        public List<QuestionSubView> List { get; set; }
        public List<QuestionImagesView> QuestionImages { get; set; }
        public string ImageIdList { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (QuestionSub)obj;
            Id = entity.Id;
            UserName = entity.UserName;
            Content = entity.Content;
            CreatedDate = entity.CreatedDate == null ? "" : ((DateTime)entity.CreatedDate).ToString("yyyy-MM-dd HH:mm"); 
            QuestionId = entity.QuestionId;
            Type = entity.Type;
            return this;
        }
    }
}
