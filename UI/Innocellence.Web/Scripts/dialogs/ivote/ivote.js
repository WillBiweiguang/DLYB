/**
 * Created by DLYB Wechat Group.
 * User: andrew.zheng
 * Date: 16-3-22
 * Time: 下午14:56
 * To build this template use File | Settings | File Templates.
 */
//Complete UE Componet Register.
UE.registerUI('vote', function (editor, uiName) {

    //创建dialog
    var dialog = new UE.ui.Dialog({
        //指定弹出层中页面的路径，这里只能支持页面,因为跟addCustomizeDialog.js相同目录，所以无需加路径
        iframeUrl: '/scripts/dialogs/ivote/ivote.html',
        //需要指定当前的编辑器实例
        editor: editor,
        //指定dialog的名字
        name: uiName,
        //dialog的标题
        title: "insert " + uiName,

        //指定dialog的外围样式
        cssRules: "width:600px;height:300px;",

        //如果给出了buttons就代表dialog有确定和取消
        buttons: [
            {
                className: 'edui-okbutton',
                label: 'Confirm',
                onclick: function () {
                    //do my thing here
                    dialog.close(true);
                    
                }
            },
            {
                className: 'edui-cancelbutton',
                label: 'Cancel',
                onclick: function () {
                    dialog.close(false);
                }
            }
        ]
    });

    //参考addCustomizeButton.js
    var btn = new UE.ui.Button({
        name: uiName,
        title: uiName,
        //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
        cssRules: 'background-position: -500px 0;',
        onclick: function () {
            //渲染dialog
            dialog.render();
            dialog.open();
        }
    });

    //当点到编辑内容上时，按钮要做的状态反射
    editor.addListener('selectionchange', function () {
        var state = editor.queryCommandState(uiName);
        if (state == -1) {
            btn.setDisabled(true);
            btn.setChecked(false);
        } else {
            btn.setDisabled(false);
            btn.setChecked(state);
        }
    });

    return btn;
}/*index 指定添加到工具栏上的那个位置，默认时追加到最后,editorId 指定这个UI是那个编辑器实例上的，默认是页面上所有的编辑器都会添加这个按钮*/);


