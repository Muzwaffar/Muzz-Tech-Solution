import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
export class TreeView {//class that handles the Template screen
    constructor() {
        let data = null;
        
        $(document).ready(() => {
            ApiService.callCustom(ApiType.POST, variable.getmatrix, "", (response) => {
                if (response) {
                    data = response.data
                    $('#tree').jstree({
                        "core": {
                            "animation": 0,
                            "check_callback": true,
                            "themes": { "icons": true },
                            'data': data
                        },
                        "types": {
                            "#": {
                                //"max_children": 10,
                                //"max_depth": 4,
                                "icon": "fa-solid fa-circle-notch",
                                "valid_children": ["root"]
                            },
                            "root": {
                                "icon": "fa-solid fa-circle-notch",
                                "valid_children": ["default"]
                            },
                            "default": {
                                "icon": "fa-regular fa-circle-dot",
                                "valid_children": ["default", "file"]
                            },
                            "file": {
                                "icon": "fa-regular fa-circle-dot",
                                "valid_children": []
                            }
                        },
                        "plugins": [
                            "contextmenu", "search",
                            "state", "types"
                        ]
                    });


                }
            });

            
        });
        
    }

    getData() {
        var treeData = $('#tree').jstree(true).get_json('#', { flat: true })

        ApiService.callCustom(ApiType.POST, variable.savematrix, treeData, (data) => {
            if (data) {
                toastr.success("Save Successful");
            } else {
                toastr.error("An error occurred. Please contact administrator.");
            }
        });
    }
}
window.TreeView = new TreeView();