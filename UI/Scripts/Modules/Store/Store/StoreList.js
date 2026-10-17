import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
export class StoreList {//class that handles the Report Uoload listing screen
    tableData = [];
    constructor() {
        $(document).ready(() => {
             //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'name', title: 'Name' },
                { data: 'stationName', title: 'Station' }]
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className, hasDateRange
            DatatableService.initializeDataGridListingStatic('Table', '/Store/Store', new Variable().getListStore, ColumnList, 'StoreList', false, false, true);
            
        });
    }
    //delete  Report Upload
    delete(Id) {
        //Parameters: API, ID, Message
        AlertBoxService.deleteAlertBox(new Variable().deleteStore, Id, "Store");
    }
}
window.StoreList = new StoreList();