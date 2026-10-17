import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";

export class RankAddEdit {//class that handles the Add Edit screen
    constructor() {
        var dropdown = null;
        var userMatrix = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getResourceCategoryDropDown, "", (data) => {
            if (data) {
                dropdown = data.data
            }
        });
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null) {

                // Set resourceCategory list of values
                DropdownService.initDropdown("select-resourceCategory", dropdown, "ResourceCategory");
                // If Screen mode edit, Set Dropdown resourceCategory checked value to get by id value
                $("#select-resourceCategory").val($("#select-resourceCategory-id").val()).trigger('change');
            }

        });
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["Name", "select-resourceCategory"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        //return boolean id valid or not
        return valid
    }

    //Button save ot update
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("Rank-AddEdit-Panel"));
        //check if required fields are input
        if (this.valid() && data != false)
        {

            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateRank, data, (response) => {
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
window.RankAddEdit = new RankAddEdit();