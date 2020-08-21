using Infrastructure.Core;
using DLYB.CA.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.ModelsView
{
    public class Finance3rdQueryEntityView : IViewModel
    {
        public int Id { get; set; }
        
        [DescriptionAttribute("公司名称")]
        public string VenderName { get; set; }
        public string ContractNO { get; set; }

        [DescriptionAttribute("会议编码")]
        public string MeetingCode { get; set; }
        [DescriptionAttribute("会议时间")]
        public DateTime MeetingTime { get; set; }
        public string MeetingPlace { get; set; }
        public string LillyId { get; set; }
        public Decimal MoneySum { get; set; }

        [DescriptionAttribute("备注")]
        public string Comment { get; set; }

        [DescriptionAttribute("状态")]
        public string Status { get; set; }

        [DescriptionAttribute("接收日期")]
        public DateTime? ReceiveDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }
        public string Error { get; set; }
        public int CommentLength { get; set; }

        public List<Finance3rdQueryEntityView> Finance3rdList { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (Finance3rdQueryEntity)obj;
            Id = entity.Id;
            VenderName = entity.VenderName;
            MeetingCode = entity.MeetingCode;
            ContractNO = entity.ContractNO;
            MeetingTime = entity.MeetingTime;
            MeetingPlace = entity.MeetingPlace;
            LillyId = entity.LillyId;
            MoneySum = entity.MoneySum;
            ReceiveDate = entity.ReceiveDate;
            Status = entity.Status;
            CreatedDate = entity.CreatedDate;
            UpdateDate = entity.UpdateDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedUserID = entity.UpdatedUserID;
            Comment = entity.Comment;
            CommentLength = entity.Comment != null ? GetLength(entity.Comment) : 0;
            return this;
        }

        public static int GetLength(string str)
        {
            if (str.Length == 0) return 0;
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0; byte[] s = ascii.GetBytes(str);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
            }
            return tempLen;
        }
    }
}
