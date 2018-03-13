var apiURL = '/api/'

// line-chart
Vue.component('line-chart', {
    extends: VueChartJs.Line,
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
        graph1_data: {
            labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
            datasets: [
                {
                    label: 'Messages Sent',
                    backgroundColor: '#087979',
                    data: [40, 20, 12, 39, 10, 40, 39, 80, 40, 20, 12, 11]
                }
            ]
        },
        graph1_options: { 
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    },
                    gridLines: {
                        display: true
                    }
                }],
                xAxes: [{
                    gridLines: {
                        display: true
                    }
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
            //this.fetchSummaryList();
            setTimeout(function () { self.changeGraph_data(self, 0) }, 1000);
        },
        changeGraph_data: function (self, dec) {
            this.graph1_data = {
                labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
                datasets: [
                    {
                        label: 'Messages Sent',
                        backgroundColor: '#F87979',
                        data: [40, 20, 12, 39, 10, 40, 39, 80, 40, 20, dec, (dec * 5)]
                    }
                ]
            };
            dec++;
            if (dec <= 20) {
                setTimeout(function () { self.changeGraph_data(self, dec) }, 100);
            };
        },
        fetchSummaryList: function () {
            self = this;
            fetch(`${apiURL}Summary`)
                .then(res => res.json())
                .then(function (summaryList) {


                    self.summaryList = summaryList;
                    self.message = 'Dashboard';
                })
                .catch(err => console.error('Fout: ' + err));
        },
    }
});