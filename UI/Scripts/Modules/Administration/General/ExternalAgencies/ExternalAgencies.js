import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";

export class ExternalAgencies {//class that handles the External Agencies listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'name', title: 'Agency Name' },
                { data: 'description', title: 'Agency Description' },
                { data: 'contactNumber', title: 'Contact Number' }
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className
            DatatableService.initializeDataGridPopup('ExternalAgencyTable', 'partial', 'ExternalAgencies.openPopupAddEdit(0, true)', variable.getListExternalAgency, ColumnList, 'ExternalAgencies');
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().getByIdExternalAgency, Id, (data) => {
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
        //Parameters: API, ID, Message
        AlertBoxService.deleteMasterAlertBox(new Variable().deleteExternalAgency, Id, "External Agencies");
    }
}
window.ExternalAgencies = new ExternalAgencies();