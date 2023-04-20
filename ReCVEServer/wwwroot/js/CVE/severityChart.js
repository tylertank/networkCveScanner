$(function () {
    fetchCveChartData("/nist/getSeverityRating", "cvePieChart", createChart)
});

function fetchCveChartData(url, elementId, onSuccess) {
  
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

function createChart(data, elementId) {
    var chartData = data.map(function (item) {
        return {
            name: item.baseSeverity,
            y: item.count
        };
    });

    // Create the Highcharts pie chart
    Highcharts.chart('cvePieChart', {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: 'CVE Base Scores Distribution'
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        accessibility: {
            point: {
                valueSuffix: '%'
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %'
                }
            }
        },
        series: [{
            name: 'CVEs',
            colorByPoint: true,
            data: chartData
        }]
    });
}
