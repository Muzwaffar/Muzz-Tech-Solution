import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { MapService } from "/Scripts/Common/Map/MapService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";

export class ShiftAddEdit {//class that handles the Add Edit screen
    constructor() {
        var dropdown = null;

        //ApiService.callCustom(ApiType.POST, variable.getDropDownZone, "", (data) => {
        //    if (data) {
        //        dropdown = data.data;
        //    }
        //})

        //Field after ajax call 
        $(document).ajaxComplete(() => {
            //if (dropdown != null || dropdown != undefined) {
            //    //Make Select
            //    DropdownService.initDropdown("select-zonetype", dropdown, "ZoneType");

            //    //Set Dropdown Value on edit 
            //    $("#select-zonetype").val($("#select-zonetype-id").val()).trigger('change');
            //}

            //MapService.initMap();
            //setTimeout(e => {
            //    this.minimizeMap();
            //}, 100)
        });
    }

    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["Name","TimeFrom","TimeTo"])
  
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }
        //return boolean id valid or not
        return valid
    }

    //Button save ot update
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("Shift-AddEdit-Panel"));

        //check if required fields are input
        if (!this.valid() && data == false) {
            //show error if required fields not input
            toastr.error("Please Fill In Mandatory Fields")
        } else {
            data["TimeFrom"] = moment(data["TimeFrom"], 'hh:mm').add(4, 'hours');
            data["TimeTo"] = moment(data["TimeTo"], 'hh:mm').add(4, 'hours');
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateShift, data, (response) => {
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

}
//init new Edit Class
window.ShiftAddEdit = new ShiftAddEdit();