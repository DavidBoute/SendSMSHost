var apiURL = '/api/';

// modal-template-select
Vue.component('modal-select', {
    props: ['show'],
    template: `
        <transition name="modal">
            <div class ="modal-mask" v-show="show">
                <div class ="modal-wrapper">
                    <div class ="modal-container modal-container-fixed-500">
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
                    <div class ="modal-container modal-container-fixed-500">
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
                    <div class ="modal-container modal-container-fixed-500">
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

// modal-template-import
Vue.component('modal-import', {
    props: ['show'],
    data: function () {
        return {
            worksheetData: {
                json: null,
                columnNames: null,
                fileName: ''
            },
            selectedFields: {
                Message: '',
                ContactNumber: ''
            }
        };
    },
    methods: {
        worksheetDataChanged: function (data) {
            this.worksheetData = data;
        },
        selectedFieldsChanged: function (data) {
            this.selectedFields = data;
        },
        importeerSms: function () {
            var vm = this;
            for (key in vm.selectedFields) {
                if (vm.selectedFields[key] == "") {
                    alert('Maak een selectie: ' + key);
                    return;
                }
            }

            var smsImportData = [];
            vm.worksheetData.json.forEach(element => {
                var sms = {};
                for (key in vm.selectedFields) {
                    var field = vm.selectedFields[key];
                    sms[key] = element[field];
                }
                smsImportData.push(sms);
            });

            app.requestCreateSmsBulk(smsImportData);

            this.$emit('close');
        }
    },
    template: `
        <transition name="modal">
            <div class ="modal-mask" v-show="show">
                <div class ="modal-wrapper">
                    <div class ="modal-container">
                        <div class ="modal-header h3">
                            <slot name="header">
                                Importeer een spreadsheet:
                            </slot>
                        </div>

                        <div class ="modal-body">
                            <slot name="body">
                                <div>
                                    <file-upload v-on:worksheetData-change="worksheetDataChanged" 
                                                :filename="worksheetData.fileName"></file-upload>
                                </div>
                                <div>
                                    <preview-data :columns="worksheetData.columnNames" 
                                                    :data="worksheetData.json" 
                                                    :noRowsPreview="5" 
                                                    :tableCaption="'Preview data'" ></preview-data>
                                </div>
                                <div>
                                    <select-fields v-on:selectedFields-change="selectedFieldsChanged" 
                                                    :columns="worksheetData.columnNames" 
                                                    :importfields="selectedFields" 
                                                    :caption="'Koppel de velden om te importeren'"></select-fields>
                                </div>
                            </slot>
                        </div>

                        <div class ="modal-footer">
                            <slot name="footer">
                                <button class ="btn btn-primary" v-on:click="importeerSms">Import</button>
                                <button class ="btn btn-primary" v-on:click="$emit('close','cancel')">Cancel</button>
                            </slot>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
  `
});

// modal-template-compose
Vue.component('modal-compose', {
    props: ['show'],
    data: function () {
        return {
            worksheetData: {
                json: null,
                columnNames: null,
                fileName: ''
            }
        };
    },
    methods: {
        worksheetDataChanged: function (data) {
            this.worksheetData = data;
        },
        importeerSms: function () {
            var vm = this;

            this.$emit('close');
        }
    },
    template: `
        <transition name="modal">
            <div class ="modal-mask" v-show="show">
                <div class ="modal-wrapper">
                    <div class ="modal-container">
                        <div class ="modal-header h3">
                            <slot name="header">
                                Importeer een spreadsheet:
                            </slot>
                        </div>

                        <div class ="modal-body">
                            <slot name="body">
                                <div>
                                    <file-upload v-on:worksheetData-change="worksheetDataChanged" 
                                                :filename="worksheetData.fileName"></file-upload>
                                </div>
                                <div>
                                    <preview-data :columns="worksheetData.columnNames" 
                                                    :data="worksheetData.json" 
                                                    :noRowsPreview="5" 
                                                    :tableCaption="'Preview data'" ></preview-data>
                                </div>
                                <div>
                                    <textarea></textarea>
                                </div>
                            </slot>
                        </div>

                        <div class ="modal-footer">
                            <slot name="footer">
                                <button class ="btn btn-primary" v-on:click="importeerSms">Import</button>
                                <button class ="btn btn-primary" v-on:click="$emit('close','cancel')">Cancel</button>
                            </slot>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
  `
});

// file-upload
Vue.component('file-upload', {
    inheritAttrs: false,
    props: ['filename'],
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
                        if (event.target.files.length !== 0) {
                            vm.convertFile(event.target.files[0]);
                        }
                           
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

            var worksheetData = {
                json: null,
                columnNames: null,
                fileName: ''
            };

            worksheetData.fileName = file.name;

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

                /* load data in object */
                worksheetData.json = XLSX.utils.sheet_to_json(ws);
                worksheetData.columnNames = Object.keys(worksheetData.json[0]);

                /* emit event to notify parent*/
                vm.$emit('worksheetData-change', worksheetData);
            };

            reader.readAsArrayBuffer(file);
        }
    },
    template: `
        <div class="container">
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
                    <input class ="form-control" readonly type="text" :value="filename">
                </div>
            </div>
        </div>
  `
});

// preview-data
Vue.component('preview-data', {
    props: {
        columns: Array,
        data: Array,
        noRowsPreview: Number,
        tableCaption: String
    },
    data: function () {
        return {
            smsImportData: Array,
            displayGrid: false
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
        },
        caption: function () {
            if (this.displayGrid) {
                return this.tableCaption + ' (top ' + this.noRowsPreview + ' rows of ' + this.data.length + ')';
            }
            else {
                return this.tableCaption;
            }

        }
    },
    methods: {
        toggleDisplayGrid: function (value) {
            this.displayGrid = !value;
        }
    },
    template: `
        <div class ="table-responsive  list-scrollable" v-if="data">         
            <div class ="alert alert-light">
                <table class ="table table-striped table-bordered table-hover table-sm">
                    <caption class="h4" style="margin: 0px; padding-top: 0px">
                        {{caption}}
                        <button class="btn btn-primary" style="float: right;" v-on:click="toggleDisplayGrid(displayGrid)">{{displayGrid ? 'Hide' : 'Show'}}</button>
                    </caption>
                    <thead>
                      <tr>
                        <th v-for="key in columns" scope="col" class ="table-header-truncate">{{key}}</th>
                      </tr>
                    </thead>
                    <tbody v-if="displayGrid">
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

// select-fields
Vue.component('select-fields', {
    props: {
        columns: Array,
        importfields: null,
        caption:''
    },
    methods: {
        selectedFieldsChanged: function () {
            var vm = this;

            /* emit event to notify parent*/
            vm.$emit('selectedFields-change', vm.importfields);
        }
    },
    template: `
        <div v-if="columns">
            <div class ="alert alert-warning" style="margin-top: 20px">
                <div class="h4" style="margin-top: 0px">{{caption}}</div>
                    <div v-for="field in Object.keys(importfields)" class ="form-group form-col" style="margin-right: 20px">
                        <label>{{field}}</label>
                        <select class ="form-control" v-model="importfields[field]" v-on:change="selectedFieldsChanged">
                            <option disabled value="">Kies een kolom</option>
                            <option v-for="key in columns" :value="key">{{key}}</option>
                        </select>
                    </div>
                </div>
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
        showModal: {
            showNewSmsSelectModal: false,
            showNewSmsContactModal: false,
            showNewSmsNumberModal: false,
            showImportModal: false,
            showComposeModal: true
        },        
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
            this.showModal.showNewSmsSelectModal = false;
            switch (selectedModal) {
                case 'showNewSmsContactModal':
                    this.showModal.showNewSmsContactModal = true;
                    break;
                case 'showNewSmsNumberModal':
                    this.showModal.showNewSmsNumberModal = true;
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