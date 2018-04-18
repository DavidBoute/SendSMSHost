// Connectie methode specifieke methodes
var connectionMethods = {
    methods: {
        // Start verbinding - niet nodig voor Ajax
        startConnection: function () { app.loadData() },

        requestWeekChartData: function (includeDeleted) {
            var self = this;

            var url = new URL(window.location.protocol + window.location.host + apiURL + 'Chartdata/week')
            var params = { includeDeleted: includeDeleted }
            url.search = new URLSearchParams(params)

            //fetch(`${apiURL}Chartdata/week`)
            fetch(url)
                .then(res => res.json())
                .then(function (data) {
                    self.weekGraph_data = data;
                })
                .catch(err => console.error('Fout: ' + err));
        },
    }
}