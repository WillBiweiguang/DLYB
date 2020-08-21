using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.CA.Entity;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.ViewModel
{
    public class UserBehaciorChanelReport : IViewModel
    {
        public Int32 Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// 成员所属部门id列表
        /// </summary>
        public int[] department { get; set; }
        public List<string> deptLvs { get; set; }
        public string deptLv1 { 
            get
        {
            if (deptLvs!=null&&deptLvs.Count() > 0)
            {
                return deptLvs[0];
            }
            else
            {
                return string.Empty;
            }
        }
            set { }
        }
        public string deptLv2
        {
            get
            {
                if (deptLvs != null && deptLvs.Count() > 1)
                {
                    return deptLvs[1];
                }
                else
                {
                    return string.Empty;
                }
            }
            set { }
        }
        public string deptLv3
        {
            get
            {
                if (deptLvs != null && deptLvs.Count() > 2)
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

        /// <summary>
        /// 性别。gender=0表示男，=1表示女 
        /// </summary>
        public Int32? AppId { get; set; }
        public string AppName { get; set; }
        public DateTime CreatedTime { get; set; }
        public int? ContentType { get; set; }
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (UserBehavior)obj;
            
            Id = entity.Id;
            CreatedTime = entity.CreatedTime;
            UserId = entity.UserId;
            ContentType = entity.ContentType;
            AppId = entity.AppId;
            return this;
        }
       
        
    }
}
