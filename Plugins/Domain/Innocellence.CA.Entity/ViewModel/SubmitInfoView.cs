using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Contracts.ViewModel
{
    public partial class SubmitInfoView : IViewModel
    {
        private SubmitInfo obj;

        public Int32? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (SubmitInfo)obj;
            Id = entity.Id;
            Name = entity.Name;
            Email = entity.Email;
            Tel = entity.Tel;

            return this;
        }
       
    }
}
