//uploader;
var uploader = WebUploader.create({

    // 选完文件后，是否自动上传。
    auto: true,

    // swf文件路径
    swf: '../scripts/webuploader/Uploader.swf',

    // 文件接收服务端。
    server: '/Common/PostQuestionImage',
    // 选择文件的按钮。可选。
    // 内部根据当前运行是创建，可能是input元素，也可能是flash.
    pick: {
        id: '#filePicker',
        multiple: false
    },
    // 只允许选择图片文件。
    accept: {
        title: 'Images',
        extensions: 'gif,jpg,jpeg,bmp,png,GIF,JPG,JPEG,BMP,PNG',
        mimeTypes: 'image/*'
    },
    sendAsBinary: true
});


var $list = $('#fileList');
var imglist = '';
$list.html('');

// 当有文件添加进来的时候
uploader.on('fileQueued', function (file) {
    var $li = $(
            '<div id="' + file.id + '" class="file-item thumbnail col-xs-4">' +
            '<img id="preview_img' + file.id + '" style="height:60px">' +
            '</div>'
        ),
        $btns = $('<div class="file-panel" >' +
                '<span >删除</span>').appendTo($li),
        $img = $li.find('img');

   
    // $list为容器jQuery实例
    $list.append($li);
    var thumbnailWidth = "100px";
    var thumbnailHeight = "100px";
    // 创建缩略图
    // 如果为非图片文件，可以不用调用此方法。
    // thumbnailWidth x thumbnailHeight 为 100 x 100
    uploader.makeThumb(file, function (error, src) {
        if (error) {
            $img.replaceWith('<span>不能预览</span>');
            return;
        }
        $img.attr('src', src);
    }, thumbnailWidth, thumbnailHeight);
    $btns.on('click', 'span', function () {
        var index = $(this).index();
        switch (index) {
            case 0:
                uploader.removeFile(file);
                return;
        }
    });
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

    $li.find('p.state').text('上传中');

    $percent.css('width', percentage * 100 + '%');

});

// 文件上传成功，给item添加成功class, 用样式标记上传成功。
uploader.on('uploadSuccess', function (file, response) {
    //序列化服务器端的数据
    var data = JSON.parse(response._raw);
    if (data.result != "" || data.result != null) {
        var result = $('#preview_img' + file.id).prop('src', "/Common/ThumbQuestionFile?id=" + data.id + "&filename=" + data.result.Src);
        $('#preview_img' + file.id).attr('data-value',data.id),
        imglist = imglist + data.id + ",";
        $('#ImageIdList').val(imglist);
        result.load(function () {
            $('#' + file.id).find('.progress').remove();
        });
    }
    $('#' + file.id).addClass('upload-state-done');
    // $('#' + file.id).text('上传成功');
});

uploader.on('fileDequeued', function(file) {
    removeFile(file);
});

// 负责view的销毁
function removeFile(file) {
    var $li = $('#' + file.id);
    var id = ($li).find('img').data('value');
    imglist = imglist.replace(id+',', '');
    $('#ImageIdList').val(imglist);
    $li.off().find('.file-panel').off().end().remove();
    
}

// 文件上传失败，显示上传出错。
uploader.on('uploadError', function (file) {
    var $li = $('#' + file.id),
        $error = $li.find('div.error');

    // 避免重复创建
    if (!$error.length) {
        $error = $('<div class="error"></div>').appendTo($li);
    }

    $error.text('上传失败');
});

// 完成上传完了，成功或者失败，先删除进度条。
uploader.on('uploadComplete', function (file) {

});
