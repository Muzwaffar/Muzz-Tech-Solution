import { DatatableService } from "/Scripts/Common/DatatableService.js";
export class TemplateList {//class that handles the Template screen
    constructor() {
        $(document).ready(() => {
            //close partial view on reload
            window.location.href = "#";
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'name', title: 'Name' },
                { data: 'lookUpType.name', title: 'Lookup Type Name' },
                { data: 'description', title: 'Description' },
            ];

            //(Table ID, Partial View ID, Button to open popup , url api to get list, column list (From above), class name where action function is defined)           
            DatatableService.initializeDataGridListing('TemplateTable', 'partial', 'LookupValue.openPopupAddEdit(0)', variable.getListLKvalue, ColumnList, 'LookupValue');
        });

        
    }

    //Export Excel Event 
    export() {
        $('.buttons-excel').click();
    };
}
window.TemplateList = new TemplateList();