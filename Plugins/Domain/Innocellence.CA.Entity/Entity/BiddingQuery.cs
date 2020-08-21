using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Entity;
namespace DLYB.CA.Entity
{
    public partial class BiddingQuery : EntityBase<int>
    {
        public override int Id { get; set; }
        // Lilly 相关治疗领域
        public String TherapeuticArea { get; set; }
        // 品牌名
        public String Brand { get; set; }
        // 化学名
        public String ChemicalName { get; set; }
       
        // 全名
        public String FullName { get; set; }
        // EDL
        public String EDL { get; set; }

        // 省或者军区
        public String Province { get; set; }

        // 市或者子军区
        public String City { get; set; }

        // 生效月份
        public String ValidMonth { get; set; }
        // 状态
        public String Status { get; set; }
       
    }
}
