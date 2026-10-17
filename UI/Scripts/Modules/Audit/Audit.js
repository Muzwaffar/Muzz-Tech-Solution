import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class Audit {//class that handles screen
    dat = null;
    constructor() {
       
        $(document).ready(() => {
            //Column list  to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                {
                    title: "Action",
                    className: 'dt-control nested-action',
                    orderable: false,
                    data: null,
                    defaultContent: ''
                },
                { data: 'id', title: 'Audit Id' },
                { data: 'entityName', title: 'Entity Name' },
                { data: 'timeStamp', title: 'DateTime', render: DataTable.render.datetime('DD/MM/YYYY HH:mm:ss'), },
                { data: 'userEmail', title: 'User Email' },
                { data: 'type', title: 'Operation Type' },
                
            ];

            var childColumnList = [
                /*{ data: 'columnName', title: 'Id', visible: false },*/
                { data: 'columnName', title: 'Column Name' },
                { data: 'original', title: 'Original Value' },
                { data: 'newValue', title: 'New Value' },
            ];

            //(Table ID, Partial View ID, Button to open popup , url api to get list, column list (From above), class name where action function is defined)           
            DatatableService.initializeDataGridNested('AuditTable',ColumnList, childColumnList);
         
            //Initialise Dropdowns
            //Entity Name Select
            //DropdownService.initMultiDropdownNull("select-entity");
            DropdownService.initMultiDropdownNullWithAll("select-entity");
            //Operation Type Select
            DropdownService.initDropdownNull("select-operation");
            //User Select
            //DropdownService.initMultiDropdownNull("select-user");
            DropdownService.initMultiDropdownNullWithAll("select-user");

            //call api to get available app user, asset, team
            ApiService.callCustom(ApiType.POST, variable.getAllDropdownValuesAudit, "", (data) => {
                if (data) {
                    //get global variable 
                    this.dropdownData = data.data;
                    //Entity Name Select
                    DropdownService.initMultiDropdownWithAll("select-entity", this.dropdownData, "EntityName");
                    //Operation Type Select
                    DropdownService.initDropdown("select-operation", this.dropdownData, "OperationType");
                    //User Select
                    DropdownService.initMultiDropdownWithAll("select-user", this.dropdownData, "User");
                }
            })
            $("#excelExport").click((e) => {
                if (this.dat != null) {
                    ApiService.callCustom(ApiType.POST, variable.downloadAuditReport, this.dat, (response) => {
                        this.downloadBlob(response.data.byteArray, response.data.fileName)
                    })
                } else {
                    toastr.warning("No data to export")
                }

            });

        });
    }
    //download Blob for file export excel
    downloadBlob(blob, name) {
        var linkSource = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + blob;
        const downloadLink = document.createElement("a");
        downloadLink.href = linkSource;
        downloadLink.download = name;
        downloadLink.click();



    }
    //search filter --> on click search
    searchFilter() {
        // get all values
        let data = DefaultService.result(document.getElementById("Audit-Panel"));

        //check if required fields are input
        if (data == false) {
            //show error if required fields not input
            toastr.error("Please select required filters")
            return;

        } else {
            var entityNameList = [];
            if ($('#select-entity').select2("data").length > 0) {
                let entityNames = $('#select-entity').select2('data');
                for (let i = 0; i < entityNames.length; i++) {
                    entityNameList.push(entityNames[i].text);
                }
                data["EntityName"] = entityNameList;
            } else {
                toastr.error("Please select required filters")
                return;
            }

            if ($('#select-operation').select2("data")[0].text == '' || $('#select-user').select2("data").length == 0) {
                toastr.error("Please select required filters")
                return;
            }

            //date validation
            if (data["To"] < data["From"]) {
                toastr.error("End Date should be set after Start Date")
                return;
            }

            //get values
            data["Type"] = $('#select-operation').select2("data")[0].text;

            //user
            var userList = [];
            if ($('#select-user').select2("data").length > 0) {
                var userSelection = $('#select-user').select2('data');
                var userDropdown = this.dropdownData.find(x => x.title == 'User').items;
                for (var i = 0; i < userSelection.length; i++) {
                    var id = userDropdown.find(x => x.text == userSelection[i].text).id;
                    userList.push(id)
                }
                data["UserId"] = userList;
            } else {
                toastr.error("Please select required filters")
                return;
            }

            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.getAllAudit, data, (response) => {
                //Response true
                if (response.data) {
                    this.dat = response.data;
                    DatatableService.updateDataGrid("AuditTable", response.data)
                    //toastr.success("Saved Successfully!")
                }
                //response false
                else if (!response.data) {
                    toastr.error(response.additionalData);
                }
                //failed
                else {
                    toastr.error("Error Fetching Record.");
                    return false;
                }
            }, () => {
                Loader.hideLoader();
            })
        }
    }
    

}
window.Audit = new Audit();