import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";

export class Roster {//class that handles the Roster screen
    constructor() {
        $(document).ready(() => {
            //close partial view on reload
            window.location.href = "#";
            //Column list for Roster to build table (predefined columns)
            //data: object key
            //title: column title to show
            var RosterColList = [
                { data: 'roster.code', title: 'Code' },
                { data: 'roster.name', title: 'Name' },
                { data: 'roster.rosterPeriod', title: 'Roster Period' },
                { data: 'date', title: 'Date', render: $.fn.dataTable.render.moment('YYYY-MM-DDTHH:mm:ss', 'DD/MM/YYYY') },
                { data: 'shift.name', title: 'Shift Name' },
                { data: 'roster.description', title: 'Description' },
            ];

            //(Table ID, Partial View ID, Button to open popup , url api to get list, column list (From above), class name where action function is defined)           
            DatatableService.initializeDataGridListingStatic('RosterTable', '/Roster/RosterAddEdit', variable.getRosterList, RosterColList, 'Roster',false,false,true);
            
            
        });
    }

    filterDatatable() {
        //get datepicker value
        var dateRange = $('#DateRangePicker').val();
        var dateRangeT = dateRange.split(" ");
        //format Date
        var rosFrom = this.formatDate(dateRangeT[0]);
        var rosTo = this.formatDate(dateRangeT[2]);
        //dto to send date from and To
        var ros = {
            RosterFrom: moment(rosFrom, "DD-MM-YYYY").add(4, 'hours'),
            RosterTo: moment(rosTo, "DD-MM-YYYY").add(4, 'hours')
        };
        ApiService.callCustom(ApiType.POST, variable.filterDatatableByRosterPeriod, ros, (data) => {
            if (data) {
                //update Datagrid
                DatatableService.updateDataGrid('RosterTable', data.data)
            }
        })
    }

    //format Date
    formatDate(dat) {
        const [month, day, year] = dat.split('/');
        const date = new Date(+year, +month - 1, +day);
        return date;
    }

    //open popup partial view
    openPopupAddEdit(Id) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().addEditRoster, Id, (data) => {
            if (data) {
                //set partial view content
                $(".content").html(data)
                //get details 
                RosterAddEdit.getRelationById($("#Id").val())
            }
        })
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

    //delete Roster
    delete(Id) {
        //show confirmation box for delete
        //Parameters: API URL, Id to delete(data to pass), module name 
        AlertBoxService.deleteAlertBox(new Variable().deleteRoster, Id, "Roster");
    }
}
window.Roster = new Roster();