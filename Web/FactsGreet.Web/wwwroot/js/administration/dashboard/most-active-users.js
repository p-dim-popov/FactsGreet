(function () {
    String.prototype.getHashCode = function () {
        return this.toString().split('').reduce((a, b) => {
            a = ((a << 5) - a) + b.charCodeAt(0);
            return a & a
        }, 0)
    };

    const usersData = JSON.parse(document.getElementById('users-data').value);
    console.log(usersData);
    
    const lineChart = new Chart(document.getElementById('line-chart').getContext('2d'),
        {
            type: 'line',
            data: {
                datasets: usersData.map(x => {
                    return {
                        label: x.email,
                        backgroundColor: `rgba(${(x.email.getHashCode() >> 16) & 0xFF},${(x.email.getHashCode() >> 8) & 0xFF},${(x.email.getHashCode() >> 0) & 0xFF}, 0.6)`,
                        data: x.edits.reduce((acc, {createdOn}) => {
                            createdOn = createdOn.split('T')[0]
                            const index = acc.findIndex(y => y.x === createdOn)
                            if (index === -1) {
                                acc[acc.length] = {x: createdOn, y: 1};
                                return acc;
                            }
                            acc[index].y++;
                            return acc;
                        }, [])
                            .sort((a, b) => a.x.localeCompare(b.x)),
                        pointRadius: 10,
                        pointHoverRadius: 15,
                        showLine: false,
                    };
                })
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                title: {
                    display: true,
                    text: 'Users activity'
                },
                scales: {
                    xAxes: [{
                        type: 'time',
                        time: {
                            parser: 'YYYY-MM-DD',
                            unit: 'day'
                        },
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: 'Day'
                        },
                    }],
                    yAxes: [{
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: 'Activity count'
                        }
                    }]
                }
            }
        });
    console.log(lineChart.config);
    
    const barChart = new Chart(document.getElementById('bar-chart').getContext('2d'), {
        type: 'bar',
        data: {
            labels: usersData.map(x => x.email),
            datasets: usersData.map((x, i) => {
                return {
                    label: x.email,
                    backgroundColor: `rgba(${(x.email.getHashCode() >> 16) & 0xFF},${(x.email.getHashCode() >> 8) & 0xFF},${(x.email.getHashCode() >> 0) & 0xFF}, 0.6)`,
                    data: Array(usersData.length).fill(0).map((y, i1) => i === i1 ? x.edits.length : 0)
                };
            })
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            title: {
                display: true,
                text: 'Users activity'
            },
            hover: {
                mode: 'nearest',
                intersect: true
            },
            scales: {
                xAxes: [{
                    stacked: true
                }]
            }
        }
    });
    console.log(barChart.config);
})()
