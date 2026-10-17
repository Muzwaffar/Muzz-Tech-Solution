import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DateRangeService } from "/Scripts/Common/DateRangeService.js";
export class ScheduleList {//class that handles the Land Requirement listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'name', title: 'Job Name' },
                { data: 'startDate', title: 'Start Date', render: function (data, type, full, meta) {
                    data = moment(full.startDate).format("DD/MM/yyyy")
                    return data;
                } },
                {
                    data: 'endDate', title: 'End Date', render: function (data, type, full, meta) {
                        data = moment(full.endDate).format("DD/MM/yyyy")
                        return data;
                    } },
                { data: 'status', title: 'Status'}
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className, hasDateRange
            DatatableService.initializeDataGridListing('Table', '/Schedule/AddEditView', "", ColumnList, 'ScheduleList', true, false, true, "", null, "d-none");
        });
    }
    filterDatatable() {
        //get datepicker value
        let range = DateRangeService.formatReturnDate()
        //API 
        ApiService.callCustom(ApiType.POST, variable.getListScheduleByRange, range, (data) => {
            if (data) {
                //update Datagrid
                DatatableService.updateDataGrid('Table', data.data)
            }
        })
    }

    //Export Excel Event 
    export() {
        $('.buttons-excel').click();
    };

    //delete Land Requirement
    delete(Id) {
        //Parameters: API, ID, Message
        AlertBoxService.deleteAlertBox(new Variable().deleteSchedule, Id, "Schedule");
    }
}
window.ScheduleList = new ScheduleList();