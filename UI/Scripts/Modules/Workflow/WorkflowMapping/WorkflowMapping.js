import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";

export class WorkflowMapping {//class that handles the Form Page listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'name', title: 'Name' },
                { data: 'wfType', title: 'Workflow Type' },
                { data: 'attributeType', title: 'Attribute Type' },
                { data: 'module', title: 'Module' },
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className, hasDateRange
            DatatableService.initializeDataGridListing('Table', '/WorkflowMapping/AddEditView', variable.getWorkflowMappingList, ColumnList, 'WorkflowMapping', false, false, true);
        });
    }

    //delete Progrm of work
    delete(Id) {
        AlertBoxService.deleteAlertBox(new Variable().deleteWorkflowMapping, Id, "Workflow Mapping");
    }

}
window.WorkflowMapping = new WorkflowMapping();