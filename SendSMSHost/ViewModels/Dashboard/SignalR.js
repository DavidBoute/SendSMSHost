var SSEHub;

// Connectie methode specifieke methodes
var connectionMethods = {
    methods: {
        // Start SignalR verbinding
        startConnection: function () {
            var self = this;

            SSEHub = $.connection.serverSentEventsHub;

            // Events gestuurd door de server

            // Bij ontvangen data ForeverChart
            SSEHub.client.notifyChangeForeverChart = function (chartData) {
                app.foreverGraph_data = chartData;
            };

            // Bij ontvangen data WeekChart
            SSEHub.client.notifyChangeWeekChart = function (chartData) {
                app.weekGraph_data = chartData;
            };

            // Bij ontvangen data DayChart
            SSEHub.client.notifyChangeDayChart = function (chartData) {
                app.dayGraph_data = chartData;
            };

            // Bij ontvangen data HourChart
            SSEHub.client.notifyChangeHourChart = function (chartData) {
                app.hourGraph_data = chartData;
            };

            // Bij ontvangen melding aanpassen charts
            SSEHub.client.notifyChangeToCharts = function () {
                app.getChartData();
            };


            $.connection.hub.logging = true;
            $.connection.hub.start().done(function () { self.loadData(); });
        },

        // Vragen gegevens in db op
        requestForeverChartData: function () {
            var self = this;
            SSEHub.server.requestForeverChart(self.includeDeleted);
        },

        requestWeekChartData: function () {
            var self = this;
            SSEHub.server.requestWeekChart(self.includeDeleted);
        },

        requestDayChartData: function () {
            var self = this;
            SSEHub.server.requestDayChart(self.includeDeleted);
        },

        requestHourChartData: function () {
            var self = this;
            SSEHub.server.requestHourChart(self.includeDeleted);
        },

        // Vragen aanpassing database
        requestClearLogs: function () {
            SSEHub.server.requestClearLogs();
        }
    }
};



