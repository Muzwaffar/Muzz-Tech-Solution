import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class EquipmentMakeAddEdit {//class that handles the Add Edit screen
    constructor() {

    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["Name"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        //return boolean id valid or not
        return valid
    }

    //Button save ot update 
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("EquipmentMake-AddEdit-Panel"));
        //check if required fields are input
        if (this.valid() && data != false) {
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateEquipmentMake, data, (response) => {
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
window.EquipmentMakeAddEdit = new EquipmentMakeAddEdit();