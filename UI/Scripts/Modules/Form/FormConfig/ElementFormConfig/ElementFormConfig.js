import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
export class ElementFormConfig {//class that handles the Element Form Configuration
    ////Global Variable : 
    data = null;
    constructor() {
        this.dropdownValues();
    }

    //Set Dropdown list of values
    dropdownValues() {
        var dropdown = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getAllDropDownValuesEFC, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {
                    // Set Form list of values
                    DropdownService.initDropdown("select-form", dropdown, "Form");
                }
            }
        });
        ApiService.callCustom(ApiType.POST, variable.getAllDropDownValuesMenuEFC, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {
                    // Set Menu list of values
                    DropdownService.initDropdown("select-menu", dropdown, "Menu");
                }
            }
        });
    }

    //retrieve data for form and module id if exist
    retrieve() {
        if ($('#select-menu').val() == "" || $('#select-form').val() == "") {
            toastr.error("Please Select Both A Menu And A Form");
        } else {
            let dataToSend = {
                FormId: $('#select-form').val(),
                ModuleId: $('#select-menu').val()
            }
            ApiService.callCustom(ApiType.POST, variable.getElementFormConfig, dataToSend, (response) => {
                if (response) {
                    var a = response.data;
                    this.data = response.data;
                    $("#retrived-data").html(""); // Clear previous content
                    let dat = [];
                    let sequence = DefaultService.createElement("div", { class: "form-floating" }, [
                        DefaultService.createElement("input", { class: "form-control", id: "sequence", type: "number", value: this.data.sequence, placeholder: "Sequence" }, [], {}),
                        DefaultService.createElement("label", {}, [], { innerHTML: "Sequence" })
                    ]);

                    let labels = DefaultService.createElement("div", { class: "mt-4 d-flex justify-content-end" }, [
                        DefaultService.createElement("span", { class: "mx-3 f-weight-bold" }, [], { innerHTML: "Readonly" }),
                        DefaultService.createElement("span", { class: "mx-3 f-weight-bold" }, [], { innerHTML: "Hidden" })
                    ]);

                    let sectionContainer = DefaultService.createElement("div", { class: "mt-3 h-65-scroll" }, []);

                    this.data.sections.forEach(sect => {

                        let sectionElements = []
                        sect.elements.forEach(elems => {
                            let ReadonlyE = DefaultService.createElement("input", { class: "form-control-checkbox mx-4 p-0", type: "checkbox", id: "readonly-element-" + elems.elementId, onclick: "ElementFormConfig.updateValuesElement(" + elems.elementId + "," + sect.sectionId + ",'readonly',$(event.target))" }, [], {})
                            let HiddenE = DefaultService.createElement("input", { class: "form-control-checkbox mx-5 p-0", type: "checkbox", id: "hidden-element-" + elems.elementId, onclick: "ElementFormConfig.updateValuesElement(" + elems.elementId + "," + sect.sectionId + ",'hidden',$(event.target))" }, [], {})

                            if (elems.readOnly) {
                                ReadonlyE = DefaultService.createElement("input", { class: "form-control-checkbox mx-4 p-0", type: "checkbox", checked: true, id: "readonly-element-" + elems.elementId, onclick: "ElementFormConfig.updateValuesElement(" + elems.elementId + "," + sect.sectionId + ",'readonly',$(event.target))" }, [], {})
                            }
                            if (elems.hidden) {
                                HiddenE = DefaultService.createElement("input", { class: "form-control-checkbox mx-5 p-0", type: "checkbox", checked: true, id: "hidden-element-" + elems.elementId, onclick: "ElementFormConfig.updateValuesElement(" + elems.elementId + "," + sect.sectionId + ",'hidden',$(event.target))" }, [], {})
                            }
                            let elem = DefaultService.createElement("div", { class: "d-flex justify-content-between align-items-center element", id: "element-" + elems.elementId }, [
                                DefaultService.createElement("div", { class: "f-weight-bold m-1" }, [
                                    DefaultService.createElement("span", { class: "elemName" }, [], { innerHTML: elems.elementName })
                                ]),
                                DefaultService.createElement("div", { class: "d-flex" }, [
                                    ReadonlyE,
                                    HiddenE
                                ])
                            ]);
                            sectionElements.push(elem);

                        })
                        let ReadonlyS = DefaultService.createElement("input", { class: "form-control-checkbox mx-4 p-0", type: "checkbox", id: "readonly-section-" + sect.sectionId, onclick: "ElementFormConfig.updateValuesSection(" + sect.sectionId + ",'readonly',$(event.target))" }, [], {})
                        let HiddenS = DefaultService.createElement("input", { class: "form-control-checkbox mx-5 p-0", type: "checkbox", id: "hidden-section-" + sect.sectionId, onclick: "ElementFormConfig.updateValuesSection(" + sect.sectionId + ",'hidden',$(event.target))" }, [], {})
                        if (sect.readOnly) {
                            ReadonlyS = DefaultService.createElement("input", { class: "form-control-checkbox mx-4 p-0", type: "checkbox", checked: true, id: "readonly-section-" + sect.sectionId, onclick: "ElementFormConfig.updateValuesSection(" + sect.sectionId + ",'readonly',$(event.target))" }, [], {})
                        }
                        if (sect.hidden) {
                            HiddenS = DefaultService.createElement("input", { class: "form-control-checkbox mx-5 p-0", type: "checkbox", checked: true, id: "hidden-section-" + sect.sectionId, onclick: "ElementFormConfig.updateValuesSection(" + sect.sectionId + ",'hidden',$(event.target))" }, [], {})
                        }


                        let section = DefaultService.createElement("div", { class: "mb-2", id: "section-" + sect.sectionId }, [
                            DefaultService.createElement("div", { class: "section w-100 mb-2" }, [
                                DefaultService.createElement("div", { class: "section-secondary", 'data-bs-toggle': "collapse", 'aria-expanded': "true", href: "#elements-section-" + sect.sectionId }, [
                                    DefaultService.createElement("span", { class: "collapsible-icon" }, [], {}),
                                    DefaultService.createElement("span", { class: "ms-3 span-medium secName" }, [], { innerHTML: sect.sectionName })
                                ], {}),
                                DefaultService.createElement("div", {}, [
                                    ReadonlyS,
                                    HiddenS
                                ], {})
                            ], {}),
                            DefaultService.createElement("div", { class: "collapse show", id: "elements-section-" + sect.sectionId }, sectionElements, {})
                        ]);
                        sectionContainer.appendChild(section)
                    })

                    dat.push(sequence)
                    dat.push(labels)
                    dat.push(sectionContainer)

                    $("#retrived-data").html(dat);
                }
                else {
                    toastr.error("Error while retrieving. Please contact Admin");
                }
            });
        }
    }

    updateValuesElement(id, sectionId, action, e) {
        if (action == "hidden") {
            this.data.sections.find(x => x.sectionId == sectionId).elements.find(y => y.elementId == id).hidden = e[0].checked;
        } else if (action == "readonly") {
            this.data.sections.find(x => x.sectionId == sectionId).elements.find(y => y.elementId == id).readonly = e[0].checked;
        }
    }


    updateValuesSection(id, action, e) {
        if (action == "hidden") {
            this.data.sections.find(x => x.sectionId == id).hidden = e[0].checked;
        } else if (action == "readonly") {
            this.data.sections.find(x => x.sectionId == id).readonly = e[0].checked;
        }
    }

    save() {
        if (this.data == null) {
            toastr.error("Please Retrieve Data First Before Saving");
        } else {
            let sectionsArray = [];

            $(".section").each(function () {
                let sectionId = $(this).parent().attr('id').replace("section-", "");
                let sectionName = $(this).find(".secName").html();
                let readOnlySection = $(this).find(".form-control-checkbox[onclick*='ElementFormConfig.updateValuesSection'][id*='readonly-section-" + sectionId + "']").prop("checked");
                let hiddenSection = $(this).find(".form-control-checkbox[onclick*='ElementFormConfig.updateValuesSection'][id*='hidden-section-" + sectionId + "']").prop("checked");

                let sectionObj = {
                    sectionId: parseInt(sectionId),
                    sectionName: sectionName,
                    readOnly: readOnlySection,
                    hidden: hiddenSection,
                    elements: []
                };
                $(this).parent().find(".element").each(function () {
                    let elementId = $(this).attr('id').replace("element-", "");
                    let elementName = $(this).find(".elemName").html();
                    let readOnlyElement = $(this).find(".form-control-checkbox[onclick*='ElementFormConfig.updateValuesElement'][id*='readonly-element-" + elementId + "']").prop("checked");
                    let hiddenElement = $(this).find(".form-control-checkbox[onclick*='ElementFormConfig.updateValuesElement'][id*='hidden-element-" + elementId + "']").prop("checked");

                    let elementObj = {
                        elementId: parseInt(elementId),
                        elementName: elementName,
                        readOnly: readOnlyElement,
                        hidden: hiddenElement
                    };

                    sectionObj.elements.push(elementObj);
                });

                sectionsArray.push(sectionObj);
            });

            let dataToSave = {
                sequence: $("#sequence").val(),
                sections: sectionsArray,
                formId: $("#select-form").val(),
                moduleId: $("#select-menu").val()
            };
            ApiService.callCustom(ApiType.POST, variable.createUpdateElementFormConfig, dataToSave, (data) => {
                if (data) {
                    toastr.success("Saved Successfully!")
                    window.location.reload();
                }//response false
                else if (!response.data) {
                    toastr.error(response.additionalData);
                }
                //failed
                else {
                    toastr.error("Could not open the record for edit.");
                    return false;
                }
            });
        }
    }

    cancel() {
        this.data = null;
        $("#retrived-data").html(""); 
    }
}
window.ElementFormConfig = new ElementFormConfig();