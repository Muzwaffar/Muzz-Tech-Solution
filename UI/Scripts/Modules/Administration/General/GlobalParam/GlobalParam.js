import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
export class GlobalParam {//class that handles the GlobalParam listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'name', title: 'Name' },
                { data: 'value', title: 'Value' },
                { data: 'additionalValue', title: 'Additional Value' },
                { data: 'comment', title: 'Comment' },
                
               
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className
            DatatableService.initializeDataGridPopup('GlobalParamTable', 'partial', 'GlobalParam.openPopupAddEdit(0, true)', variable.getListGlobalParam, ColumnList, 'GlobalParam', false, false);
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().getByIdGlobalParam, Id, (data) => {
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
}
window.GlobalParam = new GlobalParam();