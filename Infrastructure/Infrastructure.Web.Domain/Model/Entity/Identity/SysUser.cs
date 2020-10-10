using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations.Schema;
namespace Infrastructure.Web.Domain.Entity
{
    [Table("t_sysUser")]
    public partial class SysUser : EntityBase<int>, IUser<int>
    {

        /// <summary>
        ///     Email
        /// </summary>
        public string Email
        {
            get;
            set;
        }
        /// <summary>
        ///     True if the email is confirmed, default is false
        /// </summary>
        public bool EmailConfirmed
        {
            get;
            set;
        }
        /// <summary>
        ///     The salted/hashed form of the user password
        /// </summary>
        public string PasswordHash
        {
            get;
            set;
        }
        /// <summary>
        ///     A random value that should change whenever a users credentials have changed (password changed, login removed)
        /// </summary>
        public string SecurityStamp
        {
            get;
            set;
        }
        /// <summary>
        ///     PhoneNumber for the user
        /// </summary>
        public string PhoneNumber
        {
            get;
            set;
        }
        /// <summary>
        ///     True if the phone number is confirmed, default is false
        /// </summary>
        public bool PhoneNumberConfirmed
        {
            get;
            set;
        }
        /// <summary>
        ///     Is two factor enabled for the user
        /// </summary>
        public bool TwoFactorEnabled
        {
            get;
            set;
        }
        /// <summary>
        ///     DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public DateTime? LockoutEndDateUtc
        {
            get;
            set;
        }
        /// <summary>
        ///     Is lockout enabled for this user
        /// </summary>
        public bool LockoutEnabled
        {
            get;
            set;
        }
        /// <summary>
        ///     Used to record failures for the purposes of lockout
        /// </summary>
        public int AccessFailedCount
        {
            get;
            set;
        }
        /// <summary>
        ///     Navigation property for user roles
        /// </summary>
        public ICollection<SysUserRole> Roles
        {
            get;
            private set;
        }
        /// <summary>
        ///     Navigation property for user claims
        /// </summary>
        public ICollection<SysUserClaim> Claims
        {
            get;
            private set;
        }
        /// <summary>
        ///     Navigation property for user logins
        /// </summary>
        public ICollection<SysUserLogin> Logins
        {
            get;
            private set;
        }

        [NotMappedAttribute]
        public List<SysMenu> Menus
        {
            get;
            set;
        }

        /// <summary>
        ///     User ID (Primary Key)
        /// </summary>
        public override int Id
        {
            get;
            set;
        }
        /// <summary>
        ///     User name
        /// </summary>
        public string UserName
        {
            get;
            set;
        }
        public string UserTrueName
        {
            get;
            set;
        }
        public String UserId { get; set; }

        public string Department { get; set; }
        /// <summary>
        ///     Constructor
        /// </summary>
        public SysUser()
        {
            this.Claims = new List<SysUserClaim>();
            this.Roles = new List<SysUserRole>();
            this.Logins = new List<SysUserLogin>();
        }

        //
        //[Column("UserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
        //public String LillyId { get; set; }

        public bool? IsDeleted { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<SysUser, int> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }


    }
}
