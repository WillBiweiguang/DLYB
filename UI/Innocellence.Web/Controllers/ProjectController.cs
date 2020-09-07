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

namespace Innocellence.Web.Controllers
{
    public class ProjectController : BaseController<Project, ProjectView>
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService) : base(projectService)
        {
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
            Expression<Func<Project, bool>> expression = FilterHelper.GetExpression<Project>(gridRequest.FilterGroup);
            if (!string.IsNullOrEmpty(strCondition))
            {
                expression = expression.AndAlso<Project>(x => x.ProjectName.Contains(strCondition) && x.IsDeleted != true);
            }
            else
            {
                expression = expression.AndAlso<Project>(x => x.IsDeleted != true);
            }
            int rowCount = gridRequest.PageCondition.RowCount;
            List<ProjectView> listEx = GetListEx(expression, gridRequest.PageCondition);
            return this.GetPageResult(listEx, gridRequest);
        }

        //public override ActionResult GetList()
        //{
        //    GridRequest gridRequest = new GridRequest(Request);
        //    string strCondition = Request["search_condition"];
        //    Expression<Func<Project, bool>> expression = FilterHelper.GetExpression<Project>(gridRequest.FilterGroup);
        //    if (!string.IsNullOrEmpty(strCondition))
        //    {
        //        expression = expression.AndAlso<Project>(x => x.WeldLocationType.Contains(strCondition));
        //    }
        //    int rowCount = gridRequest.PageCondition.RowCount;
        //    List<ProjectView> listEx = GetListEx(expression, gridRequest.PageCondition);
        //    return this.GetPageResult(listEx, gridRequest);
        //}
    }
}