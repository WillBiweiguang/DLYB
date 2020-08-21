using System;
using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;
using System.Collections.Generic;

namespace DLYB.CA.Contracts
{
    public interface ILocalSADService : IDependency, IBaseService<LocalSADEntity>
    {
        int SyncAddressBook(DateTime? syncDate = null);
        int SyncAddressBook_LocalSad(DateTime? syncDate = null);
        List<BakupLocalSad> GetBakupLocalSad(DateTime dt);
        void RedutionBakLocalSad(List<BakupLocalSad> lstBakLocalSad);
        //int RedutionEmployee(string redutionDate);

        /// <summary>
        /// LocalSAD的接口，根据城市查找员工信息
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        List<LocalSADEntity> GetEmployeeByLocation(string city);

        /// <summary>
        /// LocalSAD的接口，根据Manager查找直线下属信息
        /// </summary>
        /// <param name="accountname"></param>
        /// <returns></returns>
        List<LocalSADEntity> GetSupervisedEmployee(string accountname);

        /// <summary>
        /// 获取一段时间内发生变化的员工列表
        /// </summary>
        /// <param name="starttime">20170616235959</param>
        /// <param name="endtime">20170620235959</param>
        /// <returns></returns>
        LocalSADEntityQueryResult GetNewAndUpdated(string starttime, string endtime);

        /// <summary>
        /// 根据ID获取员工的Manager信息
        /// </summary>
        /// <param name="accountname"></param>
        /// <returns></returns>
        LocalSADEntity GetSupervisor(string accountname);

        /// <summary>
        /// 根据三级部门名获取员工信息
        /// </summary>
        /// <param name="subDepartment"></param>
        /// <returns></returns>
        List<LocalSADEntity> GetEmployeeBySubDepartment(string subDepartment);


        /// <summary>
        /// 根据二级部门名获取员工信息
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        List<LocalSADEntity> GetEmployeeByDepartment(string department);

        /// <summary>
        /// 通过globalId获取员工信息
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        LocalSADEntity GetEmployeeByGlobalId(string globalId);


        /// <summary>
        /// 根据员工的信息查询员工
        /// </summary>
        /// <param name="lillyID"></param>
        /// <param name="globalID"></param>
        /// <param name="chineseName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        LocalSADEntity GetEmployeeByQuery(string lillyID, string globalID, string chineseName, string email);

        /// <summary>
        /// 根据员工的手机号查询员工
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        LocalSADEntity GetEmployeeByMobile(string mobile);

        IList<LocalSADEntity> GetEmployeeByAccountName(string accountname);
    }
}
