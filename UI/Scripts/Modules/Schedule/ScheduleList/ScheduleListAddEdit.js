import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class ScheduleListAddEdit {//class that handles the Add Edit screen
    //global variables
    ResourceRelation = [];
    ResourceList = [];
    qtyAsset = 0;
    jsonData = null;
    Id;

    constructor() {
        var dropdown = null;
        var dropdownDate = null;
        $('#select-form').on('select2:select', () => {
            this.getItemList();
        });
        //$('#select-form').on('select2:clear', () => {
        //    this.getItemList();
        //});
        //Field after ajax call 
        this.getDropdowns();
        //init datatable
        var colResource = [ //Resource Column List
            { data: 'resourceId', title: 'Resource Id', visible: false },
            { data: 'name', title: 'Resource Name' }
        ];
        //Init Access Rights Datatable
        DatatableService.initializeDataGridServices("relation-Resource-Table", colResource, this.ResourceRelation, true, "ScheduleListAddEdit.deleteResourceTable");


    }
    //Get All Dropdown Values Fucntion
    getDropdowns() {
        var dropdown = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getDropdownsSL, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {
                    //init  dropdown 
                    //vehicle Select
                    DropdownService.initMultiDropdownNull("multiselect-vehicle");
                    //team Select
                    DropdownService.initDropdownNull("select-team")
                    //asset Select
                    DropdownService.initDropdownNull("select-Asset");
                    //zone Select
                    DropdownService.initDropdown("select-form", dropdown, "Form");
                    //Set Dropdown Value on edit 
                    $("#select-form").val($("#select-form-id").val()).trigger('change');
                    //item select
                    DropdownService.initDropdownNull("select-item");
                    //service Select
                    DropdownService.initMultiDropdownNull("multiselect-service")
                    //$("#multiselect-service").val(selectedServices).trigger('change');
                    //Resource Select
                    DropdownService.initDropdown("select-Resource", dropdown, "Resource")
                    //zone Select
                    DropdownService.initDropdown("select-zone", dropdown, "Zone");
                    //Set Dropdown Value on edit 
                    $("#select-zone").val($("#select-zone-id").val()).trigger('change');

                    DropdownService.initDropdown("select-items", dropdown, "FormPage");
                    //Set items Dropdown Value on edit 
                    $("#select-items").val($("#select-items-id").val()).trigger('change');
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
            //Get inherited data when loading the page
            this.setDataInTable()
        };
        if (urlParams.has('View')) {
            if (parseInt(urlParams.get('View'))) {
                $(".noView").each(function (i, obj) {
                    obj.classList.add("d-none")
                });
            }
        };
    }


    // Set Data in table on edit 
    setDataInTable() {
        this.getItemList();
        // call api to get data saved trial table 
        ApiService.callCustom(ApiType.POST, variable.getHelpersById, this.Id, (data) => {
            //set list in global var
            this.ResourceRelation = data.data;
            //update trial table 
            DatatableService.updateDataGrid("relation-Resource-Table", this.ResourceRelation);
        })
    }

    getItemList() {
        let mod = $("#select-form").val();
        if (mod != "" && mod != null) {
            //api call to get dropdown values
            ApiService.callCustom(ApiType.POST, variable.getScheduleItem, mod, (response) => {
                // Set Url Name
                DropdownService.initDropdown("select-items", response.data, "FormPage");
                $("#select-items").val($("#select-items-id").val()).trigger('change')
               
                if (response.additionalData == "True") {
                  //  $("#FormItems").removeClass("d-none");
                }
          
            })
        } else {
            DropdownService.destroyReinitDropdownNull("select-item", null);
            $("#select-items").val(null).trigger('change');
           // $("#FormItems").addClass("d-none");
        }
    }
    //setValues
    getScheduleDetailByIdAPI(data) {
        this.jsonData = data;
        this.getDropdownByDate();
    }


    //updateValue
    updateValuesOnGetById() {
        //Services
        if (this.jsonData.services.length > 0) {
            $("#multiselect-service").val(this.jsonData.services).trigger("change");
        }


        //Resource
        if (this.jsonData.Resource.length > 0 && this.ResourceList.length > 0) {
            var toAdd = [];
           
            for (let Resource of this.jsonData.Resource) {
                let add = this.ResourceList.find(x => x.id == Resource);
                if (add != null) {
                    let obj = { resourceId: add.id, name: add.text }
                    toAdd.push(obj)
                }
            }
            this.ResourceRelation = toAdd;
            DatatableService.updateDataGrid("relation-Resource-Table", this.ResourceRelation);
        }
    }

    //on date selection get dropdowns 
    //clear on invalid dates
    getDropdownByDate() {
        //get start date
        var start = $("#StartTime").val();
        //get end date
        var end = $("#EndTime").val();
        //check if both start date and end dates has been inserted
        if (start != "" && end != "") {
            //Re init  dropdown 
            //asset Select
            DropdownService.destroyReinitDropdownNull("select-Asset", null, null);
            //check if start date is after end date
            if (start > end) {
                //show error message
                toastr.error("End Date Should Be Set After Start Date");
                //reinit end date
                $("#EndTime").val("")

            } else {
                //build object to send
                var dataToSend = {
                    Id: "00000000-0000-0000-0000-000000000000",
                    StartTime: start,
                    EndTime: end
                }
                //call api to get available app user, asset, team
                ApiService.callCustom(ApiType.POST, variable.getDropdownByDate, dataToSend, (data) => {
                    if (data) {
                        //get global variable 
                        this.dropdownDate = data.data;
                        //Asset Select
                        DropdownService.initDropdown("select-Asset", this.dropdownDate, "Assets");
                    }
                })
            }
        }
    }

    //helopers to table 
    addRelationResource() {
        if ($("#select-Resource :selected").val() == "") {
            toastr.error("Please select Resource.");
            return;
        }
        let selectedResource = $("#select-Resource :selected");
        let exist = this.ResourceRelation.find(x => x.resourceId == selectedResource.val());

        //reset value
        $("#select-Resource").val("").trigger("change");

        if (exist != undefined) {
            toastr.error("Resource already assigned.");
            return;
        } else {
            let data = {
                resourceId: selectedResource.val(),
                name: selectedResource.text()
            }
            this.ResourceRelation.push(data)
            DatatableService.updateDataGrid("relation-Resource-Table", this.ResourceRelation);
        }
        
    }

    //delete in grid
    deleteResourceTable(meta) {
        // remove data from list using index
        this.ResourceRelation.splice(meta, 1);
        //update Resource table
        DatatableService.updateDataGrid("relation-Resource-Table", this.ResourceRelation);
    }

    //check Valid
    valid() {
        var valid = false;
        valid = Validation.formValidation(["ScheduleName", "StartTime", "EndTime", "select-form"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        // check if table contains data
        if (this.ResourceRelation.length == 0) {
            // show error message for trial datatable 
            toastr.error("Resource Table should contain atleast one row.");
            valid = false;
        }
        return valid
    }


    //Button save ot update
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("AddEdit-Panel"));
        //check if required fields are input
        if (!this.valid() || data == false) {
            //show error in case of error
            toastr.error("Please fill in required field(s)");

        } else {
            data['SchduleId'] = this.Id;
            //data["Services"] = $("#multiselect-service").val();
            data["Helpers"] = this.ResourceRelation;
            data["Form"] = $("#select-form").val();
                //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateSchedule, data, (response) => {
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
                $('#loader-common').hide();
            })
        }
    }
    //Back button
    back() {
        //Redirect to listing
        Loader.showLoader("Redirecting To Listing...");
        location.href = "/Schedule/List#";
        Loader.hideLoader();
    }

    //Show Service Information
    showServicesInformation() {
        let selectedServices = $("#multiselect-service").val();
        var dat = {
            serviceIds: selectedServices,
            serviceRequestId: $("#ServiceRquestId").val()
        }
        if (dat.serviceIds.length == 0) {
            toastr.warning("Please select atleast one service.");
            return;
        } else {
            ApiService.callCustom(ApiType.POST, variable.getServicesByServiceId, dat, (data) => {
                if (data) {

                    $("#showInfo").html("");
                    var detailItem = [];
                    for (var item of data.data) {
                        var detail = [];
                        var dat = DefaultService.createElement("div", { class: "RenderHeader d-flex justify-content-between my-3 px-4 py-2" }, [], { innerHTML: item.name })
                        detail.push(dat)
                        if (item.elements != null) {
                            if (item.elements.find(x => x.elementType == "Datatable") == null) {
                                for (var itemDetail of item.elements) {
                                    var itemName = DefaultService.createElement("p", { class: "span-small m-0 f-weight-bold mt-2" }, {}, { innerHTML: itemDetail.elementName + " : " });
                                    detail.push(itemName)
                                    if (itemDetail.value == "") {
                                        itemDetail.value = " - ";
                                        itemDetail.uom = ""
                                    }
                                    var itemValue = DefaultService.createElement("span", { class: "span-small m-0" }, {}, { innerHTML: itemDetail.value + " " + itemDetail.uom });
                                    detail.push(itemValue)
                                    if (itemDetail.additionalValue != "") {
                                        var itemAdditional = DefaultService.createElement("span", { class: "span-small m-0" }, {}, { innerHTML: " , " + itemDetail.additionalValue });
                                        detail.push(itemAdditional)
                                    }



                                }
                            }

                        }

                        var divItem = DefaultService.createElement("div", { class: "" }, detail)
                        detailItem.push(divItem)
                    }
                    var itemToShow = DefaultService.createElement("div", { class: "" }, detailItem)
                    $("#showInfo").html(itemToShow);
                }
            })
        }

        this.jsonData.services = selectedServices;
    }
}
//init new Edit Class
window.ScheduleListAddEdit = new ScheduleListAddEdit();