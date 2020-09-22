using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Model;
using Innocellence.Web.Models;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.UI;
using Infrastructure.Utility.Filter;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;

namespace DLYB.Web.Controllers
{
    public class WeldCognitionController : BaseController<WeldCognition, WeldCognitionView>
    {
        private readonly IWeldCognitionService _service;
        public WeldCognitionController(IWeldCognitionService service) : base(service)
        {
            _service = service;            
        }
        // GET: Address
        public override ActionResult Index()
        {
            string fileId = Request["fileId"];
            string fileName = Request["fileName"];
            ViewBag.FileId = fileId;
            ViewBag.FileName = fileName;
            return View();
        }

        public JsonResult PostWeld(string dwgfile, List<WeldCognitionView> weldList)
        {
            if (string.IsNullOrEmpty(dwgfile) || weldList == null || weldList.Count == 0)
            {
                return new JsonResult { Data = new { result = "failed" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            var existing = _service.Repository.Entities.Where(x => x.FileName == dwgfile).ToList();
            if (existing.Count > 0)
            {
                _service.Repository.Delete(existing.Select(x => x.Id).ToArray());
            }
            foreach (var weldInfo in weldList)
            {
                if (weldInfo.HandleID > 0)
                {
                    weldInfo.FileName = dwgfile;
                    _service.InsertView(weldInfo);
                }
            }
            return new JsonResult { Data = new { result = "success" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<WeldCognition, bool>> expression = FilterHelper.GetExpression<WeldCognition>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<WeldCognition>(x => x.FileName.Contains(strCondition) && x.IsDeleted != true);
            }
            else
            {
                expression = expression.AndAlso<WeldCognition>(x => x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<WeldCognitionView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }

        public JsonResult SaveWelding(int dwgId, string dwgName, int handleId)
        {

            return new JsonResult { Data = new { result = "success" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        
    }
}