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

                var option = {
                    title: [
                        {
                            text: 'Dashboard statistics',
                            left: 'left',
                            top: 10,
                            textStyle: {
                                fontSize: 18,
                                fontFamily: 'IBM Plex Sans, Arial, sans-serif',
                                fontWeight: 600,
                                color: '#333'
                            }
                        },
                        {
                            text: 'Month-wise count of reports generated for the year - ' + yr,
                            left: 'left',
                            top: 40,
                            textStyle: {
                                fontSize: 13,
                                fontFamily: 'IBM Plex Sans, Arial, sans-serif',
                                fontWeight: 400,
                                color: '#777',
                                width: 500,
                                align: 'center',
                                lineHeight: 18
                            }
                        }
                    ],
                    tooltip: {
                        trigger: 'axis'
                    },
                    grid: {
                        left: '5%',
                        right: '5%',
                        bottom: '5%',
                        top: 100,
                        containLabel: true
                    },
                    xAxis: {
                        type: 'category',
                        data: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'June', 'July', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                        axisLabel: {
                            fontSize: 12
                        }
                    },
                    yAxis: {
                        type: 'value',
                        min: 0,
                        max: reportmaxcount,
                        //interval: 0.5,
                        axisLabel: {
                            fontSize: 12,
                            formatter: function (value) {
                                return value.toFixed(1);
                            }
                        }
                    },
                    series: [
                        {
                            name: 'reports',
                            type: 'bar',
                            //data: [0.5, 1.0, 0.8, 0.4, 0.3, 0.6, 0.7, 1.2, 1.4, 1.8, 2.1, 2.4],
                            data: reports,
                            itemStyle: {
                                color: '#5c249a',
                                borderRadius: [6, 6, 0, 0]
                            },
                            barWidth: '50%',
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