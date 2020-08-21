using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DLYB.Weixin.Containers;
using Infrastructure.Core;
using DLYB.Weixin.CommonAPIs;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using DLYB.Weixin.Entities;

namespace DLYB.Weixin.Cache
{


    /// <summary>
    /// 本地容器缓存策略
    /// </summary>
    public sealed class DBContainerCacheStrategy : IContainerCacheStragegy
    //where TContainerBag : class, IBaseContainerBag, new()
    {
        #region 数据源

       // private IDictionary<string, IBaseContainerBag> _cache = LocalCacheHelper.LocalCache;

        public IUnitOfWork _unitOfWork { get; set; }

        #endregion

        #region 单例

        /// <summary>
        /// LocalCacheStrategy的构造函数
        /// </summary>
        DBContainerCacheStrategy()
        {
        }


        //静态LocalCacheStrategy
        public static IContainerCacheStragegy Instance
        {
            get
            {
                return Nested.instance;//返回Nested类中的静态成员instance
            }
        }

        class Nested
        {
            static Nested()
            {
            }
            //将instance设为一个初始化的LocalCacheStrategy新实例
            internal static readonly DBContainerCacheStrategy instance = new DBContainerCacheStrategy();
        }


        #endregion

        #region ILocalCacheStrategy 成员

        public string CacheSetKey { get; set; }


        public void InsertToCache(string key, IBaseContainerBag value)
        {
            if (key == null || value == null)
            {
                return;
            }
          //  _cache[key] = value;

           // aa.Update(value);

           // InsertItem(value);
        }

        public void RemoveFromCache(string key)
        {
           // _cache.Remove(key);
           // DeleteItem(key);

            var keys = key.Split("_".ToCharArray());

            AccessTokenBag parameters = new AccessTokenBag{
             CorpId=keys[0],  CorpSecret=keys[1]
            };

            Update(key, parameters);
        }

        public IBaseContainerBag Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (!CheckExisted(key))
            {
                return null;
                //InsertToCache(key, new ContainerItemCollection());
            }

           // return _cache[key];

           // _dbSet.Select(a=>a.)

           // dynamic item= GetItem(key);

            var keys = key.Split("_".ToCharArray());

            SqlParameter[] parameters = new SqlParameter[]{
            new SqlParameter("WeixinCorpId",keys[0]),
             new SqlParameter("WeixinCorpSecret",keys[1])
            };

            var item = ((DbContext)_unitOfWork).Database.SqlQuery<SysWechatConfig>("select * from SysWechatConfig where WeixinCorpId=@WeixinCorpId and WeixinCorpSecret=@WeixinCorpSecret", parameters).FirstOrDefault();

            return new AccessTokenBag() {
                 CorpId = item.WeixinCorpId,
                TokenResult = new Entities.AccessTokenResult() { access_token = item.AccessToken },
                ExpireTime = item.AccessTokenExpireTime.Value,
                 CorpSecret = item.WeixinCorpSecret,
                Key = key
            };

        }

        public IDictionary<string, IBaseContainerBag> GetAll()
        {
            return null;// GetItem("");// _cache;
        }

        public bool CheckExisted(string key)
        {
            return  true;
        }

        public long GetCount()
        {
            return 0;
        }

        public void Update(string key, IBaseContainerBag value)
        {
            //_cache[key] = value;

            //dynamic item = GetItem(key);

            if (value is BaseContainerBag<AccessTokenResult>)
            {
                var obj = (BaseContainerBag<AccessTokenResult>)value;

                SqlParameter[] parameters = new SqlParameter[]{
            new SqlParameter("WeixinCorpId",obj.CorpId),
             new SqlParameter("WeixinCorpSecret",obj.CorpSecret),
              new SqlParameter("AccessTokenExpireTime",obj.ExpireTime),
               new SqlParameter("AccessToken",((AccessTokenBag)obj).TokenResult.access_token)
            };

                ((DbContext)_unitOfWork).Database.ExecuteSqlCommand("update SysWechatConfig set AccessToken=@AccessToken,AccessTokenExpireTime=@AccessTokenExpireTime  where WeixinCorpId=@WeixinCorpId and WeixinCorpSecret=@WeixinCorpSecret",
                    parameters);
            }
            else
            {
                var obj = (BaseContainerBag<JsApiTicketResult>)value;

                SqlParameter[] parameters = new SqlParameter[]{
            new SqlParameter("WeixinCorpId",obj.CorpId),
             new SqlParameter("WeixinCorpSecret",obj.CorpSecret),
              new SqlParameter("TicketTokenExpireTime",obj.ExpireTime),
               new SqlParameter("TicketToken",((JsApiTicketBag)obj).TokenResult.ticket)
            };

                ((DbContext)_unitOfWork).Database.ExecuteSqlCommand("update SysWechatConfig set TicketToken=@TicketToken,TicketTokenExpireTime=@TicketTokenExpireTime  where WeixinCorpId=@WeixinCorpId and WeixinCorpSecret=@WeixinCorpSecret",
                    parameters);
            }

           

        }

        public void UpdateContainerBag(string key, IBaseContainerBag bag)
        {
            Update(key, bag);
        }

        #endregion
    }


    public class SysWechatConfig 
    {

        public  Int32 Id { get; set; }

        public String WeixinToken { get; set; }
        public String WeixinEncodingAESKey { get; set; }

        public String WeixinCorpId { get; set; }
        public String WeixinCorpSecret { get; set; }

        //是否企业号
        public String IsCorp { get; set; }

        public String WeixinAppId { get; set; }

        public String AccessToken { get; set; }
        public DateTime? AccessTokenExpireTime { get; set; }


        public Boolean? IsDeleted { get; set; }


    }
}
