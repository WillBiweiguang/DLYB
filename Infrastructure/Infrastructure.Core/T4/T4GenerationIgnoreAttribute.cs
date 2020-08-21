﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//======================================================================
//
//        Copyright (C) 2014-2016 DLYB团队    
//        All rights reserved
//
//        filename :T4GenerationIgnore
//        description :
//
//        created by hy at  2014/10/27 21:22:07
//        
//
//======================================================================
namespace Infrastructure.Core.T4
{
    /// <summary>
    /// T4生成忽略【类或属性】
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class T4GenerationIgnoreAttribute : Attribute
    {
        /// <summary>
        /// 使用默认的构造函数表示此类或此属性在生成时忽略
        /// </summary>
        public T4GenerationIgnoreAttribute()
        {
            this.IgnorePart = IgnoreParts.All;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ignorePart"></param>
        public T4GenerationIgnoreAttribute(IgnoreParts ignorePart)
        {
            this.IgnorePart = ignorePart;
        }
        public IgnoreParts IgnorePart { get; set; }
    }
}
