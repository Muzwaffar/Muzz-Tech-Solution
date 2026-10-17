import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";

export class RosterAddEdit {//class that handles the Roster Add Edit screen
    //Global Variables
    relationArray = [];
    relationData = [];
    shifts = null;

    constructor() {
        var resourceColumn = [ //Relation Column List
            { data: 'resource.code', title: 'Resource Code' },
            { data: 'resource.naa.fullName', title: 'Resource Name' },
        ];

        var days = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

        var daySelectCol = [{ data: 'name', title: 'Shift' }];

        for (let i = 0; i < days.length; i++) {
            var toAdd = {
                title: days[i],
                orderable: false,
               // width: "100%",
                render: function (data, type, full, meta) {
                    data = '<input type="checkbox" id="' + full.id + days[i] + '" name="checkbox" onclick="RosterAddEdit.shiftCheck($(this))" />';
                    return data;
                }
            }
            daySelectCol.push(toAdd);
        }
        //API To get Shift list    
        ApiService.callCustom(ApiType.POST, variable.getListShift, "", (response) => {
            this.shifts = response;
        });

        //Get Dropdown Value
        var dropdown = null;
        //API To get Dropdown Value
        ApiService.callCustom(ApiType.POST, variable.getDropDownRoster, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {
                    //Team Select
                    DropdownService.initDropdown("select-team", dropdown, "Team");

                    //Resource Select
                    DropdownService.initDropdown("select-resource", dropdown, "Resource");

                    //Init Relationship Datatable
                    DatatableService.initializeDataGridDynamic('relation-Resource-R-Table', resourceColumn, 'RosterAddEdit', this.relationData);

                    //Init Day Selection Datatable
                    DatatableService.initializeDataGridDynamic('day-Selection-Table', daySelectCol, 'RosterAddEdit', this.shifts, "", false);
                    setTimeout(x => {
                        this.getRelationById();
                    }, 500)
                }
            }
        });
        

    }

    //API to get relation
    getRelationById() {
        let data = DefaultService.result(document.getElementById("Roster-AddEdit-Panel"));
        var Id = (data['Id']);
        this.relationArray = [];
        this.relationData = [];
        if (Id != undefined && Id != "00000000-0000-0000-0000-000000000000") {
            ApiService.callCustom(ApiType.POST, variable.getRosterDetailsById, Id, (data) => {
                this.relationData = data.data;
                if (this.relationData.length > 0) {
                    DatatableService.updateDataGrid("relation-Resource-R-Table", this.relationData);
                }
            })
        }

        if (Id != "" && Id != null) {
            ApiService.callCustom(ApiType.POST, variable.getDaySelectionByRosterId, Id, (data) => {
                var singleitem;
                var datax = data.data;
                if (datax) {
                    setTimeout(function () {
                        //populate checkbox days selection
                        $(datax.shift).each(function () {
                            var check = $(this)[0];
                            if (check.isIndividual == false) {
                                $("input:checkbox[name=checkbox]").each(function () {
                                    if ((check.shiftId + check.day) == $(this).attr('id') && check.isChecked == true) {
                                        $(this).prop('checked', true);
                                    }
                                });
                            }
                        });
                        //edit Shifts dropdown
                        let container = $("#EditShift");
                        container[0].innerHTML = " ";
                        var rosters = [];
                        //loop in individual shift rows and create dropdown and label for each
                        $(datax.rsr).each(function () {
                            var rosterDate = $(this)[0];
                            rosterDate.date.replace(/ /g, '');
                            var date = moment(rosterDate.date).format("DD-MM-YYYY");
                            var dateUpdate = moment(rosterDate.date).format("DD/MM/YYYY");
                            var ros = {
                                shiftId: rosterDate.shiftId,
                                day: rosterDate.day,
                                date: rosterDate.date
                            };
                            rosters.push(ros);
                            var id = rosterDate.shiftId + date;
                            var shifttoSend = rosterDate.shiftId + dateUpdate;
                            var innerhtml = rosterDate.day + "      " + dateUpdate;

                            singleitem = DefaultService.createElement("div", { class: "row mb-4" }, [
                                DefaultService.createElement("div", { class: "col-md-6" }, [
                                    DefaultService.createElement("div", { class: "ms-5 mt-3 pt-2" }, [
                                        DefaultService.createElement("h6", { class: "mt-2 secondary-header-color" }, {}, { innerHTML: innerhtml }),
                                    ])
                                ]),
                                DefaultService.createElement("div", { class: "col-md-6" }, [
                                    DefaultService.createElement("div", { class: "select-form-floating", id: "ShiftDropdown" }, [
                                        DefaultService.createElement("select", { class: "", id: id, name: "shiftEdit", Shift: shifttoSend }, {}, {},
                                            DefaultService.createElement("option", { class: "", value: "" }, {}, {}),

                                        ),
                                    ])
                                ])
                            ])
                            container.append(singleitem);
                        });

                        container.append(singleitem);
                        for (let j = 0; j < rosters.length; j++) {
                            //Shift Select
                            DropdownService.initDropdown(rosters[j].shiftId + moment(rosters[j].date).format("DD-MM-YYYY"), datax.dropdown, "Shift");

                            //Set Dropdown Value on edit 
                            $("#" + rosters[j].shiftId + moment(rosters[j].date).format("DD-MM-YYYY")).val(rosters[j].shiftId).trigger('change');
                        }
                    }, 300);

                    //display Edit individual Shift Panel
                    //if ($("#RenderShift").hasClass('d-none')) {
                    //    $("#RenderShift").removeClass('d-none');
                    //}
                }

            })

        }
    }

    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["rosterName", "rosterFr", "rosterTo"])

        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }
        //return boolean id valid or not
        return valid
    }

    //Button save or update Roster
    saveUpdateRoster() {
        // get all values
        let data = DefaultService.result(document.getElementById("Roster-AddEdit-Panel"));
        if (data['Id'] == "") {
            data['Id'] = 0;
        }
        var checkcount = false;
        if ($('#AccesRightsDataTable').find('input:checkbox[name=checkbox]:checked').length > 0) {
            checkcount = true;
        }

        ////getcheckboxes data
        let listOfObjects = [];
        var days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
        for (let i = 0; i < days.length; i++) {
            $.each(this.shifts.data, function (index, element) {
                var singleObj = {};
                var l1 = $('#' + element.id + days[i]);
                if (l1.is(':checked')) {
                    singleObj['ShiftId'] = element.id;
                    singleObj['ShiftName'] = element.name;
                    singleObj['isChecked'] = true;
                    singleObj['day'] = days[i];
                    listOfObjects.push(singleObj);
                }
            });
        }

        let ShiftId = null;
        let individualShiftL = [];
        let listOfIndividualShifts = [];
        if (listOfObjects != null && listOfObjects.length != 0 && data != false) {
            data["Shift"] = DefaultService.resultObject("Shift", document.getElementById("Roster-AddEdit-Panel"));
            individualShiftL = Object.keys(data["Shift"]);
        }

        else {
            this.valid();
            toastr.error("Please Fill All Mandatory Fields");
            return;
        }

        for (let i = 0; i < individualShiftL.length; i++) {
            const str = String(individualShiftL[i]);
            ShiftId = String(individualShiftL[i]).slice(0, 36)
            var ShiftDate = String(individualShiftL[i]).slice(36).replaceAll("/", "-");
            var id = '#' + ShiftId + ShiftDate;
            var idToSend = $(id).select2('data');

            var shiftToSend = {
                ShiftId: idToSend[0].id,
                ShiftName: idToSend[0].text,
                date: moment(ShiftDate, "DD-MM-YYYY").add(4, 'hours')
            };
            listOfIndividualShifts.push(shiftToSend);
        }

        //Add resources 
        this.relationArray = [];
        $.each(this.relationData, (index, value) => {
            let x = {
                ResourceId: value.resource.id, Code: value.resource.code
            }
            this.relationArray.push(x)
        })

        //Build Obj to send
        let dataToSend = []
        dataToSend = {
            Roster: data,
            RSR: this.relationArray,
            Shift: listOfObjects,
            ShiftUpdate: listOfIndividualShifts
        };
        //check if required fields are input
        if (!this.valid() && data == false) {
            //show error if required fields not input
            toastr.error("Please Fill In Mandatory Fields")
        }
        else if (dataToSend.Roster.RosterFrom > dataToSend.Roster.RosterTo) {
            toastr.error("Date To Should be Set After Date From")
        }
        else if (dataToSend.RSR.length == 0) {
            toastr.error("Please Select A Team or Resource")
        }
        else {
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.createUpdateRoster, dataToSend, (response) => {
                //Response true
                if (response.data) {
                    toastr.success("Saved Successfully!");
                    this.back();
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

    //Add Relation to table
    addRelation() {
        var teamr = $("#teamRadio").is(':checked');
        var toAdd = [];
        var datatableL = [];
        //Loop in datatable and add resourceCode to an array
        var table = $('#relation-Resource-R-Table').DataTable();
        table.rows().eq(0).each(function (index) {
            var row = table.row(index);
            var datax = row.data();
            datatableL.push(datax.resource.code);
        });
        if (teamr == true) {

            let id = $("#select-team").val();
            ApiService.callCustom(ApiType.POST, variable.getResourceByTeamId, id, (data) => {
                if (data.data.length > 0) {
                    for (let resource of data.data) {
                        if (datatableL.indexOf(resource.code) !== -1) {
                            toastr.error("Some Record(s) Already Exist(s)");
                        }
                        else {
                            let relationx = {
                                resource: { id: resource.id, naa: { fullName: resource.naa.fullName }, code: resource.code }
                            }
                            toAdd.push(relationx);

                        }
                    }
                }
                for (let add of toAdd) {
                    if (this.relationData.indexOf(add.resource.code) !== -1) {

                    }
                    else {
                        this.relationData.push(add);
                    }
                }
                toAdd = [];
                DatatableService.updateDataGrid("relation-Resource-R-Table", this.relationData);
                $("#select-team").select2("val", " ");
            })
        }
        else {
            let resourceCodex = $("#select-resource").select2("data")[0].lot;
            if (datatableL.indexOf(resourceCodex) !== -1) {
                toastr.error("Record Already Exists");
            }
            else {
                let relation = {
                    resource: { id: $("#select-resource").val(), naa: { fullName: $("#select-resource :selected").text() }, code: resourceCodex },
                }
                toAdd.push(relation);
            }
            for (let add of toAdd) {
                if (this.relationData.indexOf(add.resource.code) !== -1) {
                }
                else {
                    this.relationData.push(add);
                }
            }
            toAdd = [];
            DatatableService.updateDataGrid("relation-Resource-R-Table", this.relationData);
            $("#select-resource").select2("val", " ");
        }
    }

    //On clicking Edit Shift / Changing date or shifts from grid
    EditShift() {
        let data = DefaultService.result(document.getElementById("Roster-AddEdit-Panel"));
        
        if (data['Id'] == "") {
            data['Id'] = 0;
        }
        ////getcheckboxes data
        let listOfObjects = [];
        //var current = Date.now();
        var days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
        for (let i = 0; i < days.length; i++) {
            $.each(this.shifts.data, function (index, element) {
                var singleObj = {};
                var l1 = $('#' + element.id + days[i]);
                if (l1.is(':checked')) {
                    singleObj['ShiftId'] = element.id;
                    singleObj['ShiftName'] = element.name;
                    singleObj['isChecked'] = true;
                    singleObj['day'] = days[i];
                    singleObj['isIndividual'] = false;
                    singleObj['date'] = moment().toDate();
                    listOfObjects.push(singleObj);
                }
            });
        }
        //data to send
        let dataToSend = []
        dataToSend = {
                Roster: data ,
                RSR: [],
                Shift: listOfObjects,
                ShiftUpdate: [],
                dropdown: []
        };
        
        //Check if Roster Details has been filled
        if (dataToSend.Roster == false || dataToSend.Roster == null) {
            toastr.error("Please Enter Roster Details")
        }
        //Check if Shift(s) has been selected
        else if (dataToSend.Shift.length == 0 || dataToSend == null) {
            toastr.error("Please Select Appropriate Shift(s)")
            return;
        }
        else {
            this.showEditPanel();
            //API To get Shift list 
            ApiService.callCustom(ApiType.POST, variable.getIndividualShifts, dataToSend, (response) => {
                var singleitem;
                var rosterShift = response.data;

                //edit Shifts dropdown
                let container = $("#EditShift");
                container[0].innerHTML = "";
                var rosters = [];
                $(rosterShift.shift).each(function () {
                    var rosterDate = $(this)[0];
                    rosterDate.date.replace(/ /g, '');
                    var date = moment(rosterDate.date).format("DD-MM-YYYY");
                    var dateUpdate = moment(rosterDate.date).format("DD/MM/YYYY");
                    var ros = {
                        shiftId: rosterDate.shiftId,
                        day: rosterDate.day,
                        date: rosterDate.date
                    };
                    rosters.push(ros);
                    var id = rosterDate.shiftId + date;
                    var shifttoSend = rosterDate.shiftId + dateUpdate;
                    var innerhtml = rosterDate.day + "      " + dateUpdate;

                    singleitem = DefaultService.createElement("div", { class: "row mb-4" }, [
                        DefaultService.createElement("div", { class: "col-md-6" }, [
                            DefaultService.createElement("div", { class: "ms-5 mt-3 pt-2" }, [
                                DefaultService.createElement("h6", { class: "mt-2 secondary-header-color" }, {}, { innerHTML: innerhtml }),
                            ])
                        ]),
                        DefaultService.createElement("div", { class: "col-md-6" }, [
                            DefaultService.createElement("div", { class: "select-form-floating", id: "ShiftDropdown" }, [
                                DefaultService.createElement("select", { class: "", id: id, name: "shiftEdit", Shift: shifttoSend }, {}, {},
                                    DefaultService.createElement("option", { class: "", value: "" }, {}, {}),

                                ),
                            ])
                        ])
                    ])
                    container.append(singleitem);
                });

                container.append(singleitem);

                for (let j = 0; j < rosterShift.shift.length; j++) {
                    //Shift Select
                    DropdownService.initDropdown(rosterShift.shift[j].shiftId + moment(rosterShift.shift[j].date).format("DD-MM-YYYY"), rosterShift.dropdown, "Shift");

                    //Set Dropdown Value on edit 
                    $("#" + rosterShift.shift[j].shiftId + moment(rosterShift.shift[j].date).format("DD-MM-YYYY")).val(rosterShift.shift[j].shiftId).trigger('change');
                }

            });
        }

        var editShiftPanel = document.getElementById("RenderShift");
        editShiftPanel.classList.remove("d-none");

    }

    //Delete relation
    deleteRelation(meta) {
        this.relationData.splice(meta, 1);
        DatatableService.updateDataGrid("relation-Resource-R-Table", this.relationData);
    }

    //Switch Dropdown between Team and Resource
    radioClick() {
        var resource = document.getElementById("resource");
        var team = document.getElementById("team");
        var teamr = $("#teamRadio").is(':checked');
        if (teamr == true) {
            team.classList.remove("d-none");
            resource.classList.add("d-none");
        }
        else {
            resource.classList.remove("d-none");
            team.classList.add("d-none");
        }
    }

    //On Clicking a shift on the grid
    shiftCheck(x) {
        var id = x[0].id;
        var day = String(id).slice(1);
        $("input:checkbox[name=checkbox]").each(function () {
            if ($(this).attr('id').includes(day)) {
                if ($(this).attr('id') != id) {
                    $(this).prop('checked', false);
                }
            }
        });

        this.EditShift();
        this.hideEditPanel();
    }

    //Hide Individual Shift Edit Panel
    hideEditPanel() {
        /*this.EditShift();*/
        const editPanel = document.getElementById("RenderShift");
        if (!editPanel.classList.contains("d-none")) {
            editPanel.classList.add("d-none");
        }
        if ($("#rosterTo").val() != "") {
            if ($("#rosterTo").val() < $("#rosterFr").val()) {
                toastr.error("Date To Should Be Set After Date From");
            }
        }
        
    }

    //Show Individual Shift Edit Panel
    showEditPanel() {
        const editPanel = document.getElementById("RenderShift");
        if (editPanel.classList.contains("d-none")) {
            editPanel.classList.remove("d-none");
        }
    }

    //Back button
    back() {
        //show loader when refirecting to listing
        location.href = "/Roster/List#"
    }
}
//init new Roster Edit Class
window.RosterAddEdit = new RosterAddEdit();