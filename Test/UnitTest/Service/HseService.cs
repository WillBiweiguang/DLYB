using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using Infrastructure.Core;
using Infrastructure.Core.Infrastructure;
using Innocellence.HSE.Model;
using Innocellence.HSE.Services;
using LinqKit;
using NUnit.Framework;

namespace UnitTest.Service
{
    public class HseTestService : DbSetUp
    {
        //private readonly IHseService _hseService = EngineContext.Current.Resolve<IHseService>();

        [Test]
        public void SendHseMessage()
        {
            var hseService = EngineContext.Current.Resolve<IHseService>();
            hseService.SendHseMessage("大连市", "test", "1365492092", 0);
        }

        [Test]
        public void MockHseUser()
        {
            var hseUserService = EngineContext.Current.Resolve<IHseReceiverUserService>();
            var hseReplyService = EngineContext.Current.Resolve<IHseRepliedMessageService>();
            var locations = "大连,鞍山,沈阳,青岛,吉林,长春".Split(',');
            var users = new List<HseReceiverUserEntity>();
            var replies = new List<HseRepliedMessageEntity>();
            var departs = "开发团队,QA|开发团队,礼医|礼来组,EZone|礼来组,O2O|礼来组,Diabetes,DE|LifeScan,SMBG".Split('|');
            const int count = 300;
            var random = new Random();

            for (var j = 0; j < locations.Length; j++)
            {
                var location = locations[j];
                var departSplits = departs[j].Split(',');
                var departOne = departSplits[0];
                var departTwo = departSplits[1];
                var departThree = string.Empty;
                if (departSplits.Length == 3)
                {
                    departThree = departSplits[2];
                }

                #region
                for (var i = 0; i < count; i++)
                {
                    var user = new HseReceiverUserEntity
                    {
                        Tel = GetTel(j.ToString(CultureInfo.InvariantCulture) + i, random),
                        LillyId = GetName(j.ToString() + i, random),
                        CreatedDateTime = DateTime.Now,
                        CreatedLillyId = "john",
                        FristLevelDepartmentName = departOne,
                        SecondLevelDepartmentName = departTwo,
                        ThirdLevelDepartmentName = departThree,
                        Name = GetName("t" + i + j, random),
                        HseMessageId = 1,
                        ManagerName = string.Empty,
                        ManagerTel = string.Empty,
                        ManagerLillyId = string.Empty,
                        Location = location,
                        Status = Status.NoReplied.ToString(),
                        EmergencyContact = string.Empty,
                        EmergencyName = string.Empty,
                    };
                    users.Add(user);
                }
                #endregion
            }

            //hseContext.BulkSaveChanges();

            #region
            users.ForEach(u =>
            {
                var replyCount = random.Next(1, 6);
                var groupItem = new List<HseRepliedMessageEntity>();

                for (var i = 0; i < replyCount; i++)
                {
                    var phoneOrWx = random.Next(0, 2);
                    var isOk = random.Next(0, 2);
                    var timeRanger = random.Next(-1, 1);

                    var item = new HseRepliedMessageEntity
                    {
                        LillyId = u.LillyId,
                        Tel = u.Tel,
                        Content = isOk == 1 ? "ok" : Guid.NewGuid().ToString(),
                        RepliedDate = DateTime.Now.AddMinutes(-1 * i).AddSeconds(-1 * timeRanger),
                        Origin = phoneOrWx == 1 ? Origin.MobileMessage.ToString() : Origin.WeiXin.ToString(),
                        UserRange = UserRange.InSendRange.ToString(),
                        Name = u.Name,
                        Status = isOk == 1 ? Status.Safe.ToString() : Status.Dangerous.ToString(),
                    };

                    groupItem.Add(item);
                }

                var latest = groupItem.OrderByDescending(x => x.RepliedDate).ToList()[0];
                u.Status = latest.Status;

                replies.AddRange(groupItem.OrderBy(x=>x.RepliedDate).ToList());
            });

            #endregion

            var hseContext = (DbContext)hseUserService.Repository.UnitOfWork;

            hseContext.BulkInsert(users);

            var hseReplyContext = (DbContext)hseReplyService.Repository.UnitOfWork;

            hseReplyContext.BulkInsert(replies);
        }

        private static string GetTel(string pre, Random random)
        {
            const string number = "0123456789";
            string telTemp = pre + "{0}";
            var sb = new StringBuilder();

            for (var i = 0; i < 5; i++)
            {
                sb.Append(number[random.Next(1, 10)]);
            }
            return string.Format(telTemp, sb);
        }

        private static string GetName(string pre, Random random)
        {
            const string number = "qwertyuioplkjhgfdsazxcvbnm";
            var sb = new StringBuilder();

            for (var i = 0; i < 5; i++)
            {
                sb.Append(number[random.Next(number.Length)]);
            }
            return pre + sb;
        }
    }
}
