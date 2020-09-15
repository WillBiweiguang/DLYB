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
    }
}