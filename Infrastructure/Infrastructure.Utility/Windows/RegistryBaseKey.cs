// -----------------------------------------------------------------------
//  <copyright file="RegistryBaseKey.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 18:52</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructure.Utility.Windows
{
    /// <summary>
    /// 注册表基项域
    /// </summary>
    public enum RegistryBaseKey
    {
        /// <summary>
        /// 对应于HKEY_CLASSES_ROOT主键
        /// </summary>
        ClassesRoot,

        /// <summary>
        /// 对应于HKEY_CURRENT_USER主键
        /// </summary>
        CurrentUser,

        /// <summary>
        /// 对应于 HKEY_LOCAL_MACHINE主键
        /// </summary>
        LocalMachine,

        /// <summary>
        /// 对应于 HKEY_USER主键
        /// </summary>
        Users,

        /// <summary>
        /// 对应于HEKY_CURRENT_CONFIG主键
        /// </summary>
        CurrentConfig
    }
}