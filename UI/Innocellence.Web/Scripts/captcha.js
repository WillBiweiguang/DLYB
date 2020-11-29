"use strict";

function _toConsumableArray(arr) { return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) }

function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.");
}

function _unsupportedIterableToArray(o, minLen) { if (!o) return; if (typeof o === "string") return _arrayLikeToArray(o, minLen); var n = Object.prototype.toString.call(o).slice(8, -1); if (n === "Object" && o.constructor) n = o.constructor.name; if (n === "Map" || n === "Set") return Array.from(o); if (n === "Arguments" || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) return _arrayLikeToArray(o, minLen); }

function _iterableToArray(iter) { if (typeof Symbol !== "undefined" && Symbol.iterator in Object(iter)) return Array.from(iter); }

function _arrayWithoutHoles(arr) { if (Array.isArray(arr)) return _arrayLikeToArray(arr); }

function _arrayLikeToArray(arr, len) { if (len == null || len > arr.length) len = arr.length; for (var i = 0, arr2 = new Array(len); i < len; i++) { arr2[i] = arr[i]; } return arr2; }

/**
 * Created by saucxs on 2019/3/1.
 */
function CaptchaMini() {
    var _this = this;

    var params = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
    var middleParams = $.extend({
        lineWidth: 0.5,
        //线条宽度
        lineNum: 2,
        //线条数量
        dotR: 1,
        //点的半径
        dotNum: 15,
        //点的数量
        preGroundColor: [10, 80],
        //前景色区间
        backGroundColor: [150, 250],
        //背景色区间
        fontSize: 20,
        //字体大小
        fontFamily: ['Georgia', '微软雅黑', 'Helvetica', 'Arial'],
        //字体类型
        fontStyle: 'fill',
        //字体绘制方法，有fill和stroke
        content: 'acdefhijkmnpwxyABCDEFGHJKMNPQWXY12345789',
        //验证码内容
        length: 4 //验证码长度

    }, params);
    $.extend(_this, middleParams);
    //$(middleParams).each(function (index,item) {
    //    _this[item] = middleParams[item];
    //});
    this.canvas = null;
    this.paint = null;
}

;
/*Captcha的原型上绑定方法
* 获取区间的随机数
* params []*/

CaptchaMini.prototype.getRandom = function () {
    for (var _len = arguments.length, arr = new Array(_len), _key = 0; _key < _len; _key++) {
        arr[_key] = arguments[_key];
    }

    arr.sort(function (a, b) {
        return a - b;
    });
    return Math.floor(Math.random() * (arr[1] - arr[0]) + arr[0]);
};
/*Captcha的原型上绑定方法
* 获取随机颜色
* params []*/


CaptchaMini.prototype.getColor = function (arr) {
    var _this2 = this;

    var colors = ['', '', ''];
    $(colors).each(function (index) {
        colors[index] = _this2.getRandom.apply(_this2, _toConsumableArray(arr));
    });
    //colors = colors.map(function (item) {
    //    return _this2.getRandom.apply(_this2, _toConsumableArray(arr));
    //});
    return colors;
};
/*Captcha的原型上绑定方法
* 获取验证码*/


CaptchaMini.prototype.getText = function () {
    var length = this.content.length;
    var str = '';

    for (var i = 0; i < this.length; i++) {
        str += this.content[this.getRandom(0, length)];
    }

    return str;
};
/*Captcha的原型上绑定方法
* 绘制线条*/


CaptchaMini.prototype.line = function () {
    for (var i = 0; i < this.lineNum; i++) {
        /*随机获取线条的起始位置*/
        var x = this.getRandom(0, this.canvas.width);
        var y = this.getRandom(0, this.canvas.height);
        var endX = this.getRandom(0, this.canvas.width);
        var endY = this.getRandom(0, this.canvas.height);
        this.paint.beginPath();
        this.paint.lineWidth = this.lineWidth;
        /*获取颜色路径*/

        var colors = this.getColor(this.preGroundColor);
        this.paint.strokeStyle = 'rgba(' + colors[0] + ',' + colors[1] + ',' + colors[2] + ',' + '0.8)';
        /*绘制路径*/

        this.paint.moveTo(x, y);
        this.paint.lineTo(endX, endY);
        this.paint.closePath();
        this.paint.stroke();
    }
};
/*Captcha的原型上绑定方法
* 绘制圆点*/


CaptchaMini.prototype.circle = function () {
    for (var i = 0; i < this.dotNum; i++) {
        /*随机获取圆心*/
        var x = this.getRandom(0, this.canvas.width);
        var y = this.getRandom(0, this.canvas.height);
        this.paint.beginPath();
        /*绘制圆*/

        this.paint.arc(x, y, this.dotR, 0, Math.PI * 2, false);
        this.paint.closePath();
        /*随机获取路径颜色*/

        var colors = this.getColor(this.preGroundColor);
        this.paint.fillStyle = 'rgba(' + colors[0] + ',' + colors[1] + ',' + colors[2] + ',' + '0.8)';
        /*绘制*/

        this.paint.fill();
    }
};
/*Captcha的原型上绑定方法
* 绘制文字*/


CaptchaMini.prototype.font = function () {
    var str = this.getText();
    this.callback(str);
    /*指定文字风格*/

    this.paint.font = this.fontSize + 'px ' + this.fontFamily[this.getRandom(0, this.fontFamily.length)];
    this.paint.textBaseline = 'middle';
    /*指定文字绘制风格*/

    var fontStyle = this.fontStyle + 'Text';
    var colorStyle = this.fontStyle + 'Style';

    for (var i = 0; i < this.length; i++) {
        var fontWidth = this.paint.measureText(str[i]).width;
        var x = this.getRandom(this.canvas.width / this.length * i + 0.2 * fontWidth, this.canvas.width / this.length * i + 0.5 * fontWidth);
        /*随机获取字体的旋转角度*/

        var deg = this.getRandom(-6, 6);
        /*随机获取文字颜色*/

        var colors = this.getColor(this.preGroundColor);
        this.paint[colorStyle] = 'rgba(' + colors[0] + ',' + colors[1] + ',' + colors[2] + ',' + '0.8)';
        /*开始绘制*/

        this.paint.save();
        this.paint.rotate(deg * Math.PI / 180);
        this.paint[fontStyle](str[i], x, this.canvas.height / 2);
        this.paint.restore();
    }
};
/*Captcha的原型上绑定方法
* 绘制图形*/


CaptchaMini.prototype.draw = function (dom) {
    var _this3 = this;

    var callback = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : function () { };

    /*获取canvas的dom*/
    if (!this.paint) {
        this.canvas = dom;
        if (!this.canvas) return; else this.paint = this.canvas.getContext('2d');
        /*回调函数赋值给this*/

        this.callback = callback;

        this.canvas.onclick = function () {
            _this3.drawAgain();
        };
    }
    /*随机画布颜色，使用背景色*/


    var colors = this.getColor(this.backGroundColor);
    this.paint.fillStyle = 'rgba(' + colors[0] + ',' + colors[1] + ',' + colors[2] + ',' + '0.8)';
    /*绘制画布*/

    this.paint.fillRect(0, 0, this.canvas.width, this.canvas.height);
    /*绘图*/

    this.circle();
    this.line();
    this.font();
};
/*Captcha的原型上绑定方法
* 清空画布*/


CaptchaMini.prototype.clear = function () {
    this.paint.clearRect(0, 0, this.canvas.width, this.canvas.height);
};
/*Captcha的原型上绑定方法
* 重新绘制*/


CaptchaMini.prototype.drawAgain = function () {
    this.clear();
    this.draw(this.callbak);
};

