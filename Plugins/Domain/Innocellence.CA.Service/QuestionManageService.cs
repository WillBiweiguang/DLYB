// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Core.Logging;
using DLYB.CA.Contracts;
using DLYB.CA.Entity;
using DLYB.CA.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Threading.Tasks;


namespace DLYB.CA.Services
{
    /// <summary>
    /// 业务实现——在线问答
    /// </summary>
    public partial class QuestionManageService : BaseService<QuestionManage>, IQuestionManageService
    {
        private readonly IQuestionSubService _objQuetisonSubService = new QuestionSubService();
        private readonly IQuestionImagesService _objImageService = new QuestionImagesService();
        public QuestionManageService(IUnitOfWork unitOfWork)
            : base("CAAdmin")
        {


        }

        //public QuestionManageService()
        //    : base()
        //{

        //}

        public List<T> GetQuestionList<T>(Expression<Func<QuestionManage, bool>> predicate) where T : IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).OrderByDescending(m => m.CreatedDate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }

        //根据QUserId显示列表
        public List<T> GetListByQUserId<T>(int appid, string qUserId, string category = null) where T : IViewModel, new()
        {
            Expression<Func<QuestionManage, bool>> predicate;
            if (category == null)
            {
                predicate =
                    a => a.AppId == appid && a.IsDeleted == false && a.QUserName.Equals(qUserId, StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                predicate = a => a.AppId == appid && a.IsDeleted == false && a.QUserName.Equals(qUserId, StringComparison.InvariantCultureIgnoreCase)
                    && a.Category == category;
            }

            var lst = Repository.Entities.Where(predicate).OrderByDescending(m => m.CreatedDate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }
        public QuestionManageView GetQuestionDetail(string id, QuestionManage obj)
        {
            var questionView = (QuestionManageView)(new QuestionManageView().ConvertAPIModel(obj));

            var qlst = _objQuetisonSubService.GetListByQuestionID<QuestionSubView>(int.Parse(id));

            qlst.ForEach(x =>
            {
                if (x.Type.Equals("Q"))
                {
                    x.QuestionImages = _objImageService.GetListByQuestionID<QuestionImagesView>(x.Id);
                }
            });

            questionView.SubList = qlst;

            return questionView;
        }
        public QuestionManageView GetFrontQuestionDetail(int id, QuestionManage obj)
        {
            var question = Repository.GetByKey(id);

            if (question == null)
            {
                return null;
            }

            var qlst = _objQuetisonSubService.GetListByQuestionID<QuestionSubView>(id);

            qlst.ForEach(x =>
            {
                if (x.Type.Equals("Q"))
                {
                    x.QuestionImages = _objImageService.GetListByQuestionID<QuestionImagesView>(x.Id);
                }
            });

            question.ReadCount++;
            Repository.Update(question, new List<string>() { "ReadCount" });

            var questionView = (QuestionManageView)(new QuestionManageView().ConvertAPIModel(question));
            questionView.SubList = qlst;
            return questionView;
        }
        public override int InsertView<T>(T objModalSrc)
        {
            int iRet;
            var objView = objModalSrc as QuestionManageView;
            var question = objView.MapTo<QuestionManage>();
            if (objView == null)
            {
                return -1;
            }
            question.ReadCount = 0;
            iRet = Repository.Insert(question);
            objView.Id = question.Id;

            //冗余主表记录到子表里
            var questionSubView = new QuestionSubView()
            {
                Type = "Q",
                QuestionId = objView.Id,
                Content = objView.Question,
                UserName = objView.QUserName,
                ImageIdList = objView.ImageIdList
            };

            _objQuetisonSubService.InsertView(questionSubView);
            return iRet;
        }

        public override int UpdateView<T>(T objModalSrc)
        {
            int iRet;
            var objModal = objModalSrc as QuestionManageView;

            if (objModal == null)
            {
                return -1;
            }
            int Id = objModal.Id;
            var question = Repository.Entities.Where(x => x.Id == Id).AsNoTracking().FirstOrDefault();
            //是否更新主表(只有未回答或者未开单的时候才更新回答者和回答日期，否则只更新状态)
            if (question != null && question.Status.IndexOf("未") > -1)
            {
                objModal.AnswerDate = DateTime.Now;

                var lst = new List<string>() { 
                    "Status","AnswerDate","AUsername","Answer"};
                base.UpdateView(objModal, lst);
            }
            else
            {
                var lst = new List<string>() { 
                    "Status"};
                base.UpdateView(objModal, lst);
            }
            //冗余主表记录到子表里
            var questionSubView = new QuestionSubView()
            {
                Type = "A",
                QuestionId = objModal.Id,
                Content = objModal.Answer,
                UserName = objModal.AUsername
            };

            if (questionSubView == null)
            {
                return -1;
            }
            iRet = _objQuetisonSubService.InsertView(questionSubView);
            return iRet;
        }

        public void UpdateStatus(QuestionManageView obj)
        {
            var objModal = obj as QuestionManageView;

            if (objModal == null)
            {
                return;
            }
            int Id = objModal.Id;
            var question = Repository.Entities.Where(x => x.Id == Id).AsNoTracking().FirstOrDefault();
            //是否更新主表(只有未回答或者未开单的时候才更新回答者和回答日期，否则只更新状态)
            if (question != null && question.Status.IndexOf("未") > -1)
            {
                objModal.AnswerDate = DateTime.Now;

                var lst = new List<string>() { 
                    "Status","AnswerDate","AUsername","Answer"};
                base.UpdateView(objModal, lst);
            }
            else
            {
                var lst = new List<string>() { 
                    "Status"};
                base.UpdateView(objModal, lst);
            }
        }
    }
}