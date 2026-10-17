import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
export class Station {//class that handles the Station listing screen
    constructor() {
        $(document).ready(() => {
            //Column list for Make to build table (predefined columns)
            //data: object key
            //title: column title to show
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'code', title: 'Code' },
                { data: 'name', title: 'Station Name' },
                { data: 'stationType.name', title: 'Station Type' },
                { data: 'stationLocation.name', title: 'Station Location' },
                { data: 'stationCallSign', title: 'Station Call Sign' },
                { data: 'phoneNumber', title: 'Phone Number' },
                { data: 'contactNumber', title: 'Contact Number' },
                { data: 'fax', title: 'Fax' },
                { data: 'email', title: 'Email' },
            ];
            //Init Dtatable
            //tableID, addNewLink, url, columnList, className
            DatatableService.initializeDataGridPopup('StationTable', 'partial', 'Station.openPopupAddEdit(0, true)', variable.getListStation, ColumnList, 'Station');
        });
    }

    //open popup partial view
    openPopupAddEdit(Id, option = false) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().getByIdStation, Id, (data) => {
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

    //Export Excel Event 
    export() {
        $('.buttons-excel').click();
    };

    //delete Station
    delete(Id) {
        //Parameters: API, ID, Message
        AlertBoxService.deleteMasterAlertBox(new Variable().deleteStation, Id, "Station");
    }
}
window.Station = new Station();