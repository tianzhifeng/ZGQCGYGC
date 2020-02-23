Scrollbar.initAll();
mini.parse();
//        var treegird=mini.get("treegrid1");


var tree1=mini.get("tree1");

tree1.loadData([
        { text: "工程", expanded: false,
            children: [
                {  text: "工程" },
                { text: "工程项目" },
                { text: "排水 " }
            ]
        },
        {text: "排水", expanded: false,
            children: [
                {text: "工程项目工程项目" },
                {text: "工程项目工程项目" },
                {text: "工程项目工程项目 ",
                    children:[
                        { text: "工程项目工程项目" },
                        {  text: "工程项目工程项目" }
                    ]
                }
            ]
        },
        { text: "排水排水", expanded: false,
            children: [
                { text: "工程项目工程项目" },
                { text: "工程项目工程项目" },
                { text: "工程项目工程项目工程项目" },
                {  text: "工程项目工程项目工程项目工程项目工程项目",
                    children:[
                        {  text: "工程项目工程项目工程项目工程项目" },
                        {  text: "工程项目工程项目工程项目工程项目" }
                    ]
                }
            ]
        }
    ]
);
//change 
$(".close-slider").on("click",function(){
    for(var i=0;i<$("#iframe-container .iframe-item",window.parent.document).length;i++){
        if($("#iframe-container .iframe-item",window.parent.document).eq(i).css("display")=="block"){
            $("#iframe-container .iframe-item",window.parent.document).eq(i).find(".item-content").animate({
                "padding-left":"44px"
            },300);
            $("#iframe-container .iframe-item",window.parent.document).eq(i).find(".item-slider").animate({
               left:"-250px"
            },300,function(){
                $(".control-img",window.parent.document).show();
            });
        }
    }
});
//change 