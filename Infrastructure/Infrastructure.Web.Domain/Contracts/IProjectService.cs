using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;

namespace Infrastructure.Web.Domain.Contracts
{
    public interface IProjectService : IDependency, IBaseService<Project>
    {
        IList<HanjiProportionModel> HancaiQuanlityByType(LiMaiApiViewModel query);
        IList<HanjiProportionModel> HancaiQuanlity(LiMaiApiViewModel query);
    }
}
