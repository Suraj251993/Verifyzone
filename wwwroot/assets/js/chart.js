function drawDashboard(_url) {

    // Show loader overlay
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    var chartDom = document.getElementById('main');
    var myChart = echarts.init(chartDom);
    var yr = new Date().getFullYear();
    
    $.ajax({
        type: 'GET',
        data: { year: yr.toString() },
        dataType: 'JSON',
        url: _url,
        success:
            function (response) {
                var reports = [];
                var reportmaxcount = 0;
                for (let i = 0; i < response.reportcount.length; i++) {
                    reports.push(parseInt(response.reportcount[i]));
                    if (parseInt(response.reportcount[i]) > reportmaxcount)
                        reportmaxcount = parseInt(response.reportcount[i]);
                }
                var approvals = [];
                if (response.approvalcount) {
                    for (let i = 0; i < response.approvalcount.length; i++) {
                        approvals.push(parseInt(response.approvalcount[i]));
                        if (parseInt(response.approvalcount[i]) > reportmaxcount)
                            reportmaxcount = parseInt(response.approvalcount[i]);
                    }
                }

                var isDark = window.isVzThemeDark ? window.isVzThemeDark() : false;
                var textColor = isDark ? '#f1ecf7' : '#211a2b';
                var subColor = isDark ? '#b3a5c4' : '#7a7186';
                var lineColor = isDark ? '#382a4c' : '#ece4f7';

                var option = {
                    tooltip: {
                        trigger: 'axis'
                    },
                    legend: {
                        top: 0,
                        right: '5%',
                        textStyle: { fontSize: 12, fontFamily: 'IBM Plex Sans, Arial, sans-serif', color: subColor }
                    },
                    grid: {
                        left: '5%',
                        right: '5%',
                        bottom: '5%',
                        top: 35,
                        containLabel: true
                    },
                    xAxis: {
                        type: 'category',
                        data: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'June', 'July', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                        axisLabel: { fontSize: 12, color: subColor },
                        axisLine: { lineStyle: { color: lineColor } }
                    },
                    yAxis: {
                        type: 'value',
                        min: 0,
                        max: reportmaxcount,
                        //interval: 0.5,
                        axisLabel: {
                            fontSize: 12,
                            color: subColor,
                            formatter: function (value) {
                                return value.toFixed(1);
                            }
                        },
                        splitLine: { lineStyle: { color: lineColor } }
                    },
                    series: [
                        {
                            name: 'Reports generated',
                            type: 'line',
                            data: reports,
                            smooth: true,
                            symbol: 'circle',
                            symbolSize: 6,
                            lineStyle: { color: '#5c249a', width: 2.5 },
                            itemStyle: { color: '#5c249a' },
                            areaStyle: {
                                color: {
                                    type: 'linear', x: 0, y: 0, x2: 0, y2: 1,
                                    colorStops: [
                                        { offset: 0, color: 'rgba(92, 36, 154, 0.28)' },
                                        { offset: 1, color: 'rgba(92, 36, 154, 0)' }
                                    ]
                                }
                            },
                            emphasis: {
                                focus: 'series'
                            }
                        },
                        {
                            name: 'Approvals',
                            type: 'line',
                            data: approvals,
                            smooth: true,
                            symbol: 'circle',
                            symbolSize: 6,
                            lineStyle: { color: '#8546cd', width: 2.5 },
                            itemStyle: { color: '#8546cd' },
                            areaStyle: {
                                color: {
                                    type: 'linear', x: 0, y: 0, x2: 0, y2: 1,
                                    colorStops: [
                                        { offset: 0, color: 'rgba(133, 70, 205, 0.18)' },
                                        { offset: 1, color: 'rgba(133, 70, 205, 0)' }
                                    ]
                                }
                            },
                            emphasis: {
                                focus: 'series'
                            }
                        }
                    ]
                };

                myChart.setOption(option);
                window.addEventListener('resize', () => myChart.resize());

                myChart.on('finished', () => {

                    if (myChart.__resizeObserver) return;

                    const resizeObserver = new ResizeObserver(() => {
                        myChart.resize();
                    });
                    resizeObserver.observe(chartDom);

                    myChart.__resizeObserver = resizeObserver;
                });

                // Hide loader
                overlay.style.display = "none";
            },
        error:
            function (response) {
                // Hide loader
                overlay.style.display = "none";
                console.log(response);
                showErrorMessage("Error: " + response.statusText);
            }
    });

}