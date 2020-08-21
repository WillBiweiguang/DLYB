using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using DLYB.CA.Entity;
namespace DLYB.CA.Entity
{
    /// <summary>
    /// 用于记录用户手机端网页打开的性能的数据，用的是js的Performance.Timing数据
    /// </summary>
    public partial class PerformanceLogging : EntityBase<int>
    {
        public override int Id { get; set; }
        public string referer { get; set; }
        public string title { get; set; }
        public string type { get; set; }

        // 在同一个浏览器上下文中，前一个网页（与当前页面不一定同域）unload 的时间戳，如果无前一个网页 unload ，则与 fetchStart 值相等
        public long navigationStart { get; set; }

        // 前一个网页（与当前页面同域）unload 的时间戳，如果无前一个网页 unload 或者前一个网页与当前页面不同域，则值为 0
        public long unloadEventStart { get; set; }

        // 和 unloadEventStart 相对应，返回前一个网页 unload 事件绑定的回调函数执行完毕的时间戳
        public long unloadEventEnd { get; set; }

        // 第一个 HTTP 重定向发生时的时间。有跳转且是同域名内的重定向才算，否则值为 0
        public long redirectStart { get; set; }

        // 最后一个 HTTP 重定向完成时的时间。有跳转且是同域名内部的重定向才算，否则值为 0
        public long redirectEnd { get; set; }

        // 浏览器准备好使用 HTTP 请求抓取文档的时间，这发生在检查本地缓存之前
        public long fetchStart { get; set; }

        // DNS 域名查询开始的时间，如果使用了本地缓存（即无 DNS 查询）或持久连接，则与 fetchStart 值相等
        public long domainLookupStart { get; set; }

        // DNS 域名查询完成的时间，如果使用了本地缓存（即无 DNS 查询）或持久连接，则与 fetchStart 值相等
        public long domainLookupEnd { get; set; }

        // HTTP（TCP） 开始建立连接的时间，如果是持久连接，则与 fetchStart 值相等
        // 注意如果在传输层发生了错误且重新建立连接，则这里显示的是新建立的连接开始的时间
        public long connectStart { get; set; }

        // HTTP（TCP） 完成建立连接的时间（完成握手），如果是持久连接，则与 fetchStart 值相等
        // 注意如果在传输层发生了错误且重新建立连接，则这里显示的是新建立的连接完成的时间
        // 注意这里握手结束，包括安全连接建立完成、SOCKS 授权通过
        public long connectEnd { get; set; }

        // HTTPS 连接开始的时间，如果不是安全连接，则值为 0
        public long secureConnectionStart { get; set; }

        // HTTP 请求读取真实文档开始的时间（完成建立连接），包括从本地读取缓存
        // 连接错误重连时，这里显示的也是新建立连接的时间
        public long requestStart { get; set; }

        // HTTP 开始接收响应的时间（获取到第一个字节），包括从本地读取缓存
        public long responseStart { get; set; }

        // HTTP 响应全部接收完成的时间（获取到最后一个字节），包括从本地读取缓存
        public long responseEnd { get; set; }

        // 开始解析渲染 DOM 树的时间，此时 Document.readyState 变为 loading，并将抛出 readystatechange 相关事件
        public long domLoading { get; set; }

        // 完成解析 DOM 树的时间，Document.readyState 变为 interactive，并将抛出 readystatechange 相关事件
        // 注意只是 DOM 树解析完成，这时候并没有开始加载网页内的资源
        public long domInteractive { get; set; }

        // DOM 解析完成后，网页内资源加载开始的时间
        // 在 DOMContentLoaded 事件抛出前发生
        public long domContentLoadedEventStart { get; set; }

        // DOM 解析完成后，网页内资源加载完成的时间（如 JS 脚本加载执行完毕）
        public long domContentLoadedEventEnd { get; set; }

        // DOM 树解析完成，且资源也准备就绪的时间，Document.readyState 变为 complete，并将抛出 readystatechange 相关事件
        public long domComplete { get; set; }

        // load 事件发送给文档，也即 load 回调函数开始执行的时间
        // 注意如果没有绑定 load 事件，值为 0
        public long loadEventStart { get; set; }

        // load 事件的回调函数执行完毕的时间
        public long loadEventEnd { get; set; }
    }
}
