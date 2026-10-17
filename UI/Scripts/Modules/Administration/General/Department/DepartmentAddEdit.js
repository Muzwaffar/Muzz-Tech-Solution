import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
export class DepartmentAddEdit {//class that handles the Add Edit screen
    constructor() {
        //Field after ajax call 
        $(document).ajaxComplete(() => {

        });
    }
    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["Name"])
        //return boolean id valid or not
        return valid
    }
    //Button save ot update 
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("Department-AddEdit-Panel"));
        //check if required fields are input
        if (!this.valid() || data == false) {
            //show error in case of error
            toastr.error("Please fill in required field(s)");
        } else {
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateDepartment, data, (response) => {
                //Response true
                if (response.data) {
                    toastr.success("Saved Successfully!")
                    if (window.location.href.includes("Department/List")) {
                        window.location.reload();
                    } else {
                        $('.department').trigger('change');
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
window.DepartmentAddEdit = new DepartmentAddEdit(); 