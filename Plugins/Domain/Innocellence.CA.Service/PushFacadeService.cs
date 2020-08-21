using System;
using System.Collections.Concurrent;
using System.Linq;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Service.Interface;

namespace DLYB.CA.Service
{
    public class PushFacadeService : IPushFacadeService
    {
        private static readonly ConcurrentDictionary<ContentType, IPushStrategyService> PushStrategyServices = new ConcurrentDictionary<ContentType, IPushStrategyService>();

        public Result<CheckResult> Verify(ConfigedInfo appInfo, object entity, ContentType contentType)
        {
            var instance = GetPushStrategyService(contentType);

            if (instance == null)
            {
                throw new Exception(string.Format("Do not find out Implemented for this TokenType(type name:{0}).", contentType));
            }

            return instance.CheckPushRule(entity, appInfo);
        }

        private IPushStrategyService GetPushStrategyService(ContentType contentType)
        {
            return PushStrategyServices.GetOrAdd(contentType, x =>
            {
                var targetTypes =
                    typeof(IPushStrategyService).Assembly.GetTypes()
                        .AsParallel()
                        .Where(type => type.IsClass && typeof(IPushStrategyService).IsAssignableFrom(type));

                IPushStrategyService instance = null;
                var targetType = targetTypes.FirstOrDefault(
                    type =>
                    {
                        instance = (IPushStrategyService)Activator.CreateInstance(type);
                        return type.GetProperty("ContentType").GetValue(instance).ToString() == contentType.ToString();
                    });

                return targetType == null ? null : instance;
            });
        }
    }
}
