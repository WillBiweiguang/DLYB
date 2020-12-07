var LEAP = {};
LEAP.Common = {};
LEAP.Common.Controller = '.';

var datatableSetting = {
    "scrollY": '350px', //支持垂直滚动
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

//支持一个页面多个table绑定
function ajaxData(d, SearchForm, dTable) {
    // search condition
    var dSearch = $("#" + SearchForm).serializeArray();
    // jQuery.merge(d, dSearch);
    $.each(dSearch, function (key, val) {
        d[val.name] = val.value;
    });

    if (dTable == undefined || dTable == null) {
        d.iRecordsTotal = 0;
    } else {
        d.iRecordsTotal = dTable.fnSettings().fnRecordsTotal();
    }

    if (window.location.href.indexOf("?") > 0) {
        var arr = window.location.search.slice(1).replace(/\+/g, ' ').split('&');
        var result = undefined;
        for (var i = 0; i < arr.length; i++) {
            var arrHref = arr[i].split('=');
            if (arrHref[0] != '') {
                d[arrHref[0].toUpperCase()] = arrHref[1];
            }
        }
    }
}

function htmlencode(s) {
    var div = document.createElement('div');
    div.appendChild(document.createTextNode(s));
    return div.innerHTML;
}
function htmldecode(s) {
    var div = document.createElement('div');
    div.innerHTML = s;
    return div.innerText || div.textContent;
}



$(document).ready(function () {


    //$("span.icon input:checkbox, th input:checkbox").click(function() {
    //	var checkedStatus = this.checked;
    //	var checkbox = $(this).parents('.widget-box').find('tr td:first-child input:checkbox');		
    //	checkbox.each(function() {
    //		this.checked = checkedStatus;
    //		if (checkedStatus == this.checked) {
    //			$(this).closest('.checker > span').removeClass('checked');
    //		}
    //		if (this.checked) {
    //			$(this).closest('.checker > span').addClass('checked');
    //		}
    //	});
    //});

    //$('.modal select[data-ajax--url]').each(function () {
    //    var url = $(this).attr('data-ajax--url');
    //    $(this).removeAttr('data-ajax--url');
    //    $(this).prop('data-url', url);
    //});

    //fix a bug when a select2 in modal(search input cannot focus)
    $.fn.modal.Constructor.prototype.enforceFocus = function () { };

    //  $('select[class!=hidden]').not('[class*=easyui]').select2({ minimumResultsForSearch: Infinity });

    $('[data-rel=tooltip]').tooltip();

    $(document).on('click', 'th input:checkbox', function () {
        var that = this;
        $(this).closest('table').find('tr > td:first-child input:checkbox')
			.each(function () {
			    this.checked = that.checked;
			    $(this).closest('tr').toggleClass('selected');
			});
    });

    $('[data-rel="tooltip"]').tooltip({
        placement: tooltip_placement
    });

    function tooltip_placement(context, source) {
        var $source = $(source);
        var $parent = $source.closest('table');
        var off1 = $parent.offset();
        var w1 = $parent.width();

        var off2 = $source.offset();
        //var w2 = $source.width();

        if (parseInt(off2.left) < parseInt(off1.left) + parseInt(w1 / 2)) return 'right';
        return 'left';
    }

    $.metadata.setType("attr", "validate");

    LEAP.Common.MainPop = $("#ModalTable").formPopup();

    $('#btnAdd').on('click', function () {
        return LEAP.Common.MainPop.TableButtonClick(0, 0);
    });

    $('#btnUpdate').on('click', function () {
        return LEAP.Common.MainPop.TableButtonClick(0, 1);
    });

    $('#btnDelete').on('click', function () {
        return LEAP.Common.MainPop.TableButtonClick(0, 2);
    });

    $('#btnSearch').on('click', function () {
        if (!BeforeSearch()) {
            return;
        }
        return LEAP.Common.MainPop.TableSearchClick();
    });
    $("#btnPrevious").click(function () {
        var id = $("#PID").val();
        var isopen = $("#isopen").val();
        var isnew = $("#isnew").val();
        var preurl = $("#preurl").val();
        document.getElementById("btnPrevious").href = "Previous?preurl=" + preurl + "&pid=" + id + "&isopen=" + isopen + "&isnew=" +isnew;

    });
    $("#btnNext").click(function () {
        var id = $("#PID").val();
        var isopen = $("#isopen").val();
        var isnew = $("#isnew").val();
        var nexturl = $("#nexturl").val();
        document.getElementById("btnNext").href = "Next?nexturl=" +nexturl + "&pid=" + id + "&isopen=" +isopen + "&isnew=" +isnew;

    });
    $('#btnExport').on('click', function () {
        var para = '';
        var dSearch = $("#SearchForm").serializeArray();
        if (!BeforeExport(dSearch)) {
            return false;
        }

        $.each(dSearch, function (key, val) {
            para += val.name + '=' + val.value + '&';
        });

        $.download("Export", para + "t=" + (new Date()).getTime());
        return true;
    });


    //$('#btnExport3rd').on('click', function () {
    //    var para = '';
    //    var dSearch = $("#SearchForm").serializeArray();
    //    if (!BeforeExport(dSearch)) {
    //        return false;
    //    }

    //    $.each(dSearch, function (key, val) {
    //        para += val.name + '=' + val.value + '&';
    //    });

    //    $.download("Export3rd", para + "t=" + (new Date()).getTime());
    //    return true;
    //});

    $('#btnPublish').on('click', function () {
        //点击批量publish 获取当前所有选择的列 
        $('#nestable .dd-list').html("");
        var tableId = LEAP.Common.MainPop.options.dataTable;
        var selectedRows = GetSelectedRows(tableId, null);

        if (selectedRows.length <= 0) {
            artDialog.alert("请至少选择一条数据.");
            return false;
        }

        if (selectedRows.length > 10) {
            artDialog.alert("不能同时推送超过10条数据.");
            return false;
        }

        for (var i = 0; i < selectedRows.length; i++) {
            if (selectedRows[i].ArticleStatus != "Saved") {
                artDialog.alert("不能选择已经发布的图文<br/>如仍需选择，请先取消发布。");
                return false;
            }
            $('#nestable .dd-list').append('<li class="dd-item item-blue2" data-id="' + selectedRows[i].Id + '"><div class="dd-handle">' + selectedRows[i].ArticleTitle + '</div></li>');
        }
    });

    $('#SearchForm').on('keydown', function (event) {
        if (event.which == 13) {
            event.stopPropagation();
            $('#btnSearch').trigger('click');
        }
    });
    $("#btnExit").click(function () {
        var d = dialog({
            title: '提示',
            content: "您确定退出吗？请点击保存。",
            okValue: '确定',
            ok: function () {
            },
            cancelValue: '取消',
            cancel: function () {
            }
        });
        d.show();
    });
    //resize和scroll事件一定要优化
    $.resizeWaiter = false;
    $(window).resize(function () {
        if (!$.resizeWaiter) {
            $.resizeWaiter = true;
            setTimeout(function () {
                LEAP.Common.MainPop.options.dataTable && LEAP.Common.MainPop.options.dataTable.fnDraw(false);
                $.resizeWaiter = false;
            }, 300);
        }
    });
});

function BeforeSearch() {
    return true;
}

function BeforeExport(objForms) {
    return true;
}

function GetSelectedRows(tableId, id) {

    var aReturn = new Array();
    var i = 0;
    var iIndex = 0;
    var checkbox = $(tableId).find('tr td:first-child input:checkbox');
    checkbox.each(function () {

        if (id != null) {
            if (tableId.fnSettings().aoData[iIndex]._aData.Id == id) {
                aReturn[0] = tableId.fnSettings().aoData[iIndex]._aData;
                return aReturn;
            }
        } else if (this.checked) {
            aReturn[i++] = tableId.fnSettings().aoData[iIndex]._aData;
        }
        iIndex++;

    });

    return aReturn;
}

function ChangeStatus(id, appid, obj, url, row) {

    var bol = false;

    if ($(obj).prev('span').html() == "已发布") {
        bol = true;
    }

    var d = dialog({
        title: '消息',
        content: '<p>确定要' + (bol ? '取消 ' : '') + '发布?</p>' + (bol ? '' : '<label for="ispush"><input type="checkbox" name="ispush" id="ispush" style="margin-top:2px;margin-right: 4px;display: inline-block;"/><span>向微信用户推送通知</span></label>'),
        okValue: '确认',
        ok: function () {
            var ispush;
            ispush = $('input[name="ispush"]').is(":checked") ? true : false;

            $.ajax({
                type: 'Get',
                url: url + '?Id=' + id + '&ispush=' + ispush + '&appid=' + appid,
                cache: false
            }).done(function (data) {
                if (data.Status) {
                    switch (data.Status) {
                        case 200:
                            artDialog.alert("成功.");
                            HandlerPage($(obj));
                            break;
                        case 1:
                            OpenCheckErrorDialog(data, id, obj);
                            break;
                        default:
                            artDialog.alert(data.Message);
                    }
                } else {
                    artDialog.alert(data.Message.Text);
                    HandlerPage($(obj));
                }

            });
            return true;
        },
        cancelValue: '取消',
        cancel: function () { }
    });
    d.showModal();

}

function OpenCheckErrorDialog(response, contentid, dom) {
    console.log(response.Data + ',' + contentid);

    var d = dialog({
        title: '消息',
        content: response.Message,
        okValue: '继续',
        ok: function () {
            $.ajax({
                type: 'Get',
                url: 'PushMessage' + '?historyId=' + response.Data + '&contentId=' + contentid,
                cache: false
            }).done(function (data) {
                switch (data.Status) {
                    case 200:
                        HandlerPage($(dom));
                        break;
                    default:
                        artDialog.alert(data.Message);
                }

            });
            return true;
        },
        cancelValue: '取消',
        cancel: function () { }
    });
    d.showModal();
}

function HandlerPage($obj) {
    $obj.toggleClass('btn-success');
    $obj.toggleClass('btn-danger');

    if ($obj.html() == '<i class="fa fa-cloud-download"></i>') {
        $obj.html('<i class="fa fa-cloud-upload"></i>');
        $obj.prev().html('已保存');
    } else {
        $obj.html('<i class="fa fa-cloud-download"></i>');
        $obj.prev().html('已发布');
    }
    if (LEAP.Common.MainPop && LEAP.Common.MainPop.options.dataTable) {
        LEAP.Common.MainPop.options.dataTable.fnDraw(true);
    }
}