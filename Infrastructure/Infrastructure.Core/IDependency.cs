using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core
{
    /// <summary>
    /// 依赖注入接口，表示该接口的实现类将自动注册到IoC容器中
    /// </summary>
    public interface IDependency
    {
    }

    /// <summary>
    /// Base interface for services that are instantiated per shell/tenant.
    /// </summary>
    public interface ISingletonDependency : IDependency
    {
    }

    /// <summary>
    /// Base interface for services that may *only* be instantiated in a unit of work.
    /// This interface is used to guarantee they are not accidentally referenced by a singleton dependency.
    /// </summary>
    public interface IUnitOfWorkDependency : IDependency
    {
    }

}
