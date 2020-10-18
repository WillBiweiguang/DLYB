// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;


using System.Data.Entity.Infrastructure;
using System.Globalization;

using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using Infrastructure.Web.Domain.Service.Common;
using System.Security.Principal;
using Infrastructure.Web.Domain.ModelsView;



namespace Infrastructure.Web.Domain.Services
{
    /// <summary>
    /// 业务实现——身份认证模块
    /// </summary>
    public partial class SysUserService : BaseService<SysUser>, ISysUserService
    {

        public UserManager<SysUser, int> UserContext { get; set; }
        public UserStore Store { get; set; }


        public SysUserService()
        {

            Store = new UserStore((DbContext)Repository.UnitOfWork);
            UserContext = new UserManager<SysUser, int>(Store);
        }

        public SysUserService(IUnitOfWork dbContext)
            : base(dbContext)
        {

            Store = new UserStore((DbContext)dbContext);
            UserContext = new UserManager<SysUser, int>(Store);
        }

        public SysUser AutoLogin(string strUser)
        {
            // var strUser = objWI is WindowsIdentity ? SysCommon.GetUserName(objWI) : objWI.Name;
            //数据库获取设置信息
            BaseService<SysUser> objServ = new BaseService<SysUser>();
            var obj = objServ.Repository.Entities.Where(a => (a.UserId == strUser || a.UserName == strUser) && a.IsDeleted == false ).FirstOrDefault();
            if (obj != null)
            {

                SysMenuService objServ1 = new SysMenuService();
                obj.Menus = objServ1.GetMenusByUserID(obj, Store);

                // objLoginInfo = obj;
                //Session["UserInfo"] = objLoginInfo;

                //登录日志
                BaseService<LogsModel> objServLogs = new BaseService<LogsModel>();
                objServLogs.Repository.Insert(new LogsModel()
                {
                    LogCate = "AdminLogin",
                    LogContent = "登录成功",
                    CreatedUserID = obj.Id.ToString(),
                    CreatedUserName = obj.UserName
                });
            }

            return obj;

        }

        public SysUser AutoLogin(IIdentity objWI)
        {
            var strUser = objWI is WindowsIdentity ? SysCommon.GetUserName(objWI) : objWI.Name;
            return AutoLogin(strUser);

        }


        public SysUser GetLoginUser(int strUserID)
        {
            SysUser tUser = Repository.GetByKey(strUserID);
            if (tUser != null)
            {
                SysMenuService objServ = new SysMenuService();
                tUser.Menus = objServ.GetMenusByUserID(tUser, Store);
            }

            return tUser;
        }

        public SysUser UserLoginAsync(string strUser, string strPassword,bool verifyPwd = true)
        {
            SysUser tUser = Repository.Entities.Where(a => (a.UserId == strUser || a.UserName == strUser) && a.IsDeleted == false).FirstOrDefault();
            SysUser result;
            if (tUser == null)
            {
                result = default(SysUser);
            }
            else
            {
                result = tUser;
                if (verifyPwd)
                {
                    result = ((UserContext.PasswordHasher.VerifyHashedPassword(tUser.PasswordHash, strPassword) != PasswordVerificationResult.Failed) ? tUser : default(SysUser));
                }        
                if (result != null)
                {
                    SysMenuService objServ = new SysMenuService();
                    result.Menus = objServ.GetMenusByUserID(result, Store);


                }
            }
            return result;
        }


        public override int InsertView<T>(T obj)
        {
            var t = obj.MapTo<SysUser>();
            var count = Repository.Insert(t);

            var view = (SysUserView)(IViewModel)obj;
            view.Id = t.Id;
            UpdateUserRole(view);

            return count;
        }

        /// <summary>
        /// 更新viewmodel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int UpdateView<T>(T obj)
        {
            var t = obj.MapTo<SysUser>();

            UpdateUserRole((SysUserView)(IViewModel)obj);

            return Repository.Update(t);
        }


        private void UpdateUserRole(SysUserView obj)
        {
            var Ser = new BaseService<SysUserRole>();
            Ser.Repository.Delete(a => a.UserId == obj.Id);
            string []Roles;
            if (obj.strRoles != null)
            {
                 Roles = obj.strRoles.Split(',');
                 var lst = new List<SysUserRole>();
                 foreach (var a in Roles)
                 {
                     if (!string.IsNullOrEmpty(a))
                     {
                         var RoleModel = new SysUserRole()
                         {
                             RoleId = int.Parse(a),
                             UserId = obj.Id
                         };
                         lst.Add(RoleModel);
                     }

                 }
                 Ser.Repository.Insert((IEnumerable<SysUserRole>)lst);
            }
            else
            {
                Roles = null;
               
            }
            
        }

    }


    public class UserStoreNoDB : IUserStore<SysUser, Int32>
    {
        public Task CreateAsync(SysUser user) { return null; }
        public Task DeleteAsync(SysUser user) { return null; }
        public Task<SysUser> FindByIdAsync(Int32 userId) { return null; }
        public Task<SysUser> FindByNameAsync(string userName) { return null; }
        public Task UpdateAsync(SysUser user) { return null; }

        public void Dispose()
        {

        }
    }

    /// <summary>
    ///     EntityFramework based user store implementation that supports IUserStore, IUserLoginStore, IUserClaimStore and
    ///     IUserRoleStore
    /// </summary>
    /// <typeparam name="SysUser"></typeparam>
    /// <typeparam name="SysRole"></typeparam>
    /// <typeparam name="Int32"></typeparam>
    /// <typeparam name="SysUserLogin"></typeparam>
    /// <typeparam name="SysUserRole"></typeparam>
    /// <typeparam name="SysUserClaim"></typeparam>
    public class UserStore :
        IUserLoginStore<SysUser, Int32>,
        IUserClaimStore<SysUser, Int32>,
        IUserRoleStore<SysUser, Int32>,
        IUserPasswordStore<SysUser, Int32>,
        IUserSecurityStampStore<SysUser, Int32>,
        IQueryableUserStore<SysUser, Int32>,
        IUserEmailStore<SysUser, Int32>,
        IUserPhoneNumberStore<SysUser, Int32>,
        IUserTwoFactorStore<SysUser, Int32>,
        IUserLockoutStore<SysUser, Int32>
    {
        private readonly IDbSet<SysUserLogin> _logins;
        private readonly BaseService<SysRole> _roleStore;
        private readonly IDbSet<SysUserClaim> _userClaims;
        private readonly IDbSet<SysUserRole> _userRoles;
        private bool _disposed;
        private BaseService<SysUser> _userStore;


        public UserStore()
            : this(new MySqlDbContext())
        {

        }

        /// <summary>
        ///     Constructor which takes a db context and wires up the stores with default instances using the context
        /// </summary>
        /// <param name="context"></param>
        public UserStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
            AutoSaveChanges = true;
            _userStore = new BaseService<SysUser>((IUnitOfWork)context);
            _roleStore = new BaseService<SysRole>((IUnitOfWork)context);
            _logins = (IDbSet<SysUserLogin>)new BaseService<SysUserLogin>((IUnitOfWork)context).Repository.Entities;
            _userClaims = (IDbSet<SysUserClaim>)new BaseService<SysUserClaim>((IUnitOfWork)context).Repository.Entities;
            _userRoles = (IDbSet<SysUserRole>)new BaseService<SysUserRole>((IUnitOfWork)context).Repository.Entities;
        }

        /// <summary>
        ///     Context for the store
        /// </summary>
        public DbContext Context { get; private set; }

        /// <summary>
        ///     If true will call dispose on the DbContext during Dispose
        /// </summary>
        public bool DisposeContext { get; set; }

        /// <summary>
        ///     If true will call SaveChanges after Create/Update/Delete
        /// </summary>
        public bool AutoSaveChanges { get; set; }

        /// <summary>
        ///     Returns an IQueryable of users
        /// </summary>
        public IQueryable<SysUser> Users
        {
            get { return _userStore.Repository.Entities; }
        }

        /// <summary>
        ///     Return the claims for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<IList<Claim>> GetClaimsAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            await EnsureClaimsLoaded(user);
            return user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        /// <summary>
        ///     Add a claim to a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public virtual Task AddClaimAsync(SysUser user, Claim claim)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            _userClaims.Add(new SysUserClaim { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Remove a claim from a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public virtual async Task RemoveClaimAsync(SysUser user, Claim claim)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            IEnumerable<SysUserClaim> claims;
            var claimValue = claim.Value;
            var claimType = claim.Type;
            if (AreClaimsLoaded(user))
            {
                claims = user.Claims.Where(uc => uc.ClaimValue == claimValue && uc.ClaimType == claimType).ToList();
            }
            else
            {
                var userId = user.Id;
                claims = await _userClaims.Where(uc => uc.ClaimValue == claimValue && uc.ClaimType == claimType && uc.UserId.Equals(userId)).ToListAsync();
            }
            foreach (var c in claims)
            {
                _userClaims.Remove(c);
            }
        }

        /// <summary>
        ///     Returns whether the user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<bool> GetEmailConfirmedAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        ///     Set IsConfirmed on the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public virtual Task SetEmailConfirmedAsync(SysUser user, bool confirmed)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Set the user email
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public virtual Task SetEmailAsync(SysUser user, string email)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.Email = email;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Get the user's email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<string> GetEmailAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.Email);
        }

        /// <summary>
        ///     Find a user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public virtual Task<SysUser> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();
            return GetUserAggregateAsync(u => u.Email.ToUpper() == email.ToUpper());
        }

        /// <summary>
        ///     Returns the DateTimeOffset that represents the end of a user's lockout, any time in the past should be considered
        ///     not locked out.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<DateTimeOffset> GetLockoutEndDateAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }

        /// <summary>
        ///     Locks a user out until the specified end date (set to a past date, to unlock a user)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="lockoutEnd"></param>
        /// <returns></returns>
        public virtual Task SetLockoutEndDateAsync(SysUser user, DateTimeOffset lockoutEnd)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue ? (DateTime?)null : lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Used to record when an attempt to access the user has failed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<int> IncrementAccessFailedCountAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        ///     Used to reset the account access count, typically after the account is successfully accessed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task ResetAccessFailedCountAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Returns the current number of failed access attempts.  This number usually will be reset whenever the password is
        ///     verified or the account is locked out.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<int> GetAccessFailedCountAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        ///     Returns whether the user can be locked out.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<bool> GetLockoutEnabledAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>
        ///     Sets whether the user can be locked out.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public virtual Task SetLockoutEnabledAsync(SysUser user, bool enabled)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Find a user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual Task<SysUser> FindByIdAsync(Int32 userId)
        {
            ThrowIfDisposed();
            return GetUserAggregateAsync(u => u.Id.Equals(userId));
        }

        /// <summary>
        ///     Find a user by name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual Task<SysUser> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();
            return GetUserAggregateAsync(u => u.UserName.ToUpper() == userName.ToUpper());
        }

        /// <summary>
        ///     Insert an entity
        /// </summary>
        /// <param name="user"></param>
        public virtual async Task CreateAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            _userStore.Repository.Insert(user);
            await SaveChanges();
        }

        /// <summary>
        ///     Mark an entity for deletion
        /// </summary>
        /// <param name="user"></param>
        public virtual async Task DeleteAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            _userStore.Repository.Delete(user);
            await SaveChanges();
        }

        /// <summary>
        ///     Update an entity
        /// </summary>
        /// <param name="user"></param>
        public virtual async Task UpdateAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            _userStore.Repository.Update(user);
            await SaveChanges();
        }

        /// <summary>
        ///     Dispose the store
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // IUserLogin implementation

        /// <summary>
        ///     Returns the user associated with this login
        /// </summary>
        /// <returns></returns>
        public virtual async Task<SysUser> FindAsync(UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            var provider = login.LoginProvider;
            var key = login.ProviderKey;
            var userLogin =
                await _logins.FirstOrDefaultAsync(l => l.LoginProvider == provider && l.ProviderKey == key);
            if (userLogin != null)
            {
                var userId = userLogin.UserId;
                return await GetUserAggregateAsync(u => u.Id == userId);
            }
            return null;
        }

        /// <summary>
        ///     Add a login to the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public virtual Task AddLoginAsync(SysUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            _logins.Add(new SysUserLogin
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider
            });
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Remove a login from a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public virtual async Task RemoveLoginAsync(SysUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            SysUserLogin entry;
            var provider = login.LoginProvider;
            var key = login.ProviderKey;
            if (AreLoginsLoaded(user))
            {
                entry = user.Logins.SingleOrDefault(ul => ul.LoginProvider == provider && ul.ProviderKey == key);
            }
            else
            {
                var userId = user.Id;
                entry = await _logins.SingleOrDefaultAsync(ul => ul.LoginProvider == provider && ul.ProviderKey == key && ul.UserId.Equals(userId));
            }
            if (entry != null)
            {
                _logins.Remove(entry);
            }
        }

        /// <summary>
        ///     Get the logins for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            await EnsureLoginsLoaded(user);
            return user.Logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList();
        }

        /// <summary>
        ///     Set the password hash for a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public virtual Task SetPasswordHashAsync(SysUser user, string passwordHash)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Get the password hash for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<string> GetPasswordHashAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        ///     Returns true if the user has a password set
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<bool> HasPasswordAsync(SysUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        /// <summary>
        ///     Set the user's phone number
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public virtual Task SetPhoneNumberAsync(SysUser user, string phoneNumber)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Get a user's phone number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<string> GetPhoneNumberAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        ///     Returns whether the user phoneNumber is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<bool> GetPhoneNumberConfirmedAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        ///     Set PhoneNumberConfirmed on the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public virtual Task SetPhoneNumberConfirmedAsync(SysUser user, bool confirmed)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Add a user to a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public virtual async Task AddToRoleAsync(SysUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("IdentityResources.ValueCannotBeNullOrEmpty", "roleName");
            }
            var roleEntity = await _roleStore.Repository.Entities.SingleOrDefaultAsync(r => r.Name.ToUpper() == roleName.ToUpper());
            if (roleEntity == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    "IdentityResources.RoleNotFound", roleName));
            }

            var ur = new SysUserRole { UserId = user.Id, RoleId = roleEntity.Id };
            _userRoles.Add(ur);
        }

        /// <summary>
        ///     Remove a user from a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public virtual async Task RemoveFromRoleAsync(SysUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("IdentityResources.ValueCannotBeNullOrEmpty", "roleName");
            }
            var roleEntity = await _roleStore.Repository.Entities.SingleOrDefaultAsync(r => r.Name.ToUpper() == roleName.ToUpper());
            if (roleEntity != null)
            {
                var roleId = roleEntity.Id;
                var userId = user.Id;
                var userRole = await _userRoles.FirstOrDefaultAsync(r => roleId == r.RoleId && r.UserId == userId);
                if (userRole != null)
                {
                    _userRoles.Remove(userRole);
                }
            }
        }

        /// <summary>
        ///     Get the names of the roles a user is a member of
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<IList<string>> GetRolesAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var userId = user.Id;
            var query = from userRole in _userRoles
                        where userRole.UserId.Equals(userId)
                        join role in _roleStore.Repository.Entities on userRole.RoleId equals role.Id
                        select role.Name;
            return await query.ToListAsync();
        }

        /// <summary>
        ///     Returns true if the user is in the named role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsInRoleAsync(SysUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("IdentityResources.ValueCannotBeNullOrEmpty", "roleName");
            }
            var role = await _roleStore.Repository.Entities.SingleOrDefaultAsync(r => r.Name.ToUpper() == roleName.ToUpper());
            if (role != null)
            {
                var userId = user.Id;
                var roleId = role.Id;
                return await _userRoles.AnyAsync(ur => ur.RoleId.Equals(roleId) && ur.UserId.Equals(userId));
            }
            return false;
        }

        /// <summary>
        ///     Set the security stamp for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public virtual Task SetSecurityStampAsync(SysUser user, string stamp)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Get the security stamp for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<string> GetSecurityStampAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        ///     Set whether two factor authentication is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public virtual Task SetTwoFactorEnabledAsync(SysUser user, bool enabled)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Gets whether two factor authentication is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<bool> GetTwoFactorEnabledAsync(SysUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.TwoFactorEnabled);
        }

        // Only call save changes if AutoSaveChanges is true
        private async Task SaveChanges()
        {
            if (AutoSaveChanges)
            {
                await Context.SaveChangesAsync();
            }
        }

        private bool AreClaimsLoaded(SysUser user)
        {
            return Context.Entry(user).Collection(u => u.Claims).IsLoaded;
        }

        private async Task EnsureClaimsLoaded(SysUser user)
        {
            if (!AreClaimsLoaded(user))
            {
                var userId = user.Id;
                await _userClaims.Where(uc => uc.UserId == userId).LoadAsync();
                Context.Entry(user).Collection(u => u.Claims).IsLoaded = true;
            }
        }

        private async Task EnsureRolesLoaded(SysUser user)
        {
            if (!Context.Entry(user).Collection(u => u.Roles).IsLoaded)
            {
                var userId = user.Id;
                await _userRoles.Where(uc => uc.UserId == userId).LoadAsync();
                Context.Entry(user).Collection(u => u.Roles).IsLoaded = true;
            }
        }

        private bool AreLoginsLoaded(SysUser user)
        {
            return Context.Entry(user).Collection(u => u.Logins).IsLoaded;
        }

        private async Task EnsureLoginsLoaded(SysUser user)
        {
            if (!AreLoginsLoaded(user))
            {
                var userId = user.Id;
                await _logins.Where(uc => uc.UserId == userId).LoadAsync();
                Context.Entry(user).Collection(u => u.Logins).IsLoaded = true;
            }
        }

        /// <summary>
        /// Used to attach child entities to the User aggregate, i.e. Roles, Logins, and Claims
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        protected virtual async Task<SysUser> GetUserAggregateAsync(Expression<Func<SysUser, bool>> filter)
        {
            Int32 id;
            SysUser user;
            if (FindByIdFilterParser.TryMatchAndGetId(filter, out id))
            {
                user = await _userStore.Repository.GetByKeyAsync(id);
            }
            else
            {
                user = await Users.FirstOrDefaultAsync(filter);
            }
            if (user != null)
            {
                // await EnsureClaimsLoaded(user);
                //  await EnsureLoginsLoaded(user);
                //  await EnsureRolesLoaded(user);
            }
            return user;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        ///     If disposing, calls dispose on the Context.  Always nulls out the Context
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (DisposeContext && disposing && Context != null)
            {
                Context.Dispose();
            }
            _disposed = true;
            Context = null;
            _userStore = null;
        }

        // We want to use FindAsync() when looking for an User.Id instead of LINQ to avoid extra 
        // database roundtrips. This class cracks open the filter expression passed by 
        // UserStore.FindByIdAsync() to obtain the value of the id we are looking for 
        private static class FindByIdFilterParser
        {
            // expression pattern we need to match
            private static readonly Expression<Func<SysUser, bool>> Predicate = u => u.Id.Equals(default(Int32));
            // method we need to match: Object.Equals() 
            private static readonly MethodInfo EqualsMethodInfo = ((MethodCallExpression)Predicate.Body).Method;
            // property access we need to match: User.Id 
            private static readonly MemberInfo UserIdMemberInfo = ((MemberExpression)((MethodCallExpression)Predicate.Body).Object).Member;

            internal static bool TryMatchAndGetId(Expression<Func<SysUser, bool>> filter, out Int32 id)
            {
                // default value in case we can’t obtain it 
                id = default(Int32);

                // lambda body should be a call 
                if (filter.Body.NodeType != ExpressionType.Call)
                {
                    return false;
                }

                // actually a call to object.Equals(object)
                var callExpression = (MethodCallExpression)filter.Body;
                if (callExpression.Method != EqualsMethodInfo)
                {
                    return false;
                }
                // left side of Equals() should be an access to User.Id
                if (callExpression.Object == null
                    || callExpression.Object.NodeType != ExpressionType.MemberAccess
                    || ((MemberExpression)callExpression.Object).Member != UserIdMemberInfo)
                {
                    return false;
                }

                // There should be only one argument for Equals()
                if (callExpression.Arguments.Count != 1)
                {
                    return false;
                }

                MemberExpression fieldAccess;
                if (callExpression.Arguments[0].NodeType == ExpressionType.Convert)
                {
                    // convert node should have an member access access node
                    // This is for cases when primary key is a value type
                    var convert = (UnaryExpression)callExpression.Arguments[0];
                    if (convert.Operand.NodeType != ExpressionType.MemberAccess)
                    {
                        return false;
                    }
                    fieldAccess = (MemberExpression)convert.Operand;
                }
                else if (callExpression.Arguments[0].NodeType == ExpressionType.MemberAccess)
                {
                    // Get field member for when key is reference type
                    fieldAccess = (MemberExpression)callExpression.Arguments[0];
                }
                else
                {
                    return false;
                }

                // and member access should be a field access to a variable captured in a closure
                if (fieldAccess.Member.MemberType != MemberTypes.Field
                    || fieldAccess.Expression.NodeType != ExpressionType.Constant)
                {
                    return false;
                }

                // expression tree matched so we can now just get the value of the id 
                var fieldInfo = (FieldInfo)fieldAccess.Member;
                var closure = ((ConstantExpression)fieldAccess.Expression).Value;

                id = (Int32)fieldInfo.GetValue(closure);
                return true;
            }
        }
    }


}