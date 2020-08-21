using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EntityFramework.Extensions;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Core.Logging;
using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.Domain.Services;
using DLYB.CA.Contracts;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Service.Common;
using DLYB.Weixin.QY.AdvancedAPIs;
using DLYB.Weixin.QY.AdvancedAPIs.MailList;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Infrastructure.Web.Domain.Contracts;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Autofac;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Contracts.Contracts;

namespace DLYB.CA.Service
{
    /// <summary>
    /// 业务实现——App访问量统计
    /// </summary>
    public abstract class LocalSadService : BaseService<LocalSADEntity>
    {
        protected readonly ISysUserService _sysUserService;
        protected readonly IReportJobLogService ReportJobLogService = EngineContext.Current.Resolve<IReportJobLogService>();
        protected const string ExceptionTagNameKey = "SYS_EXCEPTION";
        protected const string SadConfigTagKey = "LocalSADTAG";
        protected const string NoMatchDepartmentKey = "SADNoMatchDepartmentKey";
        protected const string EmailProxyInfoKey = "EmailProxyInfo";
        protected const string SadInfoKey = "SADEmailInfo";
        protected const string SadReportJobLogDate = "SadReportJobLogDate";
        protected static readonly ILogger Logger = LogManager.GetLogger("wechat");

        protected string credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "SADAPICONNECTION", "LillY2000!")));
        private static ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(WeChatCommonService)));

        public LocalSadService(ISysUserService sysUserService)
        {
            _sysUserService = sysUserService;
        }

        /// <summary>
        /// SyncAddressBook
        /// </summary>
        /// <returns>同步后的当前数量</returns>
        public int SyncAddressBook(DateTime? syncDate = null)
        {
            // 第一步，根据日期到数据库里取出所有当天的数据
            if (syncDate == null)
            {
                syncDate = DateTime.Today;
            }

            var configTag = CommonService.lstSysConfig.FirstOrDefault(x => x.ConfigName == SadConfigTagKey);

            if (configTag == null)
            {
                //throw new DLYBException("Have not config sad tag configration mapping!");
                Logger.Error<string>("Have not config sad tag configration mapping!");
                return -1;
            }

            var configedNoMatchDepartmentModel = CommonService.lstSysConfig.FirstOrDefault(x => x.ConfigName == NoMatchDepartmentKey);

            if (configedNoMatchDepartmentModel == null || string.IsNullOrEmpty(configedNoMatchDepartmentModel.ConfigValue))
            {
                //throw new DLYBException("Have not config sad tag configration mapping!");
                Logger.Error<string>("sad no match department name does not  be configed!");
                return -1;
            }

            var today = syncDate.Value;
            var currentList = Repository.Entities.Where(a => a.SyncTime > today).ToList();

            if (!currentList.Any())
            {
                return 0;
            }

            var depts = WeChatCommonService.lstDepartment;

            var deptDic = depts.AsParallel().ToDictionary(x => x.name);

            if (!currentList.Any())
            {
                return -1;
            }

            //TODO  need to check
            // 第二步，取出企业号当前的所有数据
            var employeeWeChatList = (from emp in WeChatCommonService.lstUser
                                      select new LocalSADEntity
                                      {
                                          Email = emp.email,
                                          ChineseName = emp.name,
                                          LillyID = emp.userid,
                                          //DeptId = emp.department[emp.deptLvs.Count()],
                                          Company = depts.First(a => a.id == emp.department[2]).name,
                                          Department = depts.FirstOrDefault(a => a.id == emp.department[3]) == null ? "" : depts.First(a => a.id == emp.department[3]).name,
                                          SubDepartment = depts.FirstOrDefault(a => a.id == emp.department[4]) == null ? "" : depts.First(a => a.id == emp.department[4]).name,
                                          Gender = (emp.gender == 0 ? "男" : "女")
                                      }).ToList();

            #region handler no match department user
            var notMatchedDepartmentUsers = currentList.AsParallel().Where(x => !deptDic.ContainsKey(x.Company)).ToList();

            var result = HandlerNoMatchDepartmentUser(configedNoMatchDepartmentModel.ConfigValue, notMatchedDepartmentUsers, deptDic);

            if (!result)
            {
                SendEmail();
            }
            #endregion

            currentList = currentList.AsParallel().Where(x => deptDic.ContainsKey(x.Company)).ToList();

            // 第三步，前两个数据对比，检查出有任何变化的数据
            //      1. 数据有变化的，Email, Gender, Company, Department, SubDepartment, SFTitle, IsSupervisor, ContractGroup, BaseLocation
            //      2. 今天比昨天多的数据（要添加到地址簿中）
            //      3. 昨天比今天多的数据（要从地址簿中删除）
            var allTags = MailListApi.GetTagList(WeChatCommonService.GetWeiXinToken(0)).taglist;

            var configExceptionTag = CommonService.lstSysConfig.FirstOrDefault(x => x.ConfigName == ExceptionTagNameKey);
            var exceptionUserIds = new List<string>();

            if (configExceptionTag != null)
            {
                if (!string.IsNullOrEmpty(configExceptionTag.ConfigValue))
                {
                    var exceptionTags = configExceptionTag.ConfigValue.Split(',');
                    foreach (var exceptionTag in exceptionTags.Select(tag => allTags.FirstOrDefault(x => string.Compare(x.tagname, tag, StringComparison.OrdinalIgnoreCase) == 0)).Where(exceptionTag => exceptionTag != null))
                    {
                        exceptionUserIds.AddRange(MailListApi.GetTagMember(WeChatCommonService.GetWeiXinToken(0), int.Parse(exceptionTag.tagid)).userlist.Select(x => x.userid).ToList());
                    }
                }
            }

            //department
            //var newList = new List<LocalSADEntity>();
            var addList = new List<LocalSADEntity>();
            var deleteList = employeeWeChatList.Except(currentList, new LocalSadEntityComparer()).ToList();
            if (exceptionUserIds.Any())
            {
                deleteList = deleteList.Where(x => exceptionUserIds.All(y => x.LillyID != y)).ToList();
            }
            var updateList = new List<LocalSADEntity>();


            #region
            foreach (var newEmployee in currentList)
            {
                // 对比
                var wechatEmployee = employeeWeChatList.FirstOrDefault(a => a.LillyID == newEmployee.LillyID);
                if (wechatEmployee == null)
                {
                    addList.Add(newEmployee);
                    continue;
                }

                if (wechatEmployee.Email != newEmployee.Email
                    || wechatEmployee.ChineseName != newEmployee.ChineseName
                    || wechatEmployee.Gender != newEmployee.Gender
                    || wechatEmployee.Company != newEmployee.Company
                    || wechatEmployee.Department != newEmployee.Department
                    || wechatEmployee.SubDepartment != newEmployee.SubDepartment
                    )
                {
                    if (!exceptionUserIds.Any())
                    {
                        updateList.Add(newEmployee);
                        continue;
                    }

                    if (exceptionUserIds.All(x => x != newEmployee.LillyID))
                    {
                        updateList.Add(newEmployee);
                    }
                }
            }
            #endregion

            #region add
            // 最后，执行
            foreach (var a in addList)
            {
                MailListApi.CreateMember(WeChatCommonService.GetWeiXinToken(0), new GetMemberResult
                {
                    userid = a.LillyID,
                    email = a.Email,
                    gender = (a.Gender == "男" ? 0 : 1),
                    name = a.ChineseName,
                    department = new[] { a.DeptId }
                });
            }
            #endregion

            #region update
            foreach (var a in updateList)
            {
                MailListApi.UpdateMember(WeChatCommonService.GetWeiXinToken(0), new GetMemberResult
                {
                    userid = a.LillyID,
                    email = a.Email,
                    gender = (a.Gender == "男" ? 0 : 1),
                    name = a.ChineseName,
                    department = new[] { a.DeptId }
                });
            }
            #endregion

            #region delete
            foreach (var a in deleteList)
            {
                MailListApi.DeleteMember(WeChatCommonService.GetWeiXinToken(0), a.LillyID);
                //try
                //{
                //    // 被删除的人里面是否有系统管理员，如果有的话一起删除。
                //    // 有没有都删一遍，不查了。
                //    _sysUserService.Repository.Delete(b => b.LillyId == a.LillyID);
                //}
                //catch (Exception)
                //{ }
            }
            var deleteUsers = deleteList.Select(y => y.LillyID).ToList();
            //reduce roundtrip between with db
            _sysUserService.Repository.Entities.Where(x => deleteUsers.Contains(x.LillyId)).Delete();

            #endregion

            var configTagMapping = JsonHelper.FromJson<IList<TagConfig>>(configTag.ConfigValue).Select(x => x.Tags.Select(y => new TagMapping { GroupName = x.GroupName, Tag = y })).SelectMany(x => x).ToList();
            var type = typeof(LocalSADEntity);

            foreach (var tagMapping in configTagMapping)
            {
                var tagId = int.Parse(allTags.First(a => a.tagname == tagMapping.GroupName).tagid);
                var oldTagUsersWx = MailListApi.GetTagMember(WeChatCommonService.GetWeiXinToken(0), tagId).userlist;
                var oldTagUsers = oldTagUsersWx.Select(taguserlist => taguserlist.userid).ToList();

                var property = type.GetProperty(tagMapping.GroupName);
                var newTagUsers = currentList.Where(a => ((string)property.GetValue(a)).Contains(tagMapping.Tag)).Select(a => a.LillyID).ToList();

                var deleted = oldTagUsers.Except(newTagUsers).ToList();
                var added = newTagUsers.Except(oldTagUsers).ToList();

                if (exceptionUserIds.Any())
                {
                    deleted = deleted.Except(exceptionUserIds).ToList();
                }

                MailListApi.DelTagMember(WeChatCommonService.GetWeiXinToken(0), tagId, deleted.ToArray());
                MailListApi.AddTagMember(WeChatCommonService.GetWeiXinToken(0), tagId, added.ToArray());
            }

            return currentList.Count();
        }

        public int SyncAddressBook_LocalSad(DateTime? syncDate = null)
        {
            // 第一步，根据日期到数据库里取出所有当天的数据
            var configedNoMatchDepartmentModel = CommonService.lstSysConfig.FirstOrDefault(x => x.ConfigName == NoMatchDepartmentKey);

            if (configedNoMatchDepartmentModel == null || string.IsNullOrEmpty(configedNoMatchDepartmentModel.ConfigValue))
            {
                //throw new DLYBException("Have not config sad tag configration mapping!");
                Logger.Error<string>("sad no match department name does not  be configed!");
                return -1;
            }

            var depts = WeChatCommonService.lstDepartment;
            var deptDic = depts.AsParallel().Distinct().ToDictionary(x => x.name);

            var jobTime = AddReportJobLog(syncDate);

            var localSadCurrentList = GetNewAndUpdated(jobTime.DateFrom.ToString("yyyyMMddHHmmss"), jobTime.DateTo.ToString("yyyyMMddHHmmss"));
            if (localSadCurrentList == null) return 0;

            //TODO  need to check
            // 第二步，取出企业号当前的所有数据
            #region
            //var employeeWeChatList = (from emp in WeChatCommonService.lstUser
            //                          select new LocalSADEntity
            //                          {
            //                              Email = emp.email,
            //                              ChineseName = emp.name,
            //                              LillyID = emp.userid,
            //                              //DeptId = emp.department[emp.deptLvs.Count()],
            //                              Company = depts.First(a => a.id == (emp.department.Count() < 3 ? 0 : emp.department[2])).name,
            //                              Department = emp.department.Count() < 4 ? "" : (depts.FirstOrDefault(a => a.id == emp.department[3]) == null ? "" : depts.First(a => a.id == emp.department[3]).name),
            //                              SubDepartment = emp.department.Count() < 5 ? "" : (depts.FirstOrDefault(a => a.id == emp.department[4]) == null ? "" : depts.First(a => a.id == emp.department[4]).name),
            //                              Gender = (emp.gender == 0 ? "男" : "女")
            //                          }).ToList();
            #endregion
            #region
            //var employeeWeChatList = new List<LocalSADEntity>();
            //foreach (var emp in WeChatCommonService.lstUser)
            //{
            //    employeeWeChatList.Add(new LocalSADEntity {
            //        Email = emp.email,
            //        ChineseName = emp.name,
            //        LillyID = emp.userid,
            //        //DeptId = emp.department[emp.deptLvs.Count()],
            //        Company = emp.department.Count() < 3?"" : depts.First(a => a.id == emp.department[2]).name,
            //        Department = emp.department.Count() < 4 ? "" : (depts.FirstOrDefault(a => a.id == emp.department[3]) == null ? "" : depts.First(a => a.id == emp.department[3]).name),
            //        SubDepartment = emp.department.Count() < 5 ? "" : (depts.FirstOrDefault(a => a.id == emp.department[4]) == null ? "" : depts.First(a => a.id == emp.department[4]).name),
            //        Gender = (emp.gender == 0 ? "男" : "女")
            //    });
            //}
            #endregion
            #region handler no match department user
            //var notMatchedDepartmentUsers = localSadCurrentList.NewEmployees.AsParallel().Where(x => !deptDic.ContainsKey(x.Company)).ToList();

            //var result = HandlerNoMatchDepartmentUser(configedNoMatchDepartmentModel.ConfigValue, notMatchedDepartmentUsers, deptDic);

            //if (!result)
            //{
            //    SendEmail();
            //}
            #endregion

            List<EmployeeInfoWithDept> empWeChatList = WeChatCommonService.lstUserWithDeptTag;
            BaseService<BakupLocalSad> bakUsers = new BaseService<BakupLocalSad>();
            if (empWeChatList.Any())
            {
                int num = 0;
                DateTime tempdt = DateTime.Now;

                empWeChatList.ForEach(t =>
                {
                    try
                    {
                        bakUsers.Repository.Insert(
                            new BakupLocalSad
                            {
                                ChineseName = t.name,
                                LillyID = t.userid,
                                Email = t.email,
                                Gender = t.gender,
                                Company = t.deptLvs.Count > 2 ? t.deptLvs[2] : "",
                                Department = t.deptLvs.Count > 1 ? t.deptLvs[1] : "",
                                SubDepartment = t.deptLvs.Count > 0 ? t.deptLvs[0] : "",
                                DeptId = t.department[0],
                                Mobile = t.mobile,
                                Tags = JsonUntity.SerializeDictionaryToJsonString(t.tags),
                                OrderNum = ++num,
                                BakupDT = tempdt
                            });
                        Logger.Debug<string>("Bakup user address books success. lilly id ->" + t.userid);
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug<string>("Bakup user address books fail. lilly id ->" + t.userid);
                        Logger.Error<string>(ex, "Bakup user address books error. lilly id ->" + t.userid);
                    }
                });
            }

            //localSadCurrentList.NewEmployees = localSadCurrentList.NewEmployees.AsParallel().Where(x => deptDic.ContainsKey(x.Company)).ToList();
            //localSadCurrentList.UpdatedEmployees = localSadCurrentList.UpdatedEmployees.AsParallel().Where(x => deptDic.ContainsKey(x.Company)).ToList();

            // 第三步，前两个数据对比，检查出有任何变化的数据
            //      1. 数据有变化的，Email, Gender, Company, Department, SubDepartment, SFTitle, IsSupervisor, ContractGroup, BaseLocation
            //      2. 今天比昨天多的数据（要添加到地址簿中）
            //      3. 昨天比今天多的数据（要从地址簿中删除）
            var allTags = MailListApi.GetTagList(WeChatCommonService.GetWeiXinToken(0)).taglist;

            var configExceptionTag = CommonService.lstSysConfig.FirstOrDefault(x => x.ConfigName == ExceptionTagNameKey);
            var exceptionUserIds = new List<string>();

            if (configExceptionTag != null)
            {
                if (!string.IsNullOrEmpty(configExceptionTag.ConfigValue))
                {
                    var exceptionTags = configExceptionTag.ConfigValue.Split(',');
                    foreach (var exceptionTag in exceptionTags.Select(tag => allTags.FirstOrDefault(x => string.Compare(x.tagname, tag, StringComparison.OrdinalIgnoreCase) == 0)).Where(exceptionTag => exceptionTag != null))
                    {
                        exceptionUserIds.AddRange(MailListApi.GetTagMember(WeChatCommonService.GetWeiXinToken(0), int.Parse(exceptionTag.tagid)).userlist.Select(x => x.userid).ToList());
                    }
                }
            }

            bool execFail = false;
            var addList = localSadCurrentList.NewEmployees;
            var deleteList = localSadCurrentList.UpdatedEmployees.Where(t => t.STTS_IND == 0).ToList();

            var resultList = new List<ResultLocalSad>();

            //deleteList = employeeWeChatList.Except(deleteList, new LocalSadEntityComparer()).ToList();
            if (exceptionUserIds.Any())
            {
                deleteList = deleteList.Where(t => exceptionUserIds.All(k => t.LillyID != k)).ToList();
            }

            #region add
            int num1 = 0;
            foreach (var a in addList)
            {
                var wechatEmployee = empWeChatList.FirstOrDefault(t => t.userid == a.LillyID);
                if (wechatEmployee == null)
                {
                    var deptId = GetDeptId(a, deptDic);
                    if (deptId == -1)
                    {
                        SaveResultLocalSad(a, ++num1, 0, "未能获取当前用户部门信息，无法添加。", EnumLocalSad.LocalSadBakup);
                        continue;
                    }

                    try
                    {
                        var wxResult = MailListApi.CreateMember(WeChatCommonService.GetWeiXinToken(0), new GetMemberResult
                        {
                            userid = a.LillyID,
                            email = a.Email,
                            gender = (a.Gender == "男" ? 0 : 1),
                            name = a.ChineseName,

                            //department = new[] { deptId }
                            department = new[] { deptId }//temp
                        });
                        SaveResultLocalSad(a, ++num1, 1, "添加成功。", EnumLocalSad.LocalSadBakup);
                    }
                    catch (Exception ex)
                    {
                        execFail = true;
                        SaveResultLocalSad(a, ++num1, 0, "添加失败，" + ex.Message + "。", EnumLocalSad.LocalSadBakup);
                    }
                }
                else
                {
                    SaveResultLocalSad(a, ++num1, 0, "当前用户已经存在，无法重复添加。", EnumLocalSad.LocalSadBakup);
                }
            }
            #endregion

            #region delete
            foreach (var a in deleteList)
            {
                var wechatEmployee = empWeChatList.FirstOrDefault(t => t.userid == a.LillyID);
                if (wechatEmployee != null)
                {
                    try
                    {
                        var wxResult = MailListApi.DeleteMember(WeChatCommonService.GetWeiXinToken(0), a.LillyID);
                        SaveResultLocalSad(a, ++num1, 1, "删除成功。", EnumLocalSad.LocalSadBakup);
                    }
                    catch (Exception ex)
                    {
                        execFail = true;
                        SaveResultLocalSad(a, ++num1, 0, "删除失败，" + ex.Message + "。", EnumLocalSad.LocalSadBakup);
                    }
                }
                else
                {
                    SaveResultLocalSad(a, ++num1, 1, "当前用户不存在，无法删除。", EnumLocalSad.LocalSadBakup);
                }
            }
            #endregion

            #region
            //var configTagMapping = JsonHelper.FromJson<IList<TagConfig>>(configTag.ConfigValue).Select(x => x.Tags.Select(y => new TagMapping { GroupName = x.GroupName, Tag = y })).SelectMany(x => x).ToList();
            //var type = typeof(LocalSADEntity);

            //foreach (var tagMapping in configTagMapping)
            //{
            //    var tagId = int.Parse(allTags.First(a => a.tagname == tagMapping.GroupName).tagid);
            //    var oldTagUsersWx = MailListApi.GetTagMember(WeChatCommonService.GetWeiXinToken(0), tagId).userlist;
            //    var oldTagUsers = oldTagUsersWx.Select(taguserlist => taguserlist.userid).ToList();

            //    var property = type.GetProperty(tagMapping.GroupName);
            //    var newTagUsers = localSadCurrentList.NewEmployees.Where(a => ((string)property.GetValue(a)).Contains(tagMapping.Tag)).Select(a => a.LillyID).ToList();

            //    var deleted = oldTagUsers.Except(newTagUsers).ToList();
            //    var added = newTagUsers.Except(oldTagUsers).ToList();

            //    if (exceptionUserIds.Any())
            //    {
            //        deleted = deleted.Except(exceptionUserIds).ToList();
            //    }

            //    MailListApi.DelTagMember(WeChatCommonService.GetWeiXinToken(0), tagId, deleted.ToArray());
            //    MailListApi.AddTagMember(WeChatCommonService.GetWeiXinToken(0), tagId, added.ToArray());
            //}
            #endregion

            if (WeChatCommonService.lstUser != null)
            {
                cacheManager.Remove("UserItem");
            }
            if (WeChatCommonService.lstUserWithDeptTag != null)
            {
                cacheManager.Remove("UserWithDeptTagItem");
            }

            if(jobTime!=null)
            {
                jobTime.JobStatus = execFail ? JobStatus.Error.ToString() : JobStatus.Success.ToString();
                jobTime.ErrorMessage = execFail ? "执行存在失败。" : "全部执行成功。";
                jobTime.UpdatedDate = DateTime.Now;
                UpdateReportJobLog(jobTime);
            }

            return addList.Count() + deleteList.Count();
        }

        private void SaveResultLocalSad(LocalSADEntity lse, int orderNum, int result, string msg, EnumLocalSad els)
        {
            if (lse == null) return;

            new BaseService<ResultLocalSad>().Repository.Insert(
                new ResultLocalSad
                {
                    ChineseName = lse.ChineseName,
                    LillyID = lse.LillyID,
                    Email = lse.Email,
                    Gender = lse.Gender,
                    Company = lse.Company,
                    Department = lse.Department,
                    SubDepartment = lse.SubDepartment,
                    OrderNum = orderNum,
                    BakupDT = DateTime.Now,
                    ExecResult = result,
                    Description = msg,
                    Flag = (int)els
                });
        }

        private void SaveResultLocalSad(EmployeeInfoWithDept eiwd, int orderNum, int result, string msg, EnumLocalSad els)
        {
            try
            {
                Logger.Debug<string>("SaveResultLocalSad: Begin.");
                if (eiwd == null) return;

                new BaseService<ResultLocalSad>().Repository.Insert(
                    new ResultLocalSad
                    {
                        ChineseName = eiwd.name,
                        LillyID = eiwd.userid,
                        Email = eiwd.email,
                        Gender = eiwd.gender,
                        Company = eiwd.deptLvs.Count > 2 ? eiwd.deptLvs[2] : "",
                        Department = eiwd.deptLvs.Count > 1 ? eiwd.deptLvs[1] : "",
                        SubDepartment = eiwd.deptLvs.Count > 0 ? eiwd.deptLvs[0] : "",
                        OrderNum = orderNum,
                        BakupDT = DateTime.Now,
                        ExecResult = result,
                        Description = msg,
                        Flag = (int)els
                    });
            }
            catch (Exception ex)
            {
                Logger.Error<string>(ex, "SaveResultLocalSad: Error.");
            }
            finally
            {
                Logger.Debug<string>("SaveResultLocalSad: End.");
            }
        }

        public List<BakupLocalSad> GetBakupLocalSad(DateTime dt)
        {
            try
            {
                DateTime dtStar = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " 00:00:00");
                DateTime dtEnd = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " 23:59:59");
                Logger.Debug<string>("GetBakupLocalSad: Begin.");
                return new BaseService<BakupLocalSad>()
                    .Repository.Entities.Where(t => t.BakupDT.Value >= dtStar && t.BakupDT.Value <= dtEnd).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error<string>(ex, "GetBakupLocalSad: Error.");
                return null;
            }
            finally
            {
                Logger.Debug<string>("GetBakupLocalSad: End.");
            }
        }

        public void RedutionBakLocalSad(List<BakupLocalSad> lstBakLocalSad)
        {
            #region Init info
            Logger.Debug<string>("RedutionBakLocalSad: Begin.");

            Logger.Debug<string>("RedutionBakLocalSad: Get Department.");
            var depts = WeChatCommonService.lstDepartment;

            Logger.Debug<string>("RedutionBakLocalSad: Change Department to Distinct.");
            var deptDic = depts.AsParallel().Distinct().ToDictionary(x => x.name);

            Logger.Debug<string>("RedutionBakLocalSad: Get WeChat Employee List.");
            List<EmployeeInfoWithDept> empWeChatList = WeChatCommonService.lstUserWithDeptTag;
            int num1 = 0;
            #endregion

            #region delete
            var deleteList = new List<EmployeeInfoWithDept>();
            Logger.Debug<string>("RedutionBakLocalSad: Set Delete Employee List.");
            foreach (var item in empWeChatList)
            {
                if (lstBakLocalSad.FirstOrDefault(t => t.LillyID == item.userid) == null)
                {
                    deleteList.Add(item);
                }
            }

            Logger.Debug<string>("RedutionBakLocalSad: Exec Delete Employee.");
            foreach (var del in deleteList)
            {
                try
                {
                    var wxResult = MailListApi.DeleteMember(WeChatCommonService.GetWeiXinToken(0), del.userid);
                    SaveResultLocalSad(del, ++num1, 1, "删除成功。", EnumLocalSad.LocalSadExec);
                }
                catch (Exception ex)
                {
                    Logger.Debug<string>("RedutionBakLocalSad: Delete Employee Fail. Lilly Id -> " + del.userid);
                    Logger.Error<string>(ex, "RedutionBakLocalSad: Delete Employee Error.");
                    SaveResultLocalSad(del, ++num1, 0, "删除失败，" + ex.Message + "。", EnumLocalSad.LocalSadExec);
                }
            }
            #endregion

            #region add
            var addList = new List<EmployeeInfoWithDept>();
            Logger.Debug<string>("RedutionBakLocalSad: Set Add Employee List.");
            foreach (var item in lstBakLocalSad)
            {
                if (empWeChatList.FirstOrDefault(t => t.userid == item.LillyID) == null)
                {
                    addList.Add(
                        new EmployeeInfoWithDept
                        {
                            email = item.Email,
                            name = item.ChineseName,
                            userid = item.LillyID,
                            department = new List<int> { (int)item.DeptId },
                            deptLvs = new List<string>() { item.Company, item.Department, item.SubDepartment },
                            gender = item.Gender
                        });
                }
            }
            Logger.Debug<string>("RedutionBakLocalSad: Add Delete Employee.");
            foreach (var add in addList)
            {
                try
                {
                    var wxResult = MailListApi.CreateMember(WeChatCommonService.GetWeiXinToken(0), new GetMemberResult
                    {
                        userid = add.userid,
                        email = add.email,
                        gender = int.Parse(add.gender),
                        name = add.name,
                        department = new[] { add.department[0] }
                    });
                    SaveResultLocalSad(add, ++num1, 1, "添加成功。", EnumLocalSad.LocalSadExec);
                }
                catch (Exception ex)
                {
                    Logger.Debug<string>("RedutionBakLocalSad: Add Employee Fail. Lilly Id -> " + add.userid);
                    Logger.Error<string>(ex, "RedutionBakLocalSad: Add Employee Error.");
                    SaveResultLocalSad(add, ++num1, 0, "添加失败，" + ex.Message + "。", EnumLocalSad.LocalSadExec);
                }
            }
            #endregion

            if (WeChatCommonService.lstUser != null)
            {
                cacheManager.Remove("UserItem");
            }
            if (WeChatCommonService.lstUserWithDeptTag != null)
            {
                cacheManager.Remove("UserWithDeptTagItem");
            }

            Logger.Debug<string>("RedutionBakLocalSad: End.");
        }

        public bool CheckLocalSadPwd(string inputPwd)
        {
            Logger.Debug<string>("CheckLocalSadPwd: Begin.");
            if (string.IsNullOrEmpty(inputPwd)) return false;
            try
            {
                Logger.Debug<string>("CheckLocalSadPwd: Get DB Password.");
                string dbPwd = new BaseService<LocalSadPassword>().Repository.Entities.FirstOrDefault().Pwd;
                if (string.IsNullOrEmpty(dbPwd))
                {
                    Logger.Debug<string>("CheckLocalSadPwd: DB Password is null.");
                    return false;
                }

                Logger.Debug<string>("CheckLocalSadPwd: Encrypt return.");
                return DESCrypService.Encrypt(inputPwd) == dbPwd;
            }
            catch (Exception ex)
            {
                Logger.Error<string>(ex, "CheckLocalSadPwd: Error.");
                return false;
            }
            finally
            {
                Logger.Debug<string>("CheckLocalSadPwd: End.");
            }
        }

        public bool UpdateLocalSadPwd(string userId)
        {
            try
            {
                Logger.Debug<string>("UpdateLocalSadPwd: Begin.");
                Logger.Debug<string>("UpdateLocalSadPwd: Login User Id -> " + userId);
                Logger.Debug<string>("UpdateLocalSadPwd: Get DB Password.");
                var dbpwd = new BaseService<LocalSadPassword>().Repository.Entities.FirstOrDefault();
                if (dbpwd == null) return false;
                Logger.Debug<string>("UpdateLocalSadPwd: Create Password.");
                dbpwd.Pwd = CreatePassword();
                dbpwd.UpdateTime = DateTime.Now;

                Logger.Debug<string>("UpdateLocalSadPwd: Inseert exec people.");
                new BaseService<LocalSadUserPwd>().Repository.Insert(
                        new LocalSadUserPwd()
                        {
                            LillyId = userId,//User.Identity.Name,
                            UpdateTime = DateTime.Now
                        });

                Logger.Debug<string>("UpdateLocalSadPwd: Update New Password.");
                return new BaseService<LocalSadPassword>().Repository.Update(dbpwd) > 0;
            }
            catch (Exception ex)
            {
                Logger.Error<string>(ex, "UpdateLocalSadPwd: Error.");
                return false;
            }
            finally
            {
                Logger.Debug<string>("UpdateLocalSadPwd: End.");
            }
        }

        private string CreatePassword(int pwdLength = 8)
        {
            try
            {
                Logger.Debug<string>("CreatePassword: Begin.");
                string pwdChars = string.Empty;
                pwdChars += "abcdefghijklmnopqrstuvwxyz";
                pwdChars += "1234567890";
                pwdChars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                pwdChars += "~!@#$%^&*()_+";

                string pwdResult = string.Empty;
                Random rnd = new Random();
                for (int i = 0; i < pwdLength; i++)
                {
                    pwdResult += pwdChars[rnd.Next(pwdChars.Length)];
                }

                return DESCrypService.Encrypt(pwdResult);
            }
            catch (Exception ex)
            {
                Logger.Error<string>(ex, "CreatePassword: Error.");
                return "";
            }
            finally
            {
                Logger.Debug<string>("CreatePassword: End.");
            }
        }

        private int GetDeptId(LocalSADEntity lse, Dictionary<string, DepartmentList> dicDept)
        {
            if (lse == null || dicDept == null || dicDept.Count < 1) return -1;

            string DeptName = string.Empty;
            if (string.IsNullOrEmpty(lse.SubDepartment) == false)
                DeptName = lse.SubDepartment;
            else if (string.IsNullOrEmpty(lse.Department) == false)
                DeptName = lse.Department;
            else if (string.IsNullOrEmpty(lse.Company) == false)
                DeptName = lse.Company;

            if (dicDept.ContainsKey(DeptName) == false) return -1;

            return dicDept[DeptName].id;
        }

        protected static bool HandlerNoMatchDepartmentUser(string noMatchDepartName, IList<LocalSADEntity> noMatchUsers,
            IReadOnlyDictionary<string, DepartmentList> departmentDic)
        {
            if (!noMatchUsers.Any())
            {
                return false;
            }

            DepartmentList dept;

            if (!departmentDic.TryGetValue(noMatchDepartName, out dept))
            {
                WeChatCommonService.RemoveDepartmentFromCache();
                departmentDic = WeChatCommonService.lstDepartment.ToDictionary(x => x.name);

                if (!departmentDic.TryGetValue(noMatchDepartName, out dept))
                {
                    Logger.Error<string>("Has not config noMatchDepart on wechat");
                    return false;
                }
            }

            var inWechatNoMatchDepartUsers =
                WeChatCommonService.lstUserWithDeptTag.Where(x => x.department[0] == dept.id)
                    .ToDictionary(x => x.userid);

            //no matched and not exist in wechat
            var noMatchedUsers = noMatchUsers.Where(x => !inWechatNoMatchDepartUsers.ContainsKey(x.LillyID)).ToList();

            Parallel.ForEach(noMatchedUsers, x => MailListApi.CreateMember(WeChatCommonService.GetWeiXinToken(0), new GetMemberResult
            {
                userid = x.LillyID,
                email = x.Email,
                gender = (x.Gender == "男" ? 0 : 1),
                name = x.ChineseName,
                department = new[] { x.DeptId }
            }));

            return true;
        }

        protected void SendEmail()
        {
            var configDic = CommonService.lstSysConfig.ToDictionary(x => x.ConfigName, StringComparer.OrdinalIgnoreCase);

            SysConfigModel configProxyModel;

            if (!configDic.TryGetValue(EmailProxyInfoKey, out configProxyModel))
            {
                Logger.Debug<string>("email info does not config");
                return;
            }

            if (string.IsNullOrEmpty(configProxyModel.ConfigValue))
            {
                Logger.Debug<string>("email proxy info value column does not config");
                return;
            }

            SysConfigModel configedSadEmailInfoModel;

            if (!configDic.TryGetValue(SadInfoKey, out configedSadEmailInfoModel))
            {
                Logger.Debug<string>("configedSADEmailInfoModel info does not config");
                return;
            }

            if (string.IsNullOrEmpty(configedSadEmailInfoModel.ConfigValue))
            {
                Logger.Debug<string>("email info value column does not config");
                return;
            }


            var emailInfoDic = JsonHelper.FromJson<Dictionary<string, object>>(configProxyModel.ConfigValue);
            emailInfoDic = new Dictionary<string, object>(emailInfoDic, StringComparer.OrdinalIgnoreCase);

            var sadEmailInfoDic = JsonHelper.FromJson<Dictionary<string, object>>(configedSadEmailInfoModel.ConfigValue);
            sadEmailInfoDic = new Dictionary<string, object>(sadEmailInfoDic, StringComparer.OrdinalIgnoreCase);

            var smtpClient = new SmtpClient(emailInfoDic["host"].ToString(), (int)emailInfoDic["port"])
            {
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(emailInfoDic["from"].ToString(), emailInfoDic["pwd"].ToString())
            };

            smtpClient.Send(new MailMessage(emailInfoDic["from"].ToString(), sadEmailInfoDic["to"].ToString(), sadEmailInfoDic["title"].ToString(), sadEmailInfoDic["body"].ToString())
            {
                IsBodyHtml = true
            });
        }

        private enum EnumLocalSad
        {
            /// <summary>
            /// LocalSad每日更新
            /// </summary>
            LocalSadExec = 1,
            /// <summary>
            /// LocalSad备份执行
            /// </summary>
            LocalSadBakup = 2
        }

        /// <summary>
        /// LocalSAD的接口，根据城市查找员工信息
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public abstract List<LocalSADEntity> GetEmployeeByLocation(string city);

        /// <summary>
        /// LocalSAD的接口，根据Manager查找直线下属信息
        /// </summary>
        /// <param name="accountname"></param>
        /// <returns></returns>
        public abstract List<LocalSADEntity> GetSupervisedEmployee(string accountname);

        /// <summary>
        /// 获取一段时间内发生变化的员工列表
        /// </summary>
        /// <param name="starttime">20170616235959</param>
        /// <param name="endtime">20170620235959</param>
        /// <returns></returns>
        public abstract LocalSADEntityQueryResult GetNewAndUpdated(string starttime = "", string endtime = "");

        /// <summary>
        /// 根据ID获取员工的Manager信息
        /// </summary>
        /// <param name="accountname"></param>
        /// <returns></returns>
        public abstract LocalSADEntity GetSupervisor(string accountname);

        /// <summary>
        /// 根据三级部门名获取员工信息
        /// </summary>
        /// <param name="subDepartment"></param>
        /// <returns></returns>
        public abstract List<LocalSADEntity> GetEmployeeBySubDepartment(string subDepartment);


        /// <summary>
        /// 根据二级部门名获取员工信息
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public abstract List<LocalSADEntity> GetEmployeeByDepartment(string department);

        /// <summary>
        /// 根据员工的信息查询员工
        /// </summary>
        /// <param name="lillyID"></param>
        /// <param name="globalID"></param>
        /// <param name="chineseName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public abstract LocalSADEntity GetEmployeeByQuery(string lillyID, string globalID, string chineseName, string email);

        /// <summary>
        /// 根据GlobalID查询员工
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public abstract LocalSADEntity GetEmployeeByGlobalId(string globalId);

        /// <summary>
        /// 根据电话查询员工
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public abstract LocalSADEntity GetEmployeeByMobile(string mobile);

        /// <summary>
        /// 依据员工账号(员工AD账号，或者称Lilly ID， 大写输入)获取该员工信息
        /// </summary>
        /// <param name="accountname"></param>
        /// <returns></returns>
        public abstract IList<LocalSADEntity> GetEmployeeByAccountName(string accountname);


        protected ReportJobLogEntity AddReportJobLog(DateTime? dt = null)
        {
            ReportJobLogEntity entity = new ReportJobLogEntity();
            entity.JobName = SadReportJobLogDate;
            entity.JobStatus = JobStatus.Running.ToString();
            entity.DateTo = DateTime.Now.Date;
            entity.UpdatedDate = DateTime.Now;
            entity.CreatedDate = DateTime.Now;

            if (dt != null)
            {
                entity.DateFrom = dt.Value.Date;

            }
            else
            {
                ReportJobLogEntity entity1 = ReportJobLogService.Repository.Entities.Where
                    (x => x.JobStatus == JobStatus.Success.ToString() && x.JobName == SadReportJobLogDate)
                    .OrderByDescending(a => a.DateFrom).FirstOrDefault();
                if (entity1 != null)
                {
                    entity.DateFrom = entity1.DateTo;
                }
                else
                {
                    entity.DateFrom = DateTime.Now.AddDays(-1).Date;
                }
            }

            ReportJobLogService.Repository.Insert(entity);
            return entity;
        }
        protected void UpdateReportJobLog(ReportJobLogEntity entity)
        {
            ReportJobLogService.Repository.Entities.Where(x => x.Id == entity.Id).Update(x => new ReportJobLogEntity { JobStatus = entity.JobStatus, ErrorMessage = entity.ErrorMessage, UpdatedDate = DateTime.Now });
        }
    }

    public class LocalSadEntityComparer : IEqualityComparer<LocalSADEntity>
    {
        public bool Equals(LocalSADEntity x, LocalSADEntity y)
        {
            return x.LillyID == y.LillyID;
        }

        public int GetHashCode(LocalSADEntity obj)
        {
            return obj.LillyID.GetHashCode();
        }
    }

    //use to map sad and wechat
    public sealed class TagConfig
    {
        public string GroupName { get; set; }

        public IList<string> Tags { get; set; }
    }

    public class TagMapping
    {
        public string GroupName { get; set; }

        public string Tag { get; set; }
    }

    public class JsonUntity
    {
        public static string SerializeDictionaryToJsonString<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            if (dict.Count == 0)
                return "";

            string jsonStr = JsonConvert.SerializeObject(dict);
            return jsonStr;
        }

        public static Dictionary<TKey, TValue> DeserializeStringToDictionary<TKey, TValue>(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
                return new Dictionary<TKey, TValue>();

            Dictionary<TKey, TValue> jsonDict = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(jsonStr);

            return jsonDict;

        }
    }
}