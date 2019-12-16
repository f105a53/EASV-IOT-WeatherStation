google.charts.load('current', { 'packages': ['gauge'] });

function drawChart(element_id, chart_data ) {

    console.log('chart_data: ', JSON.stringify(chart_data));
    let dt = google.visualization.arrayToDataTable([['Name', 'Value'], ['', Number(chart_data[1])]]);

    let width = (window.innerWidth > 0) ? window.innerWidth : screen.width;
    let options = {};

    let type = element_id.substr(element_id.length - 'humidity'.length);
    if (type == 'humidity') {
        options = {
            width: 300, height: 300,
            greenFrom: 30, greenTo: 50,
            yellowFrom: 50, yellowTo: 60,
            redFrom: 60, redTo: 100,
            minorTicks: 5
        };
    } else {
        options = {
            width: 300, height: 300,
            min: -20, max: 60,
            redFrom: -20, redTo: 10,
            yellowFrom: 10, yellowTo: 15,
            greenFrom: 15, greenTo: 25,
            minorTicks: 5
        };
    }

    let chart = new google.visualization.Gauge(document.getElementById(element_id));
    chart.draw(dt, options);
}