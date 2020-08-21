using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.CA.Entity;
using DLYB.CA.Contracts.Entity;
using System.ComponentModel;

namespace DLYB.CA.Contracts.ViewModel
{
    public class UserBehaviorArticleReportView : IViewModel
    {
        public Int32 Id { get; set; }
        public string UserId { get; set; }

        [DescriptionAttribute("用户名")]
        public string UserName { get; set; }
        /// <summary>
        /// 成员所属部门id列表
        /// </summary>
        public int[] department { get; set; }
        public List<string> deptLvs { get; set; }

        [DescriptionAttribute("一级部门")]
        public string deptLv1
        {
            get
            {
                if (deptLvs != null && deptLvs.Count() > 0)
                {
                    return deptLvs[2];
                }
                else
                {
                    return string.Empty;
                }
            }
            set { }
        }

        [DescriptionAttribute("二级部门")]
        public string deptLv2
        {
            get
            {
                if (deptLvs != null && deptLvs.Count() > 1)
                {
                    return deptLvs[3];
                }
                else
                {
                    return string.Empty;
                }
            }
            set { }
        }

        [DescriptionAttribute("三级部门")]
        public string deptLv3
        {
            get
            {
                if (deptLvs != null && deptLvs.Count() > 2)
                {
                    return deptLvs[4];
                }
                else
                {
                    return string.Empty;
                }
            }
            set { }
        }

        /// <summary>
        /// 性别。gender=1表示男，=2表示女 
        /// </summary>
        [DescriptionAttribute("性别")]
        public string gender { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [DescriptionAttribute("邮箱")]
        public string email { get; set; }
        /// <summary>
        /// 关注状态: 1=已关注，2=已冻结，4=未关注 
        /// </summary>
        [DescriptionAttribute("状态")]
        public string status { get; set; }
        /// <summary>
        /// 活跃度
        /// </summary>
        [DescriptionAttribute("活跃度")]
        public int followCount { get; set; }

        [DescriptionAttribute("创建时间")]
        public string CreatedTime { get; set; }

        public Int32? AppId { get; set; }
        public string AppName { get; set; }
        public string Content { get; set; }

        [DescriptionAttribute("文章标题")]
        public string ArticleTitle { get; set; }
        public int? ContentType { get; set; }

        [DescriptionAttribute("菜单key")]
        public string MenuKey { get; set; }

        [DescriptionAttribute("菜单名称")]
        public string MenuName { get; set; }
        public string FunctionId { get; set; }
        public Boolean NeedDelete { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (UserBehavior)obj;
            Id = entity.Id;
            CreatedTime = entity.CreatedTime == null ? "" : ((DateTime)entity.CreatedTime).ToString("yyyy-MM-dd");
            UserId = entity.UserId;
            ContentType = entity.ContentType;
            AppId = entity.AppId;
            Content = entity.Content;
            FunctionId = entity.FunctionId;
            return this;
        }


    }
}
