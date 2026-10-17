import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";

export class FormSectionAndElementModuleMapping {//class that handles the Form Page listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'form', title: 'Form' },
                { data: 'module', title: 'Module' }
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className, hasDateRange
            DatatableService.initializeDataGridListingStatic('Table', '/FormSectionAndElementModuleMapping/AddEditView', variable.getFormSectionAndElementModuleMappingList, ColumnList, 'FormSectionAndElementModuleMapping', false, false, true);
        });
    }

    //delete Progrm of work
    delete(Id) {
        AlertBoxService.deleteAlertBox(new Variable().deleteFormSectionAndElementModuleMapping, Id, "Form Section And Element Module Mapping");
    }

}
window.FormSectionAndElementModuleMapping = new FormSectionAndElementModuleMapping();