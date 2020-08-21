using System;
using System.Collections.Generic;
using Infrastructure.Core;
using Innocellence.CA.Contracts.Entity;
using Infrastructure.Utility.Data;

namespace Innocellence.CA.Contracts.ViewModel
{
    public class PollingView : IViewModel
    {
        public int Id { get; set; }
        public Guid GuiId { get; set; }
        public string AppId { get; set; }
        public string Name { get; set; }
        public int? Type { get; set; }
        public string Status { get; set; }
        public string PollingHtml { get; set; }

        public int? AwardNumber { get; set; }
        public int? StandardScore { get; set; }
        public string ReplyMessage { get; set; }

        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }

        public string Title { get; set; }
        public string StatusName { get; set; }
        public string EventId { get; set; }

        public IList<PollingQuestionView> PollingQuestions { get; set; }
        
        public int PollingTotal { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var obj = (PollingEntity)model;
            Id = obj.Id;
            GuiId = obj.GuiId;
            AppId = obj.AppId;
            Name = obj.Name;
            Type = obj.Type;
            Status = obj.Status;
            AwardNumber = obj.AwardNumber;
            StandardScore = obj.StandardScore;
            ReplyMessage = obj.ReplyMessage;
            PollingHtml = obj.PollingHtml;
            StartDateTime = obj.StartDateTime;
            EndDateTime = obj.EndDateTime;
            IsDeleted = obj.IsDeleted;
            PollingQuestions = new List<PollingQuestionView>();
            
            return this;
        }
    }

    public class PollingQuestionView : IViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Type { set; get; }
        public int PollingId { get; set; }
        public bool IsRequired { get; set; }
        public string Description { get; set; }
        public string RightAnswers { get; set; }
        public int? Score { get; set; }
        public Int32? OrderIndex { get; set; }
        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }
        public IList<PollingOptionView> PollingOptionEntities { get; set; }
        public string PollingQuestionResult { get; set; }
        public string optionName { get; set; }
        public int rightPersons { get; set; }
        public int answerPersons { get; set; }
        public IViewModel ConvertAPIModel(object model)
        {
            var obj = (PollingQuestionEntity)model;
            Id = obj.Id;
            Title = obj.Title;
            Type = obj.Type;
            PollingId = obj.PollingId;
            OrderIndex = obj.OrderIndex;
            IsRequired = obj.IsRequired;
            Description = obj.Description;
            RightAnswers = obj.RightAnswers;
            Score = obj.Score;
            IsDeleted = obj.IsDeleted;
            PollingOptionEntities = new List<PollingOptionView>();
            PollingQuestionResult = "";
            return this;
        }
    }

    public class PollingOptionView : IViewModel
    {
        public int Id { get; set; }
        public string OptionName { get; set; }
        public int QuestionId { get; set; }
        public string Picture { get; set; }
        public string Type { get; set; }
        public int Percent { get; set; }
        public int VoteNum { get; set; }
        public Int32? OrderIndex { get; set; }
        public string SelectName { get; set; }
        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }
        //存type和isRequired
        public OptionType optiontype { get; set; }
     

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (PollingOptionEntity)model;
            Id = entity.Id;
            OptionName = entity.OptionName;
            QuestionId = entity.QuestionId;
            OrderIndex = entity.OrderIndex;
            Picture = entity.Picture;
            Type = entity.Type;
            IsDeleted = entity.IsDeleted;
            optiontype = !string.IsNullOrEmpty(entity.Type) ? JsonHelper.FromJson<OptionType>(entity.Type) : new OptionType();
            return this;
        }
    }

    public class OptionType
    {
        public string type { get; set; }
        public string isRequired { get; set; }
    }

    public class AnswerResult
    {
        public int Id { get; set; }
        public int PollingId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionName { get; set; }
        public int Answer { get; set; }
        public string AnswerText { get; set; }//填空题答案放入
        public int AnswerPicture { get; set; }//备用
    }

    public class PollingReslutView : IViewModel
    {
        public int Id { get; set; }
        public int PollingId { get; set; }
        public IList<AnswerResult> AnswerResults { get; set; }
        public string UserId { get; set; }
        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (PollingResultEntity)model;
            Id = entity.Id;
            PollingId = entity.PollingId;
            UserId = entity.UserId;
            return this;
        }
    }

    public class PollingResultExportView : IViewModel
    {
        public int Id { get; set; }
        public int PollingId { get; set; }
        public int QuestionId { get; set; }
        public int Answer { get; set; }
        public string AnswerText { get; set; }//填空题答案放入
        public int? AnswerPicture { get; set; }//备用

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string QuestionName { get; set; }

        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsDeleted { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (PollingResultEntity)model;

            Id = entity.Id;
            UserId = entity.UserId;
            PollingId = entity.PollingId;
            QuestionId = entity.QuestionId;
            Answer = entity.Answer;
            AnswerText = entity.AnswerText;
            AnswerPicture = entity.AnswerPicture;
            QuestionName = entity.QuestionName;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            IsDeleted = entity.IsDeleted;

            return this;
        }
    }

}
