using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Services;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Infrastructure.Web.Domain.Entity;

namespace DLYB.Web.Controllers
{

    public class HomeController : Controller
    {
        IReleaseNoteService _releaseNoteService = new ReleaseNotesService();

       // [SSOOWinAuthorize("SAML2", true)]
        public ActionResult Index()
        {
            var objLoginInfo = Session["UserInfo"] as SysUser;

            if (objLoginInfo == null)
            {
                return Redirect("~/Account/Login");
            }
            return Redirect("~/Project/Index");
            return View();
        }

        public ActionResult About()
        {
            var path = Server.MapPath("~/");
            var versionFile = path + @"releasenotes.txt";
            var releaseNotes = "";
            using (var sr = new StreamReader(versionFile))
            {
                releaseNotes = sr.ReadToEnd();
                sr.Close();
            }
            ViewBag.ReleaseNotes = releaseNotes.Replace("\n", "<BR />");
            return View();
        }
        public ActionResult ReleaseNotes()
        {
            ViewBag.ReleaseNotes = _releaseNoteService.Repository.Entities.OrderByDescending(a => a.ReleaseDate).ToList();
            return View();
        }
        public ActionResult Status()
        {
            return Content("status=ok");
        }
    }
}