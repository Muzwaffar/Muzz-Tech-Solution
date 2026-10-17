import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class FormModuleMappingSequenceAddEdit {//class that handles the Add Edit screen
    constructor() {
        var dropdown = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getFormModuleMappingSequenceDropdown, "", (data) => {
            if (data) {
                dropdown = data.data
            }
        });
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null) {
                // Set module list of values
                DropdownService.initDropdown("select-module", dropdown, "Module");
                // If Screen mode edit, Set Dropdown module selected value to get by id value
                $("#select-module").val($("#select-module-id").val()).trigger('change');
                // Set form list of values
                DropdownService.initDropdown("select-form", dropdown, "Form");
                // If Screen mode edit, Set Dropdown form selected value to get by id value
                $("#select-form").val($("#select-form-id").val()).trigger('change');
            }
        });
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["sequence", "select-module", "select-form"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        //return boolean id valid or not
        return valid
    }

    //Button save ot update 
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("FormModuleMappingSequence-AddEdit-Panel"));
        //check if required fields are input
        if (this.valid() && data != false) {
            data['Id'] = parseInt(data.Id);
            data['Module'] = $("#select-module").val();
            data['Sequenece'] = parseInt(data.Sequence);
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateFormModuleMappingSequence, data, (response) => {
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
window.FormModuleMappingSequenceAddEdit = new FormModuleMappingSequenceAddEdit();