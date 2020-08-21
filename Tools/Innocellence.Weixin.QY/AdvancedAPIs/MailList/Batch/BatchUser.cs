/*----------------------------------------------------------------
    Copyright (C) 2015 DLYB
    
    文件名：DepartmentResult.cs
    文件功能描述：部门接口返回结果
    
    
    创建标识：DLYB - 20130313
    
    修改标识：DLYB - 20130313
    修改描述：整理接口
    
    修改标识：DLYB - 20130408
    修改描述：添加order字段
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DLYB.Weixin.Entities;

namespace DLYB.Weixin.QY.AdvancedAPIs.MailList
{
    /// <summary>
    /// 创建部门返回结果
    /// </summary>
    public class BatchUserResult : WxJsonResult
    {
        /// <summary>
        /// 创建的部门id
        /// </summary>
        public string jobid { get; set; }
    }


    public class BatchUser
    {
        /// <summary>
        /// 部门id
        /// </summary>
        public string media_id { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public callback callback { get; set; }

    }


    public class callback
    {
        public string url { get; set; }
        public string token { get; set; }
        public string encodingaeskey { get; set; }

    }

    
    public class JobResult<T> : WxJsonResult
    {
        /// <summary>
        /// 任务状态，整型，1表示任务开始，2表示任务进行中，3表示任务已完成 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 操作类型，字符串，目前分别有：
        ///1. sync_user(增量更新成员)
        ///2. replace_user(全量覆盖成员)
        ///3. invite_user(邀请成员关注)
        ///4. replace_party(全量覆盖部门) 
        /// </summary>
        public string  type { get; set; }
        /// <summary>
        /// 任务运行总条数 
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 目前运行百分比，当任务完成时为100 
        /// </summary>
        public int percentage { get; set; }
        /// <summary>
        /// 预估剩余时间（单位：分钟），当任务完成时为0 
        /// </summary>
        public int remaintime { get; set; }

        /// <summary>
        ///  详细的处理结果，具体格式参考下面说明。当任务完成后此字段有效  
        /// </summary>
        public T result { get; set; }

    }

    public class JobResultObjectInvite : WxJsonResult
    {
        /// <summary>
        ///  成员UserID。对应管理端的帐号  
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 邀请类型：0 没有邀请方式或未邀请 1 微信邀请 2 邮件邀请  
        /// </summary>
        public int invitetype { get; set; }

    }

    public class JobResultObjectUser : WxJsonResult
    {
        /// <summary>
        ///  成员UserID。对应管理端的帐号  
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        ///操作类型（按位或）：1 表示修改，2 表示新增 
        /// </summary>
        public int action { get; set; }

    }

    public class JobResultObjectParty : WxJsonResult
    {
     /// <summary>
        ///  成员UserID。对应管理端的帐号  
        /// </summary>
        public string partyid { get; set; }
        /// <summary>
        ///操作类型（按位或）：1 新建部门 ，2 更改部门名称， 4 移动部门， 8 修改部门排序
        /// </summary>
        public int action { get; set; }

    }

}
