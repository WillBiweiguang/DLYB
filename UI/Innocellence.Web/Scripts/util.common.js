function reinitMainPop(obj) {
    LEAP.Common.MainPop = $(obj).formPopup();
}
function clearDatatableSetting() {
    if (LEAP.Common.MainPop.options.dataTable) {
        LEAP.Common.MainPop.options.dataTable.fnDestroy(false);
        LEAP.Common.MainPop.options.dataTable = undefined;
    }
    return {
        "serverSide": true,
        "Paginat": true,
        bAutoWidth: false,
        "bLengthChange": true,
        "searching": false,
        // "sDom": "lfrtip",
        "ordering": false,
        "ajax": {
            "url": LEAP.Common.Controller + "/GetList",
            "type": 'post',
            "data": function (d) {
                ajaxData(d, "SearchForm", LEAP.Common.MainPop == null ? null : LEAP.Common.MainPop.options.dataTable);

            },
            error: function (xhr, error, thrown) {
                artDialog.alert("系统错误! 代码:" + error + " 消息:" + thrown);
            }
        },

        "columnDefs": [
            //    {
            //    "targets": 1,
            //    "render": function (data, type, full, meta) {
            //        return '<a href="#" onclick="LEAP.Common.MainPop.ShowUpdateInfo(' + full.Id + ');return false;" class="artDailog">' + data + '</a>';
            //    }
            //},
            {
                "targets": 0,
                "render": function (data, type, full, meta) {
                    return '<input type="checkbox" value="' + data.Id + '" title="' + data + '" id="checkbox" />';
                }
            },
        ]
    };
}