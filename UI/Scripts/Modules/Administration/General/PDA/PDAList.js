import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";

export class PDAList {//class that handles the Roster screen
    constructor() {
        $(document).ready(() => {
            //close partial view on reload
            window.location.href = "#";
            //Column list for Roster to build table (predefined columns)
            //data: object key
            //title: column title to show
            var PDAColList = [
                { data: 'code', title: 'Code' },
                { data: 'stationTypeName', title: 'Station Type' },
                { data: 'natureOfIncidentName', title: 'Nature Of Incident' },
                { data: 'incidentTypeName', title: 'Incident Type' },
            ];

            //(Table ID, Partial View ID, Button to open popup , url api to get list, column list (From above), class name where action function is defined)           
            DatatableService.initializeDataGridListingStatic('PDATable', '/PDA/GetById', variable.getPDAList, PDAColList, 'PDAList',false,false,true);
            
            
        });
    }

    //delete Roster
    delete(Id) {
        //show confirmation box for delete
        //Parameters: API URL, Id to delete(data to pass), module name 
        AlertBoxService.deleteAlertBox(new Variable().deletePDA, Id, "PDA");
    }
}
window.PDAList = new PDAList();