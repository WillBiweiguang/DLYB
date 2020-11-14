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
    public interface IWeldCategoryLabelingService : IDependency, IBaseService<WeldCategoryLabeling> {
        List<WeldCategoryLabelingView> GetWeldCategoryQuerys();
    }
}
