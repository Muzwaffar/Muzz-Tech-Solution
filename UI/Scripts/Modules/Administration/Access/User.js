import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
export class User {//class that handles the User listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'userCode', title: 'Code' },
                { data: 'fullName', title: 'Name' },
                { data: 'email', title: 'Email' },
                { data: 'office', title: 'Office' },
                { data: 'postHeld', title: 'Post Held' }
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className
            DatatableService.initializeDataGridPopup('UserTable', 'partial', 'User.openPopupAddEdit(0, true)', variable.getUserList, ColumnList, 'User');
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().addEditUser, Id, (data) => {
            if (data) {
                //set partial view content
                $(".content").html(data)
                // Call api to get user matrix
                ApiService.callCustom(ApiType.POST, new Variable().getUserMatrix, Id, (response) => {
                    if (response) {
                        let userMatrix = response.data;
                        $('#tree').jstree({
                            "core": {
                                "animation": 0,
                                "check_callback": true,
                                "themes": { "icons": false },
                                'data': userMatrix
                            },
                            "types": {
                                "#": {
                                    "valid_children": ["root"]
                                },
                                "root": {
                                    "valid_children": ["default"]
                                },
                                "default": {
                                    "valid_children": ["default", "file"]
                                },
                                "file": {
                                    "valid_children": []
                                }
                            },
                            "checkbox": {
                                "tie_selection": false,
                                "three_state": false,
                            },
                            "plugins": [
                                "checkbox"
                            ]
                        });


                    }
                });
                //Hide Save Button
                if (!option) {
                    $(".noView").each(function (i, obj) {
                        obj.classList.add("d-none")
                    });
                }
            }
        })
    }
    reset(id) {
        AlertBoxService.resetPassword(new Variable().resetPassword, id, "User");
    }
    //Export Excel Event 
    export() {
        $('.buttons-excel').click();
    };

    //delete User
    delete(Id) {
        //Parameters: API, ID, Message
        AlertBoxService.deleteAlertBox(new Variable().deleteUser, Id, "User");
    }
}
window.User = new User();