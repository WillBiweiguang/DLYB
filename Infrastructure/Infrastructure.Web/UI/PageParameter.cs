using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Web.UI
{
    //[Table("CNLED_Data_Category")]
   public class PageParameter
    {
        /// <summary>
        /// DataTable请求服务器端次数
        /// </summary>       
        public string draw { get; set; }

        /// <summary>
        /// 过滤文本
        /// </summary>
        public string search { get; set; }

        /// <summary>
        /// 每页显示的数量
        /// </summary>
        public int length { get; set; }

        /// <summary>
        /// 分页时每页跨度数量
        /// </summary>
        public int start { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// 排序列的数量
        /// </summary>
        public int iSortingCols { get; set; }


        public int iRecordsTotal { get; set; }

        /// <summary>
        /// 逗号分割所有的列
        /// </summary>
        public string sColumns { get; set; }
    
    }
}
