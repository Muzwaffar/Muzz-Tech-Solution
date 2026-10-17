import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js"; 
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { Validation } from "/Scripts/Common/ValidationService.js";

export class FormAddEdit {//class that handles the Add Edit screen
    //Global Variables
    sections = null;
    checkboxToRowMap = {};
    constructor() {
        var dropdown = null;
        // Call api to get all dropdown values
        ApiService.callCustom(ApiType.POST, variable.getFormDropdown, "", (data) => {
            if (data) {
                dropdown = data.data
            }
        });
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            if (dropdown != null) {
                // Set status list of values
                DropdownService.initDropdown("select-formType", dropdown, "FormType");
                // If Screen mode edit, Set Dropdown status selected value to get by id value
                $("#select-formType").val($("#select-formType-id").val()).trigger('change');

            }
            $('.checkboxClass').on('click', () => {
                this.CheckboxChange($(event.target));
            });

        });
    }

    //API to get relation
    getRelationById(id) {
        this.sections = [];
        //set id on add new
        if (id == undefined || id == null || id == "") {
            id = 0;
        }
        //call api to get service 
        ApiService.callCustom(ApiType.POST, variable.getAllSectionForm, id, (data) => {
            this.sections = data.data;
            //init data to put in table 
            if (this.sections.length > 0) {
                DatatableService.updateDataGrid("relation-Section-Table", this.sections);
                //check element
                $.each(this.sections, function (index, element) {
                    if (element.check) {
                        $("#" + element.id).prop("checked", true);
                    }
                });
                this.editCheckboxChange(this.sections, this.checkboxToRowMap)
            }
        })
    }

    // Function to add or remove table rows based on checkbox status
    updateTable() {
        $(".contentOrderInfo").html(""); // Clear previous content
        let tableContent = DefaultService.createElement("table", { class: "table dataTable" }, [
            DefaultService.createElement("tr", {}, [
                DefaultService.createElement("th", { class: "col-8 naveo-color py-3 w-40" }, [], { innerHTML: "Section Name" }),
                DefaultService.createElement("th", { class: "col-4 naveo-color py-3 w-40" }, [], { innerHTML: "Order" }),
            ])
        ]);

        // Loop through the checkboxToRowMap to add rows to the table
        for (let checkboxValue in this.checkboxToRowMap) {
            if (this.checkboxToRowMap.hasOwnProperty(checkboxValue)) {
                let row = this.checkboxToRowMap[checkboxValue];
                tableContent.appendChild(row);
            }
        }
        $(".contentOrderInfo").html(tableContent);
    }

    CheckboxChange(event) {
        let checkbox = event;
        let checkboxValue = this.sections.find(x => x.id == event[0].id).name;
        // If checkbox is checked
        if (checkbox.is(":checked")) {
            // Create a new table row with two columns
            let row = DefaultService.createElement("tr", {}, [
                DefaultService.createElement("td", { class: "CheckboxName" }, [], { innerHTML: checkboxValue }),
                DefaultService.createElement("td", { class: "" }, [
                    DefaultService.createElement("input", { class: "form-control w-92 Order mb-2", type: "number", placeholder: "Order" }, []),
                ], {})
            ]);
            // Add the row to the map
            this.checkboxToRowMap[checkboxValue] = row;
        } else {
            // Remove the row from the map and update the table
            delete this.checkboxToRowMap[checkboxValue];
        }
        // Update the table
        this.updateTable();
    }
    editCheckboxChange(checklists, checkboxToRowMap) {
        $.each(checklists, function (index, element) {
            let checkboxValue = element.name;
            if (element.check) {
                // Create a new table row with two columns
                let row = DefaultService.createElement("tr", {}, [
                    DefaultService.createElement("td", { class: "CheckboxName" }, [], { innerHTML: checkboxValue }),
                    DefaultService.createElement("td", { class: "" }, [
                        /*DefaultService.createElement("input", { type: "number", class: "form-control" }, [])*/
                        DefaultService.createElement("input", { class: "form-control w-92 Order mb-2", type: "number", placeholder: "Order", value: element.order }, []),
                    ], {})
                ]);
                // Add the row to the map
                checkboxToRowMap[checkboxValue] = row;
            }
            else {
                delete checkboxToRowMap[checkboxValue];
            }
            // Update the table
        });
        this.updateTable();
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["name", "select-formType"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }
        let valuesMap = {}; // Use an object to store unique values
        // Iterate through input fields
        $(".contentOrderInfo #Order").each(function () {
            let value = $(this).val();
            if (value == "") {
                toastr.error("Please enter an Order")
                valid = false;
                return false;
            }
            if (value > $(".contentOrderInfo #Order").length) {
                toastr.error("Order cannot be greater than number of items")
                valid = false;
                return false;
            }
            // Check if the value already exists in the map
            if (valuesMap[value]) {
                toastr.error("Cannot have duplicate Order")
                valid = false;
                return false;
            } else {
                // Add the value to the map
                valuesMap[value] = value;
            }
        });
        //return boolean id valid or not
        return valid
    }

    //Button save ot update 
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("Form-AddEdit-Panel"));
        var checked = [];
        let nameSave = {};
        let orderSave = {};
        //check if required fields are input
        if (this.valid() && data != false) {
            data["FormType"] = DefaultService.resultObject("FormType", document.getElementById("Form-AddEdit-Panel"));
            var listSection = this.sections;
            if (data.Id == "") {
                data.Id = 0;
            }
            $(".contentOrderInfo .CheckboxName").each(function (index) {
                let value = $(this)[0].innerHTML;
                nameSave[index] = {
                    Name: value
                }
            });
            $(".contentOrderInfo .Order").each(function (index) {
                orderSave[index] = {
                    Name: nameSave[index],
                    Order: $(this).val()
                }
            });
            //get all checked element
            $.each($('#relation-Section-Table').find('input:checkbox[name=checkbox]:checked'), function (index) {
                var id = $(this).attr('id');
                var elem = listSection.find(x => x.id == id);
                var name = elem.name;
                var matchingKey = Object.keys(orderSave).find(key => orderSave[key].Name.Name === name);
                elem.order = orderSave[matchingKey].Order;
                elem.check = true;
                checked.push(elem);

            });
            //atleast one service should be checked
            if (checked.length == 0) {
                toastr.error("Please Select Atleast One Service");
                return;
            }

            //build data to send 
            let dataToSend = {
                Form: data,
                Sec: checked
            }

            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateForm, dataToSend, (response) => {
                //Response true
                if (response.data) {
                    toastr.success("Saved Successfully!")
                    window.location.reload();
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
}
//init new Edit Class
window.FormAddEdit = new FormAddEdit();