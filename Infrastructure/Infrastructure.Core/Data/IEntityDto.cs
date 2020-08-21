// -----------------------------------------------------------------------
//  <copyright file="IEntityDto.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 2:46</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructure.Core
{
    /// <summary>
    /// 添加DTO
    /// </summary>
    public interface IAddDto : IEntity
    { }


    /// <summary>
    /// 编辑DTO
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IEditDto<TKey>:IEntity
    {
        /// <summary>
        /// 获取或设置 主键，唯一标识
        /// </summary>
        TKey Id { get; set; }
    }
}