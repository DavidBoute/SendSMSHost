var apiURL = '/api/'

// line-chart
Vue.component('bar-chart', {
    extends: VueChartJs.Bar,
    mixins: [VueChartJs.mixins.reactiveProp],
    props: ['chart-data', 'options'],
    mounted() {
        this.renderChart(this.chartData, this.options)
    }
})


var app = new Vue({
    el: '#app',
    data: {
        message: 'Loading...',
        summaryList: null,
        graph1_data: null,
        graph1_options: { 
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
            self.fetchChartData();
        },
        fetchChartData: function () {
            self = this;
            fetch(`${apiURL}Chartdata/week`)
                .then(res => res.json())
                .then(function (data) {
                    self.graph1_data = data;
                    self.message = 'Dashboard';
                })
                .catch(err => console.error('Fout: ' + err));
        },
    }
});