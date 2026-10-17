import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";

export class IncidentType {//class that handles the  screen
    constructor() {
        $(document).ready(() => {
            //close partial view on reload
            window.location.href = "#";
            //Column list   to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'code', title: 'Code' },
                { data: 'name', title: 'Incident Type Name' },
                { data: 'description', title: 'Description' }
            ];

            //(Table ID, Partial View ID, Button to open popup , url api to get list, column list (From above), class name where action function is defined)           
            DatatableService.initializeDataGridPopup('IncidentTypeTable', 'partial', 'IncidentType.openPopupAddEdit(0, true)', variable.getListIncidentType, ColumnList, 'IncidentType');
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().getByIdIncidentType, Id, (data) => {
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

    //delete Incident Type
    delete(Id) {
        //show confirmation box for delete
        //Parameters: API URL, Id to delete(data to pass), module name 
        AlertBoxService.deleteAlertBox(new Variable().deleteIncidentType, Id, "Incident Type");
    }
}
window.IncidentType = new IncidentType();