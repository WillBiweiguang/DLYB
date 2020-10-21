using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Web.Domain.Common
{
    public class TableListHelper
    {
        public static List<T> GenerateIndex<T>(List<T> list, PageCondition con) where T : ViewModelBase
        {
            for (int i = 1; i <= list.Count; i++)
            {
                list[i - 1].Index = (con.PageIndex - 1) * con.PageSize + i;
            }
            return list;
        }
    }
}
