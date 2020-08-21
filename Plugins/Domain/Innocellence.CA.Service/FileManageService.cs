using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using DLYB.CA.Contracts;
using DLYB.CA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace DLYB.CA.Services
{
    public partial class FileManageService : BaseService<FileManage>, IFileManageService
    {
        public FileManageService()
            : base("CAAdmin")
        {

        }
                        
    }
}