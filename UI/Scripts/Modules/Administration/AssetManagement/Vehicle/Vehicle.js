import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";

export class Vehicle {//class that handles the Vehicle screen
    //Vehicle Id
    Id = 0;

    constructor() {
        //Get All Dropdown Values
        this.getDropdowns();

        $('#select-Make').on('select2:select', () => {
            this.setModelData()
        });
    }

    //get all dropdown values
    getDropdowns() {
        var dropdown = null;
        ApiService.callCustom(ApiType.POST, variable.getVehicleDropdown, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {

                    // Set Station list of values
                    DropdownService.initDropdown("select-Station", dropdown, "Station");
                    // If Screen mode edit, Set Station selected value to get by id value
                    $("#select-Station").val($("#select-Station-id").val()).trigger('change');

                    // Set Category list of values
                    DropdownService.initDropdown("select-Category", dropdown, "Vehicle Category");
                    // If Screen mode edit, Set Category selected value to get by id value
                    $("#select-Category").val($("#select-Category-id").val()).trigger('change');


                    // Set Asset Abbreviation list of values
                    DropdownService.initDropdown("select-Abbreviation", dropdown, "Vehicle Abbreviation");
                    // If Screen mode edit, Set Asset Abbreviation selected value to get by id value
                    $("#select-Abbreviation").val($("#select-Abbreviation-id").val()).trigger('change');


                    // Set Parent Vehicle list of values
                    DropdownService.initDropdown("select-ParentVehicle", dropdown, "Parent Vehicle");
                    // If Screen mode edit, Set Parent Vehicle selected value to get by id value
                    $("#select-ParentVehicle").val($("#select-ParentVehicle-id").val()).trigger('change');

                    // Set Make list of values
                    DropdownService.initDropdown("select-Make", dropdown, "Make");
                    // If Screen mode edit, Set Make selected value to get by id value
                    $("#select-Make").val($("#select-Make-id").val()).trigger('change');

                    // Set Model list of values
                    DropdownService.initDropdown("select-Model", dropdown, "Model");
                    // If Screen mode edit, Set Model selected value to get by id value
                    $("#select-Model").val($("#select-Model-id").val()).trigger('change');
                }
                //call urlParameter funtion to check whether this page is an edit or a create
                //used to populate data 
                this.getURLparameters();
            }
        });
    }

    //set uom details
    setModelData() {
        let dat = $("#select-Make").val();
        ApiService.callCustom(ApiType.POST, variable.getAllDropdownValuesModelV, dat, (data) => {
            if (data) {
                let dropdown = data.data;
                if (dropdown != null) {
                    // Set Species list of values
                    DropdownService.destroyReinitDropdown("select-Model", dropdown, "Model");
                    // If Screen mode edit, Set Species selected value to get by id value
                    $("#select-Model").val($("#select-Model-id").val()).trigger('change');
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
        var valid = Validation.formValidation(["RegistrationNo", "select-Station", "select-Category", "select-Abbreviation", "select-Make", "select-Model"])
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
        let data = DefaultService.result(document.getElementById("Vehicle-item-control"));
        //check validation and data 
        if (this.valid() && data != false) {

            data['Id'] = this.Id;


            if (data["ManufacturingDate"] == "") {
                data["ManufacturingDate"] = null;
            }

            // call api to save 
            ApiService.callCustom(ApiType.POST, variable.createUpdateVehicle, data, (response) => {
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
        location.href = "/Vehicle/List#"
        Loader.hideLoader();
    }
}
window.Vehicle = new Vehicle();