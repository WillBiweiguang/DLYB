using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Contracts
{
    public interface IDictionaryService : IDependency, IBaseService<DictionaryEntity>
    {
        //IList<DictionaryEntity> QueryList(Func<DictionaryEntity, bool> query);
    }
}
