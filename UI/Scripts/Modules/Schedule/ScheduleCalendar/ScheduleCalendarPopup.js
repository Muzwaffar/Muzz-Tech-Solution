import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";

export class ScheduleCalendarPopup {//class that handles the Add Edit screen

    //Global Variables
    relationArray = [];
    relationData = [];
    relationDataFiles = [];

    constructor() {

        var relationColumn = [ //Relation Column List
            { data: 'assetName', title: 'Asset Name' },
            { data: 'equipementConsumable', title: 'Equipment or Consumable' },
            { data: 'quantity', title: 'Quantity' },
        ];

        var relationColumnFile = [ //Relation Column List
            { data: 'fileName', title: 'Name' },
            {
                title: "",
                //remove sorting on action
                orderable: false,
                //set width of action column
                width: "10%",
                render: function (data, type, full, meta) {
                    // checkbox button
                    data = ' <a onclick="ScheduleCalendarPopup.downloadFile(\'' + full.fileId + '\')" title="Download"><i class="fa fa-download text-secondary"></i></a>';
                    return data;
                }
            }
        ];


        //Field after ajax call 
        $(document).ajaxComplete(() => {
            //Init Relationship Datatable
            DatatableService.initializeDataGridCustom('relation-equpment-consumables-Table', relationColumn, this.relationData);
            DatatableService.initializeDataGridCustom('relation-file-Table', relationColumnFile, this.relationDataFiles);
        });

    }

    //API to get relation
    getRelationById(id) {
        this.relationArray = [];
        this.relationData = [];
        this.relationDataFiles = [];
        if (id != undefined && id != "00000000-0000-0000-0000-000000000000") {
            ApiService.callCustom(ApiType.POST, variable.getCalendarGrid, id, (data) => {
                this.relationData = data.data;
                if (this.relationData.length > 0) {
                    DatatableService.updateDataGrid("relation-equpment-consumables-Table", this.relationData);
                }
            })
            ApiService.callCustom(ApiType.POST, variable.getCalendarFiles, id, (data) => {
                this.relationDataFiles = data.data;
                if (this.relationDataFiles.length > 0) {
                    DatatableService.updateDataGrid("relation-file-Table", this.relationDataFiles);
                }
            })


        }
    }

    //downloadFile
    downloadFile(id) {
        $('#loader-common').show();
        location.href = "/Schedule/DownloadFile?dto=" + id;
        $('#loader-common').hide();
        toastr.success("File Downloaded.");
    }

}
//init new ScheduleCalendarPopup Class
window.ScheduleCalendarPopup = new ScheduleCalendarPopup();