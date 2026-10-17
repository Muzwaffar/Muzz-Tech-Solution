import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";

export class UomDetails {//class that handles the listing screen
    constructor() {
        $(document).ready(() => {
            //close partial view on reload
            window.location.href = "#";
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var columnList = [
                { data: 'name', title: 'Name' },
                { data: 'uomHeader.name', title: 'Unit of Measure Header Name' },
                { data: 'unit', title: 'Unit' },
                { data: 'relativeFactor', title: 'Relative Factor' },
            ];

            //(Table ID, Partial View ID, Button to open popup , url api to get list, column list (From above), class name where action function is defined)           
            DatatableService.initializeDataGridPopup('UomDetailsTable', 'partial', 'UomDetails.openPopupAddEdit(0, true)', variable.getListUomDetailse, columnList, 'UomDetails');
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
       // if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().getByIdUomDetails, Id, (data) => {
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
        AlertBoxService.deleteMasterAlertBox(new Variable().deleteUomDetails, Id, "Unit of Measure Details");
    }

    //Export Excel Event 
    export() {
        $('.buttons-excel').click();
    };
}
window.UomDetails = new UomDetails();