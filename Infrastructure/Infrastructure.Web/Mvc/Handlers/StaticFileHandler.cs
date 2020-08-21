using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;


namespace Infrastructure.Web.MVC.Handlers
{
    public class StaticFileHandler : IHttpHandler, IRequiresSessionState, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {

            if (!context.Request.IsAuthenticated)
            {
                context.Response.AddHeader("SeesionExist", (context.Session != null ? "yes" : "no"));
                context.Response.StatusCode = 403;
                return;
            }
            // 获取要请求的文件名
            string filePath = context.Server.MapPath(context.Request.FilePath);
            // 用Fiddler查看响应头，如果看到有这个头，就表示是由这段代码处理的。
            context.Response.AddHeader("SeesionExist", (context.Session != null ? "yes" : "no"));
            context.Response.AddHeader("Accept-Ranges", "bytes");
            // 在这里，你可以访问context.Session
            // 设置响应内容标头
            FileInfo fi = new FileInfo(filePath);
            var strName = fi.Name;

            context.Response.ContentType = MimeMapping.GetMimeMapping(strName);

            // 输出文件内容
            context.Response.TransmitFile(filePath);

        }
        public bool IsReusable
        {
            get { return false; }
        }
    }
}
