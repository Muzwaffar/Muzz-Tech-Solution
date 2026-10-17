import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class StationAddEdit {//class that handles the Add Edit screen
    constructor() {

        var dropdown = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getStationDropdown, "", (data) => {
            if (data) {
                dropdown = data.data
            }
        });
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null) {
                // Set StationType list of values
                DropdownService.initDropdown("select-StationType", dropdown, "StationType");
                // If Screen mode edit, Set Dropdown StationType selected value to get by id value
                $("#select-StationType").val($("#select-StationType-id").val()).trigger('change');

                // Set StationType list of values
                DropdownService.initDropdown("select-StationLocation", dropdown, "Zone");
                // If Screen mode edit, Set Dropdown StationType selected value to get by id value
                $("#select-StationLocation").val($("#select-StationLocation-id").val()).trigger('change');
            }
        });
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["Name", "StationCallSign","PhoneNumber", "select-StationType","select-StationLocation"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        //return boolean id valid or not
        return valid
    }

    //Button save or update 
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("Station-AddEdit-Panel"));
        //check if required fields are input
        if (this.valid() && data != false) {
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateStation, data, (response) => {
                //Response true
                if (response.data) {
                    toastr.success("Saved Successfully!")
                    if (window.location.href.includes("Station/List")) {
                        window.location.reload();
                    } else {
                        $('.station').trigger('change');
                        window.location.href = "#";
                    }
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
                Loader.hideLoader();
            })
        }
    }
}
//init new Edit Class
window.StationAddEdit = new StationAddEdit();