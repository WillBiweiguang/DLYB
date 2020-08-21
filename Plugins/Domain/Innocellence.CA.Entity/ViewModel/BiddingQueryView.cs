using Infrastructure.Core;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;
namespace DLYB.CA.ModelsView
{
    public partial class BiddingQueryView : IViewModel
    {
        public int Id { get; set; }
        // Lilly 相关治疗领域
        public String TherapArea { get; set; }
        // 品牌名
        public String Brand { get; set; }
        // 化学名
        public String Chemical { get; set; }
        // 省或者军区
        public String Province { get; set; }
        //类型
        public string Type { get; set; }
        // EDL
        public String EDL { get; set; }
        /// <summary>
        /// 结果列表
        /// </summary>
        public provinceAndDetail ProvinceAndDetail { get; set; }
        public ArmAndDetail ArmAndDetail { get; set; }
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (BiddingQuery)obj;
            Id = entity.Id;
            TherapArea = entity.TherapeuticArea;
            Brand = entity.Brand;
            Chemical = entity.ChemicalName;
            Province = entity.Province;

            return this;
        }
    }
    public class provinceAndDetail
    {
        public provinceAndDetail()
        {
            drugDetail = new List<drugAndDetail>();
            NotdrugDetail = new List<drugAndDetail>();
            provinceSubAndDetail = new List<subAndDetail>();
        }
        // 省
        public String Province { get; set; }

        //基药
        public List<drugAndDetail> drugDetail { get; set; }
        //非基药
        public List<drugAndDetail> NotdrugDetail { get; set; }
        public List<subAndDetail> provinceSubAndDetail { get; set; }
    }
    public class ArmAndDetail
    {
        public ArmAndDetail()
        {
            drugAndDetail = new List<drugAndDetail>();
            NotdrugDetail = new List<drugAndDetail>();
            ArmSubAndDetail1 = new List<subAndDetail>();
        }
        // 军区
        public String Arm { get; set; }

        //基药
        public List<drugAndDetail> drugAndDetail { get; set; }
        //非基药
        public List<drugAndDetail> NotdrugDetail { get; set; }
        public List<subAndDetail> ArmSubAndDetail1 { get; set; }
    }
    public class subAndDetail
    {
        // 市或者子军区
        public String Name { get; set; }
        public subAndDetail()
        {
            dSubDetail = new List<drugAndDetail>();
        }
        public List<drugAndDetail> dSubDetail { get; set; }
    }
    public class drugAndDetail
    {
        // 全名
        public String FullName { get; set; }

        // 生效月份
        public String ValidMonth { get; set; }
        // 状态
        public String Status { get; set; }

    }


}
