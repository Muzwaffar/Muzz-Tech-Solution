import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class LookupValueAddEdit {//class that handles the Add Edit screen
    dropdown = [];
    constructor() {
        var dropdown = null;
        ApiService.callCustom(ApiType.POST, variable.getDropDownLKvalue, "", (data) => {
            if (data) {
                this.dropdown = data.data;
            }
        })
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (this.dropdown.length > 0) {

                //Title Select
                DropdownService.initDropdown("select-lookuptype", this.dropdown, "LookUpType");
                //Set Dropdown Value on edit 
                $("#select-lookuptype").val($("#select-lookuptype-id").val()).trigger('change');
                if ($("#Id").val() == 0) {
                    //set Preselected data
                    this.getURLparameters();
                }
               
            }

        });
    }

    //funtion to check whether this page is an edit or a create
    getURLparameters() {
        const queryString = window.location.search;
        //url Parameters
        const urlParams = new URLSearchParams(queryString);
        // if url contains id, switch to edit mode
        if (urlParams.has('Name')) {
            //Get inherited data when loading the page
            var name = urlParams.get('Name');
            this.setPreselectedData(name)
        };
    }

    //get Preselected Data
    setPreselectedData(name) {
        var list = this.dropdown.find(x => x.title == "LookUpType")
        if (list != null) {
            var option = list.items.find(y => y.text.replaceAll(" ", "") == name);
            if (option != null) {
                //Set Dropdown Value on edit 
                $("#select-lookuptype").val(option.id).trigger('change');
            }
        }
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["Name", "select-lookuptype"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        //return boolean id valid or not
        return valid
    }

    //Button save ot update 
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("LookupValue-AddEdit-Panel"));
       
        //check if required fields are input
        if (this.valid() && data != false) {
            data["LookUpType"] = DefaultService.resultObject("LookUpType", document.getElementById("LookupValue-AddEdit-Panel"));
            if (!Validation.checkDropdownSelected(data["LookUpType"].Id)) {
                toastr.error("Please Fill In Mandatory Fields")
                return;
            }
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateLKvalue, data, (response) => {
                //Response true
                if (response.data) {
                    toastr.success("Saved Successfully!")
                    if (window.location.href.includes("LookUpValue/List")) {
                        window.location.reload();
                    } else {
                        $('.lookup-value').trigger('change');
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
window.LookupValueAddEdit = new LookupValueAddEdit();