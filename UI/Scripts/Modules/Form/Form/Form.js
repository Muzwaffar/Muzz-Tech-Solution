import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
export class Form {//class that handles the Category listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'name', title: 'Name' },
                {
                    data: 'startDate', title: 'Start Date', render: function (data, type, full, meta) {
                        data = moment(full.startDate).format("DD/MM/yyyy")
                        return data;
                    }
                },
                {
                    data: 'endDate', title: 'End Date', render: function (data, type, full, meta) {
                        data = moment(full.endDate).format("DD/MM/yyyy")
                        return data;
                    }
                },
                { data: 'formTypeName', title: 'Form Type Name' }
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className
            DatatableService.initializeDataGridPopup('FormTable', 'partial', 'Form.openPopupAddEdit(0, true)', variable.getFormList, ColumnList, 'Form');
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        ApiService.callCustom(ApiType.POST, new Variable().getByIdForm, Id, (data) => {
            if (data) {
                //set partial view content
                $(".content").html(data)
                FormAddEdit.getRelationById($("#Id").val())
                //Relation table columns
                var Col = [
                    {
                        title: "",
                        width: "10%",
                        render: function (data, type, full, meta) {
                            // checkbox button
                            data = '<input type="checkbox" id="' + full.id + '" name="checkbox" class="checkboxClass" />';
                            return data;
                        }
                    },
                    { data: 'name', title: 'Section Name' },
                    { data: 'description', title: 'Description' }
                ]

                //Init Section Datatable
                DatatableService.initializeDataGridServices('relation-Section-Table', Col, this.sections, false);
                //Hide Save Button
                if (!option) {
                    $(".noView").each(function (i, obj) {
                        obj.classList.add("d-none")
                    });
                }

            }
        })
     
    }

    //delete Form
    delete(Id) {
        //Parameters: API, ID, Message
        AlertBoxService.deleteAlertBox(new Variable().deleteForm, Id, "Form");
    }
}
window.Form = new Form();