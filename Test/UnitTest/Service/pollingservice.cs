using Innocellence.Activity.Service;
using NUnit.Framework;
using System.Linq;

namespace UnitTest.Service
{
  public  class Pollingservice : DbSetUp
    {
        [Test]
        public void PollingQuery()
        {
            PollingResultService service = new PollingResultService();

            var view = service.GetPollingReslutViews(1, "c217355");

            Assert.IsTrue(view.Any());
        }
    }
     

}
