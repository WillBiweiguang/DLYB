 var mxOcx = document.getElementById("MxDrawXCtrl");
    // 执行控件命令
 function DoCmd(iCmd) {     
        mxOcx.DoCommand(iCmd);
}
function myclick5() {
    $("body").mLoading({ text: "识别中，请稍候" });
    setTimeout(GetWelding(), 100);
    $("body").mLoading("hide");
    }
function DoCommandEventFunc(iCmd) {
    if (iCmd == 1) {
        JiaoWeldAdd();
    }
}
//初始化

function InitMxDrawX() {

    var mxOcx = document.getElementById("MxDrawXCtrl");

    if (mxOcx) {

        if (!mxOcx.IsIniting()) {

            clearInterval(mxtime);

            var filename = $('#fileName').val();
            var filePath = $('#filePath').val();

            if (!filePath) {
                var path = "http://" + window.location.host + '/Files/BeamInfo/' + filename;
                mxOcx.OpenWebDwgFile(path);
            } else {
                mxOcx.OpenWebDwgFile(filePath);
            }
        }
    }
}

mxtime = setInterval(InitMxDrawX, 100);
function GetWelding() {
    var myDate = new Date();
    var strtimestart = myDate.toLocaleString();
    var mxOcx = document.all.item("MxDrawXCtrl");
    //实例化一个构造选择集进行过滤,该类封装了选择集及其处理函数。
    var ss = mxOcx.NewSelectionSet();
    //构造一个过滤链表
    var spFilte = mxOcx.NewResbuf();
    spFilte.AddStringEx("HATCH", 5020);
    //得到当前空间的所有实体
    ss.AllSelect(spFilte);
    var ArrowArray = [];
    for (var i = 0; i < ss.Count; i++) {
        var ent = ss.Item(i);
        var HatchArrow = ent;
        var PolylineArray = HatchArrow.GetPolylines();
        var PAnum = PolylineArray.Count;
        if (PAnum == 1) {
            var polyline = PolylineArray.AtObject(0);
            var num = polyline.numVerts;
            if (num == 4) {
                var pt1 = polyline.GetPointAt(0);
                var pt2 = polyline.GetPointAt(1);
                var pt3 = polyline.GetPointAt(2);
                var pt4 = polyline.GetPointAt(3);
                if (polyline.IsClosedStatus == true)//相同
                {
                    var mL1 = pt1.DistanceTo(pt2);
                    var mL2 = pt1.DistanceTo(pt3);
                    var mL3 = pt2.DistanceTo(pt3);
                    if ((mL1 <= mL2 + 0.07 && mL1 >= mL2 - 0.07) || (mL2 <= mL1 + 0.07 && mL2 >= mL1 - 0.07)) {
                        var x = (pt2.x + pt3.x) / 2;
                        var y = (pt2.y + pt3.y) / 2;
                        var midP = mxOcx.NewPoint();
                        midP.x = x;
                        midP.y = y;
                        var MObjectID = HatchArrow.handle;
                        var myArrowtest = new myArrow(MObjectID, pt1, pt2, pt3, midP)
                        ArrowArray[ArrowArray.length] = myArrowtest;
                    }
                    else if ((mL2 <= mL3 + 0.07 && mL2 >= mL3 - 0.07) || (mL3 <= mL2 + 0.07 && mL3 >= mL2 - 0.07)) {
                        var x = (pt1.x + pt2.x) / 2;
                        var y = (pt1.y + pt2.y) / 2;
                        var midP = mxOcx.NewPoint();
                        midP.x = x;
                        midP.y = y;
                        var MObjectID = HatchArrow.handle;
                        var myArrowtest = new myArrow(MObjectID, pt3, pt1, pt2, midP)
                        ArrowArray[ArrowArray.length] = myArrowtest;
                    }
                    else if ((mL1 <= mL3 + 0.07 && mL1 >= mL3 - 0.07) || (mL3 <= mL1 + 0.07 && mL3 >= mL1 - 0.07)) {
                        var x = (pt1.x + pt3.x) / 2;
                        var y = (pt1.y + pt3.y) / 2;
                        var midP = mxOcx.NewPoint();
                        midP.x = x;
                        midP.y = y;
                        var MObjectID = HatchArrow.handle;
                        var myArrowtest = new myArrow(MObjectID, pt2, pt1, pt3, midP)
                        ArrowArray[ArrowArray.length] = myArrowtest;
                    }

                }
            }
        }
    }
    var NSArrowArray = DeletSameArrow(ArrowArray);
    var m_ResWeldArr = [];
    for (var i = 0; i < NSArrowArray.length; i++)
    {
        var myWeld = GetWeldingType(NSArrowArray[i]);
        if (myWeld == null) {
            continue;
        }
        m_ResWeldArr.push(myWeld);
        //根据类型修改颜色
        ChangeColorByType(myWeld);

        //可在此处逐个获取识别到的焊缝符号的信息

        //myWeld——为识别到的焊缝符号，myWeld.myWelArrow.myArrowObjectID 保存的是箭头的handle
        //myWeld.myWelType  是焊缝的类型代号字符串
        //1-不开坡口对接焊   "N_PoKDuiJieH"
        //2 -单面-单侧-坡口-背面封底-对接焊缝    "Y_DMDCPoKDuiJieH"
        //3 单面-双侧-坡口-背面封底-对接焊缝----3(12)  "Y_DMSCPoKDuiJieH"
        //4 -双面-单侧-坡口-对接焊缝   "Y_SMDCPoKDuiJieH"
        //5 双面-双侧-坡口-对接焊缝----  "Y_SMSCPoKDuiJieH"
        //6 单面-单侧-坡口-角焊缝-----(15) "Y_DMDCPoKJiaoH"
        //7 单面-坡口-盖板-角焊缝-----(16)  "Y_DMPoKGaiBJiaoH"
        //8 双面-坡口-背面坡口-角焊缝-----(17) "Y_SMPoKBeiPJiaoH"
        //10  双面-坡口-熔透----  "Y_SMPoKRTH"
        //11 双面坡口盖板熔透,有图形(20) "Y_SMPoKGaiBRTH"
        //13 单面-角焊缝----13(22)    "Y_DMJiaoH"
        //14-双面角焊缝   "Y_SMJiaoH"
    }
    mxOcx.UpdateDisplay();
    //保存焊缝符号数据
    if (m_ResWeldArr.length == 0) {
        alert('未识别到焊缝');
        return;
    }
    var weldData = [];
    for (var i = 0; i < m_ResWeldArr.length; i++) {
        weldData.push({ WeldType: m_ResWeldArr[i].myWelType, HandleID: m_ResWeldArr[i].myWelArrow.myArrowObjectID });
    }
    saveWeldData(weldData);
    //var myDateEnd = new Date();
    //var strtimesend = myDateEnd.toLocaleTimeString();
    //alert("开始时间：" + strtimestart + ",结束时间：" + strtimesend)
}
function DrawCircleOfArrow(m_handle) {
    //删掉上一个画圈的handleid
    if (lastCircleHandle > 0) {
        deleClrByID(lastCircleHandle);
        lastCircleHandle = 0;
    }
    var mxOcx = document.all.item("MxDrawXCtrl");
    var database = mxOcx.GetDatabase();
    var ent = database.HandleToObject(m_handle);
    var HatchArrow = ent;
    var PolylineArray = HatchArrow.GetPolylines();
    var polyline = PolylineArray.AtObject(0);
    var pt1 = polyline.GetPointAt(0);

    //创建一个图层,名为"CircleLayer"
    mxOcx.AddLayer("CircleLayer");

    //设置当前图层为"CircleLayer"
    mxOcx.LayerName = "CircleLayer";

    mxOcx.DrawColor = 255;
    lastCircleHandle = mxOcx.DrawCircle(pt1.x, pt1.y, 10);

    //更新视区显示
    mxOcx.UpdateDisplay();
}
function GetWeldingType(myArrow) {
    var myWeld = new myWelding(null, null, null, 0,"", null, 0, null,null, null, null, null, 0, null, null, null, null, 0, null, null, null, "",
                                null, "", null, "", 0, null, "", null, "", null, "", 0, null, null, 0, null, null, null, 0,
                                 null, null, null, null, null, null, null, null, null, null, 0, null, null, null, 0, "")
    var mxOcx = document.all.item("MxDrawXCtrl");
    var database = mxOcx.GetDatabase();
    ent = database.HandleToObject(myArrow.myArrowObjectID);
   var points = ent.GetBoundingBox2();
   var minP= points.Item(0);
   var maxP = points.Item(1);
    //查找引线   
    var sAL = mxOcx.NewSelectionSet();
    //构造一个过滤链表
    var spALFilte = mxOcx.NewResbuf();
    spALFilte.AddStringEx("LWPOLYLINE", 5020);
    sAL.Select(1, minP, maxP, spALFilte);
    var PolyLArray2 = []; //两顶点的polyline
    var PolyLArray3 = []; //三顶点的polyline
    for (var j = 0; j < sAL.Count; j++) {
        var ent = sAL.Item(j);
        var mployLine = ent;
        if (mployLine.numVerts == 2) {
            PolyLArray2[PolyLArray2.length] = mployLine;
        }
        else if (mployLine.numVerts == 3) {
            PolyLArray3[PolyLArray3.length] = mployLine;
        }     
    }
    var myDimes = GetDimensArrowLine(myArrow, PolyLArray2, PolyLArray3)
    var myTempWeldingArray = null;
    if (myDimes==null)
    {
        return null;
    }
    else if (myDimes.myCAnnotationLine.myPolyPNum == 2) {
        if (CheckLineType(myDimes.myCAnnotationLine.myPolyStartP, myDimes.myCAnnotationLine.myPolyEndP) == "X") {
            if (myDimes.myCAnnotationCHSE == "Start") {
                var sYL = mxOcx.NewSelectionSet();
                var spYLFilte = mxOcx.NewResbuf();
                spYLFilte.AddStringEx("LWPOLYLINE", 5020);
                var miP = mxOcx.NewPoint();
                var maP = mxOcx.NewPoint();
                miP.x = myDimes.myCAnnotationLine.myPolyEndP.x - 0.1;
                miP.y = myDimes.myCAnnotationLine.myPolyEndP.y - 0.1;
                maP.x = myDimes.myCAnnotationLine.myPolyEndP.x + 0.1;
                maP.y = myDimes.myCAnnotationLine.myPolyEndP.y + 0.1;
                sYL.Select(1, miP, maP, spYLFilte);
                var YLPolyLArray2 = []; //两顶点的polyline
                var YLPolyLArray3 = []; //三顶点的polyline
                for (var j = 0; j < sYL.Count; j++) {
                    var ent = sYL.Item(j);                  
                    var mployLine = ent;
                    if (mployLine.numVerts == 2) {
                        YLPolyLArray2[YLPolyLArray2.length] = mployLine;
                    }
                    else if (mployLine.numVerts == 3) {
                        YLPolyLArray3[YLPolyLArray3.length] = mployLine;
                    }
                }
                for (var jtwo = 0; jtwo < YLPolyLArray2.length; jtwo++) {
                    if (IsHorizontal(YLPolyLArray2[jtwo].GetPointAt(0), YLPolyLArray2[jtwo].GetPointAt(1)) && IsNearPoint(myDimes.myCAnnotationLine.myPolyEndP, YLPolyLArray2[jtwo].GetPointAt(0))) {
                        var Hlength = GetDisFromTwoPoint(myDimes.myCAnnotationLine.myPolyEndP, YLPolyLArray2[jtwo].GetPointAt(1))
                        var mtline = new myPolyLine(YLPolyLArray2[jtwo].handle, 2, YLPolyLArray2[jtwo].GetPointAt(0),
                                                   YLPolyLArray2[jtwo].GetPointAt(1), null, "Start")
                        myTempWeldingArray = new myWelding(myDimes.myCAnnotationArrow, myDimes.myCAnnotationLine, mtline, Hlength,
                                                             myDimes.myCAnnotationCHSE, myDimes.myCAnnotationCHP, 2, myDimes.myCAnnotationLine.myPolyEndP,
                                                              YLPolyLArray2[jtwo].GetPointAt(1), null, null, null, 0, null, null, null, null, 0, null, null, null, "",
                                         null, "", null, "", 0, null, "", null, "", null, "", 0, null, null, 0, null, null, null, 0,
                                         null, null, null, null, null, null, null, null, null, null, 0, null, null, null, 0, "")
                        break;
                    }
                    else if (IsHorizontal(YLPolyLArray2[jtwo].GetPointAt(0), YLPolyLArray2[jtwo].GetPointAt(1)) && IsNearPoint(myDimes.myCAnnotationLine.myPolyEndP, YLPolyLArray2[jtwo].GetPointAt(1))) {
                        var Hlength = GetDisFromTwoPoint(myDimes.myCAnnotationLine.myPolyEndP, YLPolyLArray2[jtwo].GetPointAt(0))
                        var mtline = new myPolyLine(YLPolyLArray2[jtwo].handle, 2, YLPolyLArray2[jtwo].GetPointAt(0),
                                                   YLPolyLArray2[jtwo].GetPointAt(1), null, "End")
                        myTempWeldingArray = new myWelding(myDimes.myCAnnotationArrow, myDimes.myCAnnotationLine, mtline, Hlength,
                                                myDimes.myCAnnotationCHSE, myDimes.myCAnnotationCHP, 2, myDimes.myCAnnotationLine.myPolyEndP,
                                                YLPolyLArray2[jtwo].GetPointAt(0), null, null, null, 0, null, null, null, null, 0, null, null, null, "",
                                          null, "", null, "", 0, null, "", null, "", null, "", 0, null, null, 0, null, null, null, 0,
                                          null, null, null, null, null, null, null, null, null, null, 0, null, null, null, 0, "")
                        break;
                    }
                }
                for (var t = 0; t < YLPolyLArray3.length; t++) {
                    if (IsHorizontal(YLPolyLArray3[t].GetPointAt(1), YLPolyLArray3[t].GetPointAt(2)) && IsNearPoint(myDimes.myCAnnotationLine.myPolyEndP, YLPolyLArray3[t].GetPointAt(1))) {
                        var Hlength = GetDisFromTwoPoint(myDimes.myCAnnotationLine.myPolyEndP, YLPolyLArray3[t].GetPointAt(2))
                        var mtline = new myPolyLine(YLPolyLArray3[t].handle, 3, YLPolyLArray3[t].GetPointAt(0),
                                               YLPolyLArray3[t].GetPointAt(2), YLPolyLArray3[t].GetPointAt(1), "Mid")
                        myTempWeldingArray = new myWelding(myDimes.myCAnnotationArrow, myDimes.myCAnnotationLine, mtline, Hlength,
                                                      myDimes.myCAnnotationCHSE, myDimes.myCAnnotationCHP, 2, myDimes.myCAnnotationLine.myPolyEndP,
                                                       YLPolyLArray3[t].GetPointAt(2), null, null, null, 0, null, null, null, null, 0, null, null, null, "",
                                          null, "", null, "", 0, null, "", null, "", null, "", 0, null, null, 0, null, null, null, 0,
                                          null, null, null, null, null, null, null, null, null, null, 0, null, null, null, 0, "")
                        break;
                    }
                    else if (IsHorizontal(YLPolyLArray3[t].GetPointAt(1), YLPolyLArray3[t].GetPointAt(0)) && IsNearPoint(myDimes.myCAnnotationLine.myPolyEndP, YLPolyLArray3[t].GetPointAt(1))) {
                        var Hlength = GetDisFromTwoPoint(myDimes.myCAnnotationLine.myPolyEndP, YLPolyLArray3[t].GetPointAt(0))
                        var mtline = new myPolyLine(YLPolyLArray3[t].handle, 3, YLPolyLArray3[t].GetPointAt(0),
                                              YLPolyLArray3[t].GetPointAt(2), YLPolyLArray3[t].GetPointAt(1), "Mid")
                        myTempWeldingArray = new myWelding(myDimes.myCAnnotationArrow, myDimes.myCAnnotationLine, mtline, Hlength,
                                                     myDimes.myCAnnotationCHSE, myDimes.myCAnnotationCHP, 2, myDimes.myCAnnotationLine.myPolyEndP,
                                                       YLPolyLArray3[t].GetPointAt(0), null, null, null, 0, null, null, null, null, 0, null, null, null, "",
                                          null, "", null, "", 0, null, "", null, "", null, "", 0, null, null, 0, null, null, null, 0,
                                          null, null, null, null, null, null, null, null, null, null, 0, null, null, null, 0, "")
                        break;

                    }
                }

            }
            else if (myDimes.myCAnnotationCHSE == "End") {
                var sYL = mxOcx.NewSelectionSet();
                var spYLFilte = mxOcx.NewResbuf();
                spYLFilte.AddStringEx("LWPOLYLINE", 5020);
                var miP = mxOcx.NewPoint();
                var maP = mxOcx.NewPoint();
                miP.x = myDimes.myCAnnotationLine.myPolyEndP.x - 0.1;
                miP.y = myDimes.myCAnnotationLine.myPolyEndP.y - 0.1;
                maP.x = myDimes.myCAnnotationLine.myPolyEndP.x + 0.1;
                maP.y = myDimes.myCAnnotationLine.myPolyEndP.y + 0.1;
                sYL.Select(1, miP, maP, spYLFilte);
                var YLPolyLArray2 = []; //两顶点的polyline
                var YLPolyLArray3 = []; //三顶点的polyline
                for (var j = 0; j < sYL.Count; j++) {
                    var ent = sYL.Item(j);
                    var mployLine = ent;
                    if (mployLine.numVerts == 2) {
                        YLPolyLArray2[YLPolyLArray2.length] = mployLine;
                    }
                    else if (mployLine.numVerts == 3) {
                        YLPolyLArray3[YLPolyLArray3.length] = mployLine;
                    }
                }
                for (var jtwo = 0; jtwo < YLPolyLArray2.length; jtwo++) {
                    if (IsHorizontal(YLPolyLArray2[jtwo].GetPointAt(0), YLPolyLArray2[jtwo].GetPointAt(1)) && IsNearPoint(myDimes.myCAnnotationLine.myPolyStartP, YLPolyLArray2[jtwo].GetPointAt(0))) {
                        var Hlength = GetDisFromTwoPoint(myDimes.myCAnnotationLine.myPolyStartP, YLPolyLArray2[jtwo].GetPointAt(1))
                        var mtline = new myPolyLine(YLPolyLArray2[jtwo].handle, 2, YLPolyLArray2[jtwo].GetPointAt(0),
                                                  YLPolyLArray2[jtwo].GetPointAt(1), null, "Start")
                        myTempWeldingArray = new myWelding(myDimes.myCAnnotationArrow, myDimes.myCAnnotationLine, mtline, Hlength,
                                                   myDimes.myCAnnotationCHSE, myDimes.myCAnnotationCHP, 2, myDimes.myCAnnotationLine.myPolyStartP,
                                                   YLPolyLArray2[jtwo].GetPointAt(1), null, null, null, 0, null, null, null, null, 0, null, null, null, "",
                                          null, "", null, "", 0, null, "", null, "", null, "", 0, null, null, 0, null, null, null, 0,
                                          null, null, null, null, null, null, null, null, null, null, 0, null, null, null, 0, "")
                        break;

                    }
                    else if (IsHorizontal(YLPolyLArray2[jtwo].GetPointAt(0), YLPolyLArray2[jtwo].GetPointAt(1)) && IsNearPoint(myDimes.myCAnnotationLine.myPolyStartP, YLPolyLArray2[jtwo].GetPointAt(1))) {
                        var Hlength = GetDisFromTwoPoint(myDimes.myCAnnotationLine.myPolyStartP, YLPolyLArray2[jtwo].GetPointAt(0))
                        var mtline = new myPolyLine(YLPolyLArray2[jtwo].handle, 2, YLPolyLArray2[jtwo].GetPointAt(0),
                                                YLPolyLArray2[jtwo].GetPointAt(1), null, "End")
                        myTempWeldingArray = new myWelding(myDimes.myCAnnotationArrow, myDimes.myCAnnotationLine, mtline, Hlength,
                                                   myDimes.myCAnnotationCHSE, myDimes.myCAnnotationCHP, 2, myDimes.myCAnnotationLine.myPolyStartP,
                                                   YLPolyLArray2[jtwo].GetPointAt(0), null, null, null, 0, null, null, null, null, 0, null, null, null, "",
                                          null, "", null, "", 0, null, "", null, "", null, "", 0, null, null, 0, null, null, null, 0,
                                          null, null, null, null, null, null, null, null, null, null, 0, null, null, null, 0, "")
                        break;

                    }
                }
                for (var t = 0; t < YLPolyLArray3.lengthf; t++) {
                    if (IsHorizontal(YLPolyLArray3[t].GetPointAt(1), YLPolyLArray3[t].GetPointAt(2)) && IsNearPoint(myDimes.myCAnnotationLine.myPolyStartP, YLPolyLArray3[t].GetPointAt(1))) {
                        var Hlength = GetDisFromTwoPoint(myDimes.myCAnnotationLine.myPolyStartP, YLPolyLArray3[t].GetPointAt(2))
                        var mtline = new myPolyLine(YLPolyLArray3[t].handle, 3, YLPolyLArray3[t].GetPointAt(0),
                                               YLPolyLArray3[t].GetPointAt(2), YLPolyLArray3[t].GetPointAt(1), "Mid")
                        myTempWeldingArray = new myWelding(myDimes.myCAnnotationArrow, myDimes.myCAnnotationLine, mtline, Hlength,
                                                     myDimes.myCAnnotationCHSE, myDimes.myCAnnotationCHP, 2, myDimes.myCAnnotationLine.myPolyStartP,
                                                       YLPolyLArray3[t].GetPointAt(2), null, null, null, 0, null, null, null, null, 0, null, null, null, "",
                                          null, "", null, "", 0, null, "", null, "", null, "", 0, null, null, 0, null, null, null, 0,
                                          null, null, null, null, null, null, null, null, null, null, 0, null, null, null, 0, "")
                        break;
                    }
                    else if (IsHorizontal(YLPolyLArray3[t].GetPointAt(1), YLPolyLArray3[t].GetPointAt(0)) && IsNearPoint(myDimes.myCAnnotationLine.myPolyStartP, YLPolyLArray3[t].GetPointAt(1))) {
                        var Hlength = GetDisFromTwoPoint(myDimes.myCAnnotationLine.myPolyStartP, YLPolyLArray3[t].GetPointAt(0))
                        var mtline = new myPolyLine(YLPolyLArray3[t].handle, 3, YLPolyLArray3[t].GetPointAt(0),
                                               YLPolyLArray3[t].GetPointAt(2), YLPolyLArray3[t].GetPointAt(1), "Mid")
                        myTempWeldingArray = new myWelding(myDimes.myCAnnotationArrow, myDimes.myCAnnotationLine, mtline, Hlength,
                                                     myDimes.myCAnnotationCHSE, myDimes.myCAnnotationCHP, 2, myDimes.myCAnnotationLine.myPolyStartP,
                                                       YLPolyLArray3[t].GetPointAt(0), null, null, null, 0, null, null, null, null, 0, null, null, null, "",
                                          null, "", null, "", 0, null, "", null, "", null, "", 0, null, null, 0, null, null, null, 0,
                                          null, null, null, null, null, null, null, null, null, null, 0, null, null, null, 0, "")
                        break;

                    }
                }
            }
        }
    }
    else if (myDimes.myCAnnotationLine.myPolyPNum == 3) {
        if (myDimes.myCAnnotationCHSE == "Start") {
            if (IsHorizontal(myDimes.myCAnnotationLine.myPolyEndP, myDimes.myCAnnotationLine.myPolyMid)) {
                var Hlength = GetDisFromTwoPoint(myDimes.myCAnnotationLine.myPolyEndP, myDimes.myCAnnotationLine.myPolyMid)
                myTempWeldingArray = new myWelding(myDimes.myCAnnotationArrow, myDimes.myCAnnotationLine, null, Hlength,
                                              myDimes.myCAnnotationCHSE, myDimes.myCAnnotationCHP, 1, myDimes.myCAnnotationLine.myPolyMid,
                                              myDimes.myCAnnotationLine.myPolyEndP, null, null, null, 0, null, null, null, null, 0, null, null, null, "",
                                              null, "", null, "", 0, null, "", null, "", null, "", 0, null, null, 0, null, null, null, 0,
                                              null, null, null, null, null, null, null, null, null, null, 0, null, null, null, 0, "")
            }
        }
        else if (myDimes.myCAnnotationCHSE == "End") {
            if (IsHorizontal(myDimes.myCAnnotationLine.myPolyStartP, myDimes.myCAnnotationLine.myPolyMid)) {
                var Hlength = GetDisFromTwoPoint(myDimes.myCAnnotationLine.myPolyStartP, myDimes.myCAnnotationLine.myPolyMid)
                myTempWeldingArray = new myWelding(myDimes.myCAnnotationArrow, myDimes.myCAnnotationLine, null, Hlength,
                                              myDimes.myCAnnotationCHSE, myDimes.myCAnnotationCHP, 1, myDimes.myCAnnotationLine.myPolyMid,
                                              myDimes.myCAnnotationLine.myPolyStartP, null, null, null, 0, null, null, null, null, 0, null, null, null, "",
                                              null, "", null, "", 0, null, "", null, "", null, "", 0, null, null, 0, null, null, null, 0,
                                              null, null, null, null, null, null, null, null, null, null, 0, null, null, null, 0, "")
            }
        }
    }
     if (myTempWeldingArray == null) {
            return null;
     }

     var myGuaiP = myTempWeldingArray.myWelGUIP
     var myWeiP = myTempWeldingArray.myWelWEIP
     var miFHP= mxOcx.NewPoint();
     var maFHP= mxOcx.NewPoint();  
     var dw=10
     if(myWeiP.x>myGuaiP.x)
     {
         miFHP.x=myGuaiP.x-dw
         miFHP.y=myGuaiP.y-dw
         maFHP.x=myWeiP.x+dw
         maFHP.y=myWeiP.y+dw
     }
     else 
     {
         miFHP.x=myWeiP.x-dw
         miFHP.y=myWeiP.y-dw
         maFHP.x=myGuaiP.x+dw
         maFHP.y=myGuaiP.y+dw
     }
     var sFHL = mxOcx.NewSelectionSet();
     var spFHLFilte = mxOcx.NewResbuf();
     spFHLFilte.AddStringEx("LWPOLYLINE", 5020);
     sFHL.Select(0, miFHP, maFHP, spFHLFilte);
     var FHPolyLArray2 = []; //两顶点的polyline
     var FHPolyLArray3 = []; //三顶点的polyline
     var FHPolyLArray4_10 = [];//4-10顶点的polyline
     for (var j = 0; j < sFHL.Count; j++) {
         var ent = sFHL.Item(j);
         var mployLine = ent;
         if (mployLine.numVerts == 2) {
             FHPolyLArray2[FHPolyLArray2.length] = mployLine;
         }
         else if (mployLine.numVerts == 3) {
             FHPolyLArray3[FHPolyLArray3.length] = mployLine;
         }   
         else if (mployLine.numVerts >= 4 && mployLine.numVerts <= 10) {
             FHPolyLArray4_10[FHPolyLArray4_10.length] = mployLine;
         }       
     }
     var NS_PolylineArray2 = DeletSamePolyLine(FHPolyLArray2);
     var NS_PolylineArray3 = DeletSamePolyLine(FHPolyLArray3);
     var NS_PolylineArray4_10 = DeletSamePolyLine(FHPolyLArray4_10);
     var myWeldingData=GetIntersectLineArray(myTempWeldingArray, NS_PolylineArray2, NS_PolylineArray3, NS_PolylineArray4_10)
     var m_ResWeldArr = null;
     m_ResWeldArr=FirstDivideWelding(myWeldingData, NS_PolylineArray2, NS_PolylineArray3, NS_PolylineArray4_10)     
     return m_ResWeldArr;
}
//根据类型修改颜色
function ChangeColorByType(myWeldArray)
{
    var colID=0;
    //1-不开坡口对接焊   "N_PoKDuiJieH"
    //2 -单面-单侧-坡口-背面封底-对接焊缝    "Y_DMDCPoKDuiJieH"
    //3 单面-双侧-坡口-背面封底-对接焊缝----3(12)  "Y_DMSCPoKDuiJieH"
    //4 -双面-单侧-坡口-对接焊缝   "Y_SMDCPoKDuiJieH"
    //5 双面-双侧-坡口-对接焊缝----  "Y_SMSCPoKDuiJieH"
    //6 单面-单侧-坡口-角焊缝-----(15) "Y_DMDCPoKJiaoH"
    //7 单面-坡口-盖板-角焊缝-----(16)  "Y_DMPoKGaiBJiaoH"
    //8 双面-坡口-背面坡口-角焊缝-----(17) "Y_SMPoKBeiPJiaoH"
    //10  双面-坡口-熔透----  "Y_SMPoKRTH"
    //11 双面坡口盖板熔透,有图形(20) "Y_SMPoKGaiBRTH"
    //13 单面-角焊缝----13(22)    "Y_DMJiaoH"
    //14-双面角焊缝   "Y_SMJiaoH"
    if (myWeldArray.myWelType == "") {
        return;
    }
    else if(myWeldArray.myWelType=="N_PoKDuiJieH") //1
    {
        colID=1;
    }
    else if(myWeldArray.myWelType=="Y_DMDCPoKDuiJieH")//2
    {
         colID=30;
    }
    else if(myWeldArray.myWelType=="Y_DMSCPoKDuiJieH")//3
    {
        colID=3;
    }
    else if(myWeldArray.myWelType=="Y_SMDCPoKDuiJieH")//4
    {
        colID=4;
    }
    else if(myWeldArray.myWelType=="Y_SMSCPoKDuiJieH")//5
    {
        colID=150;
    }
    else if(myWeldArray.myWelType=="Y_DMDCPoKJiaoH")//6
    {
        colID=241;
    }
    else if(myWeldArray.myWelType=="Y_DMPoKGaiBJiaoH")//7
    {
        colID=6;
    }
    else if(myWeldArray.myWelType=="Y_SMPoKBeiPJiaoH")//8
    {
        colID=140;
    }
    else if(myWeldArray.myWelType=="Y_SMPoKRTH")//10
    {
        colID=51;
    }
    else if(myWeldArray.myWelType=="Y_SMPoKGaiBRTH")//11
    {
        colID=181;
    }
    else if(myWeldArray.myWelType=="Y_DMJiaoH")//13
    {
        colID=71;
    }
    else if(myWeldArray.myWelType=="Y_SMJiaoH")//14
    {
        colID=41;
    }
    ChangeWeldColorbyHandle(myWeldArray,colID)////红色
   
}
function GetDimensArrowLine(myArrowDatas,TwoP_Polyline, ThreeP_Polyline) {
    var mDimesdata=null;
        var sameNum = 0;
        var twoNum = TwoP_Polyline.length;
        var threeNum = ThreeP_Polyline.length;
        
        var tempPolyLineArray = [];
        for (var i = 0; i < twoNum; i++)
        {
            if (CheckLineType(TwoP_Polyline[i].GetPointAt(0), TwoP_Polyline[i].GetPointAt(1)) == "X")
            {
                if (IsNearPoint(TwoP_Polyline[i].GetPointAt(0), myArrowDatas.myArrowMid)) {
                    var mytempPL = new myPolyLine(TwoP_Polyline[i].handle, 2, TwoP_Polyline[i].GetPointAt(0), TwoP_Polyline[i].GetPointAt(1), null, "Start");
                    tempPolyLineArray[tempPolyLineArray.length] = mytempPL;
                    sameNum = sameNum + 1;
                }
                else if (IsNearPoint(TwoP_Polyline[i].GetPointAt(1), myArrowDatas.myArrowMid)) {
                    var mytempPL = new myPolyLine(TwoP_Polyline[i].handle, 2, TwoP_Polyline[i].GetPointAt(0), TwoP_Polyline[i].GetPointAt(1), null, "End");
                    tempPolyLineArray[tempPolyLineArray.length] = mytempPL;
                    sameNum = sameNum + 1;
                }
                //顶点
                if (IsNearPoint(TwoP_Polyline[i].GetPointAt(0), myArrowDatas.myArrowDingP) && PointToSegDist(myArrowDatas.myArrowMid.x, myArrowDatas.myArrowMid.y, TwoP_Polyline[i].GetPointAt(0).x, TwoP_Polyline[i].GetPointAt(0).y, TwoP_Polyline[i].GetPointAt(1).x, TwoP_Polyline[i].GetPointAt(1).y)<0.5) {
                    var mytempPL = new myPolyLine(TwoP_Polyline[i].handle, 2, TwoP_Polyline[i].GetPointAt(0), TwoP_Polyline[i].GetPointAt(1), null, "Start");
                    tempPolyLineArray[tempPolyLineArray.length] = mytempPL;
                    sameNum = sameNum + 1;
                }
                else if (IsNearPoint(TwoP_Polyline[i].GetPointAt(1), myArrowDatas.myArrowDingP && PointToSegDist(myArrowDatas.myArrowMid.x, myArrowDatas.myArrowMid.y, TwoP_Polyline[i].GetPointAt(0).x, TwoP_Polyline[i].GetPointAt(0).y, TwoP_Polyline[i].GetPointAt(1).x, TwoP_Polyline[i].GetPointAt(1).y) < 0.5)) {
                    var mytempPL = new myPolyLine(TwoP_Polyline[i].handle, 2, TwoP_Polyline[i].GetPointAt(0), TwoP_Polyline[i].GetPointAt(1), null, "End");
                    tempPolyLineArray[tempPolyLineArray.length] = mytempPL;
                    sameNum = sameNum + 1;
                }
            }            
        }
        for (var t = 0; t < threeNum; t++) {
            if (CheckLineType(ThreeP_Polyline[t].GetPointAt(0), ThreeP_Polyline[t].GetPointAt(1)) == "X" || CheckLineType(ThreeP_Polyline[t].GetPointAt(2), ThreeP_Polyline[t].GetPointAt(1)) == "X")
            {
                if (IsNearPoint(ThreeP_Polyline[t].GetPointAt(0), myArrowDatas.myArrowMid)) {
                    var mytempPL = new myPolyLine(ThreeP_Polyline[t].handle, 3, ThreeP_Polyline[t].GetPointAt(0), ThreeP_Polyline[t].GetPointAt(2), ThreeP_Polyline[t].GetPointAt(1), "Start");
                    tempPolyLineArray[tempPolyLineArray.length] = mytempPL;
                    sameNum = sameNum + 1;
                }
                else if (IsNearPoint(ThreeP_Polyline[t].GetPointAt(2), myArrowDatas.myArrowMid)) {
                    var mytempPL = new myPolyLine(ThreeP_Polyline[t].handle, 3, ThreeP_Polyline[t].GetPointAt(0), ThreeP_Polyline[t].GetPointAt(2), ThreeP_Polyline[t].GetPointAt(1), "End");
                    tempPolyLineArray[tempPolyLineArray.length] = mytempPL;
                    sameNum = sameNum + 1;
                }
                //顶点
                if (IsNearPoint(ThreeP_Polyline[t].GetPointAt(0), myArrowDatas.myArrowDingP) && PointToSegDist(myArrowDatas.myArrowMid.x, myArrowDatas.myArrowMid.y, ThreeP_Polyline[t].GetPointAt(0).x, ThreeP_Polyline[t].GetPointAt(0).y, ThreeP_Polyline[t].GetPointAt(1).x, ThreeP_Polyline[t].GetPointAt(1).y) < 0.5) {
                    var mytempPL = new myPolyLine(ThreeP_Polyline[t].handle, 3, ThreeP_Polyline[t].GetPointAt(0), ThreeP_Polyline[t].GetPointAt(2), ThreeP_Polyline[t].GetPointAt(1), "Start");
                    tempPolyLineArray[tempPolyLineArray.length] = mytempPL;
                    sameNum = sameNum + 1;
                }
                else if (IsNearPoint(ThreeP_Polyline[t].GetPointAt(2), myArrowDatas.myArrowDingP) && PointToSegDist(myArrowDatas.myArrowMid.x, myArrowDatas.myArrowMid.y, ThreeP_Polyline[t].GetPointAt(2).x, ThreeP_Polyline[t].GetPointAt(2).y, ThreeP_Polyline[t].GetPointAt(1).x, ThreeP_Polyline[t].GetPointAt(1).y) < 0.5) {
                    var mytempPL = new myPolyLine(ThreeP_Polyline[t].handle, 3, ThreeP_Polyline[t].GetPointAt(0), ThreeP_Polyline[t].GetPointAt(2), ThreeP_Polyline[t].GetPointAt(1), "End");
                    tempPolyLineArray[tempPolyLineArray.length] = mytempPL;
                    sameNum = sameNum + 1;
                }
            }          
        }
        if(sameNum==1)
        {
            
            if(tempPolyLineArray[0].myPolyCHSE=="Start")
            {
                mDimesdata= new myCAnnotation(myArrowDatas, tempPolyLineArray[0],
                tempPolyLineArray[0].myPolyCHSE, tempPolyLineArray[0].myPolyStartP)
            }
            else
            {
                mDimesdata = new myCAnnotation(myArrowDatas, tempPolyLineArray[0],
                 tempPolyLineArray[0].myPolyCHSE, tempPolyLineArray[0].myPolyEndP);
            }
        }
        
        else if (sameNum == 2)
        {
            if ((IsNearPoint(tempPolyLineArray[0].myPolyStartP, tempPolyLineArray[1].myPolyStartP) && IsNearPoint(tempPolyLineArray[0].myPolyEndP, tempPolyLineArray[1].myPolyEndP))
                || (IsNearPoint(tempPolyLineArray[0].myPolyStartP, tempPolyLineArray[1].myPolyEndP) && IsNearPoint(tempPolyLineArray[0].myPolyEndP, tempPolyLineArray[1].myPolyStartP)))
            {
                if (tempPolyLineArray[0].myPolyCHSE == "Start") {
                    mDimesdata = new myCAnnotation(myArrowDatas, tempPolyLineArray[0],
                    tempPolyLineArray[0].myPolyCHSE, tempPolyLineArray[0].myPolyStartP)
                }
                else {
                    mDimesdata = new myCAnnotation(myArrowDatas, tempPolyLineArray[0],
                     tempPolyLineArray[0].myPolyCHSE, tempPolyLineArray[0].myPolyEndP);
                }
            }
        }
        return mDimesdata;
    }

 function PointToSegDist(x,  y,  x1, y1,  x2,  y2)
        {
    var cross = (x2 - x1) * (x - x1) + (y2 - y1) * (y - y1);
    if (cross <= 0) 
    {
        return Math.sqrt(((x - x1) * (x - x1) + (y - y1) * (y - y1)), 2);
    }
        var d2 = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
        if (cross >= d2) 
        {
            return Math.sqrt(((x - x2) * (x - x2) + (y - y2) * (y - y2)), 2);
        }
  
        var r = cross / d2;
        var px = x1 + (x2 - x1) * r;
        var py = y1 + (y2 - y1) * r;
        return Math.sqrt(((x - px) * (x - px) + (py - y) * (py - y)),2);
}
//---4----
function GetIntersectLineArray(myWeldArray, TwoP_PolyLArray, ThreeP_PolyLArray, FourMP_PolyLArray) {

        var myGuaiP = myWeldArray.myWelGUIP
        var myWeiP = myWeldArray.myWelWEIP
             
        for (var twoi = 0; twoi < TwoP_PolyLArray.length; twoi++)
        {
            
            if (TwoP_PolyLArray[twoi].handle == myWeldArray.myWelYINline1.myPolyLineObjectID) {
                continue;
            }
            else if (myWeldArray.myWelYINnum == 2 && TwoP_PolyLArray[twoi].handle == myWeldArray.myWelYINline2.myPolyLineObjectID)
            {
                continue;
            }
            
            else if (CheckLineInHLine(TwoP_PolyLArray[twoi].GetPointAt(0), TwoP_PolyLArray[twoi].GetPointAt(1), myGuaiP, myWeiP))
            {
                TwoP_PolyLArray.splice(twoi, 1)
                twoi = twoi - 1;
                continue
            }
            
            
            var mtempLine = new myPolyLine(TwoP_PolyLArray[twoi].handle, 2, TwoP_PolyLArray[twoi].GetPointAt(0),
                                         TwoP_PolyLArray[twoi].GetPointAt(1), null, "")

            
            if (IsNearPoint(myWeiP, TwoP_PolyLArray[twoi].GetPointAt(0)))
            {
                if (myWeldArray.myWelWH_num == 0)
                {
                   
                    myWeldArray.myWelWHline_1 = mtempLine
                    myWeldArray.myWelWHline_1.myPolyCHSE = "Start"
                    myWeldArray.myWelWH_num = 1;
                    myWeldArray.myWelWHPoint_1 = mtempLine.myPolyStartP
                }
                else if (myWeldArray.myWelWH_num == 1)
                {
                    myWeldArray.myWelWHline_2 = mtempLine
                    myWeldArray.myWelWHline_2.myPolyCHSE = "Start"
                    myWeldArray.myWelWH_num = 2;
                    myWeldArray.myWelWHPoint_2 = mtempLine.myPolyStartP
                }
            }
            else if (IsNearPoint(myWeiP, TwoP_PolyLArray[twoi].GetPointAt(1))) {
                if (myWeldArray.myWelWH_num == 0) {
                    myWeldArray.myWelWHline_1 = mtempLine
                    myWeldArray.myWelWHline_1.myPolyCHSE = "End"
                    myWeldArray.myWelWH_num = 1;
                    myWeldArray.myWelWHPoint_1 = mtempLine.myPolyEndP
                }
                else if (myWeldArray.myWelWH_num == 1) {
                    myWeldArray.myWelWHline_2 = mtempLine
                    myWeldArray.myWelWHline_2.myPolyCHSE = "End"
                    myWeldArray.myWelWH_num = 2;
                    myWeldArray.myWelWHPoint_2 = mtempLine.myPolyEndP
                }
            }
            
            if (IsPointOnLine(TwoP_PolyLArray[twoi].GetPointAt(0), myGuaiP, myWeiP)) {
                
                var tempDisStr=GetPositionOfLine(TwoP_PolyLArray[twoi], myGuaiP, myWeiP)
                if (tempDisStr == "up")
                {
                    if (myWeldArray.myWelMS_num == 0) {
                        myWeldArray.myWelMSline_1 = mtempLine
                        myWeldArray.myWelMSStorEdorMd_1 = "Start"
                        myWeldArray.myWelMS_num=1
                    }
                    else if (myWeldArray.myWelMS_num == 1) {
                        myWeldArray.myWelMSline_2 = mtempLine
                        myWeldArray.myWelMSStorEdorMd_2 = "Start"
                        myWeldArray.myWelMS_num = 2
                    }
                    else if (myWeldArray.myWelMS_num == 2) {
                        myWeldArray.myWelMSline_3 = mtempLine
                        myWeldArray.myWelMSStorEdorMd_3 = "Start"
                        myWeldArray.myWelMS_num = 3
                    }
                }
                else if (tempDisStr == "down")
                {
                    if (myWeldArray.myWelMX_num == 0) {
                        myWeldArray.myWelMXline_1 = mtempLine
                        myWeldArray.myWelMXStorEdorMd_1 = "Start"
                        myWeldArray.myWelMX_num = 1
                    }
                    else if (myWeldArray.myWelMX_num == 1) {
                        myWeldArray.myWelMXline_2 = mtempLine
                        myWeldArray.myWelMXStorEdorMd_2 = "Start"
                        myWeldArray.myWelMX_num = 2
                    }
                    else if (myWeldArray.myWelMX_num == 2) {
                        myWeldArray.myWelMXline_3 = mtempLine
                        myWeldArray.myWelMXStorEdorMd_3 = "Start"
                        myWeldArray.myWelMX_num = 3
                    }
                }
                else if (tempDisStr == "mid")
                {
                    if (myWeldArray.myWelMid_num == 0) {
                        myWeldArray.myWelMidline_1 = mtempLine                        
                        myWeldArray.myWelMid_num = 1
                    }
                    else if (myWeldArray.myWelMid_num == 1) {
                        myWeldArray.myWelMidline_2 = mtempLine
                        myWeldArray.myWelMid_num = 2
                    }
                }
            }
            
            else if (IsPointOnLine(TwoP_PolyLArray[twoi].GetPointAt(1), myGuaiP, myWeiP))
            {
                var tempDisStr = GetPositionOfLine(TwoP_PolyLArray[twoi], myGuaiP, myWeiP)
                if (tempDisStr == "up") {
                    if (myWeldArray.myWelMS_num == 0) {
                        myWeldArray.myWelMSline_1 = mtempLine
                        myWeldArray.myWelMSStorEdorMd_1 = "End"
                        myWeldArray.myWelMS_num = 1
                    }
                    else if (myWeldArray.myWelMS_num == 1) {
                        myWeldArray.myWelMSline_2 = mtempLine
                        myWeldArray.myWelMSStorEdorMd_2 = "End"
                        myWeldArray.myWelMS_num = 2
                    }
                    else if (myWeldArray.myWelMS_num == 2) {
                        myWeldArray.myWelMSline_3 = mtempLine
                        myWeldArray.myWelMSStorEdorMd_3 = "End"
                        myWeldArray.myWelMS_num = 3
                    }
                }
                else if (tempDisStr == "down") {
                    if (myWeldArray.myWelMX_num == 0) {
                        myWeldArray.myWelMXline_1 = mtempLine
                        myWeldArray.myWelMXStorEdorMd_1 = "End"
                        myWeldArray.myWelMX_num = 1
                    }
                    else if (myWeldArray.myWelMX_num == 1) {
                        myWeldArray.myWelMXline_2 = mtempLine
                        myWeldArray.myWelMXStorEdorMd_2 = "End"
                        myWeldArray.myWelMX_num = 2
                    }
                    else if (myWeldArray.myWelMX_num == 2) {
                        myWeldArray.myWelMXline_3 = mtempLine
                        myWeldArray.myWelMXStorEdorMd_3 = "End"
                        myWeldArray.myWelMX_num = 3
                    }
                }
                else if (tempDisStr == "mid") {
                    if (myWeldArray.myWelMid_num == 0) {
                        myWeldArray.myWelMidline_1 = mtempLine
                        myWeldArray.myWelMid_num = 1
                    }
                    else if (myWeldArray.myWelMid_num == 1) {
                        myWeldArray.myWelMidline_2 = mtempLine
                        myWeldArray.myWelMid_num = 2
                    }
                }
            }
            
            else if (IsLineInterWithHYinLine(TwoP_PolyLArray[twoi].GetPointAt(0), TwoP_PolyLArray[twoi].GetPointAt(1), myGuaiP, myWeiP))
            {
                var str =GetPositionOfLine(TwoP_PolyLArray[twoi], myGuaiP, myWeiP)
                if(str=="mid")
                {
                    if (myWeldArray.myWelMid_num == 0) {
                        myWeldArray.myWelMidline_1 = mtempLine
                        myWeldArray.myWelMid_num = 1
                    }
                    else if (myWeldArray.myWelMid_num == 1) {
                        myWeldArray.myWelMidline_2 = mtempLine
                        myWeldArray.myWelMid_num = 2
                    }
                }
            }
        }
        
        for (var threei = 0; threei < ThreeP_PolyLArray.length; threei++)
        {
            if (ThreeP_PolyLArray[threei].handle == myWeldArray.myWelYINline1.myPolyLineObjectID)
            {
                continue;
            }
            else if (myWeldArray.myWelYINnum == 2 && ThreeP_PolyLArray[threei].handle == myWeldArray.myWelYINline2.myPolyLineObjectID)
            {
                continue;
            }
            
            var mtempline3 = new myPolyLine(ThreeP_PolyLArray[threei].handle, 3, ThreeP_PolyLArray[threei].GetPointAt(0),
                                          ThreeP_PolyLArray[threei].GetPointAt(2), ThreeP_PolyLArray[threei].GetPointAt(1), "")

           
            if (!IsNearPoint(ThreeP_PolyLArray[threei].GetPointAt(1), myGuaiP))
            {
                if (IsNearPoint(myWeiP, ThreeP_PolyLArray[threei].GetPointAt(0)))
                {
                    if (myWeldArray.myWelWH_num==0)
                    {
                        myWeldArray.myWelWHline_1 = mtempline3
                        myWeldArray.myWelWHline_1.myPolyCHSE = "Start"
                        myWeldArray.myWelWH_num = 1;
                        myWeldArray.myWelWHPoint_1 = ThreeP_PolyLArray[threei].GetPointAt(0)
                    }
                    else if (myWeldArray.myWelWH_num == 1)
                    {
                        myWeldArray.myWelWHline_2 = mtempline3
                        myWeldArray.myWelWHline_2.myPolyCHSE = "Start"
                        myWeldArray.myWelWH_num = 2;
                        myWeldArray.myWelWHPoint_2 = ThreeP_PolyLArray[threei].GetPointAt(0)
                    }
                }
                else if (IsNearPoint(myWeiP, ThreeP_PolyLArray[threei].GetPointAt(2))) {
                    if (myWeldArray.myWelWH_num == 0) {
                        myWeldArray.myWelWHline_1 = mtempline3
                        myWeldArray.myWelWHline_1.myPolyCHSE = "End"
                        myWeldArray.myWelWH_num = 1;
                        myWeldArraymyWelWHPoint_1 = ThreeP_PolyLArray[threei].GetPointAt(2)
                    }
                    else if (myWeldArray.myWelWH_num == 1) {
                        myWeldArray.myWelWHline_2 = mtempline3
                        myWeldArray.myWelWHline_2.myPolyCHSE = "End"
                        myWeldArray.myWelWH_num = 2;
                        myWeldArray.myWelWHPoint_2 = ThreeP_PolyLArray[threei].GetPointAt(2)
                    }
                }
                    
                else {
                    if (IsNearPoint(myWeiP, ThreeP_PolyLArray[threei].GetPointAt(1))) {
                        if (myWeldArray.myWelWH_num == 0)
                        {
                            myWeldArray.myWelWHline_1 = mtempline3
                            myWeldArray.myWelWHline_1.myPolyCHSE = "Mid"
                            myWeldArray.myWelWH_num = 1;
                            myWeldArray.myWelWHPoint_1 = ThreeP_PolyLArray[threei].GetPointAt(1)
                        }
                        else if (myWeldArray.myWelWH_num == 1)
                        {
                            myWeldArray.myWelWHline_2 = mtempline3
                            myWeldArray.myWelWHline_2.myPolyCHSE = "Mid"
                            myWeldArray.myWelWH_num = 2;
                            myWeldArray.myWelWHPoint_2 = ThreeP_PolyLArray[threei].GetPointAt(1)
                        }
                    }
                }
            }
            
            if (IsPointOnLine(ThreeP_PolyLArray[threei].GetPointAt(0), myGuaiP, myWeiP)) {
               
                var tempDisStr=GetPositionOfLine(ThreeP_PolyLArray[threei], myGuaiP, myWeiP)
                if (tempDisStr == "up")
                {
                    if (myWeldArray.myWelMS_num == 0) {
                        myWeldArray.myWelMSline_1 = mtempline3
                        myWeldArray.myWelMSStorEdorMd_1 = "Start"
                        myWeldArray.myWelMS_num = 1
                    }
                    else if (myWeldArray.myWelMS_num == 1) {
                        myWeldArray.myWelMSline_2 = mtempline3
                        myWeldArray.myWelMSStorEdorMd_2 = "Start"
                        myWeldArray.myWelMS_num = 2
                    }
                    else if (myWeldArray.myWelMS_num == 2) {
                        myWeldArray.myWelMSline_3 = mtempline3
                        myWeldArray.myWelMSStorEdorMd_3 = "Start"
                        myWeldArray.myWelMS_num = 3
                    }
                }
                else if (tempDisStr == "down") {
                    if (myWeldArray.myWelMX_num == 0) {
                        myWeldArray.myWelMXline_1 = mtempline3
                        myWeldArray.myWelMXStorEdorMd_1 = "Start"
                        myWeldArray.myWelMX_num = 1
                    }
                    else if (myWeldArray.myWelMX_num == 1) {
                        myWeldArray.myWelMXline_2 = mtempline3
                        myWeldArray.myWelMXStorEdorMd_2 = "Start"
                        myWeldArray.myWelMX_num = 2
                    }
                    else if (myWeldArray.myWelMX_num == 2) {
                        myWeldArray.myWelMXline_3 = mtempline3
                        myWeldArray.myWelMXStorEdorMd_3 = "Start"
                        myWeldArray.myWelMX_num = 3
                    }
                }
                else if (tempDisStr == "mid")
                {
                    if (myWeldArray.myWelMid_num == 0) {
                        myWeldArray.myWelMidline_1 = mtempline3
                        myWeldArray.myWelMid_num = 1
                    }
                    else if (myWeldArray.myWelMid_num == 1) {
                        myWeldArray.myWelMidline_2 = mtempline3
                        myWeldArray.myWelMid_num = 2
                    }
                }
            }
           
            else if (IsPointOnLine(ThreeP_PolyLArray[threei].GetPointAt(2), myGuaiP, myWeiP)) {
                
                var tempDisStr = GetPositionOfLine(ThreeP_PolyLArray[threei], myGuaiP, myWeiP)
                if (tempDisStr == "up") {
                    if (myWeldArray.myWelMS_num == 0) {
                        myWeldArray.myWelMSline_1 = mtempline3
                        myWeldArray.myWelMSStorEdorMd_1 = "End"
                        myWeldArray.myWelMS_num = 1
                    }
                    else if (myWeldArray.myWelMS_num == 1) {
                        myWeldArray.myWelMSline_2 = mtempline3
                        myWeldArray.myWelMSStorEdorMd_2 = "End"
                        myWeldArray.myWelMS_num = 2
                    }
                    else if (myWeldArray.myWelMS_num == 2) {
                        myWeldArray.myWelMSline_3 = mtempline3
                        myWeldArray.myWelMSStorEdorMd_3 = "End"
                        myWeldArray.myWelMS_num = 3
                    }
                }
                else if (tempDisStr == "down") {
                    if (myWeldArray.myWelMX_num == 0) {
                        myWeldArray.myWelMXline_1 = mtempline3
                        myWeldArray.myWelMXStorEdorMd_1 = "End"
                        myWeldArray.myWelMX_num = 1
                    }
                    else if (myWeldArray.myWelMX_num == 1) {
                        myWeldArray.myWelMXline_2 = mtempline3
                        myWeldArray.myWelMXStorEdorMd_2 = "End"
                        myWeldArray.myWelMX_num = 2
                    }
                    else if (myWeldArray.myWelMX_num == 2) {
                        myWeldArray.myWelMXline_3 = mtempline3
                        myWeldArray.myWelMXStorEdorMd_3 = "End"
                        myWeldArray.myWelMX_num = 3
                    }
                }
                else if (tempDisStr == "mid") {
                    if (myWeldArray.myWelMid_num == 0) {
                        myWeldArray.myWelMidline_1 = mtempline3
                        myWeldArray.myWelMid_num = 1
                    }
                    else if (myWeldArray.myWelMid_num == 1) {
                        myWeldArray.myWelMidline_2 = mtempline3
                        myWeldArray.myWelMid_num = 2
                    }
                }
            }
            else {
                
                if (IsPointOnLine(ThreeP_PolyLArray[threei].GetPointAt(1), myGuaiP, myWeiP))
                {
                    var tempDisStr=GetPositionOfLine(ThreeP_PolyLArray[threei], myGuaiP, myWeiP) 
                    if (tempDisStr == "up") {
                        if (myWeldArray.myWelMS_num == 0)
                        {
                            myWeldArray.myWelMSline_1 = mtempline3
                            myWeldArray.myWelMSStorEdorMd_1 = "Mid"
                            myWeldArray.myWelMS_num = 1
                        }
                        else if (myWeldArray.myWelMS_num == 1)
                        {
                            myWeldArray.myWelMSline_2 = mtempline3
                            myWeldArray.myWelMSStorEdorMd_2 = "Mid"
                            myWeldArray.myWelMS_num = 2
                        }
                        else if (myWeldArray.myWelMS_num == 2)
                        {
                            myWeldArray.myWelMSline_3 = mtempline3
                            myWeldArray.myWelMSStorEdorMd_3 = "Mid"
                            myWeldArray.myWelMS_num = 3
                        }
                    }
                    else if (tempDisStr == "down")
                    {
                        if (myWeldArray.myWelMX_num == 0) {
                            myWeldArray.myWelMXline_1 = mtempline3
                            myWeldArray.myWelMXStorEdorMd_1 = "Mid"
                            myWeldArray.myWelMX_num = 1
                        }
                        else if (myWeldArray.myWelMX_num == 1) {
                            myWeldArray.myWelMXline_2 = mtempline3
                            myWeldArray.myWelMXStorEdorMd_2 = "Mid"
                            myWeldArray.myWelMX_num = 2
                        }
                        else if (myWeldArray.myWelMX_num == 2) {
                            myWeldArray.myWelMXline_3 = mtempline3
                            myWeldArray.myWelMXStorEdorMd_3 = "Mid"
                            myWeldArray.myWelMX_num = 3
                        }
                    }
                    else if (tempDisStr == "mid")
                    {
                        if (myWeldArray.myWelMid_num == 0) {
                            myWeldArray.myWelMidline_1 = mtempline3
                            myWeldArray.myWelMid_num = 1
                        }
                        else if (myWeldArray.myWelMid_num == 1) {
                            myWeldArray.myWelMidline_2 = mtempline3
                            myWeldArray.myWelMid_num = 2
                        }
                    }
                }
            }
        }
       
        for (var fouri = 0; fouri < FourMP_PolyLArray.length; fouri++) {
            
            if (FourMP_PolyLArray[fouri].handle == myWeldArray.myWelYINline1.myPolyLineObjectID)
            {
                continue;
            }
            else if (myWeldArray.myWelYINnum == 2 && FourMP_PolyLArray[fouri].handle == myWeldArray.myWelYINline2.myPolyLineObjectID)
            {
                continue;
            }
           
            var mtempline4 = new myPolyLine(FourMP_PolyLArray[fouri].handle, FourMP_PolyLArray[fouri].numVerts, FourMP_PolyLArray[fouri].GetPointAt(0), FourMP_PolyLArray[fouri].GetPointAt(FourMP_PolyLArray[fouri].numVerts - 1), null, "")
            
            if (IsPointOnLine(FourMP_PolyLArray[fouri].GetPointAt(0), myGuaiP, myWeiP)) {
               
                var tempDisStr = GetPositionOfLine(FourMP_PolyLArray[fouri], myGuaiP, myWeiP)
                if (tempDisStr == "up")
                {
                    if (myWeldArray.myWelMS_num == 0) {
                        myWeldArray.myWelMSline_1 = mtempline4
                        myWeldArray.myWelMSStorEdorMd_1 = "Start"
                        myWeldArray.myWelMS_num = 1
                    }
                    else if (myWeldArray.myWelMS_num == 1)
                    {
                        myWeldArray.myWelMSline_2 = mtempline4
                        myWeldArray.myWelMSStorEdorMd_2 = "Start"
                        myWeldArray.myWelMS_num = 2
                    }
                    else if (myWeldArray.myWelMS_num == 2) {
                        myWeldArray.myWelMSline_3 = mtempline4
                        myWeldArray.myWelMSStorEdorMd_3 = "Start"
                        myWeldArray.myWelMS_num = 3
                    }
                }
                else if (tempDisStr == "down")
                {
                    if (myWeldArray.myWelMX_num == 0) {
                        myWeldArray.myWelMXline_1 = mtempline4
                        myWeldArray.myWelMXStorEdorMd_1 = "Start"
                        myWeldArray.myWelMX_num = 1
                    }
                    else if (myWeldArray.myWelMX_num == 1) {
                        myWeldArray.myWelMXline_2 = mtempline4
                        myWeldArray.myWelMXStorEdorMd_2 = "Start"
                        myWeldArray.myWelMX_num = 2
                    }
                    else if (myWeldArray.myWelMX_num == 2) {
                        myWeldArray.myWelMXline_3 = mtempline4
                        myWeldArray.myWelMXStorEdorMd_3 = "Start"
                        myWeldArray.myWelMX_num = 3
                    }
                }
                else if (tempDisStr == "mid") {
                    if (myWeldArray.myWelMid_num == 0) {
                        myWeldArray.myWelMidline_1 = mtempline4
                        myWeldArray.myWelMid_num = 1
                    }
                    else if (myWeldArray.myWelMid_num == 1) {
                        myWeldArray.myWelMidline_2 = mtempline4
                        myWeldArray.myWelMid_num = 2
                    }
                }
            }
           
            else if (IsPointOnLine(FourMP_PolyLArray[fouri].GetPointAt(FourMP_PolyLArray[fouri].numVerts-1), myGuaiP, myWeiP))
            {
                var tempDisStr = GetPositionOfLine(FourMP_PolyLArray[fouri], myGuaiP, myWeiP)
                if (tempDisStr == "up") {
                    if (myWeldArray.myWelMS_num == 0) {
                        myWeldArray.myWelMSline_1 = mtempline4
                        myWeldArray.myWelMSStorEdorMd_1 = "End"
                        myWeldArray.myWelMS_num = 1
                    }
                    else if (myWeldArray.myWelMS_num == 1) {
                        myWeldArray.myWelMSline_2 = mtempline4
                        myWeldArray.myWelMSStorEdorMd_2 = "End"
                        myWeldArray.myWelMS_num = 2
                    }
                    else if (myWeldArray.myWelMS_num == 2) {
                        myWeldArray.myWelMSline_3 = mtempline4
                        myWeldArray.myWelMSStorEdorMd_3 = "End"
                        myWeldArray.myWelMS_num = 3
                    }
                }
                else if (tempDisStr == "down") {
                    if (myWeldArray.myWelMX_num == 0) {
                        myWeldArray.myWelMXline_1 = mtempline4
                        myWeldArray.myWelMXStorEdorMd_1 = "End"
                        myWeldArray.myWelMX_num = 1
                    }
                    else if (myWeldArray.myWelMX_num == 1) {
                        myWeldArray.myWelMXline_2 = mtempline4
                        myWeldArray.myWelMXStorEdorMd_2 = "End"
                        myWeldArray.myWelMX_num = 2
                    }
                    else if (myWeldArray.myWelMX_num == 2) {
                        myWeldArray.myWelMXline_3 = mtempline4
                        myWeldArray.myWelMXStorEdorMd_3 = "End"
                        myWeldArray.myWelMX_num = 3
                    }
                }
                else if (tempDisStr == "mid") {
                    if (myWeldArray.myWelMid_num == 0) {
                        myWeldArray.myWelMidline_1 = mtempline4
                        myWeldArray.myWelMid_num = 1
                    }
                    else if (myWeldArray.myWelMid_num == 1) {
                        myWeldArray.myWelMidline_2 = mtempline4
                        myWeldArray.myWelMid_num = 2
                    }
                }
            }
        }
    return myWeldArray
}
//---5------
function FirstDivideWelding(myWeldArray, TwoP_PolyLArray, ThreeP_PolyLArray, FourMP_PolyLArray) {

        var nG = myWeldArray.myWelGH_num;
        var nW = myWeldArray.myWelWH_num;
        var nS = myWeldArray.myWelMS_num;
        var nX = myWeldArray.myWelMX_num;
        var nM = myWeldArray.myWelMid_num;
        if (nG == 0 && nW == 0 && nS == 1 && nX == 0 && nM == 1) {

            if (myWeldArray.myWelMSline_1.myPolyPNum == 3 && myWeldArray.myWelMidline_1.myPolyPNum == 3) {
                if (CheckAngleIn(myWeldArray.myWelMSline_1.myPolyMid, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, 47, 43)
                    && CheckAngleIn(myWeldArray.myWelMidline_1.myPolyMid, myWeldArray.myWelMidline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyEndP, 47, 43)) {

                    myWeldArray.myWelType = "Y_SMJiaoH"
                }
            }

            if (myWeldArray.myWelMSline_1.myPolyPNum == 2 && myWeldArray.myWelMidline_1.myPolyPNum == 3) {
                if (CheckLineType(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP) == "X"
                    && CheckAngleIn(myWeldArray.myWelMidline_1.myPolyMid, myWeldArray.myWelMidline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyEndP, 47, 38)) {
                    myWeldArray.myWelType = "Y_SMJiaoH"
                }
            }
        }
        else if (nG == 0 && nW == 0 && nS == 0 && nX == 0 && nM == 2) {
            if (myWeldArray.myWelMidline_1.myPolyPNum == 3 && myWeldArray.myWelMidline_2.myPolyPNum == 3) {
                if (CheckAngleIn(myWeldArray.myWelMidline_1.myPolyMid, myWeldArray.myWelMidline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyEndP, 110, 104)
                    && CheckAngleIn(myWeldArray.myWelMidline_2.myPolyMid, myWeldArray.myWelMidline_2.myPolyStartP, myWeldArray.myWelMidline_2.myPolyEndP, 110, 104)) {
                    myWeldArray.myWelType = "Y_SMSCPoKDuiJieH"
                }
            }
            else if (myWeldArray.myWelMidline_1.myPolyPNum == 2 && myWeldArray.myWelMidline_2.myPolyPNum == 3) {
                if (CheckAngleIn(myWeldArray.myWelMidline_2.myPolyMid, myWeldArray.myWelMidline_2.myPolyStartP, myWeldArray.myWelMidline_2.myPolyEndP, 83, 79)) {

                    myWeldArray.myWelType = "Y_SMDCPoKDuiJieH"
                }
                if (CheckAngleIn(myWeldArray.myWelMidline_2.myPolyMid, myWeldArray.myWelMidline_2.myPolyStartP, myWeldArray.myWelMidline_2.myPolyEndP, 92, 88)) {
                    myWeldArray.myWelType = "Y_SMJiaoH"
                }
            }
            else if (myWeldArray.myWelMidline_1.myPolyPNum == 3 && myWeldArray.myWelMidline_2.myPolyPNum == 2) {
                if (CheckAngleIn(myWeldArray.myWelMidline_1.myPolyMid, myWeldArray.myWelMidline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyEndP, 83, 79)) {
                    myWeldArray.myWelType = "Y_SMDCPoKDuiJieH"
                }
            }
        }
        else if (nG == 0 && nW == 1 && nS == 0 && nX == 0 && nM == 2) {
            if (myWeldArray.myWelWHline_1.myPolyPNum == 3 && myWeldArray.myWelMidline_1.myPolyPNum == 3 && myWeldArray.myWelMidline_2.myPolyPNum == 3) {

                if (CheckAngleIn(myWeldArray.myWelWHline_1.myPolyMid, myWeldArray.myWelWHline_1.myPolyStartP, myWeldArray.myWelWHline_1.myPolyEndP, 92, 88)
                    && CheckAngleIn(myWeldArray.myWelMidline_1.myPolyMid, myWeldArray.myWelMidline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyEndP, 138, 132)
                    && CheckAngleIn(myWeldArray.myWelMidline_2.myPolyMid, myWeldArray.myWelMidline_2.myPolyStartP, myWeldArray.myWelMidline_2.myPolyEndP, 138, 132)) {
                    myWeldArray.myWelType = "Y_SMPoKRTH" 
                }
            }
            else if (myWeldArray.myWelWHline_1.myPolyPNum == 3 && myWeldArray.myWelMidline_1.myPolyPNum == 2 && myWeldArray.myWelMidline_2.myPolyPNum == 3) {
                if (CheckAngleIn(myWeldArray.myWelWHline_1.myPolyMid, myWeldArray.myWelWHline_1.myPolyStartP, myWeldArray.myWelWHline_1.myPolyEndP, 92, 88)
                   && CheckAngleIn(myWeldArray.myWelMidline_2.myPolyMid, myWeldArray.myWelMidline_2.myPolyStartP, myWeldArray.myWelMidline_2.myPolyEndP, 100, 92)
                    && IsVertical(myWeldArray.myWelMidline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyEndP)) {
                    myWeldArray.myWelType = "Y_SMPoKRTH" 
                }
            }
            else if (myWeldArray.myWelWHline_1.myPolyPNum == 3 && myWeldArray.myWelMidline_1.myPolyPNum == 3 && myWeldArray.myWelMidline_2.myPolyPNum == 2) {
                if (CheckAngleIn(myWeldArray.myWelWHline_1.myPolyMid, myWeldArray.myWelWHline_1.myPolyStartP, myWeldArray.myWelWHline_1.myPolyEndP, 92, 88)
                   && CheckAngleIn(myWeldArray.myWelMidline_1.myPolyMid, myWeldArray.myWelMidline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyEndP, 100, 92)
                    && IsVertical(myWeldArray.myWelMidline_2.myPolyStartP, myWeldArray.myWelMidline_2.myPolyEndP)) {
                    myWeldArray.myWelType = "Y_SMPoKRTH" 
                }
            }

        }
        else if (nG == 0 && nW == 1 && nS == 1 && nX == 1 && nM == 0) {
            if (myWeldArray.myWelMSline_1.myPolyPNum == 3 && myWeldArray.myWelMSStorEdorMd_1 == "Mid"
                && myWeldArray.myWelMXline_1.myPolyPNum == 3 && myWeldArray.myWelMXStorEdorMd_1 == "Mid"
                && myWeldArray.myWelWHline_1.myPolyPNum == 3) {
                if (CheckAngleIn(myWeldArray.myWelWHline_1.myPolyMid, myWeldArray.myWelWHline_1.myPolyStartP, myWeldArray.myWelWHline_1.myPolyEndP, 92, 78)
                    && CheckAngleIn(myWeldArray.myWelMSline_1.myPolyMid, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, 51, 43)
                    && CheckAngleIn(myWeldArray.myWelMXline_1.myPolyMid, myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP, 51, 43)) {
                    var dis;
                    dis = GetDisFromTwoPoint(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP);
                    var res_PolyLineArr = [];
                    res_PolyLineArr = GetGraphFromArea(myWeldArray.myWelMSline_1.myPolyStartP.x, myWeldArray.myWelMSline_1.myPolyEndP.x, myWeldArray.myWelMSline_1.myPolyStartP.y, dis, "up", TwoP_PolyLArray, ThreeP_PolyLArray)
                    if (res_PolyLineArr.length == 2) {
                        myWeldArray.myWelUP_num = 2;
                        myWeldArray.myWelUPline_1 = res_PolyLineArr[0];
                        myWeldArray.myWelUPline_2 = res_PolyLineArr[1];
                    }
                    else if (res_PolyLineArr.length == 1) {
                        myWeldArray.myWelUP_num = 1;
                        myWeldArray.myWelUPline_1 = res_PolyLineArr[0];
                    }
                    var res_XPolyLineArr = [];
                    res_XPolyLineArr = GetGraphFromArea(myWeldArray.myWelMXline_1.myPolyStartP.x, myWeldArray.myWelMXline_1.myPolyEndP.x, myWeldArray.myWelMXline_1.myPolyStartP.y, dis, "down", TwoP_PolyLArray, ThreeP_PolyLArray)
                    if (res_XPolyLineArr.length == 2) {
                        myWeldArray.myWelDOWN_num = 2;
                        myWeldArray.myWelDOWNline_1 = res_XPolyLineArr[0];
                        myWeldArray.myWelDOWNline_2 = res_XPolyLineArr[1];
                    }
                    else if (res_XPolyLineArr.length == 1) {
                        myWeldArray.myWelDOWN_num = 1;
                        myWeldArray.myWelDOWNline_1 = res_XPolyLineArr[0];
                    }
                    if (myWeldArray.myWelUP_num == 0 && myWeldArray.myWelDOWN_num == 0) {
                        myWeldArray.myWelType = "Y_SMPoKRTH" 
                    }
                    else if (myWeldArray.myWelUP_num != 0 && myWeldArray.myWelDOWN_num != 0) {
                        myWeldArray.myWelType = "Y_SMPoKGaiBRTH" 
                    }
                }
            }
            else if (myWeldArray.myWelMSline_1.myPolyPNum == 2 && myWeldArray.myWelMSStorEdorMd_1 != "Mid"
                && myWeldArray.myWelMXline_1.myPolyPNum == 2 && myWeldArray.myWelMXStorEdorMd_1 != "Mid"
                && myWeldArray.myWelWHline_1.myPolyPNum == 3) {
                if (CheckAngleIn(myWeldArray.myWelWHline_1.myPolyMid, myWeldArray.myWelWHline_1.myPolyStartP, myWeldArray.myWelWHline_1.myPolyEndP, 92, 88)) {
                    if (IsVertical(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP)
                        && IsVertical(myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP)) {
                        var mUpChaArr = [];
                        var mDownChaArr = [];
                        mUpChaArr = GetInterLineFromVerLine(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, TwoP_PolyLArray);
                        mDownChaArr = GetInterLineFromVerLine(myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP, TwoP_PolyLArray);
                        if (mUpChaArr.length == 1 && mDownChaArr.length == 1) {
                            myWeldArray.myWelMSline_2 = mUpChaArr[0];
                            myWeldArray.myWelMS_num = 2;
                            myWeldArray.myWelMXline_2 = mDownChaArr[0];
                            myWeldArray.myWelMX_num = 2;
                            var dis;
                            dis = GetDisFromTwoPoint(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP)
                            var mtempSP, mtemEP;
                            if (myWeldArray.myWelMSStorEdorMd_1 == "Start") {
                                mtempSP = myWeldArray.myWelMSline_1.myPolyEndP;
                            }
                            else {
                                mtempSP = myWeldArray.myWelMSline_1.myPolyStartP;
                            }
                            if (myWeldArray.myWelMSline_2.myPolyCHSE == "Start") {
                                mtemEP = myWeldArray.myWelMSline_2.myPolyEndP;
                            }
                            else {
                                mtemEP = myWeldArray.myWelMSline_2.myPolyStartP;
                            }
                            var upGrapLine = [];
                            upGrapLine = GetGraphFromArea(mtempSP.x, mtemEP.x, mtempSP.y, dis, "up", TwoP_PolyLArray, ThreeP_PolyLArray)
                            if (upGrapLine.length == 2) {
                                myWeldArray.myWelUP_num = 2;
                                myWeldArray.myWelUPline_1 = upGrapLine[0];
                                myWeldArray.myWelUPline_2 = upGrapLine[1];
                            }
                            else if (upGrapLine.length == 1) {
                                myWeldArray.myWelUP_num = 1;
                                myWeldArray.myWelUPline_1 = upGrapLine[0];
                            }
                            if (myWeldArray.myWelMXStorEdorMd_1 == "Start") {
                                mtempSP = myWeldArray.myWelMXline_1.myPolyEndP;
                            }
                            else {
                                mtempSP = myWeldArray.myWelMXline_1.myPolyStartP;
                            }
                            if (myWeldArray.myWelMXline_2.myPolyCHSE == "Start") {
                                mtemEP = myWeldArray.myWelMXline_2.myPolyEndP;
                            }
                            else {
                                mtemEP = myWeldArray.myWelMXline_2.myPolyStartP;
                            }
                            var DownGrapLine = [];
                            DownGrapLine = GetGraphFromArea(mtempSP.x, mtemEP.x, mtempSP.y, dis, "down", TwoP_PolyLArray, ThreeP_PolyLArray)
                            if (DownGrapLine.length == 2) {
                                myWeldArray.myWelDOWN_num = 2;
                                myWeldArray.myWelDOWNline_1 = DownGrapLine[0];
                                myWeldArray.myWelDOWNline_2 = DownGrapLine[1];
                            }
                            else if (DownGrapLine.length == 1) {
                                myWeldArray.myWelDOWN_num = 1;
                                myWeldArray.myWelDOWNline_1 = DownGrapLine[0];
                            }
                            if (myWeldArray.myWelUP_num == 0 && myWeldArray.myWelDOWN_num == 0) {
                                myWeldArray.myWelType = "Y_SMPoKRTH" 
                            }
                            else if (myWeldArray.myWelUP_num != 0 && myWeldArray.myWelDOWN_num != 0) {
                                myWeldArray.myWelType = "Y_SMPoKGaiBRTH" 
                            }
                        }
                    }
                }
            }
        }
        else if (nG == 0 && nW == 0 && nS == 1 && nX == 0 && nM == 0) {
            if (myWeldArray.myWelMSline_1.myPolyPNum == 3) {
                var dis = GetDisFromTwoPoint(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyMid);
                var m_XXPlLineArr = [];
                m_XXPlLineArr = GetXXline(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP, "down", dis, TwoP_PolyLArray)
                if (m_XXPlLineArr.length >= 3 && m_XXPlLineArr.length <= 10) {
                    if (m_XXPlLineArr.length == 3) {
                        myWeldArray.m_XX_num = 3;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                    }
                    else if (m_XXPlLineArr.length == 4) {
                        myWeldArray.m_XX_num = 4;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                    }
                    else if (m_XXPlLineArr.length == 5) {
                        myWeldArray.m_XX_num = 5;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                    }
                    else if (m_XXPlLineArr.length == 6) {
                        myWeldArray.m_XX_num = 6;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                        myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                    }
                    else if (m_XXPlLineArr.length == 7) {
                        myWeldArray.m_XX_num = 7;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                        myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                        myWeldArray.m_XXline_7 = m_XXPlLineArr[6];
                    }
                    else if (m_XXPlLineArr.length == 8) {
                        myWeldArray.m_XX_num = 8;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                        myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                        myWeldArray.m_XXline_7 = m_XXPlLineArr[6];
                        myWeldArray.m_XXline_8 = m_XXPlLineArr[7];
                    }
                    else if (m_XXPlLineArr.length == 9) {
                        myWeldArray.m_XX_num = 9;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                        myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                        myWeldArray.m_XXline_7 = m_XXPlLineArr[6];
                        myWeldArray.m_XXline_8 = m_XXPlLineArr[7];
                        myWeldArray.m_XXline_9 = m_XXPlLineArr[8];
                    }
                    else if (m_XXPlLineArr.length == 10) {
                        myWeldArray.m_XX_num = 10;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                        myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                        myWeldArray.m_XXline_7 = m_XXPlLineArr[6];
                        myWeldArray.m_XXline_8 = m_XXPlLineArr[7];
                        myWeldArray.m_XXline_9 = m_XXPlLineArr[8];
                        myWeldArray.m_XXline_10 = m_XXPlLineArr[9];
                    }
                    var m_mtlLineArr = [];
                    m_mtlLineArr = GetMultiPointLineInterXX(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP, myWeldArray.m_XXline_1.myPolyStartP.y, FourMP_PolyLArray)
                    if (m_mtlLineArr.length == 1) {
                        if (CheckAngleIn(myWeldArray.myWelMSline_1.myPolyMid, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, 47, 43)) {
                            myWeldArray.myWelDOWN_num = 1;
                            myWeldArray.myWelDOWNline_1 = m_mtlLineArr[0];
                            myWeldArray.myWelType = "Y_DMDCPoKDuiJieH"
                        }
                        else if (CheckAngleIn(myWeldArray.myWelMSline_1.myPolyMid, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, 62, 58)) {
                            myWeldArray.myWelDOWN_num = 1;
                            myWeldArray.myWelDOWNline_1 = m_mtlLineArr[0];
                            myWeldArray.myWelType = "Y_DMSCPoKDuiJieH"
                        }
                    }
                    else if (m_mtlLineArr.length == 0) {
                        if (CheckAngleIn(myWeldArray.myWelMSline_1.myPolyMid, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, 47, 43)) {
                            if (myWeldArray.myWelMSStorEdorMd_1 == "Mid") {
                                myWeldArray.myWelType = "Y_DMDCPoKJiaoH"
                            }
                            else {
                                myWeldArray.myWelType = "Y_DMJiaoH"
                            }
                        }
                    }
                }
            }
            else if (myWeldArray.myWelMSline_1.myPolyPNum == 2) {
                var mUpChaArr = [];
                mUpChaArr = GetInterLineFromVerLine(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, TwoP_PolyLArray);
                if (mUpChaArr.length == 1) {
                    myWeldArray.myWelMSline_2 = mUpChaArr[0];
                    myWeldArray.myWelMS_num = 2;

                    var dis = GetDisFromTwoPoint(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP);
                    var m_XXPlLineArr = [];
                    m_XXPlLineArr = GetXXline(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP, "down", dis, TwoP_PolyLArray)
                    if (m_XXPlLineArr.length >= 3 && m_XXPlLineArr.length <= 10) {
                        if (m_XXPlLineArr.length == 3) {
                            myWeldArray.m_XX_num = 3;
                            myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                            myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                            myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        }
                        else if (m_XXPlLineArr.length == 4) {
                            myWeldArray.m_XX_num = 4;
                            myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                            myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                            myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                            myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        }
                        else if (m_XXPlLineArr.length == 5) {
                            myWeldArray.m_XX_num = 5;
                            myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                            myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                            myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                            myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                            myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                        }
                        else if (m_XXPlLineArr.length == 6) {
                            myWeldArray.m_XX_num = 6;
                            myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                            myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                            myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                            myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                            myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                            myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                        }
                        else if (m_XXPlLineArr.length == 7) {
                            myWeldArray.m_XX_num = 7;
                            myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                            myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                            myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                            myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                            myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                            myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                            myWeldArray.m_XXline_7 = m_XXPlLineArr[6];
                        }
                        else if (m_XXPlLineArr.length == 8) {
                            myWeldArray.m_XX_num = 8;
                            myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                            myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                            myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                            myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                            myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                            myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                            myWeldArray.m_XXline_7 = m_XXPlLineArr[6];
                            myWeldArray.m_XXline_8 = m_XXPlLineArr[7];
                        }
                        else if (m_XXPlLineArr.length == 9) {
                            myWeldArray.m_XX_num = 9;
                            myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                            myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                            myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                            myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                            myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                            myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                            myWeldArray.m_XXline_7 = m_XXPlLineArr[6];
                            myWeldArray.m_XXline_8 = m_XXPlLineArr[7];
                            myWeldArray.m_XXline_9 = m_XXPlLineArr[8];
                        }
                        else if (m_XXPlLineArr.length == 10) {
                            myWeldArray.m_XX_num = 10;
                            myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                            myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                            myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                            myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                            myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                            myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                            myWeldArray.m_XXline_7 = m_XXPlLineArr[6];
                            myWeldArray.m_XXline_8 = m_XXPlLineArr[7];
                            myWeldArray.m_XXline_9 = m_XXPlLineArr[8];
                            myWeldArray.m_XXline_10 = m_XXPlLineArr[9];
                        }


                        dis = GetDisFromTwoPoint(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP);
                        var mtempSP, mtemEP;
                        if (myWeldArray.myWelMSStorEdorMd_1 == "Start") {
                            mtempSP = myWeldArray.myWelMSline_1.myPolyEndP;
                        }
                        else {
                            mtempSP = myWeldArray.myWelMSline_1.myPolyStartP;
                        }
                        if (myWeldArray.myWelMSline_2.myPolyCHSE == "Start") {
                            mtemEP = myWeldArray.myWelMSline_2.myPolyEndP;
                        }
                        else {
                            mtemEP = myWeldArray.myWelMSline_2.myPolyStartP;
                        }
                        var upGrapLine = [];
                        upGrapLine = GetGraphFromArea(mtempSP.x, mtemEP.x, mtempSP.y, dis, "up", TwoP_PolyLArray, ThreeP_PolyLArray)
                        if (upGrapLine.length == 2) {
                            myWeldArray.myWelUP_num = 2;
                            myWeldArray.myWelUPline_1 = upGrapLine[0];
                            myWeldArray.myWelUPline_2 = upGrapLine[1];
                        }
                        else if (upGrapLine.length == 1) {
                            myWeldArray.myWelUP_num = 1;
                            myWeldArray.myWelUPline_1 = upGrapLine[0];
                        }
                        var m_mtlLineArr = [];
                        m_mtlLineArr = GetMultiPointLineInterXX(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP, myWeldArray.m_XXline_1.myPolyStartP.y, FourMP_PolyLArray)
                        if (m_mtlLineArr.length == 1) {
                            myWeldArray.myWelDOWN_num = 1;
                            myWeldArray.myWelDOWNline_1 = m_mtlLineArr[0];
                        }
                        else if (m_mtlLineArr.length == 0) {
                            var mTTwoLineArr = [];
                            mTTwoLineArr = GetTwoPointLineInterXX(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP, myWeldArray.m_XXline_1.myPolyStartP.y, TwoP_PolyLArray)
                            if (mTTwoLineArr.length == 1) {
                                myWeldArray.myWelMX_num = 1;
                                myWeldArray.myWelMXline_1 = mTTwoLineArr[0];
                                var mTDownChaArr = [];
                                mTDownChaArr = GetInterLineFromVerLine(myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP, TwoP_PolyLArray)
                                if (mTDownChaArr.length == 1) {
                                    myWeldArray.myWelMXline_2 = mTDownChaArr[0];
                                    myWeldArray.myWelMX_num = 2;
                                }
                                var mtempSP, mtemEP;
                                if (myWeldArray.myWelMXStorEdorMd_1 == "Start") {
                                    mtempSP = myWeldArray.myWelMXline_1.myPolyEndP;
                                }
                                else {
                                    mtempSP = myWeldArray.myWelMXline_1.myPolyStartP;
                                }
                                if (myWeldArray.myWelMXline_2.myPolyCHSE == "Start") {
                                    mtemEP = myWeldArray.myWelMXline_2.myPolyEndP;
                                }
                                else {
                                    mtemEP = myWeldArray.myWelMXline_2.myPolyStartP;
                                }
                                var mtDLineArr = [];
                                mtDLineArr = GetGraphFromArea(mtempSP.x, mtemEP.x, mtempSP.y, dis, "down", TwoP_PolyLArray, ThreeP_PolyLArray)
                                if (mtDLineArr.length == 2) {
                                    myWeldArray.myWelDOWN_num = 2;
                                    myWeldArray.myWelDOWNline_1 = mtDLineArr[0];
                                    myWeldArray.myWelDOWNline_2 = mtDLineArr[1];
                                }
                                else if (mtDLineArr.length == 1) {
                                    myWeldArray.myWelDOWN_num = 1;
                                    myWeldArray.myWelDOWNline_1 = mtDLineArr[0];
                                }
                            }
                        }
                        if (myWeldArray.myWelUP_num == 0 && myWeldArray.myWelDOWN_num == 1 && myWeldArray.myWelMX_num == 0) {
                            myWeldArray.myWelType = "Y_DMDCPoKDuiJieH"
                        }
                        else if (myWeldArray.myWelUP_num != 0 && myWeldArray.myWelDOWN_num == 0 && myWeldArray.myWelMX_num == 0) {
                            myWeldArray.myWelType = "Y_DMPoKGaiBJiaoH" 
                        }
                        else if (myWeldArray.myWelUP_num == 0 && myWeldArray.myWelDOWN_num == 0 && myWeldArray.myWelMX_num == 0) {
                            myWeldArray.myWelType = "Y_DMDCPoKJiaoH"
                        }
                        else if (myWeldArray.myWelUP_num == 0 && myWeldArray.myWelDOWN_num == 0 && myWeldArray.myWelMX_num != 0) {
                            myWeldArray.myWelType = "Y_SMPoKBeiPJiaoH"
                        }
                    }
                }
            }
        }
        else if (nG == 0 && nW == 0 && nS == 0 && nX == 0 && nM == 1) {
            if (myWeldArray.myWelMidline_1.myPolyPNum == 6) {
                var templine = FindLineFromArray(myWeldArray.myWelMidline_1.myPolyLineObjectID, FourMP_PolyLArray)
                if (CheckAngleIn(templine.GetPointAt(2), templine.GetPointAt(1), templine.GetPointAt(4), 92, 88)) {
                    myWeldArray.myWelType = "Y_SMJiaoH"
                }
            }
            else if (myWeldArray.myWelMidline_1.myPolyPNum == 3 && myWeldArray.myWelMidline_1.myPolyCHSE != "Mid") {
                if (CheckAngleIn(myWeldArray.myWelMidline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyMid, myWeldArray.myWelMidline_1.myPolyEndP, 92, 88)
                    || CheckAngleIn(myWeldArray.myWelMidline_1.myPolyEndP, myWeldArray.myWelMidline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyMid, 92, 88)) {
                    myWeldArray.myWelType = "Y_SMJiaoH"
                }
            }
        }

        else if (nG == 0 && nW == 0 && nS == 1 && nX == 1 && nM == 0) {
            if (myWeldArray.myWelMSline_1.myPolyPNum == 3 && myWeldArray.myWelMSStorEdorMd_1 != "Mid"
                && myWeldArray.myWelMXline_1.myPolyPNum == 3 && myWeldArray.myWelMXStorEdorMd_1 != "Mid") {
                if (CheckAngleIn(myWeldArray.myWelMSline_1.myPolyMid, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, 48, 43)
                    && CheckAngleIn(myWeldArray.myWelMXline_1.myPolyMid, myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP, 48, 43)) {
                    myWeldArray.myWelType = "Y_SMJiaoH"
                }
            }
            else if (myWeldArray.myWelMSline_1.myPolyPNum == 3 && myWeldArray.myWelMSStorEdorMd_1 == "Mid"
                && myWeldArray.myWelMXline_1.myPolyPNum == 3 && myWeldArray.myWelMXStorEdorMd_1 == "Mid") {
                if (CheckAngleIn(myWeldArray.myWelMSline_1.myPolyMid, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, 51, 43)
                    && CheckAngleIn(myWeldArray.myWelMXline_1.myPolyMid, myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP, 51, 43)) {
                    var dis;
                    dis = GetDisFromTwoPoint(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP);
                    var res_PolyLineArr = [];
                    res_PolyLineArr = GetGraphFromArea(myWeldArray.myWelMSline_1.myPolyStartP.x, myWeldArray.myWelMSline_1.myPolyEndP.x, myWeldArray.myWelMSline_1.myPolyStartP.y, dis, "up", TwoP_PolyLArray, ThreeP_PolyLArray)
                    if (res_PolyLineArr.length == 2) {
                        myWeldArray.myWelUP_num = 2;
                        myWeldArray.myWelUPline_1 = res_PolyLineArr[0];
                        myWeldArray.myWelUPline_2 = res_PolyLineArr[1];
                    }
                    else if (res_PolyLineArr.length == 1) {
                        myWeldArray.myWelUP_num = 1;
                        myWeldArray.myWelUPline_1 = res_PolyLineArr[0];
                    }
                    var res_XPolyLineArr = [];
                    res_XPolyLineArr = GetGraphFromArea(myWeldArray.myWelMXline_1.myPolyStartP.x, myWeldArray.myWelMXline_1.myPolyEndP.x, myWeldArray.myWelMXline_1.myPolyStartP.y, dis, "down", TwoP_PolyLArray, ThreeP_PolyLArray)
                    if (res_XPolyLineArr.length == 2) {
                        myWeldArray.myWelDOWN_num = 2;
                        myWeldArray.myWelDOWNline_1 = res_XPolyLineArr[0];
                        myWeldArray.myWelDOWNline_2 = res_XPolyLineArr[1];
                    }
                    else if (res_XPolyLineArr.length == 1) {
                        myWeldArray.myWelDOWN_num = 1;
                        myWeldArray.myWelDOWNline_1 = res_XPolyLineArr[0];
                    }
                    if (myWeldArray.myWelUP_num == 0 && myWeldArray.myWelDOWN_num == 0) {
                        myWeldArray.myWelType = "Y_SMDCPoKDuiJieH"
                    }
                }
                else if (CheckAngleIn(myWeldArray.myWelMSline_1.myPolyMid, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, 62, 58)
                    && CheckAngleIn(myWeldArray.myWelMXline_1.myPolyMid, myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP, 62, 58)) {
                    myWeldArray.myWelType = "Y_SMSCPoKDuiJieH"
                }
            }
            else if (myWeldArray.myWelMSline_1.myPolyPNum == 2 && myWeldArray.myWelMXline_1.myPolyPNum == 2) {
                if (IsVertical(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP)
					&& IsVertical(myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP)) {
                    var mUpChaArr = [];
                    var mDownChaArr = [];
                    mUpChaArr = GetInterLineFromVerLine(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, TwoP_PolyLArray);
                    mDownChaArr = GetInterLineFromVerLine(myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP, TwoP_PolyLArray);
                    if (mUpChaArr.length == 1 && mDownChaArr.length == 1) {
                        myWeldArray.myWelMSline_2 = mUpChaArr[0];
                        myWeldArray.myWelMS_num = 2;
                        myWeldArray.myWelMXline_2 = mDownChaArr[0];
                        myWeldArray.myWelMX_num = 2;

                        var dis;
                        dis = GetDisFromTwoPoint(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP)
                        var mtempSP, mtemEP;
                        if (myWeldArray.myWelMSStorEdorMd_1 == "Start") {
                            mtempSP = myWeldArray.myWelMSline_1.myPolyEndP;
                        }
                        else {
                            mtempSP = myWeldArray.myWelMSline_1.myPolyStartP;
                        }
                        if (myWeldArray.myWelMSline_2.myPolyCHSE == "Start") {
                            mtemEP = myWeldArray.myWelMSline_2.myPolyEndP;
                        }
                        else {
                            mtemEP = myWeldArray.myWelMSline_2.myPolyStartP;
                        }
                        var upGrapLine = [];
                        upGrapLine = GetGraphFromArea(mtempSP.x, mtemEP.x, mtempSP.y, dis, "up", TwoP_PolyLArray, ThreeP_PolyLArray)
                        if (upGrapLine.length == 2) {
                            myWeldArray.myWelUP_num = 2;
                            myWeldArray.myWelUPline_1 = upGrapLine[0];
                            myWeldArray.myWelUPline_2 = upGrapLine[1];
                        }
                        else if (upGrapLine.length == 1) {
                            myWeldArray.myWelUP_num = 1;
                            myWeldArray.myWelUPline_1 = upGrapLine[0];
                        }
                        if (myWeldArray.myWelMXStorEdorMd_1 == "Start") {
                            mtempSP = myWeldArray.myWelMXline_1.myPolyEndP;
                        }
                        else {
                            mtempSP = myWeldArray.myWelMXline_1.myPolyStartP;
                        }
                        if (myWeldArray.myWelMXline_2.myPolyCHSE == "Start") {
                            mtemEP = myWeldArray.myWelMXline_2.myPolyEndP;
                        }
                        else {
                            mtemEP = myWeldArray.myWelMXline_2.myPolyStartP;
                        }
                        var DownGrapLine = [];
                        DownGrapLine = GetGraphFromArea(mtempSP.x, mtemEP.x, mtempSP.y, dis, "down", TwoP_PolyLArray, ThreeP_PolyLArray)
                        if (DownGrapLine.length == 2) {
                            myWeldArray.myWelDOWN_num = 2;
                            myWeldArray.myWelDOWNline_1 = DownGrapLine[0];
                            myWeldArray.myWelDOWNline_2 = DownGrapLine[1];
                        }
                        else if (DownGrapLine.length == 1) {
                            myWeldArray.myWelDOWN_num = 1;
                            myWeldArray.myWelDOWNline_1 = DownGrapLine[0];
                        }
                        if (myWeldArray.myWelUP_num == 0 && myWeldArray.myWelDOWN_num == 0) {
                            myWeldArray.myWelType = "Y_SMDCPoKDuiJieH"
                        }
                    }
                }
            }
        }
        else if (nG == 0 && nW == 0 && nS == 1 && nX == 1 && nM == 1) {
            if (myWeldArray.myWelMSline_1.myPolyPNum == 2 && myWeldArray.myWelMXline_1.myPolyPNum == 2 && myWeldArray.myWelMidline_1.myPolyPNum == 2) {
                if (CheckAngleIn(myWeldArray.myWelMidline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyEndP, myWeldArray.myWelMSline_1.myPolyStartP, 47, 38)
                    && CheckAngleIn(myWeldArray.myWelMidline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyEndP, myWeldArray.myWelMSline_1.myPolyEndP, 47, 38)) {
                    myWeldArray.myWelType = "Y_SMJiaoH"
                }
            }
            else if (myWeldArray.myWelMSline_1.myPolyPNum == 3 && myWeldArray.myWelMXline_1.myPolyPNum == 3 && myWeldArray.myWelMidline_1.myPolyPNum == 2) {
                if (CheckAngleIn(myWeldArray.myWelMSline_1.myPolyMid, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, 52, 43)
                    && CheckAngleIn(myWeldArray.myWelMXline_1.myPolyMid, myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP, 52, 43)) {
                    myWeldArray.myWelType = "Y_SMJiaoH"
                }
            }
            else if (myWeldArray.myWelMSline_1.myPolyPNum == 3 && myWeldArray.myWelMXline_1.myPolyPNum == 2 && myWeldArray.myWelMidline_1.myPolyPNum == 2) {
                if (CheckAngleIn(myWeldArray.myWelMSline_1.myPolyMid, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, 52, 43)) {
                    myWeldArray.myWelType = "Y_SMJiaoH"
                }
            }
            else if (myWeldArray.myWelMSline_1.myPolyPNum == 2 && myWeldArray.myWelMXline_1.myPolyPNum == 3 && myWeldArray.myWelMidline_1.myPolyPNum == 2) {
                if (CheckAngleIn(myWeldArray.myWelMXline_1.myPolyMid, myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP, 52, 43)) {
                    myWeldArray.myWelType = "Y_SMJiaoH"
                }
            }
        }

        else if (nG == 0 && nW == 0 && nS == 2 && nX == 0 && nM == 0) {
            if (myWeldArray.myWelMSline_1.myPolyPNum == 2 && myWeldArray.myWelMSline_2.myPolyPNum == 2
                && IsVertical(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP)
                && IsVertical(myWeldArray.myWelMSline_2.myPolyStartP, myWeldArray.myWelMSline_2.myPolyEndP)) {
                myWeldArray.myWelType = "N_PoKDuiJieH"
            }
            if (myWeldArray.myWelMSline_1.myPolyPNum == 2 && myWeldArray.myWelMSline_2.m_PointNum == 2) {
                var dis = GetDisFromTwoPoint(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP)
                var m_XXPlLineArr = [];
                m_XXPlLineArr = GetXXline(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP, "down", dis, TwoP_PolyLArray)
                if (m_XXPlLineArr.length >= 3 && m_XXPlLineArr.length <= 10) {
                    if (m_XXPlLineArr.length == 3) {
                        myWeldArray.m_XX_num = 3;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                    }
                    else if (m_XXPlLineArr.length == 4) {
                        myWeldArray.m_XX_num = 4;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                    }
                    else if (m_XXPlLineArr.length == 5) {
                        myWeldArray.m_XX_num = 5;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                    }
                    else if (m_XXPlLineArr.length == 6) {
                        myWeldArray.m_XX_num = 6;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                        myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                    }
                    else if (m_XXPlLineArr.length == 7) {
                        myWeldArray.m_XX_num = 7;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                        myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                        myWeldArray.m_XXline_7 = m_XXPlLineArr[6];
                    }
                    else if (m_XXPlLineArr.length == 8) {
                        myWeldArray.m_XX_num = 8;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                        myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                        myWeldArray.m_XXline_7 = m_XXPlLineArr[6];
                        myWeldArray.m_XXline_8 = m_XXPlLineArr[7];
                    }
                    else if (m_XXPlLineArr.length == 9) {
                        myWeldArray.m_XX_num = 9;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                        myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                        myWeldArray.m_XXline_7 = m_XXPlLineArr[6];
                        myWeldArray.m_XXline_8 = m_XXPlLineArr[7];
                        myWeldArray.m_XXline_9 = m_XXPlLineArr[8];
                    }
                    else if (m_XXPlLineArr.length == 10) {
                        myWeldArray.m_XX_num = 10;
                        myWeldArray.m_XXline_1 = m_XXPlLineArr[0];
                        myWeldArray.m_XXline_2 = m_XXPlLineArr[1];
                        myWeldArray.m_XXline_3 = m_XXPlLineArr[2];
                        myWeldArray.m_XXline_4 = m_XXPlLineArr[3];
                        myWeldArray.m_XXline_5 = m_XXPlLineArr[4];
                        myWeldArray.m_XXline_6 = m_XXPlLineArr[5];
                        myWeldArray.m_XXline_7 = m_XXPlLineArr[6];
                        myWeldArray.m_XXline_8 = m_XXPlLineArr[7];
                        myWeldArray.m_XXline_9 = m_XXPlLineArr[8];
                        myWeldArray.m_XXline_10 = m_XXPlLineArr[9];
                    }
                    var m_mtlLineArr = [];
                    m_mtlLineArr = GetMultiPointLineInterXX(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP, myWeldArray.m_XXline_1.myPolyStartP.y, FourMP_PolyLArray)
                    if (m_mtlLineArr.length == 1) {
                        var bj = FALSE;
                        if (myWeldArray.myWelMSStorEdorMd_1 == "Start") {
                            if (CheckAngleIn(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, myWeldArray.myWelMSline_2.myPolyStartP, 73, 65)
                                || CheckAngleIn(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, myWeldArray.myWelMSline_2.myPolyEndP, 73, 65)) {
                                bj = TRUE;
                            }
                        }
                        else if (myWeldArray.GetAt(i).m_MSStorEdorMd_1 == "End") {
                            if (CheckAngleIn(myWeldArray.myWelMSline_1.myPolyEndP, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_2.myPolyStartP, 73, 65)
                                || CheckAngleIn(myWeldArray.myWelMSline_1.myPolyEndP, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_2.myPolyEndP, 73, 65)) {
                                bj = TRUE;
                            }
                        }
                        if (bj == TRUE) {
                            myWeldArray.myWelDOWN_num = 1;
                            myWeldArray.myWelDOWNline_1 = m_mtlLineArr[0];
                            myWeldArray.myWelType = "Y_DMSCPoKDuiJieH"
                        }
                    }
                }
            }
        }
        else if (nG == 0 && nW == 1 && nS == 1 && nX == 1 && nM == 1) {
            if (myWeldArray.myWelMSline_1.myPolyPNum == 2 && myWeldArray.myWelMSStorEdorMd_1 != "Mid"
                && myWeldArray.myWelMXline_1.myPolyPNum == 2&&myWeldArray.myWelMXStorEdorMd_1!="Mid"
                && myWeldArray.myWelMidline_1.myPolyPNum == 2
                && myWeldArray.myWelWHline_1.myPolyPNum == 3) {
                if (CheckAngleIn(myWeldArray.myWelWHline_1.myPolyMid, myWeldArray.myWelWHline_1.myPolyStartP, myWeldArray.myWelWHline_1.myPolyEndP, 92, 78)) {

                    if(myWeldArray.myWelMidline_1.myPolyStartP.y>myWeldArray.myWelMidline_1.myPolyEndP.y)
                    {
                        if ((CheckAngleIn(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, myWeldArray.myWelMidline_1.myPolyStartP, 51, 43)||
                            CheckAngleIn(myWeldArray.myWelMSline_1.myPolyEndP, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyStartP, 51, 43))
                           &&(CheckAngleIn(myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP, myWeldArray.myWelMidline_1.myPolyEndP, 51, 43)||
                            CheckAngleIn(myWeldArray.myWelMXline_1.myPolyEndP, myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyEndP, 51, 43)))
                        {
                            var dis;
                            dis = GetDisFromTwoPoint(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP);
                            var res_PolyLineArr = [];
                            res_PolyLineArr = GetGraphFromArea(myWeldArray.myWelMSline_1.myPolyStartP.x, myWeldArray.myWelMSline_1.myPolyEndP.x, myWeldArray.myWelMidline_1.myPolyStartP.y, dis, "up", TwoP_PolyLArray, ThreeP_PolyLArray)
                            if (res_PolyLineArr.length == 2) {
                                myWeldArray.myWelUP_num = 2;
                                myWeldArray.myWelUPline_1 = res_PolyLineArr[0];
                                myWeldArray.myWelUPline_2 = res_PolyLineArr[1];
                            }
                            else if (res_PolyLineArr.length == 1) {
                                myWeldArray.myWelUP_num = 1;
                                myWeldArray.myWelUPline_1 = res_PolyLineArr[0];
                            }
                            var res_XPolyLineArr = [];
                            res_XPolyLineArr = GetGraphFromArea(myWeldArray.myWelMXline_1.myPolyStartP.x, myWeldArray.myWelMXline_1.myPolyEndP.x, myWeldArray.myWelMidline_1.myPolyEndP.y, dis, "down", TwoP_PolyLArray, ThreeP_PolyLArray)
                            if (res_XPolyLineArr.length == 2) {
                                myWeldArray.myWelDOWN_num = 2;
                                myWeldArray.myWelDOWNline_1 = res_XPolyLineArr[0];
                                myWeldArray.myWelDOWNline_2 = res_XPolyLineArr[1];
                            }
                            else if (res_XPolyLineArr.length == 1) {
                                myWeldArray.myWelDOWN_num = 1;
                                myWeldArray.myWelDOWNline_1 = res_XPolyLineArr[0];
                            }
                            if (myWeldArray.myWelUP_num == 0 && myWeldArray.myWelDOWN_num == 0) {
                                myWeldArray.myWelType = "Y_SMPoKRTH"
                            }
                            else if (myWeldArray.myWelUP_num != 0 && myWeldArray.myWelDOWN_num != 0) {
                                myWeldArray.myWelType = "Y_SMPoKGaiBRTH"
                            }
                        }
                }
                else 
                {
                        if ((CheckAngleIn(myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMSline_1.myPolyEndP, myWeldArray.myWelMidline_1.myPolyEndP, 51, 43) ||
                            CheckAngleIn(myWeldArray.myWelMSline_1.myPolyEndP, myWeldArray.myWelMSline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyEndP, 51, 43))
                           && (CheckAngleIn(myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMXline_1.myPolyEndP, myWeldArray.myWelMidline_1.myPolyStartP, 51, 43) ||
                            CheckAngleIn(myWeldArray.myWelMXline_1.myPolyEndP, myWeldArray.myWelMXline_1.myPolyStartP, myWeldArray.myWelMidline_1.myPolyStartP, 51, 43))) {
                            var dis;
                            dis = GetDisFromTwoPoint(myWeldArray.myWelGUIP, myWeldArray.myWelWEIP);
                            var res_PolyLineArr = [];
                            res_PolyLineArr = GetGraphFromArea(myWeldArray.myWelMSline_1.myPolyStartP.x, myWeldArray.myWelMSline_1.myPolyEndP.x, myWeldArray.myWelMidline_1.myPolyEndP.y, dis, "up", TwoP_PolyLArray, ThreeP_PolyLArray)
                            if (res_PolyLineArr.length == 2) {
                                myWeldArray.myWelUP_num = 2;
                                myWeldArray.myWelUPline_1 = res_PolyLineArr[0];
                                myWeldArray.myWelUPline_2 = res_PolyLineArr[1];
                            }
                            else if (res_PolyLineArr.length == 1) {
                                myWeldArray.myWelUP_num = 1;
                                myWeldArray.myWelUPline_1 = res_PolyLineArr[0];
                            }
                            var res_XPolyLineArr = [];
                            res_XPolyLineArr = GetGraphFromArea(myWeldArray.myWelMXline_1.myPolyStartP.x, myWeldArray.myWelMXline_1.myPolyEndP.x, myWeldArray.myWelMidline_1.myPolyStartP.y, dis, "down", TwoP_PolyLArray, ThreeP_PolyLArray)
                            if (res_XPolyLineArr.length == 2) {
                                myWeldArray.myWelDOWN_num = 2;
                                myWeldArray.myWelDOWNline_1 = res_XPolyLineArr[0];
                                myWeldArray.myWelDOWNline_2 = res_XPolyLineArr[1];
                            }
                            else if (res_XPolyLineArr.length == 1) {
                                myWeldArray.myWelDOWN_num = 1;
                                myWeldArray.myWelDOWNline_1 = res_XPolyLineArr[0];
                            }
                            if (myWeldArray.myWelUP_num == 0 && myWeldArray.myWelDOWN_num == 0) {
                                myWeldArray.myWelType = "Y_SMPoKRTH" 
                            }
                            else if (myWeldArray.myWelUP_num != 0 && myWeldArray.myWelDOWN_num != 0) {
                                myWeldArray.myWelType = "Y_SMPoKGaiBRTH" 
                            }
                        }
                    }                                    
                }
            }
        }
        return myWeldArray;  
}

function myArrow(m_ObjectID, m_DingP, m_DiA, m_DiB, m_Mid) {
    this. myArrowObjectID=m_ObjectID;
    this.myArrowDingP=m_DingP;
    this.myArrowDiA = m_DiA;
    this.myArrowDiB = m_DiB;
    this.myArrowMid = m_Mid;
}
function myPolyLine(m_ObjectID, m_Pnum, m_StartP, m_EndP, m_MidP, m_CHStarOREnd) {
    this.myPolyLineObjectID = m_ObjectID;
    this.myPolyPNum = m_Pnum;
    this.myPolyStartP = m_StartP;
    this.myPolyEndP = m_EndP;
    this.myPolyMid = m_MidP;
    this.myPolyCHSE = m_CHStarOREnd;
}
function myCAnnotation(m_Arrow, m_Line1, m_CHStartOREnd, m_CHPoint) {
    this.myCAnnotationArrow = m_Arrow;
    this.myCAnnotationLine = m_Line1;
    this.myCAnnotationCHSE = m_CHStartOREnd;
    this.myCAnnotationCHP = m_CHPoint;
}
function myWelding(m_Arrow, m_YINLine1, m_YINLine2, m_HLength, m_CHStartOREnd, m_CHPoint, m_YIN_num,
                     m_GUAIPoint, m_WEIPoint, m_GHline_1, m_GHline_2, m_GHArc, m_GH_num, m_GHPoint_1,
                     m_GHPoint_2, m_WHline_1, m_WHline_2, m_WH_num, m_WHPoint_1, m_WHPoint_2,
                     m_MSline_1, m_MSStorEdorMd_1, m_MSline_2, m_MSStorEdorMd_2, m_MSline_3, m_MSStorEdorMd_3, m_MS_num,
                     m_MXline_1, m_MXStorEdorMd_1, m_MXline_2, m_MXStorEdorMd_2, m_MXline_3, m_MXStorEdorMd_3, m_MX_num,
                     m_Midline_1, m_Midline_2, m_Mid_num, m_UPline_1, m_UPline_2, m_UPline_3, m_UP_num,
                     m_XXline_1, m_XXline_2, m_XXline_3, m_XXline_4, m_XXline_5, m_XXline_6, m_XXline_7, m_XXline_8, m_XXline_9, m_XXline_10, m_XX_num,
                     m_DOWNline_1, m_DOWNline_2, m_DOWNline_3, m_DOWN_num, m_strType) {
    this.myWelArrow = m_Arrow;
    this.myWelYINline1 = m_YINLine1;
    this.myWelYINline2 = m_YINLine2;
    this.myWelHLength = m_HLength;
    this.myWelCHSE = m_CHStartOREnd;
    this.myWelCHP = m_CHPoint;
    this.myWelYINnum = m_YIN_num;
    this.myWelGUIP = m_GUAIPoint;
    this.myWelWEIP = m_WEIPoint;

    this.myWelGHline1 = m_GHline_1;
    this.myWelGHline2 = m_GHline_2;
    this.myWelGHArc = m_GHArc;
    this.myWelGH_num = m_GH_num;
    this.myWelGHPoint_1 = m_GHPoint_1;
    this.myWelGHPoint_2 = m_GHPoint_2;

    this.myWelWHline_1 = m_WHline_1;
    this.myWelWHline_2 = m_WHline_2;
    this.myWelWH_num = m_WH_num;
    this.myWelWHPoint_1 = m_WHPoint_1;
    this.myWelWHPoint_2 = m_WHPoint_2;

    this.myWelMSline_1 = m_MSline_1;
    this.myWelMSStorEdorMd_1 = m_MSStorEdorMd_1;
    this.myWelMSline_2 = m_MSline_2;
    this.myWelMSStorEdorMd_2 = m_MSStorEdorMd_2;
    this.myWelMSline_3 = m_MSline_3;
    this.myWelMSStorEdorMd_3 = m_MSStorEdorMd_3;
    this.myWelMS_num = m_MS_num;

    this.myWelMXline_1 = m_MXline_1;
    this.myWelMXStorEdorMd_1 = m_MXStorEdorMd_1;
    this.myWelMXline_2 = m_MXline_2;
    this.myWelMXStorEdorMd_2 = m_MXStorEdorMd_2;
    this.myWelMXline_3 = m_MXline_3;
    this.myWelMXStorEdorMd_3 = m_MXStorEdorMd_3;
    this.myWelMX_num = m_MX_num;

    this.myWelMidline_1 = m_Midline_1;
    this.myWelMidline_2 = m_Midline_2;
    this.myWelMid_num = m_Mid_num;

    this.myWelUPline_1 = m_UPline_1;
    this.myWelUPline_2 = m_UPline_2;
    this.myWelUPline_3 = m_UPline_3;
    this.myWelUP_num = m_UP_num;

    this.myWelXXline_1 = m_XXline_1;
    this.myWelXXline_2 = m_XXline_2;
    this.myWelXXline_3 = m_XXline_3;
    this.myWelXXline_4 = m_XXline_4;
    this.myWelXXline_5 = m_XXline_5;
    this.myWelXXline_6 = m_XXline_6;
    this.myWelXXline_7 = m_XXline_7;
    this.myWelXXline_8 = m_XXline_8;
    this.myWelXXline_9 = m_XXline_9;
    this.myWelXXline_10 = m_XXline_10;
    this.myWelXX_num = m_XX_num;

    this.myWelDOWNline_1 = m_DOWNline_1;
    this.myWelDOWNline_2 = m_DOWNline_2;
    this.myWelDOWNline_3 = m_DOWNline_3;
    this.myWelDOWN_num = m_DOWN_num;
    this.myWelType = m_strType;
}
function GetTwoPointLineInterXX(StartP, EndP, mY, TwoP_PolyLineArray) {
    var resPolyLineArr = [];
    var dis = 0.05;
    var max_X, min_X;
    if (StartP.x > EndP.x) {
        max_X = StartP.x;
        min_X = EndP.x;
    }
    else {
        max_X = EndP.x;
        min_X = StartP.x;
    }
    for (var i = 0; i < TwoP_PolyLineArray.length; i++) {
        if (IsVertical(TwoP_PolyLineArray[i].GetPointAt(0), TwoP_PolyLineArray[i].GetPointAt(1))) {
            if (TwoP_PolyLineArray[i].GetPointAt(0).y <= mY + dis && TwoP_PolyLineArray[i].GetPointAt(0).y >= mY - dis
				&& TwoP_PolyLineArray[i].GetPointAt(0).x <= max_X && TwoP_PolyLineArray[i].GetPointAt(0).x >= min_X) {
                var mtline = new myPolyLine(TwoP_PolyLineArray[i].handle, 2, TwoP_PolyLineArray[i].GetPointAt(0),
                                                        TwoP_PolyLineArray[i].GetPointAt(1), null, "Start")
                resPolyLineArr[resPolyLineArr.length] = mtline
            }
            else if (TwoP_PolyLineArray[i].GetPointAt(1).y <= mY + dis && TwoP_PolyLineArray[i].GetPointAt(1).y >= mY - dis
				&& TwoP_PolyLineArray[i].GetPointAt(1).x <= max_X && TwoP_PolyLineArray[i].GetPointAt(1).x >= min_X) {
                var mtline = new myPolyLine(TwoP_PolyLineArray[i].handle, 2, TwoP_PolyLineArray[i].GetPointAt(0),
                                                          TwoP_PolyLineArray[i].GetPointAt(1), null, "End")
                resPolyLineArr[resPolyLineArr.length] = mtline
            }
        }
    }
    return resPolyLineArr;
}
function GetMultiPointLineInterXX(StartP, EndP, mY, FourMP_PolyLArray) {
    var dis = 0.05;
    var max_X, min_X;
    var resPolyline = [];
    if (StartP.x > EndP.x) {
        max_X = StartP.x;
        min_X = EndP.x;
    }
    else {
        max_X = EndP.x;
        min_X = StartP.x;
    }
    for (var i = 0; i < FourMP_PolyLArray.length; i++) {
        var Pnum = FourMP_PolyLArray[i].numVerts;
        if (FourMP_PolyLArray[i].GetPointAt(0).y <= mY + dis && FourMP_PolyLArray[i].GetPointAt(0).y >= mY - dis
				&& FourMP_PolyLArray[i].GetPointAt(Pnum - 1).y <= mY + dis && FourMP_PolyLArray[i].GetPointAt(Pnum - 1).y >= mY - dis
				&& FourMP_PolyLArray[i].GetPointAt(0).x <= max_X && FourMP_PolyLArray[i].GetPointAt(0).x >= min_X
				&& FourMP_PolyLArray[i].GetPointAt(Pnum - 1).x <= max_X && FourMP_PolyLArray[i].GetPointAt(Pnum - 1).x >= min_X) {
            if (FourMP_PolyLArray[i].IsClosedStatus == true) {

                var mtline = new myPolyLine(FourMP_PolyLArray[i].handle, Pnum, FourMP_PolyLArray[i].GetPointAt(0),
                                                          FourMP_PolyLArray[i].GetPointAt(Pnum - 1), null, null)
                resPolyline[resPolyline.length] = mtline;
                break;
            }

        }
    }
    return resPolyline;

}
function GetInterLineFromVerLine(StartP, EndP, TwoP_PolyLineArray) {
    var max_Y, min_Y;
    var td = 0.05;
    var mResLineArr = [];
    if (IsVertical(StartP, EndP)) {
        if (StartP.y >= EndP.y) {
            max_Y = StartP.y;
            min_Y = EndP.y;
        }
        else {
            max_Y = EndP.y;
            min_Y = StartP.y;
        }
        for (var i = 0; i < TwoP_PolyLineArray.length; i++) {
            if (CheckLineType(TwoP_PolyLineArray[i].GetPointAt(0), TwoP_PolyLineArray[i].GetPointAt(1)) == "X") {
                if (TwoP_PolyLineArray[i].GetPointAt(0).x < (StartP.x + td) && TwoP_PolyLineArray[i].GetPointAt(0).x > (StartP.x - td)
					&& TwoP_PolyLineArray[i].GetPointAt(0).y <= max_Y && TwoP_PolyLineArray[i].GetPointAt(0).y >= min_Y) {
                    var mtline = new myPolyLine(TwoP_PolyLineArray[i].handle, 2, TwoP_PolyLineArray[i].GetPointAt(0),
                                                         TwoP_PolyLineArray[i].GetPointAt(1), null, "Start")
                    mResLineArr[mResLineArr.length] = mtline;
                }
                else if (TwoP_PolyLineArray[i].GetPointAt(1).x < (StartP.x + td) && TwoP_PolyLineArray[i].GetPointAt(1).x > (StartP.x - td)
					&& TwoP_PolyLineArray[i].GetPointAt(1).y <= max_Y && TwoP_PolyLineArray[i].GetPointAt(1).y >= min_Y) {
                    var mtline = new myPolyLine(TwoP_PolyLineArray[i].handle, 2, TwoP_PolyLineArray[i].GetPointAt(0),
                                                         TwoP_PolyLineArray[i].GetPointAt(1), null, "End")
                    mResLineArr[mResLineArr.length] = mtline;
                }
            }
        }
    }
    return mResLineArr;
}
function GetGraphFromArea(StartX, EndX, mY, dis, direction, TwoP_PolyLineArray, ThreeP_PolyLineArray) {
    var bl = false;
    var max_X = 0;
    var min_X = 0;
    var max_Y = 0;
    var min_Y = 0;
    var td = 0.05;

    var twoLnum = 0;
    var threeLnum = 0;
    var res_PolyLineArray = [];

    if (StartX > EndX) {
        max_X = StartX + td;
        min_X = EndX - td;
    }
    else {
        max_X = EndX + td;
        min_X = StartX - td;
    }
    if (direction == "up") {
        max_Y = mY + dis;
        min_Y = mY;
    }
    else if (direction == "down") {
        max_Y = mY;
        min_Y = mY - dis;
    }
    for (var i = 0; i < TwoP_PolyLineArray.length; i++) {
        if (TwoP_PolyLineArray[i].GetPointAt(0).x <= max_X && TwoP_PolyLineArray[i].GetPointAt(0).x >= min_X
			&& TwoP_PolyLineArray[i].GetPointAt(0).y <= max_Y && TwoP_PolyLineArray[i].GetPointAt(0).y >= min_Y
			&& TwoP_PolyLineArray[i].GetPointAt(1).x <= max_X && TwoP_PolyLineArray[i].GetPointAt(1).x >= min_X
			&& TwoP_PolyLineArray[i].GetPointAt(1).y <= max_Y && TwoP_PolyLineArray[i].GetPointAt(1).y >= min_Y) {

            var mtline = new myPolyLine(TwoP_PolyLineArray[i].handle, 2, TwoP_PolyLineArray[i].GetPointAt(0),
                                                          TwoP_PolyLineArray[i].GetPointAt(1), null, null)
            res_PolyLineArray[res_PolyLineArray.length] = mtline;
            twoLnum = twoLnum + 1;
        }
    }

    for (var j = 0; j < ThreeP_PolyLineArray.length; j++) {
        if (ThreeP_PolyLineArray[j].GetPointAt(0).x <= max_X && ThreeP_PolyLineArray[j].GetPointAt(0).x >= min_X
			&& ThreeP_PolyLineArray[j].GetPointAt(0).y <= max_Y && ThreeP_PolyLineArray[j].GetPointAt(0).y >= min_Y
			&& ThreeP_PolyLineArray[j].GetPointAt(2).x <= max_X && ThreeP_PolyLineArray[j].GetPointAt(2).x >= min_X
			&& ThreeP_PolyLineArray[j].GetPointAt(2).y <= max_Y && ThreeP_PolyLineArray[j].GetPointAt(2).y >= min_Y
            && ThreeP_PolyLineArray[j].GetPointAt(1).x <= max_X && ThreeP_PolyLineArray[j].GetPointAt(1).x >= min_X
            && ThreeP_PolyLineArray[j].GetPointAt(1).y <= max_Y && ThreeP_PolyLineArray[j].GetPointAt(1).y >= min_Y) {

            var mtline = new myPolyLine(ThreeP_PolyLineArray[j].handle, 3, ThreeP_PolyLineArray[j].GetPointAt(0),
                                                          ThreeP_PolyLineArray[j].GetPointAt(2), ThreeP_PolyLineArray[j].GetPointAt(1), null)
            res_PolyLineArray[res_PolyLineArray.length] = mtline;
            threeLnum = threeLnum + 1;
        }
    }
    if (twoLnum == 1 && threeLnum == 1) {
        if (CheckAngleIn(res_PolyLineArray[0].myPolyEndP, res_PolyLineArray[0].myPolyStartP, res_PolyLineArray[1].myPolyMid, 47, 43)) {
            return res_PolyLineArray;
        }
        else {
            res_PolyLineArray = [];
            return res_PolyLineArray;
        }
    }
    else if (twoLnum == 0 && threeLnum == 1) {
        if (CheckAngleIn(res_PolyLineArray[0].myPolyMid, res_PolyLineArray[0].myPolyStartP, res_PolyLineArray[0].myPolyEndP, 48, 43)
            || CheckAngleIn(res_PolyLineArray[0].myPolyMid, res_PolyLineArray[0].myPolyStartP, res_PolyLineArray[0].myPolyEndP, 92, 88)) {
            return res_PolyLineArray;
        }
        else {
            res_PolyLineArray = [];
            return res_PolyLineArray;
        }
    }
    else {
        res_PolyLineArray = [];
        return res_PolyLineArray;
    }
}
function IsSamePoint(pA1,pA2)
{
    var d=0.005;
    if(pA1.x==pA2.x&&pA1.y==pA2.y&&pA1.z==pA2.z)
    {
        return true;
    }
    else if(((pA1.x<=pA2.x+d&&pA1.x>=pA2.x-d)||(pA2.x<=pA1.x+d&&pA2.x>=pA1.x-d))
		&&((pA1.y<=pA2.y+d&&pA1.y>=pA2.y-d)||(pA2.y<=pA1.y+d&&pA2.y>=pA1.y-d)))
    {
        return true;
    }
    else
    {
        return false;
    }
}
function IsNearPoint(pA1,pA2)
{
    var d=0.05;
    if (pA1.x==pA2.x&&pA1.y==pA2.y)
    {
        return true;
    }
    else if (((pA1.x<=pA2.x+d&&pA1.x>=pA2.x-d)||(pA2.x<=pA1.x+d&&pA2.x>=pA1.x-d))
		&&((pA1.y<=pA2.y+d&&pA1.y>=pA2.y-d)||(pA2.y<=pA1.y+d&&pA2.y>=pA1.y-d)))
    {
        return true;
    }
    else
    {
        return false;
    }
}
function IsHorizontal(pA1, pA2) {
    if (pA1.x != pA2.x && pA1.y == pA2.y) {
        return true;
    }
    else if (pA1.x != pA2.x && ((pA1.y <= pA2.y + 0.001 && pA1.y >= pA2.y - 0.001) || (pA2.y <= pA1.y + 0.001 && pA2.y >= pA1.y - 0.001))) {
        return true;
    }
    else {
        return false;
    }
}
function IsVertical(pA1, pA2) {
    if (pA1.y != pA2.y && pA1.x == pA2.x) {
        return true;
    }
    else if (pA1.y != pA2.y && ((pA1.x <= pA2.x + 0.001 && pA1.x >= pA2.x - 0.001) || (pA2.x <= pA1.x + 0.001 && pA2.x >= pA1.x - 0.001))) {
        return true;
    }
    else {
        return false;
    }
}
function CheckLineType(pA1, pA2) {
    var str = "";
    if (IsHorizontal(pA1, pA2)) {
        str="H"
    }
    else if (IsVertical(pA1, pA2)) {
        str = "V"
    }
    else {
        str="X"
    }
    return str
}
function GetDisFromTwoPoint(pA1, pA2){
    var mdis;
    mdis=Math.sqrt(Math.pow((pA1.x-pA2.x),2)+Math.pow((pA1.y-pA2.y),2));
    return mdis;
}
function CheckLineInHLine(LoneP1,LoneP2,HLP1,HLP2)
{
    var bl=false;
    if (IsHorizontal(LoneP1,LoneP2)&&LoneP1.y==HLP1.y)
    {
        if (LoneP1.x>=LoneP2.x)
        {
            if (HLP1.x>HLP2.x)
            {
                if(LoneP1.x<=HLP1.x&&LoneP2.x>=HLP2.x)
                {
                    bl=true;
                }
            }
            else
            {
                if(LoneP1.x<=HLP2.x&&LoneP2.x>=HLP1.x)
                {
                    bl=true;
                }
            }
        }
        else
        {
            if (HLP1.x>HLP2.x)
            {
                if(LoneP2.x<=HLP1.x&&LoneP1.x>=HLP2.x)
                {
                    bl=true;
                }
            }
            else
            {
                if(LoneP2.x<=HLP2.x&&LoneP1.x>=HLP1.x)
                {
                    bl=true;
                }
            }
        }
    }
    return bl;  
}
function IsPointOnLine(pA,pB,pC)
{
    var bl=false;
    var td=0.05;
    if(pB.x>=pC.x)
    {
        if (pA.x<pB.x&&pA.x>pC.x
			&&pA.y>=pB.y-td&&pA.y<=pB.y+td)
        {
            bl=true;
        }
    }
    else
    {
        if (pA.x>pB.x&&pA.x<pC.x
			&&pA.y>=pB.y-td&&pA.y<=pB.y+td)
        {
            bl = true;
        }
    }
    return bl;
}
function GetPositionOfLine(mLine,pB,pC)
{
    var mrest="";
    var up_num=0;
    var down_num=0;
    var n=mLine.numVerts;
    for(var i=0; i<n; i++)
    {
        var mstr=GetPositionOfPoint(mLine.GetPointAt(i),pB,pC,0.05)
        if (mstr=="up")
        {
            up_num=up_num+1;
        }
        else if (mstr=="down")
        {
            down_num=down_num+1;
        }
    }
    if(up_num!=0&&down_num==0)
    {
        mrest="up";
    }
    else if (up_num==0&&down_num!=0)
    {
        mrest="down";
    }
    else
    {
        mrest="mid";
    }

    return mrest;

}
function GetPositionOfPoint(pA,pB,pC,dd)
{
    var mstr="";
    if (pB.x>pC.x)
    {
        if (pA.x<pB.x&&pA.x>pC.x)
        {
            if (pA.y>pB.y+dd)
            {
                mstr="up";
            }
            else if (pA.y<pB.y-dd)
            {
                mstr="down";
            }
        }
    }
    else
    {
        if (pA.x<pC.x&&pA.x>pB.x)
        {
            if (pA.y>pB.y+dd)
            {
                mstr="up";
            }
            else if (pA.y<pB.y-dd)
            {
                mstr="down";
            }
        }
    }
    return mstr;
}
function IsLineInterWithHYinLine(StartP,EndP,GuaiP,WeiP) {
    var bl=false;
    var max_X=0;
    var min_X=0;
    if (GuaiP.x>=WeiP.x)
    {
        max_X=GuaiP.x;
        min_X=WeiP.x;
    }
    else
    {
        max_X=WeiP.x;
        min_X=GuaiP.x;
    }
    if (IsVertical(StartP,EndP))
    {
        if (StartP.x<max_X&&StartP.x>min_X
			&&EndP.x<max_X&&EndP.x>min_X)
        {
            bl = true;
        }
    }
    return bl;
}
function GetXXline(sPoint1,sPoint2,direction,dis,TwoP_PolyLineArray)
{
    var mResLineArray=[]
    var num=TwoP_PolyLineArray.length;
    var max_X,min_X,max_Y,min_Y;
    var td=0.2*GetDisFromTwoPoint(sPoint1,sPoint2)
    if(sPoint1.x>=sPoint2.x)
    {
        max_X=sPoint1.x+td;
        min_X=sPoint2.x-td;
    }
    else
    {
        max_X=sPoint2.x+td;
        min_X=sPoint1.x-td;
    }
    if(direction=="down")
    {
        max_Y=sPoint1.y;
        min_Y=sPoint1.y-dis;
    }
    else if(direction=="up")
    {
        max_Y=sPoint1.y+dis;
        min_Y=sPoint1.y;
    }
    for(var i=0;i<num;i++)
    {
        if (IsHorizontal(TwoP_PolyLineArray[i].GetPointAt(0), TwoP_PolyLineArray[i].GetPointAt(1)))
        {
            if (TwoP_PolyLineArray[i].GetPointAt(0).x <= max_X && TwoP_PolyLineArray[i].GetPointAt(0).x >= min_X
                && TwoP_PolyLineArray[i].GetPointAt(1).x <= max_X && TwoP_PolyLineArray[i].GetPointAt(1).x >= min_X
                && TwoP_PolyLineArray[i].GetPointAt(0).y <= max_Y && TwoP_PolyLineArray[i].GetPointAt(0).y >= min_Y)
            {
                var mtline = new myPolyLine(TwoP_PolyLineArray[i].handle, 2, TwoP_PolyLineArray[i].GetPointAt(0),
                                                         TwoP_PolyLineArray[i].GetPointAt(1), null, null)
                mResLineArray[mResLineArray.length] = mtline
            }
        }
    }
    return mResLineArray
}
function CheckAngleIn(pA,pB,pC,maxA,minA)
{
    var mAngle=GetAngleFrom3Point(pA,pB,pC);
    if (mAngle<=maxA&&mAngle>=minA)
    {
        return true;
    }
    else
    {
        return false;
    }
}
function GetAngleFrom3Point(pA,pB,pC)
{
    var mAngle_A;
    var mAB,mAC,mCosA;
    mAB=Math.sqrt((pB.x-pA.x)*(pB.x-pA.x)+(pB.y-pA.y)*(pB.y-pA.y));
    mAC=Math.sqrt((pC.x-pA.x)*(pC.x-pA.x)+(pC.y-pA.y)*(pC.y-pA.y));
    mCosA=((pB.x-pA.x)*(pC.x-pA.x)+(pB.y-pA.y)*(pC.y-pA.y))/(mAB*mAC);
    mAngle_A=Math.acos(mCosA)*180/3.1415926;
    return mAngle_A;
}
function FindLineFromArray(myObjectID, PolyLineArray) {
    var num = PolyLineArray.length
    var myline=null
    for (var i = 0; i < num; i++)
    {
        if (PolyLineArray[i].handle == myObjectID) {
            myline = PolyLineArray[i]
            break;
        }
    }
    return myline;
}
function DeletSameArrow(ArrowArray) {
    var mNSameArrowArray = []; 
    var snum = 0;
    var mbl = false;
    var num = ArrowArray.length;
    for (var i = 0; i < num; i++) {
        mbl = false;
        snum = mNSameArrowArray.length;
        for(var j=0;j<snum;j++)
        {
            if (IsSamePoint(ArrowArray[i].myArrowDiA, mNSameArrowArray[j].myArrowDiA)
                && IsSamePoint(ArrowArray[i].myArrowDiB, mNSameArrowArray[j].myArrowDiB)
                && IsSamePoint(ArrowArray[i].myArrowDingP, mNSameArrowArray[j].myArrowDingP)) {
                mbl = true;
                break;
            }
        }
        if(mbl==false)
        {
            mNSameArrowArray[mNSameArrowArray.length] = ArrowArray[i]
        }
    }
    return mNSameArrowArray
}
function DeletSamePolyLine(polyLArray) {
    var snum = 0;
    var bool = false;
    var num = polyLArray.length;
    var NSPolyLineArr = [];
    for (var i = 0; i < num ; i++)
    {
        mbl = false;
        snum = NSPolyLineArr.length;
        for (var j = 0; j < snum;j++)
        {
            var pnum = polyLArray[i].numVerts
            var blnum=0;
            for (var t = 0; t < pnum;t++)
            {
                if (IsSamePoint(polyLArray[i].GetPointAt(t), NSPolyLineArr[j].GetPointAt(t))) {
                    blnum=blnum+1;
                }
                else {
                    blnum = 0;
                    break;
                }
            }
            if (blnum==pnum)
            {
                mbl = true;
                break;
            }
        }
        if(mbl==false)
        {
            NSPolyLineArr[NSPolyLineArr.length] = polyLArray[i];
        }
    }
    return NSPolyLineArr;
}
function ChangeWeldColorbyHandle(myWeldArray, myColorID) {

    var mxOcx = document.all.item("MxDrawXCtrl");
    var database = mxOcx.GetDatabase();   
    var ent;

        if (myWeldArray.myWelYINnum == 1) {
            ent = database.HandleToObject(myWeldArray.myWelYINline1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelYINnum == 2) {
            ent = database.HandleToObject(myWeldArray.myWelYINline1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelYINline2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }

        if (myWeldArray.myWelGH_num == 1) {
            ent = database.HandleToObject(myWeldArray.myWelGHline1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelGH_num == 2) {
            ent = database.HandleToObject(myWeldArray.myWelGHline1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelGHline2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }

        if (myWeldArray.myWelWH_num == 1) {
            ent = database.HandleToObject(myWeldArray.myWelWHline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelWH_num == 2) {
            ent = database.HandleToObject(myWeldArray.myWelWHline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelWHline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }

        if (myWeldArray.myWelMS_num == 1) {
            ent = database.HandleToObject(myWeldArray.myWelMSline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelMS_num == 2) {
            ent = database.HandleToObject(myWeldArray.myWelMSline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelMSline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelMS_num == 3) {
            ent = database.HandleToObject(myWeldArray.myWelMSline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelMSline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelMSline_3.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }

        if (myWeldArray.myWelMX_num == 1) {
            ent = database.HandleToObject(myWeldArray.myWelMXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelMX_num == 2) {
            ent = database.HandleToObject(myWeldArray.myWelMXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelMXline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelMX_num == 3) {
            ent = database.HandleToObject(myWeldArray.myWelMXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelMXline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelMXline_3.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }

        if (myWeldArray.myWelMid_num == 1) {
            ent = database.HandleToObject(myWeldArray.myWelMidline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelMid_num == 2) {
            ent = database.HandleToObject(myWeldArray.myWelMidline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelMidline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }

        if (myWeldArray.myWelUP_num == 1) {
            ent = database.HandleToObject(myWeldArray.myWelUPline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelUP_num == 2) {
            ent = database.HandleToObject(myWeldArray.myWelUPline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelUPline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelUP_num == 3) {
            ent = database.HandleToObject(myWeldArray.myWelUPline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelUPline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelUPline_3.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }

        if (myWeldArray.myWelXX_num == 1) {
            ent = database.HandleToObject(myWeldArray.myWelXXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelXX_num == 2) {
            ent = database.HandleToObject(myWeldArray.myWelXXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelXX_num == 3) {
            ent = database.HandleToObject(myWeldArray.myWelXXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_3.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelXX_num == 4) {
            ent = database.HandleToObject(myWeldArray.myWelXXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_3.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_4.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelXX_num == 5) {
            ent = database.HandleToObject(myWeldArray.myWelXXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_3.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_4.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_5.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelXX_num == 6) {
            ent = database.HandleToObject(myWeldArray.myWelXXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_3.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_4.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_5.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_6.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelXX_num == 7) {
            ent = database.HandleToObject(myWeldArray.myWelXXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_3.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_4.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_5.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_6.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_7.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelXX_num == 8) {
            ent = database.HandleToObject(myWeldArray.myWelXXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_3.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_4.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_5.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_6.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_7.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_8.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelXX_num == 9) {
            ent = database.HandleToObject(myWeldArray.myWelXXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_3.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_4.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_5.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_6.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_7.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_8.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_9.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelXX_num == 10) {
            ent = database.HandleToObject(myWeldArray.myWelXXline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_3.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_4.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_5.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_6.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_7.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_8.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_9.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelXXline_10.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }

        if (myWeldArray.myWelDOWN_num == 1) {
            ent = database.HandleToObject(myWeldArray.myWelDOWNline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelDOWN_num == 2) {
            ent = database.HandleToObject(myWeldArray.myWelDOWNline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelDOWNline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
        else if (myWeldArray.myWelDOWN_num == 3) {
            ent = database.HandleToObject(myWeldArray.myWelDOWNline_1.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelDOWNline_2.myPolyLineObjectID);
            ent.ColorIndex = myColorID
            ent = database.HandleToObject(myWeldArray.myWelDOWNline_3.myPolyLineObjectID);
            ent.ColorIndex = myColorID
        }
}


//角焊缝
function JiaoWeldAdd() {
    // 创建一个与用户交互取点的对象。
    var getPt = mxOcx.NewComObject("IMxDrawUiPrPoint");
    getPt.message = "输入标注插入基点";
    // 设置动态绘制参数.
    var spDrawData = getPt.InitUserDraw("DrawJiaoWeld");
    // 开始取第一个点。
    if (getPt.go() != 1)
        return;
    // 创建一个与用户交互取点的对象。
    var getSecondPt = mxOcx.NewComObject("IMxDrawUiPrPoint");

    getSecondPt.message = "输入标注位置点";
    getSecondPt.basePoint = getPt.value();

    getSecondPt.setUseBasePt(false);

    spDrawData = getSecondPt.InitUserDraw("DrawJiaoWeld2");
    // 设置动态绘制参数.
    spDrawData.SetPoint("BasePoint", getPt.value());
    // 开始取第二个点。
    if (getSecondPt.go() != 1)
        return;


    var ret = spDrawData.Draw();

    // 设置绘制的批注文字样式。
    //for (var i = 0; i < ret.Count; i++) {
    //    var ent = ret.AtObject(i);
    //    var hd = ent.handle;
    //    var type = ent.ObjectName;
    //}
    var ent = ret.AtObject(0);
    var hd = ent.handle;
    return hd;
}
// 拖放动态绘制函数。
function DoDynWorldDrawFun(dX, dY, pWorldDraw, pData) {
    var sGuid = pData.Guid;
    mxOcx.SetEventRet(0);
    // 当前拖放位置点.
    var curPt = mxOcx.NewPoint();
    curPt.x = dX;
    curPt.y = dY;
    if (sGuid == "DrawJiaoWeld") {
        DynWorldDrawJiaoWeld(pData, pWorldDraw, curPt);
        mxOcx.SetEventRet(1);
    }
    else if (sGuid == "DrawJiaoWeld2") {
        DynWorldDrawJiaoWeld2(pData, pWorldDraw, curPt);
        mxOcx.SetEventRet(1);
    }  
}
//动态绘制角
function DynWorldDrawJiaoWeld(pCustomEntity,
                 pWorldDraw, curPt) {
    // 得到绘制参数.
    var pt1 = curPt;
    var pt2 = mxOcx.NewPoint();
    pt2.x = curPt.x + 5;
    pt2.y = curPt.y
    // 创建一个批注对象.
    var pl = mxOcx.NewEntity("IMxDrawLine");
    pl.StartPoint = pt1;
    pl.EndPoint = pt2;
    //画三角形
    var vec = mxOcx.NewComObject("IMxDrawVector3d");
    vec.MakeYAxis();
    vec.Mult(1.5);
    var ptJ1 = mxOcx.NewPoint();
    ptJ1.x = curPt.x + 1.5;
    ptJ1.y = curPt.y;
    var ptJ2 = mxOcx.NewPoint();
    ptJ2.x = ptJ1.x;
    ptJ2.y = ptJ1.y;
    ptJ2.Add(vec);
    vec.MakeXAxis();
    vec.Mult(1.5);
    var ptJ3 = mxOcx.NewPoint();
    ptJ3.x = ptJ1.x;
    ptJ3.y = ptJ1.y;
    ptJ3.Add(vec);
    // 动态绘制.
    pWorldDraw.DrawEntity(pl);
    pWorldDraw.DrawLine(ptJ1.x, ptJ1.y, ptJ2.x, ptJ2.y);
    pWorldDraw.DrawLine(ptJ2.x, ptJ2.y, ptJ3.x, ptJ3.y);
}
//动态绘制角
function DynWorldDrawJiaoWeld2(pCustomEntity,
                 pWorldDraw, curPt) {
    var basePoint = pCustomEntity.GetPoint("BasePoint");
    var pt2 = mxOcx.NewPoint();
    var pt1 = mxOcx.NewPoint();
    if (basePoint.x < curPt.x) {
        // 得到绘制参数.
        pt1 = curPt;
        pt2.x = curPt.x + 5;
        pt2.y = curPt.y
        //画三角形
        var vec = mxOcx.NewComObject("IMxDrawVector3d");
        vec.MakeYAxis();
        vec.Mult(1.5);
        var ptJ1 = mxOcx.NewPoint();
        ptJ1.x = curPt.x + 1.5;
        ptJ1.y = curPt.y;
        var ptJ2 = mxOcx.NewPoint();
        ptJ2.x = ptJ1.x;
        ptJ2.y = ptJ1.y;
        ptJ2.Add(vec);
        vec.MakeXAxis();
        vec.Mult(1.5);
        var ptJ3 = mxOcx.NewPoint();
        ptJ3.x = ptJ1.x;
        ptJ3.y = ptJ1.y;
        ptJ3.Add(vec);

    }
    else {
        pt1 = curPt;
        pt2.x = curPt.x - 5;
        pt2.y = curPt.y
        //画三角形
        var vec = mxOcx.NewComObject("IMxDrawVector3d");
        vec.MakeYAxis();
        vec.Mult(1.5);
        var ptJ1 = mxOcx.NewPoint();
        ptJ1.x = curPt.x - 3;
        ptJ1.y = curPt.y;
        var ptJ2 = mxOcx.NewPoint();
        ptJ2.x = ptJ1.x;
        ptJ2.y = ptJ1.y;
        ptJ2.Add(vec);
        vec.MakeXAxis();
        vec.Mult(1.5);
        var ptJ3 = mxOcx.NewPoint();
        ptJ3.x = ptJ1.x;
        ptJ3.y = ptJ1.y;
        ptJ3.Add(vec);
    }
    // 创建一个批注对象.
    var pl = mxOcx.NewEntity("IMxDrawLine");
    pl.StartPoint = pt1;
    pl.EndPoint = pt2;
    var pl2 = mxOcx.NewEntity("IMxDrawLine");
    pl2.StartPoint = basePoint;
    pl2.EndPoint = pt1;

    // 动态绘制.
    pWorldDraw.DrawEntity(pl);
    pWorldDraw.DrawEntity(pl2);
    pWorldDraw.DrawLine(ptJ1.x, ptJ1.y, ptJ2.x, ptJ2.y);
    pWorldDraw.DrawLine(ptJ2.x, ptJ2.y, ptJ3.x, ptJ3.y);
}
// 控件鼠标事件
function MouseEvent(dX, dY, lType) {
    if (lType == 4) {

        // dTol是搜索范围，50是屏幕像素.
        var dTol = 50;

        // 临时修改CursorWidth的大小，用FindEntAtPoint函数确定搜索范围.
        var bak = mxOcx.CursorWidth;
        mxOcx.CursorWidth = dTol;

        // 搜索过滤条件，只选择图片.
        var filter = mxOcx.NewResbuf();

        // MxImageMark是图片对象的DXF组码名称，5020在DXF组码，代表MxImageMark是对象类型.
        filter.AddStringEx("MxDrawXCustomEntity", 5020);

        // 查找鼠标点击的实体。
        var ent = mxOcx.FindEntAtPoint(dX, dY, filter);

        // 恢复光标拾取框的大小.
        mxOcx.CursorWidth = bak;
        if (ent != null) {

            var param = mxOcx.NewResbuf();
            param.AddString(ent.GetString("Text"));
            param.AddDouble(ent.GetDouble("TextHeight"));
            param.AddLong(ent.colorIndex);

            var ret = mxOcx.CallEx("Mx_ShowMTextDialog", param);

            if (ret.AtString(0) != "Ok") {
                return;
            }


            var txt = ret.AtString(1);
            var txtH = ret.AtDouble(2);
            var txtColorIndex = ret.AtLong(3);

            ent.SetString("Text", txt);
            ent.SetDouble("TextHeight", txtH);
            ent.colorIndex = txtColorIndex;

            mxOcx.SendStringToExecute("");
            mxOcx.SetEventRet(1);
            mxOcx.UpdateDisplay();

        }






    }
}
// 移动自定义实体夹点
function MoveGripPointsFun(pCustomEntity, lGridIndex, dOffsetX, dOffsetY) {

    var sGuid = pCustomEntity.Guid;
    if (sGuid == "TestMxCustomEntity") {
        if (!pCustomEntity.IsHave("First"))
            return;


        var stp = pCustomEntity.GetPoint("First");

        var ept = pCustomEntity.GetPoint("BasePoint");

        var dimpt = pCustomEntity.GetPoint("DimPoint");



        if (lGridIndex == 0) {
            stp.x = stp.x + dOffsetX;
            stp.y = stp.y + dOffsetY;
            pCustomEntity.SetPoint("First", stp);
        }
        else if (lGridIndex == 1) {
            ept.x = ept.x + dOffsetX;
            ept.y = ept.y + dOffsetY;
            pCustomEntity.SetPoint("BasePoint", ept);
        }
        else {
            dimpt.x = dimpt.x + dOffsetX;
            dimpt.y = dimpt.y + dOffsetY;
            pCustomEntity.SetPoint("DimPoint", dimpt);
        }

        mxOcx.SetEventRet(1);
    }
}
// 返回自定义实体夹点
function GetGripPointsFun(pCustomEntity) {

    var sGuid = pCustomEntity.Guid;
    if (sGuid == "TestMxCustomEntity") {
        if (!pCustomEntity.IsHave("First"))
            return;

        var stp = pCustomEntity.GetPoint("First");

        var ept = pCustomEntity.GetPoint("BasePoint");

        var dimpt = pCustomEntity.GetPoint("DimPoint");


        var ret = mxOcx.NewResbuf();

        ret.AddPoint(stp);
        ret.AddPoint(ept);
        ret.AddPoint(dimpt);

        mxOcx.SetEventRetEx(ret);
    }
}
// 自定义实体绘制函数
function ExplodeFun(pCustomEntity, pWorldDraw, txt) {


    var sGuid = pCustomEntity.Guid;
    if (sGuid == "TestMxCustomEntity") {
        if (!pCustomEntity.IsHave("First"))
            return;

        var stp = pCustomEntity.GetPoint("First");

        var ept = pCustomEntity.GetPoint("BasePoint");

        var dimpt = pCustomEntity.GetPoint("DimPoint");

        var txt = pCustomEntity.GetString("Text");

        var textH = pCustomEntity.GetDouble("TextHeight");

        var edgeNum = pCustomEntity.GetLong("EdgeNumber");

        var shapRadius = pCustomEntity.GetDouble("ShapRadius");

        var isCircle = pCustomEntity.GetLong("isCircle");


        var comment = mxOcx.NewEntity("IMxDrawComment");
        comment.Text = txt;
        comment.TextHeight = textH;
        comment.EdgeNumber = edgeNum;
        comment.ShapRadius = shapRadius;
        comment.basePoint = ept;
        comment.Position = dimpt;
        pWorldDraw.TextStyle = "MyCommentFont";

        // 动态绘制.
        pWorldDraw.DrawEntity(comment);

        // 绘制矩形框.
        if (isCircle) {

            var dR = stp.DistanceTo(ept) * 0.5;

            var vec = stp.SumVector(ept);
            vec.Mult(0.5);

            ept.Add(vec);

            pWorldDraw.DrawCircle(ept.x, ept.y, dR);
        }
        else {
            pWorldDraw.DrawLine(stp.x, stp.y, stp.x, ept.y);
            pWorldDraw.DrawLine(stp.x, ept.y, ept.x, ept.y);
            pWorldDraw.DrawLine(ept.x, ept.y, ept.x, stp.y);
            pWorldDraw.DrawLine(ept.x, stp.y, stp.x, stp.y);
        }
        mxOcx.SetEventRet(1);

    }
}
document.getElementById("MxDrawXCtrl").ImplementCommandEventFun = DoCommandEventFunc;
document.getElementById("MxDrawXCtrl").ImpDynWorldDrawFun = DoDynWorldDrawFun;
document.getElementById("MxDrawXCtrl").ImpExplodeFun = ExplodeFun;
document.getElementById("MxDrawXCtrl").ImpGetGripPointsFun = GetGripPointsFun;
document.getElementById("MxDrawXCtrl").ImpMoveGripPointsAtFun = MoveGripPointsFun;
document.getElementById("MxDrawXCtrl").ImplementMouseEventFun = MouseEvent;