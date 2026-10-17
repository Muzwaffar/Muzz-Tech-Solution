import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class NAAAddEdit {//class that handles the Add Edit screen
    constructor() {
        var dropdown = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getNAADropdown, "", (data) => {
            if (data) {
                dropdown = data.data
            }
        });
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null) {
                // Set title list of values
                DropdownService.initDropdown("select-title", dropdown, "Title");
                // If Screen mode edit, Set Dropdown title selected value to get by id value
                $("#select-title").val($("#select-title-id").val()).trigger('change');

                // Set gender list of values
                DropdownService.initDropdown("select-gender", dropdown, "Gender");
                // If Screen mode edit, Set Dropdown gender selected value to get by id value
                $("#select-gender").val($("#select-gender-id").val()).trigger('change');
            }
        });
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["select-title", "LastName", "Identifier"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        //return boolean id valid or not
        return valid
    }

    //Button save ot update 
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("NAA-AddEdit-Panel"));
        //check if required fields are input
        if (this.valid() && data != false) {
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateNAA, data, (response) => {
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
window.NAAAddEdit = new NAAAddEdit();