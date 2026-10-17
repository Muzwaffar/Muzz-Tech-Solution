import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";

export class CallerList {//class that handles the Caller listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'fullName', title: 'Name' },
                { data: 'mobileNo', title: 'Contact Number' },
                {
                    data: 'isNuisance',
                    title: 'Is nuisance?',
                    render: function (data, type, row) {
                        if (data == true) {
                            return 'Yes';
                        } else if (data == false) {
                            return 'No';
                        } else {
                            return '';  // Empty for undefined or null
                        }
                    }
                },
                { data: 'remarks', title: 'Remarks' }
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className
            DatatableService.initializeDataGridListingStatic('CallerTable', '/Caller/Caller', new Variable().getListCaller, ColumnList, 'CallerList', false, false, true);
        });
    }

    //Export Excel Event 
    export() {
        $('.buttons-excel').click();
    };

    //delete Resource
    delete(Id) {
        //Parameters: API, ID, Message
        AlertBoxService.deleteAlertBox(new Variable().deleteCaller, Id, "Caller");
    }
}
window.CallerList = new CallerList();