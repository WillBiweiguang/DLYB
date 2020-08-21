using System.IO;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Extensions;
using Infrastructure.Web.Net.WebPull.Images;
using DLYB.CA.Contracts;
using DLYB.CA.Contracts.ViewModel;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Service
{
    public class ReportService : BaseService<UserBehavior>, IReportService
    {
        public ReportService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }

    }
}