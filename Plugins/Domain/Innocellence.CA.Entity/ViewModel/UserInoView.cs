using Infrastructure.Core;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Contracts.ViewModel
{
    public class UserInfoView : IViewModel
    {
        public Int32 Id { get; set; }
        //public string OId { get; set; }
        //public string ActionId { get; set; }
        //public DateTime Time { get; set; }

        public string LillyId { get; set; }
        public string Tel { get; set; }
       
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (UserInfo)obj;
            Id = entity.Id;
            LillyId = entity.LillyId;
            Tel = entity.Tel;
            return this;
        }
    }
}
