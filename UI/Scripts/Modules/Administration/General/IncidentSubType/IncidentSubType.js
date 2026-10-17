import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";

export class IncidentSubType {//class that handles the  screen
    constructor() {
        $(document).ready(() => {
            //close partial view on reload
            window.location.href = "#";
            //Column list   to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'code', title: 'Code' },
                { data: 'name', title: 'Incident Sub Type Name' },
                { data: 'incidentType', title: 'Incident Type' },
                { data: 'description', title: 'Description' }
            ];

            //(Table ID, Partial View ID, Button to open popup , url api to get list, column list (From above), class name where action function is defined)           
            DatatableService.initializeDataGridPopup('IncidentSubTypeTable', 'partial', 'IncidentSubType.openPopupAddEdit(0, true)', variable.getListIncidentSubType, ColumnList, 'IncidentSubType');
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().getByIdIncidentSubType, Id, (data) => {
            if (data) {
                //set partial view content
                $(".content").html(data)
                //Hide Save Button
                if (!option) {
                    $(".noView").each(function (i, obj) {
                        obj.classList.add("d-none")
                    });
                }
            }
        })
    }

    //Export Excel Event 
    export() {
        $('.buttons-excel').click();
    };

    //delete NAA
    delete(Id) {
        //show confirmation box for delete
        //Parameters: API URL, Id to delete(data to pass), module name 
        AlertBoxService.deleteAlertBox(new Variable().deleteIncidentSubType, Id, "Incident Sub Type");
    }
}
window.IncidentSubType = new IncidentSubType();