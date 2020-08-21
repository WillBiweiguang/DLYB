using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Web.UI
{
    //[Table("CNLED_Data_Category")]
   public class PageParameter
    {
        /// <summary>
        /// DataTable����������˴���
        /// </summary>       
        public string draw { get; set; }

        /// <summary>
        /// �����ı�
        /// </summary>
        public string search { get; set; }

        /// <summary>
        /// ÿҳ��ʾ������
        /// </summary>
        public int length { get; set; }

        /// <summary>
        /// ��ҳʱÿҳ�������
        /// </summary>
        public int start { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// �����е�����
        /// </summary>
        public int iSortingCols { get; set; }


        public int iRecordsTotal { get; set; }

        /// <summary>
        /// ���ŷָ����е���
        /// </summary>
        public string sColumns { get; set; }
    
    }
}
