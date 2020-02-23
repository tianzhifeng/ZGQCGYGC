/*
* Author: Abdullah A Almsaeed
* Date: 4 Jan 2014
* Description:
*      This is a demo file used only for the main dashboard (index.html)
**/
"use strict";

$(function () {
    
    //Make the dashboard widgets sortable Using jquery UI
    if (!$('#portal-lock').is(':checked') && (typeof (getCache('isLockPortal')) == 'undefined' || getCache('isLockPortal') == 'false' || window.location.href.indexOf('/Base/UI/Portal/Portal') > 0)) {
        $(".connectedSortable").sortable({
            placeholder: "sort-highlight",
            connectWith: ".connectedSortable",
            handle: ".box-header, .nav-tabs, .box-primary, .new_task_left, .userinfo, .iframe-pic-news",
            forcePlaceholderSize: true,
            zIndex: 999999,
            scroll: true,  //不设置为false，当元素过多无法实现拖拽效果
            scrollSensitivity: 50, //设置当元素移动至边缘多少像素时，便开始滚动页面。设置无意义
            scrollSpeed: 40, //设置页面滚动的速度。无意义，默认即可
            stop: function (event, ui) {
                dashboardChanged();
            }
        });
        $(".connectedSortable .box-header, .connectedSortable .nav-tabs-custom").css("cursor", "move");
    }
    
    //jQuery UI sortable for the todo list
    $(".todo-list").sortable({
        placeholder: "sort-highlight",
        handle: ".handle",
        forcePlaceholderSize: true,
        zIndex: 999999
    });

    //Fix for charts under tabs
    $('.box ul.nav a').on('shown.bs.tab', function (e) {
        //area.redraw();
        //donut.redraw();
    });
});