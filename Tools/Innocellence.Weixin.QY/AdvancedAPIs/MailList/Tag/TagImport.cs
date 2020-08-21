/*----------------------------------------------------------------
  Copyright (C) 2015 Innocellence
    
  文件名：DepartmentResult.cs
  文件功能描述：标签接口批量导入用户
 * 
  创建标识：Innocellence - 20151014
    
  修改标识：Innocellence - 20151014
  修改描述：整理接口
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.Weixin.QY.AdvancedAPIs.MailList
{
    public class TagImport
    {   
        //Tag标签对应的ID
        public string tagid { get; set; }

        //对应Tag标签要导入的用户list
        public List<string> userlist { get; set; }

        //企业部门ID列表
        public List<int> partylist { get; set; }
    }
}
