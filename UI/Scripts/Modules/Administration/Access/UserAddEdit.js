import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class UserAddEdit {//class that handles the Add Edit screen
    constructor() {
        var dropdown = null;
        var userMatrix = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getUserDropDown, "", (data) => {
            if (data) {
                dropdown = data.data
            }
        });
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null) {
                // Set title list of values
                DropdownService.initDropdown("select-title", dropdown, "Title");
                // If Screen mode edit, Set Dropdown title checked value to get by id value
                $("#select-title").val($("#select-title-id").val()).trigger('change');

                // Set roles list of values
                DropdownService.initDropdown("select-roles", dropdown, "Roles");
                // If Screen mode edit, Set Dropdown roles checked value to get by id value
                $("#select-roles").val($("#select-roles-id").val()).trigger('change');

                // Set resourceType list of values
                DropdownService.initDropdown("select-resourceType", dropdown, "ResourceType");
                // If Screen mode edit, Set Dropdown resourceType checked value to get by id value
                $("#select-resourceType").val($("#select-resourceType-id").val()).trigger('change');

                // Set postHeld list of values
                DropdownService.initDropdown("select-postHeld", dropdown, "PostHeld");
                // If Screen mode edit, Set Dropdown postHeld checked value to get by id value
                $("#select-postHeld").val($("#select-postHeld-id").val()).trigger('change');

                // Set office list of values
                DropdownService.initDropdown("select-office", dropdown, "Office");
                // If Screen mode edit, Set Dropdown office checked value to get by id value
                $("#select-office").val($("#select-office-id").val()).trigger('change');

                // Set User Type list of values
                DropdownService.initDropdown("select-UserType", dropdown, "UserType");
                // If Screen mode edit, Set Dropdown User Type checked value to get by id value
                $("#select-UserType").val($("#select-UserType-id").val()).trigger('change');
            }

            //Tree View for user matrix
            $(document).ready(() => {
               
            });
        });

       
    }
    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["firstName", "lastName", "email", "select-roles", "select-resourceType", "select-title", "select-postHeld"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }
        //return boolean id valid or not
        return valid
    }
    //Button save ot update 
    saveUpdate() {
        //get tree selected data
        var treeData = $('#tree').jstree(true).get_json('#', { flat: true }).filter(x => x.state.checked == true)
        //get tree selected ids
        var selectedIds = []
        if (treeData.length > 0) {
            selectedIds = treeData.map(function (datum) {
                return parseInt(datum['id']);
            });
        }
        // get all values
        let data = DefaultService.result(document.getElementById("User-AddEdit-Panel"));
        //check if required fields are input
        if (this.valid() && data != false) {
            data["Roles"] = $("#select-roles").val();
            data["UserMatrix"] = selectedIds;
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateUser, data, (response) => {
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
window.UserAddEdit = new UserAddEdit();