import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
export class Store {//class that handles the station Livestock screen
    //station Livestock Id
    Id = 0;
    constructor() {
        //Get All Dropdown Values
        this.getDropdowns();

    }

    //get all dropdown values
    getDropdowns() {
        var dropdown = null;
        ApiService.callCustom(ApiType.POST, variable.getAllDropdownValuesStore, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {
                    // Set Category list of values
                    DropdownService.initDropdown("select-StationId", dropdown, "Station");
                    // If Screen mode edit, Set Category selected value to get by id value
                    $("#select-StationId").val($("#select-StationId-id").val()).trigger('change');
                    
                }
                //call urlParameter funtion to check whether this page is an edit or a create
                //used to populate data 
                this.getURLparameters();
            }
        });
    }

    //funtion to check whether this page is an edit or a create
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
        var valid = Validation.formValidation(["select-StationId", "Name"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }
        valid = valid;

        //return boolean id valid or not
        return valid
    }
    

    // save action button
    save() {
        //Show loader 
        Loader.showLoader("Saving Data...");
        // Set data to send to api
        let data = DefaultService.result(document.getElementById("store-control"));
        //check validation and data 
        if (this.valid() && data != false) {

            data['Id'] = this.Id;
            // call api to save 
            ApiService.callCustom(ApiType.POST, variable.saveAsyncStore, data, (response) => {
                // if response true: Save successfull
                if (response.data) {
                    toastr.success("Saved Successfully")
                    this.back();
                }
                // if response false: Error
                else {
                    toastr.error("An Error Occurred. Please contact Admin")
                }
            })
        }
        //hide loader
        Loader.hideLoader();
    }

    //Back button
    back() {
        //show loader when refirecting to listing
        Loader.showLoader("Redirecting To Listing...");
        location.href = "/Store/List#"
        Loader.hideLoader();
    }
}
window.Store = new Store();