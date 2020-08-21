using Infrastructure.Core;
using System.ComponentModel;

namespace DLYB.CA.Contracts.ViewModel
{
    public class AppUserView : IViewModel
    {
        public string UserName { get; set; }

        public string LillyId { get; set; }

        public string EmailName { get; set; }

        public string Position { get; set; }

        public string MobileNumber { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TagView
    {
        public string LillyId { get; set; }

        [DescriptionAttribute("名称")]
        public string Name { get; set; }

        [DescriptionAttribute("部门")]
        public string DepartmentName { get; set; }

        public string Type { get; set; }

        public int DepartmentId { get; set; }

        public string Avatar { get; set; }
    }

    public enum TagType
    {
        User,
        Department
    }
}
