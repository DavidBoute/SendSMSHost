var apiURL = '/api/';

// modal-select
Vue.component('modal-select', {
    props: ['show'],
    inject: [
        'showModalWindow'
    ],
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
                                            <td><button class ="btn btn-primary btn-block" v-on:click="showModalWindow('NewSmsContactModal')">Contact</button></td>
                                        </tr>
                                        <tr>
                                            <td><button class ="btn btn-primary btn-block" v-on:click="showModalWindow('NewSmsNumberModal')">Nummer</button></td>
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

// modal-new-sms-contact
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

// modal-new-sms-number
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

// modal-import
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
                ContactNumber: '',
                FirstName: '',
                LastName: ''
            },
            importContacts: true
        };
    },
    computed:{
        passedFields: function (){
            var vm = this;
            var passedFields;

            if (vm.importContacts) {
                passedFields = Object.assign({},vm.selectedFields);
            }
            else {
                // TODO: velden dynamisch maken

                passedFields = {
                    Message: vm.selectedFields.Message,
                    ContactNumber: vm.selectedFields.ContactNumber
                }
            }

            return passedFields;

        }
    },
    inject: [
        'showModalWindow',
        'openValidateModal'
    ],
    methods: {
        worksheetDataChanged: function (data) {
            this.worksheetData = data;
        },
        selectedFieldsChanged: function (data) {
            this.selectedFields = Object.assign(this.selectedFields, data);
        },
        valideerSms: function () {
            var vm = this;
            for (key in vm.passedFields) {
                if (vm.passedFields[key] === "") {
                    alert('Maak een selectie: ' + key);
                    return;
                }
            }

            var smsImportData = [];
            var index = 0;
            vm.worksheetData.json.forEach(element => {
                var sms = {};
                for (key in vm.passedFields) {
                    var field = vm.passedFields[key];
                    sms[key] = element[field];
                }

                sms['Id'] = index;
                index++;

                smsImportData.push(sms);
            });

            var fieldsArray = Object.keys(vm.passedFields);

            vm.openValidateModal(fieldsArray, smsImportData);

            this.$emit('close');
        }
    },
    provide: function () {
        return {
            selectedFieldsChanged_parent: this.selectedFieldsChanged,
            worksheetDataChanged_parent: this.worksheetDataChanged
        };
    },
    template: `
        <transition name="modal">
            <div class ="modal-mask" v-show="show">
                <div class ="modal-wrapper">
                    <div class ="modal-container scrollbar">
                        <div class ="modal-header h3">
                            <slot name="header">
                                Importeer een spreadsheet:
                            </slot>
                        </div>

                        <div class ="modal-body">
                            <slot name="body">
                                <div>
                                    <file-upload :filename="worksheetData.fileName"></file-upload>
                                </div>
                                <div v-if="worksheetData.columnNames">
                                    <div>
                                        <preview-data :columns="worksheetData.columnNames" 
                                                        :data="worksheetData.json" 
                                                        :noRowsPreview="5" 
                                                        :tableCaption="'Preview data'" ></preview-data>
                                    </div>
                                    <div>
                                        <div class="form-check container-fluid">
                                            <input type="checkbox" class="form-check-input" v-model="importContacts"></input>                                        
                                            <label class="form-check-label">Importeer namen</label>    
                                        </div>
                                        <select-fields  :columns="worksheetData.columnNames" 
                                                        :importfields="passedFields" 
                                                        :caption="'Koppel de velden om te importeren'"></select-fields>
                                    </div>
                                </div>
                            </slot>
                        </div>

                        <div class ="modal-footer">
                            <slot name="footer">
                                <button class ="btn btn-primary" v-on:click="valideerSms">Import</button>
                                <button class ="btn btn-primary" v-on:click="$emit('close','cancel')">Cancel</button>
                            </slot>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
  `
});

// modal-compose
Vue.component('modal-compose', {
    props: ['show'],
    data: function () {
        return {
            worksheetData: {
                json: null,
                columnNames: null,
                fileName: ''
            },
            textboxText: '',
            selectedFields: {
                ContactNumber: '',
                FirstName: '',
                LastName: ''
            },
            importContacts: true
        };
    },
    computed: {
        passedFields: function () {
            var vm = this;
            var passedFields;

            if (vm.importContacts) {
                passedFields = Object.assign({}, vm.selectedFields);
            }
            else {
                // TODO: velden dynamisch maken

                passedFields = {
                    Message: vm.selectedFields.Message,
                    ContactNumber: vm.selectedFields.ContactNumber
                }
            }

            return passedFields;

        }
    },
    inject: [
        'showModalWindow',
        'openValidateModal'
    ],
    methods: {
        worksheetDataChanged: function (data) {
            this.worksheetData = data;
        },
        selectedFieldsChanged: function (data) {
            this.selectedFields = Object.assign({}, data);
        },
        getFieldValue: function (fieldName, record, columns) {
            var vm = this;
            var text = "";

            if (columns.includes(fieldName)) {
                text = record[fieldName];
            }
            else {
                text = "Ongeldige verwijzing!";
            }

            return text;
        },
        convertTemplateToHTML: function (template, record, columns) {
            var vm = this;
            var text = '';
            var field = false;

            var textArray = template.split(/([$])/g);
            textArray.forEach(function (part) {
                if (part === '$') {
                    field = !field;
                }
                else {
                    if (field) {
                        text += "<span style='color: blue'>";
                        text += vm.getFieldValue(part, record, columns);
                        text += "</span>";
                    }
                    else {
                        text += "<span>" + part + "</span>";
                    }
                }
            });

            return text;
        },
        getSmsContentFromHtml: function (html) {
            return html.replace(/(<([^>]+)>)/ig, "");
        },

        valideerSms: function () {
            var vm = this;

            var columns = vm.worksheetData.columnNames;
            var template = vm.textboxText;

            for (key in vm.passedFields) {
                if (vm.passedFields[key] === "") {
                    alert('Maak een selectie: ' + key);
                    return;
                }
            }

            if (columns === null) {
                alert('Importeer een spreadsheet! ');
                return;
            }

            if (template === '') {
                alert('Geef een template in! ');
                return;
            }

            var smsImportData = [];
            var index = 0;
            vm.worksheetData.json.forEach(element => {
                var sms = {};
                for (key in vm.passedFields) {
                    var field = vm.passedFields[key];
                    sms[key] = element[field];
                }

                sms['Message'] = vm.getSmsContentFromHtml(vm.convertTemplateToHTML(template, element, columns));

                sms['Id'] = index;
                index++;

                smsImportData.push(sms);
            });


            var fieldsArray = ['Message', ...Object.keys(vm.passedFields)];

            vm.openValidateModal(fieldsArray, smsImportData);

            this.$emit('close');
        }
    },
    provide: function () {
        return {
            getFieldValue: this.getFieldValue,
            convertTemplateToHTML: this.convertTemplateToHTML,
            getSmsContentFromHtml: this.getSmsContentFromHtml,

            selectedFieldsChanged_parent: this.selectedFieldsChanged,
            worksheetDataChanged_parent: this.worksheetDataChanged
        };
    },
    template: `
        <transition name="modal">
            <div class ="modal-mask" v-show="show">
                <div class ="modal-wrapper">
                    <div class ="modal-container scrollbar">
                        <div class ="modal-header h3">
                            <slot name="header">
                                Importeer een spreadsheet:
                            </slot>
                        </div>

                        <div class ="modal-body">
                            <slot name="body">

                                    <file-upload :filename="worksheetData.fileName"></file-upload>


                                    <div v-if="worksheetData.columnNames">

                                        <preview-data :columns="worksheetData.columnNames" 
                                                        :data="worksheetData.json" 
                                                        :noRowsPreview="5" 
                                                        :tableCaption="'Preview data'" ></preview-data>
                                    
                                    
                                        <div class="row container-fluid">
                                            <div class="form-check container-fluid">
                                                <input type="checkbox" class="form-check-input" v-model="importContacts"></input>                                        
                                                <label class="form-check-label">Importeer namen</label>    
                                            </div>
                                            <select-fields :columns="worksheetData.columnNames" 
                                                           :importfields="passedFields" 
                                                           :caption="'Koppel de velden om te importeren'"></select-fields>
                                        </div>
                                        <div class="row container-fluid">
                                            <div class="col-md-3">
                                                <div class="panel panel-default" >
                                                    <div class="panel-heading clearfix">Template</div>
                                                    <textarea v-model="textboxText" v-if="worksheetData.columnNames"
                                                                class="form-control input-md panel-body"
                                                                style="min-height: 100px;"></textarea>
                                                    <div class="panel-footer">
                                                        Gebruik de naam van een kolom tussen $-tekens om data in te voegen.
                                                    </div>
                                                </div>
                                             </div>
                                            <div class="col-md-3">
                                                <linked-textbox :templateText="textboxText"
                                                                :columns="worksheetData.columnNames"
                                                                :data="worksheetData.json"></linked-textbox>
                                            </div>                                      
                                        </div>
                                    </div>
                            </slot>
                        </div>

                        <div class ="modal-footer">
                            <slot name="footer">
                                <button class ="btn btn-primary" v-on:click="valideerSms">Import</button>
                                <button class ="btn btn-primary" v-on:click="$emit('close','cancel')">Cancel</button>
                            </slot>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
  `
});

// modal-validate
Vue.component('modal-validate', {
    props: {
        show: Boolean,
        importdata: null
    },
    data: function () {
        return {
            data: Array,
            columns: Array,
            smsToImport: []
        };
    },
    inject: [
        'showModalWindow'
    ],
    methods: {
        importeerSms: function () {
            var vm = this;

            var smsImportData = vm.smsToImport;

            app.requestCreateSmsBulk(smsImportData);

            this.$emit('close');
        },
        getIsReadyToImport: function (sms) {
            var vm = this;

            return vm.smsToImport.includes(sms);
        },
        setIsReadyToImport: function (sms, isImport) {
            var vm = this;

            if (isImport && !vm.getIsReadyToImport(sms)) {
                vm.smsToImport.push(sms);
            }
            else if (!isImport && vm.getIsReadyToImport(sms)) {
                vm.smsToImport = vm.smsToImport.filter(x => x != sms);
            }
        }
    },
    provide: function () {
        return {
            openModalValidate: this.openModalValidate,
            smsToImport: this.smsToImport,
            getIsReadyToImport: this.getIsReadyToImport,
            setIsReadyToImport: this.setIsReadyToImport
        };
    },
    template: `
        <transition name="modal">
            <div class ="modal-mask" v-show="show">
                <div class ="modal-wrapper">
                    <div class ="modal-container scrollbar">
                        <div class ="modal-header h3">
                            <slot name="header">
                                Controleer de berichten:
                            </slot>
                        </div>

                        <div class ="modal-body">
                            <slot name="body">
                                <edit-bulk-sms  :columns="importdata.columns" 
                                                :data="importdata.data" 
                                                :tableCaption="'Controleer de berichten'"></edit-bulk-sms> 
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
    inject: [
        'worksheetDataChanged_parent'
    ],
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

                vm.worksheetDataChanged_parent(worksheetData);
            };

            reader.readAsArrayBuffer(file);
        }
    },
    template: `
        <div class="container-fluid" style="margin-bottom: 10px">
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
        <div class ="table-responsive list-scrollable scrollbar container-fluid" v-if="data">         
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
                    <transition>
                        <tbody v-if="displayGrid">
                            <tr v-for="entry in previewData">
                            <td v-for="key in columns" scope="row">
                                {{entry[key]}}
                            </td>
                            </tr>
                        </tbody>
                    </transition>
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
        caption: ''
    },
    inject: [
        'selectedFieldsChanged_parent'
    ],
    methods: {
        selectedFieldsChanged: function () {
            var vm = this;

            vm.selectedFieldsChanged_parent(vm.importfields);
        }
    },
    template: `
        <div v-if="columns" class="container-fluid">
            <div class ="alert alert-warning" style="margin-top: 20px">
                <div class="h4" style="margin-top: 0px">{{caption}}</div>
                    <div v-for="field in Object.keys(importfields)" class ="form-group form-col" style="margin-right: 20px">
                        <label>{{field}}</label>
                        <select class ="form-control fixed-width" v-model="importfields[field]" v-on:change="selectedFieldsChanged">
                            <option disabled value="">Kies een kolom</option>
                            <option v-for="key in columns" :value="key">{{key}}</option>
                        </select>                      
                    </div>
                </div>
            </div>
        </div>
    `
});

// linked-textbox
Vue.component('linked-textbox', {
    props: {
        columns: Array,
        data: null,
        templateText: ''
    },
    inject: [
        'getFieldValue',
        'convertTemplateToHTML',
        'getSmsContentFromHtml'
    ],
    data: function () {
        return {
            rowIndex: 0
        };
    },
    computed: {
        textHTML: function () {
            var vm = this;
            return vm.convertTemplateToHTML(vm.templateText, vm.data[vm.rowIndex], vm.columns);
        },
        characterCountWithoutHTML: function () {
            var vm = this;
            return vm.getSmsContentFromHtml(vm.textHTML).length;
        },
        characterCountWithStyle: function () {
            var vm = this;
            var text = "";

            if (vm.characterCountWithoutHTML <= 160) {
                text = "<span>";
            }
            else {
                text = "<span style='color: red'>";
            }
            text += " (" + vm.characterCountWithoutHTML + "/160)";
            text += "</span>";

            return text;
        }
    },
    methods: {
        setCurrentIndex: function (newIndex) {
            var vm = this;

            if (newIndex >= 0 && newIndex < vm.data.length) {
                vm.rowIndex = newIndex;
            }
        }
    },
    template: `
            <div class="panel panel-default">
                <div class="panel-heading text-center">
                    <button class ="btn btn-primary" @click="setCurrentIndex(0)"> << </button>
                    <button class ="btn btn-primary" @click="setCurrentIndex(rowIndex-1)"> < </button> 
                    <span>{{(rowIndex + 1) + "/" + data.length}}</span>
                    <button class ="btn btn-primary" @click="setCurrentIndex(rowIndex+1)"> > </button>
                    <button class ="btn btn-primary" @click="setCurrentIndex(data.length-1)"> >> </button>
                </div>                
                <div class="panel-body" style="padding-top: 0px; padding-bottom: 0px;">
                    <div v-html="textHTML"  style="word-wrap: break-word; min-height: 100px; margin: 0px"></div>
                </div>
                <div class="panel-footer text-right">
                    <div v-html="characterCountWithStyle"></div>
                </div>
            </div>
        `
});

// edit-bulk-sms
Vue.component('edit-bulk-sms', {
    props: {
        columns: Array,
        data: Array,
        tableCaption: String
    },
    data: function () {
        return {
            activeRow: -1,
        };
    },
    computed: {
        caption: function () {
            return this.tableCaption + ' (' + this.data.length + ' sms)';
        },
        columnsWithValidation: function () {
            var vm = this;

            return [...vm.columns, 'Count', 'IsValid', 'IsImport'];
        },
    },
    methods: {
        setActiveSms: function (rowIndex) {
            var vm = this;

            vm.activeRow = rowIndex;
        },
        updateSms: function (oldSms, updatedSms) {
            var vm = this;

            var changedSms = vm.data[oldSms.Id];
            for (var k in changedSms) changedSms[k] = updatedSms[k];

            Vue.set(vm.data, updatedSms.Id, changedSms);
        },
    },
    provide: function () {
        return {
            setActiveSms: this.setActiveSms,
            updateSms: this.updateSms
        };
    },
    template: `
        <div class ="table-responsive list-scrollable scrollbar container-fluid" v-if="data">         
            <div class ="alert alert-light">
                <table class ="table table-striped table-hover table-sm">
                    <caption class="h4" style="margin: 0px; padding-top: 0px">
                        {{caption}}
                    </caption>
                    <thead>
                        <tr>
                        <th class="table-row-header-fixed-width"></th>
                        <th v-for="key in columnsWithValidation" scope="col" class ="table-header-truncate">{{key}}</th>
                        <th></th>
                        </tr>
                    </thead>
                    <transition>
                        <tbody>
                            <tr v-for="entry in data" is="sms-validation-wrapper" 
                                                        :columns="columnsWithValidation"
                                                        :sms="entry"
                                                        :active-row="activeRow">
                            </tr>
                        </tbody>
                    </transition>
                </table>
            </div>
        </div>
    `
});

// sms-validation-wrapper
Vue.component('sms-validation-wrapper', {
    props: {
        sms: Object,
        activeRow: Number,
        columns: Array,
    },
    data: function () {
        return {
            isEdit: false,
            editSms: null
        };
    },
    computed: {
        Message: function () {
            var vm = this;

            return vm.sms.Message;
        },
        ContactNumber: function () {
            var vm = this;

            return vm.sms.ContactNumber;
        },
        FirstName: function () {
            var vm = this;

            return vm.sms.FirstName;
        },
        LastName: function () {
            var vm = this;

            return vm.sms.LastName;
        },
        Id: function () {
            var vm = this;

            return vm.sms.Id;
        },
        Count: function () {
            var sms = this;

            return sms.Message.length;
        },
        IsValid: function () {
            var sms = this;

            if (sms.Count > 150) { return false; };
            if (sms.ContactNumber.length < 10) { return false; };

            return true;
        },
        IsActive: function () {
            var sms = this;

            return sms.Id == sms.activeRow;
        },
        smsClass: function () {
            var sms = this;

            style = 'list-group-item';

            if (sms.IsValid) {
                style += ' list-group-item-success';
            }
            else {
                style += ' list-group-item-danger';
            }

            return style;
        },
        IsImport: function () {
            var vm = this;

            return vm.getIsReadyToImport(vm.sms);
        }
    },
    methods: {
        selectSms: function () {
            var vm = this;

            vm.setActiveSms(vm.sms.Id);
        },
        getValue: function (key) {
            var sms = this;

            return sms[key];
        },
        setEdit: function (value) {
            var vm = this;

            vm.isEdit = value;

            if (value) {
                vm.editSms = Object.assign({}, vm.sms, { IsImport: vm.IsImport });
            }
            else {
                vm.editSms = null;
            }
        },
        updateData: function () {
            var vm = this;

            vm.updateSms(vm.sms, vm.editSms);
            vm.setIsReadyToImport(vm.sms, vm.editSms.IsImport);
            vm.setEdit(false);
        },
        isEditColumn: function (key) {
            var vm = this;

            return Object.keys(vm.sms).includes(key);
        },
        isCheckboxColumn: function (key) {
            var vm = this;

            return key == "IsImport";
        }
    },
    inject: ['setActiveSms', 'updateSms', 'setIsReadyToImport', 'getIsReadyToImport'],
    template: `
                <tr>                    
                    <td style="vertical-align: middle; padding: 8px 0px;" scope="row">
                        <div style="padding: 20px 0px; margin-left: 5px;" v-if="IsActive"  :class="[smsClass, 'clearfix']"></div>
                    </td>
                    <td v-for="key in columns" class="align-middle">
                        <div v-if="!isEdit" :class="smsClass" v-on:click="selectSms()">{{getValue(key)}}</div>
                        <div v-if="isEdit">
                            <textarea v-if="isEditColumn(key)" v-model="editSms[key]" class="align-bottom max-width"></textarea>
                            <label class="checkboxContainer align-middle" v-if="isCheckboxColumn(key)"><input type="checkbox" v-model="editSms.IsImport"><span class="checkmark"></span></label>
                        </div>

                    </td>
                    <td style="vertical-align: middle;">
                        <button v-if="IsActive && !isEdit" v-on:click="setEdit(true)">+</button>
                        <button v-if="isEdit" v-on:click="setEdit(false)">x</button>
                        <button v-if="isEdit" v-on:click="updateData()">v</button>
                    </td>
                </tr>
    `
});

// edit-sms-form
Vue.component('edit-sms-form', {
    props: {
        selectedSms: Object,
        statusList: Array,
        contactList: Array
    },
    data: function () {
        return {
            editMode: false
        }
    },
    computed: {
        editableSms: function () {
            var vm = this;

            return Object.assign({}, vm.selectedSms);
        },
        showContactField: function () {
            var vm = this;

            return vm.editMode || vm.editableSms.ContactIsNotAnonymous;
        }
    },
    methods: {
        showEditMode: function () {
            var vm = this;

            vm.editMode = true;
        },
        hideEditMode: function () {
            var vm = this;

            vm.editMode = false;
        },
        saveEdit: function () {
            var vm = this;

            app.requestEditSms(vm.editableSms);
            vm.hideEditMode();
        },
        deleteSms: function () {
            var vm = this;

            app.requestDeleteSms(vm.editableSms);
            vm.hideEditMode();
        },
        editableSmsSelectedContactChanged: function (contactId) {
            var vm = this;

            var selectedContact = vm.contactList.filter(x => x.Id === contactId)[0];
            vm.editableSms.ContactFirstName = selectedContact.FirstName;
            vm.editableSms.ContactLastName = selectedContact.LastName;
            vm.editableSms.ContactNumber = selectedContact.Number;
            vm.editableSms.ContactIsNotAnonymous = !selectedContact.IsAnonymous;
        },
        editableSmsSelectedStatusChanged: function (statusId) {
            var vm = this;

            var selectedStatus = vm.statusList.filter(x => x.Id === statusId)[0];

            vm.editableSms.StatusName = selectedStatus.Name;
        }
    },
    watch: {
        'editableSms.StatusId': function (newVal, oldVal) {
            var vm = this;

            vm.editableSmsSelectedStatusChanged(newVal);
        }
    },
    template: `
        <div class="container">
            <div class="panel panel-primary">
                <div class="panel-heading"><strong>Details</strong></div>
                <div class="panel-body" >
                    <div class="form-group" >
                        <label v-if="showContactField">Contact:</label>
                        <select v-model="editableSms.ContactId" class="form-control"
                                v-if="showContactField"
                                :disabled="!editMode">
                            <option v-for="contact in contactList"
                                    v-bind:value="contact.Id">
                                {{ contact.FirstName + ' ' + contact.LastName }}
                            </option>
                        </select>
                    </div>
                    <div class="form-group" >
                        <label>Nummer:</label>
                        <input v-model="editableSms.ContactNumber" class="form-control" :disabled="!editMode">
                    </div>
                    <div class="form-group" >
                        <label>Bericht:</label>
                        <textarea v-model="editableSms.Message" class="form-control" style="min-height: 100px" :disabled="!editMode"> </textarea>
                    </div>
                    <div class="form-group" >
                        <label>Status:</label>
                        <select v-model="editableSms.StatusId" class="form-control"
                                v-on:change="editableSmsSelectedStatusChanged(editableSms.StatusId)"
                                :disabled="!editMode">
                            <option v-for="status in statusList"
                                    v-bind:value="status.Id">
                                {{ status.Name }}
                            </option>
                        </select>
                    </div>
                    <div class="pull-right">
                        <button v-on:click="saveEdit()" class="btn btn-primary" v-if="editMode">Save</button>
                        <button v-on:click="showEditMode()" class="btn btn-primary" v-else>Edit</button>

                        <button v-on:click="hideEditMode()" class="btn btn-primary" v-if="editMode">Cancel</button>
                        <button v-on:click="deleteSms()" class="btn btn-danger" v-else>Delete</button>
                    </div>
                    

                </div>
            </div>
        </div>
  `
});

// main viewmodel
var app = new Vue({
    el: '#app',
    mixins: [connectionMethods],
    data: {
        message: 'Loading...',
        smsList: null,
        contactList: null,
        statusList: null,
        currentSms: null,
        showButtons: false,
        showModal: {
            NewSmsSelectModal: false,
            NewSmsContactModal: false,
            NewSmsNumberModal: false,
            ImportModal: false,
            ComposeModal: false,
            ValidateModal: false
        },
        newSms: null,
        sendStatus: null,
        importdata: {
            columns: [],
            data: []
        }
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
            style = 'list-group-item sms-list-item';
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
            var desiredLength = 30;
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
        },
        getFullContactName: function (sms) {
            var vm = this;

            if (sms.ContactFirstName != null && sms.ContactLastName != null) {
                return sms.ContactFirstName + ' ' + sms.ContactLastName + ' ';
            }

            return ""
        },

        // Modal pages
        showModalWindow: function (modalName) {
            var vm = this;

            var modals = Object.keys(vm.showModal);
            if (modals.includes(modalName)) {
                modals.forEach(m => { vm.showModal[m] = false; })
                vm.showModal[modalName] = true;
            }
        },
        openValidateModal: function (columns, data) {
            var vm = this;

            vm.importdata.columns = columns;
            vm.importdata.data = data;

            vm.showModalWindow('ValidateModal')
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

            Vue.set(this.smsList, smsIndex, smsDTO);

            if (this.currentSms !== null
                && this.currentSms.Id === smsDTO.Id) {
                this.selectSms(smsDTO);
                //this.smsList.filter(x => x.Id === smsDTO.Id)[0].isActive = true;
            }
        },
        removeSms: function (smsDTO) {
            this.smsList = this.smsList.filter(x => x.Id !== smsDTO.Id);
            this.currentSms = null;
        }
    },
    provide: function () {
        return {
            showModalWindow: this.showModalWindow,
            openValidateModal: this.openValidateModal
        };
    },
});