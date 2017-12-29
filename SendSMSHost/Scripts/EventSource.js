function startEventSource()
{
    if (window.EventSource == undefined)
    {
        // If not supported
        document.getElementById('targetDiv').innerHTML = "Your browser doesn't support Server Sent Events.";
        return;
    } else
    {
        var source = new EventSource('../EventSource/Message');
        var isOpen = false;

        source.onopen = function (event)
        {
            document.getElementById('targetDiv').innerHTML += 'Connection Opened.<br>';
            isOpen = true;
        };

        source.onerror = function (event)
        {
            if (event.eventPhase == EventSource.CLOSED)
            {
                if (isOpen)
                {
                    document.getElementById('targetDiv').innerHTML += 'Connection Closed.<br>';
                    isOpen = false;
                }
            }
        };

        source.onmessage = function (event)
        {
            if (event.data !== "")
            {
                document.getElementById('targetDiv').innerHTML += event.data + '<br>';
            }
            
        };
    }
}