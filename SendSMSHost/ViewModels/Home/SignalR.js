$(function ()
{
    // Create a function that the hub can call back to notify changes.
    SSEHub.client.notifyChangeToPage = function (smsDTOWithClient)
    {
        // Even een versimpelde versie -> event = refresh all
        app.fetchSmsList();
    };
});

