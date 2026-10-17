import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class DivisionAddEdit {//class that handles the Add Edit screen
    constructor() {

        var dropdown = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getDeptDropdown, "", (data) => {
            if (data) {
                dropdown = data.data
            }
        });
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null) {
                if ($("#select-department-D").val() == "") {
                    // Set researchType list of values
                    DropdownService.initDropdown("select-department-D", dropdown, "Department");
                    // If Screen mode edit, Set Dropdown researchType selected value to get by id value
                    $("#select-department-D").val($("#select-department-D-id").val()).trigger('change');
                }
            }
        });
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["Name", "select-department-D"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        //return boolean id valid or not
        return valid
    }

    //Button save or update 
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("Division-AddEdit-Panel"));
        //check if required fields are input
        if (this.valid() && data != false) {
            data["Department"] = DefaultService.resultObject("Department", document.getElementById("Department-AddEdit-Panel"));
            if (!Validation.checkDropdownSelected(data["Department"].Id)) {
                toastr.error("Please Fill In Mandatory Fields")
                return;
            }
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateDivision, data, (response) => {
                //Response true
                if (response.data) {
                    toastr.success("Saved Successfully!")
                    if (window.location.href.includes("Division/List")) {
                        window.location.reload();
                    } else {
                        $('.division').trigger('change');
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
window.DivisionAddEdit = new DivisionAddEdit();