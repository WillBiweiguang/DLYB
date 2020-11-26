using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using System.Web.Mvc;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Core.Data;
using Microsoft.AspNet.Identity;

namespace DLYB.Web.Controllers
{
    public class SysUserController : BaseController<SysUser, SysUserView>
    {
        ISysRoleService ServiceRole;

        public SysUserController(ISysUserService newsService, ISysRoleService _ServiceRole)
            : base(newsService)
        {
            ServiceRole = _ServiceRole;
        }

        public override ActionResult Index()
        {
            ViewBag.Roles = ServiceRole.Repository.Entities.Where(a => !a.IsDeleted.Value & a.Name != "Super Admin").ToList();
            return base.Index();
        }

        public ActionResult ExtIndex()
        {
            ViewBag.Roles = ServiceRole.Repository.Entities.Where(a => !a.IsDeleted.Value).ToList();
            return base.Index();
        }
        //初始化list页面
        public override List<SysUserView> GetListEx(Expression<Func<SysUser, bool>> predicate, PageCondition ConPage)
        {
            string strModeId = Request["SearchLoadModeId"];
            string strGroup = Request["SearchGroup"];
            string strCondition = Request["search_condition"];
        
            var superUser = Infrastructure.Core.Infrastructure.EngineContext.Current.WebConfig.SupperUser;
            if (!string.IsNullOrEmpty(strCondition))
            {
                strCondition = strCondition.Trim().ToLower();
                predicate = predicate.AndAlso(x => (x.Email != null && x.Email.ToLower().Contains(strCondition)) ||
                        x.UserName.ToLower().Contains(strCondition) ||
                        x.UserTrueName.ToLower().Contains(strCondition));
            }
            if(objLoginInfo.UserName != superUser)
            {
                if (!string.IsNullOrEmpty(objLoginInfo.Department))
                {
                    predicate = predicate.AndAlso(x => x.Department == objLoginInfo.Department);
                }
                else
                {
                    predicate = predicate.AndAlso(x => x.Department == "");
                }          
            }
            var q = _objService.GetList<SysUserView>(predicate.AndAlso(x => x.IsDeleted == false && x.UserName != superUser), ConPage);

            return q.ToList();
        }

        public override void BeforeGet(SysUserView obj, SysUser objSrc)
        {
            var UserRoles = new BaseService<SysUserRole>().Repository.Entities.Where(a => a.UserId == objSrc.Id).ToList();
            foreach (var a in UserRoles)
            {
                obj.strRoles += "," + a.RoleId.ToString();
            }
            if (!string.IsNullOrEmpty(obj.strRoles))
            {
                obj.strRoles = obj.strRoles.Substring(1);
            }
        }

        public override bool BeforeAddOrUpdate(SysUserView objModal, string Id)
        {
            if (objModal.Id == 0)
            {
                //create
                if (_objService.Repository.CheckExists(x => x.UserName == objModal.UserName && !x.IsDeleted.Value))
                {
                    ModelState.AddModelError("userName", @"该用户名已经被注册,请选择其它用户名进行注册!");
                    return false;
                }
                if (objModal.strRoles==null)
                {
                    ModelState.AddModelError("userName", @"请选择Role!");
                    return false;
                }
                objModal.PasswordHash = new PasswordHasher().HashPassword(objModal.PasswordHash);
                objModal.SecurityStamp = "DLYB";
            }
            else
            {
                //update
                var userEntity = _objService.Repository.Entities.Where(x => x.Id == objModal.Id).AsNoTracking().FirstOrDefault();
                if (userEntity == null)
                {
                    ModelState.AddModelError("sysUser", @"您操作的用户不存在 ,操作失败!");
                    return false;
                }
                if (objModal.strRoles == null)
                {
                    ModelState.AddModelError("userName", @"请选择Role!");
                    return false;
                }
                if (userEntity.PasswordHash != objModal.PasswordHash)
                {
                    objModal.PasswordHash = new PasswordHasher().HashPassword(objModal.PasswordHash);
                }
               
            }

            //去掉密码外入力项的多余空格
            if (objModal != null)
            {
                objModal.UserName = objModal.UserName.Trim();
                objModal.UserTrueName = objModal.UserTrueName.Trim();
                objModal.Email = objModal.Email?.Trim();
            }

            return true;
        }

        [AllowAnonymous]
        public JsonResult GetCurrentUser()
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();
            var userEntity = _objService.Repository.Entities.Where(x => x.Id == userId).AsNoTracking().FirstOrDefault();
            SysUserView data = ConvertToSysUserView(userEntity);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private SysUserView ConvertToSysUserView(SysUser userEntity)
        {
            if (userEntity != null)
            {
                var role = string.Empty;
                if (userEntity.Roles != null && userEntity.Roles.Count > 0)
                {
                    var rolesId = userEntity.Roles.Select(r => r.RoleId).ToList();
                    var roles = string.Join(",", rolesId);
                    role += roles;
                }
                return new SysUserView
                {
                    Id = userEntity.Id,
                    PasswordHash = userEntity.PasswordHash,
                    UserName = userEntity.UserName,
                    UserTrueName = userEntity.UserTrueName,
                    Email = userEntity.Email,
                    SecurityStamp = userEntity.SecurityStamp,
                    PhoneNumber = userEntity.PhoneNumber,
                    strRoles = role,
                    UpdatedDate = DateTime.UtcNow,
                };
            }
            return new SysUserView { };
        }

        //public override JsonResult Post(SysUserView objModal, string Id)
        //{

        //    //验证错误
        //    if (!BeforeAddOrUpdate(objModal, Id) || !ModelState.IsValid)
        //    {
        //        return Json(GetErrorJson(), JsonRequestBehavior.AllowGet);
        //    }

        //    // T0 t = new T0();

        //    if (string.IsNullOrEmpty(Id) || Id == "0")
        //    {
        //        _BaseService.InsertView(objModal);
        //    }
        //    else
        //    {
        //        _BaseService.UpdateView(objModal);

        //    }
        //    return Json(doJson(null), JsonRequestBehavior.AllowGet);
        //}

    }
}
