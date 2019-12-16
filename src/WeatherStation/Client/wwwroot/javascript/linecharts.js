google.charts.load('current', { packages: ['corechart'] });

function mergeTimeArrays(timeArray) {
    let a = [timeArray[0][0]];

    for (let i = 1; i < timeArray[0].length; i++) {
        let p_val = undefined;
        for (let j = 0; j < timeArray.length; j++) {
            if (timeArray[j][i] != undefined) {
                p_val = timeArray[j][i];
                break;
            }
        }
        a.push(p_val);
        p_val = undefined;
    }

    return a;
}

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
        let timeArray = [];

        for (let recObj of timeGrouped[timeKey]) {
            let headerPos = deviceHeaders.indexOf(recObj.device);

            let row = [recObj.time];
            for (let h of deviceHeaders) {
                row.push(undefined);
            }

            row[headerPos + 1] = Number(recObj.value);
            timeArray.push(row);
        }

        let timeStamp = mergeTimeArrays(timeArray);
        dataArray.push(timeStamp);
        
    }

    return dataArray;
}

function drawSingleLineChart(element_id, chart_name, chart_data) {

    let formatted = formatData(["DateTime", "Device", "Value"], chart_data);

    var data = new google.visualization.arrayToDataTable(formatted);
    
    var options = {
        title: chart_name,
        curveType: 'function',
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
