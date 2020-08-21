using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Data;
using Infrastructure.Core.Infrastructure;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Service
{
    public class DictionaryService : BaseService<DictionaryEntity>, IDictionaryService
    {
        private static readonly ICacheManager _cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(DictionaryService)));

        public static IList<DictionaryEntity> QueryList(Func<DictionaryEntity, bool> query)
        {
            var list = getList();
            return list.Where(query).ToList();
        }

        private static IEnumerable<DictionaryEntity> getList()
        {
            BaseService<DictionaryEntity> ser = new BaseService<DictionaryEntity>();
            return _cacheManager.Get("GlobleDictionary", () => ser.Repository.Entities.ToList());
        }
    }
}
