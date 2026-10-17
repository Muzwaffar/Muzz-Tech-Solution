import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";

export class UomHeader {//class that handles the listing screen
    constructor() {
        $(document).ready(() => {
            //close partial view on reload
            window.location.href = "#";
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var columnList = [
                { data: 'name', title: 'Name' },
            ];

            //(Table ID, Partial View ID, Button to open popup , url api to get list, column list (From above), class name where action function is defined)           
            DatatableService.initializeDataGridPopup('UomHeaderTable', 'partial', 'UomHeader.openPopupAddEdit(0, true)', variable.getListUomHeader, columnList, 'UomHeader');
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
       // if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().getByIdUomHeader, Id, (data) => {
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

    //delete 
    delete(Id) {
        //show confirmation box for delete
        //Parameters: API URL, Id to delete(data to pass), module name 
        AlertBoxService.deleteMasterAlertBox(new Variable().deleteUomHeader, Id, "Unit of Measure Header");
    }

    //Export Excel Event 
    export() {
        $('.buttons-excel').click();
    };
}
window.UomHeader = new UomHeader();