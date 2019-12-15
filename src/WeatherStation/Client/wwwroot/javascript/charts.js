google.charts.load('current', { packages: ['corechart'] });
google.charts.setOnLoadCallback(drawSingleLineChart);


function formatData(headers, data) {
    let arr = [];
    for (let i = 0; i < data.length; i += headers.length) {
        let temp_data = data.slice(i, i + headers.length);

        arr.push({
            time: temp_data[0],
            device: temp_data[1],
            value: temp_data[2]
        });
    }

    let deviceGrouped = _.groupBy(arr, function (r) {
        return r.device;
    });

    let deviceHeaders = [];
    for (let deviceKey in deviceGrouped) {
        deviceHeaders.push(deviceKey);
    }

    let timeGrouped = _.groupBy(arr, function (r) {
        return r.time;
    });

    let dataHeaders = ['DateTime'];
    for (let header of deviceHeaders) {
        dataHeaders.push(header);
    }

    let dataArray = [dataHeaders];
    for (let timeKey of Object.keys(timeGrouped)) {
        //console.log('timeGrouped[timeKey]: ', timeGrouped[timeKey]);
        for (let recObj of timeGrouped[timeKey]) {
            let headerPos = deviceHeaders.indexOf(recObj.device);

            let row = [recObj.time];
            for (let h of deviceHeaders) {
                row.push(undefined);
            }

            row[headerPos + 1] = Number(recObj.value);

            dataArray.push(row);

            //console.log('headerPos: ', headerPos);
            //console.log('recObj: ', recObj);
        }
        
    }

    //console.log('dataArray: ', dataArray);

    return dataArray;
}

function drawSingleLineChart(element_id, chart_name, chart_data) {

    console.log('Element ID: ', element_id);
    console.log('Chart Name: ', chart_name);
    console.log('Chart Data: ', chart_data);

    let formatted = formatData(["DateTime", "Device", "Value"], chart_data);

    console.log('Formatted: ', JSON.stringify(formatted));

    var data = new google.visualization.arrayToDataTable(formatted);
    
    //var data = new google.visualization.arrayToDataTable([
    //    ['Time', 'Temperature_C', 'Humidity'],
    //    ['20:00', 23, 100],
    //    ['21:00', 22, 98],
    //    ['22:00', 18, 60],
    //    ['23:00', 15, 66],
    //    ['00:00', 13, 75],
    //    ['01:00', 8, 80],
    //    ['02:00', 5, 85],
    //    ['03:00', 4, 90],
    //    ['04:00', 3, 95],
    //    ['05:00', 2, 99]
    //]);

    var options = {
        title: chart_name,
        curveType: 'none',
        legend: { position: 'bottom' },
        animation: {
            startup: true,
            duration: 300,
            easing: 'out'
        }
    };

    var chart = new google.visualization.LineChart(document.getElementById(element_id));
    chart.draw(data, options);
}
