using DLYB.CA.Contracts.Contracts;
using WebBackgrounder;

namespace DLYB.CA.Service.job
{
    public interface ICustomerJob : IJob
    {
        void ManuallyRunJob();

        JobName JobName { get;  }

        bool Success { get; }
    }
}
