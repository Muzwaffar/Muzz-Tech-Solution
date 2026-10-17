import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
export class ResourceType {//class that handles the ResourceType listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'name', title: 'Name' },
                { data: 'description', title: 'Description' },
                { data: 'isSystem', title: 'Is System' },
                { data: 'ignoreStation', title: 'Ignore Station' },
                
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className
            DatatableService.initializeDataGridPopup('ResourceTypeTable', 'partial', 'ResourceType.openPopupAddEdit(0, true)', variable.getListResourceType, ColumnList, 'ResourceType');
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().getByIdResourceType, Id, (data) => {
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

    //delete Feed
    delete(Id) {
        //Parameters: API, ID, Message
        AlertBoxService.deleteMasterAlertBox(new Variable().deleteResourceType, Id, "Resource Type");
    }
}
window.ResourceType = new ResourceType();