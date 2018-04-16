// Connectie methode specifieke methodes
var connectionMethods = {
    methods: {
        // Start verbinding - niet nodig voor Ajax
        startConnection: function () { app.loadData() },

        // Vragen gegevens in db op
        requestSmsList: function () {
            fetch(`${apiURL}Sms`)
                .then(res => res.json())
                .then(function (smsList) {
                    if (app.smsList != null) {
                        // Is er een actieve sms? Instellen in nieuwe lijst
                        if (app.currentSms != null) {
                            smsList.filter(x => x.Id === app.currentSms.Id)[0].isActive = true;
                        }
                    }

                    app.smsList = smsList;
                    app.showHeader();
                })
                .catch(err => console.error('Fout: ' + err));
        },
        requestContacts: function () {
            fetch(`${apiURL}Contacts`)
                .then(res => res.json())
                .then(function (res) {
                    app.contacts = res;
                })
                .catch(err => console.error('Fout: ' + err));
        },
        requestStatusList: function () {
            fetch(`${apiURL}Status`)
                .then(res => res.json())
                .then(function (res) {
                    app.statusList = res;
                })
                .catch(err => console.error('Fout: ' + err));
        },

        // Vragen aanpassingen in db aan
        requestCreateSms: function (smsDTO) {
            var ajaxHeaders = new Headers();
            ajaxHeaders.append("Content-Type", "application/json");
            var ajaxConfig = {
                body: JSON.stringify(self.newSms),
                headers: ajaxHeaders
            }

            ajaxConfig.method = 'POST';
            var myRequest = new Request(app.apiUrl + 'Sms/', ajaxConfig)


            // TODO: antwoord server opvangen
            fetch(myRequest)
                .then(function () {
                    // newSms resetten
                    self.newSms = new Object();
                    self.newSms.Id = null;
                    self.newSms.ContactId = null;
                    self.newSms.ContactFirstName = null;
                    self.newSms.ContactLastName = null;
                    self.newSms.ContactNumber = null;
                    self.newSms.Message = null;
                    self.newSms.TimeStamp = null;
                })
                .catch(err => console.error('Fout: ' + err));;
        },
        requestEditSms: function (smsDTO) {
            var ajaxHeaders = new Headers();
            ajaxHeaders.append("Content-Type", "application/json");
            var ajaxConfig = {
                body: JSON.stringify(app.currentSms),
                headers: ajaxHeaders
            }
            ajaxConfig.method = 'PUT';
            var myRequest = new Request(`${apiURL}Sms/${app.currentSms.Id}`, ajaxConfig)


            // TODO: antwoord server opvangen
            fetch(myRequest)
                .catch(err => console.error('Fout: ' + err));

            app.changeSms(smsDTO);
        },
        requestDeleteSms: function (smsDTO) {
            var ajaxHeaders = new Headers();
            ajaxHeaders.append("Content-Type", "application/json");
            var ajaxConfig = {
                headers: ajaxHeaders
            }
            ajaxConfig.method = 'DELETE';
            var myRequest = new Request(`${apiURL}Sms/${app.currentSms.Id}`, ajaxConfig)


            // TODO: antwoord server opvangen
            fetch(myRequest)
                .catch(err => console.error('Fout: ' + err));

            app.removeSms(smsDTO);
        },

        // Vragen versturen van berichten aan
        requestSendSelected: function (smsId) {
            
        },
        requestToggleSendPending: function () {
            
        },
    }
};