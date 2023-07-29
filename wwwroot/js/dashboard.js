var serverUrl = 'https://corp.cloudinteractive.net';

function get_latency() {
    var startTime = new Date().getTime();

    fetch(serverUrl, { mode: 'no-cors' })
        .then(function () {
            var endTime = new Date().getTime();
            var latency = endTime - startTime;
            document.getElementById('latency').innerText = latency + ' ms';
            set_statebtn(latency);
        })
        .catch(function (error) {
            console.error('Error:', error);
            document.getElementById('latency').innerText = '-- ms';
            set_statebtn(-1);
        });
}

function set_statebtn(ping) {
    var color;
    if (ping == -1) {
        color = "red";
    }
    else if (ping <= 50) {
        color = "green";
    }
    else if (ping <= 200) {
        color = "yellow";
    } else {
        color = "red";
    }
    document.getElementById('statebtn').className = color + " statebtn btn-floating pulse";
}

get_latency();
setInterval(get_latency, 5000);