//-----------------------------------------------
//added by andrew 2016/2/26   基于微信图片预览接口
//-----------------------------------------------
(function () {
    var imgsSrc = [];
    //获取主机地址
    var pathName = window.document.location.href;
    var pos = pathName.indexOf('/', 10);

    var hostPath = pathName.substring(0, pos);

    function reviewImage(src) {
        
        if (typeof window.WeixinJSBridge != 'undefined') {
            WeixinJSBridge.invoke('imagePreview', {
                'current': src,
                'urls': imgsSrc
            });
        }
    }

    function onImgLoad() {
        var imgs = document.getElementsByTagName('img');
        for (var i = 0, l = imgs.length; i < l; i++) {
            var img = imgs.item(i);
            var src = img.getAttribute('src'); // || img.getAttribute('data-src');

            if (src) {

                if (src.indexOf('http') < 0) {
                    src = hostPath + src;
                    imgsSrc.push(src);
                } else { continue; }

                (function (src) {
                    if (img.addEventListener) {
                        //alert("adding event: addEventListener");
                        img.addEventListener('click', function (event) {
                            reviewImage(src);
                            event.stopPropagation();//阻止事件冒泡
                        });
                    } else if (img.attachEvent) {
                        //alert("adding event: attachEvent");
                        img.attachEvent('click', function () {
                            reviewImage(src);
                            window.event.cancelBubble = true;//ie 阻止事件冒泡
                        });
                    }
                })(src);
            }
        }
        return true;
    }

    function load() {
        $('body').on('click touchstart', 'img', function (e) {
            var $this = $(this), isFindA = findAtag($this);

            if (isFindA === false) {
                reviewImage(hostPath + $this.attr('src'));
            }
        });

        $('img').each(function () {
            var $this = $(this), isFindA = findAtag($this), imgUrl = $this.attr('src');

            if (isFindA === false) {
                if (imgUrl.indexOf('http') < 0) {
                    imgUrl = hostPath + imgUrl;
                    if (imgsSrc.indexOf(imgUrl) < 0 ) {
                        imgsSrc.push(imgUrl);
                    }
                    
                }
            }
        });

        function findAtag($img) {
            var $atag = $img.closest('a');

            if ($atag && $atag.attr('href') && $atag.attr('href') != '#') {
                return true;
            }

            return false;
        }
    }

    $(function () {
        load();
    });
})();