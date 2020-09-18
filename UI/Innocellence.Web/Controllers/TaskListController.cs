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
    public class TaskListController : BaseController<TaskList, TaskListView>
    {
        private readonly ITaskListService _TaskListService;
        public TaskListController(ITaskListService TaskListService) : base(TaskListService)
        {
            _TaskListService = TaskListService;
        }
        // GET: Address
        public override ActionResult Index()
        {
            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<TaskList, bool>> expression = FilterHelper.GetExpression<TaskList>(gridRequest.FilterGroup);

            expression = expression.AndAlso<TaskList>(x => x.IsDeleted != true);

            int rowCount = gridRequest.PageCondition.RowCount;
            List<TaskListView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PostFile(TaskListView objModal,int ProjectName)
        {
            //验证错误
            if (!ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                objModal = objModal ?? new TaskListView();
                objModal.DWGFile = file.FileName;
                objModal.ProjectId = ProjectName;
                var fileExtension = System.IO.Path.GetExtension(file.FileName);
                if (fileExtension.ToLower() != ".dwg")
                {
                    var result = GetErrorJson();
                    result.Message = new JsonMessage(103, "请上传dwg文件");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (!System.IO.Directory.Exists(Server.MapPath("/Files/TaskList/")))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("/Files/TaskList/"));
                }
                string path = "/Files/TaskList/" + file.FileName;
                file.SaveAs(Server.MapPath(path));
                _TaskListService.InsertView(objModal);
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
    }
}