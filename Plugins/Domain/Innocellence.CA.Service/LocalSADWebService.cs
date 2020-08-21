using System.Net;
using Infrastructure.Core.Logging;
using Infrastructure.Web.Domain.Contracts;
using DLYB.CA.Contracts.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using DLYB.CA.Contracts;
using System.IO;
using Infrastructure.Core.Data;

namespace DLYB.CA.Service
{
    /// <summary>
    /// 业务实现——App访问量统计
    /// </summary>
    public class LocalSADWebService : LocalSadService, ILocalSADService
    {
        protected readonly ILogger Logger = LogManager.GetLogger("wechat");
        public LocalSADWebService(ISysUserService sysUserService)
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
            var url = string.Format("/dev/lsad/Api/Employee/GetEmployeeByLocation?location={0}", city);
            return GetResultprotected(url);
        }

        /// <summary>
        /// LocalSAD的接口，根据Manager查找直线下属信息
        /// </summary>
        /// <param name="accountname"></param>
        /// <returns></returns>
        public override List<LocalSADEntity> GetSupervisedEmployee(string accountname)
        {
            var url = string.Format("/dev/lsad/Api/Employee/GetSupervisedEmployee?accountname={0}", accountname);
            return GetResultprotected(url);
        }

        /// <summary>
        /// 获取一段时间内发生变化的员工列表
        /// </summary>
        /// <param name="starttime">20170616235959</param>
        /// <param name="endtime">20170620235959</param>
        /// <returns></returns>
        public override LocalSADEntityQueryResult GetNewAndUpdated(string starttime = "", string endtime = "")
        {
            //string strjosn = testReadFile();
            //return JsonConvert.DeserializeObject<LocalSADEntityQueryResult>(strjosn);

            //var url = @"/lsad/Api/Employee/GetNewAndUpdated?starttime=20170829000000&endtime=20171013000000";
            var url = @"/lsad/Api/Employee/GetNewAndUpdated?starttime={0}&endtime={1}";
            Logger.Debug("sad url :{0}", url);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 |
                                      SecurityProtocolType.Tls11;

            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://soa-ch.xh3.lilly.com:8443/");
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "SADAPICONNECTION", "LillY2000@"))));
                var response = httpClient.GetAsync(url).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                Logger.Debug("sad json :{0}", jsonString);
                if (jsonString.Contains("error"))
                {
                    Logger.Error("local sad GetNewAndUpdated interface error. return json is : {0}", jsonString);
                    return new LocalSADEntityQueryResult();
                }

                return JsonConvert.DeserializeObject<LocalSADEntityQueryResult>(response.Content.ReadAsStringAsync().Result);
            }
        }

        private void test123()
        {
            var url = "https://soa-ch.xh3.lilly.com:8443/lsad/Api/Employee/GetNewAndUpdated?starttime=20170829000000&endtime=20171013000000";
            var request = WebRequest.Create(url);
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.Default.GetBytes(string.Format("{0}:{1}", "SADAPICONNECTION", "LillY2000@")));
            var response = request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        private string testReadFile()
        {
            string filePath = @"C:\10_CA\localsad.txt";

            return File.Exists(filePath) ? File.ReadAllText(filePath,System.Text.Encoding.UTF8) : "[]";
        }

        /// <summary>
        /// 根据ID获取员工的Manager信息
        /// </summary>
        /// <param name="accountname"></param>
        /// <returns></returns>
        public override LocalSADEntity GetSupervisor(string accountname)
        {
            var url = string.Format("/dev/lsad/Api/Employee/GetSupervisor?accountname={0}", accountname);
            return GetResultprotected(url).FirstOrDefault();
        }

        /// <summary>
        /// 根据三级部门名获取员工信息
        /// </summary>
        /// <param name="subDepartment"></param>
        /// <returns></returns>
        public override List<LocalSADEntity> GetEmployeeBySubDepartment(string subDepartment)
        {
            var url = string.Format("/dev/lsad/Api/Employee/GetEmployeeBySubDepartment?subDepartment={0}", subDepartment);
            return GetResultprotected(url);
        }


        /// <summary>
        /// 根据二级部门名获取员工信息
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public override List<LocalSADEntity> GetEmployeeByDepartment(string department)
        {
            var url = string.Format("/dev/lsad/Api/Employee/GetEmployeeByDepartment?department={0}", department);
            return GetResultprotected(url);
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
            var url = string.Format("/dev/lsad/Api/Employee/GetEmployeeByQuery?lillyID={0}&globalID={1}&chineseName={2}&email={3}", lillyID, globalID, chineseName, email);
            return GetResultprotected(url).FirstOrDefault();
        }


        protected List<LocalSADEntity> GetResultprotected(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 |
                                                  SecurityProtocolType.Tls11;

            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://soa-ch.xh3.lilly.com:8443/");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                var response = httpClient.GetAsync(url).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;

                //Logger.Debug("jsonstring : {0}", jsonString);

                if (jsonString.Contains("error"))
                {
                    Logger.Error("local sad web interface error. return json is : {0}", jsonString);
                    return new List<LocalSADEntity>();
                }

                return JsonConvert.DeserializeObject<List<LocalSADEntity>>(response.Content.ReadAsStringAsync().Result);
            }
        }


        public override LocalSADEntity GetEmployeeByGlobalId(string globalId)
        {
            var url = string.Format("/dev/lsad/Api/Employee/GetEmployeeByGlobalID?globalID={0}", globalId);
            return GetResultprotected(url).FirstOrDefault();
        }

        public override LocalSADEntity GetEmployeeByMobile(string mobile)
        {
            throw new NotImplementedException();
        }

        public override IList<LocalSADEntity> GetEmployeeByAccountName(string accountname)
        {
            var url = string.Format("/dev/lsad/Api/Employee/GetEmployeeByAccountName?accountname={0}", accountname);
            return GetResultprotected(url);
        }

        public int RedutionEmployee(string redutionDate)
        {
            var bakupEmployee = new BaseService<BakupLocalSad>().Repository.Entities.Where(t => t.BakupDT.Value.ToString("yyyyMMdd") == redutionDate).ToList();

            return 1;
        }
    }
}