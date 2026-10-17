import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class Caller {//class that handles the Add Edit screen
    //Caller Id
    Id = 0;

    constructor() {
        var dropdown = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getDropDownCaller, "", (data) => {
            if (data) {
                dropdown = data.data
            }
        });
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null) {
                // Set Locality list of values
                DropdownService.initDropdown("select-LocalityId", dropdown, "Locality");
                // If Screen mode edit, Set Dropdown Locality selected value to get by id value
                $("#select-LocalityId").val($("#select-LocalityId-id").val()).trigger('change');

                // Set District list of values
                DropdownService.initDropdown("select-DistrictId", dropdown, "District");
                // If Screen mode edit, Set Dropdown District selected value to get by id value
                $("#select-DistrictId").val($("#select-DistrictId-id").val()).trigger('change');

               
            }

            $('#select-LocalityId').on('select2:select', () => {
                $("#Locality").val($('#select-LocalityId :selected').text());
            });
            $('#select-LocalityId').on('select2:clear', () => {
                $("#Locality").val('');
            });

            $('#select-DistrictId').on('select2:select', () => {
                $("#District").val($('#select-DistrictId :selected').text());
            });
            $('#select-DistrictId').on('select2:clear', () => {
                $("#District").val('');
            });

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
        var valid = Validation.formValidation(["MobileNo"])
        //return boolean id valid or not
        return valid
    }
    //Button save ot update 
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("Caller-item-control"));
        //check if required fields are input
        if (!this.valid() || data == false) {
            //show error if required fields not input
            toastr.error("Please fill in required field(s)");
        } else {

            data['Id'] = this.Id;

            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateCaller, data, (response) => {
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
        location.href = "/Caller/List#"
        Loader.hideLoader();
    }
}
//init new Edit Class
window.Caller = new Caller();