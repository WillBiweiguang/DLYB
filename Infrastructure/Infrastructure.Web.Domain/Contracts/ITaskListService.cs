﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;

namespace Infrastructure.Web.Domain.Contracts
{
    public interface ITaskListService : IDependency, IBaseService<TaskList> { }
}
