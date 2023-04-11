const chart = Highcharts.chart('chart-container', {
    chart: {
        type: 'line',
        animation: Highcharts.svg
    },
    title: {
        text: 'CPU and Memory Usage'
    },
    xAxis: {
        type: 'datetime'
    },
    yAxis: {
        title: {
            text: 'Usage'
        }
    },
    series: [
        {
            name: 'CPU Usage',
            data: []
        },
        {
            name: 'Memory Usage',
            data: []
        }
    ]
});

async function updateChartData() {

    var client = $("#client").val();
    $.get({

        url: "/device/GetSystemInfo/",
        data: { computerID: client }

    }).done(function (response) {
   
    const currentTime = new Date().getTime();
    const cpuUsageData = response.cpu;
    const memoryUsageData = response.memory;
    chart.series[0].addPoint([currentTime, cpuUsageData], true, false);
    chart.series[1].addPoint([currentTime, memoryUsageData], true, false);
    setInterval(updateChartData, 5000);

    }).catch(error => {
        window.location.reload();
        console.log("Error");
    });

 
}
