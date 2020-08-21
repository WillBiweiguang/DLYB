using System.Transactions;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using DLYB.CA.Contracts;
using DLYB.CA.Entity;
using DLYB.CA.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;


namespace DLYB.CA.Services
{
    public partial class PerformanceLoggingService : BaseService<PerformanceLogging>, IPerformanceLoggingService
    {
        public void InsertWithTransaction(PerformanceLogging performance)
        {
            using (var trans = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                Repository.Insert(performance);

                trans.Complete();
            }
        }
    }
}