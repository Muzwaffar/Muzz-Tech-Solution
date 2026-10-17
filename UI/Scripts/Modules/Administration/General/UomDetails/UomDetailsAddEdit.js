import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class UomDetailsAddEdit {//class that handles the Add Edit screen
    constructor() {
        var dropdown = null;
        ApiService.callCustom(ApiType.POST, variable.getDropDownUomDetails, "", (data) => {
            if (data) {
                dropdown = data.data;
            }
        })
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null) {
                //Title Select
                DropdownService.initDropdown("select-uomheader", dropdown, "UomHeader");
                //Set Dropdown Value on edit 
                $("#select-uomheader").val($("#select-uomheader-id").val()).trigger('change');
            }

        });
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["detailsName", "select-uomheader", "detailsUnit", "detailsRF"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        //return boolean id valid or not
        return valid
    }

    //Button save ot update 
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("UomDetails-AddEdit-Panel"));
       
        //check if required fields are input
        if (this.valid() && data != false) {
            data["UomHeader"] = DefaultService.resultObject("UomHeader", document.getElementById("UomDetails-AddEdit-Panel"));
            if (!Validation.checkDropdownSelected(data["UomHeader"].Id)) {
                toastr.error("Please Fill In Mandatory Fields")
                return;
            }
            if (data.Id == "") {
                data.Id = 0;
            }
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateUomDetailse, data, (response) => {
                //Response true
                if (response.data) {
                    toastr.success("Saved Successfully!")
                    if (window.location.href.includes("UomDetails/List")) {
                        window.location.reload();
                    } else {
                        $('.uomDetails').trigger('change');
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
window.UomDetailsAddEdit = new UomDetailsAddEdit();