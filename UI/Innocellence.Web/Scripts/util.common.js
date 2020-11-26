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
function getByteLen(val) {
    var len = 0;
    for (var i = 0; i < val.length; i++) {
        var length = val.charCodeAt(i);
        if (length >= 0 && length <= 128) {
            len += 1;
        }
        else {
            len += 2;
        }
    }
    return len;
}
function initWebuploaderEvent(uploader) {
    $('.webuploader-pick').addClass('btn btn-default btn-sm');
    $('.webuploader-pick').on('click', function () { $('.webuploader-element-invisible').click(); });
    //uploader.refresh();
    uploader.on('fileQueued', function (file) {
        var filename = getByteLen(file.name) > 50 ? file.name.substring(0, 27) + '...' : file.name;
        var $li = $('<div id="' + file.id + '" class="item">' +
            '<span class="info">' + filename + '</span>' +
            '<span class="remove-this glyphicon glyphicon-remove"></span>' +
            '<span class="state">等待上传...</p>' +
            '</div>');
        $list.append($li);
        $li.on('click', '.remove-this', function () {
            uploader.removeFile(file, true);
        });
    });
    uploader.on('fileDequeued', function (file) {
        var $li = $('#' + file.id);
        $li.remove();
    });
    // 文件上传过程中创建进度条实时显示。
    uploader.on('uploadProgress', function (file, percentage) {
        var $li = $('#' + file.id),
            $percent = $li.find('.progress .progress-bar');

        // 避免重复创建
        if (!$percent.length) {
            $percent = $('<div class="progress progress-striped active">' +
                '<div class="progress-bar" role="progressbar" style="width: 0%">' +
                '</div>' +
                '</div>').appendTo($li).find('.progress-bar');
        }

        $li.find('span.state').text('上传中');

        $percent.css('width', percentage * 100 + '%');
    });
    uploader.on('uploadSuccess', function (file, r) {
        if (r.Message.Status == 200) {
            $('#' + file.id).find('span.state').text('已上传');
        } else {
            $('#' + file.id).find('span.state').text(r.Message.Text);
        }
    });

    uploader.on('uploadError', function (file, response) {
        $('#' + file.id).find('span.state').text('上传出错');
        //uploader.removeFile(uploader.getFile(file.id));
    });

    uploader.on('uploadComplete', function (file, response) {
        $('#' + file.id).find('.progress').fadeOut();
    });

    uploader.on('all', function (type) {
        if (type === 'startUpload') {
            state = 'uploading';
        } else if (type === 'stopUpload') {
            state = 'paused';
        } else if (type === 'uploadFinished') {
            state = 'done';
            LEAP.Common.MainPop.options.dataTable.fnDraw(false);
            $btn.text('完成');
            uploader.reset();
            return;
        }

        if (state === 'uploading') {
            $btn.text('暂停上传');
        } else {
            $btn.text('开始上传');
        }
    });

    $btn.on('click', function () {
        if (state === 'uploading') {
            uploader.stop();
        } else {
            uploader.upload();
        }
        if ($btn.text() == '完成') {
            $('.close').click();
            $list.children().remove();
        }
    });
    $('.close').on('click', function () {
        uploader.reset();
        $list.children().remove();
    });
}