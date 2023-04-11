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
        console.log("getting info")
    $.get({
        url: "/device/GetSystemInfo/",
        data: { computerID: client }
    }).done(function (response) {
        console.log("got info")
        const currentTime = new Date().getTime();

        const cpuUsageData = response.reduce((total, status) => total + status.cpu, 0);
        const memoryUsageData = response.reduce((total, status) => total + status.memory, 0);

        chart.series[0].addPoint([currentTime, cpuUsageData], true, false);
        chart.series[1].addPoint([currentTime, memoryUsageData], true, false);

    }).catch(error => {
        window.location.reload();
        console.log("Error");
    });
}
let intervalId;
function callUpdateChart() {
    if (!intervalId) { // Check if the interval is not already running
        intervalId = setInterval(updateChartData, 1000); // Start the interval and store the ID
    } else {
        clearInterval(intervalId); // Stop the interval
        intervalId = null; // Clear the interval ID
    }
}
function stopInterval() {
    if (intervalId) { // Check if the interval is running
        clearInterval(intervalId); // Stop the interval
        intervalId = null; // Clear the interval ID
    }
}