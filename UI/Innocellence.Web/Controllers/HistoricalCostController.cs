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
using Infrastructure.Utility.IO;

namespace DLYB.Web.Controllers
{
    public class HistoricalCostController : BaseController<HistoricalCost,HistoricalCostView>
    {
        private readonly IHistoricalCostService _service;
        private readonly ISysUserService _sysUserService;
        private readonly IProjectService _projectService;
        public HistoricalCostController(IHistoricalCostService service,ISysUserService sysUserService,
            IProjectService projectService) : base(service)
        {
            _service = service;
            _sysUserService = sysUserService;
            _projectService = projectService;
        }
        // GET: Address
        public override ActionResult Index()
        {
            string projectId = Request["projectId"];
            ViewBag.ProjectId = projectId;
            return View();
        }

        public ActionResult Template()
        {
            string downloadFileName = "历史消耗_Template.xlsx";
            string fileName = Server.MapPath("~/Files/Template/历史消耗.xlsx");
            string projectId = Request["projectId"];
            int pid = 0;
            if(!string.IsNullOrEmpty(projectId)&& int.TryParse(projectId,out pid))
            {
                var project = _projectService.GetList<ProjectView>(1, x => x.Id == pid).FirstOrDefault();
                downloadFileName = project == null ? downloadFileName : project.ProjectName + "_" + downloadFileName;
            }
            
            return File(fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", downloadFileName);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PostFile(HistoricalCostView objModal, string Id)
        {
            //验证错误
            if (!ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                objModal = objModal ?? new HistoricalCostView();
                objModal.HistoricalFile = file.FileName;
                var fileExtension = System.IO.Path.GetExtension(file.FileName);
                if (fileExtension.ToLower() != ".xls" && fileExtension.ToLower() != ".xlsx")
                {
                    var result = GetErrorJson();
                    result.Message = new JsonMessage(103, "请上传excel文件");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (!System.IO.Directory.Exists(Server.MapPath("/Files/HistoricalCost/")))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("/Files/HistoricalCost/"));
                }
                string path = "/Files/HistoricalCost/" + file.FileName;
                file.SaveAs(Server.MapPath(path));
                _service.InsertView(objModal);
                //if (string.IsNullOrEmpty(Id) || Id == "0")
                //{
                //    _service.InsertView(objModal);
                //}
                //else
                //{
                //    _service.UpdateView(objModal);
                //}
            }
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetDropdownList(string keyword = "")
        //{
        //    var list = _addressService.GetList<AddressView>(int.MaxValue, x => !x.IsDeleted && x.AddressName.Contains(keyword.Trim()))
        //        .Select(x => new { key = x.Id, value = x.AddressName }).ToList();
        //    return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            string projectId = Request["project_id"];
            int pid = string.IsNullOrEmpty(projectId) ? 0 : int.Parse(projectId);
            Expression<Func<HistoricalCost, bool>> expression = FilterHelper.GetExpression<HistoricalCost>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<HistoricalCost>(x =>x.ProjectId == pid && x.HistoricalFile.Contains(strCondition) && x.IsDeleted != true);
            }
            else
            {
                expression = expression.AndAlso<HistoricalCost>(x => x.ProjectId == pid && x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<HistoricalCostView> listEx = GetListEx(expression, gridRequest.PageCondition);
            var usersIds = listEx.Select(x => x.CreatedUserID).ToList();
            var users = _sysUserService.Repository.Entities.Where(x => usersIds.Contains(x.Id)).ToList();
            listEx.ForEach(x =>
            {
                if (users.Any(u => u.Id == x.CreatedUserID))
                {
                    x.CreatedUserName = users.FirstOrDefault(u => u.Id == x.CreatedUserID).UserName;
                }
            });
            return this.GetPageResult(listEx, gridRequest);
        }     
    }
}