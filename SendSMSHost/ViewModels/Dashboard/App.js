var apiURL = '/api/'

// line-chart
Vue.component('line-chart', {
    extends: VueChartJs.Line,
    mounted() {
        this.renderChart({
            labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
            datasets: [
                {
                    label: 'Data One',
                    backgroundColor: '#f87979',
                    data: [40, 39, 10, 40, 39, 80, 40]
                }
            ]
        }, { responsive: true, maintainAspectRatio: false  })
    }
})

Vue.component('line-chart-2', {
    extends: VueChartJs.Line,
    mounted() {
        this.renderChart({
            labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
            datasets: [
                {
                    label: 'GitHub Commits',
                    backgroundColor: '#087979',
                    data: [40, 20, 12, 39, 10, 40, 39, 80, 40, 20, 12, 11]
                }
            ]
        }, { responsive: true, maintainAspectRatio: false })
    }
})


var app = new Vue({
    el: '#app',
    data: {
        message: 'Loading...',
        summaryList: null,

    },
    created: function () {
        var self = this;
        self.loadData();
    },
    methods: {
        loadData: function () {
            this.fetchSummaryList();
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