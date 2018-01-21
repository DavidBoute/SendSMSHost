var SSEHub;
$(function ()
{
    // Reference the auto-generated proxy for the hub.  
    SSEHub = $.connection.serverSentEventsHub;


    // Create a function that the hub can call back to display messages.
    SSEHub.client.displayMessage = function (message)
    {
        alert(htmlEncode(message));
    };

    // Create a function that the hub can call back to notify changes.
    SSEHub.client.notifyChangeToPage = function (smsDTOWithClient)
    {
        // Even een versimpelde versie -> event = refresh all
        app.fetchSmsList();
    };

    // Start the connection.
    $.connection.hub.start();
});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value)
{
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}
