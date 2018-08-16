/**
作者：wyd
日期：2016-09-01
备注：通用js框架
*/

/**  扩展 String 对象 */
String.prototype.Trim = function () { return this.replace(/(^\s*)|(\s*$)/g, ""); };
String.prototype.Len = function () { return this.replace(/[^\x00-\xff]/g, "aa").length; };
String.prototype.Contains = function (A) { return (this.indexOf(A) > -1); };
String.prototype.EndChar = function () { return this.substring(this.length - 1, this.length); };
String.prototype.ClearEndChar = function (s) { return this.substring(0, this.lastIndexOf(s)); };
String.prototype.EndWith = function (s) { return this.substring(this.length - s.length) == s; };
String.prototype.StartWith = function (s) { return this.substring(0, s.length) == s; };
String.prototype.Lstr = function (s) {
    var len = this.indexOf(s); if (len > 0) { return this.substring(0, len); } else { return null; }
};
String.prototype.Rstr = function (s) {
    var len = this.lastIndexOf(s); if (len > 0) { return this.substring(len, this.length); } else { return null; }
};
var StringBuilder = function () { this.arr = new Array(); this.Append = function (s) { this.arr.push(s); }; this.ToString = function () { return this.arr.join(''); }; };



/*全局变量*/
var $db = [];

/* 基类 */
function Class() { var T, F = arguments[arguments.length - 1]; if (!F) return; var B = (arguments.length > 1) ? arguments[0] : $o; function o_() { }; o_.prototype = B.prototype; var PO = new o_(); for (var m in F) if (m != "create") PO[m] = F[m]; if (F.toString != Object.prototype.toString) PO.toString = F.toString; if (F.toLocaleString != Object.prototype.toLocaleString) PO.toLocaleString = F.toLocaleString; if (F.valueOf != Object.prototype.valueOf) PO.valueOf = F.valueOf; if (F.create) { T = F.create; } else { T = function () { this.base.apply(this, arguments) } }; T.prototype = PO; T.Base = B; T.prototype.Type = T; return T; };
function $o() { }; $o.prototype.isA = function (t) { var f = this.Type; while (f) { if (f == t) return true; f = f.Base; }; return false; }; $o.prototype.base = function () { var Base = this.Type.Base; if (!Base.Base) { Base.apply(this, arguments); } else { this.base = MakeBase(Base); Base.apply(this, arguments); delete this.base; }; function MakeBase(Type) { var Base = Type.Base; if (!Base.Base) return Base; return function () { this.base = MakeBase(Base); Base.apply(this, arguments); }; }; };

/* 载入后运行函数 */
function $run(f) { var o = window.onload; if (typeof window.onload != "function") { window.onload = f; } else { window.onload = function () { o(); f(); } }; };
/* 载入后运行函数 */
function $init(f) { var o = window.onload; if (typeof window.onload != "function") { window.onload = f; } else { window.onload = function () { o(); f(); } }; };

/* 选择器 */
function $id(e) { return document.getElementById(e); };
function $name(e) { return document.getElementsByName(e); };

/* 值 */
function val(e, str) { var em = $id(e), t = em.tagName.toUpperCase(); if (str == null) { if (t == "INPUT" || t == "TEXTAREA") { return em.value; }; if (t == "SELECT") { return em.options[em.selectedIndex].value; }; if (t == "DIV") { return em.innerHTML; }; } else { if (t == "INPUT" || t == "TEXTAREA") { em.value = str; }; if (t == "SELECT") { em.options[em.selectedIndex].value = str; }; if (t == "DIV") { em.innerHTML = str; }; } };
function rdoVal(s) { var em = $name(s); if (em.length) { for (var i = 0; i < em.length; i++) { if (em[i].checked) return em[i].value; } } else { return em.value; } };
function chkVal(s) { var em = $name(s), str = ""; if (em.length) { for (var i = 0; i < em.length; i++) { if (em[i].checked) str += em[i].value + ","; }; str = str.ClearEndChar(","); } else { str = em.value; }; return str; };
/*输入框的值*/
function inpVal(s) { var em = $name(s), str = ""; if (em.length) { for (var i = 0; i < em.length; i++) { str += em[i].value + ","; }; str = str.ClearEndChar(","); } else { str = em.value; }; return str; }
function inpAttr(s, attr) {
    var em = $name(s), str = ""; if (em.length) { for (var i = 0; i < em.length; i++) { str += em[i].getAttribute(attr) + ","; }; str = str.ClearEndChar(","); } else { str = em.getAttribute(attr); }; return str;
}//获取属性的值


/* 输入输出 */
function $out(s) { document.write(s); };
function $outln(s) { document.writeln(s); };
function $goto(s) { window.location = (s != null) ? s : window.location; };

/* 常用方法 */
function $copy(e) { var rng = document.body.createTextRange(); rng.moveToElementText($id(e)); rng.scrollIntoView(); rng.select(); rng.execCommand("Copy"); rng.collapse(false); };
function $print() { bdhtml = window.document.body.innerHTML; sprnstr = "<!--startprint-->"; eprnstr = "<!--endprint-->"; prnhtml = bdhtml.substr(bdhtml.indexOf(sprnstr) + 17); prnhtml = prnhtml.substring(0, prnhtml.indexOf(eprnstr)); window.document.body.innerHTML = prnhtml; window.print(); };
function $enter(id, e) {
    var ev = e || window.event, k = ev.which || ev.keyCode;
    if (k == 13) { ev.returnValue = false; ev.cancel = true; $(id).trigger("click"); return false; }
};
function $click(id) { $(id).trigger("click"); };
function $focus(e) { $id(e).focus(); };
function $select(e) { $id(e).select(); };
function $lnkFocus(e) { var lnks = $tag("a", e); for (i = 0; i < lnks.length; i++) lnks[i].onfocus = function () { this.blur() }; };
function getWebUrl() { var url = window.location.toString(); return url.Contains("?") ? url.substring(0, url.lastIndexOf("?")) : url; };
function $rand(n) { rnd.today = new Date(); rnd.seed = rnd.today.getTime(); function rnd() { rnd.seed = (rnd.seed * 9301 + 49297) % 233280; return rnd.seed / (233280.0); }; return Math.ceil(rnd() * number); };
//将逗号分割的字符串转换为数组
function $csv(s) { return s.split(","); }
function $ticks() { return new Date().getTime(); };
function $timezone() { return new Date().getTime(); };
function IsNullOrEmpty(v) { if (v != null && v != 'null' && v != "" && v != undefined && v != 'undefined') return false; else return true; };

/**
* 初始化验证表单
* 
*/
$.fn.initValidform = function () {
    var checkValidform = function (formObj) {
        $(formObj).Validform({
            //btnSubmit: "#btnSubmit",
            tiptype: function (msg, o, cssctl) {
                /*msg：提示信息;
                o:{obj:*,type:*,curform:*}
                obj指向的是当前验证的表单元素（或表单对象）；
                type指示提示的状态，值为1、2、3、4， 1：正在检测/提交数据，2：通过验证，3：验证失败，4：提示ignore状态；
                curform为当前form对象;
                cssctl:内置的提示信息样式控制函数，该函数需传入两个参数：显示提示信息的对象 和 当前提示的状态（既形参o中的type）；*/
                //全部验证通过提交表单时o.obj为该表单对象;
                if (!o.obj.is("form")) {
                    //定位到相应的Tab页面
                    //页面上不存在提示信息的标签时，自动创建;
                    if (o.obj.parents("dd").length>0) {
                        if (o.obj.parents("dd").find(".Validform_checktip").length == 0) {
                            o.obj.parents("dd").append("<span class='Validform_checktip' />");
                            o.obj.parents("dd").next().find(".Validform_checktip").remove();
                            var objtip = o.obj.parents("dd").find(".Validform_checktip");
                            cssctl(objtip, o.type);
                            objtip.text(msg);
                        } else {
                            var objtip = o.obj.parents("dd").find(".Validform_checktip");
                            cssctl(objtip, o.type);
                            objtip.text(msg);
                        }
                    }else{
                        //页面上不存在提示信息的标签时，自动创建;
                        if (o.obj.parents("div").find(".Validform_checktip").length == 0) {
                            o.obj.parents("div").append("<span class='Validform_checktip' />");
                            o.obj.parents("div").next().find(".Validform_checktip").remove();
                        }
                        var objtip = o.obj.parents("div:first").find(".Validform_checktip");
                        cssctl(objtip, o.type);
                        objtip.text(msg);
                    }
                }
            },
            showAllError: true
        });
    };
    return $(this).each(function () {
        checkValidform($(this));
    });
}
/**
* 初始化验证表单
*/
$.fn.initValidform1 = function (fn1) {
    var checkValidform = function (formObj,fn1) {
        $(formObj).Validform({
            tiptype: function (msg, o, cssctl) {
                /*msg：提示信息;
                o:{obj:*,type:*,curform:*}
                obj指向的是当前验证的表单元素（或表单对象）；
                type指示提示的状态，值为1、2、3、4， 1：正在检测/提交数据，2：通过验证，3：验证失败，4：提示ignore状态；
                curform为当前form对象;
                cssctl:内置的提示信息样式控制函数，该函数需传入两个参数：显示提示信息的对象 和 当前提示的状态（既形参o中的type）；*/
                //全部验证通过提交表单时o.obj为该表单对象;
                if (!o.obj.is("form")) {
                    //定位到相应的Tab页面
                    //页面上不存在提示信息的标签时，自动创建;
                    if (o.obj.parents("dd").length > 0) {
                        if (o.obj.parents("dd").find(".Validform_checktip").length == 0) {
                            o.obj.parents("dd").append("<span class='Validform_checktip' />");
                            o.obj.parents("dd").next().find(".Validform_checktip").remove();
                            var objtip = o.obj.parents("dd").find(".Validform_checktip");
                            cssctl(objtip, o.type);
                            objtip.text(msg);
                        } else {
                            var objtip = o.obj.parents("dd").find(".Validform_checktip");
                            cssctl(objtip, o.type);
                            objtip.text(msg);
                        }
                    } else {
                        //页面上不存在提示信息的标签时，自动创建;
                        if (o.obj.parents("div").find(".Validform_checktip").length == 0) {
                            o.obj.parents("div").append("<span class='Validform_checktip' />");
                            o.obj.parents("div").next().find(".Validform_checktip").remove();
                        }
                        var objtip = o.obj.parents("div:first").find(".Validform_checktip");
                        cssctl(objtip, o.type);
                        objtip.text(msg);
                    }
                }
            },
            showAllError: true,
            callback:fn1
            //callback: function (data) {
            //    fn1;//ajax提交，（不是表单提交，表单提交必须有name属性）能控制返回json之后的动作，而不是单纯页面显示json
            //    return false;
            //}
        });
    };
    return $(this).each(function () {
        checkValidform($(this),fn1);
    });
}
/**
* 调用ajax
* file 文件名
* param get参数 (a=1&b=2&c=3)
* fn 回调函数 ： 服务端返回 result=xxx; 回调函数直接处理result变量。
*/
$.ajaxMethod = function (url, param, fn) {
    $.ajax({
        type: "GET",
        cache: false,
        url: url,
        data: param,
        dataType: "json",
        success: fn
    });
};
/**
* 调用ajaxGet
* file 文件名
* param get参数 (a=1&b=2&c=3)
* fn 回调函数 ： 服务端返回 result=xxx; 回调函数直接处理result变量。
*/
$.ajaxGetJson = function (url, param, fn, er, bs) {
    $.ajax({
        type: "GET",
        cache: false,
        url: url,
        data: param,
        dataType: "json",
        success: fn,
        error: er,
        beforeSend: bs
    });
};
/**
* 调用ajaxGet
* file 文件名
* param get参数 (a=1&b=2&c=3)
* fn 回调函数 ： 服务端返回 result=xxx; 回调函数直接处理result变量。
*/
$.ajaxGetText = function (url, param, fn, er, bs) {
    $.ajax({
        type: "GET",
        cache: false,
        url: url,
        data: param,
        dataType: "text",
        success: fn,
        error: er,
        beforeSend: bs
    });
};
/**
* 调用ajaxPostJson
* file 文件名
* param get参数 (a=1&b=2&c=3)
* fn 回调函数 ： 服务端返回 result=xxx; 回调函数直接处理result变量。
*/
$.ajaxPostJson = function (url, param, fn, er, bs) {
    $.ajax({
        type: "POST",
        url: url,
        data: param,
        dataType: "json",
        success: fn,
        error: er,
        beforeSend: bs
    });
};
/**
* 调用ajaxPostText
* file 文件名
* param get参数 (a=1&b=2&c=3)
* fn 回调函数 ： 服务端返回 result=xxx; 回调函数直接处理result变量。
*/
$.ajaxPostText = function (url, param, fn, er, bs) {
    $.ajax({
        type: "POST",
        cache: false,
        url: url,
        data: JSON.stringify(param),
        dataType: "text",
        success: fn,
        error: er,
        beforeSend: bs
    });
};

//写Cookie
function addCookie(objName, objValue, objHours) {
    var str = objName + "=" + escape(objValue);
    if (objHours > 0) {//为0时不设定过期时间，浏览器关闭时cookie自动消失
        var date = new Date();
        var ms = objHours * 3600 * 1000;
        date.setTime(date.getTime() + ms);
        str += "; expires=" + date.toGMTString() + " ;path=/;";
    }
    document.cookie = str;
}

//读Cookie
function getCookie(objName) {//获取指定名称的cookie的值
    var arrStr = document.cookie.split("; ");
    for (var i = 0; i < arrStr.length; i++) {
        var temp = arrStr[i].split("=");
        if (temp[0] == objName) return unescape(temp[1]);
    }
    return "";
}


//全选取消按钮函数
function checkAll(chkobj) {
    if ($(chkobj).text() == "全选") {
        $(chkobj).children("span").text("取消");
        $(".checkall input:enabled").prop("checked", true);
    } else {
        $(chkobj).children("span").text("全选");
        $(".checkall input:enabled").prop("checked", false);
    }
}

/**** dialog弹出框 ****/
function $dialog(o) {
    return $.dialog(o);
};
//刷新当前Tabs
function $tabs_reload() {
    $("#framecenter").ligerGetTabManager().reload(top.tab.selectedTabId);
}
//关闭弹出窗口
function $dialog_close(dialogId) {
    $.dialog({ id: dialogId }).close();
}
function $dialog_close_reload(dialogId) {
    $tabs_reload();
    $dialog_close(dialogId);
}

/**** $tip  信息提示窗口 ****/
function $tip(msg, fn) {
    $tip(msg, 2, "loading.gif", fn);
}
function $tip(msg, t, ic, fn) {
    $.dialog.tips(msg, t, ic, fn);
};

/**** Alert 弹出框 ****/

function $alert(msg, fn) {
    msg = "&nbsp;&nbsp;&nbsp;&nbsp; " + msg + " &nbsp;&nbsp;&nbsp;&nbsp;";
    //$.dialog.alert(msg, fn);
    var d = dialog({
        content: msg
    });
    d.show();
    setTimeout(function () {
        d.close().remove();
    }, 5000);
};
function $delAlert(title, msg, fn) {
    msg = "&nbsp;&nbsp;&nbsp;&nbsp; " + msg + " &nbsp;&nbsp;&nbsp;&nbsp;";
    //$.dialog.delAlert(title, msg, fn, null);
    var d = dialog({
        title:title,
        content: msg
    });
    d.show();
    //后续刷新页面，这里不再计时
    //setTimeout(function () {
    //    d.close().remove();
    //}, 2000);
};
function $alert_reload(msg) {
    $dialog({
        content: msg,
        ok: function () { this.reload(); }
    });
    return true;
};
function $alert_close(msg) {
    $dialog({
        content: msg,
        ok: function () { this.close(); }
    });
    return true;
};
/*
删除时弹出
*/
function delAlert(msg, url) {
    $delAlert('温馨提示', msg + "2秒后自动刷新页面");
    setTimeout(function () {//延迟2秒
        self.location = url;
    }, 2000);
}
/**** Confirm 弹出框 ****/
function $confirm(msg, fn1, fn2) {
    dialog({
        title: '提示',
        content: msg,
        okValue: '确定',
        ok: fn1,
        cancelValue: '取消',
        cancel:fn2
    }).show();
    return false;
};
function $confirm_delete(fn) {
    dialog({
        title: '提示',
        content: "您确定要删除吗",
        okValue: '确定',
        ok: fn,
        cancelValue: '取消',
        cancel: function () { }
    }).showModal();
    return false;
};

/**** prompt 提问框 ***
* @tit 标题
* @content 显示内容
* @fn  回调函数
* @val 默认值
*/
function $prompt(tit, content, fn, defVal) {
    var promp = $.dialog.prompt(content, fn, defVal).title(tit);
    return promp;
};
//========================基于lhgdialog插件========================
function jsprintOnly(url) {
    window.open(url, "", "status=no,toolbar=no,menubar=no,scrollbars=yes,resizable=yes,width=800,height=600");
};
//弹出一个Dialog窗口
function jsdialog(msgtitle, msgcontent, url, msgcss, callback) {
    var iconurl = "";
    var argnum = arguments.length;
    switch (msgcss) {
        case "Success":
            iconurl = "success.gif";
            break;
        case "Error":
            iconurl = "error.gif";
            break;
        default:
            iconurl = "alert.gif";
            break;
    }
    var dialog = $.dialog({
        title: msgtitle,
        content: msgcontent,
        fixed: true,
        min: false,
        max: false,
        lock: true,
        icon: iconurl,
        ok: true,
        close: function () {
            if (url == "back") {
                history.back(-1);
            } else if (url != "") {
                location.href = url;
            }
            //执行回调函数
            if (argnum == 5) {
                callback();
            }
        }
    });
}
//打开一个Dialog
function artShowDialog(tit, url, width, height) {
    dialog({
        title: tit,
        url: url,
        width: width,
        height: height
    }).show();
    return false;
}
//打开一个最大化的Dialog
function artShowMaxDialog(tit, url) {
    //$.dialog({
    //    title: tit,
    //    content: 'url:' + url,
    //    min: false,
    //    max: false,
    //    lock: false
    //}).max();
    dialog({
        title: tit,
        url: url
    }).show();
    return false;
}
//======================以上基于lhgdialog插件======================
//Tab控制函数
function tabs(tabObj) {
    var tabNum = $(tabObj).parent().index(".nav-tabs li")
    //设置点击后的切换样式
    $(tabObj).parent().parent().find("li").removeClass("active");
    $(tabObj).parent().first().addClass("active");
    //根据参数决定显示内容
    $(".tab-content").hide();
    $(".tab-content").eq(tabNum).show();
}



//初始化Tree目录结构
function initCategoryHtml(parentObj, layNum) {
    $(parentObj).find('li.layer-' + layNum).each(function (i) {
        var liObj = $(this);
        var nextNum = layNum + 1;
        if (liObj.next('.layer-' + nextNum).length > 0) {
            initCategoryHtml(parentObj, nextNum);
            var newObj = $('<ul></ul>').appendTo(liObj);
            moveCategoryHtml(liObj, newObj, nextNum);
        }
    });
}
function moveCategoryHtml(liObj, newObj, nextNum) {
    if (liObj.next('.layer-' + nextNum).length > 0) {
        liObj.next('.layer-' + nextNum).appendTo(newObj);
        moveCategoryHtml(liObj, newObj, nextNum);
    }
}
//初始化Tree目录事件
$.fn.initCategoryTree = function (isOpen) {
    var fCategoryTree = function (parentObj) {
        //遍历所有的UL
        parentObj.find("ul").each(function (i) {
            //遍历UL第一层LI
            $(this).children("li").each(function () {
                var liObj = $(this);
                //判断是否有子菜单和设置距左距离
                var parentIconLenght = liObj.parent().parent().children(".tbody").children(".index").children(".icon").length; //父节点的左距离
                var indexObj = liObj.children(".tbody").children(".index"); //需要树型的目录列
                //设置左距离
                if (parentIconLenght == 0) {
                    parentIconLenght = 1;
                }
                for (var n = 0; n <= parentIconLenght; n++) { //注意<=
                    $('<i class="icon"></i>').prependTo(indexObj); //插入到index前面
                }
                //设置按钮和图标
                indexObj.children(".icon").last().addClass("iconfont icon-folder"); //设置最后一个图标
                //如果有下级菜单
                if (liObj.children("ul").length > 0) {
                    //如果要求全部展开
                    if (isOpen) {
                        indexObj.children(".icon").eq(-2).addClass("expandable iconfont icon-open"); //设置图标展开状态
                    } else {
                        indexObj.children(".icon").eq(-2).addClass("expandable iconfont icon-close"); //设置图标闭合状态
                        liObj.children("ul").hide(); //隐藏下级的UL
                    }
                    //绑定单击事件
                    indexObj.children(".expandable").click(function () {
                        //如果菜单已展开则闭合
                        if ($(this).hasClass("icon-open")) {
                            //设置自身的右图标为+号
                            $(this).removeClass("icon-open");
                            $(this).addClass("icon-close");
                            //隐藏自身父节点的UL子菜单
                            $(this).parent().parent().parent().children("ul").slideUp(300);
                        } else {
                            //设置自身的右图标为-号
                            $(this).removeClass("icon-close");
                            $(this).addClass("icon-open");
                            //显示自身父节点的UL子菜单
                            $(this).parent().parent().parent().children("ul").slideDown(300);
                        }
                    });
                } else {
                    indexObj.children(".icon").eq(-2).addClass("iconfont icon-csac");
                }
            });
            //显示第一个UL
            if (i == 0) {
                $(this).show();
                //展开第一个菜单
                if ($(this).children("li").first().children("ul").length > 0) {
                    $(this).children("li").first().children(".tbody").children(".index").children(".expandable").removeClass("icon-close");
                    $(this).children("li").first().children(".tbody").children(".index").children(".expandable").addClass("icon-open");
                    $(this).children("li").first().children("ul").show();
                }
            }
        });
    };
    return $(this).each(function () {
        fCategoryTree($(this));
    });
}