using Infrastructure.Core;
using Infrastructure.Core.Logging;
//using Infrastructure.Core.Strategy.Logger;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Infrastructure.Core.Performance.Watch
{
    /// <summary>
    /// 代码执行时间监控
    /// </summary>
    public class CodeWatch : IDisposable
    {
        /// <summary>
        /// 应用程序日志类
        /// </summary>
        ILogger logger = LogManager.GetLogger(typeof(CodeWatch));

        readonly Stopwatch _watch = new Stopwatch();
        string _currrentTag;
        int? _currentWarnThreshold;
        readonly Action<string, ILogger, int?, long> _currentThresholdAction;
        /// <summary>
        /// 集成MiniProfiler，以监控站点性能
        /// </summary>
        private static MiniProfiler profiler = MiniProfiler.Current;
        IDisposable profilerStep;
        public CodeWatch(string tag)
        {
            Init(tag);
        }

        private void Init(string tag)
        {
            _currrentTag = tag;
            profilerStep = profiler.Step(_currrentTag);
            _watch.Start();
        }
        /// <summary>
        /// 初始化代码性能监视器
        /// </summary>
        /// <param name="tag">标记</param>
        /// <param name="warnThreshold">提醒阀值（毫秒）</param>
        public CodeWatch(string tag, int warnThreshold)
        {
            Init(tag);
            _currentWarnThreshold = warnThreshold;
        }
        /// <summary>
        /// 初始化代码性能监视器
        /// </summary>
        /// <param name="tag">标记</param>
        /// <param name="warnThreshold">提醒阀值（毫秒）</param>
        /// <param name="action">超过阀值后的操作</param>
        public CodeWatch(string tag, int warnThreshold, Action<string, ILogger, int?, long> action)
        {
            Init(tag);
            _currentWarnThreshold = warnThreshold;
            _currentThresholdAction = action;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 获取当前执行毫秒数
        /// </summary>
        /// <returns></returns>
        public long GetCurrentElapsedMilliseconds()
        {
            return _watch.ElapsedMilliseconds;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            _watch.Stop();
            //释放计数器
            if (profilerStep != null) profilerStep.Dispose();
            if (_currentWarnThreshold == null || _watch.ElapsedMilliseconds < _currentWarnThreshold.Value) return;
            //如果超过阀值，则执行相关操作
            if (_currentThresholdAction != null)
                _currentThresholdAction.Invoke(_currrentTag, logger, _currentWarnThreshold, _watch.ElapsedMilliseconds);

            logger.Warn( "\t{0}:Execution time({1})ms.已超过阀值（{2}）ms.", _currrentTag, _watch.ElapsedMilliseconds, _currentWarnThreshold);
        }
    }
}
