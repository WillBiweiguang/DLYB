using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
	//[Table("Push")]
    public partial class DemoView : IViewModel
	{
	
		public Int32 Id { get;set; }

        public String Name { get; set; }
        public DateTime CreateTime
        {
            get;
            set;
        }
        public DateTime UpdateTime { get; set; }
        public int State { get; set; }


        public IViewModel ConvertAPIModel(object obj) {
            var entity = (Demo)obj;
            Id = entity.Id;
            Name = entity.Name;
            CreateTime = entity.CreateTime;
            UpdateTime = entity.UpdateTime;
            State = entity.State;
            return this;
        }
	}
}
