using Innocellence.CA.Service;
using Innocellence.CA.Service.job;
using NUnit.Framework;

namespace UnitTest
{
    public class AppAccessJobTest : DbSetUp
    {
        [Test]
        public void JobLogTest()
        {
            var jobLogService = new ReportJobLogService();

            var entity = jobLogService.CreateJobLog("AppAccessReportJob");

            Assert.IsTrue(entity.Id > 0);
        }

        [Test]
        public void MenuJob()
        {
            var job = new AppAccessReportJob();
            var task = job.Execute();
            task.Start();
            task.Wait();

        }
    }
}
