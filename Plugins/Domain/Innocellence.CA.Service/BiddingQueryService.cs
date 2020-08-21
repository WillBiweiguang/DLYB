using Infrastructure.Core;
using Infrastructure.Core.Data;
using DLYB.CA.Contracts;
using DLYB.CA.Entity;
using DLYB.CA.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace DLYB.CA.Services
{
    public partial class BiddingQueryService : BaseService<BiddingQuery>, IBiddingQueryService
    {
        public BiddingQueryService()
            : base("CAAdmin")
        {

        }

        public List<BiddingQueryView> GetBiddingQuerys()
        {
            return Repository.Entities.Select(x => new BiddingQueryView()
            {
                TherapArea = x.TherapeuticArea,
                Brand = x.Brand,
                Province = x.Province
            }).Distinct().ToList();
        }

        public List<T> GetSearchResult<T>(Expression<Func<BiddingQuery, bool>> predicate) where T : IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }

        public BiddingQueryView GetBiddingQueryConditions(BiddingQueryView condition)
        {
            BiddingQueryView result = new BiddingQueryView();
            var BiddingQueryList = Repository.Entities.Where(x => condition.TherapArea.Equals(x.TherapeuticArea) &&
                 condition.Brand.Equals(x.Brand) &&
                 condition.Province.Equals(x.Province)).ToList();

            if (!BiddingQueryList.Any())
            {
                return null;
            }
            result.Brand = BiddingQueryList[0].Brand;
            result.Province = BiddingQueryList[0].Province;
            result.TherapArea = BiddingQueryList[0].TherapeuticArea;
            result.Chemical = BiddingQueryList[0].ChemicalName;
            result.EDL = BiddingQueryList[0].EDL;

            result.ProvinceAndDetail = new provinceAndDetail();
            result.ArmAndDetail = new ArmAndDetail();
            List<subAndDetail> ASubDetail1 = new List<subAndDetail>();
            foreach (var item in BiddingQueryList)
            {
                drugAndDetail drug = new drugAndDetail();
                drug.FullName = item.FullName;
                drug.Status = item.Status;
                drug.ValidMonth = item.ValidMonth;

                if (item.Province.Contains("军"))
                {
                    result.Type = "1";

                    subAndDetail arm = new subAndDetail();
                    result.ArmAndDetail.Arm = item.Province;
                    if (string.IsNullOrEmpty(item.City))
                    {
                        if (item.EDL == "基药目录")
                        {
                            result.ArmAndDetail.drugAndDetail.Add(drug);
                        }
                        else
                        {
                            result.ArmAndDetail.NotdrugDetail.Add(drug);
                        }
                    }
                    else
                    {

                        arm.Name = item.City;
                      
                        arm.dSubDetail.Add(drug);

                        foreach (var item1 in ASubDetail1)
                        {
                            if (item1.Name == arm.Name)
                            {
                                item1.dSubDetail.Add(drug);
                                arm = item1;
                                ASubDetail1.Remove(arm);
                                break;
                            }

                        }
                        ASubDetail1.Add(arm);
                        result.ArmAndDetail.ArmSubAndDetail1 = ASubDetail1;
                    }


                }
                else
                {
                    result.Type = "2";

                    subAndDetail pro = new subAndDetail();
                    result.ProvinceAndDetail.Province = item.Province;
                    if (string.IsNullOrEmpty(item.City))
                    {
                        if (item.EDL == "基药目录")
                        {
                            result.ProvinceAndDetail.drugDetail.Add(drug);
                        }
                        else
                        {
                            result.ProvinceAndDetail.NotdrugDetail.Add(drug);
                        }

                    }
                    else
                    {

                        pro.Name = item.City;
                      
                        pro.dSubDetail.Add(drug);
                        foreach (var item1 in ASubDetail1)
                        {
                            if (item1.Name == pro.Name)
                            {
                                item1.dSubDetail.Add(drug);
                                pro = item1;
                                ASubDetail1.Remove(pro);
                                break;
                            }

                        }
                        ASubDetail1.Add(pro);
                        result.ProvinceAndDetail.provinceSubAndDetail = ASubDetail1;
                    }

                }
            }
            return result;
        }
    }
}