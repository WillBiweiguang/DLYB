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
using System.IO;
using NPOI.XSSF.UserModel;

namespace DLYB.Web.Controllers
{
    public class TaskListController : BaseController<TaskList, TaskListView>
    {
        private readonly ITaskListService _TaskListService;
        const string templateExcelFilename = "/content/焊材统计表.xlsx";
        private readonly IWeldCategoryLabelingService _weldCategoryService;
        public TaskListController(ITaskListService TaskListService,
            IWeldCategoryLabelingService weldCategoryService) : base(TaskListService)
        {
            _TaskListService = TaskListService;
            _weldCategoryService = weldCategoryService;
        }
        // GET: Address
        public override ActionResult Index()
        {
            ViewBag.list = _weldCategoryService.Repository.Entities.Where(a => !a.IsDeleted).ToList();
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
        public ActionResult ExportToExcel()
        {
            string fileName =  "焊材_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";

            string templateFilename = Server.MapPath(templateExcelFilename);
            using (FileStream file = new FileStream(templateFilename, FileMode.Open, FileAccess.Read))
            {
                var workbook = new XSSFWorkbook(file);
                var sheet1 = workbook.GetSheet("焊材");

                // 导出 答卷 
                //var answer = _pollingResultService.GetList(Id);
                var reportList1 = _weldCategoryService.Repository.Entities.Where(a => !a.IsDeleted).ToList();
                int i = 1;
                foreach (var v in reportList1)
                {
                    int j = 0;
                    var row = sheet1.CreateRow(i++);
                    row.CreateCell(j++).SetCellValue(i-1);
                    row.CreateCell(j++).SetCellValue(v.BeamId);
                    row.CreateCell(j++).SetCellValue(v.BeamId);
                    row.CreateCell(j++).SetCellValue(v.FigureNumber);
                    row.CreateCell(j++).SetCellValue(v.BoardNumber);
                    row.CreateCell(j++).SetCellValue(v.WeldType);
                    row.CreateCell(j++).SetCellValue(v.Thickness);
                    row.CreateCell(j++).SetCellValue(v.WeldLocation);
                    row.CreateCell(j++).SetCellValue(v.ConsumeFactor);
                    row.CreateCell(j++).SetCellValue(v.SectionArea);
                    row.CreateCell(j++).SetCellValue(v.WeldLength);
                    row.CreateCell(j++).SetCellValue(v.WeldQuanlity);
                    row.CreateCell(j++).SetCellValue(v.WeldingNumber);
                    row.CreateCell(j++).SetCellValue(v.Quantity);
                    row.CreateCell(j++).SetCellValue(v.BeamNum);
                    row.CreateCell(j++).SetCellValue(v.ConsumeFactor*v.WeldQuanlity);

                }
                
                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    ms.Flush();
                    ms.Position = 0;
                    //sheet = null;
                    //workbook = null;
                    //workbook.Close();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet
                    return File(ms.ToArray(), "application/vnd-excel", fileName);
                }

            }

        }
    }
}