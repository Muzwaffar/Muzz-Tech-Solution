import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { SectionRendering } from "/Scripts/Common/Rendering/SectionRendering.js";
import { ElementRendering } from "/Scripts/Common/Rendering/ElementRendering.js";
export class Template {//class that handles the Template screen
    screenID;
    constructor() {
        //Data Single Dropdown
        var dat = [
            {
                "title": "Dropdown",
                "items": [
                    {
                        "id": 1,
                        "text": "Value 1",
                        "additional": "",
                        "type": "",
                        "lot": "",
                        "qty": null,
                        "isAvailable": null,
                        "isSelected": false
                    },
                    {
                        "id": 2,
                        "text": "Value 2",
                        "additional": "",
                        "type": "",
                        "lot": "",
                        "qty": null,
                        "isAvailable": null,
                        "isSelected": false
                    }
                ]
            }
        ];
        //screenId
        this.screenID = 'SEC001';

        var section = [{
            sectionCode: "SEC001",
            sectionName: "Program of Work",
            sectionStatus: 3 //1 Not Started // 2 Draft // 3 Complated
        }, {
            sectionCode: "SEC002",
            sectionName: "Methodology",
            sectionStatus: 2 //1 Not Started // 2 Draft // 3 Complated
        }, {
            sectionCode: "SEC003",
            sectionName: "Attachment",
            sectionStatus: 1 //1 Not Started // 2 Draft // 3 Complated
        }]


        $(document).ajaxComplete(() => {
            SectionRendering.sectionRenderer(section)
            if (dat != null) {
                //Title Select
                DropdownService.initDropdown("select-dropdown", dat, "Dropdown");
                //Set Dropdown Value on edit 
                $("#select-dropdown").val($("#select-dropdown-id").val()).trigger('change');

                DropdownService.initDropdown("select-dropdown-addnew", dat, "Dropdown");
                //Set Dropdown Value on edit 
                $("#select-dropdown").val($("#select-dropdown-id").val()).trigger('change');

                DropdownService.initMultiDropdown("select-multiselect", dat, "Dropdown");
            }

        });
    }

    refresh(e) {
        console.log(e)
    }

    changeView(id) {
        if (id == this.screenID) {
            toastr.info("Already On this Page")
        } else {
            $(".section-menu").each(function (i, obj) {
                obj.classList.remove("active-section")
            });
            $("#" + id).addClass("active-section");

            Loader.showLoader("Rendering and Loading Data...");
            this.screenID = id;

            let data = [{
                sectionCode: "SEC002",
                subSectionCode: "MET001",
                subSectionName: "General",
                elems: [{
                    elementTypeName: "text",
                    htmlType: "text",
                    elementId: "ELE0001",
                    elementName: "Label text",
                    listValue: ["Field Value"]
                },
                {
                    elementTypeName: "number",
                    htmlType: "number",
                    elementId: "ELE0002",
                    elementName: "Label Number",
                    listValue:[ "2"]
                }, {
                    elementTypeName: "multiline",
                    htmlType: "multiline",
                    elementId: "ELE0003",
                    elementName: "Label Multiline",
                    listValue: ["Example Text for multiline"]
                }, {
                    elementTypeName: "date",
                    htmlType: "date",
                    elementId: "ELE0004",
                    elementName: "Label Date",
                    listValue: ["2023-09-14"]
                }, {
                    elementTypeName: "time",
                    htmlType: "time",
                    elementId: "ELE0005",
                    elementName: "Label Time",
                    listValue: ["13:00:00"]
                }, {
                    elementTypeName: "datetime",
                    htmlType: "datetime-local",
                    elementId: "ELE0006",
                    elementName: "label Date Time",
                    listValue: ["2023-09-14T23:00:00"]
                }, {
                    elementTypeName: "checkbox",
                    htmlType: "checkbox",
                    elementId: "ELE0007",
                    elementName: "Label Checkbox",
                    listValue: ["true"]
                }, {
                    elementTypeName: "radio",
                    htmlType: "radio",
                    elementId: "ELE0008",
                    elementName: "Label Radio",
                    listValue: ["2"],
                    list: [
                        {
                            "id": 1,
                            "text": "Radio 1",
                        },
                        {
                            "id": 2,
                            "text": "Radio 2",
                        },
                        {
                            "id": 3,
                            "text": "Radio 3",
                        }
                    ]
                }, {
                    elementTypeName: "dropdown",
                    htmlType: "dropdown",
                    elementId: "ELE0019",
                    elementName: "Label Dropdown",
                    listValue:[ "2"],
                    list: [
                        {
                            "id": 1,
                            "text": "Value 1 Dynamic",
                        },
                        {
                            "id": 2,
                            "text": "Value 2 Dynamic",
                        }
                    ]
                }, {
                    elementTypeName: "dropdown",
                    htmlType: "dropdown",
                    elementId: "ELE0018",
                    elementName: "Label Dropdown",
                    listValue: ["2"],
                    list: [
                        {
                            "id": 1,
                            "text": "Value 1 Dynamic",
                        },
                        {
                            "id": 2,
                            "text": "Value 2 Dynamic",
                        }
                    ]
                }, {
                    elementTypeName: "multiselect",
                    htmlType: "multiselect",
                    elementId: "ELE0028",
                    elementName: "Label multiselect",
                    listValue:["2", "3"],
                    list: [
                        {
                            "id": 1,
                            "text": "Value 1 Dynamic",
                        },
                        {
                            "id": 2,
                            "text": "Value 2 Dynamic",
                        },
                        {
                            "id": 3,
                            "text": "Value 3 Dynamic",
                        },
                        {
                            "id": 4,
                            "text": "Value 4 Dynamic",
                        }
                    ]
                }]
            }, {
                sectionCode: "SEC003",
                subSectionCode: "MET002",
                subSectionName: "Attachment",
                elems: [{
                    elementTypeName: "text",
                    htmlType: "text",
                    elementId: "ELE003",
                    elementName: "Label Name",
                    listValue: ["Field Value"]
                },
                {
                    elementTypeName: "text",
                    htmlType: "text",
                    elementId: "ELE0004",
                    elementName: "Label Name",
                    listValue: ["Field Value"]
                }, {
                    elementTypeName: "dropdown",
                    htmlType: "dropdown",
                    elementId: "ELE0019",
                    elementName: "Label Dropdown",
                    listValue: ["2"],
                    list: [
                        {
                            "id": 1,
                            "text": "Value 1 Dynamic",
                        },
                        {
                            "id": 2,
                            "text": "Value 2 Dynamic",
                        }
                    ]
                }, {
                    elementTypeName: "dropdown",
                    htmlType: "dropdown",
                    elementId: "ELE002218",
                    elementName: "Label Dropdown",
                    listValue: ["2"],
                    list: [
                        {
                            "id": 1,
                            "text": "Value 1 Dynamic",
                        },
                        {
                            "id": 2,
                            "text": "Value 2 Dynamic",
                        }
                    ]
                }]
            }, {
                sectionCode: "SEC003",
                subSectionCode: "MET003",
                subSectionName: "Secondary",
                elems: [{
                    elementTypeName: "text",
                    htmlType: "text",
                    elementId: "ELE0005",
                    elementName: "Label Name",
                    listValue: ["Field Value"]
                },
                {
                    elementTypeName: "text",
                    htmlType: "text",
                    elementId: "ELE000226",
                    elementName: "Secondary Name",
                    listValue: ["Field Value"]
                }]
            }]


            ElementRendering.renderer(data, id);

            Loader.hideLoader();
        }

    }

    save() {
        Loader.showLoader("Saving Data...");
        setTimeout(x => {
            Loader.hideLoader();
            toastr.success("Saved Successfully")
        }, 2000)
    }

    back() {
        Loader.showLoader("Redirecting To Listing...");
        setTimeout(x => {
            Loader.hideLoader();
            toastr.info("Redirecting")
        }, 2000)
    }

    error() {
        Loader.showLoader("Simulating Error ...");
        setTimeout(x => {
            Loader.hideLoader();
            toastr.error("This is an error")
        }, 2000)
    }

    cancel() {
        Loader.showLoader("Cancelling...");
        setTimeout(x => {
            Loader.hideLoader();
            toastr.warning("Save action has been cancelled")
        }, 2000)
    }

}
window.Template = new Template();