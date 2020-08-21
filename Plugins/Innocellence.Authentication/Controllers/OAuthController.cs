using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Web.Domain.Service;
using DLYB.CA.Service.Interface;

namespace DLYB.Authentication.Controllers
{
    public class OAuthController : Controller
    {
        private readonly IOAuthService _authService;
        private readonly ISysUserService _sysUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger _logger = LogManager.GetLogger(typeof(OAuthController).Name);

        public OAuthController(IOAuthService authService, ISysUserService sysUserService, IAuthenticationService authenticationService)
        {
            _authService = authService;
            _sysUserService = sysUserService;
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<ActionResult> OAuthToken(string clientId, string relayState)
        {
            var claims = HttpContext.GetOwinContext().Authentication.AuthenticationResponseGrant.Identity.Claims.FirstOrDefault(a =>
                    a.Type == ClaimTypes.NameIdentifier);

            var builder = new UriBuilder(new Uri(relayState));
            var query = HttpUtility.ParseQueryString(builder.Query);
            
            if (claims == null)
            {
                _logger.Debug<string>("auth failed");
                //query.Add("token", string.Empty);

                return View("~/Views/Shared/Error.cshtml", new HandleErrorInfo(new Exception("auth failed"), "OAuth", "OAuthToken"));
            }

            var user = _sysUserService.AutoLogin(claims.Value);
            if (user == null)
            {
                var strMsg = string.Format("login OK,but user [{0}] not found!", claims.Value);
                _logger.Error(strMsg);
                return View("~/Views/Shared/Error.cshtml", new HandleErrorInfo(new Exception(strMsg), "OAuth", "OAuthToken"));
            }
            await _authenticationService.SignInAsync(user, true);

            var token = _authService.GetToken(clientId, claims.Value, relayState);
            query.Add("token", token);

            builder.Query = query.ToString();

            return Redirect(builder.ToString());
        }

        [HttpPost]
        public JsonResult OAuthUserId(string clientId, string token)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException("clientId");
            }

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("token");
            }

            var id = _authService.GetUserId(clientId, token);
            return Json(id);
        }


    }
}