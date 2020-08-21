using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;
namespace Infrastructure.Web.Domain.Contracts
{
    public interface IReleaseNoteService : IDependency, IBaseService<ReleaseNote>
    {
    }
}
