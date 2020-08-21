using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Utility.Data;
using DLYB.CA.Contracts.CommonEntity;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.Entity;
using DLYB.CA.Service.Common;
using DLYB.CA.Service.Interface;
using DLYB.Weixin.QY.AdvancedAPIs.Mass;

namespace DLYB.CA.Service
{
    public class PushService : BasePushService, IPushService
    {
        private readonly IPushHistoryService _pushHistoryService;
        private readonly IPushHistoryDetailService _historyDetailService;
        private readonly IPushFacadeService _pushFacadeService;

        public PushService(IPushHistoryService pushHistoryService,
            IPushHistoryDetailService historyDetailService,
            IPushFacadeService pushFacadeService)
            : base(pushHistoryService, historyDetailService)
        {
            _pushHistoryService = pushHistoryService;
            _historyDetailService = historyDetailService;
            _pushFacadeService = pushFacadeService;
        }

        public Result<object> PushCheck(TargetInfoObject entity, int appId, ContentType contentType)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (appId < 0)
            {
                throw new ArgumentException("appId");
            }

            var result = new Result<object>
               {
                   Status = 200,
               };

            var history = new PushHistoryEntity
            {
                ContentId = entity.ContentId,
                AppId = appId,
                ContentType = contentType.ToString(),
                TargetType = entity.IsToAllUsers ? TargetType.AllUser.ToString() : TargetType.Config.ToString()
            };

            _pushHistoryService.Repository.Insert(history);

            result.Data = history.Id;

            if (entity.IsToAllUsers)
            {
                return result;
            }

            var appInfo = WeChatCommonService.GetAppInfo(appId);

            var verifyResult = _pushFacadeService.Verify(new ConfigedInfo(appInfo), entity, contentType);

            var passPart = verifyResult.Data.Success;
            //string.Join(",", passPart.ToDepartments.Select(JsonHelper.ToJson)) string.Join(",", passPart.ToTags.Select(JsonHelper.ToJson))
            history.ToDepartments = (passPart.ToDepartments != null && passPart.ToDepartments.Any()) ? JsonHelper.ToJson(passPart.ToDepartments) : null;
            history.ToTags = passPart.ToTags != null ? JsonHelper.ToJson(passPart.ToTags) : null;
            history.ToUsers = passPart.ToUsers != null ? string.Join("|", passPart.ToUsers) : null;

            //updated
            _pushHistoryService.Repository.Update(history, new List<string> { "ToDepartments", "ToTags", "ToUsers" });

            if (verifyResult.Status == 200) return result;

            result.Status = verifyResult.Status;

            var errorResult = verifyResult.Data.Error;

            _historyDetailService.Repository.Insert(new PushHistoryDetailEntity
            {
                HistoryId = history.Id,
                ErrorDepartments = errorResult.ErrorDepartments != null ? JsonHelper.ToJson(errorResult.ErrorDepartments) : null,
                ErrorTags = errorResult.ErrorTags != null ? JsonHelper.ToJson(errorResult.ErrorTags) : null,
                ErrorUsers = errorResult.ErrorUsers != null ? string.Join("|", errorResult.ErrorUsers) : null,
                ErrorType = ErrorType.Checked.ToString(),
            });

            return result;
        }

        //update history after send out
        public async Task Update(MassResult messageResult, int historyId)
        {
            if (messageResult.errcode != 0)
            {
                await _historyDetailService.Repository.InsertAsync(new PushHistoryDetailEntity
                  {
                      HistoryId = historyId,
                      ErrorUsers = messageResult.invaliduser,
                      ErrorTags = messageResult.invalidtag,
                      ErrorDepartments = messageResult.invalidparty,
                      ErrorType = ErrorType.Returned.ToString()
                  });
            }
        }
    }

    public class BasePushService
    {
        private readonly IPushHistoryService _pushHistoryService;
        private readonly IPushHistoryDetailService _historyDetailService;

        public BasePushService(IPushHistoryService pushHistoryService, IPushHistoryDetailService historyDetailService)
        {
            _pushHistoryService = pushHistoryService;
            _historyDetailService = historyDetailService;
        }

        /// <summary>
        /// template method
        /// </summary>
        protected virtual void CreatePushHistory(PushHistoryEntity pushHistoryEntity)
        {
            _pushHistoryService.Repository.GetByKey(1);
            _historyDetailService.Repository.GetByKey(1);
        }
    }
}
