import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
export class Workflow {//class that handles the Workflow
    ////Global Variable : 
    data = null;
    constructor() {
        this.data = {
            data: [
            //    {
            //    id: 1,
            //    stepperlength: 5, // The number of nodes (steps) in this workflow
            //    stepperTitle: "Program of work - Crop Productivity and quality Improvement", // Main Title of the workflow
            //    steppers: [{ //List of nodes (Steps)
            //        id: 1, //node (step) id
            //        step: 1, //node (step) number
            //        stepTitle: "Program of work", //node (step) title 
            //        isActive: true, // node (step) status to put color completed 
            //        icon: "fa-solid fa-table-list", //node (step) icon
            //        details: { //deatils of node (step ) on click
            //            code: "POW201", //Code of form
            //            createdBy: "Ayushna Gopal", //created by
            //            createdDate: "01/02/2024", // created date
            //            status: "Approved By Jane Smith", // approval if any concat name if appoved or declined
            //            remarks: "Trial Monitoring should start as from 12/02/2024.", // remarks if any (approval remarks)
            //            link: "/CropProgramOfWork/CropProgramOfWorkAddEdit?Id=201&View=1" // link to form if any (for view button)

            //        },
            //    }, {
            //        id: 2,
            //        step: 2,
            //        stepTitle: "Requirement",
            //        isActive: true,
            //        icon: "fa-solid fa-clipboard-list",
            //        details: {
            //            code: "LDR136",
            //            createdBy: "Ayushna Gopal",
            //            createdDate: "08/02/2024",
            //            status: "Approved By Johnathan Doe",
            //            remarks: "Trial Monitoring should start as from 12/02/2024.",
            //            link: "/CropLandRequirement/CropLandRequirementAddEdit?Id=136&View=1#"

            //        },
            //    }, {
            //        id: 3,
            //        step: 3,
            //        stepTitle: "Research Protocol",
            //        isActive: false,
            //        icon: "fa-solid fa-list-ul",
            //        details: {
            //            code: "RPC76",
            //            createdBy: "Ayushna Gopal",
            //            createdDate: "10/02/2024",
            //            status: "Awaiting Approval",
            //            remarks: "",
            //            link: "/CropLandRequirement/CropLandRequirementAddEdit?Id=136&View=1#"
            //        },
            //    }, {
            //        id: 4,
            //        step: 4,
            //        stepTitle: "Trial Monitoring",
            //        isActive: false,
            //        icon: "fa-solid fa-sun-plant-wilt",
            //        details: null
            //    }, {
            //        id: 5,
            //        step: 5,
            //        stepTitle: "Research Report",
            //        isActive: false,
            //        icon: "fa-regular fa-file",
            //        details: null
            //    }]
            //},
            {
                id: 2,
                stepperlength: 4, // The number of nodes (steps) in this workflow
                stepperTitle: "Workmen Compensation - Workmen's Compensation Ordinance 1931", // Main Title of the workflow
                steppers: [{ //List of nodes (Steps)
                    id: 1, //node (step) id
                    step: 1, //node (step) number
                    stepTitle: "Complaint", //node (step) title 
                    isActive: true, // node (step) status to put color completed 
                    icon: "fa-solid fa-table-list", //node (step) icon
                    details: { //deatils of node (step ) on click
                        code: "POW201", //Code of form
                        createdBy: "Ayushna Gopal", //created by
                        createdDate: "01/02/2024", // created date
                        status: "Approved By Jane Smith", // approval if any concat name if appoved or declined
                        remarks: "Trial Monitoring should start as from 12/02/2024.", // remarks if any (approval remarks)
                        link: "" // link to form if any (for view button)

                    },
                }, {
                    id: 2,
                    step: 2,
                    stepTitle: "Doctor",
                    isActive: true,
                    icon: "fa-solid fa-clipboard-list",
                    details: {
                        code: "LDR136",
                        createdBy: "Ayushna Gopal",
                        createdDate: "08/02/2024",
                        status: "In Progress",
                        remarks: "Doctor Name/Tel : Mr John Smith / 5888 8891",
                        link: ""

                    },
                }, {
                    id: 3,
                    step: 3,
                    stepTitle: "Enquiry",
                    isActive: false,
                    icon: "fa-solid fa-list-ul",
                    details: {
                        code: "RPC76",
                        createdBy: "Ayushna Gopal",
                        createdDate: "10/02/2024",
                        status: "Awaiting Approval",
                        remarks: "",
                        link: ""
                    },
                }, {
                    id: 4,
                    step: 4,
                    stepTitle: "Contact Company To Find Solution",
                    isActive: false,
                    icon: "fa-regular fa-file",
                    details: null
                }]
            },
            {
                id: 2,
                stepperlength: 5, // The number of nodes (steps) in this workflow
                stepperTitle: "Workfare Programme - TUB [Section 84 of WRA 2019]", // Main Title of the workflow
                steppers: [{ //List of nodes (Steps)
                    id: 1, //node (step) id
                    step: 1, //node (step) number
                    stepTitle: "Complaint", //node (step) title 
                    isActive: true, // node (step) status to put color completed 
                    icon: "fa-solid fa-table-list", //node (step) icon
                    details: { //deatils of node (step ) on click
                        code: "POW201", //Code of form
                        createdBy: "Ayushna Gopal", //created by
                        createdDate: "01/02/2024", // created date
                        status: "Approved By Jane Smith", // approval if any concat name if appoved or declined
                        remarks: "Complaint has been logged", // remarks if any (approval remarks)
                        link: "" // link to form if any (for view button)

                    },
                }, {
                    id: 2,
                    step: 2,
                    stepTitle: "Inspection",
                    isActive: false,
                    icon: "fa-solid fa-clipboard-list",
                    details: null
                }, {
                    id: 3,
                    step: 3,
                    stepTitle: "Approval From SLIRO",
                    isActive: false,
                    icon: "fa-solid fa-list-ul",
                    details: null,
                }, {
                    id: 4,
                    step: 4,
                    stepTitle: "Approval From PLIRO",
                    isActive: false,
                    icon: "fa-regular fa-file",
                    details: null
                }, {
                    id: 5,
                    step: 5,
                    stepTitle: "Approval From AD",
                    isActive: false,
                    icon: "fa-regular fa-file",
                    details: null
                }]
                }, {
                    id: 2,
                    stepperlength: 4, // The number of nodes (steps) in this workflow
                    stepperTitle: "Violence at Work - Section 114 of WRA 2019", // Main Title of the workflow
                    steppers: [{ //List of nodes (Steps)
                        id: 1, //node (step) id
                        step: 1, //node (step) number
                        stepTitle: "Complaint", //node (step) title 
                        isActive: true, // node (step) status to put color completed 
                        icon: "fa-solid fa-table-list", //node (step) icon
                        details: { //deatils of node (step ) on click
                            code: "POW201", //Code of form
                            createdBy: "Ayushna Gopal", //created by
                            createdDate: "01/02/2024", // created date
                            status: "Approved By Jane Smith", // approval if any concat name if appoved or declined
                            remarks: "Complaint has been logged", // remarks if any (approval remarks)
                            link: "" // link to form if any (for view button)

                        },
                    }, {
                        id: 2,
                        step: 2,
                        stepTitle: "Equiry (SLIRO / PLIRO)",
                        isActive: true,
                        icon: "fa-solid fa-clipboard-list",
                        details: { //deatils of node (step ) on click
                            code: "POW201", //Code of form
                            createdBy: "Ayushna Gopal", //created by
                            createdDate: "01/02/2024", // created date
                            status: "Approved By Jane Smith", // approval if any concat name if appoved or declined
                            remarks: "Complaint has been logged", // remarks if any (approval remarks)
                            link: "" // link to form if any (for view button)

                        }
                    }, {
                        id: 3,
                        step: 3,
                        stepTitle: "Send To DPP",
                        isActive: true,
                        icon: "fa-solid fa-list-ul",
                        details: { //deatils of node (step ) on click
                            code: "POW201", //Code of form
                            createdBy: "Ayushna Gopal", //created by
                            createdDate: "01/02/2024", // created date
                            status: "Approved By Jane Smith", // approval if any concat name if appoved or declined
                            remarks: "Complaint has been logged", // remarks if any (approval remarks)
                            link: "" // link to form if any (for view button)

                        },
                    }, {
                        id: 4,
                        step: 4,
                        stepTitle: "Send To Procecution Unit",
                        isActive: false,
                        icon: "fa-regular fa-file",
                        details: null
                    }]
                }]
        }
        this.setWorkflowStepper();
    }

    //display steppers for workflow
    setWorkflowStepper() {
        var data = this.data;
        $("#steppers-main-container").html(""); // Clear previous content
        let mainContainer = [];

        data.data.forEach((steppersElements, index) => {
            let stepperIndicatorContainer = null;
            let stepperDetailContainer = null;

            let stepsDat = [];
            let detailsDat = [];


            steppersElements.steppers.forEach((steps, i) => {
                let active = steps.isActive ? "active" : "";
                let IsActive = DefaultService.createElement("div", { class: "indicator-line" }, []);
                let Details = DefaultService.createElement("div", { class: "box-shadow-grey p-3 border-grey naveo-color collapse", id: "details_" + steps.id, "data-bs-parent": "#stepper-container_" + index }, [
                    DefaultService.createElement("span", { class: "span-medium f-weight-bold" }, [], { innerHTML: 'No Details' })
                ]);

                if (steps.isActive) {
                    IsActive = DefaultService.createElement("div", { class: "indicator-line active" }, [])
                };
                if (i == steppersElements.stepperlength - 1) {
                    IsActive = null;
                }

                let stepsContainer = DefaultService.createElement("a", { class: "step " + active, "data-bs-toggle": "collapse", "aria-expanded": active, "data-bs-target": "#details_" + steps.id }, [
                    DefaultService.createElement("div", { class: "step-icon" }, [
                        DefaultService.createElement("i", { class: steps.icon }, [], {}),
                    ], {}),
                    DefaultService.createElement("span", {}, [], { innerHTML: steps.stepTitle }),
                ])

                stepsDat.push(stepsContainer);
                stepsDat.push(IsActive);



                if (steps.details != null) {
                    Details = DefaultService.createElement("div", { class: "box-shadow-grey border-grey collapse", id: "details_" + steps.id, "data-bs-parent": "#stepper-container_" + index }, [
                        DefaultService.createElement("div", { class: "d-flex flex-column p-3" }, [
                            DefaultService.createElement("div", { class: "naveo-color mb-3" }, [
                                DefaultService.createElement("i", { class: "fa fa-file-lines me-2" }, [], {}),
                                DefaultService.createElement("span", { class: "span-medium f-weight-bold" }, [], { innerHTML: steps.stepTitle }),
                            ], {}),
                            DefaultService.createElement("div", {}, [
                                DefaultService.createElement("span", {}, [], { innerHTML: 'Code: ' }),
                                DefaultService.createElement("span", { class: "f-weight-bold" }, [], { innerHTML: steps.details.code })
                            ], {}),
                            DefaultService.createElement("div", {}, [
                                DefaultService.createElement("span", {}, [], { innerHTML: 'Created By: ' }),
                                DefaultService.createElement("span", { class: "f-weight-bold" }, [], { innerHTML: steps.details.createdBy })
                            ], {}),
                            DefaultService.createElement("div", {}, [
                                DefaultService.createElement("span", {}, [], { innerHTML: 'Created Date: ' }),
                                DefaultService.createElement("span", { class: "f-weight-bold" }, [], { innerHTML: steps.details.createdDate })
                            ], {}),
                            DefaultService.createElement("div", {}, [
                                DefaultService.createElement("span", {}, [], { innerHTML: 'Status: ' }),
                                DefaultService.createElement("span", { class: "f-weight-bold" }, [], { innerHTML: steps.details.status })
                            ], {}),
                            DefaultService.createElement("div", {}, [
                                DefaultService.createElement("span", {}, [], { innerHTML: 'Remarks: ' }),
                                DefaultService.createElement("span", { class: "f-weight-bold" }, [], { innerHTML: steps.details.remarks })
                            ], {}),
                            DefaultService.createElement("button", { class: "form-control btn-c-secondary mt-3", onclick: "Workflow.goToLocation('" + steps.details.link + "')" }, [
                                DefaultService.createElement("i", { class: "fa fa-save me-2" }, [], {}),
                            ], { innerHTML: 'View' }),
                        ])
                    ]);
                }
                detailsDat.push(Details)


            });

            stepperIndicatorContainer = DefaultService.createElement("div", { class: "step-indicator" }, stepsDat, {});
            stepperDetailContainer = DefaultService.createElement("div", { class: "accordion-group mx-5" }, detailsDat, {});

            let title = DefaultService.createElement("span", { class: "stepper-title" }, [], { innerHTML: steppersElements.stepperTitle })

            let StepperContainer = DefaultService.createElement("div", { class: "stepper-container", id: "stepper-container_" + index }, [
                title,
                stepperIndicatorContainer,
                stepperDetailContainer
            ]);
            mainContainer.push(StepperContainer)
        });


        $("#steppers-main-container").html(mainContainer);
    }

    goToLocation(link) {
        window.open(link, '_blank');
    }
}
window.Workflow = new Workflow();