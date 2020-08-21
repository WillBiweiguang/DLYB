using Infrastructure.Core.Data.API.SiteNavs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//======================================================================
//
//        Copyright (C) 2014-2016 Innocellence团队    
//        All rights reserved
//
//        filename :APIContext
//        description :
//
//        created by hy at  2015/1/5 14:11:52
//        
//
//======================================================================
namespace Infrastructure.Core.Data.API
{
    public class APIContext<TUserKeyType> 
    {
        private static readonly Lazy<APIContext<TUserKeyType>> LazyContext = new Lazy<APIContext<TUserKeyType>>(() => new APIContext<TUserKeyType>());
        /// <summary>
        /// 当前全局上下文对象
        /// </summary>
        public static APIContext<TUserKeyType> Current { get { return LazyContext.Value; } }
        public SiteAdminNavigationRepositoryBase<TUserKeyType> SiteAdminNavigationRepository { get; set; }
        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
