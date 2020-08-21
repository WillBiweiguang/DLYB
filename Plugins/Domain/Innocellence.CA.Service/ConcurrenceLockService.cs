using System;
using System.Threading;
using EntityFramework.Extensions;
using Infrastructure.Core.Data;
using Infrastructure.Core.Logging;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;
using System.Linq;

namespace DLYB.CA.Service
{
    public class ConcurrenceLockService : BaseService<ConcurrenceLockEntity>, IConcurrenceLockService
    {
        private string _code;
        private const int MaxLoopCount = 10;
        private readonly ILogger _logger = LogManager.GetLogger(typeof(ConcurrenceLockService));

        public bool HandlerConcurrence(string code, Action handler)
        {
            _code = code;
            var isOk = true;

            var index = 0;
            while (true)
            {
                index++;

                if (Repository.Entities.Where(x => x.Code == _code && !x.IsLocked).Update(x => new ConcurrenceLockEntity { IsLocked = true }) == 1)
                {
                    try
                    {
                        handler();
                    }
                    catch (Exception)
                    {
                        isOk = false;
                        throw;
                        //_logger.Error("Concurrence lock error: code {0}, message: {1}.{2} stack {3}", _code, e.InnerException == null ? e.Message : e.InnerException.Message, Environment.NewLine, e.StackTrace);
                    }
                    finally
                    {
                        ReleaseLock();
                    }
                    break;
                }

                if (index >= MaxLoopCount)
                {
                    isOk = false;
                    break;
                }
                Thread.Sleep(1000);
            }

            return isOk;
        }

        private void ReleaseLock()
        {
            if (!string.IsNullOrWhiteSpace(_code))
            {
                Repository.Entities.Where(x => x.Code == _code && x.IsLocked).Update(x => new ConcurrenceLockEntity { IsLocked = false });
            }
        }
    }
}
