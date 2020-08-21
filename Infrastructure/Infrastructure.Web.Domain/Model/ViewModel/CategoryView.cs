using System;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    //[Table("Category")]
    public partial class CategoryView : IViewModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// IdEn
        /// </summary>
        public Int32? IdEN { get; set; }

        public String CategoryCode { get; set; }
        public String CategoryName { get; set; }
        public Int32? AppId { get; set; }
        public String CategoryNameCN { get; set; }
        public String Role { get; set; }
        public String Function { get; set; }
        
        public String LanguageCode { get; set; }
        public String CategoryDesc { get; set; }
        public Int32? ParentCode { get; set; }
        public Boolean? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String CreatedUserID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public String UpdatedUserID { get; set; }
        public string NoRoleMessage { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsVirtual { get; set; }
        public int? CategoryOrder { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (Category)obj;
            Id = entity.Id;
            CategoryCode = entity.CategoryCode;
            CategoryName = entity.CategoryName;
            AppId = entity.AppId;
            LanguageCode = entity.LanguageCode;
            CategoryDesc = entity.CategoryDesc;
            ParentCode = entity.ParentCode;
            Role = entity.Role;
            IsDeleted = entity.IsDeleted;
            CreatedDate = entity.CreatedDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            NoRoleMessage = entity.NoRoleMessage;
            IsAdmin = entity.IsAdmin;
            IsVirtual = entity.IsVirtual;
            CategoryOrder = entity.CategoryOrder;
            return this;
        }
    }
}
