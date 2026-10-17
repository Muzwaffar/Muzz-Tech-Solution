import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { MapService } from "/Scripts/Common/Map/MapService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";

export class ZoneAddEdit {//class that handles the Add Edit screen
    constructor() {
        var dropdown = null;
        
        ApiService.callCustom(ApiType.POST, variable.getDropDownZone, "", (data) => {
            if (data) {
                dropdown = data.data;
            }
        })
        
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null || dropdown != undefined) {
                //Make Select
                DropdownService.initDropdown("select-zonetype", dropdown, "ZoneType");

                //Set Dropdown Value on edit 
                $("#select-zonetype").val($("#select-zonetype-id").val()).trigger('change');
            }
            
            MapService.initMap();
            setTimeout(e => {
                this.minimizeMap();
            }, 100)
        });
    }

    //Button save ot update
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("Zone-AddEdit-Panel"));
        
        //check if required fields are input
        if (data == false ) {
            //show error if required fields not input
            toastr.error("Please Fill In Mandatory Fields")
        } else {
            //get zone type value
            //data["ZoneType"] = DefaultService.resultObject("ZoneType", document.getElementById("Zone-AddEdit-Panel"));
            ////check zone type dropdown
            //if (!Validation.checkDropdownSelected(data["ZoneType"].Id)) {
            //    toastr.error("Please Fill In Mandatory Fields")
            //    return;
            //}
            
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateZone, data, (response) => {
                //Response true
                if (response.data) {
                    toastr.success("Saved Successfully!")
                    window.location.reload();
                }
                //response false
                else if (!response.data) {
                    toastr.error(response.additionalData);
                }
                //failed
                else {
                    toastr.error("Could not open the record for edit.");
                    return false;
                }
            }, () => {
                $('#loader-common').hide();
            })
        }
    }
    //Enlarge / Minimize map
    enlargeManimizeMap() {
        var buttonCustom = document.getElementById("enlarge-minimize");
        var isMinimized = buttonCustom.innerHTML.includes('Enlarge')
        if (isMinimized) {
            buttonCustom.innerHTML = "Minimize Map";
            var popup = document.getElementById("zone-popup");
            popup.style.margin = '75px auto';
            popup.style.width = '90vw';
            popup.style.maxHeight = '90vh';
            popup.style.height = '90vh';
            var content = document.getElementById("zone-content");
            content.style.maxHeight = 'calc(100vh - 7vh - 8vh)';
            var map = document.getElementById("map");
            map.style.height = '70vh';
        } else {
            buttonCustom.innerHTML = "Enlarge Map";
            var popup = document.getElementById("zone-popup");
            popup.style.margin = '100px auto';
            popup.style.width = '50vw';
            popup.style.maxHeight = '75vh';
            popup.style.height = '75vh';
            var content = document.getElementById("zone-content");
            content.style.maxHeight = 'calc(100vh - 7vh - 30vh)';
            var map = document.getElementById("map");
            map.style.height = '40vh';
        }
    }

    minimizeMap() {
        var buttonCustom = document.getElementById("enlarge-minimize");
        if (buttonCustom != null) {
            buttonCustom.innerHTML = "Enlarge Map";
            var popup = document.getElementById("zone-popup");
            popup.style.margin = '100px auto';
            popup.style.width = '50vw';
            popup.style.maxHeight = '75vh';
            popup.style.height = '75vh';
            var content = document.getElementById("zone-content");
            content.style.maxHeight = 'calc(100vh - 7vh - 30vh)';
            var map = document.getElementById("map");
            map.style.height = '40vh';
        }
    }
}
//init new Edit Class
window.ZoneAddEdit = new ZoneAddEdit();