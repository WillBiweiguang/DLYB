using Innocellence.CA.Service;
using Innocellence.CA.Service.job;
using NUnit.Framework;

namespace UnitTest
{
    public class ArticleJobTest : DbSetUp
    {
        [Test]
        public void JobLogTest()
        {
            var jobLogService = new ReportJobLogService();

            var entity = jobLogService.CreateJobLog("ArticleReportJob");

            Assert.IsTrue(entity.Id > 0);
        }

        [Test]
        public void ArticleJob() 
        {
            var job = new ArticleReportJob();
            var task = job.Execute();
            task.Start();
            task.Wait();
        }

    }
}
