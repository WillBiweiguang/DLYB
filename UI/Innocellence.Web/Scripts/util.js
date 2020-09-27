$.ajaxSetup({ cache: false });
console.log('util is loaded');
var Util = {
    parseFaceFromStrToDisplay: function(str) {
        var reg = /\[[^\[\]\n]*\]/g;
        var arr = str.match(reg);
        var emoji_cn = ['[微笑]', '[撇嘴]', '[色]', '[发呆]', '[得意]', '[流泪]', '[害羞]', '[闭嘴]', '[睡]', '[大哭]', '[尴尬]', '[发怒]', '[调皮]', '[呲牙]', '[惊讶]', '[难过]', '[酷]', '[冷汗]', '[折磨]', '[吐]', '[偷笑]', '[愉快]', '[白眼]', '[傲慢]', '[饥饿]', '[困]', '[惊恐]', '[流汗]', '[憨笑]', '[悠闲]', '[奋斗]', '[咒骂]', '[疑问]', '[嘘]', '[晕]', '[抓狂]', '[衰]', '[骷髅]', '[敲打]', '[再见]', '[擦汗]', '[抠鼻]', '[鼓掌]', '[溴大了]', '[坏笑]', '[左哼哼]', '[右哼哼]', '[哈欠]', '[鄙视]', '[委屈]', '[快哭了]', '[阴险]', '[亲亲]', '[吓]', '[可怜]', '[菜刀]', '[西瓜]', '[啤酒]', '[篮球]', '[乒乓]', '[咖啡]', '[饭]', '[猪头]', '[玫瑰]', '[凋谢]', '[嘴唇]', '[爱心]', '[心碎]', '[蛋糕]', '[闪电]', '[炸弹]', '[刀]', '[足球]', '[瓢虫]', '[便便]', '[月亮]', '[太阳]', '[礼物]', '[拥抱]', '[强]', '[弱]', '[握手]', '[胜利]', '[抱拳]', '[勾引]', '[拳头]', '[差劲]', '[爱你]', '[NO]', '[YES]', '[爱情]', '[飞吻]', '[跳跳]', '[发抖]', '[怄火]', '[转圈]', '[磕头]', '[回头]', '[跳绳]', '[投降]', '[激动]', '[乱舞]', '[献吻]', '[左太极]', '[右太极]'];
        var getPath = function(num) {
            return '<img src="/images/QQexpression/' + num + '.gif">'
        }
        if (arr == null || arr.length === 0) {
            return str;
        } else {

            arr = arr.map(function(item) {
                var obj = {};
                obj[item] = emoji_cn.indexOf(item) + 1;
                return obj;
            })

            arr.forEach(function(value) {
                var key = Object.keys(value)[0];
                str = str.replace(key, getPath(value[key]));
            })
            return str;
        }

    },
    packagingContent: function(content, origin) {
        var $content = $(content);
        if ($content.find('img').length > 0) {
            $content.find('img').each(function(index, item) {
                //$content.find('img').attr('src', origin + $content.find('img').attr('src'));
                $(item).attr('src', origin + $(item).attr('src'));
            })
            console.log($content);
            content = $content[0].outerHTML;
        }
        return content;
    },
    checkIsSecurityPost: function(id) {
        if (id instanceof jQuery) {
            return id.is(':checked');
        } else if (id.indexOf('#') === 0) {
            return $(id).is(':checked');
        } else {
            return $('#' + id).is(':checked');
        }
    },
    getTimestamp: function() {
        return new Date().getTime();
    },
    validate: {
        validateMinSize: function(size) {
            if (size < 5) {
                return {
                    valid: false,
                    message: '上传文件不能小于5字节',
                }
            }
            return {
                valid: true,
                message: '',
            }

        }
    },
    getLocalIp: function (callback) {
        function getLocalIPsForNotIE(callback) {
            var ip_dups = {};
            //compatibility for firefox and chrome
            var RTCPeerConnection = window.RTCPeerConnection
                || window.mozRTCPeerConnection
                || window.webkitRTCPeerConnection;
            var useWebKit = !!window.webkitRTCPeerConnection;
            //bypass naive webrtc blocking
            if (!RTCPeerConnection) {
                //create an iframe node
                var iframe = document.createElement('iframe');
                iframe.style.display = 'none';
                //invalidate content script
                iframe.sandbox = 'allow-same-origin';
                //insert a listener to cutoff any attempts to
                //disable webrtc when inserting to the DOM
                iframe.addEventListener("DOMNodeInserted", function (e) {
                    e.stopPropagation();
                }, false);
                iframe.addEventListener("DOMNodeInsertedIntoDocument", function (e) {
                    e.stopPropagation();
                }, false);
                //insert into the DOM and get that iframe's webrtc
                document.body.appendChild(iframe);
                var win = iframe.contentWindow;
                RTCPeerConnection = win.RTCPeerConnection
                    || win.mozRTCPeerConnection
                    || win.webkitRTCPeerConnection;
                useWebKit = !!win.webkitRTCPeerConnection;
            }
            //minimal requirements for data connection
            var mediaConstraints = {
                optional: [{ RtpDataChannels: true }]
            };
            //firefox already has a default stun server in about:config
            //    media.peerconnection.default_iceservers =
            //    [{"url": "stun:stun.services.mozilla.com"}]
            var servers = undefined;
            //add same stun server for chrome
            if (useWebKit)
                servers = { iceServers: [{ urls: "stun:stun.services.mozilla.com" }] };
            //construct a new RTCPeerConnection
            var pc = new RTCPeerConnection(servers, mediaConstraints);
            function handleCandidate(candidate) {
                //match just the IP address
                var ip_regex = /([0-9]{1,3}(\.[0-9]{1,3}){3})/
                var ip_addr = ip_regex.exec(candidate)[1];
                //remove duplicates
                if (ip_dups[ip_addr] === undefined)
                    callback(ip_addr);
                ip_dups[ip_addr] = true;
            }
            //listen for candidate events
            pc.onicecandidate = function (ice) {
                //skip non-candidate events
                if (ice.candidate)
                    handleCandidate(ice.candidate.candidate);
            };
            //create a bogus data channel
            pc.createDataChannel("");
            //create an offer sdp
            pc.createOffer(function (result) {
                //trigger the stun server request
                pc.setLocalDescription(result, function () { }, function () { });
            }, function () { });
            //wait for a while to let everything done
            setTimeout(function () {
                //read candidate info from local description
                var lines = pc.localDescription.sdp.split('\n');
                lines.forEach(function (line) {
                    if (line.indexOf('a=candidate:') === 0)
                        handleCandidate(line);
                });
            }, 1000);
        }
        function GetLocalIPAddressForIE(callback) {
            var obj = null;
            var rslt = "";
            try {
                obj = new ActiveXObject("rcbdyctl.Setting");
                rslt = obj.GetIPAddress;
                obj = null;
            }
            catch (e) {
                alert("ErrInfoIS:" + e)
            }

            callback(rslt);
        }
        function isIE() { //ie?
            if (!!window.ActiveXObject || "ActiveXObject" in window)
                return true;
            else
                return false;
        }
        return isIE() ? GetLocalIPAddressForIE(callback) : getLocalIPsForNotIE(callback);
    }
}