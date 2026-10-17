import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { ResearchTypeEnum, ResearchSubModuleEnum, SaveStatus, ParentModule } from "/Scripts/Common/Enums.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { FileService } from "/Scripts/Common/File/FileService.js";
export class FormPagePDFMappingAddEdit {//class that handles the Report Upload Monitoring screen
    /////Global Variables:
    //Datatble Element Data
    elementData = [];
    //trial Monitoring Id
    Id;
    constructor() {
        //Get All Dropdown Values
        this.getDropdowns();
        //Event Listener : Populate Section Drodown when selecting Form
        $('#select-FormId').on('select2:select', () => {
            this.getSectionById()
            this.getTemplateTextById();
            this.showHideElementSectoin();
        });
        $('#select-FormId').on('select2:clear', () => {
            this.getSectionById()
            this.getTemplateTextById();
            this.showHideElementSectoin();
            this.clearElementDatatable();
        });
        //Event Listener : Populate Element Drodown when selecting Section
        $('#select-section').on('select2:select', () => {
            this.getElementById()
        });
        $('#select-section').on('select2:clear', () => {
            this.getElementById()
        });
        //Event Listener : Populate Element Drodown when selecting Section
        $('#select-TemplateName').on('select2:select', () => {
            this.getTemplateTextById();
            this.showHideElementSectoin();
        });
        $('#select-TemplateName').on('select2:clear', () => {
            this.getTemplateTextById();
            this.showHideElementSectoin();
            this.clearElementDatatable();
        });
        //Event Listener : showhide Fields
        $('#select-element-type').on('select2:select', () => {
            this.showHideFields()
        });
        $('#select-element-type').on('select2:clear', () => {
            $("#FieldparentId").addClass("d-none");
            $("#Fieldrow").addClass("d-none");
            $("#Fieldcolumn").addClass("d-none");
        });
        $(document).ready(function () {
            //Datatble column for Trial
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'elementId', title: 'Element Id'},
                { data: 'elementName', title: 'Element Name' },
                { data: 'elementTypeId', title: 'Element Type Id', visible: false },
                { data: 'elementType', title: 'Element Type'},
                { data: 'parentId', title: 'Parent Id' },
                { data: 'row', title: 'Row'},
                { data: 'column', title: 'Column' },
                { data: 'textName', title: 'Template Text' }

            ];
            // Init Trial Dtatable 
            // Parameters: Table Id, list of columns, data, has delete action? , delete action function
            DatatableService.initializeDataGridServices("ElementTable", ColumnList, this.elementData, true, "FormPagePDFMappingAddEdit.deleteFromElementTable")
        })

    }
    //get all dropdown values
    getDropdowns() {
        var dropdown = null;
        ApiService.callCustom(ApiType.POST, variable.getFPMAllDropdown, "", (data) => {
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

                    // Set TemplateName list of values
                    DropdownService.initDropdown("select-TemplateName", dropdown, "TemplateName");

                    //Section will get values from selecting Form
                    DropdownService.initDropdownNull("select-section");

                    //Element will get values from selecting section
                    DropdownService.initDropdownNull("select-element");

                    // Set Element Type list of values
                    DropdownService.initDropdown("select-element-type", dropdown, "ElementType");

                    //Template Text will get values from selecting Template Name
                    DropdownService.initDropdownNull("select-template-text");

                }
                //call urlParameter funtion to check whether this page is an edit or a create
                //used to populate data 
                this.getURLparameters();
            }
        });
    }

    //get all dropdown values
    getTemplateDropdown() {
        var dropdown = null;
        setTimeout(e => {
            ApiService.callCustom(ApiType.POST, variable.getFPMAllDropdown, "", (data) => {
                if (data) {
                    dropdown = data.data;
                    if (dropdown != null) {
                        // Set TemplateName list of values
                        DropdownService.initDropdown("select-TemplateName", dropdown, "TemplateName");
                    }
                }
            });
        }, 100)
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
            this.setTemplateName();
            this.getSectionById()
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

    setTemplateName() {
        let name = $("#select-TemplateName").find("option:contains('" + $("#TemplateName").val() + "')").val()
        $("#select-TemplateName").val(name).trigger('change');
        this.showHideElementSectoin();
        this.getTemplateTextById();
    }

    //get sectoin by id
    getSectionById() {
        let frm = $("#select-FormId").val();
        if (frm != null) {
            let data = parseInt(frm);
            //api call to get dropdown values
            ApiService.callCustom(ApiType.POST, variable.getSectionByIdDropdown, data, (response) => {
                // Set sectoin list of values
                DropdownService.destroyReinitDropdown("select-section", response.data, "Section");
            })
        } else {
            DropdownService.destroyReinitDropdownNull("select-section", null);
            $("#select-section").val(null).trigger('change');
        }

    }

    //get sectoin by id
    getElementById() {
        let sec = $("#select-section").val();
        if (sec != "") {
            let data = parseInt(sec);
            //api call to get dropdown values
            ApiService.callCustom(ApiType.POST, variable.getElementByIdDropdown, data, (response) => {
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
        let tmpName = $("#select-TemplateName").val();
        if (frm != "" && tmpName != "") {
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
        document.getElementById("select-TemplateName").setAttribute('disabled', true);
    }

    showHideFields() {
        let elmType = $("#select-element-type option:selected").text()
        if (elmType == "Datatable") {
            $("#FieldparentId").removeClass("d-none");
            $("#Fieldrow").removeClass("d-none");
            $("#Fieldcolumn").removeClass("d-none");
        } else {
            $("#FieldparentId").addClass("d-none");
            $("#Fieldrow").addClass("d-none");
            $("#Fieldcolumn").addClass("d-none");
        }
    }

    //get sectoin by id
    getTemplateTextById() {
        let tmpName = $("#select-TemplateName").val();
        let frm = $("#select-FormId").val();
        if (tmpName != "" && frm != "") {
            let data = $("#select-TemplateName option:selected").text();
            //api call to get dropdown values
            ApiService.callCustom(ApiType.POST, variable.getTemplateTextByIdDropdown, data, (response) => {
                // Set sectoin list of values
                DropdownService.destroyReinitDropdown("select-template-text", response.data, "TemplateText");
            })
        } else {
            DropdownService.destroyReinitDropdownNull("select-template-text", null);
            $("#select-template-text").val(null).trigger('change');
        }

    }

    // Add to Datatatable dropdown 
    addDataElement() {
        // data to add in list of trial data
        let data = {
            id: 0,
            elementId: parseInt($("#select-element").val()),
            elementName: $("#select-element option:selected").text().split(" - ")[1],
            elementTypeId: parseInt($("#select-element-type").val()),
            elementType: $("#select-element-type option:selected").text(),
            parentId: $("#parentId").val(),
            row: $("#row").val(),
            column: $("#column").val(),
            textName: $("#select-template-text option:selected").text(),
        }
        // Validation
        if (!Validation.formValidation(["select-element", "select-element-type", "select-template-text"])) {
            toastr.warning("Please fill in mandatory fields");
        }else if (data.elementType == "Datatable" && !Validation.formValidation(["parentId", "row", "column"])) {
            toastr.warning("Please fill in mandatory fields");
        } else {
            var exist = this.elementData.find(x => x.textName == data.textName)
            if (exist) {
                toastr.error("Already selected this Text Name");
                $("#select-template-text").val(null).trigger('change');
            }else {
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
        $("#FieldparentId").addClass("d-none");
        $("#Fieldrow").addClass("d-none");
        $("#Fieldcolumn").addClass("d-none");

        $("#select-section").val(null).trigger('change');
        $("#select-element").val(null).trigger('change');
        $("#select-element-type").val(null).trigger('change');
        $("#parentId").val("");
        $("#row").val("");
        $("#column").val("");
        $("#select-template-text").val(null).trigger('change');
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
        // call api to get data saved trial table 
        ApiService.callCustom(ApiType.POST, variable.getPDFElementTable, this.Id, (data) => {
            //set list in global var
            this.elementData = data.data;
            //update trial table 
            DatatableService.updateDataGrid("ElementTable", this.elementData);
        })
    }

    //Validate Input type field 
    validateFileType() {
        var valid = Validation.validateFileType('reports', 4);
        if (valid) {
            toastr.success("Upload Successfully")
            var file = FileService.addSingleFileForUpload("reports");
            if (this.saveAttachment(file)) {
                // show success message
                toastr.success("Saved Successfully")
                setTimeout(e => {
                    this.getTemplateDropdown();
                }, 100)
            } else {
                toastr.error("File not saved. Please contact Admin")
            };
        }
    }
    saveAttachment(file) {
        return FileService.uploadSingleFile(file);
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["select-FormId", "select-ModuleId", "select-TemplateName"])
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
        let data = DefaultService.result(document.getElementById("template-form-mapping"));
        //check validation and data 
        if (this.valid() && data != false) {
            //Set Id
            data['Id'] = this.Id;   
            if (this.Id == undefined) { data['Id'] = 0}
            data['ModuleId'] = $("#select-ModuleId option:selected").text();
            data['TemplateName'] = $("#select-TemplateName option:selected").text();
            let elementToSave = [];
            this.elementData.forEach(z => {
                let element = {
                    Id : z.id,
                    FkTFM : 0,
                    ElementId : z.elementId,
                    ParentId : z.parentId == "" ? null : z.parentId,
                    Row : z.row == "" ? null : z.row,
                    Column : z.column == "" ? null : z.column,
                    TextName : z.textName,
                    ElementName : z.elementName,
                    ElementTypeId : z.elementTypeId,
                    ElementType : z.elementType,
                }
                elementToSave.push(element);
            })
            data['Element'] = elementToSave;
            // call api to save 
            ApiService.callCustom(ApiType.POST, variable.getFPPDFMSaveUpdate, data, (response) => {
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
        location.href = "/FormPagePDFMapping/List#";
        Loader.hideLoader();
    }
}
window.FormPagePDFMappingAddEdit = new FormPagePDFMappingAddEdit();