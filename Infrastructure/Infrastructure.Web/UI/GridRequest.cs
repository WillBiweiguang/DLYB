// -----------------------------------------------------------------------
//  <copyright file="GridRequest.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 19:12</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

using Infrastructure.Utility.Data;
using Infrastructure.Utility.Exceptions;
using Infrastructure.Utility.Extensions;
using Infrastructure.Utility.Filter;


namespace Infrastructure.Web.UI
{
    /// <summary>
    /// Grid查询请求
    /// </summary>
    public class GridRequest
    {
        /// <summary>
        /// 初始化一个<see cref="GridRequest"/>类型的新实例
        /// </summary>
        public GridRequest(HttpRequestBase request)
        {
            string jsonWhere = request.Params["where"];
            FilterGroup = !jsonWhere.IsNullOrEmpty() ? JsonHelper.FromJson<FilterGroup>(jsonWhere) : new FilterGroup();

            int pageIndex = request.Params["start"].CastTo(1);
            int pageSize = request.Params["length"].CastTo(20);
            if (pageSize < 0)
            {
                pageSize = 1000;
            }
            PageCondition = new PageCondition(pageIndex / pageSize+1, pageSize);
            PageCondition.RowCount = request.Params["iRecordsTotal"].CastTo(0);
            

            List<SortCondition> sortConditions = new List<SortCondition>();

            for (int i = 0; i < 20; i++) {

                if (request.Params["order[" + i.ToString() + "][column]"] == null)
                {
                    break;
                }
                    string fieldsIndex = request.Params["order[" + i.ToString() + "][column]"];

                    string fields = request.Params["columns[" + fieldsIndex + "][data]"];

                    string orders = request.Params["order[" + i.ToString() + "][dir]"];

                  

                    ListSortDirection direction = orders == "desc"
                            ? ListSortDirection.Descending
                            : ListSortDirection.Ascending;
                    sortConditions.Add(new SortCondition(fields, direction));
            }


            PageCondition.SortConditions = sortConditions;
        }

        /// <summary>
        /// 获取 查询条件组
        /// </summary>
        public FilterGroup FilterGroup { get; private set; }

        /// <summary>
        /// 获取 分页查询条件信息
        /// </summary>
        public PageCondition PageCondition { get; private set; }
    }
}