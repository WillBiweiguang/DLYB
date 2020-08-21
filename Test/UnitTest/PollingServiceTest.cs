using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Data;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Web.Domain.Entity;
using Innocellence.CA.Entity;
using NUnit.Framework;
using Innocellence.CA.Service;

namespace UnitTest
{
    [TestFixture]
    public class PollingServiceTest
    {
        [Test]
        public void TestPolling()
        {
            var yn = PollingService.ConvertAbcToYnStatic("ABC");
            var yn1 = PollingService.ConvertAbcToYnStatic("aC");
            var yn2 = PollingService.ConvertAbcToYnStatic("D");
            var yn3 = PollingService.ConvertAbcToYnStatic("ABCDEFGHIJK");
            var yn4 = PollingService.ConvertAbcToYnStatic("AAACV");
            var yn5 = PollingService.ConvertAbcToYnStatic("DCAE");



            Assert.AreEqual("YYYNNNNNNN", yn);
            Assert.AreEqual("YNYNNNNNNN", yn1);
            Assert.AreEqual("NNNYNNNNNN", yn2);
            Assert.AreEqual("YYYYYYYYYY", yn3);
            Assert.AreEqual("YNYNNNNNNN", yn4);
            Assert.AreEqual("YNYYYNNNNN", yn5);

        }
    }

}


