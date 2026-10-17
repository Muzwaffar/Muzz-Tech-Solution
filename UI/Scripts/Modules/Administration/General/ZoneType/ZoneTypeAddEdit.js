import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";

export class ZoneTypeAddEdit {//class that handles the Add Edit screen
    constructor() {
    }

    //Button save ot update
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("ZoneType-AddEdit-Panel"));
        //check if required fields are input
        if (data == false) {
            //show error if required fields not input
            toastr.error("Please Fill In Mandatory Fields")
        } else {
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateZoneType, data, (response) => {
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
window.ZoneTypeAddEdit = new ZoneTypeAddEdit();