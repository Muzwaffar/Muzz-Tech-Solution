import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";

export class FormPagePDFMapping {//class that handles the Form Page listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'templateName', title: 'TemplateName' },
                { data: 'formName', title: 'Form Name' },
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className, hasDateRange
            DatatableService.initializeDataGridListingStatic('Table', '/FormPagePDFMapping/AddEditView', variable.getFormPagePDFMappingList, ColumnList, 'FormPagePDFMapping', false, false, true);
        });
    }

    //delete Progrm of work
    delete(Id) {
        AlertBoxService.deleteAlertBox(new Variable().deleteFormPagePDFMapping, Id, "FormPage PDF Mapping");
    }

}
window.FormPagePDFMapping = new FormPagePDFMapping();