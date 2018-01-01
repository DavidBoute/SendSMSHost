var apiURL = '/api/'

// modal-template-select
Vue.component('modal-select', {
    template: '#modal-template-select',
    props: ['show'],
})

// modal-template-new-sms-contact
Vue.component('modal-new-sms-contact', {
    template: '#modal-template-new-sms-contact',
    props: ['show', 'sms', 'contacts'],
    data: function ()
    {
        return {
            newSms: {
                Id: null,
                ContactId: null,
                ContactFirstName: null,
                ContactLastName: null,
                ContactNumber: null,
                Message: null,
                TimeStamp: null,
                StatusName: 'Created'
            },
            apiUrl: apiURL
        }
    },
    methods: {
        save: function ()
        {
            var self = this
            // opslaan - ajax configuratie
            var ajaxHeaders = new Headers();
            ajaxHeaders.append("Content-Type", "application/json");
            var ajaxConfig = {
                body: JSON.stringify(self.newSms),
                headers: ajaxHeaders
            }

            ajaxConfig.method = 'POST';
            var myRequest = new Request(self.apiUrl + 'Sms/', ajaxConfig)

            fetch(myRequest)
                .then(res => res.json())
                .then(function (res)
                {
                    console.log(res);

                    self.newSms.Id = res.Id;
                    self.newSms.ContactId = res.ContactId;
                    self.newSms.ContactFirstName = res.ContactFirstName;
                    self.newSms.ContactLastName = res.ContactLastName;
                    self.newSms.ContactNumber = res.ContactNumber;
                    self.newSms.Message = res.Message;
                    self.newSms.StatusName = res.StatusName;
                    self.newSms.TimeStamp = res.TimeStamp
                    self.sms.push(self.newSms);

                    // newSms resetten
                    self.newSms = new Object();
                    self.newSms.Id = null;
                    self.newSms.ContactId = null;
                    self.newSms.ContactFirstName = null;
                    self.newSms.ContactLastName = null;
                    self.newSms.ContactNumber = null;
                    self.newSms.Message = null;
                    self.newSms.TimeStamp = null;

                    // andere clients inlichten
                    SSEHub.server.notifyChange({Client: clientName, Operation: 'POST', SmsDTO: res});
                })
                .catch(err => console.error('Fout: ' + err));

            this.$emit('close');
        }
    }
})

// modal-template-new-sms-number
Vue.component('modal-new-sms-number', {
    template: '#modal-template-new-sms-number',
    props: ['show', 'sms'],
    data: function ()
    {
        return {
            newSms: {
                Id: null,
                ContactId: null,
                ContactFirstName: null,
                ContactLastName: null,
                ContactNumber: null,
                Message: null,
                TimeStamp: null,
                StatusName: 'Created'
            },
            apiUrl: apiURL
        }
    },
    methods: {
        save: function ()
        {
            var self = this

            // opslaan - ajax configuratie
            var ajaxHeaders = new Headers();
            ajaxHeaders.append("Content-Type", "application/json");
            var ajaxConfig = {
                body: JSON.stringify(self.newSms),
                headers: ajaxHeaders
            }

            ajaxConfig.method = 'POST';
            var myRequest = new Request(self.apiUrl + 'Sms/', ajaxConfig)

            fetch(myRequest)
                .then(res => res.json())
                .then(function (res)
                {
                    console.log(res);

                    self.newSms.Id = res.Id;
                    self.newSms.ContactId = res.ContactId;
                    self.newSms.ContactFirstName = res.ContactFirstName;
                    self.newSms.ContactLastName = res.ContactLastName;
                    self.newSms.ContactNumber = res.ContactNumber;
                    self.newSms.Message = res.Message;
                    self.newSms.StatusName = res.StatusName;
                    self.newSms.TimeStamp = res.TimeStamp
                    self.sms.push(self.newSms);

                    // newSms resetten
                    self.newSms = new Object();
                    self.newSms.Id = null;
                    self.newSms.ContactId = null;
                    self.newSms.ContactFirstName = null;
                    self.newSms.ContactLastName = null;
                    self.newSms.ContactNumber = null;
                    self.newSms.Message = null;
                    self.newSms.TimeStamp = null;

                    // andere clients inlichten
                    SSEHub.server.notifyChange({ Client: clientName, Operation: 'POST', SmsDTO: res });
                })
                .catch(err => console.error('Fout: ' + err));;

            this.$emit('close');
        }
    }
})

var app = new Vue({
    el: '#app',
    data: {
        message: 'Loading...',
        sms: null,
        currentSms: null,
        contacts: null,
        editMode: false,
        showButtons: false,
        showNewSmsSelectModal: false,
        showNewSmsContactModal: false,
        showNewSmsNumberModal: false,
        newSms: null
    },
    created: function ()
    {
        var self = this;
        self.loadData();
        //self.interval = setInterval(function ()
        //{
        //    self.loadData();
        //}.bind(self), 5000);
    },
    methods: {
        loadData: function ()
        {
            this.fetchSmsList();
            this.fetchContacts();
        },
        fetchSmsList: function ()
        {
            self = this;
            fetch(`${apiURL}Sms`)
                .then(res => res.json())
                .then(function (smsList)
                {
                    if (self.sms != null)
                    {
                        // Is er een actieve sms? Instellen in nieuwe lijst
                        if (self.currentSms != null)
                        {
                            smsList.filter(x => x.Id === self.currentSms.Id)[0].isActive = true;
                        }
                    }

                    self.sms = smsList;
                    self.message = 'Berichten';
                    self.showButtons = true;
                })
                .catch(err => console.error('Fout: ' + err));
        },
        fetchSmsDetails: function (s)
        {
            self = this;
            fetch(`${apiURL}Sms/${s.Id}`)
                .then(res => res.json())
                .then(function (res)
                {
                    self.currentSms = res;
                    self.sms.forEach(function (s2, i)
                    {
                        s2.isActive = false;
                    })
                    s.isActive = true;
                })
                .catch(err => console.error('Fout: ' + err));
        },
        getSmsClass: function (s)
        {
            style = 'list-group-item'
            if (s.isActive) style += ' active';
            switch (s.StatusName)
            {
                case "Queued":
                    style += ' list-group-item-info';
                    break
                case "Pending":
                    style += ' list-group-item-warning';
                    break
                case "Sent":
                    style += ' list-group-item-success';
                    break
                case "Error":
                    style += ' list-group-item-danger';
                    break
            }
            return style;
        },
        fetchContacts: function ()
        {
            self = this;
            fetch(`${apiURL}Contacts`)
                .then(res => res.json())
                .then(function (res)
                {
                    self.contacts = res;
                })
                .catch(err => console.error('Fout: ' + err));
        },
        toEditMode: function (isEdit)
        {
            self = this;
            this.editMode = true;
        },
        saveEdit: function ()
        {
            var self = this
            // opslaan - ajax configuratie
            var ajaxHeaders = new Headers();
            ajaxHeaders.append("Content-Type", "application/json");
            var ajaxConfig = {
                body: JSON.stringify(self.currentSms),
                headers: ajaxHeaders
            }
            ajaxConfig.method = 'PUT';
            var myRequest = new Request(`${apiURL}Sms/${self.currentSms.Id}`, ajaxConfig)

            fetch(myRequest)
                .then(res => res.json())
                .then(function (res)
                {
                    console.log(res);

                    // Lijst updaten
                    smsInList = self.sms.filter(s => (s.Id === res.Id))[0];
                    smsInList.Id = res.Id;
                    smsInList.ContactId = res.ContactId;
                    smsInList.ContactFirstName = res.ContactFirstName;
                    smsInList.ContactLastName = res.ContactLastName;
                    smsInList.ContactNumber = res.ContactNumber;
                    smsInList.Message = res.Message;
                    smsInList.TimeStamp = res.TimeStamp;
                    smsInList.StatusName = res.StatusName;

                    // Editvenster updaten
                    self.currentSms.Id = res.Id;
                    self.currentSms.ContactId = res.ContactId;
                    self.currentSms.ContactFirstName = res.ContactFirstName;
                    self.currentSms.ContactLastName = res.ContactLastName;
                    self.currentSms.ContactNumber = res.ContactNumber;
                    self.currentSms.Message = res.Message;
                    self.currentSms.TimeStamp = res.TimeStamp;
                    self.currentSms.StatusName = res.StatusName;

                    // andere clients inlichten
                    SSEHub.server.notifyChange({ Client: clientName, Operation: 'PUT', SmsDTO: res });

                    self.editMode = false;
                })
                .catch(err => console.error('Fout: ' + err));;
        },
        deleteSms: function ()
        {
            self = this;
            var ajaxHeaders = new Headers();
            ajaxHeaders.append("Content-Type", "application/json");
            var ajaxConfig = {
                headers: ajaxHeaders
            }
            ajaxConfig.method = 'DELETE';
            var myRequest = new Request(`${apiURL}Sms/${self.currentSms.Id}`, ajaxConfig)

            fetch(myRequest)
                .then(function ()
                {
                    // andere clients inlichten
                    // hier voor de operatie opdat self.currentSms op null gezet wordt
                    SSEHub.server.notifyChange({ Client: clientName, Operation: 'DELETE', SmsDTO: self.currentSms });

                    self.sms.forEach(function (s, i)
                    {
                        if (s.Id == self.currentSms.Id)
                        {
                            self.sms.splice(i, 1);
                            self.currentSms = null;
                            return;
                        }
                    });
                })
                .catch(err => console.error('Fout: ' + err));
        },
        getShortenedMessage: function (text)
        {
            var desiredLength = 20;
            if (text.length <= desiredLength)
            {
                return text;
            }
            else
            {
                return text.substring(0, desiredLength) + '...'
            }

        },
        closedNewSmsSelectModal: function (payload)
        {
            this.showNewSmsSelectModal = false;
            switch (payload)
            {
                case 'showNewSmsContactModal':
                    this.showNewSmsContactModal = true;
                    break;
                case 'showNewSmsNumberModal':
                    this.showNewSmsNumberModal = true;
                    break;
            }
        },
        currentSmsSelectedContactChanged: function ()
        {
            selectedContact = this.contacts.filter(x => x.Id == this.currentSms.ContactId)[0];
            this.currentSms.ContactFirstName = selectedContact.FirstName;
            this.currentSms.ContactLastName = selectedContact.LastName;
            this.currentSms.ContactNumber = selectedContact.Number;
        }
    }
});