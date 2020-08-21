﻿using System;
using System.Net;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Infrastructure.Utility.Data;
using Infrastructure.Core.Data;
using Infrastructure.Web.UI;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Contracts;
using System.IO;

namespace DLYB.Web.Controllers
{
    public class FileLogsController : BaseController<LogsModel, LogsView>
    {
        public FileLogsController(ILogsService objService)
            : base(objService)
        {
           
        }


        //public ActionResult Index()
        //{
        //    //if (string.IsNullOrEmpty(strPath))
        //    //{
        //    //    strPath = "Logs";
        //    //}

        //    //string strLogPath = string.Format("{0}{1}", Request.PhysicalApplicationPath, strPath);

        //    //var strFiles = Directory.GetFiles(strLogPath);



        //    //ViewBag.Files = strFiles.OrderByDescending(a => a);

        //    return View();
        //}

        public ActionResult Open(string strPath)
        {
            string strFile = string.Format("{0}{1}", Server.MapPath("~/"), strPath);
            string strRet = System.IO.File.ReadAllText(strFile, System.Text.UTF8Encoding.Default);
            return Content(strRet);
        }


        public ActionResult GetListTree(string strPath,string id)
        {
            if (string.IsNullOrEmpty(strPath))
            {
                strPath = "App_data/Logs\\";
            }

            if (!string.IsNullOrEmpty(id))
            {
                strPath = id;
            }

            strPath = strPath+( strPath.EndsWith("\\") ? "" : "\\");

            string strLogPath = string.Format("{0}{1}", Server.MapPath("~/"), strPath);
            var strFiles = Directory.GetFiles(strLogPath);

            var strFolder = Directory.GetDirectories(strLogPath);

            List<EasyUITreeData> lstRet =null;
            lstRet=(strFolder.Select(a => new EasyUITreeData() { id = a.Replace(Server.MapPath("~/"), ""), state = "closed", text = a.Replace(strLogPath, ""), iconCls = "icon_Folder", attributes = "folder" }).ToList());
            lstRet.AddRange (strFiles.Select(a => new EasyUITreeData() { id = a.Replace(Server.MapPath("~/"), ""), state = "open", text = a.Replace(strLogPath, ""), iconCls = "icon_File",attributes="file" }).ToList());



            return Json(lstRet, JsonRequestBehavior.AllowGet);

        
            //o => o.Asc(f => f.Name, f => f.Id)

        }

        //初始化list页面
        public override List<LogsView> GetListEx(Expression<Func<LogsModel, bool>> predicate, PageCondition ConPage)
        {
            string date=Request["txtDate"];
            string logCate=Request["txtCateLog"];

            if (!string.IsNullOrEmpty(date)) { 
               DateTime dateTime=Convert.ToDateTime(date);
               DateTime dateAdd=dateTime.AddDays(1);
               predicate = predicate.AndAlso(a => a.CreatedDate >= dateTime && a.CreatedDate <= dateAdd);
            }

            if (!string.IsNullOrEmpty(logCate))
            {
                predicate = predicate.AndAlso(a => a.LogCate.Contains(logCate));
            }

            ConPage.SortConditions.Add(new SortCondition("Id", System.ComponentModel.ListSortDirection.Descending));

            var q = _BaseService.GetList<LogsView>(predicate, ConPage);

            return q.ToList();
        }

    }
}
