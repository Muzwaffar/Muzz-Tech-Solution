import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class AccessRights {//class that handles the AR screen

    GridData;
    constructor() {
        $(document).ready(() => {
            //close partial view on reload
            window.location.href = "#";
            //Column list for AR to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ARColList = [
                { data: 'roleName', title: 'Role Name' }
            ];

            //(Table ID, Partial View ID, Button to open popup , url api to get list, column list (From above), class name where action function is defined)           
            DatatableService.initializeDataGridPopup('ARTable', 'partial', 'AccessRights.openPopupAddEdit(0, true)', variable.getARList, ARColList, 'AccessRights');
        });                                                                                                                                     
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, variable.GetARById, Id, (data) => {
            if (data) {
                //set partial view content
                $(".content").html(data);
                //Hide Save Button
                if (!option) {
                    $(".noView").each(function (i, obj) {
                        obj.classList.add("d-none")
                    });
                }
            }
        })

        var columns = [ //Module Column List
            {
            title: "Action",
            className: 'dt-control nested-action',
            orderable: false,
            data: null,
            defaultContent: ''
                },
            { data: 'level1', title: 'Main Menu' },
            { data: 'level2', title: 'Menu' },
            { data: 'level3', title: 'Sub Menu' }
        ];

        var childColumns = [ 
            {
                title: "",
                //remove sorting on action
                orderable: false,
                //set width of action column
                width: "10%",
                render: function (data, type, full, meta) {
                    var checked = "";
                    if (full.permission) {
                        checked = "checked";
                    }
                    var id = meta.settings.sTableId + "_" + full.operationTypeName + "_" + full.operationType + "_" + full.id;
                    // checkbox button
                    var datax = '<input type="checkbox" id="' + id + '" name="checkbox" ' + checked + ' onclick="AccessRights.valueChange(\'' + id + '\')"/>';
                    return datax;
                }
            },
            { data: 'operationTypeName', title: 'Operation' }
        ]

        let API = variable.GetARByRoleId;
        if (Validation.isNullOrUndefined(Id)) {
            API = variable.GetModules
        }

        ApiService.callCustom(ApiType.POST, API, Id, (data) => {
            if (data) {
                this.GridData = data.data;
                //Init Access Rights Datatable
                DatatableService.initializeDataGridNested('AccesRightsDataTable', columns, childColumns, this.GridData.accessRightDTO);
            }
        })

    }

    valueChange(id) {
        var retrive = id.split("_")
        var parentId = retrive[1];
        var operationTypeName = retrive[2];
        var operationType = retrive[3];

        this.GridData.accessRightDTO[parentId].accessRightDetailDTO.find(x => x.operationType == operationType).permission = $("#" + id).prop('checked');

        if (operationTypeName.toLowerCase() != "read" && $("#" + id).prop('checked')) {
            $('input[id^="child_' + parentId + '_Read"]').each(function () {
                var $this = $(this);
                $this.prop('checked', true)
            })
            this.GridData.accessRightDTO[parentId].accessRightDetailDTO.find(x => x.operationTypeName == "Read").permission = true;
        } else if (operationTypeName.toLowerCase() == "read" && !($("#" + id).prop('checked'))) {
            $('input[id^="child_' + parentId + '"]').each(function () {
                var $this = $(this);
                $this.prop('checked', false)
                
            })
            this.GridData.accessRightDTO[parentId].accessRightDetailDTO.forEach(dy => {
                dy.permission = false;
            })
        }

    }

    saveUpdate() {

        var rolename = $("#RoleName").val();
        if (Validation.isNullOrUndefined(rolename)) {
            toastr.error("Please Fill In Mandatory Fields")
        } else {
            Loader.showLoader("Saving Data...");
            this.GridData.roleName = rolename;
            this.GridData.roleId = $("#Id").val() == "" ? null : $("#Id").val();
            ApiService.callCustom(ApiType.POST, variable.createUpdateAR, this.GridData, (response) => {
                if (response.data) {
                    setTimeout(
                        location.reload()
                        ,500);
                    toastr.success(response.additionalData);
                    Loader.hideLoader();
                }
                //response false
                else if (!response.data) {
                    toastr.error(response.additionalData);
                }
                //failed
                else {
                    toastr.error("Error. Please contact Admin.");
                    return false;
                }
              
            })
        }
    }

    //delete AR
    delete(Id) {
        //show confirmation box for delete
        //Parameters: API URL, Id to delete(data to pass), module name 
        AlertBoxService.deleteAlertBox(variable.deleteAR, Id, "Role Access");
    }

    //Export Excel Event 
    export() {
        $('.buttons-excel').click();
    };

}
window.AccessRights = new AccessRights();