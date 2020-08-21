﻿using Infrastructure.Core.T4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Core.Config
{
    /// <summary>
    /// 配置
    /// </summary>
    public abstract class ConfigBase
    {
        public ConfigBase()
        {
            UpdateTime = DateTime.Now;
        }
        [T4GenerationIgnoreAttribute]
        public DateTime UpdateTime { get; set; }
    }
}
