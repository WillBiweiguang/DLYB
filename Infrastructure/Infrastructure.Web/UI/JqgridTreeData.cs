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

namespace Infrastructure.Web.UI
{
    /// <summary>
    /// 列表数据，封装列表的行数据与总记录数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JqgridTreeData
    {
        /// <summary>
        /// 初始化一个<see cref="GridData{T}"/>类型的新实例
        /// </summary>
        public JqgridTreeData()
        { }

        /// <summary>
        /// 获取或设置 行数据
        /// </summary>
        

        /// <summary>
        /// 获取或设置 数据行数
        /// </summary>
        public string id { get; set; }
        public string text { get; set; }

        //节点状态， 'open' 或 'closed'，默认是 'open'。当设为 'closed' 时，此节点有子节点，并且将从远程站点加载它们。
        public int level { get; set; }  //"isLeaf":"false","loaded":"true","expanded

        //指示节点是否被选中
        public bool isLeaf { get; set; }

        public bool loaded { get; set; }

        public bool expanded { get; set; }


        public static List<JqgridTreeData> GetTreeData<T>(List<T> lstAll, string strID, string strName, string strParent,int Level)
        {
             MemberFactory<T> t = new MemberFactory<T>();

             var lstParent = lstAll.FindAll(a => t.GetValue(a, strParent) == null || t.GetValue(a, strParent).ToString() == "0");


             return GetTreeDataTemp(lstParent, lstAll, strID, strName, strParent,0);

        }
        public static List<JqgridTreeData> GetTreeDataTemp<T>(List<T> lstParent, List<T> lstAll, string strID, string strName, string strParent,int iLevel)
        {
            var lstRet = new List<JqgridTreeData>();

            MemberFactory<T> t = new MemberFactory<T>();

            foreach (var a in lstParent)
            {
                var d = new JqgridTreeData()
                {
                    id = t.GetValue(a, strID).ToString(),
                    text = t.GetValue(a, strName).ToString(),
                    level = iLevel,
                    isLeaf = false,
                    loaded=true,
                     expanded=true
                };

                //所有子节点
                var lstTemp=lstAll.FindAll(e => t.GetValue(e, strParent).ToString() == d.id);
                if (lstTemp.Count == 0)
                {
                    d.isLeaf = true;
                }

                //移除已经处理的节点
                lstTemp.ForEach(e=> lstAll.Remove(e));

                lstAll.Remove(a);

                //添加子节点及递归子节点的子节点
                GetTreeDataTemp<T>(lstTemp, lstAll, strID, strName, strParent, iLevel+1);

                lstRet.Add(d);
            }

           // iLevel++;

            return lstRet;

        }

        public static List<JqgridTreeData> GetTreeData<T>(
            Func<T, List<T>, List<T>, JqgridTreeData> act, List<T> lstSource)
        {
            List<JqgridTreeData> lst = new List<JqgridTreeData>();
            List<T> lstAdded = new List<T>();


            foreach (T a in lstSource)
            {
                JqgridTreeData obj = new JqgridTreeData();

                var d = act(a, lstAdded, lstSource);
                if (d != null)
                {
                    lst.Add(d);
                }

            }
            return lst;
        }
    }




}