/*----------------------------------------------------------------
    Copyright (C) 2016 DLYB

    文件名：DLYBMessageQueueThreadUtility.cs
    文件功能描述：DLYBMessageQueue消息列队线程处理


    创建标识：DLYB - 20160210

----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DLYB.Weixin.MessageQueue;

namespace DLYB.Weixin.Threads
{
    /// <summary>
    /// DLYBMessageQueue线程自动处理
    /// </summary>
    public class DLYBMessageQueueThreadUtility
    {
        private readonly int _sleepMilliSeconds;


        public DLYBMessageQueueThreadUtility(int sleepMilliSeconds = 2000)
        {
            _sleepMilliSeconds = sleepMilliSeconds;
        }

        /// <summary>
        /// 析构函数，将未处理的列队处理掉
        /// </summary>
        ~DLYBMessageQueueThreadUtility()
        {
            try
            {
                var mq = new DLYBMessageQueue();
                System.Diagnostics.Trace.WriteLine(string.Format("DLYBMessageQueueThreadUtility执行析构函数"));
                System.Diagnostics.Trace.WriteLine(string.Format("当前列队数量：{0}", mq.GetCount()));

                DLYBMessageQueue.OperateQueue();//处理列队
            }
            catch (Exception ex)
            {
                //此处可以添加日志
                System.Diagnostics.Trace.WriteLine(string.Format("DLYBMessageQueueThreadUtility执行析构函数错误：{0}", ex.Message));
            }

        }

        /// <summary>
        /// 启动线程轮询
        /// </summary>
        public void Run()
        {
            do
            {
                DLYBMessageQueue.OperateQueue();
                Thread.Sleep(_sleepMilliSeconds);
            } while (true);
        }
    }
}
