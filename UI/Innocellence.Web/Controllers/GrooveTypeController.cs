using DLYB.Web.Controllers;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Innocellence.Web.Controllers
{
    public class GrooveTypeController : BaseController<GrooveTypes, GrooveTypeView>
    {
        private readonly IGrooveTypeService _GrooveTypeService;

        public GrooveTypeController(IGrooveTypeService grooveTypeService) : base(grooveTypeService)
        {
            _GrooveTypeService = grooveTypeService;
        }
        // GET: Address
        public override ActionResult Index()
        {
            //var list = _addressService.GetList<AddressView>(int.MaxValue, x => !x.IsDeleted).ToList();
                 
            return View();
        }
        public ActionResult Calculate()
        {
            //ViewBag.GrooveTypes = _GrooveTypeService.GetList<GrooveTypeView>(int.MaxValue, x => !x.IsDeleted).ToList();
            ViewBag.GrooveTypes = _GrooveTypeService.GetGrooveTypeQuerys();
            return View();
        }
        public ActionResult GetGrooveTypeSearchQuerys()
        {
            var list = _GrooveTypeService.GetGrooveTypeQuerys();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<GrooveTypes, bool>> expression = FilterHelper.GetExpression<GrooveTypes>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<GrooveTypes>(x => x.GrooveType.Contains(strCondition) && x.IsDeleted != true);
            }
            else
            {
                expression = expression.AndAlso<GrooveTypes>(x => x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<GrooveTypeView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }

        public JsonResult GetDropdownList(string keyword = "")
        {
            var list = _GrooveTypeService.GetList<GrooveTypeView>(int.MaxValue, x => !x.IsDeleted && x.GrooveType.Contains(keyword.Trim()))
                        .Select(x => new { key = x.Id, value = x.GrooveType }).ToList();
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PostFile(GrooveTypeView objModal, int ProjectName)
        {
            //验证错误
            if (!ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                objModal = objModal ?? new GrooveTypeView();
                objModal.PreviewImage = "/Content/GrooveImage/" + file.FileName;
                objModal.Id = ProjectName;
                var fileExtension = System.IO.Path.GetExtension(file.FileName);
                if (fileExtension.ToLower() != ".png")
                {
                    var result = GetErrorJson();
                    result.Message = new JsonMessage(103, "请上传png文件");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (!System.IO.Directory.Exists(Server.MapPath("/Content/GrooveImage/")))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("/Content/GrooveImage/"));
                }
                string path = "/Content/GrooveImage/" + file.FileName;
                file.SaveAs(Server.MapPath(path));
                if (ProjectName ==0)
                {
                    _GrooveTypeService.InsertView(objModal);
                }
                else
                {
                    _GrooveTypeService.UpdateView(objModal);
                }
            }
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }
    }
}