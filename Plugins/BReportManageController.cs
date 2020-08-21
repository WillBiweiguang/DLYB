using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.IO;
using Infrastructure.Web.UI;
using Innocellence.FaultSearch.Model;
using Innocellence.FaultSearch.Services;
using Innocellence.FaultSearch.ViewModel;
using Innocellence.FaultSearch.Service;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Filter;
using Microsoft.Office.Interop.Word;

namespace Innocellence.FaultSearch.Controllers
{
    public class BReportManageController : AdminBaseController<GasInputEntity, GasInputView>
    {
        private IGasInputService _fautService = new GasInputService();
        public BReportManageController(IGasInputService objService)
            : base(objService)
        {
            _fautService = objService;
        }


        public override ActionResult CheckIndex()
        {
            return View();
        }


        public override List<GasInputView> GetListEx(Expression<Func<GasInputEntity, bool>> predicate, PageCondition ConPage)
        {

            predicate = predicate.AndAlso(a => a.IsDeleted != true);

            var q = _fautService.GetList1(predicate, ConPage).ToList();

            return q;

        }
        public ActionResult reportOutPut(string projectNameList)
        {
            if (string.IsNullOrEmpty(projectNameList))
            {
                return Json("请选择至少一个气瓶编号!", JsonRequestBehavior.AllowGet);
            }

            string[] nameList = projectNameList.Split(',');

            if (nameList.Length == 0)
            {
                return Json("没有提供正确的气瓶处理信息", JsonRequestBehavior.AllowGet);
            }
            Export(nameList);
            return SuccessNotification("操作执行完毕");
        }
        public ActionResult Export(string projectNameList,string checkstandList)
        {
            string[] nameList = projectNameList.Split(',');
            Application app = new Application();
            Document doc;
            string type = "1";
            string strFile = "";
            string strFileName3 = "";
            if (type == "1")
            {
               strFile = "regular" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".docx"; ;
                string strFileName1 = "/plugins/Innocellence.FaultSearch/content/regular.docx";
                string strFileName = Server.MapPath(strFileName1);
                string strFileName2 = "/plugins/Innocellence.FaultSearch/content/" + strFile;
                strFileName3 = Server.MapPath(strFileName2);
                if (!System.IO.File.Exists((string)strFileName))
                {
                    System.IO.File.Create(strFileName);
                }

                Object oMissing = System.Reflection.Missing.Value;

                doc = app.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                try
                {

                   outRegularPutWord(oMissing, app, doc, nameList);
                    app.NormalTemplate.Saved = true;
                    doc.SaveAs(strFileName3, oMissing);

                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (doc != null)
                    {
                        doc.Close();//关闭文档
                    }
                    if (app != null)
                    {
                        app.Quit();//退出应用程序
                    }
                }
            }
            else
            {
                strFile = "postpone" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".docx"; ;
                string strFileName1 = "/plugins/Innocellence.FaultSearch/content/postpone.docx";
                string strFileName = Server.MapPath(strFileName1);
                string strFileName2 = "/plugins/Innocellence.FaultSearch/content/" + strFile;
                strFileName3 = Server.MapPath(strFileName2);
                if (!System.IO.File.Exists((string)strFileName))
                {
                    System.IO.File.Create(strFileName);
                }

                Object oMissing = System.Reflection.Missing.Value;

                doc = app.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                try
                {

                    outPutPostponeWord(oMissing, app, doc, nameList);
                    //MemoryStream stream = new MemoryStream();
                    doc.SaveAs(strFileName3, oMissing);
                    //var buf = stream.ToArray();
                    //stream.Close();
                    //using (FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Write))
                    //{
                    //    fs.Write(buf, 0, buf.Length);
                    //    fs.Flush();
                    //    fs.Position = 0;
                    //    fs.Close();
                    //}
                    //return File(buf, "application/ms-word", strFile);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (doc != null)
                    {
                        doc.Close();//关闭文档
                    }
                    if (app != null)
                    {
                        app.Quit();//退出应用程序
                    }
                }

            }
            System.IO.FileStream fs = System.IO.File.OpenRead(strFileName3);
            byte[] fileB = new byte[fs.Length];
            fs.Read(fileB, 0, fileB.Length);
            return File(fileB, "application/ms-word", strFile);

        }

        protected void InsertOrUpdate(GasInputView objModal, string Id)
        {
            if (string.IsNullOrEmpty(Id) || Id == "0")
            {

                _objService.InsertView(objModal);
            }
            else
            {
                _objService.UpdateView(objModal);
            }
        }
        public void outRegularPutWord(Object oMissing, Application app, Document doc, string[] nameList)
        {

            int rows = 10;//表格行数加1是为了标题栏
            int cols = 23;//表格列数

            //输出大标题加粗加大字号水平居中
            app.Selection.Font.Bold = 700;
            app.Selection.Font.Size = 16;
            app.Selection.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
            app.Selection.Text = "液 化 石 油 气 钢 瓶 延 期 使 用 安 全 评 估 报 告";

            //换行添加表格
            object line = Microsoft.Office.Interop.Word.WdUnits.wdLine;
            app.Selection.MoveDown(ref line, oMissing, oMissing);
            app.Selection.TypeParagraph();//换行
            Microsoft.Office.Interop.Word.Range range = app.Selection.Range;
            Microsoft.Office.Interop.Word.Table table = app.Selection.Tables.Add(range, rows, cols, ref oMissing, ref oMissing);

            //设置表格的字体大小粗细
            table.Range.Font.Size = 10;
            table.Range.Font.Bold = 0;
            //设置表格样式  
            table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
            table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
            doc.PageSetup.LineNumbering.Active = 0;
            doc.PageSetup.Orientation = WdOrientation.wdOrientLandscape;
            doc.PageSetup.TopMargin = app.CentimetersToPoints(float.Parse("3.17"));
            doc.PageSetup.BottomMargin = app.CentimetersToPoints(float.Parse("3.17"));
            doc.PageSetup.LeftMargin = app.CentimetersToPoints(float.Parse("2.54"));
            doc.PageSetup.RightMargin = app.CentimetersToPoints(float.Parse("2.54"));
            doc.PageSetup.Gutter = app.CentimetersToPoints(float.Parse("0"));
            doc.PageSetup.HeaderDistance = app.CentimetersToPoints(float.Parse("1.5"));
            doc.PageSetup.FooterDistance = app.CentimetersToPoints(float.Parse("1.75"));
            doc.PageSetup.PageWidth = app.CentimetersToPoints(float.Parse("41.99"));
            doc.PageSetup.PageHeight = app.CentimetersToPoints(float.Parse("29.7"));
            doc.PageSetup.FirstPageTray = WdPaperTray.wdPrinterDefaultBin;
            doc.PageSetup.OtherPagesTray = WdPaperTray.wdPrinterDefaultBin;
            doc.PageSetup.SectionStart = WdSectionStart.wdSectionNewPage;
            doc.PageSetup.OddAndEvenPagesHeaderFooter = 0;
            doc.PageSetup.DifferentFirstPageHeaderFooter = 0;
            doc.PageSetup.VerticalAlignment = WdVerticalAlignment.wdAlignVerticalTop;
            doc.PageSetup.SuppressEndnotes = 0;
            doc.PageSetup.MirrorMargins = 0;
            doc.PageSetup.TwoPagesOnOne = false;
            doc.PageSetup.BookFoldPrinting = false;
            doc.PageSetup.BookFoldRevPrinting = false;
            doc.PageSetup.BookFoldPrintingSheets = 1;
            doc.PageSetup.GutterPos = WdGutterStyle.wdGutterPosLeft;
            //doc.PageSetup.LinesPage = 26;
            doc.PageSetup.LayoutMode = WdLayoutMode.wdLayoutModeLineGrid;
            //设置表格标题
            int rowIndex = 1;
            table.Cell(rowIndex, 1).Range.Text = "检验日期：20__年_月_日";//1行22列

            rowIndex++;
            table.Cell(rowIndex, 1).Range.Text = "检验编号";//4行1列
            table.Cell(rowIndex, 2).Range.Text = "使用单位";//4行1列
            table.Cell(rowIndex, 3).Range.Text = "原始标志";//1行6列

            table.Cell(rowIndex, 8).Range.Text = "技术检验";//1行12列

            table.Cell(rowIndex, 22).Range.Text = "综合评定";//4行1列
            table.Cell(rowIndex, 23).Range.Text = "评定有效日期";//4行1列

            rowIndex++;
            table.Cell(rowIndex, 3).Range.Text = "制造单位（代码）";//3行1列
            table.Cell(rowIndex, 4).Range.Text = "钢瓶编号";//3行1列
            table.Cell(rowIndex, 5).Range.Text = "公称容积（L）";//3行1列
            table.Cell(rowIndex, 6).Range.Text = "制造年月";//3行1列
            table.Cell(rowIndex, 7).Range.Text = "设计壁厚（mm）";//3行1列
            table.Cell(rowIndex, 8).Range.Text = "外观检查（包括焊接接头和底座）";//3行1列

            table.Cell(rowIndex, 9).Range.Text = "壁厚测定";//1行8列

            table.Cell(rowIndex, 17).Range.Text = "容积测定";//3行1列
            table.Cell(rowIndex, 18).Range.Text = "水压试验压力3.2MPa保压时间≥1min";//3行1列
            table.Cell(rowIndex, 19).Range.Text = "瓶阀检验";//3行1列
            table.Cell(rowIndex, 20).Range.Text = "气密性试验压力2.1MPa保压时间≥1min";//3行1列
            table.Cell(rowIndex, 21).Range.Text = "真空度MPa";//3行1列


            rowIndex++;
            table.Cell(rowIndex, 9).Range.Text = "顶部";//3行1列
            table.Cell(rowIndex, 11).Range.Text = "筒体";//3行1列
            table.Cell(rowIndex, 14).Range.Text = "底部";//3行1列



            rowIndex++;


            table.Cell(rowIndex, 9).Range.Text = "1";//3行1列
            table.Cell(rowIndex, 10).Range.Text = "2";//3行1列
            table.Cell(rowIndex, 11).Range.Text = "1";//3行1列
            table.Cell(rowIndex, 12).Range.Text = "2";//3行1列
            table.Cell(rowIndex, 13).Range.Text = "3";//3行1列
            table.Cell(rowIndex, 14).Range.Text = "1";//3行1列
            table.Cell(rowIndex, 15).Range.Text = "2";//3行1列
            table.Cell(rowIndex, 16).Range.Text = "3";//3行1列

            foreach (string item in nameList)
            {
                rowIndex++;
                GasInputView regular = _fautService.GetRegularDetail(item);
                if (!string.IsNullOrEmpty(regular.RI.InspectNum))
                {
                    table.Cell(rowIndex, 1).Range.Text = regular.RI.InspectNum;
                }
                if (!string.IsNullOrEmpty(regular.UseCompany))
                {
                    table.Cell(rowIndex, 2).Range.Text = regular.UseCompany;
                }
                if (!string.IsNullOrEmpty(regular.MadeCompany))
                {
                    table.Cell(rowIndex, 3).Range.Text = regular.MadeCompany;//3行1列
                }
                if (!string.IsNullOrEmpty(regular.BottleNum))
                {
                    table.Cell(rowIndex, 4).Range.Text = regular.BottleNum;
                }
                if (!string.IsNullOrEmpty(regular.Volume))
                {
                    table.Cell(rowIndex, 5).Range.Text = regular.Volume;
                }
                if (!string.IsNullOrEmpty(regular.MadeTime.ToString()))
                {
                    table.Cell(rowIndex, 6).Range.Text = regular.MadeTime.ToString();
                }
                if (!string.IsNullOrEmpty(regular.DesignWall))
                {
                    table.Cell(rowIndex, 7).Range.Text = regular.DesignWall;
                }
                if (!string.IsNullOrEmpty(regular.RI.InspectNum))
                {
                    table.Cell(rowIndex, 8).Range.Text = regular.RI.InspectNum;
                }
                if (!string.IsNullOrEmpty(regular.RI.TopOne))
                {
                    table.Cell(rowIndex, 9).Range.Text = regular.RI.TopOne;//3行1列
                }
                if (!string.IsNullOrEmpty(regular.RI.TopTwo))
                {
                    table.Cell(rowIndex, 10).Range.Text = regular.RI.TopTwo;//3行1列
                }
                if (!string.IsNullOrEmpty(regular.RI.BarrelOne))
                {
                    table.Cell(rowIndex, 11).Range.Text = regular.RI.BarrelOne;//3行1列
                }
                if (!string.IsNullOrEmpty(regular.RI.BarrelTwo))
                {
                    table.Cell(rowIndex, 12).Range.Text = regular.RI.BarrelTwo;//3行1列
                }
                if (!string.IsNullOrEmpty(regular.RI.BarrelThree))
                {
                    table.Cell(rowIndex, 13).Range.Text = regular.RI.BarrelThree;//1行8列
                }
                if (!string.IsNullOrEmpty(regular.RI.BottomOne))
                {
                    table.Cell(rowIndex, 14).Range.Text = regular.RI.BottomOne;//3行1列
                }
                if (!string.IsNullOrEmpty(regular.RI.BottomTwo))
                {
                    table.Cell(rowIndex, 15).Range.Text = regular.RI.BottomTwo;//3行1列
                }
                if (!string.IsNullOrEmpty(regular.RI.BottomThree))
                {
                    table.Cell(rowIndex, 16).Range.Text = regular.RI.BottomThree;//1行8列
                }
                if (!string.IsNullOrEmpty(regular.RI.VolumnTest))
                {
                    table.Cell(rowIndex, 17).Range.Text = regular.RI.VolumnTest;
                }
                if (!string.IsNullOrEmpty(regular.RI.WaterTest))
                {
                    table.Cell(rowIndex, 18).Range.Text = regular.RI.WaterTest;
                }
                if (!string.IsNullOrEmpty(regular.RI.BottleTest))
                {
                    table.Cell(rowIndex, 19).Range.Text = regular.RI.BottleTest;
                }
                if (!string.IsNullOrEmpty(regular.RI.GasTest))
                {
                    table.Cell(rowIndex, 20).Range.Text = regular.RI.GasTest;
                }
                if (!string.IsNullOrEmpty(regular.RI.VacuumTest))
                {
                    table.Cell(rowIndex, 21).Range.Text = regular.RI.VacuumTest;
                }
                if (!string.IsNullOrEmpty(regular.RI.SyncTest))
                {
                    table.Cell(rowIndex, 22).Range.Text = regular.RI.SyncTest;
                }
                if (regular.RI.EvalDate != null)
                {
                    table.Cell(rowIndex, 23).Range.Text = regular.RI.EvalDate.ToString();
                }

            }
            //合并单元格  
            table.Cell(1, 1).Merge(table.Cell(1, 23));
            //合并单元格  
            table.Cell(2, 1).Merge(table.Cell(5, 1));
            table.Cell(2, 2).Merge(table.Cell(5, 2));
            table.Cell(2, 3).Merge(table.Cell(2, 7));
            table.Cell(2, 4).Merge(table.Cell(2, 17));
            table.Cell(2, 5).Merge(table.Cell(5, 22));
            table.Cell(2, 6).Merge(table.Cell(5, 23));

            table.Cell(3, 3).Merge(table.Cell(5, 3));
            table.Cell(3, 4).Merge(table.Cell(5, 4));
            table.Cell(3, 5).Merge(table.Cell(5, 5));
            table.Cell(3, 6).Merge(table.Cell(5, 6));
            table.Cell(3, 7).Merge(table.Cell(5, 7));
            table.Cell(3, 8).Merge(table.Cell(5, 8));

            table.Cell(3, 9).Merge(table.Cell(3, 16));
            table.Cell(3, 10).Merge(table.Cell(5, 17));
            table.Cell(3, 11).Merge(table.Cell(5, 18));
            table.Cell(3, 12).Merge(table.Cell(5, 19));
            table.Cell(3, 13).Merge(table.Cell(5, 20));
            table.Cell(3, 14).Merge(table.Cell(5, 21));

            table.Cell(4, 9).Merge(table.Cell(4, 10));
            table.Cell(4, 10).Merge(table.Cell(4, 12));
            table.Cell(4, 11).Merge(table.Cell(4, 13));
            table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitWindow);


            //for (int i = 1; i < 24; i++)
            //{
            //    table.Cell(2, i).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            //    table.Cell(3, i).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            //    table.Cell(4, i).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            //    table.Cell(5, i).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            //}

            app.Selection.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
        }
        public void outPutPostponeWord(Object oMissing, Application app, Document doc, string[] nameList)
        {
            int rows = 10;//表格行数加1是为了标题栏
            int cols = 29;//表格列数

            //输出大标题加粗加大字号水平居中
            app.Selection.Font.Bold = 700;
            app.Selection.Font.Size = 16;
            app.Selection.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            //doc.PageSetup.PaperSize = WdPaperSize.wdPaperA4;

            doc.PageSetup.LineNumbering.Active = 0;
            doc.PageSetup.Orientation = WdOrientation.wdOrientLandscape;
            doc.PageSetup.TopMargin = app.CentimetersToPoints(float.Parse("3.17"));
            doc.PageSetup.BottomMargin = app.CentimetersToPoints(float.Parse("3.17"));
            doc.PageSetup.LeftMargin = app.CentimetersToPoints(float.Parse("2.54"));
            doc.PageSetup.RightMargin = app.CentimetersToPoints(float.Parse("2.54"));
            doc.PageSetup.Gutter = app.CentimetersToPoints(float.Parse("0"));
            doc.PageSetup.HeaderDistance = app.CentimetersToPoints(float.Parse("1.5"));
            doc.PageSetup.FooterDistance = app.CentimetersToPoints(float.Parse("1.75"));
            doc.PageSetup.PageWidth = app.CentimetersToPoints(float.Parse("41.99"));
            doc.PageSetup.PageHeight = app.CentimetersToPoints(float.Parse("29.7"));
            doc.PageSetup.FirstPageTray = WdPaperTray.wdPrinterDefaultBin;
            doc.PageSetup.OtherPagesTray = WdPaperTray.wdPrinterDefaultBin;
            doc.PageSetup.SectionStart = WdSectionStart.wdSectionNewPage;
            doc.PageSetup.OddAndEvenPagesHeaderFooter = 0;
            doc.PageSetup.DifferentFirstPageHeaderFooter = 0;
            doc.PageSetup.VerticalAlignment = WdVerticalAlignment.wdAlignVerticalTop;
            doc.PageSetup.SuppressEndnotes = 0;
            doc.PageSetup.MirrorMargins = 0;
            doc.PageSetup.TwoPagesOnOne = false;
            doc.PageSetup.BookFoldPrinting = false;
            doc.PageSetup.BookFoldRevPrinting = false;
            doc.PageSetup.BookFoldPrintingSheets = 1;
            doc.PageSetup.GutterPos = WdGutterStyle.wdGutterPosLeft;
            //doc.PageSetup.LinesPage = 26;
            doc.PageSetup.LayoutMode = WdLayoutMode.wdLayoutModeLineGrid;
            //app.Selection.Text = "液 化 石 油 气 钢 瓶 延 期 使 用 安 全 评 估 报 告";

            //换行添加表格
            object line = WdUnits.wdLine;
            app.Selection.MoveDown(ref line, oMissing, oMissing);
            app.Selection.TypeParagraph();//换行
            Microsoft.Office.Interop.Word.Range range = app.Selection.Range;
            Microsoft.Office.Interop.Word.Table table = app.Selection.Tables.Add(range, rows, cols, ref oMissing, ref oMissing);

            //设置表格的字体大小粗细
            table.Range.Font.Size = 10;
            table.Range.Font.Bold = 0;
            //设置表格样式  
            table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
            table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
            //设置表格标题
            int rowIndex = 1;
            table.Cell(rowIndex, 1).Range.Text = "液 化 石 油 气 钢 瓶 延 期 使 用 安 全 评 估 报 告";//1行22列

            rowIndex++;
            table.Cell(rowIndex, 1).Range.Text = "检验日期：20__年_月_日";//1行22列
            rowIndex++;
            table.Cell(rowIndex, 1).Range.Text = "检验编号";//4行1列
            table.Cell(rowIndex, 2).Range.Text = "使用单位";//4行1列
            table.Cell(rowIndex, 3).Range.Text = "原始标志";//1行6列

            table.Cell(rowIndex, 8).Range.Text = "技术检验";//1行12列

            table.Cell(rowIndex, 28).Range.Text = "综合评定";//4行1列
            table.Cell(rowIndex, 29).Range.Text = "评定有效日期";//4行1列

            rowIndex++;
            table.Cell(rowIndex, 3).Range.Text = "制造单位（代码）";//3行1列
            table.Cell(rowIndex, 4).Range.Text = "钢瓶编号";//3行1列
            table.Cell(rowIndex, 5).Range.Text = "公称容积（L）";//3行1列
            table.Cell(rowIndex, 6).Range.Text = "制造年月";//3行1列
            table.Cell(rowIndex, 7).Range.Text = "设计壁厚（mm）";//3行1列
            table.Cell(rowIndex, 8).Range.Text = "延期次数";//3行1列

            table.Cell(rowIndex, 9).Range.Text = "壁厚测定";//1行8列

            table.Cell(rowIndex, 23).Range.Text = "容积测定";//3行1列
            table.Cell(rowIndex, 24).Range.Text = "水压试验压力3.2MPa保压时间≥1min";//  
                                                                          // table.Cell(rowIndex, 24).Width = 200f;

            table.Cell(rowIndex, 25).Range.Text = "瓶阀检验";//3行1列
            table.Cell(rowIndex, 26).Range.Text = "气密性试验压力2.1MPa保压时间≥1min";//3行1列
            //table.Cell(rowIndex, 26).Width = 200f;
            table.Cell(rowIndex, 27).Range.Text = "真空度MPa";//3行1列

            rowIndex++;
            table.Cell(rowIndex, 9).Range.Text = "上圆弧过渡区";//3行1列
            table.Cell(rowIndex, 14).Range.Text = "筒体";//3行1列
            table.Cell(rowIndex, 18).Range.Text = "下圆弧过渡区";//3行1列

            rowIndex++;


            table.Cell(rowIndex, 9).Range.Text = "1";//3行1列
            table.Cell(rowIndex, 10).Range.Text = "2";//3行1列
            table.Cell(rowIndex, 11).Range.Text = "3";//3行1列
            table.Cell(rowIndex, 12).Range.Text = "4*";//3行1列
            table.Cell(rowIndex, 13).Range.Text = "5*";//3行1列

            table.Cell(rowIndex, 14).Range.Text = "1";//3行1列
            table.Cell(rowIndex, 15).Range.Text = "2";//3行1列
            table.Cell(rowIndex, 16).Range.Text = "3*";//3行1列
            table.Cell(rowIndex, 17).Range.Text = "4*";//3行1列

            table.Cell(rowIndex, 18).Range.Text = "1";//3行1列
            table.Cell(rowIndex, 19).Range.Text = "2";//3行1列
            table.Cell(rowIndex, 20).Range.Text = "3";//3行1列
            table.Cell(rowIndex, 21).Range.Text = "4*";//3行1列
            table.Cell(rowIndex, 22).Range.Text = "5*";//3行1列
            foreach (string item in nameList)
            {
                rowIndex++;
                GasInputView postpone = _fautService.GetPostponeDetail(item);
                if (!string.IsNullOrEmpty(postpone.PI.InspectNum))
                {
                    table.Cell(rowIndex, 1).Range.Text = postpone.PI.InspectNum;
                }
                if (!string.IsNullOrEmpty(postpone.UseCompany))
                {
                    table.Cell(rowIndex, 2).Range.Text = postpone.UseCompany;
                }
                if (!string.IsNullOrEmpty(postpone.MadeCompany))
                {
                    table.Cell(rowIndex, 3).Range.Text = postpone.MadeCompany;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.BottleNum))
                {
                    table.Cell(rowIndex, 4).Range.Text = postpone.BottleNum;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.Volume))
                {
                    table.Cell(rowIndex, 5).Range.Text = postpone.Volume;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.MadeTime.ToString()))
                {
                    table.Cell(rowIndex, 6).Range.Text = postpone.MadeTime.ToString();//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.DesignWall))
                {
                    table.Cell(rowIndex, 7).Range.Text = postpone.DesignWall;
                }
                if (!string.IsNullOrEmpty(postpone.PI.InspectDes))
                {
                    table.Cell(rowIndex, 8).Range.Text = postpone.PI.InspectDes;
                }
                if (!string.IsNullOrEmpty(postpone.PI.UpperArcOne))
                {
                    table.Cell(rowIndex, 9).Range.Text = postpone.PI.UpperArcOne;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.UpperArcTwo))
                {
                    table.Cell(rowIndex, 10).Range.Text = postpone.PI.UpperArcTwo;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.UpperArcThree))
                {
                    table.Cell(rowIndex, 11).Range.Text = postpone.PI.UpperArcThree;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.UpperArcFour))
                {
                    table.Cell(rowIndex, 12).Range.Text = postpone.PI.UpperArcFour;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.UpperArcFive))
                {
                    table.Cell(rowIndex, 13).Range.Text = postpone.PI.UpperArcFive;//1行8列
                }
                if (!string.IsNullOrEmpty(postpone.PI.BarrelOne))
                {
                    table.Cell(rowIndex, 14).Range.Text = postpone.PI.BarrelOne;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.BarrelTwo))
                {
                    table.Cell(rowIndex, 15).Range.Text = postpone.PI.BarrelTwo;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.BarrelThree))
                {
                    table.Cell(rowIndex, 16).Range.Text = postpone.PI.BarrelThree;//1行8列
                }
                if (!string.IsNullOrEmpty(postpone.PI.BarrelFour))
                {
                    table.Cell(rowIndex, 17).Range.Text = postpone.PI.BarrelFour;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.BottomOne))
                {
                    table.Cell(rowIndex, 18).Range.Text = postpone.PI.BottomOne;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.BottomTwo))
                {
                    table.Cell(rowIndex, 19).Range.Text = postpone.PI.BottomTwo;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.BottomThree))
                {
                    table.Cell(rowIndex, 20).Range.Text = postpone.PI.BottomThree;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.BottomFour))
                {
                    table.Cell(rowIndex, 21).Range.Text = postpone.PI.BottomFour;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.BottomFive))
                {
                    table.Cell(rowIndex, 22).Range.Text = postpone.PI.BottomFive;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.VolumnTest))
                {
                    table.Cell(rowIndex, 23).Range.Text = postpone.PI.VolumnTest;//1行8列
                }
                if (!string.IsNullOrEmpty(postpone.PI.WaterTest))
                {
                    table.Cell(rowIndex, 24).Range.Text = postpone.PI.WaterTest;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.BottleTest))
                {
                    table.Cell(rowIndex, 25).Range.Text = postpone.PI.BottleTest;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.GasTest))
                {
                    table.Cell(rowIndex, 26).Range.Text = postpone.PI.GasTest;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.VacuumTest))
                {
                    table.Cell(rowIndex, 27).Range.Text = postpone.PI.VacuumTest;//3行1列
                }
                if (!string.IsNullOrEmpty(postpone.PI.SyncTest))
                {
                    table.Cell(rowIndex, 28).Range.Text = postpone.PI.SyncTest;//3行1列
                }
                if (postpone.PI.EvalDate != null)
                {
                    table.Cell(rowIndex, 29).Range.Text = postpone.PI.EvalDate.ToString();//3行1列
                }
            }
            //合并单元格  
            table.Cell(1, 1).Merge(table.Cell(1, 29));
            table.Cell(2, 1).Merge(table.Cell(2, 29));

            //合并单元格  
            table.Cell(3, 1).Merge(table.Cell(6, 1));
            table.Cell(3, 2).Merge(table.Cell(6, 2));
            table.Cell(3, 3).Merge(table.Cell(3, 7));
            table.Cell(3, 4).Merge(table.Cell(3, 23));
            table.Cell(3, 5).Merge(table.Cell(6, 28));
            table.Cell(3, 6).Merge(table.Cell(6, 29));



            table.Cell(4, 3).Merge(table.Cell(6, 3));
            table.Cell(4, 4).Merge(table.Cell(6, 4));
            table.Cell(4, 5).Merge(table.Cell(6, 5));
            table.Cell(4, 6).Merge(table.Cell(6, 6));
            table.Cell(4, 7).Merge(table.Cell(6, 7));
            table.Cell(4, 8).Merge(table.Cell(6, 8));
            table.Cell(4, 9).Merge(table.Cell(4, 22));
            table.Cell(4, 10).Merge(table.Cell(6, 23));
            table.Cell(4, 11).Merge(table.Cell(6, 24));
            table.Cell(4, 12).Merge(table.Cell(6, 25));
            table.Cell(4, 13).Merge(table.Cell(6, 26));
            table.Cell(4, 14).Merge(table.Cell(6, 27));



            table.Cell(5, 9).Merge(table.Cell(5, 13));
            table.Cell(5, 10).Merge(table.Cell(5, 13));
            table.Cell(5, 11).Merge(table.Cell(5, 15));

            table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitWindow);


            app.Selection.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
        }
    }
}