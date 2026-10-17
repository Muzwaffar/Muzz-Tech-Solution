import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
export class TreeView {//class that handles the Template screen
    constructor() {

        let data = [
            {
                "id": 1,"text": "Root node", "children": [
                    { "id": 2, "text": "Child node 1" },
                    { "id": 3, "text": "Child node 2" }
                ]
            }
        ]

        $(document).ready(() => {
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


        });
        
    }

    getData() {
        var v = $('#tree').jstree(true).get_json('#', { flat: true })
        console.log(v)
    }
}
window.TreeView = new TreeView();