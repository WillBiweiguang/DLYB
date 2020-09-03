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

namespace Innocellence.Web.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        // GET: Address
        public ActionResult Index()
        {
            //var list = _addressService.GetList<AddressView>(int.MaxValue, x => !x.IsDeleted).ToList();
                 
            return View();
        }

        public ActionResult Create()
        {
            return PartialView();
        }

        public ActionResult Edit(int id)
        {
            //var addr = _addressService.GetList<AddressView>(1, x => !x.IsDeleted).FirstOrDefault();
            return View();
        }

        [HttpPost]
        public JsonResult Update(AddressView view)
        {
            //_addressService.UpdateView(view);
            return Json(new { result = "success" });
        }
    }
}