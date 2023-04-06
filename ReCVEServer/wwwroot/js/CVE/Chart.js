$(document).ready(function () {
    $.ajax({
        url: '@Url.Action("GetCveChartData", "YourControllerName")',
        method: 'GET',
        dataType: 'json',
        success: function (data) {
            var chartData = data.map(function (item) {
                return {
                    name: 'Base Score: ' + item.baseScore,
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
        },
        error: function (error) {
            console.error('Error fetching CVE chart data:', error);
        }
    });
});