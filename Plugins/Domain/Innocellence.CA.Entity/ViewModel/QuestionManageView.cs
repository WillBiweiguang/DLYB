using Infrastructure.Core;
using DLYB.CA.Contracts.ViewModel;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace DLYB.CA.ModelsView
{
    public class QuestionManageView : IViewModel
    {

        public Int32 Id { get; set; }
        public Int32? AppId { get; set; }
        [DescriptionAttribute("类别")]
        public string Category { get; set; }

        //提问者id
        [DescriptionAttribute("创建者")]
        public string CreatedUserId { get; set; }
        //提问者name
        [DescriptionAttribute("提问者")]
        public string QUserName { get; set; }

        //回答者id
        [DescriptionAttribute("更新者")]
        public string UpdatedUserId { get; set; }
        //回答者name
        [DescriptionAttribute("回答者")]
        public string AUsername { get; set; }

        //问题描述
        [DescriptionAttribute("问题")]
        public string Question { get; set; }
        //问题答复
        [DescriptionAttribute("答案")]
        public string Answer { get; set; }
        //提问时间
        [DescriptionAttribute("提问时间")]
        public DateTime? CreatedDate { get; set; }
        //修改时间
        [DescriptionAttribute("修改时间")]
        public DateTime? UpdatedDate { get; set; }
        //回答时间
        [DescriptionAttribute("回答时间")]
        public DateTime? AnswerDate { get; set; }
        //问题状态
        [DescriptionAttribute("问题状态")]
        public string Status { get; set; }
        //阅读次数
        [DescriptionAttribute("阅读次数")]
        public Int32? ReadCount { get; set; }
        //删除标识
        public Boolean? IsDeleted { get; set; }
         [DescriptionAttribute("满意度")]
        public Int32? Satisfaction { get; set; }
        public string Tel { get; set; }
        public string EMail { get; set; }
       
        public List<QuestionManageView> List { get; set; }
        public List<QuestionSubView> SubList { get; set; }
        public List<QuestionImagesView> QuestionImages { get; set; }
        public string ImageIdList { get; set; }

        public QuestionConfig questionConfig { get; set; }

        public QAFeedbackConfig QAFBConfig { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (QuestionManage)obj;
            Id = entity.Id;
            AppId = entity.AppId;
            Category = entity.Category;
            CreatedUserId = entity.CreatedUserId;
            QUserName = entity.QUserName;
            UpdatedUserId = entity.UpdatedUserId;
            AUsername = entity.AUsername;
            Question = entity.Question;
            Answer = entity.Answer;
            CreatedDate = entity.CreatedDate; 
            UpdatedDate = entity.UpdatedDate; 
            AnswerDate = entity.AnswerDate;
            Status = entity.Status;
            ReadCount = entity.ReadCount;
            IsDeleted = entity.IsDeleted;
            Satisfaction = entity.Satisfaction;
            return this;
        }
    }
}
