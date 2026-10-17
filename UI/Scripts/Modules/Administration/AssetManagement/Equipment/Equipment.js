import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { AssignmentType } from "/Scripts/Common/Enums.js";
import { xhrService } from "/Scripts/Common/xhrService.js";
export class Equipment {//class that handles the station Equipment screen
    //Equipment Id
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
        ApiService.callCustom(ApiType.POST, variable.getAllDropdownValuesEquipment, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {
                    // Set Category list of values
                    DropdownService.initDropdown("select-Category", dropdown, "Equipment Category");
                    // If Screen mode edit, Set Group selected value to get by id value
                    $("#select-Category").val($("#select-Category-id").val()).trigger('change');

                    // Set Abbreviation list of values
                    DropdownService.initDropdown("select-Abbreviation", dropdown, "Equipment Abbreviation");
                    // If Screen mode edit, Set Abbreviation selected value to get by id value
                    $("#select-Abbreviation").val($("#select-Abbreviation-id").val()).trigger('change');
                    
                    // Set Make list of values
                    DropdownService.initDropdown("select-Make", dropdown, "Make");
                    // If Screen mode edit, Set make selected value to get by id value
                    $("#select-Make").val($("#select-Make-id").val()).trigger('change');

                    // Set Model list of values
                    DropdownService.initDropdown("select-Model", dropdown, "Model");
                    // If Screen mode edit, Set Model selected value to get by id value
                    $("#select-Model").val($("#select-Model-id").val()).trigger('change');

                    // Set ParentEquipment list of values
                    DropdownService.initDropdown("select-ParentEquipment", dropdown, "Parent Equipment");
                    // If Screen mode edit, Set ParentEquipment selected value to get by id value
                    $("#select-ParentEquipment").val($("#select-ParentEquipment-id").val()).trigger('change');

                    DropdownService.initDropdownNull("select-AssignmentId");

                    // Set Species list of values
                    DropdownService.initDropdownNull("select-Category");
                    DropdownService.initDropdownNull("select-Abbreviation");
                    DropdownService.initDropdownNull("select-Make");
                    DropdownService.initDropdownNull("select-Model");
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
        ApiService.callCustom(ApiType.POST, variable.getAllDropdownValuesModelE, dat, (data) => {
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
        this.setAssignmentType();
    }

    setAssignmentType() {
        let dat = $("#AssignmentType").val();
        if (dat != "" && dat != undefined) {
            let datum = AssignmentType[dat];
            $('input:radio[name="AssignmentType"]').filter('[value="' + datum + '"]').attr('checked', true);
            this.getDropdownsByAssignementType(datum)
        }

    }

    //get all dropdown values by assignement Type
    getDropdownsByAssignementType(type) {
        var datum = { "data": type }
        xhrService.xhrRequestCustom(ApiType.POST, variable.EquipmentGetAllDropdownValuesDependent, datum).then(data => {
            if (data) {
                var dropdown = data.data;
                if (dropdown != null) {
                    DropdownService.destroyReinitDropdown("select-AssignmentId", dropdown, "Dropdown");
                    $("#select-AssignmentId").val($("#select-AssignmentId-id").val()).trigger('change');
                }
            }
        })
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["select-Category", "select-Abbreviation", "select-Make", "select-Model", "Name"])
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
        let data = DefaultService.result(document.getElementById("equipment-control"));
        //check validation and data 
        if (this.valid() && data != false) {
            
            data['Id'] = this.Id;

            if (data["ManufacturingDate"] == "") {
                data["ManufacturingDate"] = null;
            }
            if ($('input[type=radio]:checked').length > 0) {
                data["AssignmentType"] = parseInt($("input[name='AssignmentType']:checked").val());
            }
             //call api to save 
            ApiService.callCustom(ApiType.POST, variable.saveAsyncEquipment, data, (response) => {
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
        location.href = "/Equipment/List#"
        Loader.hideLoader();
    }
}
window.Equipment = new Equipment();