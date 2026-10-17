import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { CaseStatus } from "/Scripts/Common/Enums.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { LocalStorageValues } from "/Scripts/Common/Enums.js";
import { Validation } from "/Scripts/Common/ValidationService.js";

export function ApprovalPopup(id, url, CaseStatus) {
    //empty partial view content 
    $(".content").html("")
    //data to send
    let dat = {
        Id: id,
        Url: url,
        CaseStatus: CaseStatus
    }
    //call api to get partial view
    ApiService.callCustom(ApiType.POST, new Variable().getCommonApprovalView, dat, (data) => {
        if (data) {
            //set partial view content
            $(".content").html(data)
            //Dattable Column 
            var ColumnListUser = [
                { data: 'TId', title: 'Table Id', visible: false },
                { data: 'UId', title: 'User Id', visible: false },
                { data: 'Username', title: 'User' }
            ];
            // Init Trial Dtatable
            // Parameters: Table Id, list of columns, data, has delete action? , delete action function
            DatatableService.initializeDataGridServices("UsersDataTable", ColumnListUser, [], true, "ApprovalService.deleteFromTable")
            ApiService.callCustom(ApiType.POST, new Variable().getCommonAllDropdownApproval, "", (response) => {
                if (response) {
                    var dropdown = response.data;
                    DropdownService.initDropdown("select-app-department", dropdown, "Department");
                    $('#select-app-department, #select-app-resourceType').on('select2:select', () => {
                        GetUserDropdown();
                    });
                    DropdownService.initDropdown("select-app-resourceType", dropdown, "ResourceType");
                    DropdownService.initDropdownNull("select-app-users");

                    ApiService.callCustom(ApiType.POST, new Variable().getAssignToMe, "", (responseToMe) => {
                        if (responseToMe) {
                            if (responseToMe.data.approvals.length > 0) {
                                let dataToUpdate = [];
                                for (var datum of responseToMe.data.approvals) {
                                    let datumFormatted = {
                                        TId: datum.tId,
                                        UId: datum.uId,
                                        Username: datum.username
                                    };
                                    dataToUpdate.push(datumFormatted);
                                }
                                DatatableService.updateDataGrid("UsersDataTable", dataToUpdate)
                            }
                        }
                    })
                }
            })


        }
    })
}

export function ActivityPopup(id, url, CaseStatus) {
    //empty partial view content 
    $(".content").html("")
    //data to send
    let dat = {
        Id: id,
        Url: url,
        CaseStatus: CaseStatus
    }
    //call api to get partial view
    ApiService.callCustom(ApiType.POST, new Variable().getCommonApprovalView, dat, (data) => {
        if (data) {
            //set partial view content
            $(".content").html(data)
            $("#approval-popup-title").html("Send As Activity")
            //Dattable Column 
            var ColumnListUser = [
                { data: 'TId', title: 'Table Id', visible: false },
                { data: 'UId', title: 'User Id', visible: false },
                { data: 'Username', title: 'User' }
            ];
            // Init Trial Dtatable
            // Parameters: Table Id, list of columns, data, has delete action? , delete action function
            DatatableService.initializeDataGridServices("UsersDataTable", ColumnListUser, [], true, "ApprovalService.deleteFromTable")
            ApiService.callCustom(ApiType.POST, new Variable().getCommonAllDropdownApproval, "", (response) => {
                if (response) {
                    var dropdown = response.data;
                    DropdownService.initDropdown("select-app-department", dropdown, "Department");
                    $('#select-app-department, #select-app-resourceType').on('select2:select', () => {
                        GetUserDropdown();
                    });
                    DropdownService.initDropdown("select-app-resourceType", dropdown, "ResourceType");
                    DropdownService.initDropdownNull("select-app-users");

                    ApiService.callCustom(ApiType.POST, new Variable().getAssignToMe, "", (responseToMe) => {
                        if (responseToMe) {
                            if (responseToMe.data.approvals.length > 0) {
                                let dataToUpdate = [];
                                for (var datum of responseToMe.data.approvals) {
                                    let datumFormatted = {
                                        TId: datum.tId,
                                        UId: datum.uId,
                                        Username: datum.username
                                    };
                                    dataToUpdate.push(datumFormatted);
                                }
                                DatatableService.updateDataGrid("UsersDataTable", dataToUpdate)
                            }
                        }
                    })
                }
            })

        }
    })
}

export function SaveApprovalStatus(id, url, status, caseStatus) {
    let urlToSave = "/" + location.href.replace("#", "").replace("partial", "").replace(new Variable()._baseInternalApi, "")
    let data = {
        Id: id,
        Url: urlToSave,
        CaseStatus: caseStatus,
        ApprovalStatus: status
    }
    Loader.showLoader("Saving Approval...");
    ApiService.callCustom(ApiType.POST, new Variable().WorkflowSave, data, (response) => {
        if (response) {
            setTimeout(
                function () {
                    const queryString = window.location.search;
                    const urlParams = new URLSearchParams(queryString);
                    location.href = urlParams.get('Back');
                },
                1000);
        }
    })
    Loader.hideLoader();
}

export function GetUserDropdown() {
    var data = {
        DepartmentId: $('#select-app-department').val() == "" ? 0 : parseInt($('#select-app-department').val()),
        ResourceTypeId: $('#select-app-resourceType').val() == "" ? 0 : parseInt($('#select-app-resourceType').val())
    }
    if (data.department != 0 && data.resource != 0) {
        Loader.showLoader("Fetching Data...");
        ApiService.callCustom(ApiType.POST, new Variable().getCommonUserDropdownApproval, data, (response) => {
            if (response) {
                var dropdown = response.data;
                DropdownService.destroyReinitDropdown("select-app-users", dropdown, "Users");
                Loader.hideLoader();
            }
        })
        Loader.hideLoader();
    }

}

export class ApprovalService {//class that handles the listing screen
    table = [];
    constructor() {

        $(document).ready(() => {
        });
    }

    //open approval
    saveApproval() {
        if ($("#UsersDataTable").DataTable().rows().data() != undefined) {
            if ($("#UsersDataTable").DataTable().rows().data()[0] != undefined) {
                this.table.push($("#UsersDataTable").DataTable().rows().data()[0])
            }
        }
        if (this.table.length == 0) {
            toastr.error("Please select at least one user")
        } else {
            let url = $("#Approval-Url").val();
            let urlToSave = "";
            if (location.href.includes("&Id=")) {
                urlToSave = "/" + location.href.replace("#", "").replace("partial", "").replace(new Variable()._baseInternalApi, "")
            } else {
                let loc = location.href;
                if (location.href.includes("&MId=")) {
                    loc = location.href.split("&MId=")[0]
                }
                urlToSave = "/" + loc.replace("#", "").replace("partial", "").replace(new Variable()._baseInternalApi, "") + "&Id=" + $("#Approval-Id").val() + "&MId=" + sessionStorage.getItem(LocalStorageValues.MODULEID);
            }

            //data to send
            let dat = {
                Id: $("#Approval-Id").val(),
                Url: urlToSave,
                ApprovalRemarks: $("#ApprovalRemarks").val(),
                CaseStatus: CaseStatus[$("#Approval-CaseStatus").val()],
                WFUserDTO: this.table

            }
            //call api to get partial view
            ApiService.callCustom(ApiType.POST, new Variable().WorkflowSave, dat, (data) => {
                if (data) {
                    toastr.success("Approval Saved Successfully")
                    setTimeout(
                        function () {
                            const queryString = window.location.search;
                            const urlParams = new URLSearchParams(queryString);
                            location.href = urlParams.get('Back');
                        },
                        1000);
                } else {
                    toastr.error("An error occurred. Please contact Admin")
                }
            })
        }
    }
    //open SendActivty
    sendAsActivity() {
        if ($("#UsersDataTable").DataTable().rows().data() != undefined) {
            if ($("#UsersDataTable").DataTable().rows().data()[0] != undefined) {
                this.table.push($("#UsersDataTable").DataTable().rows().data()[0])
            }
        }
        if (this.table.length == 0) {
            toastr.error("Please select at least one user")
        } else {
            let url = $("#Approval-Url").val();
            let urlToSave = "";
            if (location.href.includes("&Id=")) {
                urlToSave = "/" + location.href.replace("#", "").replace("partial", "").replace(new Variable()._baseInternalApi, "")
            } else {
                urlToSave = "/" + location.href.replace("#", "").replace("partial", "").replace(new Variable()._baseInternalApi, "") + "&Id=" + $("#Approval-Id").val() + "&MId=" + sessionStorage.getItem(LocalStorageValues.MODULEID);
            }
            //data to send
            let dat = {
                Id: $("#Approval-Id").val(),
                Form: $("#Approval-Form").val(),
                Url: urlToSave,
                ApprovalRemarks: $("#ApprovalRemarks").val(),
                CaseStatus: CaseStatus[$("#Approval-CaseStatus").val()],
                WFUserDTO: this.table

            }
            //call api to get partial view
            ApiService.callCustom(ApiType.POST, new Variable().sendAsActivity, dat, (data) => {
                if (data) {
                    toastr.success("Approval Saved Successfully")
                    setTimeout(
                        function () {
                            const queryString = window.location.search;
                            const urlParams = new URLSearchParams(queryString);
                            location.href = urlParams.get('Back');
                        },
                        1000);
                } else {
                    toastr.error("An error occurred. Please contact Admin")
                }
            })
        }
    }
    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["select-app-department", "select-app-resourceType", "select-app-users"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        if (this.table.find(x => x.UId == $('#select-app-users').val())) {
            toastr.error("Cannot enter same username");
            valid = false
        }
        //return boolean id valid or not
        return valid
    }
    //Add To User Table 
    addUserToTable() {
        if (this.valid()) {
            if ($("#UsersDataTable").DataTable().rows().data() != undefined) {
                if ($("#UsersDataTable").DataTable().rows().data()[0] != undefined) {
                    this.table.push($("#UsersDataTable").DataTable().rows().data()[0])
                }
            }
            let data = {
                TId: this.table.length + 1,
                UId: $('#select-app-users').val(),
                Username: $('#select-app-users option:selected').text()
            }

            this.table.push(data)
            DatatableService.updateDataGrid("UsersDataTable", this.table);
            Validation.clearValues(["select-app-department", "select-app-resourceType", "select-app-users"])
        }
    }

    //Delete relation
    deleteFromTable(meta) {
        // remove data from list using index
        this.table.splice(meta, 1);
        //update trial table 
        DatatableService.updateDataGrid("UsersDataTable", this.table);
    }

}
window.ApprovalService = new ApprovalService();