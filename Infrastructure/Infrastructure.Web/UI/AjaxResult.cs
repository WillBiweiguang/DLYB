using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Infrastructure.Web.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonMessage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="text"></param>
        public JsonMessage(int status, string text)
        {
            Status = status;
            Text = text;
        }
        /// <summary>
        /// 
        /// </summary>
        public JsonMessage()
        {
            // TODO: Complete member initialization
            Status = 200;
            Text = string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="text"></param>
        public void SetMessage(int status, string text)
        {
            Status = status;
            Text = text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public void SetMessage(Exception ex)
        {
            Status = (int)HttpStatusCode.BadRequest;
            Text = ex.Message;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AjaxResult<T> 
    {
        /// <summary>
        /// 
        /// </summary>
        public AjaxResult()
        {
            Message = new JsonMessage(200, string.Empty);
            ServerTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            ServerDateTime = DateTime.Now;

            _JsonFlag_ = "__";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public void AddData(T model)
        {
            List<T> data = new List<T>();
            if (this.Data != null)
            {
                data = this.Data.ToList();
            }

            data.Add(model);
            Data = data;
        }

        /// <summary>
        /// 返回给前端的数据
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// 执行此操作时的服务器时间
        /// </summary>
        public string ServerTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime  ServerDateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string _JsonFlag_ { get; set; }

        /// <summary>
        /// Stats
        /// </summary>
        public JsonMessage Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="text"></param>
        public void SetMessage(int status, string text)
        {
            Message.SetMessage(status, text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public void SetMessage(Exception ex)
        {
            Message.SetMessage(ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public void SetMessage(JsonMessage ex)
        {
            Message = ex;
        }
    }
}