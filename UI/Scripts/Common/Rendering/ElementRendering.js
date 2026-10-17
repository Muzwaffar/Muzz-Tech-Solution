import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { SectionRendering } from "/Scripts/Common/Rendering/SectionRendering.js";
import { ScreenMode, ActivityElementType, ActivityElementSource } from "/Scripts/Common/Enums.js";
import { FileService } from "/Scripts/Common/File/FileService.js";

export class ElementRendering {//class to handle loading animation

    static renderer(data, func, id = null) {
        $(".div-to-change").html("")
        $(".scrollspy-to-change").html("")
        let elem = [];
        let scrollspyRdrs = [];
        let contents = [];
        let radios = [];
        let dropdowns = [];
        let signatures = [];
        let uomDropdowns = [];
        let dropdowndt = [];
        let multiselects = [];
        let attachments = [];
        let virtualSection = [];
        if (id == null) {
            virtualSection = data
        } else {
            virtualSection = data.filter(x => x.sectionCode.toLowerCase() == id.toLowerCase());
        }
        

        if (virtualSection != undefined) {
            for (var virtualData of virtualSection) {
                //content panel
                elem = this.elementsRenderer(virtualData.elements, virtualData, func)
               
                let content = SectionRendering.contentPanelRenderer(virtualData, elem)
                contents.push(content);

                //scroll spy rendering
                let scrollspyRdr = SectionRendering.scrollspyRenderer(virtualData);
                scrollspyRdrs.push(scrollspyRdr);

                //dropdown
                let dropdown = virtualData.elements.filter(x => ["dropdown", "dropdown with add new", "dropdown with details", "dropdown with details and add new"].includes(x.elementType.toLowerCase()));
                if (dropdown.length > 0) {
                    dropdown.forEach(dpd => {
                        dropdowns.push(dpd)
                    })
                }

                //number with UOM Dropdown
                let uomDropdown = virtualData.elements.filter(x => x.elementType.toLowerCase() == "number with uom");
                if (uomDropdown.length > 0) {
                    uomDropdown.forEach(dpd => {
                        uomDropdowns.push(dpd)
                    })
                }

                //multiselect
                let multiselect = virtualData.elements.filter(x => x.elementType.toLowerCase() == "multiselect");
                if (multiselect.length > 0) {
                    multiselect.forEach(dpd => {
                        multiselects.push(dpd)
                    })
                }

                //radio
                let radio = virtualData.elements.filter(x => x.elementType.toLowerCase() == "radio");
                if (radio.length > 0) {
                    radio.forEach(rad => {
                        radios.push(rad)
                    })
                }

                //datatable
                let datatables = virtualData.elements.filter(x => x.elementType.toLowerCase() == "datatable");
                if (datatables.length > 0) {
                    datatables.forEach(dt => {
                        dt.innerelements.forEach(dte => {
                            if (dte.elementType == "dropdown") {
                                dropdowndt.push(dte)
                            }
                            
                        })
                    })
                }

                //signature
                let signature = virtualData.elements.filter(x => x.elementType.toLowerCase() == "signature");
                if (signature.length > 0) {
                    signature.forEach(dt => {
                        dt["sectionId"] = virtualData.sectionId;
                        signatures.push(dt)
                    })
                }

                let attachmentTable = virtualData.elements.filter(x => x.elementType.toLowerCase() == "attachment");
                if (attachmentTable.length > 0) {
                    attachmentTable.forEach(dt => {
                        dt["sectionId"] = virtualData.sectionId;
                        attachments.push(dt)
                    })
                }
            }
            $(".div-to-change").html(contents);
            $(".scrollspy-to-change").html(scrollspyRdrs);
            setTimeout(
                e => {
                    //dropdown Rendering
                    if (dropdowns.length > 0) {
                        this.dropdownsRenderer(dropdowns);
                    }

                    //dropdown Rendering
                    if (multiselects.length > 0) {
                        this.multiselectsRenderer(multiselects);
                    }

                    //dropdown Rendering
                    if (dropdowndt.length > 0) {
                        this.dropdownsRenderer(dropdowndt);
                    }

                    //dropdown UOM Rendering
                    if (uomDropdowns.length > 0) {
                        this.dropdownsUOMRenderer(uomDropdowns);
                    }

                    //Signatures Rendering
                    if (signatures.length > 0) {
                        this.signatureRenderer(signatures);
                    }

                    //Attchments Rendering
                    if (attachments.length > 0) {
                        this.attachmentRenderer(attachments, func);
                    }

                    ////radio rendering
                    //if (radios.length > 0) {
                    //    this.radioRenderer(radios)
                    //};
                },
                500);

           

        }
    }

    static elementsRenderer(elements, service, func) {
        let elem = [];
        var radios = [];
        var singleElem;
        let value;
        for (var item of elements) {
            //Set Hidden
            var hiddenClass = "";
            var readonly = "";
            if (item.hidden) {
                hiddenClass = " d-none"
            }
            if (item.readOnly) {
                readonly = " readonly";
            }

            if (["text", "number", "multiline", "date", "time", "datetime", "checkbox", "checkbox with remarks","read-only text", "read-only multiline"].includes(item.elementType.toLowerCase())) {
                value = item.value;
            }

            switch (item.elementType.toLowerCase()) {
                case 'text':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                        DefaultService.createElement("input", { class: "form-control" + readonly, type: item.elementType, placeholder: item.elementName, value: value, id: item.elementId, onfocusout: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, []),
                        DefaultService.createElement("label", {}, {}, { innerHTML: item.elementName })
                    ]);
                    break;

                case 'read-only text':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                        DefaultService.createElement("input", { class: "form-control" + readonly, type: item.elementType, placeholder: item.elementName, value: value, id: item.elementId, readonly:true, onfocusout: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, []),
                        DefaultService.createElement("label", {}, {}, { innerHTML: item.elementName })
                    ]);
                    break;

                case 'number':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                        DefaultService.createElement("input", { class: "form-control" + readonly, type: item.elementType, placeholder: item.elementName, value: parseFloat(value), id: item.elementId, onfocusout: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, []),
                        DefaultService.createElement("label", {}, {}, { innerHTML: item.elementName })
                    ]);
                    break;

                case 'multiline':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                        DefaultService.createElement("textarea", { class: "form-control" + readonly, placeholder: item.elementName, value: value, id: item.elementId, onfocusout: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, {}, { innerHTML: value }),
                        DefaultService.createElement("label", {}, {}, { innerHTML: item.elementName })
                    ]);
                    break;

                case 'read-only multiline':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                        DefaultService.createElement("textarea", { class: "form-control" + readonly, placeholder: item.elementName, value: value, id: item.elementId, readonly: true, onfocusout: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, {}, { innerHTML: value }),
                        DefaultService.createElement("label", {}, {}, { innerHTML: item.elementName })
                    ]);
                    break;

                case 'date':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                        DefaultService.createElement("input", { class: "form-control" + readonly, type: item.elementType, placeholder: item.elementName, value: value, id: item.elementId, onfocusout: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, []),
                        DefaultService.createElement("label", {}, {}, { innerHTML: item.elementName })
                    ]);
                    break;

                case 'time':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                        DefaultService.createElement("input", { class: "form-control" + readonly, type: item.elementType, placeholder: item.elementName, value: value, id: item.elementId, onfocusout: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, []),
                        DefaultService.createElement("label", {}, {}, { innerHTML: item.elementName })
                    ]);
                    break;

                case 'datetime':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                        DefaultService.createElement("input", { class: "form-control" + readonly, type: "datetime-local", placeholder: item.elementName, value: value, id: item.elementId, onfocusout: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, []),
                        DefaultService.createElement("label", {}, {}, { innerHTML: item.elementName })
                    ]);
                    break;

                case 'checkbox':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                        DefaultService.createElement("input", { class: "form-control-checkbox" + readonly, type: item.elementType, placeholder: item.elementName, value: value, id: item.elementId, onfocusout: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, []),
                        DefaultService.createElement("label-fp", { class:"label-checkbox"}, {}, { innerHTML: item.elementName })
                    ]);
                    break;
                case 'checkbox with remarks':
                    singleElem = DefaultService.createElement("div", { class: "d-flex w-100" + hiddenClass }, [
                        DefaultService.createElement("div", { class: "form-floating  m-auto" }, [
                            DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                                DefaultService.createElement("input", { class: "form-control-checkbox" + readonly, type: "checkbox", placeholder: item.elementName, value: value, id: item.elementId, onfocusout: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, []),
                                DefaultService.createElement("label-fp", { class: "label-checkbox" }, {}, { innerHTML: item.elementName })
                            ]),
                        ]),
                        DefaultService.createElement("div", { class: "d-flex w-100" }, [
                            DefaultService.createElement("div", { class: "form-floating" + readonly }, [
                                DefaultService.createElement("textarea", { class: "form-control" + readonly, placeholder: "Remarks", value: item.additionalValue, id: "remarks" + item.elementId, onfocusout: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'text',$(event.target), true)" }, []),
                                DefaultService.createElement("label", {}, {}, { innerHTML: "Remarks" })
                            ])
                        ])
                    ]);
                    break;

                case 'radio':
                    
                    //singleElem = DefaultService.createElement("div", { class: "form-floating form-control-radio", id: item.elementId  }, [
                    //    DefaultService.createElement("label", { class: "radio-label" }, {}, { innerHTML: item.elementName })
                    //]);


                    break;

                case 'dropdown':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass + readonly }, [
                        DefaultService.createElement("input", { class: "d-none", id: item.elementId + "-id" }, []),
                        DefaultService.createElement("select", { class: "form-select" , id:"elem-" + item.elementId, name: item.elementId, onchange: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, [
                            DefaultService.createElement("option", { value: "" }, [])
                        ]),
                        DefaultService.createElement("label", { for: "elem-" + item.elementId }, {}, { innerHTML: item.elementName })
                    ]);

                    break;

                case 'dropdown with details':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass + readonly }, [
                        DefaultService.createElement("input", { class: "d-none", id: item.elementId + "-id" }, []),
                        DefaultService.createElement("select", { class: "form-select", id: "elem-" + item.elementId, name: "elem-" + item.elementId, onchange: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))"}, [
                            DefaultService.createElement("option", { value: "" }, [])
                        ]),
                        DefaultService.createElement("label", { for: "elem-" + item.elementId }, {}, { innerHTML: item.elementName })
                    ]);

                    break;

                case 'dropdown with details and add new':
                    singleElem = DefaultService.createElement("div", { class: "d-flex w-100" + hiddenClass }, [
                        DefaultService.createElement("div", { class: "form-floating" + hiddenClass + readonly }, [
                            DefaultService.createElement("input", { class: "d-none", id: item.elementId + "-id" }, []),
                            DefaultService.createElement("select", { class: "form-select", id: "elem-" + item.elementId, name: "elem-" + item.elementId, onchange: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, [
                                DefaultService.createElement("option", { value: "" }, [])
                            ]),
                            DefaultService.createElement("label", { for: "elem-" + item.elementId }, {}, { innerHTML: item.elementName })
                        ]),
                        DefaultService.createElement("div", { class: "d-flex m-3 mt-4 naveo-color pt-3" }, [
                            DefaultService.createElement("a", { class: readonly, onclick: func + ".refreshDropdown(" + service.sectionId + "," + item.elementId + ")" }, [
                                DefaultService.createElement("i", { class: "fa-solid fa-arrows-rotate fa-16" }, [], {})
                            ], {})
                        ]),
                        DefaultService.createElement("div", { class: "d-flex my-3 mt-4 naveo-color pt-3" }, [
                            DefaultService.createElement("a", { class: readonly, href: item.url, target: "_blank" }, [
                                DefaultService.createElement("i", { class: "fa fa-plus fa-16" }, [], {})
                            ], {})
                        ])
                    ]);

                    break;

                case 'multiselect':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass + readonly }, [
                        DefaultService.createElement("select", { class: "form-select" , id: item.elementId, name: item.elementId, multiple: "multiple" }, []),
                        DefaultService.createElement("label", { class: "label-multiselect" }, {}, { innerHTML: item.elementName })
                    ]);
                    break;

                case 'label':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                        DefaultService.createElement("span", { class:"f-weight-bold" }, {}, { innerHTML: item.elementName })
                    ]);
                    break;

                case 'datatable':
                    var DTElems = [];
                    DTElems.push(
                        DefaultService.createElement("hr", { class: "my-5 border-top-dark-grey" }, {}, {})
                    )
                    item.innerelements.forEach(x => {
                        if (x.elementType.toLowerCase() == "text") {
                            var DTElem = DefaultService.createElement("div", { class: "form-floating" }, [
                                DefaultService.createElement("input", { class: "form-control" , type: "text", placeholder: x.elementName, id: x.elementId }, []),
                                DefaultService.createElement("label", {}, {}, { innerHTML: x.elementName })
                            ])
                            DTElems.push(DTElem)
                        } else if (x.elementType.toLowerCase() == "number") {
                            var DTElem = DefaultService.createElement("div", { class: "form-floating" }, [
                                DefaultService.createElement("input", { class: "form-control" , type: "number", placeholder: x.elementName, id: x.elementId }, []),
                                DefaultService.createElement("label", {}, {}, { innerHTML: x.elementName })
                            ])
                            DTElems.push(DTElem)
                        } else if (x.elementType.toLowerCase() == "dropdown") {
                            var DTElem = DefaultService.createElement("div", { class: "form-floating" + readonly }, [
                                DefaultService.createElement("input", { class: "d-none", id: x.elementId + "-id" }, []),
                                DefaultService.createElement("select", { class: "form-select", id: "elem-" + x.elementId, name: "elem-" +  x.elementId }, [
                                    DefaultService.createElement("option", { value: "" }, [])
                                ]),
                                DefaultService.createElement("label", { for: "elem-" + x.elementId }, {}, { innerHTML: x.elementName })
                            ]);
                            DTElems.push(DTElem)
                        } else if (x.elementType.toLowerCase() == "date") {
                            var DTElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                                DefaultService.createElement("input", { class: "form-control", type: "date", placeholder: x.elementName, id: x.elementId}, []),
                                DefaultService.createElement("label", {}, {}, { innerHTML: x.elementName })
                            ]);
                            DTElems.push(DTElem)
                        } else if (x.elementType.toLowerCase() == "datetime") {
                            var DTElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                                DefaultService.createElement("input", { class: "form-control", type: "datetime-local", placeholder: x.elementName, value: value, id: x.elementId }, []),
                                DefaultService.createElement("label", {}, {}, { innerHTML: x.elementName })
                            ]);
                            DTElems.push(DTElem)
                        }
                    })
                    DTElems.push(
                        DefaultService.createElement("button", { class: "btn-c-primary form-control mt-4 w-100" + readonly, id: "btn_" + service.sectionId, onclick: func + ".addToGrid('" + service.sectionId + "','" + item.elementId + "')" }, {}, { innerHTML: "<i class='fa fa-plus me-2'></i>Add To Table" }),
                        DefaultService.createElement("table", { id: item.elementId, class: "mt-4" }, {}, {}),
                        DefaultService.createElement("hr", { class: "my-5 border-top-dark-grey" }, {}, {})
                    )
                    singleElem = DefaultService.createElement("div", { class: "w-100" + hiddenClass },
                        DTElems
                    )

                    let dropdowns = item.innerelements.filter(x => x.elementType.toLowerCase() == "dropdown");
                    this.dropdownsRenderer(dropdowns);

                    break;


                case 'dropdown with add new':

                    singleElem = DefaultService.createElement("div", { class: "d-flex w-100" + hiddenClass }, [
                        DefaultService.createElement("div", { class: "form-floating" + readonly }, [
                            DefaultService.createElement("input", { class: "d-none", id: item.elementId + "-id" }, []),
                            DefaultService.createElement("select", { class: "form-select", id: "elem-" + item.elementId, name: item.elementId, onchange: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, [
                                DefaultService.createElement("option", { value: "" }, [])
                            ]),
                            DefaultService.createElement("label", { for: "elem-" +  item.elementId }, {}, { innerHTML: item.elementName })
                        ]),
                        DefaultService.createElement("div", { class: "align-items-end d-flex m-3 naveo-color" }, [
                            DefaultService.createElement("a", { class: readonly, onclick: func + ".refreshDropdown(" + service.sectionId + "," + item.elementId + ")" }, [
                                DefaultService.createElement("i", { class: "fa-solid fa-arrows-rotate fa-16" }, [], {})
                            ], {})
                        ]),
                        DefaultService.createElement("div", { class: "align-items-end d-flex my-3 naveo-color" }, [
                            DefaultService.createElement("a", { class: readonly, href: item.url, target: "_blank" }, [
                                DefaultService.createElement("i", { class: "fa fa-plus fa-16" }, [], {})
                            ], {})
                        ])
                    ]);

                    break;


                case 'number with uom':
                    singleElem = DefaultService.createElement("div", { class: "d-flex w-100" + hiddenClass }, [
                        DefaultService.createElement("div", { class: "form-floating pe-3" }, [
                            DefaultService.createElement("input", { class: "form-control" + readonly, type: "number", placeholder: item.elementName, value: item.value, id: item.elementId, onfocusout: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target))" }, []),
                            DefaultService.createElement("label", {}, {}, { innerHTML: item.elementName })
                        ]),
                        DefaultService.createElement("div", { class: "d-flex w-100" }, [
                            DefaultService.createElement("div", { class: "form-floating" + readonly }, [
                                DefaultService.createElement("input", { class: "d-none", id: item.elementId + "-uom-id" }, []),
                                DefaultService.createElement("select", { class: "form-select" , id: item.elementId + "-uom", name: item.elementId + "-uom", onchange: func + ".updateValueRender(" + service.sectionId + "," + item.elementId + ",'" + item.elementType + "',$(event.target), 'uom')" }, [
                                    DefaultService.createElement("option", { value: "" }, [])
                                ]),
                                DefaultService.createElement("label", { for: item.elementId + "-uom" }, {}, { innerHTML: "Unit Of Measure" })
                            ])
                        ])
                    ]);

                    break;

                case 'attachment':
                    singleElem = DefaultService.createElement("div", { class: "w-100 mt-4 " + hiddenClass }, [
                        DefaultService.createElement("div", { class: "w-100" }, [
                            DefaultService.createElement("input", { class: "form-control p-3", type: "file", id: "file-" + item.elementId, onchange: func +".validateFileType('" + "file-" + item.elementId +"', 3)" }, []),
                            DefaultService.createElement("span", { class: "naveo-color span-smaller" }, {}, { innerHTML: "Accepted file extensions: .doc, .csv, .docx, .pdf, .rtf, .txt, .zip, .gz, .rar, .xls, .xlsx, .ppt, .pptx, .jpeg, .jpg, .bmp, .png" })
                        ]),
                        DefaultService.createElement("div", { class: "form-floating" }, [
                            DefaultService.createElement("input", { class: "form-control", type: "text", id: "keywords-" + item.elementId, placeholder: "Keywords" }),
                            DefaultService.createElement("label", {}, {}, { innerHTML: "Keywords" }),
                            DefaultService.createElement("span", { class: "naveo-color span-smaller" }, {}, { innerHTML: "Delimeter ;" })
                        ]),
                        DefaultService.createElement("button", { class: "form-floating", id: "add-to-attachment" + item.elementId, title: "Add To Attachment Table", class: "btn-c-primary form-control mt-4 w-100", onclick: func + ".addDataToAttachmentTable(" + service.sectionId + "," + item.elementId + ")" }, [
                            DefaultService.createElement("i", { class: "fa fa-plus me-2" }, {})
                        ], { innerHTML: "Add Data To Table" }),
                        DefaultService.createElement("div", { class: "w-100" }, [
                            DefaultService.createElement("table", { id: "attachment-table-" + item.elementId }, {}, {})
                        ]),
                    ]);
                    break;

                case 'map':
                    break;

                case 'signature':
                    var disabled = "";
                    var remove = "";
                    // check for edit from url parameters
                    if (this.screenMode() == ScreenMode.EDIT && item.value != "") {
                        disabled = " readonly";
                        remove = " d-none";
                    }
                    singleElem = DefaultService.createElement("div", { class: "form-floating" + hiddenClass }, [
                        DefaultService.createElement("div", { class: "wrapper-signature mt-3" + disabled }, [
                            DefaultService.createElement("canvas", { class: "signature-pad w-100", type: item.elementType, id: "signature-pad" + item.elementId }, [])
                        ]),
                        DefaultService.createElement("div", { class: "d-flex" }, [
                            DefaultService.createElement("button", { class: "btn-c-secondary form-control mt-1 w-100" + remove, id: "clear_" + item.elementId + "_" + service.sectionId }, [], { innerHTML: "Clear Signature" })
                        ])
                    ]);
                   
                    break;


                default:
                    console.info("Below element cannot be rendered:\n Element Id: " + item.elementId + "\n Element Name: " + item.elementName + "\n Element Type: " + item.elementType)
                    break;
            }
            elem.push(singleElem)
        }
       

        return elem;
    }

    static dropdownsRenderer(dropdowns = []) {
        dropdowns.forEach(dpd => {
            DropdownService.initDropdownFiltered("elem-" + dpd.elementId, dpd.sourceValues);
            let value = dpd.sourceValues.length > 0 ? dpd.sourceValues[0] : "";
            $("#" + dpd.elementId + "-id").val(value)
            //Set Dropdown Value on edit 
            $("#" + "elem-" + dpd.elementId).val(dpd.value).trigger('change');
        })
    }

    static dropdownsUOMRenderer(dropdowns = []) {
        dropdowns.forEach(dpd => {
            DropdownService.initDropdownFiltered(dpd.elementId + "-uom", dpd.uomlist);
            let additionalValue = dpd.uomlist.length > 0 ? dpd.uomlist[0] : "";
            $("#" + dpd.elementId + "-uom-id").val(additionalValue)
            //Set Dropdown Value on edit 
            $("#" + dpd.elementId + "-uom").val(dpd.additionalValue).trigger('change');
        })
    }

    static multiselectsRenderer(multiselects) {
        multiselects.forEach(mtsl => {
            DropdownService.initMultiDropdownFiltered(mtsl.elementId, mtsl.sourceValues);
            $("#" + mtsl.elementId).val(mtsl.value).trigger('change');
        })
    }

    static radioRenderer(radios = []) {
        for (var item of radios) {
            item.list.forEach(rad => {
                
                const radio = DefaultService.createElement("div", { class: "form-check" }, [
                    DefaultService.createElement("input", { class: "form-check-input", type: item.elementType, name: item.elementId, id: "radio-" + rad.id }, []),
                    DefaultService.createElement("label", { class: "form-check-label", for: "radio-" + rad.id }, {}, { innerHTML: rad.text })
                ])
                document.getElementById(item.elementId).appendChild(radio);
                let value = item.sourceValues.length > 0 ? item.sourceValues[0] : "";
                if (value == rad.id) {
                    $("#radio-" + rad.id).prop("checked", "checked");
                }
            })
        }
    }

    static attachmentRenderer(attachments = [], func) {
        attachments.forEach(atcs => {
            // Init Attachement Dtatable
            FileService.initialiseFileTable("attachment-table-" + atcs.elementId, "FormPageAddEdit.deleteRelationAttachment", true, true);
        })
    }

    static signatureRenderer(signatures = []) {
        var canvas = [];
        var signaturePad = [];
        signatures.forEach(function (item, i) {
            canvas[i] = document.getElementById('signature-pad' + item.elementId);
            window.onresize = ElementRendering.resizeCanvas;
            ElementRendering.resizeCanvas(canvas[i]);
            signaturePad[i] = new SignaturePad(canvas[i], {
                backgroundColor: 'rgb(245 245 245)'
            });
            //on edit - show signature
            signaturePad[i].fromDataURL(item.additionalValue);
            document.getElementById('clear_' + item.elementId + "_"+ item.sectionId).addEventListener('click', e=> {
                signaturePad[i].clear();
                FormPageAddEdit.updateValueRender(item.sectionId, item.elementId, item.elementType, null, null, "signature");
            });

            signaturePad[i].addEventListener("endStroke", (e) => {
                FormPageAddEdit.updateValueRender(item.sectionId, item.elementId, item.elementType, signaturePad[i].toDataURL("image/png"), null, "signature");
            });
        })
    }

    // Adjust canvas coordinate space taking into account pixel ratio,
    // to make it look crisp on mobile devices.
    // This also causes canvas to be cleared.
    static resizeCanvas(canvas) {
        // When zoomed out to less than 100%, for some very strange reason,
        // some browsers report devicePixelRatio as less than 1
        // and only part of the canvas is cleared then.
        var ratio = Math.max(window.devicePixelRatio || 1, 1);
        canvas.width = canvas.offsetWidth * ratio;
        canvas.height = canvas.offsetHeight * ratio;
        canvas.getContext("2d").scale(ratio, ratio);
    }

    static dropdownNaaDetailsRenderer(response, elementId) {
        let rendered = []
        response.forEach(datum => {

            if (!["text", "id"].includes(datum.key.toLowerCase())) {
                rendered.push(this.dropdownDetailsBuilder(datum.key, datum.value))
            }
                
        })
        let div = DefaultService.createElement("div", { class: "d-flex flex-column my-3", id: "details-container_" + elementId }, [
            DefaultService.createElement("div", { }, [
                DefaultService.createElement("span", { class: "f-weight-bold ms-2 naveo-color span-medium" }, [], { innerHTML: "Details :" })
            ], {}),
            DefaultService.createElement("div", {}, rendered , {})
        ], {})
        return div;
    }

    static dropdownDetailsBuilder(name, data) {
        let span = null;
        if (data != "") {
            // returns span
            span = DefaultService.createElement("div", { class: "" }, [
                DefaultService.createElement("span", { class: "span-small ms-2 f-weight-bold" }, [], { innerHTML: name + " : "  }),
                DefaultService.createElement("span", { class: "span-small ms-2" }, [], { innerHTML: data })
            ], {});
        }
       
        return span;
    }

    static screenMode() {
        if (new URLSearchParams(window.location.search).has('Id')) {
            return ScreenMode.EDIT
        } else {
            return ScreenMode.VIEW
        }
    }

    static dynamicElementsRenderer(containerId, data) {
        $("#" + containerId).html(""); 
        let Elements = [];
        let Dropdowns = [];

        data.forEach((datum, index) => {
            let singleElem;
            var type = Object.keys(ActivityElementType).find(name => ActivityElementType[name] === datum.type)
            switch (type.toLowerCase()) {
                case 'text':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" }, [
                        DefaultService.createElement("input", { class: "form-control", type: "text", placeholder: datum.labelName, id: datum.fieldId }, []),
                        DefaultService.createElement("label", {}, {}, { innerHTML: datum.labelName })
                    ]);
                    break;
                case 'number':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" }, [
                        DefaultService.createElement("input", { class: "form-control", type: "text", placeholder: datum.labelName, id: datum.fieldId }, []),
                        DefaultService.createElement("label", {}, {}, { innerHTML: datum.labelName })
                    ]);
                    break;
                case 'dropdown':
                    singleElem = DefaultService.createElement("div", { class: "form-floating" }, [
                        DefaultService.createElement("select", { class: "form-select", id: datum.fieldId, name: datum.fieldId }, [
                            DefaultService.createElement("option", { value: "" }, [])
                        ]),
                        DefaultService.createElement("label", { for: datum.fieldId }, {}, { innerHTML: datum.labelName })
                    ]);
                    Dropdowns.push(datum)
                    break;
                default:
                    console.info("Below element cannot be rendered:\n " + datum)
                    break;
            }

            Elements.push(singleElem);
            
        });
        $("#" + containerId).html(Elements);
        if (Dropdowns.length > 0) {
            this.dynamicDropdownRenderer(Dropdowns)
        }
    }

    static dynamicDropdownRenderer(dropdowns =[]) {
        dropdowns.forEach(dpd => {
            DropdownService.initDropdownNull(dpd.fieldId);
        })
    }
}
window.ElementRendering = new ElementRendering();
