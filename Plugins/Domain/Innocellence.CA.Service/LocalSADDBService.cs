using Infrastructure.Web.Domain.Contracts;
using DLYB.CA.Contracts;
using DLYB.CA.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLYB.CA.Service
{
    /// <summary>
    /// 业务实现——App访问量统计
    /// </summary>
    public class LocalSadDBService : LocalSadService, ILocalSADService
    {
        public LocalSadDBService(ISysUserService sysUserService)
            : base(sysUserService)
        {
        }

        /// <summary>
        /// LocalSAD的接口，根据城市查找员工信息
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public override List<LocalSADEntity> GetEmployeeByLocation(string city)
        {
            var tempCity = city + "市";
            var tempCity1 = string.Empty;

            if (!string.IsNullOrWhiteSpace(city))
            {
                tempCity1 = city.Remove(city.Length - 1, 1);
            }
            return Repository.Entities.Where(a => a.BaseLocation == city || a.BaseLocation == tempCity || a.BaseLocation == tempCity1).ToList();
        }

        /// <summary>
        /// LocalSAD的接口，根据Manager查找直线下属信息
        /// </summary>
        /// <param name="accountname"></param>
        /// <returns></returns>
        public override List<LocalSADEntity> GetSupervisedEmployee(string accountname)
        {
            return Repository.Entities.Where(a => a.ManagerID == accountname).ToList();
        }

        /// <summary>
        /// 获取一段时间内发生变化的员工列表
        /// </summary>
        /// <param name="starttime">20170616235959</param>
        /// <param name="endtime">20170620235959</param>
        /// <returns></returns>
        public override LocalSADEntityQueryResult GetNewAndUpdated(string starttime, string endtime)
        {
            throw new Exception("Should not be triggered.");
        }

        /// <summary>
        /// 根据ID获取员工的Manager信息
        /// </summary>
        /// <param name="accountname"></param>
        /// <returns></returns>
        public override LocalSADEntity GetSupervisor(string accountname)
        {
            var me = Repository.Entities.FirstOrDefault(a => a.LillyID == accountname);

            return Repository.Entities.FirstOrDefault(a => a.LillyID == me.ManagerID);
        }

        /// <summary>
        /// 根据三级部门名获取员工信息
        /// </summary>
        /// <param name="subDepartment"></param>
        /// <returns></returns>
        public override List<LocalSADEntity> GetEmployeeBySubDepartment(string subDepartment)
        {
            return Repository.Entities.Where(a => a.SubDepartment == subDepartment).ToList();
        }


        /// <summary>
        /// 根据二级部门名获取员工信息
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public override List<LocalSADEntity> GetEmployeeByDepartment(string department)
        {
            return Repository.Entities.Where(a => a.Department == department).ToList();
        }

        /// <summary>
        /// 根据员工的信息查询员工
        /// </summary>
        /// <param name="lillyID"></param>
        /// <param name="globalID"></param>
        /// <param name="chineseName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public override LocalSADEntity GetEmployeeByQuery(string lillyID, string globalID, string chineseName, string email)
        {
            var rep = Repository.Entities;
            if (string.IsNullOrEmpty(lillyID))
            {
                rep.Where(a => a.LillyID == lillyID);
            }
            if (string.IsNullOrEmpty(globalID))
            {
                rep.Where(a => a.GlobalID == long.Parse(globalID));
            }
            if (string.IsNullOrEmpty(lillyID))
            {
                rep.Where(a => a.LillyID == lillyID);
            }
            if (string.IsNullOrEmpty(chineseName))
            {
                rep.Where(a => a.ChineseName == chineseName);
            }
            if (string.IsNullOrEmpty(email))
            {
                rep.Where(a => a.Email == email);
            }

            return rep.FirstOrDefault();
        }

        public override LocalSADEntity GetEmployeeByGlobalId(string globalId)
        {
            var gid = long.Parse(globalId);
            return Repository.Entities.FirstOrDefault(a => a.GlobalID == gid);
        }

        public override LocalSADEntity GetEmployeeByMobile(string mobile)
        {
            return Repository.Entities.FirstOrDefault(a => a.Phone == mobile);
        }

        public override IList<LocalSADEntity> GetEmployeeByAccountName(string accountname)
        {
            return Repository.Entities.Where(x => x.LillyID == accountname).ToList();
        }
    }
}