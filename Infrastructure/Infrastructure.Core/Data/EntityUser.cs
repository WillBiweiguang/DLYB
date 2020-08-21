// -----------------------------------------------------------------------
//  <copyright file="EntityBase.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 18:24</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructure.Core
{
        
    /// <summary>
    /// 可持久化到数据库的数据模型基类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public  class EntityUser : EntityBase<int>
    {
        /// <summary>
        /// 
        /// </summary>
        public EntityUser()
        {
            //IsDeleted = false;
           // CreatedTime = DateTime.Now;
        }

        #region 属性

        /// <summary>
        /// 
        /// </summary>
        public override int Id { get; set; }

        [NotMapped]
        public CultureInfo objCulture { get; set; }



        #endregion
    }
}