using DLYB.Web.Controllers;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Innocellence.Web.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace DLYB.Web.Controllers.API
{
    public class LimaiController : ApiBaseController<Project, ProjectView>
    {
        private readonly IWeldCategoryLabelingService _weldCategoryService;
        private readonly IBeamInfoService _beamInfoService;
        private readonly ITaskListService _taskListService;
        private readonly IProjectService _projectService;
        public LimaiController(IWeldCategoryLabelingService weldCategoryLabelingService, IBeamInfoService beamInfoService
            , ITaskListService taskListService, IProjectService projectService) : base(projectService)
        {
            _weldCategoryService = weldCategoryLabelingService;
            _beamInfoService = beamInfoService;
            _taskListService = taskListService;
            _projectService = projectService;
        }

        [System.Web.Mvc.HttpPost]
        [VerifyParam(Param = "")]
        public JsonResult HanjiProportion(LiMaiApiViewModel search)
        {
            var list = _projectService.HancaiQuanlityByType(search);
            return new JsonResult { Data = new { Status = "200", Data = JsonConvert.SerializeObject(list) }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [System.Web.Mvc.HttpPost]
        [VerifyParam(Param = "")]
        public JsonResult HancaiProportion(LiMaiApiViewModel search)
        {
            var list = _projectService.HancaiQuanlity(search);
            return new JsonResult { Data = new { Status = "200", Data= JsonConvert.SerializeObject(list) }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
