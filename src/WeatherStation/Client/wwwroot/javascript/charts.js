google.charts.load('current', { packages: ['corechart'] });
google.charts.setOnLoadCallback(drawChart);

function drawHistoryChart(element) {
    // Define chart
    var data = new google.visualization.arrayToDataTable([
        ['Time', 'Temperature_C', 'Humidity'],
        ['20:00', 23, 100],
        ['21:00', 22, 98],
        ['22:00', 18, 60],
        ['23:00', 15, 66],
        ['00:00', 13, 75],
        ['01:00', 8, 80],
        ['02:00', 5, 85],
        ['03:00', 4, 90],
        ['04:00', 3, 95],
        ['05:00', 2, 99]
    ]);

    var options = {
        title: 'Humidity Chart for <wireless_module>',
        curveType: 'none',
        legend: { position: 'bottom' },
        animation: {
            startup: true,
            duration: 300,
            easing: 'out'
        }
    }

    var chart = new google.visualization.LineChart(document.getElementById(element));
    chart.draw(data, options);
}
