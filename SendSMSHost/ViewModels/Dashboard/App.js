var apiURL = '/api/';

// line-chart
Vue.component('bar-chart', {
    extends: VueChartJs.Bar,
    mixins: [VueChartJs.mixins.reactiveProp],
    props: ['options'],
    mounted() {
        this.renderChart(this.chartData, this.options);
    }
});

// pie-chart
Vue.component('pie-chart', {
    extends: VueChartJs.Pie,
    mixins: [VueChartJs.mixins.reactiveProp],
    props: ['options'],
    mounted() {
        this.renderChart(this.chartData, this.options);
    }
});


var app = new Vue({
    el: '#app',
    mixins: [connectionMethods],
    data: {
        message: 'Loading...',
        includeDeleted: false,
        foreverGraph_data: null,
        weekGraph_data: null,
        dayGraph_data: null,
        hourGraph_data: null,
        barGraph_options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true,
                        suggestedMax: 10
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
        this.startConnection();
    },
    methods: {
        loadData: function () {
            this.getChartData(this.includeDeleted);
            this.message = 'Dashboard';
        },
        getChartData: function (includeDeleted) {
            this.requestForeverChartData(includeDeleted);
            this.requestWeekChartData(includeDeleted);
            this.requestDayChartData(includeDeleted);
            this.requestHourChartData(includeDeleted);
        },
        clearLogs: async function () {
            this.requestClearLogs();
        }
    },
    watch: {
        includeDeleted: function (newVal, oldVal) {
            this.getChartData(newVal);
        }
    }
});