import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
export class Resource {//class that handles the Resource listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'station', title: 'Station' },
                { data: 'fullName', title: 'Name' },
                { data: 'resourceType', title: 'Resource Category' },
                { data: 'rank', title: 'Rank' },
                { data: 'pfNumber', title: 'PF Number' },
                { data: 'mobileNo', title: 'Mobile Number' },
                { data: 'userId', title: 'User' }
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className
            DatatableService.initializeDataGridListingStatic('ResourceTable', '/Resource/Resource', new Variable().getListResource, ColumnList, 'Resource', false, false, true);
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().getByIdResource, Id, (data) => {
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

    //delete Resource
    delete(Id) {
        //Parameters: API, ID, Message
        AlertBoxService.deleteAlertBox(new Variable().deleteResource, Id, "Resource");
    }
}
window.Resource = new Resource();