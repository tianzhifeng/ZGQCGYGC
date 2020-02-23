
var targetPieChart = {
    chart: {
        plotBackgroundColor: null,
        plotBorderWidth: null,
        plotShadow: false,
        spacing: [-10, -10, -10, -10]
    },
    title: {
        floating: true,
        text: '100%'
    },
    tooltip: {
        enabled: false,
        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },
    credits: {
        enabled: false
    },
    plotOptions: {
        series: {
            enableMouseTracking: false
        },
        pie: {
            lineWidth: 3,
            allowPointSelect: false,
            cursor: 'pointer',
            dataLabels: {
                enabled: false,
                format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                style: {
                    color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'red'
                }
            },
            point: {
            },
        }
    },
    series: [{
        type: 'pie',
        innerSize: '80%',
        name: '完成率',
        data: []
    }]
};


var standardPieChart = {
    title: {
        text: '',
    },
    tooltip: {
        //headerFormat: '{series.name}<br>',
        pointFormat: '{point.name}: <b>{point.percentage:.1f}%</b>'
    },
    credits: {
        enabled: false
    },
    plotOptions: {
        pie: {
            allowPointSelect: true,  // 可以被选择
            cursor: 'pointer',       // 鼠标样式
            dataLabels: {
                enabled: true,
                format: '<b>{point.name}</b>: {point.percentage:.1f} ',
                style: {
                    color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                }
            },
            showInLegend: false
        },
        series: {
                events: {
                          click: function(e) {
                                   if (e.point.url!=undefined &&  e.point.url!=null &&e.point.url!="") 
                                   {
                                          openWindow( e.point.url, { width: "80%", height: "70%" });
                                   }
                                            }
                         }
        }
    },
    legend: {
        layout: 'vertical',
        verticalAlign: 'top',
        align: 'right',
        floating: false
    },
    series: [{
        type: 'pie',
        name: '',
        minSize: 100,
        data: []
    }]
};

var columnChart = {
    chart: {
    },
    title: {
        text: ''
    },
    credits: {
        enabled: false
    },
    xAxis: [{
        categories: [],
        crosshair: true
    }],
    yAxis: [],
    tooltip: {
        shared: true
    },
    legend: {
        align: 'center',
        verticalAlign: 'bottom',
        backgroundColor: (Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'
    },
    plotOptions: {
        series: {
                events: {
                          click: function(e) {
                            console.info(e);
                                   if (e.point.url!=undefined &&  e.point.url!=null &&e.point.url!="") 
                                   {
                                          openWindow( e.point.url, { width: "80%", height: "70%" });
                                   }
                                            }
                         }
          }
  },

    series: []
}

var solidgaugeChart = {
    chart: {
        type: 'solidgauge'
    },
    title: null,
    pane: {
        center: ['50%', '85%'],
        size: '195px',
        startAngle: -90,
        endAngle: 90,
        background: {
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || '#EEE',
            innerRadius: '60%',
            outerRadius: '100%',
            shape: 'arc'
        }
    },
    tooltip: {
        enabled: false
    },
    credits: {
        enabled: false
    },
    yAxis: {
        stops: [
			[0.1, '#55BF3B'] // green
        ],
        lineWidth: 0,
        minorTickInterval: null,
        tickPixelInterval: 400,
        tickWidth: 0,
        title: {
            y: -70
        },
        labels: {
            y: 16
        }
    },
    plotOptions: {
        solidgauge: {
            dataLabels: {
                y: 5,
                borderWidth: 0,
                useHTML: true
            }
        }
    }
}

var solidgaugeChart = {
    chart: {
        type: 'solidgauge',
        spacingBottom: 3
    },
    title: null,
    pane: {
        center: ['50%', '90%'],
        size: '180%',
        startAngle: -90,
        endAngle: 90,
        background: {
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || '#EEE',
            innerRadius: '60%',
            outerRadius: '100%',
            shape: 'arc'
        }
    },
    tooltip: {
        enabled: false
    },
    credits: {
        enabled: false
    },
    yAxis: {
        stops: [
            [0.1, '#DF5353'], // red
            [0.5, '#DDDF0D'], // yellow
            [0.9, '#55BF3B'] // green
        ],
        minorTickInterval: 'auto',
        minorTickWidth: 1,
        minorTickLength: 10,
        minorTickPosition: 'inside',
        minorTickColor: '#666',
        tickPixelInterval: 30,
        tickWidth: 2,
        tickPosition: 'inside',
        tickLength: 10,
        tickColor: '#666',
        labels: {
            step: 2,
            rotation: 'auto'
        },
    },
    plotOptions: {
        solidgauge: {
            dataLabels: {
                y: 10,
                borderWidth: 0,
                useHTML: true
            }
        }
    }
};

function onChartDrawingComplete(c) {
    var centerY = c.series[0].center[1],
        titleHeight = parseInt(c.title.styles.fontSize);
    c.setTitle({
        y: centerY + titleHeight / 2
    });
}


