using Infrastructure.Core;
using Infrastructure.Core.Data;
using DLYB.CA.Contracts;
using DLYB.CA.Entity;
using System.Collections.Generic;
using System.Linq;
using DLYB.CA.ModelsView;


namespace DLYB.CA.Services
{
    /// <summary>
    /// 业务实现——在线问答
    /// </summary>
    public partial class QuestionSubService : BaseService<QuestionSub>, IQuestionSubService
    {
        public QuestionSubService()
            : base("CAAdmin")
        {

        }
        //搜出图片列表
        public List<T> GetListByQuestionID<T>(int questionId) where T : IViewModel, new()
        {

            var ens = Repository.Entities.Where(a => a.QuestionId == questionId).OrderBy(a => a.CreatedDate).ToList();

            var lst = ens.Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
        }
        public int GetSubId(int id)
        {
            var subId = Repository.Entities.Where(a => a.QuestionId == id && a.Type == "Q").OrderByDescending(a => a.CreatedDate).FirstOrDefault();
            return subId.Id;
        }
        public override int InsertView<T>(T objModalSrc)
        {
            var objView = objModalSrc as QuestionSubView;
            if (objView == null)
            {
                return -1;
            }

            var questionSub = objView.MapTo<QuestionSub>();
            int iRet = Repository.Insert(questionSub);
            objView.Id = questionSub.Id;
            
            var ser = new BaseService<QuestionImages>("CAAdmin");
            if (!string.IsNullOrEmpty(objView.ImageIdList))
            {
                foreach (var imageid in objView.ImageIdList.Split(','))
                {
                    if (!string.IsNullOrEmpty(imageid))
                    {
                        var qt = new QuestionImages {Id = int.Parse(imageid), QuestionID = objView.Id};
                        ser.Repository.Update(qt, new List<string>() { "QuestionID" });
                    }
                }
            }
            return iRet;
        }

    }
}