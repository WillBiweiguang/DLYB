using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;

namespace Infrastructure.Web.Domain.Entity
{
    //[Table("Category")]
    public partial class Category : EntityBase<int>
    {

        public override Int32 Id { get; set; }

        public String CategoryCode { get; set; }

        public Int32? AppId { get; set; }
        public String CategoryName { get; set; }
        
        /// <summary>
        /// 如果某个菜单只对某个标签的人可用，就用这个控制。
        /// 目前这个还没使用。
        /// </summary>
        public String Role { get; set; }

        /// <summary>
        /// 考虑后期如果可以做到的话，用户点击某个菜单后需要执行哪个函数或者什么代码，可以放在这里实现。
        /// </summary>
        public String Function { get; set; }

        //public String Extra1 { get; set; }
        public String LanguageCode { get; set; }
        public String CategoryDesc { get; set; }
        public Int32? ParentCode { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsVirtual { get; set; }
        public Boolean? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String UpdatedUserID { get; set; }
        public int? CategoryOrder { get; set; }

        /// <summary>
        /// </summary>
        public string NoRoleMessage { get; set; }

    }
}
