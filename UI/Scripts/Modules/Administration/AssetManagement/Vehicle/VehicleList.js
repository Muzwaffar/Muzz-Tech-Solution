import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";

export class VehicleList {//class that handles the Report Uoload listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'registrationNo', title: 'Registration No' },
                { data: 'make', title: 'Make' },
                { data: 'model', title: 'Model' },
                { data: 'category', title: 'Category' },
                { data: 'abbreviation', title: 'Abbreviation' },
                { data: 'stationName', title: 'Station Name' },
               ]
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className, hasDateRange
            DatatableService.initializeDataGridListingStatic('Table', '/Vehicle/Vehicle', new Variable().getListVehicle, ColumnList, 'VehicleList', false, false, false, false);

        });
    }

    //delete  Report Upload
    delete(Id) {
        //Parameters: API, ID, Message
        AlertBoxService.deleteAlertBox(new Variable().deleteVehicle, Id, "Vehicle");
    }
}
window.VehicleList = new VehicleList();