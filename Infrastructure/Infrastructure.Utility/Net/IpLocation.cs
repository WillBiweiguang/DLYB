// -----------------------------------------------------------------------
//  <copyright file="IpLocation.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 1:26</last-date>
// -----------------------------------------------------------------------

namespace Infrastructure.Utility.Net
{
    /// <summary>
    /// IPλ����Ϣ��
    /// </summary>
    public class IpLocation
    {
        /// <summary>
        /// IP��ַ
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// IP��ַ��������
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// λ����Ϣ
        /// </summary>
        public string Local { get; set; }
    }
}