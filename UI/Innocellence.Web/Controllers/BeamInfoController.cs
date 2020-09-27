﻿using System;
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

        public BeamInfoController(IBeamInfoService beamInfoService) : base(beamInfoService)
        {
            _beamInfoService = beamInfoService;
        }
        // GET: Address
        public override ActionResult Index()
        {
            //var list = _addressService.GetList<AddressView>(int.MaxValue, x => !x.IsDeleted).ToList();
            string projectId = Request["projectId"];
            ViewBag.ProjectId = projectId;
            return View();
        }

        public override ActionResult GetList()
        {
            GridRequest gridRequest = new GridRequest(Request);
            string strCondition = Request["search_condition"];
            string projectId = Request["project_id"];
            int pid = string.IsNullOrEmpty(projectId) ? 0 : int.Parse(projectId);
            Expression<Func<BeamInfo, bool>> expression = FilterHelper.GetExpression<BeamInfo>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(projectId))
            {
                expression = expression.AndAlso<BeamInfo>(x => x.ProjectId == pid && x.IsDeleted != true);
            }
            else
            {
                expression = expression.AndAlso<BeamInfo>(x => x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<BeamInfoView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult PostFile(BeamInfoView objModal, int ProjectName)
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
                objModal.ProjectId = ProjectName;
                var fileExtension = System.IO.Path.GetExtension(file.FileName);
                if (fileExtension.ToLower() != ".dwg")
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