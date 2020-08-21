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
    public class FinanceEntityView : IViewModel
    {
        public int Id { get; set; }
        public string TEANO { get; set; }
        public string GEDNO { get; set; }
        public string LillyId { get; set; }
        public Decimal MoneySum { get; set; }

        [DescriptionAttribute("接收日期")]
        public DateTime ReceiveDate { get; set; }

        [DescriptionAttribute("状态")]
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreatedUserID { get; set; }
        public string UpdatedUserID { get; set; }

         [DescriptionAttribute("备注")]
        public string Comment { get; set; }
        public string Error { get; set; }
        public int CommentLength { get; set; }
        public List<FinanceEntityView> FinanceList { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (FinanceQueryEntity)obj;
            Id = entity.Id;
            GEDNO = entity.GEDNO;
            TEANO = entity.TEANO;
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
