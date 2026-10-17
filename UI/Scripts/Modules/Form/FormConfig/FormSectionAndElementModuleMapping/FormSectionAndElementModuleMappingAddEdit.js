import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
export class FormSectionAndElementModuleMappingAddEdit {//class that handles the Report Upload Monitoring screen
    /////Global Variables:
    //Datatble Element Data
    elementData = [];
    //trial Monitoring Id
    Id;
    constructor() {
        //Get All Dropdown Values
        this.getDropdowns();
        //Event Listener : Populate Element Drodown when selecting Form
        $('#select-FormId').on('select2:select', () => {
            this.getElementById()
            this.showHideElementSectoin();
            this.clearElementDatatable();
        });
        $('#select-FormId').on('select2:clear', () => {
            this.getElementById()
            this.showHideElementSectoin();
            this.clearElementDatatable();
        });
        //Event Listener : Populate Element Drodown when selecting Form
        $('#select-ModuleId').on('select2:select', () => {
            this.getElementById()
            this.showHideElementSectoin();
            this.clearElementDatatable();
        });
        $('#select-ModuleId').on('select2:clear', () => {
            this.getElementById()
            this.showHideElementSectoin();
            this.clearElementDatatable();
        });
        $(document).ready(function () {
            //Datatble column for Trial
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'elementId', title: 'Element Id' },
                { data: 'elementName', title: 'Element Name' },
                { data: 'readOnly', title: 'ReadOnly' }

            ];
            // Init Trial Dtatable 
            // Parameters: Table Id, list of columns, data, has delete action? , delete action function
            DatatableService.initializeDataGridServices("ElementTable", ColumnList, this.elementData, true, "FormSectionAndElementModuleMappingAddEdit.deleteFromElementTable")
        })

    }
    //get all dropdown values
    getDropdowns() {
        var dropdown = null;
        ApiService.callCustom(ApiType.POST, variable.getFPSEAllDropdown, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {
                    // Set Form list of values
                    DropdownService.initDropdown("select-FormId", dropdown, "Form");
                    // If Screen mode edit, Set Form selected value to get by id value
                    $("#select-FormId").val($("#select-FormId-id").val()).trigger('change');

                    // Set Module list of values
                    DropdownService.initDropdown("select-ModuleId", dropdown, "Module");
                    // If Screen mode edit, Set Module selected value to get by id value
                    $("#select-ModuleId").val($("#select-ModuleId-id").val()).trigger('change');

                    //Element will get values from selecting Form
                    DropdownService.initDropdownNull("select-element");

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
            this.setFieldsReadonly();
            this.setDataInTable();
        };
        if (urlParams.has('View')) {
            if (parseInt(urlParams.get('View'))) {
                $(".noView").each(function (i, obj) {
                    obj.classList.add("d-none")
                });
            }
        };
    }

    //get sectoin by id
    getElementById() {
        let frm = $("#select-FormId").val();
        if (frm != "") {
            let data = parseInt(frm);
            //api call to get dropdown values
            ApiService.callCustom(ApiType.POST, variable.getSEByIdDropdown, data, (response) => {
                // Set sectoin list of values
                DropdownService.destroyReinitDropdown("select-element", response.data, "Element");
            })
        } else {
            DropdownService.destroyReinitDropdownNull("select-element", null);
            $("#select-element").val(null).trigger('change');
        }
    }

    showHideElementSectoin() {
        let frm = $("#select-FormId").val();
        let mdl = $("#select-ModuleId").val();
        if (frm != "" && mdl != "") {
            $("#scrollspy-Element").removeClass("d-none");
            $("#section-element").removeClass("d-none");
        } else {
            $("#scrollspy-Element").addClass("d-none");
            $("#section-element").addClass("d-none");
        }
    }

    setFieldsReadonly() {
        document.getElementById("select-FormId").setAttribute('disabled', true);
        document.getElementById("select-ModuleId").setAttribute('disabled', true);
        this.getElementById();
        this.showHideElementSectoin();
    }

    // Add to Datatatable dropdown 
    addDataElement() {
        // data to add in list of trial data
        var remember = document.getElementById("ReadOnly");
        let data = {
            id: 0,
            elementId: parseInt($("#select-element").val()),
            elementName: $("#select-element option:selected").text().split(" - ")[1],
            readOnly: remember.checked ? true : false,
        }
        // Validation
        if (!Validation.formValidation(["select-element"])) {
            toastr.warning("Please fill in mandatory fields");
        } else {
            var exist = this.elementData.find(x => x.elementId == data.elementId)
            if (exist) {
                toastr.error("Already selected this Element");
                this.resetData();
            } else {
                //push data in list 
                this.elementData.push(data);
                //Update datatable
                DatatableService.updateDataGrid("ElementTable", this.elementData);
                this.resetData();
            }
        }
    }

    //reset data 
    resetData() {
        $("#select-element").val(null).trigger('change');
        $("#ReadOnly").prop("checked", false);
    }

    //clear trial Data 
    clearElementDatatable() {
        this.elementData = [];
        DatatableService.updateDataGrid("ElementTable", this.elementData);
    }
    //delete Attachmenet
    deleteFromElementTable(meta) {
        // remove data from list using index
        this.elementData.splice(meta, 1);
        //update trial table 
        DatatableService.updateDataGrid("ElementTable", this.elementData);
    }

    // Set Data in table on edit 
    setDataInTable() {
        let data = {
            Form: $("#select-FormId").val(),
            Module: $("#select-ModuleId").val(),
        }
        // call api to get data saved trial table 
        ApiService.callCustom(ApiType.POST, variable.getSEElementTable, data, (data) => {
            //set list in global var
            this.elementData = data.data;
            //update trial table 
            DatatableService.updateDataGrid("ElementTable", this.elementData);
        })
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["select-FormId", "select-ModuleId"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }
        // check if table contains data
        if (this.elementData.length == 0) {
            // show error message for trial datatable 
            toastr.error("Element Mapping Table should contain atleast one row.");
            valid = false;
        }

        //return boolean id valid or not
        return valid
    }

    // save action button
    save() {
        //Show loader 
        Loader.showLoader("Saving Data...");
        // Set data to send to api
        let data = DefaultService.result(document.getElementById("section-element-mapping"));
        //check validation and data 
        if (this.valid() && data != false) {
            //Set Id
            data['Id'] = this.Id;
            if (this.Id == undefined) { data['Id'] = 0 }
            data['Module'] = $("#select-ModuleId").val();
            let elementToSave = [];
            this.elementData.forEach(z => {
                let element = {
                    Id: z.id,
                    ElementId: z.elementId,
                    ReadOnly: z.readOnly,
                }
                elementToSave.push(element);
            })
            data['Element'] = elementToSave;
            // call api to save 
            ApiService.callCustom(ApiType.POST, variable.getFPSEMSaveUpdate, data, (response) => {
                //if response true: Save successfull
                if (response.data) {
                    // show success message
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
        location.href = "/FormSectionAndElementModuleMapping/List#";
        Loader.hideLoader();
    }
}
window.FormSectionAndElementModuleMappingAddEdit = new FormSectionAndElementModuleMappingAddEdit();