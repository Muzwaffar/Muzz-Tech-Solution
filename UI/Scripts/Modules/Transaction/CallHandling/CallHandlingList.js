import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { LocalStorageValues } from "/Scripts/Common/Enums.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
export class CallHandlingList {//class that handles the Report Uoload listing screen
    tableData = [];
    constructor() {
        $(document).ready(() => {
             //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'incidentNo', title: 'Incident No' },
                { data: 'parentIncident', title: 'Parent Incident' },
                { data: 'station', title: 'Station' },
                { data: 'incidentType', title: 'Incident Type' },
                { data: 'incidentSubType', title: 'Incident Sub Type' },
                { data: 'incidentCode', title: 'Incident Code' },
                { data: 'natureOfIncident', title: 'Nature Of Incident' },
                { data: 'location', title: 'Location' },
                {
                    data: 'dateSubmitted', title: 'Date Submitted To Station', render: function (data, type, full, meta) {
                        data = full.dateSubmitted;
                        if (data != "") {
                            data = moment(full.dateSubmitted).format("DD/MM/YYYY HH:mm")
                        }
                        return data;
                    }
                },
                { data: 'status', title: 'Status' },
                { data: 'callInStatus', title: 'Call In Status' },
                { data: 'crewStatus', title: 'Crew Status' },
                { data: 'dateCreated', title: 'Date created' }
            ]
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className, hasDateRange
            DatatableService.initializeDataGridListingStatic('INCtable', '/IncidentHandling/AddEdit','', ColumnList, 'CallHandlingList', true, false, true,true, " d-none", null, "d-none", "CallHandlingList.openNewRecord()");
            
        });
    }
    //filtertable
    filterDatatable() {
        //get datepicker value
        var dateRange = $('#DateRangePicker').val();
        var dateRangeT = dateRange.split(" ");
        //format Date
        var rosFrom = this.formatDate(dateRangeT[0]);
        var rosTo = this.formatDate(dateRangeT[2]);
        //dto to send date from and To
        var ros = {
            From: moment(rosFrom, "DD-MM-YYYY").add(4, 'hours'),
            To: moment(rosTo, "DD-MM-YYYY").add(4, 'hours')
        };
        ApiService.callCustom(ApiType.POST, variable.getCallHandlingList, ros, (data) => {
            if (data) {
                //update Datagrid
                DatatableService.updateDataGrid('INCtable', data.data)
            }
        })
    }
    //format Date
    formatDate(dat) {
        const [month, day, year] = dat.split('/');
        const date = new Date(+year, +month - 1, +day);
        return date;
    }
    //Open New Record
    openNewRecord() {
        ApiService.callCustom(ApiType.POST, new Variable().initialiseCallHandling, localStorage.getItem(LocalStorageValues.USERTOKEN), (resp) => { //Calls api to get data
            if (resp) {
                window.location.href = '/IncidentHandling/AddEdit?Id=' + resp.id + '&UserToken=' + localStorage.getItem(LocalStorageValues.USERTOKEN) + '&Code=' + resp.additionalData;
            }
        })
    }
    //delete  Report Upload
    delete(Id) {
        //Parameters: API, ID, Message
        AlertBoxService.deleteAlertBox(new Variable().getCallHandlingDelete, Id, "Call Handling");
    }
}
window.CallHandlingList = new CallHandlingList();