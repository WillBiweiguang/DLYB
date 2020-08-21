using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.CA.Entity;
using DLYB.CA.ModelsView;
namespace DLYB.CA.Contracts
{
    public interface IFaqInfoService : IDependency, IBaseService<FaqInfo>
    {
        List<FaqInfoView> GetListBySearchKey(int AppId, string key);
        List<FaqInfoView> GetFAQList();
    }
}
