import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { xhrService } from "/Scripts/Common/xhrService.js";
import { ApiTypes, ActivityElementType, ActivityModule } from "/Scripts/Common/Enums.js";
import { ElementRendering } from "/Scripts/Common/Rendering/ElementRendering.js";

export class ActivitiesAddEdit {//class that handles the Add Edit screen
    details = [];
    dataDetail = [];
    id = 0;
    constructor() {
        var dropdown = null;
        //get Dropdown Values
        ApiService.callCustom(ApiType.POST, variable.getDropDownActivities, ActivityModule.General, (data) => {
            if (data) {
                dropdown = data.data;
            }
        })
        //Field after ajax call 
        $(document).ajaxComplete(() => {
            this.id = $("#Id").val();
            if (dropdown != null) {
                //Task List Select
                DropdownService.initDropdown("select-tasklist", dropdown, "ActivitiesTaskList");
                //Set Dropdown Value on edit 
                $("#select-tasklist").val($("#select-tasklist-id").val()).trigger('change');
                //Status Select
                DropdownService.initDropdown("select-status", dropdown, "ActivitiesStatus");
                //Set Dropdown Value on edit 
                if ($("#select-status").data('select2')) {
                    $("#select-status").select2("val", $("#select-status option:contains(" + $("#select-status-id").val() + ")").val());
                }
                
                //$("#select-status").val($("#select-status-id").val()).trigger('change');
                //User Select
                DropdownService.initDropdown("select-user", dropdown, "User");
                //Set Dropdown Value on edit 
                $("#select-user").val($("#select-user-id").val()).trigger('change');
            }

            if (this.id > 0) {
                var id = $('#select-tasklist').val();
                this.getDynamicFields(id);
                setTimeout(e => {
                    this.setReadonlyElements();
                }, 100)
            }

            $('#select-tasklist').on('change', (e) => {
                var id = $('#select-tasklist').val();
                this.getDynamicFields(id)
            });
        });
    }
    //get Dynamic Fields
    getDynamicFields(taskId) {
        if (taskId > 0) {
            var data = { "data": taskId }
            xhrService.xhrRequestCustom(ApiTypes.POST, variable.getByIdConfigActivities, data).then(res => {
                //render elem
                ElementRendering.dynamicElementsRenderer("dynamicElements", res.data)
                this.details = res.data;
                res.data.forEach((datum, index) => {
                    var type = Object.keys(ActivityElementType).find(name => ActivityElementType[name] === datum.type)
                    if (type == "Dropdown") {
                        this.getDropdownByParameter(datum.labelName, datum.fieldId);
                        if (datum.labelName == "Scheme Name") {
                            //Event Listener : Populate Services when form is selected
                            $('#' + datum.fieldId).on('change', (e) => {
                                let code = this.details.find(x => x.labelName == "Scheme Code")
                                this.getDropdownByParameter(code.labelName, code.fieldId, $('#' + datum.fieldId).val())
                            });
                        }
                        if (datum.labelName == "Scheme Code") {
                            //Event Listener : Populate Services when form is selected
                            $('#' + datum.fieldId).on('change', (e) => {
                                let code = this.details.find(x => x.labelName == "Section")
                                this.getDropdownByParameter(code.labelName, code.fieldId, $('#' + datum.fieldId).val())
                            });
                        }
                    }
                })
                setTimeout(e => {
                    this.getDetailsById();
                }, 500)

            })
        } else {
            this.details = [];
            $("#dynamicElements").html("");
        }
    }

    //get Dropdown Value by parameter
    getDropdownByParameter(labelName, fieldId, parameter = 0) {
        var data = {
            "data": {
                "LabelName": labelName,
                "Parameter": parameter
            }
        }
        xhrService.xhrRequestCustom(ApiTypes.POST, variable.getDrpdownValuesByLabelActivities, data).then(response => {
            DropdownService.destroyReinitDropdown(fieldId, response.data, labelName);
        })
    }

    //getDetailsById
    getDetailsById() {
        if (this.id > 0) {
            var data = {
                "data": this.id
            }
            xhrService.xhrRequestCustom(ApiTypes.POST, variable.getByIdDetailsActivities, data).then(response => {
                this.dataDetail = response.data;
                response.data.forEach((datum, index) => {
                    var type = Object.keys(ActivityElementType).find(name => ActivityElementType[name] === datum.config.type)
                    if (type.toLowerCase() == "number") {
                        $("#" + datum.config.fieldId).val(parseInt(datum.value))
                    } else if (type.toLowerCase() == "dropdown") {
                        $("#" + datum.config.fieldId).val(parseInt(datum.value)).trigger('change');
                    }
                    if (datum.config.labelName == "Scheme Code") {
                        setTimeout(e => {
                            $("#" + datum.config.fieldId).val(parseInt(datum.value)).trigger('change');
                        }, 400)
                    }
                    if (datum.config.labelName == "Section") {
                        setTimeout(e => {
                            $("#" + datum.config.fieldId).val(parseInt(datum.value)).trigger('change');
                        }, 800)
                    }

                })
            })
        }
    }

    //se tRead only Elements on edit
    setReadonlyElements() {
        $("#select-tasklist").prop("disabled", true);
        $("#StartDate").prop("disabled", true);
        $("#EndDate").prop("disabled", true);
        $("#select-user").prop("disabled", true);
        this.details.forEach(e => {
            $("#" + e.fieldId).prop("disabled", true);
        })
       
    }
    
    //format details for save
    formatData(data, edit = false) {
        var formattedData = data;
        if (!edit) {
            var formattedDataDetails = [];
            this.details.forEach((datum, index) => {
                var data = {
                    Id: 0,
                    Config: datum,
                    Value: $("#" + datum.fieldId).val()
                }
                formattedDataDetails.push(data)
            })
            formattedData['Details'] = formattedDataDetails;
        }
        

        formattedData['URL'] = window.location.pathname + "?dto=";
        if (formattedData["EndDate"] == "") {
            formattedData["EndDate"] = null
        }
        if (formattedData["StartDate"] == "") {
            formattedData["StartDate"] = null
        }
        return formattedData;
    }

    //Save and update button
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("Activities-AddEdit-Panel"));
        //check if required fields are input
        if (data == false) {
            //show error if required fields not input
            toastr.error("Please Fill In Mandatory Fields")
        } else {
            let dataFormatted = null;
            // Format Data To send
            if (this.id > 0) {
                dataFormatted = this.formatData(data, true);
            } else {
                dataFormatted = this.formatData(data);
            }
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateActivities, dataFormatted, (response) => {
                //Response true
                if (response.data) {
                    toastr.success("Saved Successfully!")
                    window.location.reload();
                    location.href = "/Activities/Activities#"
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

    //Redirect Button to open scheme
    redirect(url) {
        window.open(url, "_blank");
    }
}
//init new Edit Class
window.ActivitiesAddEdit = new ActivitiesAddEdit();