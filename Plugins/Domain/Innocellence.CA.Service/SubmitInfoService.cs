﻿using Infrastructure.Core.Data;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Service
{
    public class SubmitInfoService : BaseService<SubmitInfo>, ISubmitInfoService
    {
        public SubmitInfoService():base("CAAdmin")
        {

        }
    }
}
