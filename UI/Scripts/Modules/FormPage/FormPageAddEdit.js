import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { ElementRendering } from "/Scripts/Common/Rendering/ElementRendering.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { LocalStorageValues, StatusValues, ResearchSubModuleEnum, CaseStatus, ApprovalStatus } from "/Scripts/Common/Enums.js";
import { ApprovalPopup, ActivityPopup, SaveApprovalStatus } from "/Scripts/Modules/Common/ApprovalService.js";
import { FileService } from "/Scripts/Common/File/FileService.js";

export class FormPageAddEdit {//class that handles the Form Page Add Edit

    Id;
    tableColumnList = [];
    sections = [];
    gridList = [];
    attachementList = [];
    dropdownForm = [];
    constructor() {
        //Get All Dropdown Values
        this.setDropdown();
        //Event Listener : Populate Services when form is selected
        $('#select-FormId').on('select2:select', () => {
            this.populateSections()
        });
        //Event Listener : Populate Services when form is selected
        $('#select-FormId').on('select2:clear', () => {
            this.clearContents()
        });
    }

    //set Dropdown Values
    setDropdown() {
        var dropdown = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getAllDropDownValuesFormPage, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {
                    this.dropdownForm = dropdown.find(y => y.title == "FormList") == undefined ? [] : dropdown.find(y => y.title == "FormList").items;
                    // Set treatment list of values
                    DropdownService.initDropdown("select-FormId", dropdown, "FormList");

                    this.getURLparameters();
                }
            }
        });

    }

    //funtion to check whether this page is an edit or a create
    getURLparameters() {
        const queryString = window.location.search;
        //url Parameters
        const urlParams = new URLSearchParams(queryString);
        if (urlParams.has('MId')) {
            var MID = urlParams.get('MId')
            sessionStorage.setItem(LocalStorageValues.MODULEID, MID);
        };
        // if url contains id, switch to edit mode
        if (urlParams.has('Id')) {
            //get Id from URL
            this.Id = parseInt(urlParams.get('Id'))
            //Get inherited data when loading the page
            this.getDetailsById()
        };
        // if url contains form, populae form
        if (urlParams.has('Form')) {
            //Populate Form
            this.getFormDetails(urlParams.get('Form'))
        };
        //if view mode
        if (urlParams.has('View')) {
            if (parseInt(urlParams.get('View'))) {
                $(".noView").each(function (i, obj) {
                    obj.classList.add("d-none")
                });
            }
        };
    }

    //populate for details
    getFormDetails(strForm) {
        var updatedDropdown = [];
        updatedDropdown = this.dropdownForm.filter(x => x.text.replaceAll(' ', '').toLowerCase().includes(strForm));
        DropdownService.destroyReinitDropdownFiltered("select-FormId", updatedDropdown);
        if (this.Id == undefined) {
            if (updatedDropdown.length == 1) {
                $("#select-FormId").val(updatedDropdown[0].id).trigger('change');
                this.populateSections()
            }
        }
       
    }
    
    //Set Detail
    getDetailsById() {

        ApiService.callCustom(ApiType.POST, variable.getByIdApiFormPage, this.Id, (response) => {
            if (response) {
                $("#select-FormId").val($("#select-FormId-id").val()).trigger('change');
                this.sections = response.data.sections;
                ElementRendering.renderer(this.sections, "FormPageAddEdit");
                this.sections.forEach(sections => {
                    var table = sections.elements.filter(x => x.elementType.toLowerCase().includes("datatable"));
                    if (table.length > 0) {
                        for (let tab of table) {
                            var cols = [];
                            for (let innerElem of tab.innerelements) {

                                if (innerElem.elementType.toLowerCase() == "dropdown") {
                                    DropdownService.initDropdownFiltered("elem-" + innerElem.elementId, innerElem.sourceValues)
                                }
                                var col;
                                if (innerElem.elementType.toLowerCase() == "date") {
                                    col = {
                                        data: innerElem.elementId, title: innerElem.elementName, render: function (data, type, full, meta) {
                                            data = moment(data).format('DD/MM/yyyy')
                                            return data;
                                        }
                                    };
                                } else if (innerElem.elementType.toLowerCase() == "datetime") {
                                    col = {
                                        data: innerElem.elementId, title: innerElem.elementName, render: function (data, type, full, meta) {
                                            data = moment(data).format("DD/MM/yyyy") + " " + moment(data).format("HH:mm")
                                            return data;
                                        }
                                    };
                                } else {
                                    col = { data: innerElem.elementId, title: innerElem.elementName };
                                }

                                cols.push(col)
                            }
                            this.tableColumnList.push({ id: tab.elementId, dataColumns: cols })
                            if (tab.readOnly) {
                                DatatableService.initializeDataGridDynamic(tab.elementId, cols, "FormPageAddEdit", null, "d-none")
                            } else {
                                DatatableService.initializeDataGridDynamic(tab.elementId, cols, "FormPageAddEdit", null)
                            }
                            
                        }

                    }

                    //Checkbox state
                    var checklist = sections.elements.filter(x => x.elementType.toLowerCase().includes("checkbox"));
                    if (checklist.length > 0) {
                        for (let check of checklist) {
                            $("#" + check.elementId).prop("checked", check.value == "true" ? true : false);

                        }
                    }

                    //table view
                    var table = sections.elements.filter(x => x.elementType.toLowerCase().includes("datatable"));
                    if (table.length > 0) {
                        for (let tab of table) {
                            var cols = [];
                            for (let innerElem of tab.innerelements) {
                                if (innerElem.elementType.toLowerCase() == "dropdown") {
                                    DropdownService.initDropdownFiltered("elem-" + innerElem.elementId, innerElem.sourceValues)
                                }
                                var col = { data: "elem-" + innerElem.elementId, title: innerElem.elementName };
                                cols.push(col)
                            }
                            this.tableColumnList.push({ id: tab.elementId, dataColumns: cols })

                            if (tab.readOnly) {
                                DatatableService.initializeDataGridDynamic(tab.elementId, cols, "FormPageAddEdit", null, "d-none")
                            } else {
                                DatatableService.initializeDataGridDynamic(tab.elementId, cols, "FormPageAddEdit", null)
                            }
                            var rows = tab.innerelementsvalue.length / tab.innerelements.length;
                            let dat = [];

                            for (var i = 1; i <= rows; i++) {
                                let data = {};
                                for (let datum of tab.innerelementsvalue.filter(y => y.row == i)) {
                                    data[datum.elementId] = datum.value;
                                    data[datum.elementId + "_" + datum.id] = datum.value;
                                }
                                data.row = i;
                                dat.push(data)
                            }
                            this.gridList.push({
                                "name": tab.elementId,
                                "sectionId": sections.sectionId,
                                "data": dat
                            });
                            DatatableService.updateDataGrid(tab.elementId, this.gridList.find(x => x.name == tab.elementId).data)
                        }

                    }

                    var attachment = sections.elements.filter(x => x.elementType.toLowerCase().includes("attachment"));
                    if (attachment.length > 0) {
                        for (let att of attachment) {
                            let dat = [];
                            for (let innerElem of att.innerelementsvalue) {
                                let datum = {
                                    file: innerElem.additionalValue,
                                    id: innerElem.elementId,
                                    keywords: innerElem.value.split("__")[1],
                                    fileName: innerElem.value.split("__")[0]
                                }
                                dat.push(datum);
                            }
                            setTimeout(e => {
                                DatatableService.updateDataGrid("attachment-table-" + att.elementId, dat);
                            }, 1000)

                            this.attachementList.push({
                                "name": att.elementId,
                                "sectionId": sections.sectionId,
                                "data": att
                            });
                        }
                    }
                })
            }
        });

    }

    //populate Sections when form is seletced
    populateSections() {
        let data = parseInt($("#select-FormId").val());
        //api call to get dropdown values
        ApiService.callCustom(ApiType.POST, variable.getSectionElementsByForm, data, (response) => {
            if (response.data) {
                if (response.data.sections.length > 0) {
                    this.sections = response.data.sections;
                    ElementRendering.renderer(this.sections, "FormPageAddEdit");
                    this.sections.forEach(sections => {
                        var table = sections.elements.filter(x => x.elementType.toLowerCase().includes("datatable"));
                        if (table.length > 0) {
                            for (let tab of table) {
                                var cols = [];
                                for (let innerElem of tab.innerelements) {

                                    if (innerElem.elementType.toLowerCase() == "dropdown") {
                                        DropdownService.initDropdownFiltered("elem-" + innerElem.elementId, innerElem.sourceValues)
                                    }
                                    var col;
                                    if (innerElem.elementType.toLowerCase() == "date") {
                                        col = {
                                            data: innerElem.elementId, title: innerElem.elementName, render: function (data, type, full, meta) {
                                                data = moment(data).format('DD/MM/yyyy')
                                                return data;
                                            }
                                        };
                                    } else if (innerElem.elementType.toLowerCase() == "datetime") {
                                        col = {
                                            data: innerElem.elementId, title: innerElem.elementName, render: function (data, type, full, meta) {
                                                data = moment(data).format("DD/MM/yyyy") + " " + moment(data).format("HH:mm")
                                                return data;
                                            }
                                        };
                                    } else {
                                        col = { data: innerElem.elementId, title: innerElem.elementName };
                                    }
                                    cols.push(col)
                                }
                                this.tableColumnList.push({ id: tab.elementId, dataColumns: cols })
                                if (tab.readOnly) {
                                    DatatableService.initializeDataGridDynamic(tab.elementId, cols, "FormPageAddEdit", null, "d-none")
                                } else {
                                    DatatableService.initializeDataGridDynamic(tab.elementId, cols, "FormPageAddEdit", null)
                                }
                            }

                        }

                        var attachment = sections.elements.filter(x => x.elementType.toLowerCase().includes("attachment"));
                        if (attachment.length > 0) {
                            for (let att of attachment) {
                                this.attachementList.push({
                                    "name": att.elementId,
                                    "sectionId": sections.sectionId,
                                    "data": att
                                });
                            }
                        }
                    })
                   
                   
                } else {
                    this.clearContents();
                }
            }
            
        })
    }

    //update value when chnaging value 
    updateValueRender(sectionId, elementId, type, e ,additional = null, customType = null) {
        var getValue = null;
        var additionalValue = null;
        var getId = null;
        var getAdditionalValue = null;
        if (customType == null) {
            //get value to set in object 
            getValue = e[0].value;
            getId = e[0].id;
            if (type.toLowerCase().includes("checkbox")) {
                getValue = $("#" + getId).is(':checked').toString()
            }
            if (type.toLowerCase().includes("dropdown")) {
                getValue = $("#" + getId + " :selected").val();
                getAdditionalValue = $("#" + getId + " :selected").text();
            }

            //set data in list 
            if (this.sections.find(x => x.sectionId == sectionId) != undefined) {
                if (additional != null) {
                    this.sections.find(x => x.sectionId == sectionId).elements.find(y => y.elementId == elementId).additionalValue = getValue;
                } else {
                    this.sections.find(x => x.sectionId == sectionId).elements.find(y => y.elementId == elementId).value = getValue;
                    if (getAdditionalValue != null) {
                        this.sections.find(x => x.sectionId == sectionId).elements.find(y => y.elementId == elementId).additionalValue = getAdditionalValue;
                    }
                }
            }

            //Get Details when value is selected

            if (type.toLowerCase().includes("dropdown with details")) {
                if ($("#details-container_" + elementId).length > 0) {
                    $("#details-container_" + elementId).remove();
                }
                if (getValue != "") {
                    let data = {
                        ElementId: elementId,
                        DetailsId: parseInt(getValue)
                    }
                    //api call to get dropdown values
                    ApiService.callCustom(ApiType.POST, variable.getCustomDetailsByElementId, data, (response) => {
                        if (response.data) {
                            let div = $("#" + getId.replace("elem-", "") + "-id")[0].parentElement;
                            div.append(
                                ElementRendering.dropdownNaaDetailsRenderer(response.data, elementId)
                            );
                        }

                    })
                }
            }
            //Custom Element update for signature
        } else if (customType == "signature") {
            if (e == null) {
                getValue = "";
                additionalValue = "";
            } else {
                getValue = e;
                additionalValue = localStorage.getItem(LocalStorageValues.FULLUSERNAME);
            }
            if (this.sections.find(x => x.sectionId == sectionId) != undefined) {
                this.sections.find(x => x.sectionId == sectionId).elements.find(y => y.elementId == elementId).additionalValue = getValue;
                this.sections.find(x => x.sectionId == sectionId).elements.find(y => y.elementId == elementId).value = additionalValue;
            }
        }
    }

    //add to table
    addToGrid(sectionId, elementId) {
        var service = this.sections.find(x => x.sectionId == sectionId);
        if (service != null) {
            var elem = service.elements.find(x => x.elementId == elementId)
            var datum = {};
            elem.innerelements.forEach(y => {
                $("#" + y.elementId).val();
                if (y.elementType.toLowerCase() == "dropdown") {
                    datum[y.elementId] = $("#" + "elem-" +  y.elementId + " :selected").text();
                    y.entityid = $("#" + y.elementId).val();
                } else if (["text", "number", "date", "datetime"].includes(y.elementType.toLowerCase())) {
                    datum[y.elementId] = $("#" + y.elementId).val();
                }

            })
            datum["added"] = true;
            datum["rows"] = 1;
            if (this.gridList.find(x => x.name == elementId) == null) {
                var dat =
                {
                    "name": elem.elementId,
                    "sectionId": sectionId,
                    "data": []
                };
                dat.data.push(datum)
                this.gridList.push(dat)
            } else {
                datum["rows"] = (this.gridList.find(x => x.name == elementId).data.length) + 1;
                this.gridList.find(x => x.name == elementId).data.push(datum)
            }
            DatatableService.updateDataGrid(elementId, this.gridList.find(x => x.name == elementId).data)
        }
    }

    //refresh dropdown button
    refreshDropdown(sectionId, elementId) {
        var service = this.sections.find(x => x.sectionId == sectionId);
        if (service != null) {
            var elem = service.elements.find(x => x.elementId == elementId)
            ApiService.callCustom(ApiType.POST, variable.getSourceValueByElementId, elementId, (response) => {
                if (response.data) {
                    if (response.data) {
                        DropdownService.destroyReinitDropdownFiltered("elem-" + elementId, response.data)
                        if ($("#details-container_" + elementId).length > 0) {
                            $("#details-container_" + elementId).remove();
                        }
                    }
                }

            })
        }
        
        
    }

    //delete in grid
    deleteRelation(meta, tableId) {
        this.gridList.find(x => x.name == tableId).data.splice(meta, 1);
        DatatableService.updateDataGrid(tableId, this.gridList.find(x => x.name == tableId).data);
        this.gridList.find(x => x.name == tableId).data.forEach((y, index) => {
            y.row = index + 1;
        })
    }

    //delete in attachmenet
    deleteRelationAttachment(meta, tableId) {
        tableId = tableId.replace("attachment-table-","")
        this.attachementList.find(x => x.name == tableId).data.innerelementsvalue.splice(meta, 1);
        let listTable = [];
        for (let datum of this.attachementList.find(x => x.name == tableId).data.innerelementsvalue) {
            let valKey = datum.value.split("__")
            let data = {
                id: datum.row,
                fileName: valKey[0],
                keywords: valKey.length > 1 ? valKey[1] : ""
            };
            listTable.push(data)
        }
        DatatableService.updateDataGrid("attachment-table-" + tableId, listTable);
        this.attachementList.find(x => x.name == tableId).data.innerelementsvalue.forEach((y, index) => {
            y.row = index + 1;
        })
    }

    //clear contents
    clearContents() {
        $(".div-to-change").html("")
        $(".scrollspy-to-change").html("")
    }

    //format service with datatble 
    formatDynamicDataGrid(status) {
        if (this.gridList.length > 0) {
            this.gridList.forEach(z => {
                let innerelementsvalue = [];
                if (z.data.length > 0) {
                    if ($("#FormPageId").val() == 0) {
                        let dt = this.sections.find(x => x.sectionId == z.sectionId).elements.find(y => y.elementId == z.name).innerelements;
                        z.data.forEach(dat => {
                            for (const key in dat) {
                                let data = { ...dt.find(x => x.elementId == `${key}`) };
                                if (data.hasOwnProperty('elementId')) {
                                    data.id = 0;
                                    data.parentid = data.elementId;
                                    data.column = this.tableColumnList.find(t => t.id == z.name).dataColumns.findIndex(p => p.data == data.elementId) + 1;
                                    data.elementId = 0;
                                    data.value = `${dat[key]}`;
                                    data.row = dat.rows;
                                    innerelementsvalue.push(data)
                                }

                            }

                        })
                    } else {
                        let dt = this.sections.find(x => x.sectionId == z.sectionId).elements.find(y => y.elementId == z.name).innerelementsvalue;
                        let dtE = this.sections.find(x => x.sectionId == z.sectionId).elements.find(y => y.elementId == z.name).innerelements;
                        z.data.forEach(dat => {
                            for (const key in dat) {

                                let data = { ...dt.find(x => `${key}`.includes(x.id)) };
                                if (data.hasOwnProperty('parentid')) {
                                    data.row = dat.row;
                                    innerelementsvalue.push(data)
                                } else {
                                    let datax = { ...dtE.find(x => x.elementId == `${key}`) };
                                    if (dat.rows != undefined) {
                                        if (datax.hasOwnProperty('elementId')) {
                                            datax.id = 0;
                                            datax.parentid = datax.elementId;
                                            datax.column = this.tableColumnList.find(t => t.id == z.name).dataColumns.findIndex(p => p.data == datax.elementId) + 1;
                                            datax.elementId = 0;
                                            datax.value = `${dat[key]}`;
                                            datax.row = dat.rows;
                                            innerelementsvalue.push(datax)
                                        }
                                    }

                                }

                            }

                        })
                    }

                }
                this.sections.find(x => x.sectionId == z.sectionId).elements.find(y => y.elementId == z.name)["innerelementsvalue"] = innerelementsvalue;


            })
        }

    }

    //Validate attachemnt 
    validateFileType(id, type) {
        Validation.validateFileType(id, type)
    }

    //Add File to attachement Table 
    addDataToAttachmentTable(sectionId, elementId) {
        var file = FileService.addFileForUpload("file-" + elementId, "keywords-" + elementId, elementId);
        let dt = [];
        var dat = [];
        for (let datum of this.sections.find(x => x.sectionId == sectionId).elements.find(y => y.elementId == elementId)["innerelementsvalue"]) {
            dt.push(datum)
            let DataToShow = {
                file: datum.additionalValue,
                id: datum.elementId,
                keywords: datum.value.split("__")[1],
                fileName: datum.value.split("__")[0]
            }
            dat.push(DataToShow)
        }
        let base64Value;
        var value = null;
        var filereader = new FileReader();
        filereader.readAsDataURL(file.file);
        filereader.onload = ( evt => {
            var base64 = evt.target.result;
            base64Value = base64;
            value = file.fileName + "__" + file.keywords

            dat.push(file);
            var inner = { ...this.sections.find(x => x.sectionId == sectionId).elements.find(y => y.elementId == elementId)["innerelements"][0] }
            inner["additionalValue"] = base64Value.toString();
            inner["value"] = value;
            inner["entityId"] = inner["elementId"];
            inner["parentId"] = inner["elementId"];
            inner["elementId"] = 0;
            inner["row"] = dt.length +1;
            dt.push(inner)
            this.sections.find(x => x.sectionId == sectionId).elements.find(y => y.elementId == elementId)["innerelementsvalue"] = dt;
            
            DatatableService.updateDataGrid("attachment-table-" + elementId, dat);
        })
    }

    //set valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["select-FormId"])
        if (!valid) {
            toastr.error("Please Fill In Mandatory Fields")
        }
        //return boolean id valid or not
        return valid
    }

    //save
    save(status) {
        // Set data to send to api
        let data = DefaultService.result(document.getElementById("Form-Page-AddEdit-Panel"));
        if (this.valid() && data != false) {
            //Show loader 
            Loader.showLoader("Saving Data...");
            this.formatDynamicDataGrid(status);
            data["Status"] = status;
            data["Sections"] = this.sections;
            $('#loader-common').show();
            data["FormPageId"] = parseInt(data["FormPageId"])
            if (this.Id != undefined || this.Id != null || this.Id != 0) {
                data["FormPageId"] = this.Id;
            }
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateFormPage, data, (response) => {
                //Response true
                if (response.data) {

                    //Change Condition for AType instead of StatusValues
                    if (status == StatusValues.Submitted) {
                        
                        if (this.Id == undefined || this.Id == null) {
                            this.Id = parseInt(response.additionalData)
                        }
                        if (response.aType == 1) {
                            var url = window.location.pathname + window.location.search;
                            //Open Partial for apporval
                            ApprovalPopup(this.Id, url, CaseStatus.AwaitingApproval);
                            location.href = "#partial";
                        } else if (response.aType == 4) {
                            //Redirect to listing
                            Loader.showLoader("Redirecting To Listing...");
                            this.back();
                        } else if (response.aType == 2) {
                            var url = window.location.pathname + window.location.search;
                            //Open Partial for apporval
                            ActivityPopup(this.Id, url, CaseStatus.Submitted);
                            location.href = "#partial";
                        }
                    } else if (status == StatusValues.Draft) {
                        //Redirect to listing
                        Loader.showLoader("Redirecting To Listing...");
                       this.back();
                    }

                    Loader.hideLoader();
                }
                //response false
                else if (!response.data) {
                    $('#loader-common').hide();
                    toastr.error(response.additionalData);
                }
                //failed
                else {
                    $('#loader-common').hide();
                    toastr.error("Could not open the record for edit.");
                    return false;
                }
            }, () => {

            })

        }
    }

    //Approval 
    approval(status) {
        var url = window.location.pathname + window.location.search;
        let Case = null;
        //Save Approval Status
        if (status == ApprovalStatus.Approved) {
            Case = CaseStatus.Approved;
        } else {
            Case = CaseStatus.Declined;
        }
        SaveApprovalStatus(this.Id, url, status, Case);
    }

    back() {
        const queryString = window.location.search;
        //url Parameters
        const urlParams = new URLSearchParams(queryString);
        // if url contains form
        if (urlParams.has('Back')) {
            //Populate Form
            window.location.href = urlParams.get('Back')
        } else {
            window.location.href = "/FormPage/List";
        }
    }
}
window.FormPageAddEdit = new FormPageAddEdit();