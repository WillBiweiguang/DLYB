// -----------------------------------------------------------------------
//  <copyright file="GridData.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 21:45</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Infrastructure.Utility.Reflection;
using System.Linq;
using Infrastructure.Utility;

namespace Infrastructure.Web.UI
{
    /// <summary>
    /// 列表数据，封装列表的行数据与总记录数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EasyUITreeData
    {
        /// <summary>
        /// 初始化一个<see cref="GridData{T}"/>类型的新实例
        /// </summary>
        public EasyUITreeData()
        { }

        /// <summary>
        /// 获取或设置 行数据
        /// </summary>
        public List<EasyUITreeData> children { get; set; }

        /// <summary>
        /// 获取或设置 数据行数
        /// </summary>
        public string id { get; set; }
        public string text { get; set; }

        //节点状态， 'open' 或 'closed'，默认是 'open'。当设为 'closed' 时，此节点有子节点，并且将从远程站点加载它们。
        public string state { get; set; }

        //指示节点是否被选中
        public bool @checked { get; set; }

        public string iconCls { get; set; }

        public object attributes { get; set; }


        public static List<EasyUITreeData> GetTreeData<T>(List<T> lstAll, string strID, string strName, string strParent)
        {
             MemberFactory<T> t = new MemberFactory<T>();

             var lstParent = lstAll.FindAll(a => t.GetValue(a, strParent) == null || t.GetValue(a, strParent).ToString() == "0");


             return GetTreeDataTemp(lstParent, lstAll, strID, strName, strParent);

        }
        public static List<EasyUITreeData> GetTreeDataTemp<T>(List<T> lstParent, List<T> lstAll, string strID, string strName, string strParent)
        {
            var lstRet=new List<EasyUITreeData>();

            MemberFactory<T> t = new MemberFactory<T>();

            foreach (var a in lstParent)
            {
                var d = new EasyUITreeData()
                {
                    id = t.GetValue(a, strID).ToString(),
                    text = t.GetValue(a, strName).ToString(),
                };

                //所有子节点
                var lstTemp=lstAll.FindAll(e => t.GetValue(e, strParent).ToString() == d.id);

                //移除已经处理的节点
                lstTemp.ForEach(e=> lstAll.Remove(e));

                lstAll.Remove(a);

                //添加子节点及递归子节点的子节点
                d.children = GetTreeDataTemp<T>(lstTemp,lstAll, strID, strName, strParent);

                lstRet.Add(d);
            }

            return lstRet;

        }

        public static List<EasyUITreeData> GetTreeData<T>(
            Func<T, List<T>, List<T>, EasyUITreeData> act, List<T> lstSource)
        {
            List<EasyUITreeData> lst = new List<EasyUITreeData>();
            List<T> lstAdded = new List<T>();


            foreach (T a in lstSource)
            {
                EasyUITreeData obj = new EasyUITreeData();

                var d = act(a, lstAdded, lstSource);
                if (d != null)
                {
                    lst.Add(d);
                }

            }
            return lst;
        }

        public static void SetChecked(List<int> lst,List<EasyUITreeData> lstTree) 
        {
            foreach (var a in lstTree)
            {
                int iID = int.Parse(a.id);

                if (lst.Exists(b => b == iID))
                {
                    a.@checked = true;
                }

                if (a.children != null)
                {
                    SetChecked(lst,a.children);
                }
              
            }
            
        }


        public static void SetChecked<T>(List<T> lst, List<EasyUITreeData> lstTree)
        {
            foreach (var a in lstTree)
            {
                int iID = int.Parse(a.id);

                if (lst.Exists(b => b.ToString() == a.id))
                {
                    a.@checked = true;
                }

                if (a.children != null)
                {
                    SetChecked<T>(lst, a.children);
                }

            }

        }
    }




}