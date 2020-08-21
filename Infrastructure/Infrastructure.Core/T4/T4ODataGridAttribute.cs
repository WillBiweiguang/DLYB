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
//        filename :T4ODataGridAttribute
//        description :
//
//        created by hy at  2015/1/9 14:36:56
//        
//
//======================================================================
namespace Infrastructure.Core.T4
{
    /// <summary>
    /// ODataGrid生成特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class T4ODataGridAttribute : Attribute
    {
        public string ActionName { get; set; }
        public T4ODataGridAttribute()
        {

        }
        public T4ODataGridAttribute(string actionName)
        {
            this.ActionName = actionName;
        }
    }
}
