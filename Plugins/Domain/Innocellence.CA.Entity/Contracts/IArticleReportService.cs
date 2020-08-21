using System;
using System.Collections.Generic;
using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;
using Infrastructure.Utility.Data;
using DLYB.CA.Contracts.ViewModel;
using System.Linq.Expressions;

namespace DLYB.CA.Contracts
{
    public interface IArticleReportService : IDependency, IBaseService<ArticleReport>
    {
        List<ArticleReportView> QueryTable(DateTime stardate, DateTime enddate, PageCondition pageCondition);
        List<T> GetListByDate<T>(Expression<Func<ArticleReport, bool>> predicate) where T : IViewModel, new();
    }
}
