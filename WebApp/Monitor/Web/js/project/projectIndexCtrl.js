/**
 * 产品名称：Web.
 * 创建用户： 秦佳.
 * 创建时间： 2015/12/11.
 * 创建原因：
 */
//获取数据
$monitor.getRawData("ProjectIndex",function(info){
    //数据绑定
    var chartData = {"departments":[],"delay":[],"execute":[]};
    //处理图表数据
    if(info.DepartmentInfo.length!==0){
        $.each(info.DepartmentInfo,function(n,value) {
            chartData.departments.push(value.Name);
            chartData.delay.push(value.DelayProject);
            chartData.execute.push(value.ExecuteProject);
        });
    }
    way.set("projectIndex",info);
    //在建项目信息柱状图
    var projectChart = echarts.init(document.getElementById('project-chart'));
    var projectChartOption = {
        tooltip : {
            trigger: 'axis'
        },
        calculable : false,
        grid:{
            /*x:'10%',width:'90%',
            y:'5%',
            x2:5*/
            x:50,
            x2:20,
            y:5,
            y2:20,
            height:'80%'
        },
        xAxis : [
            {
                type : 'category',
                data : chartData.departments
            }
        ],
        yAxis : [
            {
                type : 'value',
                axisLabel:{formatter:'{value}'}
            }
        ],
        series : [
            {
                name:'延期项目数',
                type:'bar',
                stack:'group1',
                itemStyle: {normal: {color:'#ffb215'}},
                data:chartData.delay/*,
                markPoint : {
                    data : [
                        {type : 'max', name: '最大值'},
                        {type : 'min', name: '最小值'}
                    ]
                }
*/
            },
            {
                name:'在建项目数',
                type:'bar',
                stack:'group1',
                itemStyle: {normal: {color:'#6396ec'}},
                data:chartData.execute
/*                markPoint : {
                    data : [
                        {type : 'max', name: '最大值'},
                        {type : 'min', name: '最小值'}
                    ]
                }*/
            }
        ]
    };
    projectChart.setOption(projectChartOption);
    setTimeout(    $monitor.scroll(),500);
//时间过滤器
    way.registerTransform("dateFormatter", function(date) {
        return date ? date.split('T')[0]:"--";
    });
    //专业过滤器
    way.registerTransform("majorFormatter", function(major) {
        if(major){
            var newData = jQuery.parseJSON(major);
            var str = '';
            $.each(newData,function(n,value) {
                str = (n===0? value.Name:str+','+value.Name);
            });
            return str;
        }
        return '无';
    });
});
                    