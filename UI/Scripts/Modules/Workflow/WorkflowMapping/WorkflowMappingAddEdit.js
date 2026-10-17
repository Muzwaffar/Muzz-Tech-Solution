import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { ResearchTypeEnum, ResearchSubModuleEnum, SaveStatus, ParentModule } from "/Scripts/Common/Enums.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { FileService } from "/Scripts/Common/File/FileService.js";
export class WorkflowMappingAddEdit {//class that handles the Report Upload Monitoring screen
    /////Global Variables:
    //Datatble Element Data
    elementData = [];
    //trial Monitoring Id
    Id;
    constructor() {
        //Get All Dropdown Values
        this.getDropdowns();
        //Event Listener : Populate Section Drodown when selecting Form
        $('#select-ModuleId').on('select2:select', () => {
            this.showHideDetailSection();
        });
        $('#select-ModuleId').on('select2:clear', () => {
            this.showHideDetailSection();
        });
        $('#select-WFType').on('select2:select', () => {
            this.showHideDetailSection();
        });
        $('#select-WFType').on('select2:clear', () => {
            this.showHideDetailSection();
        });
        $('#select-CaseStatus').on('select2:select', () => {
            this.showHideDetailSection();
        });
        $('#select-CaseStatus').on('select2:clear', () => {
            this.showHideDetailSection();
        });
        $('#select-AttributeType').on('select2:select', () => {
            this.showHideDetailSection();
        });
        $('#select-AttributeType').on('select2:clear', () => {
            this.showHideDetailSection();
        });
        //Event Listener : Populate URL Name Drodown when selecting Next Screen Name
        $('#select-dModuleNext').on('select2:select', () => {
            this.getURLName();
        });
        $('#select-dModuleNext').on('select2:clear', () => {
            this.getURLName();
        });
        $(document).ready(function () {
            //Datatble column for Trial
            var ColumnList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'name', title: 'Name' },
                { data: 'description', title: 'Description' },
                { data: 'ext1', title: 'Module Id', visible: false },
                { data: 'moduleNext', title: 'Next Screen Name' },
                { data: 'ext2', title: 'URL Name' },
                { data: 'caseStatusId', title: 'Case Status Id', visible: false },
                { data: 'caseStatus', title: 'Case Status' },
                { data: 'aTypeId', title: 'AType Id', visible: false },
                { data: 'aType', title: 'AType' },
                { data: 'wfTypeId', title: 'Workflow TypeId', visible: false },
                { data: 'wfType', title: 'Workflow Type' },
                { data: 'activityTypeId', title: 'Activity TypeId', visible: false },
                { data: 'activityType', title: 'Activity Type' },
                { data: 'sequence', title: 'Sequence' },
                { data: 'formSequence', title: 'Form Sequence' },
                { data: 'moduleIdCurrent', title: 'Module Id', visible: false },
                { data: 'moduleCurrent', title: 'Current Screen Name' },
                { data: 'message', title: 'Message' },

            ];
            // Init Trial Dtatable 
            // Parameters: Table Id, list of columns, data, has delete action? , delete action function
            DatatableService.initializeDataGridServices("ElementTable", ColumnList, this.elementData, true, "WorkflowMappingAddEdit.deleteFromElementTable")
        })

    }
    //get all dropdown values
    getDropdowns() {
        var dropdown = null;
        ApiService.callCustom(ApiType.POST, variable.getWFMAllDropdown, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {
                    // Set Module list of values
                    DropdownService.initDropdown("select-ModuleId", dropdown, "Module");
                    // If Screen mode edit, Set Module selected value to get by id value
                    $("#select-ModuleId").val($("#select-ModuleId-id").val()).trigger('change');

                    // Set AttributeType list of values
                    DropdownService.initDropdown("select-AttributeType", dropdown, "AttributeType");
                    // If Screen mode edit, Set AttributeType selected value to get by id value
                    $("#select-AttributeType").val($("#select-AttributeType-id").val()).trigger('change');

                    // Set CaseStatus list of values
                    DropdownService.initDropdown("select-CaseStatus", dropdown, "CaseStatus");
                    // If Screen mode edit, Set CaseStatus selected value to get by id value
                    $("#select-CaseStatus").val($("#select-CaseStatus-id").val()).trigger('change');

                    // Set WFType list of values
                    DropdownService.initDropdown("select-WFType", dropdown, "WFType");
                    // If Screen mode edit, Set WFType selected value to get by id value
                    $("#select-WFType").val($("#select-WFType-id").val()).trigger('change');

                    // Set ModuleNext list of values
                    DropdownService.initDropdown("select-dModuleNext", dropdown, "Module");

                     // Set ModuleCurrent list of values
                    DropdownService.initDropdown("select-dModuleCurrent", dropdown, "Module");

                    DropdownService.initDropdown("select-dCaseStatus", dropdown, "CaseStatus");
                    $("#select-dCaseStatus option:contains('Draft')").prop('selected', true).trigger('change');

                    DropdownService.initDropdown("select-aType", dropdown, "AType");

                    DropdownService.initDropdown("select-dwfType", dropdown, "WFType");

                    // Set Activity Type list of values
                    DropdownService.initDropdown("select-activity-type", dropdown, "ActivityType");

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
            this.showHideDetailSection();
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

    getURLName() {
        let mouduleNext = $("#select-dModuleNext").val();
        if (mouduleNext != "") {
            //api call to get dropdown values
            ApiService.callCustom(ApiType.POST, variable.getURLName, mouduleNext, (response) => {
                // Set Url Name
                $("#dURLName").val(response.data);
            })
        } else {
            $("#dURLName").val("");
        }
    }
    showHideDetailSection() {
        let moduleid = $("#select-ModuleId").val() ?? "";
        let workflow = $("#select-WFType").val() ?? "";
        let caseStatus = $("#select-CaseStatus").val() ?? "";
        let attr = $("#select-AttributeType").val() ?? "";
        if (moduleid != "" && workflow != "" && caseStatus != "" && attr != "") {
            $("#scrollspy-details").removeClass("d-none");
            $("#section-details").removeClass("d-none");
        } else {
            $("#scrollspy-details").addClass("d-none");
            $("#section-details").addClass("d-none");
            this.clearElementDatatable();
        }
    }

    setFieldsReadonly() {
        $('#Name').prop('readonly', true);
        $('#Sequence').prop('readonly', true);
        $('#select-ModuleId').prop('disabled', true);
        $('#select-WFType').prop('disabled', true);
        $('#select-CaseStatus').prop('disabled', true);
        $('#select-AttributeType').prop('disabled', true);
    }

    // Add to Datatatable dropdown 
    addDataElement() {
        // data to add in list of trial data
        let data = {
            id: 0,
            name: $("#dName").val(),
            sequence: $("#dSequence").val(),
            description: $("#dDescription").val(),
            ext1: $("#select-dModuleNext").val(),
            moduleNext: $("#select-dModuleNext option:selected").text(),
            ext2: $("#dURLName").val(),
            caseStatusId: parseInt($("#select-dCaseStatus").val()),
            caseStatus: $("#select-dCaseStatus option:selected").text(),
            aTypeId: parseInt($("#select-aType").val()),
            aType: $("#select-aType option:selected").text(),
            wfTypeId: parseInt($("#select-dwfType").val()),
            wfType: $("#select-dwfType option:selected").text(),
            activityTypeId: parseInt($("#select-activity-type").val()),
            activityType: $("#select-activity-type option:selected").text(),
            moduleIdCurrent: $("#select-dModuleCurrent").val(),
            moduleCurrent: $("#select-dModuleCurrent option:selected").text(),
            formSequence: $("#dFormSequence").val(),
            message: $("#message").val(),
        }
        // Validation
        if (!Validation.formValidation(["dName", "dSequence", "select-dCaseStatus", "select-aType", "select-dwfType", "select-activity-type", "select-dModuleCurrent"])) {
            toastr.warning("Please fill in mandatory fields");
        } else {
            var exist = this.elementData.find(x => x.sequence == data.sequence)
            if (exist) {
                toastr.error("Sequence already selected");
                $("#dSequence").val("");
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
        $("#dName").val("");
        $("#dSequence").val("");
        $("#dDescription").val("");
        $("#select-dModuleNext").val(null).trigger('change');
        $("#dURLName").val("");
        $("#select-dCaseStatus").val(null).trigger('change');
        $("#select-attribute-type").val(null).trigger('change');
        $("#select-dwfType").val(null).trigger('change');
        $("#select-aType").val(null).trigger('change');
        $("#select-dModuleCurrent").val(null).trigger('change');
        $("#dFormSequence").val("");
        $("#message").val("");
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
        ApiService.callCustom(ApiType.POST, variable.getWFDTable, this.Id, (data) => {
            //set list in global var
            this.elementData = data.data;
            //update trial table 
            DatatableService.updateDataGrid("ElementTable", this.elementData);
        })
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["Name", "select-ModuleId", "Sequence", "select-WFType", "select-CaseStatus", "select-AttributeType"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }
        // check if table contains data
        if (this.elementData.length == 0) {
            // show error message for details datatable 
            toastr.error("Workflow Mapping Table should contain atleast one row.");
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
        let data = DefaultService.result(document.getElementById("workflow-mapping"));
        //check validation and data 
        if (this.valid() && data != false) {
            //Set Id
            data['Id'] = this.Id;
            data['Sequence'] = parseInt($("#Sequence").val());
            data['ModuleId'] = $("#select-ModuleId").val();
            if (this.Id == undefined) { data['Id'] = 0 }
            let elementToSave = [];
            this.elementData.forEach(z => {
                let details = {
                    Id: z.id,
                    Name: z.name,
                    Description: z.description,
                    Ext1: z.ext1,
                    Ext2: z.ext2,
                    CaseStatusId: z.caseStatusId,
                    ATypeId: z.aTypeId,
                    WFTypeId: z.wfTypeId,
                    Sequence: z.sequence,
                    ActivityTypeId: z.activityTypeId,
                    FormSequence: z.formSequence == "" ? 0 : z.formSequence,
                    ModuleIdCurrent: z.moduleIdCurrent,
                    Message: z.message,
                }
                elementToSave.push(details);
            })
            data['Details'] = elementToSave;
             //call api to save 
            ApiService.callCustom(ApiType.POST, variable.getWFMSaveUpdate, data, (response) => {
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
        location.href = "/WorkflowMapping/List#";
        Loader.hideLoader();
    }
}
window.WorkflowMappingAddEdit = new WorkflowMappingAddEdit();