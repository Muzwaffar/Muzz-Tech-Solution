import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
export class Menu {//class that handles the Menu listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'displayName', title: 'Display Name' },
                { data: 'icon', title: 'Icon' },
                { data: 'order', title: 'Order' }
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className
            DatatableService.initializeDataGridPopup('MenuTable', 'partial', 'Menu.openPopupAddEdit(0, true)', variable.getListMenu, ColumnList, 'Menu', false, false);
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        
        if (Id == 0) {
            setTimeout(function () {
                window.location.href = "#";
            }, 10);
            toastr.info("No access to create a new module.");
        } else {
            //empty partial view content
            $(".content").html("")
            //call api to get partial view
            ApiService.callCustom(ApiType.POST, new Variable().getByIdMenu, Id, (data) => {
                if (data) {
                    //set partial view content
                    $(".content").html(data)
                    //Hide Save Button
                    if (!option) {
                        $(".noView").each(function (i, obj) {
                            obj.classList.add("d-none")
                        });
                    }
                }
            })
        }
    }

}
window.Menu = new Menu();