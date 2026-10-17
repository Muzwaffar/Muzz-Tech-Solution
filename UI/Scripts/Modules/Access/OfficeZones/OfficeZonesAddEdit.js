import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class OfficeZonesAddEdit {//class that handles the Add Edit screen
    constructor() {
        var dropdown = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getOfficeZonesDropdown, "", (data) => {
            if (data) {
                dropdown = data.data
            }
        });
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null) {
                // Set department list of values
                DropdownService.initDropdown("select-department", dropdown, "Department");
                // If Screen mode edit, Set Dropdown department selected value to get by id value
                $("#select-department").val($("#select-department-id").val()).trigger('change');

                // Set locality list of values
                DropdownService.initDropdown("select-locality", dropdown, "Locality");
                // If Screen mode edit, Set Dropdown locality selected value to get by id value
                $("#select-locality").val($("#select-locality-id").val()).trigger('change');

                // Set zone list of values
                DropdownService.initDropdown("select-zone", dropdown, "Zone");
                // If Screen mode edit, Set Dropdown zone selected value to get by id value
                $("#select-zone").val($("#select-zone-id").val()).trigger('change');
            }
        });
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["OfficeName", "select-department", "select-locality", "select-zone", "Abbreviation"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        //return boolean id valid or not
        return valid
    }

    //Button save ot update 
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("OfficeZones-AddEdit-Panel"));
        //check if required fields are input
        if (this.valid() && data != false) {
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateOfficeZones, data, (response) => {
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
                Loader.hideLoader();
            })
        }
    }
}
//init new Edit Class
window.OfficeZonesAddEdit = new OfficeZonesAddEdit();