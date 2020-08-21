using System;
using System.Collections.Generic;
using Infrastructure.Core;
using DLYB.CA.Entity;
using System.Linq.Expressions;
using DLYB.CA.ModelsView;

namespace DLYB.CA.Contracts
{
    public interface IBiddingQueryService : IDependency, IBaseService<BiddingQuery>
    {
        
        List<BiddingQueryView> GetBiddingQuerys();

        List<T> GetSearchResult<T>(Expression<Func<BiddingQuery, bool>> predicate) where T : IViewModel, new();
        BiddingQueryView GetBiddingQueryConditions(BiddingQueryView condition);
    }
}
