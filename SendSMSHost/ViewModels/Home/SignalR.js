var SSEHub;

// Connectie methode specifieke methodes
var connectionMethods = {
    methods: {
        // Start SignalR verbinding
        startConnection: function () {
            var self = this;

            SSEHub = $.connection.serverSentEventsHub;

            // Events gestuurd door de server

            // Bij ontvangen nieuwe SmsList
            SSEHub.client.getSmsList = function (smsList) {
                app.smsList = smsList;
                app.showHeader();
            };

            // Bij ontvangen nieuwe StatusList
            SSEHub.client.getStatusList = function (statusList) {
                app.statusList = statusList;
                app.loadFilterData(statusList);
            };

            // Bij ontvangen nieuwe ContactList
            SSEHub.client.getContactList = function (contactList) {
                app.contactList = contactList;
            };

            // Bij algemene melding wijziging
            SSEHub.client.notifyChangeToSmsList = function () {
                app.requestSmsList(this);
            };

            // Bij ontvangen nieuwe SendStatus
            SSEHub.client.notifySendStatus = function (isSending) {
                app.sendStatus = isSending;
            };

            // Bij maken sms
            SSEHub.client.notifyCreateSms = function (smsDTO) {
                app.addSms(smsDTO);
            };

            // Bij aanpassen sms
            SSEHub.client.notifyEditSms = function (smsDTO) {
                app.changeSms(smsDTO);
            };

            // Bij verwijderen sms
            SSEHub.client.notifyDeleteSms = function (smsDTO) {
                app.removeSms(smsDTO);
            };

            $.connection.hub.logging = true;
            $.connection.hub.start().done(function () { self.loadData(); });
        },

        // Vragen gegevens in db op
        requestSmsList: function (includeCreated) {
            SSEHub.server.requestSmsList(includeCreated);
        },
        requestContacts: function () {
            SSEHub.server.requestContactList();
        },
        requestStatusList: function () {
            SSEHub.server.requestStatusList();
        },

        // Vragen aanpassingen in db aan
        requestCreateSms: function (smsDTO) {
            SSEHub.server.requestCreateSms(smsDTO);
        },
        requestEditSms: function (smsDTO) {
            SSEHub.server.requestEditSms(smsDTO);
        },
        requestDeleteSms: function (smsDTO) {
            SSEHub.server.requestDeleteSms(smsDTO);
        },
        requestCreateSmsBulk: function (array) {
            // 400 is te veel, 100 voor de veiligheid 
            // max size 64k
            // recommended max size 32k

            var smsDTOList = array.map(obj =>
            {
                var smsDTO = {};
                smsDTO.ContactNumber = obj.Nummer;
                smsDTO.ContactFirstName = obj.Voornaam;
                smsDTO.ContactLastName = obj.Naam;
                smsDTO.Message = obj.Bericht;

                return smsDTO;
            })

            var step = 100;
            for (var i = 0; i <= smsDTOList.length; i += step) {
                SSEHub.server.requestCreateSmsBulk(smsDTOList.slice(i, i + step));
            }
             
        },

        // Vragen versturen van berichten aan
        requestSendSelected: function (smsId) {
            SSEHub.server.sendSelectedSms(smsId);
        },
        requestToggleSendPending: function (startSend) {
            SSEHub.server.toggleSendPending(startSend);
        },
        requestSendStatus: function () {
            SSEHub.server.requestSendStatus();
        }
    }
};

