using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts.ViewModel
{
    public class UserBehaviorActivityReportView : IViewModel
    {
        public Int32 Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// 成员所属部门id列表
        /// </summary>
        public int[] department { get; set; }
        /// <summary>
        /// <summary>
        /// 性别。gender=1表示男，=2表示女 
        /// </summary>
        public string gender { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 关注状态: 1=已关注，2=已冻结，4=未关注 
        /// </summary>
        public string status { get; set; }
     
        public string CreatedTime { get; set; }
        public List<UserBehaviorActivityReportView> reportList { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (UserBehavior)obj;
            Id = entity.Id;
                      
            return this;
        }

        
    }
}
