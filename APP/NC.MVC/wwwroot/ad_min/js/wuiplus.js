//自定义js

//公共配置


$(document).ready(function () {


    // 打开右侧边栏
    $('.right-sidebar-toggle').click(function () {
        $('#right-sidebar').toggleClass('sidebar-open');
    });

    // 右侧边栏使用slimscroll
    $('.sidebar-container').slimScroll({
        height: '100%',
        railOpacity: 0.4,
        wheelStep: 10
    });
    initMenuTree();
    // Small todo handler
    $('.check-link').click(function () {
        var button = $(this).find('i');
        var label = $(this).next('span');
        button.toggleClass('fa-check-square').toggleClass('fa-square-o');
        label.toggleClass('todo-completed');
        return false;
    });

    //固定菜单栏
    $(function () {
        $('.sidebar-collapse').slimScroll({
            height: '100%',
            railOpacity: 0.9,
            alwaysVisible: false
        });
    });


    //宽度太小就不要再出现顶部

    // 菜单切换
    $('.navbar-minimalize').click(function () {
        if ($(window).width()<1165) {
            return;
        }
        $("body").toggleClass("mini-navbar");
        if($("body").hasClass("mini-navbar")){
            $(".logoshow").hide();
            $("#main-nav").hide();
            $(".side-menu-group").removeClass("active");
        } else {
            $(".logoshow").show();
            $("#main-nav").show();
        }

        SmoothlyMenu();
    });

    //页面尺寸改变时触发,
    $(window).bind("load resize", function () {
        if ($(this).width() < 769) {
            $('body').addClass('mini-navbar');
                $(".logoshow").hide();
                $("#main-navMenu").hide();
            
            $('.navbar-static-side').fadeIn();
            $(".navbar-left").css("width",$(window).width()-$(".navbar-right").width()-30);

        }
        else if($(this).width()< 1165){
            $('body').addClass('mini-navbar');
            $(".logoshow").hide();
            $(".navbar-left").css("width",$(window).width()-$(".navbar-right").width()-30);
            $("#main-navMenu").hide();
        }
        else{
            if($("body").hasClass("mini-navbar")){
                $(".logoshow").hide();
                $("#main-nav").hide();
                $(".side-menu-group").removeClass("active");
            } else {
                $(".logoshow").show();
                $("#main-nav").show();
            }
            $(".navbar-left").css("width",$(window).width()-$(".navbar-right").width()-30);
            $("#main-navMenu").show();
        }
    });


    // 侧边栏高度
    function fix_height() {
        var heightWithoutNavbar = $("body > #wrapper").height() - 61;
        $(".sidebard-panel").css("min-height", heightWithoutNavbar + "px");
    }
    fix_height();

    $(window).bind("load resize click scroll", function () {
        if (!$("body").hasClass('body-small')) {
            fix_height();
        }
    });

    //侧边栏滚动
    $(window).scroll(function () {
        if ($(window).scrollTop() > 0 && !$('body').hasClass('fixed-nav')) {
            $('#right-sidebar').addClass('sidebar-top');
        } else {
            $('#right-sidebar').removeClass('sidebar-top');
        }
    });

    $('.full-height-scroll').slimScroll({
        height: '100%'
    });

    $('#side-menu>li').click(function () {
        if ($('body').hasClass('mini-navbar')) {
            NavToggle();
        }
    });
    $('#side-menu>li li a').click(function () {
        if ($(window).width() < 769) {
            NavToggle();
        }
    });

    $('.nav-close').click(NavToggle);

    //ios浏览器兼容性处理
    if (/(iPhone|iPad|iPod|iOS)/i.test(navigator.userAgent)) {
        $('#content-main').css('overflow-y', 'auto');
    }

});
function NavToggle() {
    $('.navbar-minimalize').trigger('click');
}

function SmoothlyMenu() {
    if (!$('body').hasClass('mini-navbar')) {
        $('#side-menu').hide();
        setTimeout(
            function () {
                $('#side-menu').fadeIn(500);
            }, 100);
    } else if ($('body').hasClass('fixed-sidebar')) {
        $('#side-menu').hide();
        setTimeout(
            function () {
                $('#side-menu').fadeIn(500);
            }, 300);
    } else {
        $('#side-menu').removeAttr('style');
    }
}


//主题设置
$(function () {

    // 顶部菜单固定
    $('#fixednavbar').click(function () {
        if ($('#fixednavbar').is(':checked')) {
            $(".navbar-static-top").removeClass('navbar-static-top').addClass('navbar-fixed-top');
            $("body").removeClass('boxed-layout');
            $("body").addClass('fixed-nav');
            $('#boxedlayout').prop('checked', false);

            if (localStorageSupport) {
                localStorage.setItem("boxedlayout", 'off');
            }

            if (localStorageSupport) {
                localStorage.setItem("fixednavbar", 'on');
            }
        } else {
            $(".navbar-fixed-top").removeClass('navbar-fixed-top').addClass('navbar-static-top');
            $("body").removeClass('fixed-nav');

            if (localStorageSupport) {
                localStorage.setItem("fixednavbar", 'off');
            }
        }
    });


    // 收起左侧菜单
    $('#collapsemenu').click(function () {
        if ($('#collapsemenu').is(':checked')) {
            $("body").addClass('mini-navbar');
            SmoothlyMenu();

            if (localStorageSupport) {
                localStorage.setItem("collapse_menu", 'on');
            }

        } else {
            $("body").removeClass('mini-navbar');
            SmoothlyMenu();

            if (localStorageSupport) {
                localStorage.setItem("collapse_menu", 'off');
            }
        }
    });

    // 固定宽度
    $('#boxedlayout').click(function () {
        if ($('#boxedlayout').is(':checked')) {
            $("body").addClass('boxed-layout');
            $('#fixednavbar').prop('checked', false);
            $(".navbar-fixed-top").removeClass('navbar-fixed-top').addClass('navbar-static-top');
            $("body").removeClass('fixed-nav');
            if (localStorageSupport) {
                localStorage.setItem("fixednavbar", 'off');
            }


            if (localStorageSupport) {
                localStorage.setItem("boxedlayout", 'on');
            }
        } else {
            $("body").removeClass('boxed-layout');

            if (localStorageSupport) {
                localStorage.setItem("boxedlayout", 'off');
            }
        }
    });

    // 默认主题
    $('.s-skin-0').click(function () {
        $("body").removeClass("skin-1");
        $("body").removeClass("skin-2");
        $("body").removeClass("skin-3");
        return false;
    });

    // 蓝色主题
    $('.s-skin-1').click(function () {
        $("body").removeClass("skin-2");
        $("body").removeClass("skin-3");
        $("body").addClass("skin-1");
        return false;
    });

    // 黄色主题
    $('.s-skin-3').click(function () {
        $("body").removeClass("skin-1");
        $("body").removeClass("skin-2");
        $("body").addClass("skin-3");
        return false;
    });

    if (localStorageSupport) {
        var collapse = localStorage.getItem("collapse_menu");
        var fixednavbar = localStorage.getItem("fixednavbar");
        var boxedlayout = localStorage.getItem("boxedlayout");

        if (collapse == 'on') {
            $('#collapsemenu').prop('checked', 'checked')
        }
        if (fixednavbar == 'on') {
            $('#fixednavbar').prop('checked', 'checked')
        }
        if (boxedlayout == 'on') {
            $('#boxedlayout').prop('checked', 'checked')
        }
    }

    if (localStorageSupport) {

        var collapse = localStorage.getItem("collapse_menu");
        var fixednavbar = localStorage.getItem("fixednavbar");
        var boxedlayout = localStorage.getItem("boxedlayout");

        var body = $('body');

        if (collapse == 'on') {
            if (!body.hasClass('body-small')) {
                body.addClass('mini-navbar');
            }
        }

        if (fixednavbar == 'on') {
            $(".navbar-static-top").removeClass('navbar-static-top').addClass('navbar-fixed-top');
            body.addClass('fixed-nav');
        }

        if (boxedlayout == 'on') {
            body.addClass('boxed-layout');
        }
    }
});

//判断浏览器是否支持html5本地存储
function localStorageSupport() {
    return (('localStorage' in window) && window['localStorage'] !== null)
}


//初始化导航菜单
function initMenuTree() {
    var navObj = $("#main-nav");
    var navGroupObj = $("#side-menu .side-menu-group");
    var navItemObj = $("#side-menu .side-menu-group .side-menu-wrap");
    //先清空NAV菜单内容
    navObj.html('');
    navGroupObj.each(function (i) {
        //添加菜单导航
        var navHtml = $('<li><a data-toggle="tab" href="#"><i class="' + $(this).children("h1").attr("main-nav-icon") + '"></i><span>' + $(this).children("h1").attr("title") + '</span></a></li>').appendTo(navObj);
        //默认选中第一项
        if (i == 0) {
            $(this).show();
            navHtml.addClass("active");
        }
        //为菜单添加事件
        navHtml.click(function () {
            navGroupObj.hide();
            navGroupObj.eq(navObj.children("li").index($(this))).show();
            navItemObj.eq(navObj.children("li").index($(this))).children("ul").addClass("in");//
        });
        //遍历菜单开始

        //绑定树菜单事件.开始
        navItemObj.children("ul").each(function (j) { //遍历所有的UL
            $(this).addClass("nav-second-level");
            if ($(this).children("li").children("ul")) {

                $(this).children("li").children("ul").eq(0).addClass("collapse in");
                $(this).children("li").children("ul").each(function () {
                    $(this).addClass("nav-third-level")
                    $(this).children("li").children("ul").eq(0).addClass("collapse in");
                    $(this).children("li").children("ul").each(function () {
                        $(this).addClass("nav-four-level")
                    });
                });
            }
        });
        //遍历菜单结束

    });
    //首先隐藏所有的UL
    navGroupObj.hide();
    navGroupObj.eq(0).show();//展开第一个菜单

    $('.J_menuItem').on('click', menuItem);
    // MetsiMenu
    $('#side-menu').metisMenu();
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