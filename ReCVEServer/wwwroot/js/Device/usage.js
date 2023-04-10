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
    const response = await fetch('/Device/GetSystemInfo'); // Replace this with the correct URL for your action method.
    const latestData = await response.json();

    const currentTime = new Date().getTime();
    const cpuUsageData = latestData.cpu;
    const memoryUsageData = latestData.memory;

    // Add new points to the chart
    chart.series[0].addPoint([currentTime, cpuUsageData], true, false);
    chart.series[1].addPoint([currentTime, memoryUsageData], true, false);
}

setInterval(updateChartData, 5000);