using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core
{
    public interface IViewModel : IEntity
    {
        IViewModel ConvertAPIModel(object model);
        
    }
}
