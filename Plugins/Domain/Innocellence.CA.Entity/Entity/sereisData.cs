using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLYB.CA.Entity
{
    /// <summary>

    /// 定义一个SereisData类 设置其每一组sereisData的一些基本属性

    /// </summary>

    public class SereisData
    {
        /// <summary>
        /// SereisData序列组value
        /// </summary>
        public int Sereisvalue { get; set; }

        /// <summary>
        /// SereisData序列组名称
        /// </summary>
        public string Sereisname { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public int Id { get; set; }
    }
}
