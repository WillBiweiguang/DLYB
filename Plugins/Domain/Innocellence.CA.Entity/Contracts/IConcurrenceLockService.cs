using System;
using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;

namespace DLYB.CA.Contracts.Contracts
{
    public interface IConcurrenceLockService : IDependency, IBaseService<ConcurrenceLockEntity>
    {
        /// <summary>
        /// try to get 10 times lock, if  fail return false
        /// </summary>
        /// <param name="code"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        bool HandlerConcurrence(string code, Action handler);
    }
}
