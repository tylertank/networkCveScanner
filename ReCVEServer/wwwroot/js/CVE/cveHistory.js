$(document).ready(function () {
    fetchHistoryData("/nist/getCveHistory", "cveHistoryChart", historyChart)
});

function fetchHistoryData(url, elementId, onSuccess) {

    fetch(url)
        // Rest of the function remains the same
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            onSuccess(data, elementId);
        })
        .catch(error => {
            console.error('Error fetching CVE chart data:', error);
        });
}

function historyChart(data, elementId) {
    var chartData = data.map(function (item) {
        return {
            name: item.date,
            y: item.cveScore
        };
    });

    // Create the Highcharts pie chart
    Highcharts.chart(elementId, {
        chart: {
            type: 'line',
            animation: Highcharts.svg
        },
        title: {
            text: 'CVE Score Over Time'
        },
        xAxis: {
            type: 'category'
        },
        yAxis: {
            title: {
                text: 'Score'
            }
        },
        series: [
            {
                name: 'Average CVE Score',
                data: chartData
            },
         
        ]
    });
}
