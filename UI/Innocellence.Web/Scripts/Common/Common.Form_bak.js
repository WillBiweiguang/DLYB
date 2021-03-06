/* 
* FormPopup 0.1 
* Copyright (c) 2015 hy 
* Date: 2015-04-03 
* 使用 formPopup弹出编辑窗口，需引用jquery.datatables.js,artdailog.js
*/
(function ($) {

    $.fn.formPopup = function (_options) {
        var defaults = {
            editForm: "DetailEdit",
            dataTable: null,
            formAction: "Get",   //form action url
            openType: 1,    //1 open with id 2 open with other url(ajax)
            afterShowModal: function () { },     //excute after show modal
            fnAfterSuccess:function(){},
            closeModal: _CloseModal,             //excute when close modal
            formatBindEntity: function (o) { },  //实体绑定前格式化
            BindFormData: BrowerInfoBind,
            afterBindData: function (o) { },      //实体绑定后执行

            validateSetting: {},                  //jquery validate rules
            editors:[]                            //ckeditor控件的id
        };


        if (_options == undefined || _options == null) {
            _options = {};
        }

        if (_options.editForm == undefined || _options.editForm == null) {
            _options.editForm = this;
        }

        this.options = $.extend({}, defaults, _options);

        var options = this.options;
       // var dTable = options.dataTable;

        var validator=  this.validate($.extend({
            errorPlacement: function (error, element) {
                if (element.parent().hasClass("input-prepend")) {
                    error.appendTo(element.parent().next('.errorMsg'));
                } else {
                    error.appendTo(element.parent());
                }
            },
            ignore: '',
            //submitHandler: function (form) {
            //    //if()
            //    this.AddOrUpdateInfo();
            //    //$(form).formPopup();
            //},
            invalidHandler: function (form, validator) {  //验证不通过回调 
                return false;
            }
        }, options.validateSetting));


        this.submit(function() {
          var bool=  AddInfoPost(this.action == "#" ? "Post" : this.action,this);
          return false;
        });
        // var strAction='';
        // var dTable;

        //绑定修改显示详细信息的方法
        this.ShowUpdateInfo = function (KeyID) {

            // $("#NewsEdit").dialog({ modal: true, width: 400,height:400 });
            var strTitle = $('.menu .current a').text();
            if (strTitle == '') { strTitle = '增加/编辑'; }

            var content;
            var opID;
            if (options.openType==1){
                var editForm = options.editForm;
                ClearForm($(editForm));
                $(editForm).find('label.error').css("display", "none");
                $(editForm).find("#ID").val(KeyID);
                content = $(editForm).parent().parent();
                opID=content.id;

            } else { //ajax form data
                content = $.ajax({
                    'url': options.formAction,
                    'data': { id: KeyID },
                    'cache': false,
                    'async': false,
                    'dataType': 'html',
                    success: function (data, textStatus, jqXHR) {
                        content = data;
                    }
                });

            }

            var d = dialog({
                title: strTitle,
                id: '_DetailEditPop' + opID,
                content: content,
                onclose: options.closeModal
            });

            d.showModal();

                //bootbox.dialog({
                //    message: content,
                //    title: strTitle,
                //    callback: function (result) {
                //        alert(result);
                //    }
                //});





            if (KeyID != null && KeyID != 0) {
                options.BindFormData(KeyID);
            } else {
                options.afterBindData(null);
                InitAllCKEditor();
            }

            options.afterShowModal();
        }

        function _CloseModal() {
            if (typeof CKEDITOR == undefined) { return; }
            $.each(options.editors, function (i,objID) {
                eval('var _editor=CKEDITOR.instances.' + objID);
                if (typeof (_editor) != 'undefined' && _editor) { _editor.destroy(); _editor = null; }
            });
            return true;
        }



        //绑定浏览详细信息的方法
        function BrowerInfoBind(KeyID) {
            _DataBindInit({ Id: KeyID });
        }

        //画面数据绑定
        function _DataBindInit(para) {
            var strUrl = $(options.editForm).attr("DataSource");
            if (strUrl == null || strUrl == "") { strUrl = options.formAction; }

            para.t = (new Date).getTime();

            $.getJSON(strUrl, para, function (permissionInfo) {

                if (permissionInfo == null) {
                    artDialog.alert('请选择要编辑数据读取失败或者已经被删除！', "", function () {
                        dialog.get("_DetailEditPop").close().remove();
                    });
                    return null;
                }

                //实体格式化
                options.formatBindEntity(permissionInfo);

                var objBind = $(options.editForm).find("[data-bind]");

                //alert(objBind);
                objBind.each(function (i, item) {
                    var bindattr = $(item).attr("data-bind").split(':');
                    if (bindattr.length > 1) {

                        //alert("class:" + $(item).attr("class") + "  bind:" + bindattr[0]);

                        if ($(item).attr("type") == "radio") {

                            $('input[name="' + $(item).attr("name") + '"][value="' + eval("permissionInfo." + bindattr[1]) + '"]').prop("checked", true);

                        } else if ($(item).attr("type") == "checkbox") {
                            $(item).prop("checked", Boolean(Number(eval("permissionInfo." + bindattr[1]))));
                           // $(item).val('true');

                            //$(item).on("click", function () {
                               // if (this.checked) { this.value = true; } else { this.value = false; }
                            //});

                        } else if ($(item).attr("class") != null && $(item).attr("class").indexOf("easyui-numberbox") >= 0) {
                            var dValue = eval("permissionInfo." + bindattr[1]);
                            if (dValue != null) {
                                $(item).numberbox('setValue', dValue);
                            }
                        } else if ($(item).attr("class") != null && $(item).attr("class").indexOf("easyui-datebox") >= 0) {
                            var dValue = eval("permissionInfo." + bindattr[1]);
                            if (dValue != null) {
                                dValue = eval(dValue.replace(/\/Date\((\d+)\)\//gi, "new Date($1)")).pattern("yyyy-M-d h:m:s");
                                $(item).datebox('setValue', dValue);
                            }
                        } else if ($(item).attr("class") != null && $(item).attr("class").indexOf("easyui-combobox") >= 0 && bindattr[0] == "value") {
                            //alert(eval("permissionInfo." + bindattr[1]));
                            $(item).combobox('select', eval("permissionInfo." + bindattr[1]));
                        } else if ($(item).attr("class") != null && $(item).attr("class").indexOf("easyui-combotree") >= 0 && bindattr[0] == "value") {
                            //alert(eval("permissionInfo." + bindattr[1]));
                            var d = eval("permissionInfo." + bindattr[1]);
                            if (typeof d === 'object' ) {
                                $(item).combotree('setValues', d);
                            }else if ( typeof d === 'number') {
                                $(item).combotree('setValue', d);
                            } else {
                                $(item).combotree('setValues', d.split(','));
                            }

                        } else if (bindattr[0] == "value") {
                            $(item).val(eval("permissionInfo." + bindattr[1]));
                        } else if (bindattr[0] == "html") {
                            $(item).html(eval("permissionInfo." + bindattr[1]));
                        } else if (bindattr[0] == "text") {
                            $(item).text(eval("permissionInfo." + bindattr[1]));
                        } else if (bindattr[0].indexOf("css") == 0) { //绑定css时 使用cssWidth等名称
                            $(item).css(bindattr[0].replace("css", ""), eval("permissionInfo." + bindattr[1]));
                        } else {
                            $(item).attr(bindattr[0], eval("permissionInfo." + bindattr[1]));
                        }


                        if ($(item)[0].nodeName.toUpperCase() == "SELECT") {
                            $(item).trigger("onchange")
                        }
                        //alert( $(item).attr(bindattr[0]));
                    }
                });

                InitAllCKEditor();
                options.afterBindData(permissionInfo);
               

                //$(objForm).form("validate");
                return permissionInfo;
            });

            return null;
        }

        this.TableButtonClick = function (id, type, oTable) {

            var d;
            oTable = oTable == null ? options.dataTable : oTable;

            if (type == 0) {

                this.ShowUpdateInfo(0);

                //InitCKEditor();

            } else if (type == 1) {

                var oData = GetSelectedRows(oTable);

                if (oData.length == 1) {
                    this.ShowUpdateInfo(oData[0].Id);
                } else {
                    artDialog.alert('请选择要编辑的行或者选择的行超过1行！');

                }

            } else {
                var oData = GetSelectedRows(oTable);
                doDelete(oData, '1', oTable);
            }
            return false;

        }

        this.RowClick = function (id, type, oTable) {

            oTable = oTable == null ? options.dataTable : oTable;
            if (type == 0) {
                this.ShowUpdateInfo(0);

            } else if (type == 1) {
                this.ShowUpdateInfo(id);

            } else {
                var oData = GetSelectedRows(oTable, id);
                doDelete(oData, '1', oTable);
            }
            return false;
        }

        this.AddOrUpdateInfo=function (strAdd) {

            if (strAdd == undefined || strAdd == null) { strAdd = "Post";}
            AddInfoPost(strAdd,this);
            return true;
        }

      


        //实现添加
        function AddInfoPost(strUrl,oForm) {
            // $("#btnAddRole").click(function () {
            //判断用户的信息是否通过验证
            //var validate = $("#ff").form('validate');
            //if (validate == false) {
            //    return false;
            //}


            //validate
            if (!validator.form()) { return false; }

            //获取需要传递的参数传递给前台
            $(oForm).find('select:disabled').attr('readonly', 'readonly').attr('disabled', false);
            var postData = $(oForm).serializeArray();
            $(oForm).find('select[readonly="readonly"]').attr('disabled', true);
           // var chs = $(oForm).find("input[type='checkbox',type='radio']");


            var id = $(oForm).find("#ID").val();
            var isUpdate = (id == null || id == "" || id == '0') ? false : true;

            //发送异步请求到后台保存用户数据
            $.post(strUrl, postData, function (data) {
                if (ReturnValueFilter(data)) {
                    //添加成功  1.关闭弹出层，2.刷新DataGird
                    // alert("添加角色成功");


                    var dd = dialog({
                        title: '提示信息',
                        content: isUpdate ? '修改成功！' : '添加成功!',
                        cancel: false,
                        ok: function () {

                            dialog.getCurrent().close().remove();
                            options.fnAfterSuccess(oForm);
                            dialog.get("_DetailEditPop" + oForm.id).close().remove();
                            // $("#ff").form("clear");
                            ClearForm($(oForm));
                            //多个table不知道该刷新哪一个
                            //dTable.fnClearTable(0);
                            if (options.dataTable) {
                                options.dataTable.fnDraw(true);
                            }
                        }
                    });
                    dd.showModal();

                } else { return false;}
            });

            return true;
            // });
        }


        function ClearForm(oForm) {

            var objBind = $(oForm).find("[data-bind]");

            //alert(objBind);
            objBind.each(function (i, item) {

                var bindattr = $(item).attr("data-bind").split(':');
                if (bindattr.length > 1) {
                    if ($(item).attr("type") == "radio") {
                        $('input[name="' + $(item).attr("name") + '"]').prop("checked", false);
                    } else if ($(item).attr("type") == "checkbox") {
                        $(item).prop("checked", false);
                    } else if (bindattr[0] == "value") {
                        $(item).val('');
                    } else if (bindattr[0] == "html") {
                        $(item).html('');
                    } else if (bindattr[0] == "text") {
                        $(item).text('');
                    } else if (bindattr[0].indexOf("css") == 0) { //绑定css时 使用cssWidth等名称
                        $(item).attr(bindattr[0].replace("css", ""), '');
                    } else {
                        $(item).attr(bindattr[0], '');
                    }
                }
            });

            //Clear Process Bar and file

            var progressBars = $(oForm).find('.progress-bar');
            progressBars.each(function (i, item) {
                $(item).css('width', 0);
            });

            var files = $('input[type=file]');
            files.each(function (i, item) {
                $(item).val('');
            });
        }

        this.fnDelete = function (oDatas,strAction) {

            doDelete(oDatas,null,null, strAction);

        }


        //删除按钮点击
        //实现直接删除数据和伪删除数据的方法
        function doDelete(oDatas, not, oTable, strAction) {
            //得到用户选择的数据的ID




            //首先判断用户是否已经选择了需要删除的数据,然后循环将用户选择的数据传递到后台
            if (oDatas.length >= 1) {

                var postData = '';
                $.each(oDatas, function (o, i) { postData += this.Id + ","; });

                postData = postData.substring(0, postData.length - 1);

                var d = dialog({
                    title: 'message',
                    content: '确定要删除选择的数据吗？共' + oDatas.length + '条',
                    okValue: '确定',
                    ok: function () {
                        $.get(strAction == null ? "Delete" : strAction, { "sIds": postData }, function (data) {
                            if (ReturnValueFilter(data)) {

                                options.fnAfterSuccess(oDatas);

                                if (options.dataTable) {
                                    options.dataTable.fnSettings()._iRecordsTotal = 0;
                                    //友情提示用户删除成功，刷新表格
                                    options.dataTable.fnClearTable(0);
                                    options.dataTable.fnDraw();
                                }
                                return true;
                            }
                        });
                        return true;
                    },
                    cancelValue: '取消',
                    cancel: function () { }
                });
                d.showModal();

            }
            else {
                artDialog.alert("友情提示" + "请先选择要操作的行数据！");
            }
        }

      this.TableSearchClick=  function () {

            options.dataTable.fnSettings()._iRecordsTotal = 0;
            options.dataTable.fnClearTable(0);
            options.dataTable.fnDraw();
        }

        function InitAllCKEditor() {
            $.each(options.editors, function (i,item) {
                InitCKEditor(item);
            });
        }

        var _that = this;
        _that = null;
        return this;
    }
    })(jQuery);


    (function ($, window, document, undefined) {
        var xhrFileUploadHandle = undefined;

        $.fn.FileUpload = function (options) {
            var defaults = {
                self: null,
                url: "/backOffice/upload/uploadfile",
                maxsize: 123,
                allowExtension: '.mp4,.png',
                rewriteDocumentId: 'video-src',
                processBarId: ''
            };

            var settings = $.extend({}, defaults, options);

            var file = settings.self.files;

            if (file == null)
                return;
            var validated = true;
            for (var i = 0; i < file.length; i++) {
                if (!validateFile(file[i],settings))
                    validated=false;

            }
            if (validated) {
                executeUpload(settings.url, settings.self.files, handleUploadComplete, settings.rewriteDocumentId, settings.processBarId);
            }
        }

        var validateFile = function (file, settings) {
            if (file == undefined) {
                artDialog.alert("请选择文件!");
                return false;
            }

            if (file.size > 209715200) {
                artDialog.alert('请上传200M一下的文件！');
                return false;
            }

            if (validateExtension(file, settings) < 0) {
                artDialog.alert('请上传后缀名为：' + settings.allowExtension + '的文件');
                return false;
            }
            return true;


        };

        function validateExtension(file, settings) {
            return settings.allowExtension.indexOf(file.type.substring(file.type.indexOf('/') + 1, file.type.length));
        }

        function handleUploadComplete() {
            if (4 == this.readyState) {
                if (this.response != "") {
                    var responseJSON = eval('(' + this.response + ')');
                    uploadCallBack(responseJSON);
                }
                //resetInputFile($("#section-image-input"));
            }
        }

        function uploadCallBack(data) {
            if (data.success) {
                $("#" + data.progressbarId).html("上传成功");          
                $('#' + data.target).val(data.serverfileName);
                $('#' + data.target).focus();
            } else {
                $("#" + data.progressbarId).css('width', '99%');
                artDialog.alert("文件上传失败，请重新上传。");
            }
        }

        function executeUpload(url, file, callback, rewriteDocumentId, processBarId) {

            var xhr = new XMLHttpRequest();
            if (xhr.upload) {
                xhr.upload.onprogress = function (e) {
                    var done = e.position || e.loaded;
                    var total = e.totalSize || e.total;
                    var percent = Math.floor(done / total * 1000) / 10;
                    $("#" + processBarId).css('width', (percent) + '%');
                    $("#" + processBarId).html(percent + "%");              
                };
            }
            xhr.onreadystatechange = callback;

            xhr.open("post", url, true);
            var formData = new FormData();
            for (var i = 0; i < file.length; i++) {
                formData.append("file" + i, file[i]);
            }
            formData.append("target", rewriteDocumentId);
            formData.append("progressbarId",processBarId);
            xhrFileUploadHandle = xhr;
            xhr.send(formData);


        }
    })(jQuery, window, document);



    jQuery.extend(jQuery.validator.messages, {
        required: "必选字段",
        remote: "请修正该字段",
        email: "请输入正确格式的电子邮件",
        url: "请输入合法的网址",
        date: "请输入合法的日期",
        dateISO: "请输入合法的日期 (ISO).",
        number: "请输入合法的数字",
        digits: "只能输入整数",
        creditcard: "请输入合法的信用卡号",
        equalTo: "请再次输入相同的值",
        accept: "请输入拥有合法后缀名的字符串",
        maxlength: jQuery.validator.format("请输入一个长度最多是 {0} 的字符串"),
        minlength: jQuery.validator.format("请输入一个长度最少是 {0} 的字符串"),
        rangelength: jQuery.validator.format("请输入一个长度介于 {0} 和 {1} 之间的字符串"),
        range: jQuery.validator.format("请输入一个介于 {0} 和 {1} 之间的值"),
        max: jQuery.validator.format("请输入一个最大为 {0} 的值"),
        min: jQuery.validator.format("请输入一个最小为 {0} 的值")
    });

