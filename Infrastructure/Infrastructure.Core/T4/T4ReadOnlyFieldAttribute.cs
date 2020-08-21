using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//======================================================================
//
//        Copyright (C) 2014-2016 DLYB团队    
//        All rights reserved
//
//        filename :T4ReadOnlyFieldAttribute
//        description :
//
//        created by hy at  2015/1/12 16:11:07
//        
//
//======================================================================
namespace Infrastructure.Core.T4
{
    /// <summary>
    /// T4字段生成只读特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class T4ReadOnlyFieldAttribute : Attribute
    {
        public T4ReadOnlyFieldAttribute()
        {
            this.ReadOnlyType = ReadOnlyTypes.Add;
        }
        public T4ReadOnlyFieldAttribute(ReadOnlyTypes readOnlyType)
        {
            this.ReadOnlyType = readOnlyType;
        }
        public ReadOnlyTypes ReadOnlyType { get; set; }
    }
}
