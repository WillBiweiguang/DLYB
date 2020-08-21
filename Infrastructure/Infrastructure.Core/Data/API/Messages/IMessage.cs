using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//======================================================================
//
//        Copyright (C) 2014-2016 Innocellence团队    
//        All rights reserved
//
//        filename :IMessage
//        description :
//
//        created by hy at  2014/12/30 15:47:10
//        
//
//======================================================================
namespace Infrastructure.Core.Data.API.Messages
{
    /// <summary>
    /// 消息接口
    /// </summary>
    public interface IMessage
    {
        string Title { get; set; }
        string Content { get; set; }
        int MessageType { get; set; }
    }
}
