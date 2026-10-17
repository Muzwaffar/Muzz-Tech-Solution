import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
export class OfficeZones {//class that handles the OfficeZones listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'officeName', title: 'Office Name' },
                { data: 'department', title: 'Department' },
                { data: 'locality', title: 'Locality' },
                { data: 'zone', title: 'Zone' }
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className
            DatatableService.initializeDataGridPopup('OfficeZonesTable', 'partial', 'OfficeZones.openPopupAddEdit(0, true)', variable.getListOfficeZones, ColumnList, 'OfficeZones');
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().getByIdOfficeZones, Id, (data) => {
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

    //delete OfficeZones
    delete(Id) {
        //Parameters: API, ID, Message
        AlertBoxService.deleteMasterAlertBox(new Variable().deleteOfficeZones, Id, "Office Zones");
    }
}
window.OfficeZones = new OfficeZones();