function GetDashboardData(_url) {
    const monthNames = ["January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    var date = new Date();
    var url = _url + '?month=' + (date.getMonth() + 1) + '&year=' + date.getFullYear();
    var type = 'GET';
    $.ajax({
        url: url,
        type: type,
        processData: false,
        contentType: false,
        error: function (xhr) {
            console.log(xhr);
            showErrorMessage('Error: ' + xhr.statusText);
        },
        success: function (response) {
            for (let i = 0; i < response.data.length; i++) {
                if (response.data[i].text == "Customers") {
                    $("#customerslabel").html(response.data[i].value);
                    $("#customerssubtitle").html("as on " + monthNames[date.getMonth()] + ", " + date.getFullYear());
                }
                else if (response.data[i].text == "Reports") {
                    $("#reportslabel").html(response.data[i].value);
                }
                else if (response.data[i].text == "Companies") {
                    $("#bgvslabel").html(response.data[i].value);
                    $("#bgvssubtitle").html("as on " + monthNames[date.getMonth()] + ", " + date.getFullYear());
                }
                else if (response.data[i].text == "Users") {
                    $("#userslabel").html(response.data[i].value);
                }
            }
            if (response.yearwisecount && document.getElementById("adminMain")) {
                drawAdminYearChart(response.yearwisecount);
            }
            if (response.rcviewmodels && document.getElementById("topCustomersList")) {
                renderTopCustomers(response.rcviewmodels);
            }
            renderTrend("customerstrend", response.customerstrend);
            renderTrend("reportstrend", response.reportstrend);
        },
        async: true,
        processData: false
    });
}

function drawAdminYearChart(yearwisecount) {
    var months = yearwisecount.map(function (m) { return m.customerName; });
    var counts = yearwisecount.map(function (m) { return parseInt(m.count) || 0; });
    var chartDom = document.getElementById('adminMain');
    var chart = echarts.init(chartDom);
    chart.setOption({
        tooltip: { trigger: 'axis' },
        grid: { left: '4%', right: '4%', bottom: '6%', top: '6%', containLabel: true },
        xAxis: { type: 'category', data: months, axisLabel: { fontSize: 12 } },
        yAxis: { type: 'value', axisLabel: { fontSize: 12 } },
        series: [{
            name: 'Reports generated',
            type: 'bar',
            data: counts,
            itemStyle: { color: '#5c249a', borderRadius: [6, 6, 0, 0] },
            barWidth: '50%'
        }]
    });
    window.addEventListener('resize', function () { chart.resize(); });
}

function renderTrend(elementId, trendValue) {
    var el = document.getElementById(elementId);
    if (!el || trendValue === null || trendValue === undefined) return;
    var isUp = trendValue >= 0;
    el.className = "exz-trend " + (isUp ? "up" : "down");
    el.innerHTML = '<i class="ri-arrow-' + (isUp ? "right-up" : "right-down") + '-line"></i>' + Math.abs(trendValue) + '%';
    el.style.display = "flex";
}

function renderTopCustomers(rcviewmodels) {
    var top = rcviewmodels
        .slice()
        .sort(function (a, b) { return (parseInt(b.count) || 0) - (parseInt(a.count) || 0); })
        .slice(0, 5);
    var html = top.map(function (c) {
        var initials = (c.customerName || "?").split(" ").map(function (w) { return w[0]; }).slice(0, 2).join("").toUpperCase();
        return '<div class="admin-rank-row">' +
            '<div class="admin-rank-avatar">' + initials + '</div>' +
            '<div class="admin-rank-name">' + c.customerName + '</div>' +
            '<div class="admin-rank-count">' + c.count + '</div>' +
            '</div>';
    }).join('');
    document.getElementById("topCustomersList").innerHTML = html || '<p class="admin-rank-empty">No reports generated yet this month.</p>';
}