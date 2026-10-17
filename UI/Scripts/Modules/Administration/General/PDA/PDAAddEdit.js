import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";

export class PDAAddEdit {//class that handles the Roster Add Edit screen
    //Global Variables
    relationArray = [];
    relationDataVeh = [];
    relationDataEquip = [];
    relationDataRank = [];
    Id = 0;

    constructor() {
        var vehicleColumn = [ //Relation Column List
            { data: 'VehicleCategory.name', title: 'Vehicle Category' },
            { data: 'VehicleQuantity', title: 'Quantity' },
        ];

        var equipmentColumn = [ //Relation Column List
            { data: 'EquipmentCategory.name', title: 'Equipment Category' },
            { data: 'EquipmentQuantity', title: 'Quantity' },
        ];

        var resourceColumn = [ //Relation Column List
            { data: 'name.order', title: 'Order', visible: false },
            { data: 'name.name', title: 'Rank' },
            { data: 'RankQuantity', title: 'Quantity' },
        ];

        //Init Relationship Datatable
        DatatableService.initializeDataGridDynamic('vehicle-Resource-R-Table', vehicleColumn, 'PDAAddEdit', this.relationDataVeh);

        //Init Day Selection Datatable
        DatatableService.initializeDataGridDynamic('equipment-Resource-R-Table', equipmentColumn, 'PDAAddEdit', this.relationDataEquip);

        DatatableService.initializeDataGridDynamic('resource-Resource-R-Table', resourceColumn, 'PDAAddEdit', this.relationDataRank, "", true, false, true);

        this.getURLparameters();
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
        };
        if (urlParams.has('View')) {
            if (parseInt(urlParams.get('View'))) {
                $(".noView").each(function (i, obj) {
                    obj.classList.add("d-none")
                });
            }
        };
        this.getDropdown();
        this.getRelationById();
    }

    //getDropdownValues
    getDropdown() {
        //Get Dropdown Value
        var dropdown = null;
        //API To get Dropdown Value
        ApiService.callCustom(ApiType.POST, variable.getDropDownPDA, "", (data) => {
            if (data) {
                dropdown = data.data;
                //Team Select
                DropdownService.initDropdown("select-station", dropdown, "StationType");
                $("#select-station").val($("#select-station-id").val()).trigger('change');

                //Resource Select
                DropdownService.initDropdown("select-incident", dropdown, "NatureOfIncident");
                $("#select-incident").val($("#select-incident-id").val()).trigger('change');

                //Inicdent Type Select
                DropdownService.initDropdown("select-incident-type", dropdown, "IncidentType");
                $("#select-incident-type").val($("#select-incident-type-id").val()).trigger('change');

                DropdownService.initDropdown("select-vehicle", dropdown, "VehicleCat");

                DropdownService.initDropdown("select-equipment", dropdown, "EquipmentCat");

                DropdownService.initDropdown("select-resource", dropdown, "Rank");
            }
        });


    }
    //API to get relation
    getRelationById() {
        let data = DefaultService.result(document.getElementById("PDA-AddEdit-Panel"));
        
        this.relationArray = [];
        this.relationData = [];
        if (this.Id > 0) {
            ApiService.callCustom(ApiType.POST, variable.getTableDetails, this.Id, (data) => {
              
                var veh = data.data.pdaVehicleMappingDTO;
                var vehList = [];
                $(veh).each(function () {
                    var check = $(this)[0];
                    let dat = {
                        id: check.id,
                        VehicleCategory: {
                            id: check.vehicleCategory.id,
                            name: check.vehicleCategory.name
                        },
                        VehicleQuantity: check.vehicleQuantity,
                        Fk_PDA: check.fk_PDA
                    }
                    vehList.push(dat);
                });
                this.relationDataVeh = vehList;
                DatatableService.updateDataGrid("vehicle-Resource-R-Table", this.relationDataVeh);

                var equip = data.data.pdaEquipmentMappingDTO;
                var equipList = [];
                $(equip).each(function () {
                    var checkequip = $(this)[0];
                    let datEquip = {
                        id: checkequip.id,
                        EquipmentCategory: {
                            id: checkequip.equipmentCategory.id,
                            name: checkequip.equipmentCategory.name
                        },
                        EquipmentQuantity: checkequip.equipmentQuantity,
                        Fk_PDA: checkequip.fk_PDA
                    }
                    equipList.push(datEquip);
                });
                this.relationDataEquip = equipList;
                DatatableService.updateDataGrid("equipment-Resource-R-Table", this.relationDataEquip);

                var rank = data.data.pdaResourceMappingDTO;
                var rankList = [];
                $(rank).each(function () {
                    var checkrank = $(this)[0];
                    let datRank = {
                        id: checkrank.id,
                        name: {
                            id: checkrank.name.id,
                            name: checkrank.name.name,
                            order: checkrank.name.order,
                        },
                        RankQuantity: checkrank.rankQuantity,
                        Fk_PDA: checkrank.fk_PDA
                    }
                    rankList.push(datRank);
                });
                this.relationDataRank = rankList;
                DatatableService.updateDataGrid("resource-Resource-R-Table", this.relationDataRank);
            })
        }

    }

    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["select-station", "select-incident", "select-incident-type"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }
        //return boolean id valid or not
        return valid
    }

    //Button save or update Roster
    saveUpdate() {
        // get all values
        let data = DefaultService.result(document.getElementById("PDA-AddEdit-Panel"));
        //check if required fields are input
        if (this.valid() && data != false) {
            data["Id"] = this.Id;
            data["PDAVehicleMappingDTO"] = this.relationDataVeh;
            data["PDAEquipmentMappingDTO"] = this.relationDataEquip;
            data["PDAResourceMappingDTO"] = this.relationDataRank;

            ApiService.callCustom(ApiType.POST, variable.createUpdatePDA, data, (response) => {
                //Response true
                if (response.data) {
                    toastr.success("Saved Successfully!")
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
    addRelationVeh() {
        if ($("#select-vehicle :selected").val() == "") {
            toastr.error("Please select Vehicle Category.");
            return;
        }
        let selectedResource = $("#select-vehicle :selected");
        let exist = this.relationDataVeh.find(x => x.resourceId == selectedResource.val());

        //reset value
        $("#select-vehicle").val("").trigger("change");

        if (exist != undefined) {
            toastr.error("Vehicle Category already assigned.");
            return;
        } else {
            let data = {
                id: 0,
                VehicleCategory: {
                    id: selectedResource.val(),
                    name: selectedResource.text()
                },
                VehicleQuantity: $("#vehQty").val(),
                Fk_PDA: 0

            }
            $("#vehQty").val("")
            this.relationDataVeh.push(data)
            DatatableService.updateDataGrid("vehicle-Resource-R-Table", this.relationDataVeh);
        }
    }

    addRelationEquip() {
        if ($("#select-equipment :selected").val() == "") {
            toastr.error("Please select Equipment Category.");
            return;
        }
        let selectedResource = $("#select-equipment :selected");
        let exist = this.relationDataEquip.find(x => x.resourceId == selectedResource.val());

        //reset value
        $("#select-equipment").val("").trigger("change");

        if (exist != undefined) {
            toastr.error("Equipment Category already assigned.");
            return;
        } else {
            let data = {
                id: 0,
                EquipmentCategory: {
                    id: selectedResource.val(),
                    name: selectedResource.text()
                },
                EquipmentQuantity: $("#equipQty").val(),
                Fk_PDA: 0

            }
            $("#equipQty").val("")
            this.relationDataEquip.push(data)
            DatatableService.updateDataGrid("equipment-Resource-R-Table", this.relationDataEquip);
        }
    }

    addRelationRank() {
        if ($("#select-resource :selected").val() == "") {
            toastr.error("Please select Rank.");
            return;
        }
        let selectedResource = $("#select-resource :selected");
        let exist = this.relationDataRank.find(x => x.resourceId == selectedResource.val());

        //reset value
        $("#select-resource").val("").trigger("change");

        if (exist != undefined) {
            toastr.error("Rank already assigned.");
            return;
        } else {
           
            let data = {
                id: 0,
                name: {
                    id: selectedResource.val(),
                    name: selectedResource.text(),
                    order: this.relationDataRank.length + 1
                },
                RankQuantity: $("#rankQty").val(),
                Fk_PDA: 0

            }
            $("#rankQty").val("")
            this.relationDataRank.push(data)
            DatatableService.updateDataGrid("resource-Resource-R-Table", this.relationDataRank);
        }
    }

   
    //Delete relation
    deleteRelation(meta) {
        this.relationDataVeh.splice(meta, 1);
        DatatableService.updateDataGrid("vehicle-Resource-R-Table", this.relationDataVeh);
    }
    //Delete Equip
    deleteRelationEquip(meta) {
        this.relationDataEquip.splice(meta, 1);
        DatatableService.updateDataGrid("equipment-Resource-R-Table", this.relationDataEquip);
    }

    //Delete rank
    deleteRelationRank(meta) {
        this.relationDataRank.splice(meta, 1);
        DatatableService.updateDataGrid("resource-Resource-R-Table", this.relationDataRank);
    }

    //Back button
    back() {
        //show loader when refirecting to listing
        location.href = "/PDA/List#"
    }
}
//init new Roster Edit Class
window.PDAAddEdit = new PDAAddEdit();