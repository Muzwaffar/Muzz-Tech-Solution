import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";

export class FormModuleMappingSequence {//class that handles the Form Page listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'module', title: 'Module' },
                { data: 'formName', title: 'Form Name' },
                { data: 'sequence', title: 'Sequence' },
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className, hasDateRange
            DatatableService.initializeDataGridPopup('Table', 'partial', 'FormModuleMappingSequence.openPopupAddEdit(0, true)', variable.getFormModuleMappingSequenceList, ColumnList, 'FormModuleMappingSequence');
        });
    }
    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().getByIdFormModuleMappingSequence, Id, (data) => {
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

    //delete Progrm of work
    delete(Id) {
        AlertBoxService.deleteAlertBox(new Variable().deleteFormModuleMappingSequence, Id, "Form Module Mapping Sequence");
    }

}
window.FormModuleMappingSequence = new FormModuleMappingSequence();