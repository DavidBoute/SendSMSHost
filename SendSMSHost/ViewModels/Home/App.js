﻿var apiURL = '/api/';

// modal-template-select
Vue.component('modal-select', {
    props: ['show'],
    template: `
        <transition name="modal">
            <div class ="modal-mask" v-show="show">
                <div class ="modal-wrapper">
                    <div class ="modal-container">

                        <div class ="modal-header">
                            <slot name="header">
                                Maak een nieuwe sms op basis van:
                            </slot>
                        </div>

                        <div class ="modal-body">
                            <slot name="body">
                                <div class ="form-group">
                                    <table>
                                        <tr>
                                            <td><button class ="btn btn-primary btn-block" v-on:click="$emit('close','showNewSmsContactModal')">Contact</button></td>
                                        </tr>
                                        <tr>
                                            <td><button class ="btn btn-primary btn-block" v-on:click="$emit('close','showNewSmsNumberModal')">Nummer</button></td>
                                        </tr>
                                    </table>
                                </div>
                            </slot>
                        </div>

                        <div class ="modal-footer">
                            <slot name="footer">
                                <button class ="btn btn-primary" v-on:click="$emit('close','cancel')">Cancel</button>
                            </slot>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
  `
});

// modal-template-new-sms-contact
Vue.component('modal-new-sms-contact', {
    props: ['show', 'contact-list'],
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
        };
    },
    methods: {
        save: function () {
            app.requestCreateSms(this.newSms);

            // newSms resetten
            this.newSms = new Object();
            this.newSms.Id = null;
            this.newSms.ContactId = null;
            this.newSms.ContactFirstName = null;
            this.newSms.ContactLastName = null;
            this.newSms.ContactNumber = null;
            this.newSms.Message = null;
            this.newSms.TimeStamp = null;

            this.$emit('close');
        }
    },
    template: `
        <transition name="modal">
            <div class ="modal-mask" v-show="show">
                <div class ="modal-wrapper">
                    <div class ="modal-container">

                        <div class ="modal-header">
                            <slot name="header">
                                Nieuw bericht
                            </slot>
                        </div>

                        <div class ="modal-body">
                            <slot name="body">
                                <div class ="form-group">
                                    <table>
                                        <tr>
                                            <td>Contact: </td>
                                            <td>
                                                <select v-model="newSms.ContactId" class ="form-control">
                                                    <option v-for="contact in contactList" v-bind:value="contact.Id">{{ contact.FirstName +' '+contact.LastName }}</option>
                                                </select>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Bericht: </td>
                                            <td><textarea v-model="newSms.Message" class ="form-control"></textarea></td>
                                        </tr>
                                    </table>

                                </div>
                            </slot>
                        </div>

                        <div class ="modal-footer">
                            <slot name="footer">
                                <button class ="btn btn-primary" v-on:click="$emit('close')">Cancel</button>
                                <button v-on:click="save" class ="btn btn-primary">Save</button>
                            </slot>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
  `
});

// modal-template-new-sms-number
Vue.component('modal-new-sms-number', {
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
        };
    },
    methods: {
        save: function () {
            app.requestCreateSms(this.newSms);

            // newSms resetten
            this.newSms = new Object();
            this.newSms.Id = null;
            this.newSms.ContactId = null;
            this.newSms.ContactFirstName = null;
            this.newSms.ContactLastName = null;
            this.newSms.ContactNumber = null;
            this.newSms.Message = null;
            this.newSms.TimeStamp = null;

            this.$emit('close');
        }
    },
    template: `
        <transition name="modal">
            <div class ="modal-mask" v-show="show">
                <div class ="modal-wrapper">
                    <div class ="modal-container">

                        <div class ="modal-header">
                            <slot name="header">
                                Nieuw bericht
                            </slot>
                        </div>

                        <div class ="modal-body">
                            <slot name="body">
                                <div class ="form-group">
                                    <table>
                                        <tr>
                                            <td>Nummer: </td>
                                            <td><input v-model="newSms.ContactNumber" class ="form-control"></td>
                                        </tr>
                                        <tr>
                                            <td>Bericht: </td>
                                            <td><textarea v-model="newSms.Message" class ="form-control"></textarea></td>
                                        </tr>
                                    </table>

                                </div>
                            </slot>
                        </div>

                        <div class ="modal-footer">
                            <slot name="footer">
                                <button class ="btn btn-primary" v-on:click="$emit('close')">Cancel</button>
                                <button v-on:click="save" class ="btn btn-primary">Save</button>
                            </slot>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
  `
});

// custom file imput
Vue.component('cust-file-input', {
    inheritAttrs: false,
    data: function () {
        return {
            worksheetData: {
                raw: null,
                json: null,
                isValid: false,
                columnNames: null,
                fileName: '',
                sheetName: ''
            }
        };
    },
    computed: {
        inputListeners: function () {
            var vm = this;
            // `Object.assign` merges objects together to form a new object
            return Object.assign({},
              // We add all the listeners from the parent
              this.$listeners,
              // Then we can add custom listeners or override the
              // behavior of some listeners.
              {
                  // This ensures that the component works with v-model
                  input: function (event) {
                      vm.$emit('input', event.target);
                      if (event.target.files.length !== 0)
                          vm.convertFile(event.target.files[0]);
                  }
              }
            );
        }
    },
    methods: {
        convertFile: function (file) {
            var vm = this;

            // Validatie bestand TODO echt uitwerken
            var isValid = false;
            isValid = file !== null
                     && file.name.trim().length !== 0;

            if (!isValid) return;

            var vmData = vm.worksheetData;
            vmData.fileName = file.name;
            vmData.isValid = isValid;

            var reader = new FileReader();
            reader.onload = function (e) {
                // pre-process data
                var binary = "";
                var bytes = new Uint8Array(e.target.result);
                var length = bytes.byteLength;
                for (var i = 0; i < length; i++) {
                    binary += String.fromCharCode(bytes[i]);
                }

                /* read workbook */
                var wb = XLSX.read(binary, { type: 'binary' });

                /* grab first sheet */
                var wsname = wb.SheetNames[0];
                var ws = wb.Sheets[wsname];

                /* load data in viewmodel */
                vmData.raw = ws;
                vmData.json = XLSX.utils.sheet_to_json(ws);
                vmData.sheetName = wsname;
                vmData.columnNames = Object.keys(vmData.json[0]);
            };

            reader.readAsArrayBuffer(file);
        }
    },
    template: `
        <div class="container" style="margin-top: 20px">
            <div class ="form-inline">
                <div class="input-group">
                    <label class="input-group-btn">
                        <span class="btn btn-primary">
                            Browse...
                            <input style="display: none;"  type="file" accept=".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"
                            v-bind="$attrs"
                            v-on="inputListeners">
                        </span>
                    </label>
                    <input class ="form-control" readonly type="text" :value="worksheetData.fileName">
                </div>
            </div>
            <div>
                <data-grid :columns="worksheetData.columnNames" :data="worksheetData.json" :noRowsPreview="5" :tableCaption="'Preview data'" ></data-grid>
            </div>
        </div>
  `
});

// datagrid
Vue.component('data-grid', {
    props: {
        columns: Array,
        data: Array,
        noRowsPreview: Number,
        tableCaption: String
    },
    data: function () {
        return {
            smsImportFields: ['Message', 'ContactNumber']
        };
    },
    computed: {
        previewData: function () {
            if (this.data !== null) {
                return this.data.slice(0, this.noRowsPreview);
            }
            else {
                return null;
            }
        }
    },
    template: `
        <div class ="table-responsive" v-if="data">
            <div class ="alert alert-warning"" style="margin-top: 20px">
                <div class="h4" style="margin-top: 0px">Koppel de velden om te importeren</div>
                <div class="form-inline">
                    <div v-for="field in smsImportFields" class ="form-group form-col" style="margin-right: 20px">
                        <label>{{field}}</label>
                        <select class="form-control">
                            <option v-for="key in columns" :value="key">{{key}}</option>
                        </select>
                    </div>
                    <button class ="btn btn-primary btn-md">Import</button>
                </div>
            </div>
            <div class ="alert alert-light">
                <table class ="table table-striped table-bordered table-hover table-sm">
                    <caption class="h4" style="margin: 0px; padding-top: 0px">{{tableCaption +' (top ' + noRowsPreview +' rows)' }}</caption>
                    <thead>
                      <tr>
                        <th v-for="key in columns" scope="col" class ="table-header-truncate">{{key}}</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="entry in previewData">
                        <td v-for="key in columns" scope="row">
                            {{entry[key]}}
                        </td>
                      </tr>
                    </tbody>
                </table>
            </div>
        </div>
    `
});

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
        sendStatus: null
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
            this.requestSendStatus();
        },
        showHeader: function () {
            this.showButtons = true;
            this.message = 'Berichten';
        },

        // Passen weergave SmsList items aan
        getSmsClass: function (sms) {
            style = 'list-group-item';
            if (sms.isActive) style += ' active';
            switch (sms.StatusName) {
                case "Queued":
                    style += ' list-group-item-info';
                    break;
                case "Pending":
                    style += ' list-group-item-warning';
                    break;
                case "Sent":
                    style += ' list-group-item-success';
                    break;
                case "Error":
                    style += ' list-group-item-danger';
                    break;
            }
            return style;
        },
        getShortenedMessage: function (text) {
            var desiredLength = 20;
            if (text.length <= desiredLength) {
                return text;
            }
            else {
                return text.substring(0, desiredLength) + '...';
            }
        },
        selectSms: function (sms) {
            this.smsList.forEach(
                function (item) {
                    item.isActive = false;
                });
            sms.isActive = true;
            this.currentSms = Object.assign({}, sms); // shallow copy ipv pointer
            this.currentSms.ContactIsNotAnonymous = this.currentSms.ContactFirstName !== null
                                                    || this.currentSms.ContactLastName !== null;
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
            selectedContact = this.contactList.filter(x => x.Id === contactId)[0];
            this.currentSms.ContactFirstName = selectedContact.FirstName;
            this.currentSms.ContactLastName = selectedContact.LastName;
            this.currentSms.ContactNumber = selectedContact.Number;
            this.currentSms.ContactIsNotAnonymous = !selectedContact.IsAnonymous;
        },
        currentSmsSelectedStatusChanged: function (statusId) {
            selectedStatus = this.statusList.filter(x => x.Id === statusId)[0];

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
        sendSelected: function () {
            this.requestSendSelected(this.currentSms.Id);
        },
        toggleSendPending: function (startSend) {
            this.requestToggleSendPending(startSend);
        },

        // Passen inhoud smsList aan
        addSms: function (smsDTO) {
            this.smsList.push(smsDTO);
        },
        changeSms: function (smsDTO) {
            smsIndex = this.smsList.findIndex(s => s.Id === smsDTO.Id);
            this.smsList[smsIndex] = smsDTO;

            if (this.currentSms !== null
                && this.currentSms.Id === smsDTO.Id) {
                this.currentSms = smsDTO;
                this.smsList.filter(x => x.Id === smsDTO.Id)[0].isActive = true;
            }
        },
        removeSms: function (smsDTO) {
            this.smsList = this.smsList.filter(x => x.Id !== smsDTO.Id);
            this.currentSms = null;
        }    
    }
});