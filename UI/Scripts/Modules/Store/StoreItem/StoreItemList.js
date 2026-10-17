import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
export class StoreItemList {//class that handles the Report Uoload listing screen
    constructor() {
        $(document).ready(() => {
             //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'name', title: 'Name' },
                { data: 'uomDetails', title: 'UOM' },
                { data: 'group', title: 'Group Type' },
                { data: 'type', title: 'Category' },
                { data: 'category', title: 'Species' },
                { data: 'subCategory', title: 'Variety' },
                { data: 'isConsumable', title: 'Is Consumable?' }]
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className, hasDateRange
            DatatableService.initializeDataGridListingStatic('Table', '/StoreItem/StoreItem', new Variable().getListStoreItem, ColumnList, 'StoreItemList', false, false, true);
            
        });
    }
    
    //delete  Report Upload
    delete(Id) {
        //Parameters: API, ID, Message
        AlertBoxService.deleteAlertBox(new Variable().deleteStoreItem, Id, "Store Item");
    }
}
window.StoreItemList = new StoreItemList();