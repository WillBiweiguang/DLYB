using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using DLYB.CA.Entity;

namespace DLYB.CA.Contracts
{
    public interface IImageInfoService : IDependency, IBaseService<ImageInfo>
    {
        List<T> GetListByParent<T>(int ownerId) where T : IViewModel, new();
    }
}
