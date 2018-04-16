var apiURL = '/api/'

// modal-template-select
Vue.component('modal-select', {
    template: '#modal-template-select',
    props: ['show'],
})

// modal-template-new-sms-contact
Vue.component('modal-new-sms-contact', {
    template: '#modal-template-new-sms-contact',
    props: ['show', 'contactList'],
    data: function () {
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
        save: function () {
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
                .catch(err => console.error('Fout: ' + err));

            this.$emit('close');
        }
    }
})

// modal-template-new-sms-number
Vue.component('modal-new-sms-number', {
    template: '#modal-template-new-sms-number',
    props: ['show'],
    data: function () {
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
        save: function () {
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

            this.$emit('close');
        }
    }
})

var app = new Vue({
    el: '#app',
    mixins: [connectionMethods],
    data: {
        message: 'Loading...',
        smsList: null,
        contactList: null,
        statusList: null,
        currentSms: null,
        editMode: false,
        showButtons: false,
        showNewSmsSelectModal: false,
        showNewSmsContactModal: false,
        showNewSmsNumberModal: false,
        newSms: null,
    },
    created: function () {
        this.startConnection();
    },
    methods: {
        // Inladen data
        loadData: function () {
            this.requestContacts();
            this.requestStatusList();
            this.requestSmsList(true); // arg = include Created (bool)
        },
        showHeader: function (){
            this.showButtons = true;
            this.message = 'Berichten';
        },

        // Passen weergave SmsList items aan
        getSmsClass: function (sms) {
            style = 'list-group-item'
            if (sms.isActive) style += ' active';
            switch (sms.StatusName) {
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
        getShortenedMessage: function (text) {
            var desiredLength = 20;
            if (text.length <= desiredLength) {
                return text;
            }
            else {
                return text.substring(0, desiredLength) + '...'
            }
        },
        selectSms: function (sms) {
            this.smsList.forEach(
                function (item) {
                    item.isActive = false;
                })
            sms.isActive = true;
            this.currentSms = Object.assign({}, sms); // shallow copy ipv pointer
            this.currentSms.ContactIsNotAnonymous = (this.currentSms.ContactFirstName != null
                                                    || this.currentSms.ContactLastName != null);
            this.hideEditMode();
        },

        // Edit mode
        showEditMode: function () {
            this.editMode = true;
        },
        hideEditMode: function () {
            this.editMode = false;
        },
        saveEdit: function () {
            this.requestEditSms(this.currentSms);
            this.hideEditMode();
        },
        currentSmsSelectedContactChanged: function (contactId) {
            selectedContact = this.contactList.filter(x => x.Id == contactId)[0];
            this.currentSms.ContactFirstName = selectedContact.FirstName;
            this.currentSms.ContactLastName = selectedContact.LastName;
            this.currentSms.ContactNumber = selectedContact.Number;
            this.currentSms.ContactIsNotAnonymous = true;
        },
        currentSmsSelectedStatusChanged: function (statusId) {
            selectedStatus = this.statusList.filter(x => x.Id == statusId)[0];

            this.currentSms.StatusName = selectedStatus.Name;
        },


        // Modal pages
        closedNewSmsSelectModal: function (selectedModal) {
            this.showNewSmsSelectModal = false;
            switch (selectedModal) {
                case 'showNewSmsContactModal':
                    this.showNewSmsContactModal = true;
                    break;
                case 'showNewSmsNumberModal':
                    this.showNewSmsNumberModal = true;
                    break;
            }
        },


        // Send sms
        sendSelected: function(){
            this.requestSendSelected(this.currentSms.Id);
        },
        toggleSendPending: function () {
            this.requestToggleSendPending();
        },

        // Passen inhoud smsList aan
        addSms: function (smsDTO) {
            this.smsList = this.smsList.push(smsDTO);
        },
        changeSms: function (smsDTO) {
            smsIndex = this.smsList.findIndex(s => s.Id == smsDTO.Id);
            this.smsList[smsIndex] = smsDTO;

            if (this.currentSms.Id == smsDTO.Id) {
                this.currentSms = smsDTO;
                this.smsList.filter(x => x.Id === smsDTO.Id)[0].isActive = true;
            };
        },
        removeSms: function (smsDTO) {
            this.smsList = this.smsList.filter(x => x.Id != smsDTO.Id);
            this.currentSms = null;
        },
    }
});