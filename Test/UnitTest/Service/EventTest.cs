using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Infrastructure.Core.Infrastructure;
using Innocellence.Activity.Services;
using Innocellence.CA.Contracts.CommonEntity;
using NUnit.Framework;

namespace UnitTest.Service
{
    public class EventTest : DbSetUp
    {
        [Test]
        public void QueryEvent()
        {
            var service = new EventService();

            var list = service.Repository.Entities.Select(x => new { id = x.Id, name = x.Name }).ToList();

            Assert.IsTrue(list.Any());
        }

        [Test]
        public void Register()
        {
            var service = EngineContext.Current.Resolve<IEventProfileService>();
            var resultList = new List<ResultResponse<object, EventStatus>>();
            //var service = new EventProfileService(null, es);

            for (int x = 1; x < 20; x++)
            {
                var userId = x > 2 ? "1" : x.ToString(CultureInfo.CurrentCulture);
                resultList.Add(service.RegisteredEvent(1, userId));
            }

            Assert.IsTrue(resultList.Count(x => x.Status == EventStatus.Success) == 2);
        }

        [Test]
        public void Checkin()
        {
            var service = EngineContext.Current.Resolve<IEventProfileService>();

            var result = service.CheckinEvent(1, "1");

            Assert.IsTrue(result.Status == EventStatus.Success);
        }
    }
}
