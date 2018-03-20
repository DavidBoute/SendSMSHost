var apiURL = '/api/'

// line-chart
Vue.component('bar-chart', {
    extends: VueChartJs.Bar,
    mixins: [VueChartJs.mixins.reactiveProp],
    props: ['options'],
    mounted() {
        this.renderChart(this.chartData, this.options)
    },
})


var app = new Vue({
    el: '#app',
    data: {
        message: 'Loading...',
        summaryList: null,
        weekGraph_data: null,
        weekGraph_options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true,
                        suggestedMax: 10,
                    },
                    gridLines: {
                        display: true
                    },
                    stacked: true
                }],
                xAxes: [{
                    gridLines: {
                        display: true
                    },
                    stacked: true
                }]
            },
            legend: {
                display: true
            },
            responsive: true,
            maintainAspectRatio: false
        }
    },
    created: function () {
        var self = this;
        self.loadData();
    },
    methods: {
        loadData: function () {
            var self = this;
            self.fetchWeekChartData();
            self.message = 'Dashboard';
        },
        fetchChartData: function () {
            self = this;
            self.fetchWeekChartData()

        },
        fetchWeekChartData: function () {
            self = this;
            fetch(`${apiURL}Chartdata/week`)
                .then(res => res.json())
                .then(function (data) {
                    self.weekGraph_data = data;
                })
                .catch(err => console.error('Fout: ' + err));
        },
    }
});