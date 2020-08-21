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
        /// ���ĳ���˵�ֻ��ĳ����ǩ���˿��ã�����������ơ�
        /// Ŀǰ�����ûʹ�á�
        /// </summary>
        public String Role { get; set; }

        /// <summary>
        /// ���Ǻ���������������Ļ����û����ĳ���˵�����Ҫִ���ĸ���������ʲô���룬���Է�������ʵ�֡�
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
