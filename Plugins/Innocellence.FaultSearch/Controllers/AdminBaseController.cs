
using System.Web;
using System.Web.Mvc;
using Infrastructure.Web.UI;
using Infrastructure.Core;
using DLYB.CA.Contracts.Contracts;

using Infrastructure.Web.MVC.Attribute;


namespace DLYB.FaultSearch.Controllers
{
    /// <summary>
    /// 基类BaseController，过滤器
    /// </summary>
    // [HandleError]

    //[FilterError]
    //[CustomAuthorize]

    [CustomAuthorize]

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T1"></typeparam>
    public class AdminBaseController<T, T1> : ParentController<T, T1>
        where T : EntityBase<int>, new()
        where T1 : IViewModel, new()
    {
        /// <summary>
        /// 当前的DBService
        /// </summary>
        public IBaseService<T> _objService;
        //全局用户对象，当前的登录用户
      
        public int AppId;

        protected bool isAuthed;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newsService"></param>
        public AdminBaseController(IBaseService<T> newsService)
            : base(newsService)
        {
           
            _objService = newsService;
           

        }
        public virtual ActionResult CheckIndex()
        {
            return View();
        }
        public virtual ActionResult Previous()
        {
            var id = Request["pid"];
            var isopen = Request["isopen"];
            var isnew = Request["isnew"];
            string url = Request["preurl"];
            return Redirect(url + "?pid=" + id + "&isopen=" + isopen + "&isnew=" + isnew);
        }
        public virtual ActionResult Next()
        {
            var id = Request["pid"];
            var isopen = Request["isopen"];
            var isnew = Request["isnew"];
            string url = Request["nexturl"];
            return Redirect(url + "?pid=" + id + "&isopen=" + isopen + "&isnew=" + isnew);
        }

        /// <summary>
        /// 重新基类在Action执行之前的事情
        /// </summary>
        /// <param name="filterContext">重写方法的参数</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.AppId = AppId;

            if (!Request.IsAuthenticated || User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            {
                isAuthed = false;
                string redirectUrl = "";

                redirectUrl = "~/Account/Login";

                filterContext.Result = Redirect(redirectUrl);
            }


            base.OnActionExecuting(filterContext);
           

            isAuthed = true;
          

            base.OnActionExecuting(filterContext);
        }

        public string AccessToken
        {
            get { return string.Empty; }
        }

        protected virtual void AppDataPermissionCheck(IDataPermissionCheck service, int targetAppId, int currentAppId)
        {
            if (service.AppDataCheck(targetAppId, currentAppId)) return;
            //log.Warn("user({0}) want to access {1} app that have not been authorization!", sysUser.UserName, targetAppId);
            if (HttpContext.Request.IsAjaxRequest())
            {
                throw new HttpException(403, "You are not authorized to access this page.");
            }
            HttpContext.Response.Redirect("/403.html", true);
        }
    }
}
