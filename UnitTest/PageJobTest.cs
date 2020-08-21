using Innocellence.CA.Service;
using Innocellence.CA.Service.job;
using NUnit.Framework;

namespace UnitTest
{
    public class PageJobTest : DbSetUp
    {
        [Test]
        public void JobLogTest()
        {
            var jobLogService = new ReportJobLogService();

            var entity = jobLogService.CreateJobLog("PageReportJob");

            Assert.IsTrue(entity.Id > 0);
        }

        [Test]
        public void MenuJob()
        {
            var job = new PageReportJob();
            var task = job.Execute();
            task.Start();
            task.Wait();

        }
    }
}
