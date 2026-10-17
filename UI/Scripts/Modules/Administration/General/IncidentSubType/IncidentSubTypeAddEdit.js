import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";

export class IncidentSubTypeAddEdit {//class that handles the Add Edit screen
    constructor() {
        var dropdown = null;
        
        ApiService.callCustom(ApiType.POST, variable.getDropDownIncidentType, "", (data) => {
            if (data) {
                dropdown = data.data;
            }
        })
        
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null || dropdown != undefined) {
                //Make Select
                DropdownService.initDropdown("select-incidenttype", dropdown, "IncidentType");

                //Set Dropdown Value on edit 
                $("#select-incidenttype").val($("#select-incidenttype-id").val()).trigger('change');
            }
        });
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["Name","select-incidenttype"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        //return boolean id valid or not
        return valid
    }

    //Button save ot update
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("IncidentSubType-AddEdit-Panel"));
        
        //check if required fields are input
        if (this.valid() && data != false)
        {
           
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateIncidentSubType, data, (response) => {
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
window.IncidentSubTypeAddEdit = new IncidentSubTypeAddEdit();