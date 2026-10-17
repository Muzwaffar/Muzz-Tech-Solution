import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class ResourceAddEdit {//class that handles the Add Edit screen
    //Resource Id
    Id = 0;

    constructor() {
        var dropdown = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getResourceDropdown, "", (data) => {
            if (data) {
                dropdown = data.data
            }
        });
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null) {
                // Set title list of values
                DropdownService.initDropdown("select-title", dropdown, "Title");
                // If Screen mode edit, Set Dropdown title selected value to get by id value
                $("#select-title").val($("#select-title-id").val()).trigger('change');

                // Set gender list of values
                DropdownService.initDropdown("select-gender", dropdown, "Gender");
                // If Screen mode edit, Set Dropdown gender selected value to get by id value
                $("#select-gender").val($("#select-gender-id").val()).trigger('change');

                // Set user list of values
                DropdownService.initDropdown("select-user", dropdown, "User");
                // If Screen mode edit, Set Dropdown user selected value to get by id value
                $("#select-user").val($("#select-user-id").val()).trigger('change');

                // Set ResourceType list of values
                DropdownService.initDropdown("select-resourceType", dropdown, "ResearchType");
                // If Screen mode edit, Set Dropdown user selected value to get by id value
                $("#select-resourceType").val($("#select-resourceType-id").val()).trigger('change');

                // Set License Category list of values
                DropdownService.initDropdown("select-PrivateLicenseDriverCategory", dropdown, "License Category");
                // If Screen mode edit, Set Dropdown user selected value to get by id value
                $("#select-PrivateLicenseDriverCategory").val($("#select-PrivateLicenseDriverCategory-id").val()).trigger('change');

                // Set ResourceType list of values
                DropdownService.initDropdown("select-Rank", dropdown, "Rank");
                // If Screen mode edit, Set Dropdown user selected value to get by id value
                $("#select-Rank").val($("#select-Rank-id").val()).trigger('change');

                // Set ResourceType list of values
                DropdownService.initDropdown("select-StationUnitName", dropdown, "Station");
                // If Screen mode edit, Set Dropdown user selected value to get by id value
                $("#select-StationUnitName").val($("#select-StationUnitName-id").val()).trigger('change');
            }
            //call urlParameter funtion to check whether this page is an edit or a create
            //used to populate data 
            this.getURLparameters();
        });
    }

    getURLparameters() {
        const queryString = window.location.search;
        //url Parameters
        const urlParams = new URLSearchParams(queryString);
        // if url contains id, switch to edit mode
        if (urlParams.has('Id')) {
            //get Id from URL
            this.Id = parseInt(urlParams.get('Id'))
        };
        if (urlParams.has('View')) {
            if (parseInt(urlParams.get('View'))) {
                $(".noView").each(function (i, obj) {
                    obj.classList.add("d-none")
                });
            }
        };
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["select-resourceType", "NICPassport", "select-title", "FirstName",
             "LastName", "MobileNo", "select-Rank", "DatejoinedMFRS", "PFNumber", "select-StationUnitName"])
        //return boolean id valid or not
        return valid
    }
    //Button save ot update 
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("Resource-item-control"));
        //check if required fields are input
        if (!this.valid() || data == false) {
            //show error if required fields not input
            toastr.error("Please fill in required field(s)");
        } else {

            data['Id'] = this.Id;

            if (data["NAAId"] == "") {
                data["NAAId"] = 0;
            }
            if (data["PrivateLicenseIssueDate"] == "") {
                data["PrivateLicenseIssueDate"] = null;
            }

            if (data["PrivateLicenseExpiryDate"] == "") {
                data["PrivateLicenseExpiryDate"] = null;
            }

            if (data["DOB"] == "") {
                data["DOB"] = null;
            }

            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateResource, data, (response) => {
                //Response true
                if (response.data) {
                    toastr.success("Saved Successfully!")
                    this.back();
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

    //Back button
    back() {
        //show loader when refirecting to listing
        Loader.showLoader("Redirecting To Listing...");
        location.href = "/Resource/List#"
        Loader.hideLoader();
    }
}
//init new Edit Class
window.ResourceAddEdit = new ResourceAddEdit();