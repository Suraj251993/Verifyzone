var chartDom = document.getElementById('main');
var myChart;

function renderChart() {
    if (myChart) {
        myChart.dispose(); // destroy previous chart instance
    }

    myChart = echarts.init(chartDom);

    var option = {
        tooltip: { trigger: 'item' },
        legend: {
            orient: 'horizontal',
            bottom: 0,
            left: 'center',
            icon: 'circle'
        },
        series: [
            {
                name: 'Status',
                type: 'pie',
                radius: ['20%', '70%'],
                center: ['50%', '45%'],
                data: [
                    { value: 28, name: 'Valid', itemStyle: { color: '#47e76dff' } },
                    { value: 24, name: 'Invalid', itemStyle: { color: '#ec4d5dff' } }
                ]
            }
        ]
    };

    myChart.setOption(option);
    myChart.resize();
}

// When modal is fully open
document.getElementById('uploadPop')
    .addEventListener('shown.bs.modal', function () {
        setTimeout(() => {
            renderChart();
        }, 5); // short delay ensures perfect size
    });

// Responsive
window.addEventListener("resize", function () {
    if (myChart) myChart.resize();
});
