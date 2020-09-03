using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;

namespace Infrastructure.Web.Domain.ModelsView
{
    public partial class ProjectView : IViewModel
	{	
		public string Id { get;set; }

        public string Name { get; set; }

        public int? type { get; set; }

        public string img_url { get; set; }

        public int? audit_state { get; set; }

        public int? bridge_type_id { get; set; }

        public string design_institute_id { get; set; }

        public string descript { get; set; }

        public string create_by { get; set; }

        public string dept_id { get; set; }

        public string dept_name { get; set; }

        public int? progress_state { get; set; }

        public DateTime create_time { get; set; }

        public DateTime? update_time { get; set; }

        public int? state { get; set; }

        public IViewModel ConvertAPIModel(object obj) {
            var entity = (Project)obj;
            Id = entity.Id;
            Name = entity.Name;
            type = entity.type;
            audit_state = entity.audit_state;
            bridge_type_id = entity.bridge_type_id;
            design_institute_id = entity.design_institute_id;
            descript = entity.descript;
            create_by = entity.create_by;
            dept_id = entity.dept_id;
            dept_name = entity.dept_name;
            progress_state = entity.progress_state;
            create_time = entity.create_time;
            update_time = entity.update_time;
            state = entity.state;
            return this;
        }
	}
}
