using System.Threading.Tasks;
using Infrastructure.Core.Data;
using System.Collections.Generic;
using System.Linq;
using System;
using Infrastructure.Core.Logging;
using Infrastructure.Core.Caching;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Core.Infrastructure;
using DLYB.CA.Entity;
using Autofac;
using DLYB.Weixin.QY.AdvancedAPIs.App;
using DLYB.Weixin.QY.AdvancedAPIs.MailList;
using DLYB.Weixin.QY.CommonAPIs;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Service;
using DLYB.Weixin.QY.AdvancedAPIs;
using DLYB.CA.Contracts.CommonEntity;


namespace DLYB.CA.Service.Common
{
    public class WeChatCommonService : ICommonService
    {
        // public ILogger Logger { get; set; }

        private static object objLock = new object();

        private static object objLockSys = new object();

        private static object objLockWeChat = new object();
        private static ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(WeChatCommonService)));

        //将所有成员列表加入到缓存中去
        public static List<GetMemberResult> lstUser
        {
            get
            {
                var lst = cacheManager.Get<List<GetMemberResult>>("UserItem", () =>
                {
                    var Config = WeChatCommonService.GetWeChatConfig(0);
                    string accessToken = AccessTokenContainer.TryGetToken(Config.WeixinCorpId, Config.WeixinCorpSecret);
                    var userlist = MailListApi.GetDepartmentMemberInfo(accessToken, 1, 1, 0).userlist;
                    return userlist;
                });
                return lst;
            }
        }

        public static List<EmployeeInfoWithDept> lstUserWithDeptTag
        {
            get
            {
                var lst = cacheManager.Get<List<EmployeeInfoWithDept>>("UserWithDeptTagItem", () =>
                {
                    var Config = WeChatCommonService.GetWeChatConfig(0);
                    string accessToken = AccessTokenContainer.TryGetToken(Config.WeixinCorpId, Config.WeixinCorpSecret);
                    var userlist = MailListApi.GetDepartmentMemberInfo(accessToken, 1, 1, 0).userlist;
                    var userWithDeptTagList = new List<EmployeeInfoWithDept>();

                    var tags = lstTag;

                    // 取人
                    foreach (var getMemberResult in userlist)
                    {
                        userWithDeptTagList.Add(new EmployeeInfoWithDept()
                        {
                            avatar = getMemberResult.avatar,
                            department = getMemberResult.department.ToList(),
                            deptLvs = getMemberResult.deptLvs,
                            email = getMemberResult.email,
                            gender = getMemberResult.gender.ToString(),
                            mobile = getMemberResult.mobile,
                            name = getMemberResult.name,
                            position = getMemberResult.position,
                            status = getMemberResult.status,
                            userid = getMemberResult.userid,
                            weixinid = getMemberResult.weixinid
                        });
                    }

                    // 更新人的标签，默认为N
                    foreach (var us in userWithDeptTagList)
                    {
                        us.tags = new Dictionary<string, string>();
                        foreach (var t in tags)
                        {
                            us.tags[t.tagid] = "N";
                        }
                    }

                    // 获取每个标签里的人，给人打标签
                    foreach (var tagitem in tags)
                    {
                        var tagUsers = GetTagMembers(int.Parse(tagitem.tagid), 0).userlist;
                        foreach (var u in tagUsers)
                        {
                            var employee = userWithDeptTagList.FirstOrDefault(a => a.userid.ToUpper() == u.userid.ToUpper());
                            if (employee != null)
                            {
                                employee.tags[tagitem.tagid] = "Y";
                            }
                        }

                    }

                    // 获取部门列表
                    var departments = lstDepartment;
                    // 处理部门关系，暂定只有5层
                    foreach (var dept in departments)
                    {
                        int level = 1;
                        int parentId = dept.parentid;
                        while (parentId != 0)
                        {
                            var parentDept = departments.FirstOrDefault(t => t.id == parentId);
                            if (parentDept != null)
                            {
                                level++;
                                parentId = parentDept.parentid;
                            }
                        }
                        dept.level = level;
                    }

                    // 更新员工信息，把部门的名称填入
                    foreach (var emp in userWithDeptTagList)
                    {
                        // 如果有多个部门的话，只列出其中一个部门。
                        if (emp.department.Count() >= 1)
                        {
                            var dept = departments.FirstOrDefault(t => t.id == emp.department[0]);
                            if (dept != null)
                            {
                                int level = dept.level;
                                while (level > 1)
                                {
                                    emp.deptLvs[level] = dept.name;
                                    dept = departments.FirstOrDefault(t => t.id == dept.parentid);
                                    level--;
                                }
                            }
                            if (emp.department.Count() > 1)
                            {
                                emp.deptLvs[2] = "*" + emp.deptLvs[2];
                            }
                        }
                    }


                    return userWithDeptTagList;

                });
                return lst;
            }
        }

        public static List<TagItem> lstTag
        {
            get
            {
                var lst = cacheManager.Get<List<TagItem>>("TagItem", () =>
                {
                    var Config = WeChatCommonService.GetWeChatConfig(0);
                    string accessToken = AccessTokenContainer.TryGetToken(Config.WeixinCorpId, Config.WeixinCorpSecret);
                    var tagList = MailListApi.GetTagList(accessToken).taglist;

                    return tagList;
                });

                return lst;
            }
        }

        public static List<WeChatEmotion> lstWechatEmotion
        {
            get
            {
                var lst = cacheManager.Get<List<WeChatEmotion>>("WechatEmotionItem", () =>
                {
                    var str = CommonService.GetSysConfig("WechatEmotion", "[]");
                    try
                    {
                        var emotionList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WeChatEmotion>>(str);
                        return emotionList;
                    }
                    catch (Exception ex)
                    {
                        return new List<WeChatEmotion>();
                    }
                });
                return lst;
            }
        }

        private static ILogger Logger = LogManager.GetLogger(typeof(WeChatCommonService));

        public static void ClearCache(int iType)
        {
            if (iType == 1)
            {
                //CacheManager.GetCacher("Default").Remove("Category");
            }
            else if (iType == 2)
            {
                //CacheManager.GetCacher("Default").Remove("SysConfig");
            }

        }

        public static SysWechatConfig GetWeChatConfig(int iAppID = 0)
        {
            var obj = CommonService.lstSysWeChatConfig.Find(a => a.WeixinAppId == iAppID.ToString());

            if (obj == null)
            {
                return CommonService.lstSysWeChatConfig.Find(a => a.WeixinAppId == "0");
            }

            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="getNewToken"></param>
        /// <returns></returns>
        public static string GetWeiXinToken(int appId, bool getNewToken = false)
        {
            if (appId < 0)
            {
                throw new ArgumentNullException("appId");
            }

            var config = GetWeChatConfig(appId);
            return AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret, getNewToken);
        }

        public static List<DepartmentList> lstDepartment
        {
            get
            {
                Logger.Debug("Getting lstDepartment");
                var lst = cacheManager.Get<List<DepartmentList>>("DepartmentList", () =>
                {
                    Logger.Debug("cache is empty, creating cache....");
                    var config = WeChatCommonService.GetWeChatConfig(0);
                    var strToken = AccessTokenContainer.TryGetToken(config.WeixinCorpId, config.WeixinCorpSecret);
                    var result = MailListApi.GetDepartmentList(strToken);
                    return result.department;
                });
                Logger.Debug("returning lstDepartment");
                return lst;
            }
        }

        public static void RemoveDepartmentFromCache()
        {
            cacheManager.Remove("DepartmentList");
        }

        /// <summary>
        /// 建议走缓存
        /// </summary>
        /// <param name="iAppID"></param>
        /// <returns></returns>
        public static GetAppInfoResult GetAppInfo(int iAppID)
        {
            var objConfig = GetWeChatConfig(iAppID);

            string strToken = AccessTokenContainer.TryGetToken(objConfig.WeixinCorpId, objConfig.WeixinCorpSecret);

            return AppApi.GetAppInfo(strToken, iAppID);
            // Ret.allow_userinfos
        }

        private static void GetSubDepartments(int parentDepartmentId, IEnumerable<DepartmentList> allDepartments, List<DepartmentList> subDepartments)
        {
            var departments = allDepartments.Where(x => x.parentid == parentDepartmentId || x.id == parentDepartmentId).ToList();
            subDepartments.AddRange(departments);

            if (!departments.Any() || departments.Count == 1) return;

            //Parallel.ForEach(departments.AsParallel().Where(x => x.id != parentDepartmentId), department => GetSubDepartments(department.id, lstDepartment, subDepartments));
            foreach (var department in departments.AsParallel().Where(x => x.id != parentDepartmentId).ToList())
            {
                GetSubDepartments(department.id, lstDepartment, subDepartments);
            }
        }

        public static IEnumerable<DepartmentList> GetSubDepartments(IList<int> parentDepartmentIds)
        {
            var departments = new List<DepartmentList>();
            foreach (var id in parentDepartmentIds)
            {
                GetSubDepartments(id, lstDepartment, departments);
            }
            //Parallel.ForEach(parentDepartmentIds, id => GetSubDepartments(id, lstDepartment, departments));

            return departments.Distinct(new DepartmentCompare());
        }

        public static IEnumerable<DepartmentList> GetSubDepartments(int parentDepartmentId)
        {
            var departments = new List<DepartmentList>();
            GetSubDepartments(parentDepartmentId, lstDepartment, departments);

            return departments.Distinct(new DepartmentCompare());
        }

        public static GetTagMemberResult GetTagMembers(int tagId, int appId)
        {
            return MailListApi.GetTagMember(WeChatCommonService.GetWeiXinToken(appId), tagId);
        }
    }

    public class DepartmentCompare : IEqualityComparer<DepartmentList>
    {
        public bool Equals(DepartmentList x, DepartmentList y)
        {
            return x.id == y.id;
        }

        public int GetHashCode(DepartmentList obj)
        {
            return obj.id;
        }
    }

    public class WeChatEmotion
    {
        public string Code { get; set; }
        public string Target { get; set; }
    }
}
