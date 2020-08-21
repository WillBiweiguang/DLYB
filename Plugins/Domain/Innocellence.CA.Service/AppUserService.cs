using System.Collections.Generic;
using System.Linq;
using Infrastructure.Utility;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Extensions;
using DLYB.CA.Contracts.Contracts;
using DLYB.CA.Contracts.ViewModel;
using DLYB.CA.Service.Common;
using DLYB.Weixin.QY.AdvancedAPIs.MailList;

namespace DLYB.CA.Service
{
    public class AppUserService : IAppUserService
    {
        public IList<AppUserView> QueryUser(string searchKey, PageCondition pageCondition)
        {
            var list = new List<GetMemberResult>();

            list.AddRange(SearchByLillyId(searchKey).ToList());

            list.AddRange(SearchByUserName(searchKey).ToList());

            list.AddRange(SearchByEmail(searchKey).ToList());

            var result = list.AsParallel().Distinct(new CompareUserEntity()).Select(x => new AppUserView
                    {
                        EmailName = x.email,
                        LillyId = x.userid,
                        MobileNumber = x.mobile,
                        Position = x.position,
                        UserName = x.name
                    }).ToList();

            pageCondition.RowCount = result.Count;
            pageCondition.PageIndex = pageCondition.PageIndex == 0 ? 1 : pageCondition.PageIndex;
            result = result.Skip((pageCondition.PageIndex - 1) * pageCondition.PageSize).Take(pageCondition.PageSize).ToList();

            return result;
        }

        private static IEnumerable<GetMemberResult> SearchByLillyId(string lillyId)
        {
            return WeChatCommonService.lstUser.AsParallel().Where(x => !x.userid.IsNullOrEmpty() && x.userid.Contains(lillyId)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userNamme"></param>
        /// <returns></returns>
        private static IEnumerable<GetMemberResult> SearchByUserName(string userNamme)
        {
            return WeChatCommonService.lstUser.AsParallel().Where(x => !x.name.IsNullOrEmpty() && (x.name.ToUpper().Contains(userNamme.ToUpper()) || x.name.ToUpper().Contains(PinYinConverter.Get(userNamme.ToUpper()))) || PinYinConverter.Get(x.name).ToUpper().Contains(userNamme.ToUpper())).ToList();
        }

        private static IEnumerable<GetMemberResult> SearchByEmail(string email)
        {
            return WeChatCommonService.lstUser.AsParallel().Where(x => !x.email.IsNullOrEmpty() && (x.email.ToUpper().Contains(email.ToUpper()))).ToList();
        }
    }

    public class CompareUserEntity : IEqualityComparer<GetMemberResult>
    {
        public bool Equals(GetMemberResult x, GetMemberResult y)
        {
            return x.userid == y.userid;
        }

        public int GetHashCode(GetMemberResult obj)
        {
            return obj.userid.GetHashCode();
        }
    }
}
