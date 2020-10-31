using Infrastructure.Core;
using Infrastructure.Core.Logging;
using Infrastructure.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLYB.Web.Controllers.API
{
    public class ApiBaseController<T, T1> : ParentController<T, T1>
        where T : EntityBase<int>, new()
        where T1 : IViewModel, new()
    {
        public ILogger Logger { get; set; }
        public ApiBaseController(IBaseService<T> newsService) : base(newsService)
        {
            Logger = LogManager.GetLogger(this.GetType());
        }
    }
}