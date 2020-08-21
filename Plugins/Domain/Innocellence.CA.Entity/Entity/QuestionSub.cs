using System;
using Infrastructure.Core;

namespace DLYB.CA.Entity
{
   
    public partial class QuestionSub : EntityBase<int>
    {
        public override Int32 Id { get; set; }
       
        /// <summary>
        /// 提问者或回答者Name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 问题或答案
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 文字内容
        /// </summary>
        public string Content { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public Int32 QuestionId { get; set; }
    }
}
