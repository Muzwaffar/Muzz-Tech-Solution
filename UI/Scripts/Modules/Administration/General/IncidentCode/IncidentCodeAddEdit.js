import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";

export class IncidentCodeAddEdit {//class that handles the Add Edit screen
    constructor() {
        var dropdown = null;
        
        ApiService.callCustom(ApiType.POST, variable.getDropDownIncidentSubType, "", (data) => {
            if (data) {
                dropdown = data.data;
            }
        })

        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null || dropdown != undefined) {
                //Make Select
                DropdownService.initDropdown("select-incidentsubtype", dropdown, "IncidentSubType");

                //Set Dropdown Value on edit 
                $("#select-incidentsubtype").val($("#select-incidentsubtype-id").val()).trigger('change');
                if (!Validation.isNullOrUndefined($("#select-incidentsubtype").val())) {
                    this.GetIncidentType();
                }
            }

            $('#select-incidentsubtype').on('select2:select', () => {
                this.GetIncidentType();
            });
            $('#select-incidentsubtype').on('select2:clear', () => {
                this.GetIncidentType();
            });
           
        });
    }

    GetIncidentType() {

        let dat = $("#select-incidentsubtype").val();
        if (dat != "") {
            //new httpRequest
            var request = new XMLHttpRequest();
            //httpRequest : open a post request , set url, is async is set to true
            request.open("POST", variable.getIncidentType, true);
            //header is sent in json format
            request.setRequestHeader("Content-Type", "application/json");
            //response
            request.onreadystatechange = function () {
                if (request.readyState == 4) {
                    if (request.status == 200) {
                        var jsonString = request.responseText;
                        var response = JSON.parse(jsonString);
                        if (response) {
                            $("#IncidentType").val(response.data.name);
                        }
                    }
                }
            };
            //set data to send
            var data = JSON.stringify({ "data": dat });
            //send httpRequest
            request.send(data);
        }
        else {
            $("#IncidentType").val("");
        }
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["Name","select-incidentsubtype"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        //return boolean id valid or not
        return valid
    }

    //Button save ot update
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("IncidentCode-AddEdit-Panel"));
        
        //check if required fields are input
        if (this.valid() && data != false)
        {
           
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateIncidentCode, data, (response) => {
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
window.IncidentCodeAddEdit = new IncidentCodeAddEdit();