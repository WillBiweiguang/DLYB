using MySql.Data.MySqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DLYBWindowsService
{
    public class AutomationService
    {
        public static void Dotask()
        {
            string connStr = "Data Source=111.207.36.126;Port=5986;User Id=root;Password=123123;Database=quota;Charset=utf8;TreatTinyAsBoolean=false;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string sql = "select Id from t_BeamInfo b where Id > 100 and (AutoRegStatus is null or AutoRegStatus < 2) and not EXISTS (select 1 from t_weldcategorylabeling t where b.Id = t.BeamId and t.IsDeleted <> 1) and EXISTS(select 1 from t_ProjectInfo p where b.ProjectId = p.Id and p.IsDeleted = 0 and p.Status = '未完成') limit 0,1 ";
                string sqlUpdatestart = "update t_BeamInfo set AutoRegStatus = 1 where Id = {0}";
                string sqlUpdateend = "update t_BeamInfo set AutoRegStatus = 2 where Id = {0}";
                conn.Open();
                string beamId = "";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {

                    using (MySqlDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            beamId = dataReader[0].ToString();
                        }
                    }
                }
                var sql2 = string.Format(sqlUpdatestart, beamId);
                using (MySqlCommand cmd = new MySqlCommand(sql2, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                DoAuto(beamId);

                sql2 = string.Format(sqlUpdateend, beamId);
                using (MySqlCommand cmd = new MySqlCommand(sql2, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void DoAuto(string beamId)
        {
            IWebDriver selenium = new InternetExplorerDriver();
            try
            {
                selenium.Navigate().GoToUrl("http://111.207.36.126:4001/Account/Login");
                selenium.FindElement(By.Name("UserName")).Clear();
                selenium.FindElement(By.Name("Password")).Clear();
                selenium.FindElement(By.Name("UserName")).SendKeys("13012345678");
                selenium.FindElement(By.Name("Password")).SendKeys("1qaz@WSX");
                selenium.FindElement(By.Id("Login")).Click();
                Thread.Sleep(2000);
                selenium.Navigate().GoToUrl("http://111.207.36.126:4001/weldcategory/index?beamId="+ beamId);
                Thread.Sleep(2000);  
                //selenium.SwitchTo().Alert().Accept();
                selenium.FindElement(By.Id("weldingFinding")).Click();
                Thread.Sleep(2000);
                selenium.SwitchTo().Alert().Accept();
                Thread.Sleep(30000);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            finally
            {
                selenium.Close();
                selenium.Quit();
            }
        }
    }
}
