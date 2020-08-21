using System;
using Infrastructure.Core;

namespace DLYB.CA.Entity
{
   
    public partial class QuestionSub : EntityBase<int>
    {
        public override Int32 Id { get; set; }
       
        /// <summary>
        /// �����߻�ش���Name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string Content { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public Int32 QuestionId { get; set; }
    }
}
