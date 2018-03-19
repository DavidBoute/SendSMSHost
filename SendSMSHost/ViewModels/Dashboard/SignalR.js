$(function ()
{
    // Create a function that the hub can call back to notify changes.
    SSEHub.client.notifyChangeToCharts = function ()
    {
        // Even een versimpelde versie -> event = refresh all
        app.fetchChartData();
    };
});

