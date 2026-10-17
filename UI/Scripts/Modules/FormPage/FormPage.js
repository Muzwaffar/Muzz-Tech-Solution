import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DateRangeService } from "/Scripts/Common/DateRangeService.js";

export class FormPage {//class that handles the Form Page listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'formName', title: 'Form Name' },
                {
                    data: 'colour', title: 'Colour', render: function (data, type, full, meta) {
                        return '<span style="color:' + data + '"><i class="fa fa-circle"></i></span>';
                    }
                },
                { data: 'additional1', title: 'Status' },
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className, hasDateRange
            DatatableService.initializeDataGridListing('Table', '/FormPage/AddEditView', "", ColumnList, 'FormPage', true, false, true);
        });
    }

    filterDatatable() {
        //get datepicker value
        let range = DateRangeService.formatReturnDate()
        //API 
        ApiService.callCustom(ApiType.POST, variable.getFormPageList, range, (data) => {
            if (data) {
                //update Datagrid
                DatatableService.updateDataGrid('Table', data.data)
            }
        })
    }

    //delete Progrm of work
    delete(Id) {
        AlertBoxService.deleteAlertBox(new Variable().deleteFormPage, Id, "Form Page");
    }

}
window.FormPage = new FormPage();