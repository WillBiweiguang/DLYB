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
using DLYB.Web.Controllers;
using Infrastructure.Web.Domain.Services;

namespace Innocellence.Web.Controllers
{
    public class BeamInfoController : BaseController<BeamInfo, BeamInfoView>
    {
        private readonly IBeamInfoService _beamInfoService;
        private readonly IProjectService _projectService;

        public BeamInfoController(IBeamInfoService beamInfoService, IProjectService projectService) : base(beamInfoService)
        {
            _beamInfoService = beamInfoService;
            _projectService = projectService;
        }
        // GET: Address
        public override ActionResult Index()
        {
            //var list = _addressService.GetList<AddressView>(int.MaxValue, x => !x.IsDeleted).ToList();

            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            Expression<Func<BeamInfo, bool>> expression = FilterHelper.GetExpression<BeamInfo>(gridRequest.FilterGroup);

            expression = expression.AndAlso<BeamInfo>(x => x.IsDeleted != true);

            int rowCount = gridRequest.PageCondition.RowCount;
            List<BeamInfoView> listEx = GetListEx(expression, gridRequest.PageCondition);
            var projectIDs = listEx.Select(x => x.ProjectId).ToList();
            var projects = _projectService.GetList<ProjectView>(int.MaxValue, x => projectIDs.Contains(x.Id)).ToList();
            listEx.ForEach(w => {
                var p = projects.FirstOrDefault(x => x.Id == w.ProjectId);
                if(p!= null)
                {
                    w.ProjectName = p.ProjectName;
                }
            });
            return this.GetPageResult(listEx, gridRequest);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PostFile(BeamInfoView objModal, int ProjectId)
        {
            //验证错误
            if (!ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                objModal = objModal ?? new BeamInfoView();
                objModal.DwgFile = file.FileName;
                objModal.ProjectId = ProjectId;
                var fileExtension = System.IO.Path.GetExtension(file.FileName);
                if (fileExtension.ToLower() != ".dwg" )
                {
                    var result = GetErrorJson();
                    result.Message = new JsonMessage(103, "请上传dwg文件");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (!System.IO.Directory.Exists(Server.MapPath("/Files/BeamInfo/")))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("/Files/BeamInfo/"));
                }
                string path = "/Files/BeamInfo/" + file.FileName;
                file.SaveAs(Server.MapPath(path));
                _beamInfoService.InsertView(objModal);
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