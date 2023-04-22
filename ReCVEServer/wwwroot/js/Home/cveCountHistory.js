$(document).ready(function () {
    fetchCountData("/nist/getCveHistory", "cveCountChart", countChart)
});

function fetchCountData(url, elementId, onSuccess) {

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

function countChart(data, elementId) {
    var chartData = data.map(function (item) {
        return {
            name: item.date,
            y: item.totalCount
        };
    });

    // Create the Highcharts pie chart
    Highcharts.chart('cveCountChart', {
        chart: {
            type: 'line',
            animation: Highcharts.svg
        },
        title: {
            text: 'CVE Count Over Time'
        },
        xAxis: {
            type: 'category'
        },
        yAxis: {
            title: {
                text: 'Number of CVEs'
            }
        },
        series: [
            {
                name: 'Number of CVEs',
                data: chartData
            },

        ]
    });
}
