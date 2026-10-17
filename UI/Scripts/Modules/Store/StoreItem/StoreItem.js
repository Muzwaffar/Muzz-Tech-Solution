import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
export class StoreItem {//class that handles the station Livestock screen
    //station Livestock Id
    Id = 0;
    constructor() {
        //Get All Dropdown Values
        this.getDropdowns();
        //Event Listener : Populate Species from category
        $('#select-Group').on('select2:select', () => {
            this.setPrefilledData(1)
        });
        $('#select-Category').on('select2:select', () => {
            this.setPrefilledData(2)
        });
        $('#select-Species').on('select2:select', () => {
            this.setPrefilledData(3)
        });
        $('#select-UOM').on('select2:select', () => {
            this.setUOMData()
        });
        $('#select-Group').on('select2:clear', () => {
            DropdownService.destroyReinitDropdownNull("select-Category");
            DropdownService.destroyReinitDropdownNull("select-Species");
            DropdownService.destroyReinitDropdownNull("select-Variety");
        });
        $('#select-Category').on('select2:clear', () => {
            DropdownService.destroyReinitDropdownNull("select-Species");
            DropdownService.destroyReinitDropdownNull("select-Variety");
        });
        $('#select-Species').on('select2:clear', () => {
            DropdownService.destroyReinitDropdownNull("select-Variety");
        });
        $('#select-UOM').on('select2:clear', () => {
            DropdownService.destroyReinitDropdownNull("select-UomDetails");
        });
    }

    //get all dropdown values
    getDropdowns() {
        var dropdown = null;
        ApiService.callCustom(ApiType.POST, variable.getAllDropdownValuesStoreItem, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {
                    // Set Group list of values
                    DropdownService.initDropdown("select-Group", dropdown, "Group");
                    // If Screen mode edit, Set Group selected value to get by id value
                    $("#select-Group").val($("#select-Group-id").val()).trigger('change');

                    // Set UOM list of values
                    DropdownService.initDropdown("select-UOM", dropdown, "UOM");
                    // If Screen mode edit, Set UOM selected value to get by id value
                    $("#select-UOM").val($("#select-UOM-id").val()).trigger('change');
                    
                    // Set AnimalProductType list of values
                    DropdownService.initDropdown("select-AnimalProductType", dropdown, "Type");
                    // If Screen mode edit, Set AnimalProductType selected value to get by id value
                    $("#select-AnimalProductType").val($("#select-AnimalProductType-id").val()).trigger('change');

                    // Set Parent list of values
                    DropdownService.initDropdown("select-ParentItem", dropdown, "Parent");
                    // If Screen mode edit, Set Parent selected value to get by id value
                    $("#select-ParentItem").val($("#select-ParentItem-id").val()).trigger('change');

                    // Set Species list of values
                    DropdownService.initDropdownNull("select-Category");
                    DropdownService.initDropdownNull("select-Species");
                    DropdownService.initDropdownNull("select-Variety");
                    DropdownService.initDropdownNull("select-UomDetails");
                }
                //call urlParameter funtion to check whether this page is an edit or a create
                //used to populate data 
                this.getURLparameters();
            }
        });
    }

    //set category - species and variety
    setPrefilledData(type) {
        let dat = {
            id: 0,
            type: type
        }
        if (type == 1) { //Category Selected
            dat.id = $("#select-Group").val()
        } else if (type == 2) { //Species Selected
            dat.id = $("#select-Category").val()
        } else if (type == 3) { //Species Selected
            dat.id = $("#select-Species").val()
        }
        ApiService.callCustom(ApiType.POST, variable.getStationLivestockAllDropdownDependant, dat, (data) => {
            if (data) {
                let dropdown = data.data;
                if (dropdown != null) {
                    if (type == 1) {
                        // Set Species list of values
                        DropdownService.destroyReinitDropdown("select-Category", dropdown, "Category");
                        // If Screen mode edit, Set Species selected value to get by id value
                        $("#select-Category").val($("#select-Category-id").val()).trigger('change');
                    }else if (type == 2) {
                        // Set Category list of values
                        DropdownService.destroyReinitDropdown("select-Species", dropdown, "Species");
                        // If Screen mode edit, Set Category selected value to get by id value
                        $("#select-Species").val($("#select-Species-id").val()).trigger('change');
                    } else if (type == 3) {
                        // Set Species list of values
                        DropdownService.destroyReinitDropdown("select-Variety", dropdown, "Variety");
                        // If Screen mode edit, Set Species selected value to get by id value
                        $("#select-Variety").val($("#select-Variety-id").val()).trigger('change');
                    }

                }
            }
        });
       
    }

    //set uom details
    setUOMData() {
        let dat = $("#select-UOM").val();
            ApiService.callCustom(ApiType.POST, variable.getAllDropdownValuesUOMStoreItem, dat, (data) => {
            if (data) {
                let dropdown = data.data;
                if (dropdown != null) {
                    // Set Species list of values
                    DropdownService.destroyReinitDropdown("select-UomDetails", dropdown, "UOMDetails");
                    // If Screen mode edit, Set Species selected value to get by id value
                    $("#select-UomDetails").val($("#select-UomDetails-id").val()).trigger('change');
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
            this.setPrefilledData(1)
            setTimeout(e => {
                this.setPrefilledData(2);
                this.setUOMData();
                setTimeout(e => {
                    this.setPrefilledData(3)
                }, 500)
            },500)
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
        var valid = Validation.formValidation(["select-Group", "select-Category", "select-UomDetails", "select-AnimalProductType", "IsConsumable"])
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
        let data = DefaultService.result(document.getElementById("store-item-control"));
        //check validation and data 
        if (this.valid() && data != false) {
            data['Id'] = this.Id;
            // call api to save 
            ApiService.callCustom(ApiType.POST, variable.saveAsyncStoreItem, data, (response) => {
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
        location.href = "/StoreItem/List#"
        Loader.hideLoader();
    }
}
window.StoreItem = new StoreItem();