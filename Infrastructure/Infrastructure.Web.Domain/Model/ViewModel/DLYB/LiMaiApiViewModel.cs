using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infrastructure.Web.Domain.ModelsView
{
    public class LiMaiApiViewModel
    {
        public string DepartmentId { get; set; }
        public string ProjectName { get; set; }
        public string BridgeType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class HanjiProportionModel
    {
        public string DepartmentID { get; set; }
        public string ProjectName { get; set; }
        public string BridgeType { get; set; }
        public string HanjiType { get; set; }
        public double HancaiTotal { get; set; }
    }

    public class HancaiProportion
    {
        public string ProjectTotal { get; set; }
        public double HancaiTotal { get; set; }
    }
}