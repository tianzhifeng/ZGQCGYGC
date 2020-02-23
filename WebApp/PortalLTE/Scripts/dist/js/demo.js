/**
* AdminLTE Demo Menu
* ------------------
* You should not use this file in production.
* This file is for demo purposes only.
*/
function IsUserNewPortal() {
    return typeof (IsUseNewPortal) != 'undefined' && IsUseNewPortal.toLocaleLowerCase() == 'true';
}
(function ($, AdminLTE) {

    /**
    * List of all the available skins
    * 
    * @type Array
    */
    var my_skins = [
    "skin-blue",
    "skin-black",
    "skin-red",
    "skin-yellow",
    "skin-purple",
    "skin-green",
    "skin-blue-light",
    "skin-black-light",
    "skin-red-light",
    "skin-yellow-light",
    "skin-purple-light",
    "skin-green-light"
    ];

    /*背景图设置*/
    var bgs = [
      { "data": "/PortalLTE/Images/bgImage/BanGong.jpg", "title": "办公" },
      { "data": "/PortalLTE/Images/bgImage/GongYe.jpg", "title": "工业" },
      { "data": "/PortalLTE/Images/bgImage/JiHe.jpg", "title": "几何" },
      { "data": "/PortalLTE/Images/bgImage/JieGouZhuYi.jpg", "title": "结构主义" },
      { "data": "/PortalLTE/Images/bgImage/MuWen.jpg", "title": "木纹" },
      { "data": "/PortalLTE/Images/bgImage/SuiPian.jpg", "title": "碎片" },
      { "data": "/PortalLTE/Images/bgImage/TianKong.jpg", "title": "天空" },
      { "data": "/PortalLTE/Images/bgImage/WuHou.jpg", "title": "午后" },
      { "data": "/PortalLTE/Images/bgImage/XianDai.jpg", "title": "现代" },
      { "data": "/PortalLTE/Images/bgImage/YangWang.jpg", "title": "仰望" }
    ];

    //Create the new tab
    var tab_pane = $("<div />", {
        "id": "control-sidebar-theme-demo-options-tab",
        "class": "tab-pane active"
    });

    //Create the tab button
    var tab_button = $("<li />", { "class": "active" })
          .html("<a href='#control-sidebar-theme-demo-options-tab' data-toggle='tab'>"
                  + "<i class='fa fa-wrench'></i>"
                  + "</a>");

    //Add the tab button to the right sidebar tabs
    $("[href='#control-sidebar-home-tab']")
          .parent()
          .before(tab_button);

    //Create the menu
    var demo_settings = $("<div />");

    //Layout options
    var isLockPortal = getCache('isLockPortal');
    demo_settings.append(
          "<h4 class='control-sidebar-heading'>"
          + "模块设置"
          + "</h4>"
          + "<div class='form-group'>"
          + "<label class='control-sidebar-subheading'>"
          + "<input type='checkbox' onclick='resetSetting(this)' class='pull-right'/> "
          + "恢复默认模块"
          + "</label>"
    //+ "<p>选择“恢复默认模块”后，将显示所有模块，布局也将被还原！</p>"
          + "</div>"
          + "<div class='form-group'>"
          + "<label class='control-sidebar-subheading'>"
          + ((typeof (isLockPortal) == 'undefined' || isLockPortal == null || isLockPortal == 'true') ? "<input type='checkbox' checked='checked' id='portal-lock' onclick='lockPortal(this)' class='pull-right'/> " : "<input type='checkbox' id='portal-lock' onclick='lockPortal(this)' class='pull-right'/> ")
          + "是否锁定块"
          + "</label>"
    //+ "<p>选择“恢复默认模块”后，将显示所有模块，布局也将被还原！</p>"
          + "</div>"
          );


    var bg_list = $("<ul />", { "class": 'bg-image list-unstyled clearfix' });
    for (var i = 0; i < bgs.length; i++) {
        var item = $("<li />").append("<div data-bg='" + bgs[i].data + "' style='cursor:pointer'><img src='" + bgs[i].data + "'/><div class='title'>" + bgs[i].title + "</div></div>");
        bg_list.append(item);
    }
    demo_settings.append("<h4 class='control-sidebar-heading'>背景图设置</h4>");
    demo_settings.append(bg_list);


    var skins_list = $("<ul />", { "class": 'list-unstyled clearfix' });

    var skin_purple =
          $("<li />", { style: "float:left; width: 50%; padding: 5px;" })
          .append("<a href='javascript:void(0);' data-skin='skin-purple' style='display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4)' class='clearfix full-opacity-hover'>"
                  + "<div><span style='display:block; width: 20%; float: left; height: 12px;background-color : gray;'></span><span style='display:block; width: 80%; float: left; height: 12px;background-color : gray;'></span></div>"
                  + "<div><span style='display:block; width: 20%; float: left; height: 3px;background-color : #ad2f28;'></span><span style='display:block; width: 80%; float: left; height: 3px;background-color : #ad2f28;'></span></div>"
                  + "<div><span style='display:block; width: 20%; float: left; height: 32px; background: gray;'></span><span style='display:block; width: 80%; float: left; height: 32px; background: #f4f5f7;'></span></div>"
                  + "</a>"
                  + "<p class='text-center no-margin'>红-黑</p>");
    if (!IsUserNewPortal())
        skins_list.append(skin_purple);
    var skin_black =
          $("<li />", { style: "float:left; width: 50%; padding: 5px;" })
          .append("<a href='javascript:void(0);' data-skin='skin-black' style='display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4)' class='clearfix full-opacity-hover'>"
                  + "<div><span style='display:block; width: 20%; float: left; height: 12px;background-color : gray;'></span><span style='display:block; width: 80%; float: left; height: 12px;background-color : gray;'></span></div>"
                  + "<div><span style='display:block; width: 20%; float: left; height: 3px;background-color : #2379cc;'></span><span style='display:block; width: 80%; float: left; height: 3px;background-color : #2379cc;'></span></div>"
                  + "<div><span style='display:block; width: 20%; float: left; height: 32px; background: gray;'></span><span style='display:block; width: 80%; float: left; height: 32px; background: #f4f5f7;'></span></div>"
                  + "</a>"
                  + "<p class='text-center no-margin' style='font-size: 12px'>蓝-黑</p>");
    if (!IsUserNewPortal())
        skins_list.append(skin_black);
    var skin_blue =
          $("<li />", { style: "float:left; width: 50%; padding: 5px;" })
          .append("<a href='javascript:void(0);' data-skin='skin-blue' style='display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4)' class='clearfix full-opacity-hover'>"
          + "<div><span style='display:block; width: 20%; float: left; height: 12px;background-color : rgba(16,40,81,0.95);'></span><span style='display:block; width: 80%; float: left; height: 12px;background-color : rgba(16,40,81,0.95);'></span></div>"
                  + "<div><span style='display:block; width: 20%; float: left; height: 3px;background-color : rgba(16,40,81,0.95);'></span><span style='display:block; width: 80%; float: left; height: 3px;background-color : rgba(16,40,81,0.95);'></span></div>"
                  + "<div><span style='display:block; width: 20%; float: left; height: 32px; background: #dbe2e9;'></span><span style='display:block; width: 80%; float: left; height: 32px; background: #fff;'></span></div>"
                  + "</a>"
                  + "<p class='text-center no-margin' style='font-size: 12px'>蓝-白</p>");
    if (!IsUserNewPortal())
        skins_list.append(skin_blue);
    var skin_transparent_blue =
      $("<li />", { style: "float:left; width: 50%; padding: 5px;" })
      .append("<a href='javascript:void(0);' data-skin='skin_transparent_blue' style='display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4)' class='clearfix full-opacity-hover'>"
      + "<div><span style='display:block; width: 20%; float: left; height: 12px;background-color : rgba(16,40,81,0.95);'></span><span style='display:block; width: 80%; float: left; height: 12px;background-color : rgba(16,40,81,0.95);'></span></div>"
              + "<div><span style='display:block; width: 20%; float: left; height: 3px;background-color : rgba(16,40,81,0.95);'></span><span style='display:block; width: 80%; float: left; height: 3px;background-color : rgba(16,40,81,0.95);'></span></div>"
              + "<div><span style='display:block; width: 20%; float: left; height: 32px; background: #dbe2e9;'></span><span style='display:block; width: 80%; float: left; height: 32px; background: #fff;'></span></div>"
              + "</a>"
              + "<p class='text-center no-margin' style='font-size: 12px'>蓝-白</p>");
    var skin_transparent_yellow =
      $("<li />", { style: "float:left; width: 50%; padding: 5px;" })
      .append("<a href='javascript:void(0);' data-skin='skin_transparent_yellow' style='display: block; box-shadow: 0 0 3px rgba(0,0,0,0.4)' class='clearfix full-opacity-hover'>"
              + "<div><span style='display:block; width: 20%; float: left; height: 12px;background-color : #ffe9c6;'></span><span style='display:block; width: 80%; float: left; height: 12px;background-color : #ffe9c6;'></span></div>"
              + "<div><span style='display:block; width: 20%; float: left; height: 3px;background-color : #ffe9c6;'></span><span style='display:block; width: 80%; float: left; height: 3px;background-color : #ffe9c6;'></span></div>"
              + "<div><span style='display:block; width: 20%; float: left; height: 32px; background: #dbe2e9;'></span><span style='display:block; width: 80%; float: left; height: 32px; background: #fff;'></span></div>"
              + "</a>"
              + "<p class='text-center no-margin' style='font-size: 12px'>黄-白</p>");
    if (IsUserNewPortal()) {
        var portalKeys = GetPortalKeys();
        if (portalKeys != '') {
            var keys = portalKeys.split(',');
            if (keys.length > 0) {
                for (var i = 0; i < keys.length; i++) {
                    var skin = get('skin');
                    if (keys[i] == "Blue") {
                        skins_list.append(skin_transparent_blue);
                        if (i == 0) {
                            if (skin == 'skin_transparent_blue' || skin == null)
                                change_skin('skin_transparent_blue');
                        }
                    } else {
                        skins_list.append(skin_transparent_yellow);
                        if (i == 0) {
                            if (skin == 'skin_transparent_yellow' || skin == null)
                                change_skin('skin_transparent_yellow');
                        }
                    }
                }
            }
        } else {
            skins_list.append(skin_transparent_blue);
            skins_list.append(skin_transparent_yellow);
        }
    }

    demo_settings.append("<h4 class='control-sidebar-heading'>皮肤设置</h4>");
    demo_settings.append(skins_list);

    tab_pane.append(demo_settings);
    $("#control-sidebar-home-tab").after(tab_pane);

    setup();

    /**
    * Toggles layout classes
    * 
    * @param String cls the layout class to toggle
    * @returns void
    */
    function change_layout(cls) {
        $("body").toggleClass(cls);
        AdminLTE.layout.fixSidebar();
        //Fix the problem with right sidebar and layout boxed
        if (cls == "layout-boxed")
            AdminLTE.controlSidebar._fix($(".control-sidebar-bg"));
        if ($('body').hasClass('fixed') && cls == 'fixed') {
            AdminLTE.pushMenu.expandOnHover();
            AdminLTE.controlSidebar._fixForFixed($('.control-sidebar'));
            AdminLTE.layout.activate();
        }


    }

    /**
    * Replaces the old skin with the new skin
    * @param String cls the new skin class
    * @returns Boolean false to prevent link's default action
    */
    function change_skin(cls) {
        $.each(my_skins, function (i) {
            $("body").removeClass(my_skins[i]);
        });

        $("body").addClass(cls);

        store('skin', cls);

        var cssType = "/PortalLTE/portalLTERW.css";
        if (IsUserNewPortal()) {
            cssType = "/PortalLTE/portalLTE2018BL.css";
            if (cls == "skin_transparent_yellow") {
                cssType = "/PortalLTE/portalLTE2018BY.css";
            }
        }
        if (cls == "skin-purple")
        { }
        else if (cls == "skin-black")
        { cssType = "/PortalLTE/portalLTEBW.css"; }
        else if (cls == "skin-blue")
        { cssType = "/PortalLTE/portalLTEBL.css"; }
        else if (cls == "skin_transparent_blue")
        { cssType = "/PortalLTE/portalLTE2018BL.css"; }
        else if (cls == "skin_transparent_yellow")
        { cssType = "/PortalLTE/portalLTE2018BY.css"; }
        $("link[newappend='true']").remove();
        $("<link>").attr({
            rel: "stylesheet",
            type: "text/css",
            href: cssType,
            newappend: 'true'
        }).appendTo("head");

        return false;
    }

    function change_bg(backimg) {
        $("body").css("background-image", "url(" + backimg + ")");
        store('background-image', backimg);
        var stopIndex = backimg.lastIndexOf('.');
        store('background-image-login', backimg.substring(0, stopIndex) + "Login" + backimg.substring(stopIndex));
        return false;
    }
    /**
    * Store a new settings in the browser
    * 
    * @param String name Name of the setting
    * @param String val Value of the setting
    * @returns void
    */
    function store(name, val) {
        setCache(name, val);
    }

    /**
    * Get a prestored setting
    * 
    * @param String name Name of of the setting
    * @returns String The value of the setting | null
    */
    function get(name) {
        return getCache(name);
    }

    /**
    * Retrieve default settings and apply them to the template
    * 
    * @returns void
    */
    function setup() {
        var tmp = get('skin');
        if (tmp && $.inArray(tmp, my_skins))
            change_skin(tmp);


        //Add the change skin listener
        $("[data-skin]").on('click', function (e) {
            e.preventDefault();
            change_skin($(this).data('skin'));
            if (IsUserNewPortal()) {
                location.reload(true);
            }
        });

        //        var bg = get("background-image");
        //        if (bg == null)
        //            bg = "/PortalLTE/Images/default.jpg";
        //        if (bg && $.inArray(bg, bg_list.map(function (item) { return item.data; })))
        //            change_bg(bg);
        $("[data-bg]").on("click", function (e) {
            e.preventDefault();
            change_bg($(this).data('bg'));
        });


        //Add the layout manager
        $("[data-layout]").on('click', function () {
            change_layout($(this).data('layout'));
        });

        $("[data-controlsidebar]").on('click', function () {
            change_layout($(this).data('controlsidebar'));
            var slide = !AdminLTE.options.controlSidebarOptions.slide;
            AdminLTE.options.controlSidebarOptions.slide = slide;
            if (!slide)
                $('.control-sidebar').removeClass('control-sidebar-open');
        });

        $("[data-sidebarskin='toggle']").on('click', function () {
            var sidebar = $(".control-sidebar");
            if (sidebar.hasClass("control-sidebar-dark")) {
                sidebar.removeClass("control-sidebar-dark")
                sidebar.addClass("control-sidebar-light")
            } else {
                sidebar.removeClass("control-sidebar-light")
                sidebar.addClass("control-sidebar-dark")
            }
        });

        $("[data-enable='expandOnHover']").on('click', function () {
            $(this).attr('disabled', true);
            AdminLTE.pushMenu.expandOnHover();
            if (!$('body').hasClass('sidebar-collapse'))
                $("[data-layout='sidebar-collapse']").click();
        });

        // Reset options
        if ($('body').hasClass('fixed')) {
            $("[data-layout='fixed']").attr('checked', 'checked');
        }
        if ($('body').hasClass('layout-boxed')) {
            $("[data-layout='layout-boxed']").attr('checked', 'checked');
        }
        if ($('body').hasClass('sidebar-collapse')) {
            $("[data-layout='sidebar-collapse']").attr('checked', 'checked');
        }

    }
})(jQuery, $.AdminLTE);