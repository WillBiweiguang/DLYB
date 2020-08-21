using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using Infrastructure.Core;
using System.Linq.Expressions;
using System.Net;
using Infrastructure.Core.Logging;
using Infrastructure.Utility.Filter;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.IO;

namespace Infrastructure.Web.UI
{
    /// <summary>
    /// 基类BaseController，过滤器
    /// </summary>
    // [HandleError]
    //[FilterError]
    //[CustomAuthorize]
    public class ParentController<T0, T1> : Controller
        where T0 : EntityBase<int>, new()
        where T1 : IViewModel, new()
    {

        public string T(string strMsg)
        {
            return strMsg;

        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseService<T0> _BaseService;

        /// <summary>
        /// 
        /// </summary>
        public ILogger _Logger { get; set; }


        //  public LoginInfo objLoginInfo;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newsService"></param>
        public ParentController(IBaseService<T0> newsService)
        {
            //var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //json.SerializerSettings.DateFormatHandling
            //= Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;

            _BaseService = newsService;
            _Logger = LogManager.GetLogger(this.GetType());

            // T = NullLocalizer.Instance;

            //获取当前用户ID，传递给Service
            //  if (_AuthenticationService.GetAuthenticatedUser()!=null){
            // _BaseService.LoginUsrID = _AuthenticationService.GetAuthenticatedUser().Id;
            //  }

        }


        //统一Index action处理
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Index()
        {
            return View();
        }
       
        /// <summary>
        /// Edit page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ActionResult Edit(string id)
        {
            T1 obj = GetObject(id);

            return View(obj);
        }

        protected T1 GetObject(string id)
        {
            T1 obj;
            if (string.IsNullOrEmpty(id) || id == "0")
            {
                obj = new T1();
            }
            else { obj = (T1)new T1().ConvertAPIModel(_BaseService.Repository.GetByKey(int.Parse(id))); }

            return obj;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult List()
        {
            return View();
        }

        /// <summary>
        /// 获得分页列表数据
        /// </summary>
        /// <returns>返回Json对象</returns>
        public virtual ActionResult GetList()
        {

            GridRequest req = new GridRequest(Request);
            Expression<Func<T0, bool>> predicate = FilterHelper.GetExpression<T0>(req.FilterGroup);
            int iCount = req.PageCondition.RowCount;



            //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
            var list = GetListEx(predicate, req.PageCondition);// _newsService.GetList(null, null, 10, 10);


            return GetPageResult(list, req);
        }

        public virtual List<T1> GetListData()
        {

            GridRequest req = new GridRequest(Request);
            Expression<Func<T0, bool>> predicate = FilterHelper.GetExpression<T0>(req.FilterGroup);
            int iCount = req.PageCondition.RowCount;



            //实现对用户和多条件的分页的查询，rows表示一共多少条，page表示当前第几页
            var list = GetListEx(predicate, req.PageCondition);// _newsService.GetList(null, null, 10, 10);


            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        public ActionResult GetPageResult(object list, GridRequest req)
        {
            return Json(new
            {
                sEcho = Request["draw"],
                iTotalRecords = req.PageCondition.RowCount,
                iTotalDisplayRecords = req.PageCondition.RowCount,
                aaData = list
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CurPage"></param>
        /// <param name="PageSize"></param>
        /// <param name="iTotal"></param>
        /// <returns></returns>
        public virtual List<T1> GetListEx(Expression<Func<T0, bool>> predicate, PageCondition con)
        {
            return _BaseService.GetList<T1>(predicate, con);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ActionResult Get(string id)
        {

            var obj = _BaseService.Repository.GetByKey(int.Parse(id));
            if (obj == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
            {
                T1 a = (T1)new T1().ConvertAPIModel(obj);
                BeforeGet(a, obj);
                return Json(a, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// Get前处理
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objSrc"></param>
        public virtual void BeforeGet(T1 obj, T0 objSrc)
        {


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objModal"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(true)]
        public virtual JsonResult Post(T1 objModal, string Id)
        {

            //验证错误
            if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
            {
                return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
            }

            // T0 t = new T0();

            if (string.IsNullOrEmpty(Id) || Id == "0")
            {
                _BaseService.InsertView(objModal);
            }
            else
            {
                _BaseService.UpdateView(objModal);

            }
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证错误处理
        /// </summary>
        /// <returns></returns>
        protected AjaxResult<int> GetErrorJson()
        {

            AjaxResult<int> result = new AjaxResult<int>();
            StringBuilder sbError = new StringBuilder();
            foreach (KeyValuePair<string, ModelState> s in ModelState)
            {
                if (s.Value.Errors.Count > 0)
                {
                    foreach (ModelError me in s.Value.Errors)
                    {
                        sbError.AppendFormat("{0}\r\n", me.ErrorMessage);
                    }
                }
            }

            result.Message = new JsonMessage(103, sbError.ToString());
            return result;

        }

        public virtual ActionResult Export()
        {

            var lst = BeforeExport();
            var csv = new CsvSerializer<T1>();
            string sRet = csv.Serialize(lst, (string[])null);

            return File(Encoding.Default.GetBytes(sRet), "text/comma-separated-values", string.Format("{0}.csv", DateTime.Now.ToString("yyyMMddHHmmss")));
        }

        /// <summary>
        /// 导出前处理
        /// </summary>
        /// <returns></returns>
        public virtual List<T1> BeforeExport()
        {
            //_BaseService.Repository.Entities.ToList()
            return null;
        }

        /// <summary>
        /// 更新前处理
        /// </summary>
        /// <param name="objModal"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public virtual bool BeforeAddOrUpdate(T1 objModal, string Id)
        {
            return true;
        }

        public virtual bool AfterDelete(string sIds)
        {
            return true;
        }

        /// <summary>
        /// 删除action处理
        /// </summary>
        /// <param name="sIds"></param>
        /// <returns></returns>
        public virtual JsonResult Delete(string sIds)
        {
            if (!string.IsNullOrEmpty(sIds))
            {
                int[] arrID = sIds.TrimEnd(',').Split(',').Select(a => int.Parse(a)).ToArray();

                //   var lst=   arrID.SelectMany(a => !string.IsNullOrEmpty(a)).ToArray();
                // T0 objModal = new T0();
                // objModal.Id = int.Parse(strID);
                _BaseService.Repository.Delete(arrID);
                AfterDelete(sIds);
            }

            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取树形json数据的action
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult getTreeData()
        {
            //int PageRowCnt = 0;
            //var lst = AllService.getList<T>("", ""
            //      , 0, 0, 0
            //      , GetListCondition, GetListJoin, ref PageRowCnt);
            //var result = new { total = lst.Count, rows = lst };
            //string json = Data2Json(result);
            //json = json.Replace("\"PARENTID\": null,", "").Replace("PARENTID", "_parentId");
            return Content("");

        }

        public virtual ActionResult getDictionary()
        {

            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 成功返回json处理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected AjaxResult<T1> doJson(List<T1> data)
        {
            return doJson(data, "");
        }

        protected AjaxResult<T1> doJson(List<T1> data, string strMsg)
        {
            AjaxResult<T1> result = new AjaxResult<T1>();
            result.Data = data;
            result.Message = new JsonMessage(200, strMsg);
            return result;
        }

        protected JsonResult SuccessNotification(string strMsg)
        {
            AjaxResult<T1> result = new AjaxResult<T1>();
            result.Data = null;
            result.Message = new JsonMessage(200, strMsg);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult ErrorNotification(Exception ex, string strMsg)
        {
            AjaxResult<T1> result = new AjaxResult<T1>();
            result.Data = null;
            if (ex != null)
            {
                result.Message = new JsonMessage(500, strMsg + "\r\n" + ex.Message);
            }
            else
            {
                result.Message = new JsonMessage(500, strMsg);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult ErrorNotification(Exception ex)
        {
            return ErrorNotification(ex, "");
        }

        protected JsonResult ErrorNotification(string strMsg)
        {
            return ErrorNotification(null, strMsg);
        }

        protected void SetLanguage(string strLang)
        {

            EntityUser obj = new EntityUser()
            {
                objCulture = new System.Globalization.CultureInfo(strLang)
            };

            HttpContext.Items["UserInfo"] = obj;
        }


        /// <summary>
        /// 重新基类在Action执行之前的事情
        /// </summary>
        /// <param name="filterContext">重写方法的参数</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           

            if (Request.IsAuthenticated)
            {
                //_BaseService.LoginUserName = objLoginInfo.UserID;
            }




            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 统一的错误处理
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            //错误记录
            //WHC.Framework.Commons.LogTextHelper.Error(filterContext.Exception);

            // 当自定义显示错误 mode = On，显示友好错误页面
            // if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                filterContext.ExceptionHandled = true;

                Exception ex=filterContext.Exception;
                while(ex!=null){
                      LogManager.GetLogger(this.GetType().Name).Error(ex,ex.Message);
                      ex = ex.InnerException;
                }
              
                // this.View("Error").ExecuteResult(this.ControllerContext);

                if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                {
                    AjaxResult<int> result = new AjaxResult<int>();
                    result.Message = new JsonMessage((int)HttpStatusCode.BadRequest, filterContext.Exception.Message);
                    filterContext.Result = Json(result, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    string controllerName = (string)filterContext.RouteData.Values["controller"];
                    string actionName = (string)filterContext.RouteData.Values["action"];
                    HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "Error",
                        MasterName = "",
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                        TempData = filterContext.Controller.TempData
                    };
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 500;

                    // Certain versions of IIS will sometimes use their own error page when
                    // they detect a server error. Setting this property indicates that we
                    // want it to try to render ASP.NET MVC's error page instead.
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;


                }
            }



        }



    }

    /// <summary>
    /// 
    /// </summary>
    public static class Ext
    {


        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)  
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first  
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression   
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="a"></param>
        ///// <param name="b"></param>
        ///// <returns></returns>
        //public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> a,Expression<Func<T, bool>> b)
        //{
        //    var p = Expression.Parameter(typeof(T), "m");
        //    var bd = Expression.AndAlso(
        //            Expression.Invoke(a, p),
        //            Expression.Invoke(b, p));
        //    var ld = Expression.Lambda<Func<T, bool>>(bd, p);
        //    return ld;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="a"></param>
        ///// <param name="b"></param>
        ///// <returns></returns>
        //public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        //{
        //    var p = Expression.Parameter(typeof(T), "m");
        //    var bd = Expression.Or(
        //            Expression.Invoke(a, p),
        //            Expression.Invoke(b, p));
        //    var ld = Expression.Lambda<Func<T, bool>>(bd, p);
        //    return ld;
        //}

    }

    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }

}
