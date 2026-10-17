import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { MapService } from "/Scripts/Common/Map/MapService.js";
import { ApiTypes, TransactionType, TransactionStatus } from "/Scripts/Common/Enums.js";
import { xhrService } from "/Scripts/Common/xhrService.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
export class CallHandling {//class that handles the screen
    IncidentId = 0;
    stationId = 0;
    InitialRender = true;
    stations = [];
    vehicles = [];
    equipments = [];
    resources = [];
    externalAgencies = [];
    casualties = [];
    fatalities = [];
    damages = [];
    files = [];
    chats = [];
    IsView = false;
    constructor() {
        $(document).ready(() => {
            //Get All Dropdown Values
            this.getDropdowns();
            this.InitialiseTables();
            this.InitialiseMap();
        });

        $('#CallerContact').on('focusout', () => {
            this.getCallerInformation();
        });
        $('#EstimatedValue').on('focusout', () => {
            this.getSalvage();
        });
        $('#DamageCost').on('focusout', () => {
            this.getSalvage();
        });
        //Vehicle category Selection
        $('#select-vehicle-Category, #select-vehicle-StationId').on('select2:select', () => {
            this.getVehicleByCategory(false)
        });
        $('#select-vehicle-Category, #select-vehicle-StationId').on('select2:clear', () => {
            this.getVehicleByCategory(true)
        });
        //Equipment category Selection
        $('#select-equipment-Category').on('select2:select', () => {
            this.getEquipmentByCategory(false)
        });
        $('#select-equipment-Category').on('select2:clear', () => {
            this.getEquipmentByCategory(true)
        });
        //Resource By Rank Selection
        $('#select-resource-Rank').on('select2:select', () => {
            this.getResourceByRank(false)
        });
        $('#select-resource-Rank').on('select2:clear', () => {
            this.getResourceByRank(true)
        });
        //Resource By category Selection
        $('#select-resource-Category').on('select2:select', () => {
            this.getResourceByCategory(false)
        });
        $('#select-resource-Category').on('select2:clear', () => {
            this.getResourceByCategory(true)
        });
        //Station Selection
        $('#select-StationId').on('select2:select', () => {
            this.setAllStation(false)
            this.getPDADetails()
        });
        $('#select-StationId').on('select2:clear', () => {
            this.setAllStation(true)
            this.getPDADetails()
        });
        //Nature of incident Selection
        $('#select-NatureOfIncidentId').on('select2:select', () => {
            this.getPDADetails()
        });
        $('#select-NatureOfIncidentId').on('select2:clear', () => {
            this.getPDADetails()
        });
        //InicdentType Selection
        $('#select-IncidentType').on('select2:select', () => {
            this.getPDADetails()
        });
        $('#select-IncidentType').on('select2:clear', () => {
            this.getPDADetails()
        });
    }

    //open scrollspy click
    openScrollspy(id) {
        $("#" + id).addClass("show");
    }

    //open scrollspy click
    clickScrollspy(id) {
        $("#" + id).addClass("show");
    }

    //funtion to check whether this page is an edit or a create
    getURLparameters() {
        const queryString = window.location.search;
        //url Parameters
        const urlParams = new URLSearchParams(queryString);
        // if url contains id, switch to edit mode
        if (urlParams.has('Id')) {
            //get Id from URL
            this.IncidentId = parseInt(urlParams.get('Id'))
            $("#Id").val(this.IncidentId)
            this.setDropdownOnEdit();
            this.getTablesData();
            this.getCallerInformationById();
            this.getPDADetails();
            this.getRefreshedDataFile();
            this.getRefreshedDataChat();
            this.getRequestMobile();
            setInterval(e => {
                this.getRefreshedDataFile();
            }, 30000);
            setInterval(e => {
                this.getRefreshedDataChat();
            }, 5000);
            setInterval(e => {
                this.getRequestMobile();
            }, 10000);
            if ($("#select-StationId").val() > 0) {
                this.stationId = $("#select-StationId").val();
            }
            this.InitialRender = false;
        };
        if (urlParams.has('View')) {
            if (parseInt(urlParams.get('View'))) {
                $(".noView").each(function (i, obj) {
                    obj.classList.add("d-none")
                });
                this.IsView = true;
                Validation.setDisabledToListOfFields([
                    //Incident Details
                    "Code",
                    "select-IncidentType",
                    "InitialIncidentType",
                    "select-IncidentSubType",
                    "select-IncidentCode",
                    "select-ParentIncidentId",
                    "IncidentRemarks",
                    //Location Info
                    "Address",
                    "LandMark",
                    //Caller Info
                    "CallerContact",
                    "CallerFirstName",
                    "CallerLastName",
                    "CallerAddress",
                    "CallerStreet",
                    "select-caller-Locality",
                    "select-caller-District",
                    "CallerRemarks",
                    "CallerNuisance",
                    //Fire Station
                    "select-StationId",
                    "select-NatureOfIncidentId",
                    //Vehicle Info
                    "select-vehicle-StationId",
                    "select-vehicle-Category",
                    "select-vehicle",
                    "vehicle-add-btn",
                    //Equipment info
                    "select-equipment-StationId",
                    "select-equipment-Category",
                    "select-equipment",
                    "equipment-add-btn",
                    //Resource Info
                    "select-resource-StationId",
                    "select-resource-Category",
                    "select-resource-Rank",
                    "select-resource",
                    "ResourceCrewCommander",
                    "resource-add-btn",
                    //External Agency
                    "select-external-agency",
                    "ExternalRemarks",
                    "external-add-btn",
                    //Casualty Info
                    "TotalCasualties",
                    "TotalRescued",
                    "TotalEvacuated",
                    "IncidentCasualtyRemarks",
                    "select-casualty-Type",
                    "select-casualty-Gender",
                    "CasualtyAge",
                    "CasualtyName",
                    "CasualtyAddress",
                    "CasualtyRemarks",
                    "casualty-add-btn",
                    //Fatality Info
                    "TotalDeath",
                    "select-fatality-Type",
                    "select-fatality-Gender",
                    "FatalityAge",
                    "FatalityName",
                    "FatalityAddress",
                    "FatalityRemarks",
                    "fatality-add-btn",
                    //Damage Detail
                    "DamageDetails",
                    "DamageCost",
                    "damage-add-btn",
                    "EstimatedValue",
                    //File Information
                    "FileFile",
                    "file-add-btn",
                    //chat information
                    "chat-message",
                    "chat-send-btn",
                    //final Report
                    "IncidentReportRemarks"
                ]);
                $(".leaflet-draw-toolbar").addClass("d-none")
                $('.RequestTable-checkbox').prop('disabled', true);
            }
        };
        if (urlParams.has('Field')) {
            let fieldId = urlParams.get('Field');
            this.openScrollspy(fieldId);

        }
    }

    //get table adta
    getTablesData() {
        ApiService.callCustom(ApiType.POST, variable.getCallhandlingByIdDetails, this.IncidentId, (response) => {
            // if response true: Save successfull
            if (response.data) {
                this.stations = response.data.incidentOtherStation;
                DatatableService.updateDataGrid("StationTable", this.stations);
                this.vehicles = response.data.incidentVehicle;
                DatatableService.updateDataGrid("VehicleTable", this.vehicles);
                this.equipments = response.data.incidentEquipment;
                DatatableService.updateDataGrid("EquipmentTable", this.equipments);
                this.resources = response.data.incidentResource;
                this.resources.forEach(rsc => {
                    rsc.isCrewCommander = rsc.isCrewCommander == true ? "Yes" : "No";
                })
                DatatableService.updateDataGrid("ResourceTable", this.resources);
                this.externalAgencies = response.data.incidentExternalAgency;
                DatatableService.updateDataGrid("ExternalAgencyTable", this.externalAgencies);
                this.casualties = response.data.incidentCasualtyDetails;
                DatatableService.updateDataGrid("CasualtyTable", this.casualties);
                this.fatalities = response.data.incidentFatalityDetails;
                DatatableService.updateDataGrid("FatalityTable", this.fatalities);
                this.damages = response.data.incidentDamage;
                DatatableService.updateDataGrid("DamageTable", this.damages);
                //this.stations = response.data.incidentVehicle;
                //DatatableService.updateDataGrid("CallerTransactionTable", this.resources);

            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }

    //get Data refreshed
    getRefreshedDataFile() {
        var data = {
            "data": this.IncidentId,
            UserToken: localStorage.getItem("userToken")
        }
        xhrService.xhrRequestCustom(ApiTypes.POST, variable.getCallhandlingByIdRefreshed, data).then(response => {
            if (response.data) {
                if (response.data.filesList.length > 0) {
                    this.files = response.data.filesList;
                    DatatableService.updateDataGrid("FileTable", this.files);
                }
            } else {
                toastr.error("Cannot display files. Please contact admin.")
            }
        })
    }

    //get Data refreshed
    getRefreshedDataChat() {
        var data = {
            "data": this.IncidentId,
            UserToken: localStorage.getItem("userToken")
        }
        xhrService.xhrRequestCustom(ApiTypes.POST, variable.getChatlistIncident, data).then(response => {
            if (response.data) {
                if (response.data.length > 0) {
                    this.setChatBox(response.data);
                }
            } else {
                toastr.error("Cannot display files. Please contact admin.")
            }
        })
    }

    //Request To CR Checbox
    getRequestMobile() {
        var data = {
            "data": this.IncidentId,
            UserToken: localStorage.getItem("userToken")
        }
        xhrService.xhrRequestCustom(ApiTypes.POST, variable.mobileRequestIncident, data).then(response => {
            if (response.data) {
                if (response.data.length > 0) {
                    DatatableService.updateDataGrid("RequestTable", response.data);
                    if (this.IsView) {
                        $('.RequestTable-checkbox').prop('disabled', true);
                    }
                }
            } else {
                toastr.error("An Error occurred. Please contact admin.")
            }
        })
    }


    //set chat in message box
    setChatBox(chatList = []) {
        $(".imessage").html("");
        this.chats = [];
        chatList.forEach(chat => {
            if (chat["isSender"] == true) {
                let NameContentFromMe = DefaultService.createElement("span", { class: "info-message-from-me span-smaller" }, [], { innerHTML: chat["name"] + " - " + chat["timeStamp"] })
                this.chats.push(NameContentFromMe)
                let messageContentFromMe = DefaultService.createElement("p", { class: "from-me" }, [], { innerHTML: chat["text"] })
                this.chats.push(messageContentFromMe)
            } else {
                let NameContentFromThem = DefaultService.createElement("span", { class: "info-message-from-them span-smaller" }, [], { innerHTML: chat["timeStamp"] + " - " + chat["name"] })
                this.chats.push(NameContentFromThem)
                let messageContentFromThem = DefaultService.createElement("p", { class: "from-them" }, [], { innerHTML: chat["text"] })
                this.chats.push(messageContentFromThem)
            }
        })
        $(".imessage").html(this.chats);
    }

    // set Edited Dropdown Values
    setDropdownOnEdit() {
        //Set Dropdown Value on edit 
        if ($("#SelectedIncidentSubType").val() != '') {
            let text = $("#SelectedIncidentSubType").val();
            let id = $("#select-IncidentSubType-id").val();
            DropdownService.search_value('select-IncidentSubType', text, id);
        }
        //Set Dropdown Value on edit 
        if ($("#SelectedIncidentCode").val() != '') {
            let text = $("#SelectedIncidentCode").val();
            let id = $("#select-IncidentCode-id").val();
            DropdownService.search_value('select-IncidentCode', text, id);
        }
    }

    //get all dropdown values
    getDropdowns() {
        var dropdown = null;

        DropdownService.initDropdownDynamic("select-IncidentSubType", variable.getCallhandlingIncidentSubType, "IncidentSubType", "select-IncidentType", true);

        DropdownService.initDropdownDynamic("select-IncidentCode", variable.getCallhandlingIncidentCode, "IncidentCode", "select-IncidentSubType", true);
        ApiService.callCustom(ApiType.POST, variable.getCallHandlingDropdown, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {
                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-IncidentType", dropdown, "IncidentType");
                    $("#select-IncidentType").val($("#select-IncidentType-id").val()).trigger('change');

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-ParentIncidentId", dropdown, "Incident");
                    $("#select-ParentIncidentId").val($("#select-ParentIncidentId-id").val()).trigger('change');

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-caller-Locality", dropdown, "Locality");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-caller-District", dropdown, "District");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-StationId", dropdown, "Station");
                    $("#select-StationId").val($("#select-StationId-id").val()).trigger('change');

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-NatureOfIncidentId", dropdown, "NatureOfIncident");
                    $("#select-NatureOfIncidentId").val($("#select-NatureOfIncidentId-id").val()).trigger('change');

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-vehicle-StationId", dropdown, "Station");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-vehicle-Category", dropdown, "VehicleCategory");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdownNull("select-vehicle");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-equipment-StationId", dropdown, "Station");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-equipment-Category", dropdown, "EquipmentCategory");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdownNull("select-equipment");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-resource-StationId", dropdown, "Station");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-resource-Category", dropdown, "ResourceCategory");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-resource-Rank", dropdown, "ResourceRank");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdownNull("select-resource");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-external-agency", dropdown, "ExternalAgency");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-casualty-Type", dropdown, "Casualty");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-casualty-Gender", dropdown, "Gender");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-fatality-Type", dropdown, "Fatality");

                    // Set list of values & Init with value on edit
                    DropdownService.initDropdown("select-fatality-Gender", dropdown, "Gender");
                }
                //call urlParameter funtion to check whether this page is an edit or a create
                //used to populate data 
                this.getURLparameters();
            }
        });
    }

    //Init Tables
    InitialiseTables() {
        let StationCols = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'incidentId', title: 'Id', visible: false },
            { data: 'otherStationId', title: 'Id', visible: false },
            { data: 'name', title: 'Name' }
        ]
        DatatableService.initializeDataGridDynamic("StationTable", StationCols, "CallHandling", null, "", false);

        let VehicleCols = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'incidentId', title: 'Id', visible: false },
            { data: 'vehicleId', title: 'Id', visible: false },
            { data: 'name', title: 'Name' },
            { data: 'stationId', title: 'Station Id', visible: false },
            { data: 'stationName', title: 'Station Name' }
        ]
        DatatableService.initializeDataGridDynamic("VehicleTable", VehicleCols, "CallHandling", null, "", true, false, false, "deleteVehicle");

        let EquipmentCols = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'incidentId', title: 'Id', visible: false },
            { data: 'equipmentId', title: 'Id', visible: false },
            { data: 'name', title: 'Name' },
            { data: 'stationId', title: 'Station Id', visible: false },
            { data: 'stationName', title: 'Station Name' }
        ]
        DatatableService.initializeDataGridDynamic("EquipmentTable", EquipmentCols, "CallHandling", null, "", true, false, false, "deleteEquipment");

        let ResourceCols = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'incidentId', title: 'Id', visible: false },
            { data: 'resourceId', title: 'Id', visible: false },
            { data: 'name', title: 'Name' },
            { data: 'isCrewCommander', title: 'Is Crew Commander?' },
            { data: 'stationId', title: 'Station Id', visible: false },
            { data: 'stationName', title: 'Station Name' }
        ]
        DatatableService.initializeDataGridDynamic("ResourceTable", ResourceCols, "CallHandling", null, "", true, false, false, "deleteResource");

        let ExternalAgencyCols = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'incidentId', title: 'Id', visible: false },
            { data: 'externalAgencyId', title: 'Id', visible: false },
            { data: 'name', title: 'Name' },
            { data: 'contactNumber', title: 'Contact Number' },
            { data: 'remarks', title: 'Remarks' }
        ]
        DatatableService.initializeDataGridDynamic("ExternalAgencyTable", ExternalAgencyCols, "CallHandling", null, "", true, false, false, "deleteExternalAgency");

        let CasualtyCols = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'incidentId', title: 'Id', visible: false },
            { data: 'gender', title: 'Id', visible: false },
            { data: 'casualtyType', title: 'Id', visible: false },
            { data: 'genderName', title: 'Gender' },
            { data: 'casualtyTypeName', title: 'Casualty Type' },
            { data: 'age', title: 'Age' },
            { data: 'name', title: 'Name' },
            { data: 'address', title: 'Address' },
            { data: 'remarks', title: 'Remarks' }
        ]
        DatatableService.initializeDataGridDynamic("CasualtyTable", CasualtyCols, "CallHandling", null, "", true, false, false, "deleteCasualty");

        let FatalityCols = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'incidentId', title: 'Id', visible: false },
            { data: 'gender', title: 'Id', visible: false },
            { data: 'fatalityType', title: 'Id', visible: false },
            { data: 'genderName', title: 'Gender' },
            { data: 'fatalityTypeName', title: 'Fatality Type' },
            { data: 'age', title: 'Age' },
            { data: 'name', title: 'Name' },
            { data: 'address', title: 'Address' },
            { data: 'remarks', title: 'Remarks' }
        ]
        DatatableService.initializeDataGridDynamic("FatalityTable", FatalityCols, "CallHandling", null, "", true, false, false, "deleteFatality");

        let DamageCols = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'incidentId', title: 'Id', visible: false },
            { data: 'details', title: 'Details' },
            { data: 'estimatedValue', title: 'Estimated Value' },
            { data: 'cost', title: 'Damage Cost' },
            { data: 'salvage', title: 'Salvage' }
        ]
        DatatableService.initializeDataGridDynamic("DamageTable", DamageCols, "CallHandling", null, "", true, false, false, "deleteDamage");

        let FileCols = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'fileName', title: 'File Name' }
        ]
        DatatableService.initializeDataGridDynamic("FileTable", FileCols, "CallHandling", null, "", true, false, false, "deleteFile", '<a onclick="CallHandling.downloadFile([ID])" title="Download"><i class="fa fa-download text-secondary ms-2"></i></a> ');

        let CallerCols = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'incidentNo', title: 'Incident No' },
            { data: 'incidentType', title: 'Incident Type' },
            { data: 'dateCreated', title: 'Date/Time' },
            { data: 'location', title: 'Location' },
            { data: 'status', title: 'Status' }
        ]
        DatatableService.initializeDataGridDynamic("CallerTransactionTable", CallerCols, "CallHandling", null, "", false);

        let RequestCols = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'type', title: 'Request Type' },
            { data: 'category', title: 'Category' },
            { data: 'datetime', title: 'Date/Time' },
            //{ data: 'remarks', title: 'Remarks' },
            {
                title: "Completed",
                //remove sorting on action
                orderable: false,
                //set width of action column
                width: "10%",
                render: function (data, type, full, meta) {
                    var checked = "";
                    if (full.completed) {
                        checked = "checked";
                    }
                    // checkbox button
                    var datax = '<input type="checkbox" class="RequestTable-checkbox" id="' + full.id + '" name="checkbox" ' + checked + ' onclick="CallHandling.requestCheck(\'' + full.id + '\')"/>';
                    return datax;
                }
            }
        ]
        DatatableService.initializeDataGridDynamic("RequestTable", RequestCols, "CallHandling", null, "", false, false, false,);
    }

    //Init Map
    InitialiseMap() {
        MapService.initPinMap();
    }

    //get Caller Information
    getCallerInformation() {
        var datum = $("#CallerContact").val();
        if (datum != "") {
            ApiService.callCustom(ApiType.POST, variable.getCallerByMobile, datum, (data) => {
                if (data.data != null) {
                    var caller = data.data;
                    if (caller.id != 0) {
                        this.setCallerInformation(caller)

                        ApiService.callCustom(ApiType.POST, variable.getCallerLastTransactions, caller.id, (resp) => {
                            if (resp) {
                                var CallerTransactions = resp.data;
                                DatatableService.updateDataGrid('CallerTransactionTable', CallerTransactions);
                            }
                        });
                    } else {
                        this.setCallerInformation()
                    }

                    this.saveIncidentCallerInformation(caller.id)


                } else {
                    this.setCallerInformation()
                    $("#CallerId").val("")
                    $("#caller-add-btn").removeClass("d-none")
                }
            })

        } else {
            this.setCallerInformation(null)
        }

    }

    //get Caller Information By Id
    getCallerInformationById() {
        var datum = $("#CallerId").val();
        if (datum != "") {
            ApiService.callCustom(ApiType.POST, variable.getCallerById, datum, (data) => {
                if (data.data != null) {
                    var caller = data.data;
                    if (caller.id != 0) {
                        this.setCallerInformation(caller)
                    } else {
                        this.setCallerInformation()
                    }
                    ApiService.callCustom(ApiType.POST, variable.getCallerLastTransactions, caller.id, (resp) => {
                        if (resp) {
                            var CallerTransactions = resp.data;
                            DatatableService.updateDataGrid('CallerTransactionTable', CallerTransactions);
                        }
                    });

                } else {
                    this.setCallerInformation()
                }
            })
        }
    }

    //set Caller Information Field
    setCallerInformation(caller = null) {
        if (caller == null) {
            $("#CallerFirstName").val("")
            $("#CallerLastName").val("")
            $("#CallerAddress").val("")
            $("#CallerStreet").val("")
            $("#select-caller-Locality").val("").trigger('change');
            $("#select-caller-District").val("").trigger('change');
            $("#CallerRemarks").val("")
            $("#CallerNuisance").attr("checked", false)
            DatatableService.updateDataGrid('CallerTransactionTable', []);
        } else {
            $("#CallerContact").val(caller.mobileNo)
            $("#CallerId").val(caller.id)
            $("#CallerFirstName").val(caller.firstName)
            $("#CallerLastName").val(caller.lastName)
            $("#CallerAddress").val(caller.address)
            $("#CallerStreet").val(caller.street)
            $("#select-caller-Locality").val(caller.localityId).trigger('change');
            $("#select-caller-District").val(caller.districtId).trigger('change');
            $("#CallerRemarks").val(caller.remarks)
            if (caller.isNuisance) {
                $("#CallerNuisance").attr("checked", true)
            }
            $("#caller-add-btn").addClass("d-none")
            ApiService.callCustom(ApiType.POST, variable.getCallerLastTransactions, caller.id, (resp) => {
                if (resp) {
                    var CallerTransactions = resp.data;
                    DatatableService.updateDataGrid('CallerTransactionTable', CallerTransactions);
                }
            });
        }

    }

    //Save Caller Information
    saveCallerInformation() {
        let data = DefaultService.result(document.getElementById("caller"));
        if (this.validCaller() && data != false) {
            data["Id"] = 0;
            data["Code"] = "";
            data["Identifier"] = data["MobileNo"];
            data["NICPassport"] = "";
            if (data["DistrictId"] != 0) {
                data["District"] = $("select-caller-District option:selected").text();
            } else {
                data["District"] = "";
            }
            if (data["LocalityId"] != 0) {
                data["Locality"] = $("select-caller-Locality option:selected").text();
            } else {
                data["Locality"] = "";
            }

            ApiService.callCustom(ApiType.POST, variable.createUpdateCaller, data, (response) => {
                //Response true
                if (response.data) {
                    toastr.success("Saved Successfully!")
                    if (response.id > 0) {
                        this.saveIncidentCallerInformation(response.id)
                    }

                }
                //response false
                else if (!response.data) {
                    toastr.error(response.additionalData);
                }
                //failed
                else {
                    toastr.error("An Error occurred. Please contact admin.");
                    return false;
                }
            }, () => {
                Loader.hideLoader();
            })
        }
    }

    //Incident Update Caller Infor
    saveIncidentCallerInformation(callerid) {
        let CallerData = {
            IncidentCaller: {
                CallerId: callerid,
                IncidentId: this.IncidentId,
                Id: $("#IncidentCallerId").val() == "" ? 0 : parseInt($("#IncidentCallerId").val())
            }
        }
        ApiService.callCustom(ApiType.POST, variable.saveCallHandling, CallerData, (response) => {
            // if response true: Save successfull
            if (response.data) {
                $("#IncidentCallerId").val(response.id)
                //toastr.success("Saved Successfully")
            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }

    //validate Caller information before save
    validCaller() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["CallerContact"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }
        valid = valid;

        //return boolean id valid or not
        return valid
    }

    //Vehicle category Selection
    getVehicleByCategory(isClear) {
        if (isClear) {
            DropdownService.destroyReinitDropdownNull("select-vehicle")
        } else {
            if ($("#select-vehicle-StationId").val() == '' || $("#select-vehicle-Category").val() == '') {
                toastr.info("Please select Station and Category to get list of vehicle")
            } else {
                let data = {
                    Parameter1: $("#select-vehicle-StationId").val(),
                    Parameter2: $("#select-vehicle-Category").val()
                }
                ApiService.callCustom(ApiType.POST, variable.getVehicleByCategory, data, (resp) => {
                    if (resp) {
                        DropdownService.destroyReinitDropdownNull("select-vehicle", resp.data, "Vehicle")
                    }
                });
            }

        }
    }

    //Equipment category Selection
    getEquipmentByCategory(isClear) {
        if (isClear) {
            DropdownService.destroyReinitDropdownNull("select-equipment")
        } else {
            if ($("#select-equipment-StationId").val() == '' || $("#select-equipment-Category").val() == '') {
                toastr.info("Please select Station and Category to get list of equipment")
            } else {
                let data = {
                    Parameter1: $("#select-equipment-StationId").val(),
                    Parameter2: $("#select-equipment-Category").val()
                }
                ApiService.callCustom(ApiType.POST, variable.getEquipmentByCategory, data, (resp) => {
                    if (resp) {
                        DropdownService.destroyReinitDropdownNull("select-equipment", resp.data, "Equipment")
                    }
                });
            }

        }
    }

    //Resource By Rank Selection
    getResourceByRank(isClear) {
        if (isClear) {
            DropdownService.destroyReinitDropdownNull("select-resource")
        } else {
            let data = {
                Parameter1: $("#select-resource-StationId").val(),
                Parameter2: $("#select-resource-Rank").val()
            }
            ApiService.callCustom(ApiType.POST, variable.getResourceByRank, data, (resp) => {
                if (resp) {
                    DropdownService.destroyReinitDropdownNull("select-resource", resp.data, "Resource")
                }
            });
        }
        if ($("#select-resource-StationId").val() != '') {
            let station = {
                id: 0,
                incidentId: this.IncidentId,
                otherStationId: $("#select-resource-StationId").val(),
                name: $("#select-resource-StationId option:selected").val()
            };
        }
    }

    //Resource By category Selection
    getResourceByCategory(isClear) {
        if (isClear) {
            DropdownService.destroyReinitDropdownNull("select-resource")
        } else {
            let data = {
                Parameter1: $("#select-resource-StationId").val(),
                Parameter2: $("#select-resource-Category").val()
            }
            ApiService.callCustom(ApiType.POST, variable.getResourceByCategory, data, (resp) => {
                if (resp) {
                    DropdownService.destroyReinitDropdownNull("select-resource", resp.data, "Resource")
                }
            });
        }
    }

    //set station list
    setStationList(type, stationObj) {
        let data = {
            Id: stationObj.id,
            IncidentId: stationObj.incidentId,
            OtherStationId: stationObj.otherStationId
        }
        if (this.stationId == 0) {
            if (type == TransactionType.Add) {
                ApiService.callCustom(ApiType.POST, variable.addDeleteOtherStationToTable, data, (response) => {
                    // if response true: Save successfull
                    if (response.data) {
                        stationObj["id"] = response.id;
                        this.stations.push(stationObj);
                        DatatableService.updateDataGrid('StationTable', this.stations);
                    }
                    // if response false: Error
                    else {
                        toastr.error("An Error Occurred. Please contact Admin")
                    }
                })

            } else {
                this.stations = this.stations.filter(x => x["otherStationId"] != stationObj["otherStationId"])
                DatatableService.updateDataGrid('StationTable', this.stations);
            }
        } else {
            if (type == TransactionType.Add) {
                if (this.stationId != stationObj["otherStationId"]) {
                    if (this.stations.find(x => x["otherStationId"] == stationObj["otherStationId"]) == null) {
                        ApiService.callCustom(ApiType.POST, variable.addDeleteOtherStationToTable, data, (response) => {
                            // if response true: Save successfull
                            if (response.data) {
                                stationObj["id"] = response.id;
                                this.stations.push(stationObj);
                                DatatableService.updateDataGrid('StationTable', this.stations);
                            }
                            // if response false: Error
                            else {
                                toastr.error("An Error Occurred. Please contact Admin")
                            }
                        })
                    }
                }
            } else {
                this.stations = this.stations.filter(x => x["otherStationId"] != stationObj["otherStationId"])
                DatatableService.updateDataGrid('StationTable', this.stations);
            }
        }

    }

    //Set All Station Dropdown
    setAllStation(isClear) {
        if (isClear) {
            $("#select-vehicle-StationId").val("").trigger('change');
            $("#select-equipment-StationId").val("").trigger('change');
            $("#select-resource-StationId").val("").trigger('change');
            this.stationId = 0;
        } else {
            let stationId = $("#select-StationId").val();
            $("#select-vehicle-StationId").val(stationId).trigger('change');
            $("#select-equipment-StationId").val(stationId).trigger('change');
            $("#select-resource-StationId").val(stationId).trigger('change');
            this.stationId = stationId;
        }
    }

    //Get PDA
    getPDADetails() {

        let data = {
            StationId: $('#select-StationId option:selected').val(),
            NatureofIncidentId: $('#select-NatureOfIncidentId option:selected').val(),
            IncidentTypeId: $('#select-IncidentType option:selected').val()
        }
        $("#vehicle-PDA").html("");
        $("#equipment-PDA").html("");
        $("#resource-PDA").html("");

        if (data.StationId != "" && data.NatureofIncidentId != "" && data.IncidentTypeId != "") {
            ApiService.callCustom(ApiType.POST, variable.getIncidentPDA, data, (response) => {
                // if response true: Save successfull
                if (response.data) {
                    $("#vehicle-PDA").html(response.data.vehicle);
                    $("#equipment-PDA").html(response.data.equipment);
                    $("#resource-PDA").html(response.data.resource);
                }
                // if response false: Error
                else {
                    toastr.error("An Error Occurred. Please contact Admin")
                }
            })
        } 
    }

    //Add Vehicle To Datatable
    addVehicleToTable() {
        if ($("#select-vehicle").val() != '') {
            var isExist = this.vehicles.filter(x => x.vehicleId == parseInt($("#select-vehicle").val()));
            if (isExist.length == 0) {

                let data = {
                    Id: 0,
                    IncidentId: this.IncidentId,
                    VehicleId: parseInt($("#select-vehicle").val()),
                    StationId: parseInt($("#select-vehicle-StationId").val()),
                    StationName: $("#select-vehicle-StationId option:selected").text()
                }
                ApiService.callCustom(ApiType.POST, variable.addDeleteVehicleToTable, data, (response) => {
                    // if response true: Save successfull
                    if (response.data) {
                        let datum = {
                            id: response.id,
                            incidentId: this.IncidentId,
                            vehicleId: parseInt($("#select-vehicle").val()),
                            name: $("#select-vehicle option:selected").text(),
                            stationId: parseInt($("#select-vehicle-StationId").val()),
                            stationName: $("#select-vehicle-StationId option:selected").text(),
                        }
                        this.vehicles.push(datum)
                        DatatableService.updateDataGrid('VehicleTable', this.vehicles);
                        let dat = {
                            id: 0,
                            incidentId: this.IncidentId,
                            otherStationId: $("#select-vehicle-StationId").val(),
                            name: $("#select-vehicle-StationId option:selected").text(),
                        }
                        this.setStationList(TransactionType.Add, dat)
                        $("#select-vehicle-StationId").val("").trigger("change");
                        $("#select-vehicle-Category").val("").trigger("change");
                        $("#select-vehicle").val("").trigger("change");

                        let AssignmentData = {
                            AssignmentType: 3,
                            Id: datum.vehicleId
                        }
                        this.getEquipmentListByAssignment(AssignmentData)
                    }
                    // if response false: Error
                    else {
                        toastr.error("An Error Occurred. Please contact Admin")
                    }
                })
            } else {
                toastr.warning("Vehicle Already exist in table. Please choose another vehicle.")
            }

        } else {
            toastr.warning("Please Select a vehicle")
        }

    }


    //delete From Vehicle Datatable
    deleteVehicle(meta) {
        let vehicle = this.vehicles[meta];
        let data = {
            Id: vehicle.id,
            IncidentId: vehicle.incidentId,
            VehicleId: vehicle.vehicleId
        }
        ApiService.callCustom(ApiType.POST, variable.addDeleteVehicleToTable, data, (response) => {
            // if response true: Save successfull
            if (response.data) {
                this.vehicles.splice(meta, 1);
                DatatableService.updateDataGrid("VehicleTable", this.vehicles);
            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }
    //get Equipment list by assignment
    getEquipmentListByAssignment(data) {
        ApiService.callCustom(ApiType.POST, variable.getEquipmentByAssignment, data, (response) => {
            // if response true: Save successfull
            if (response.data) {
                if (response.data.length > 0) {
                    for (let ret of response.data) {
                        var isExist = this.equipments.filter(x => x.equipmentId == ret.id);
                        if (isExist.length == 0) {
                            let dat = {
                                Id: 0,
                                IncidentId: this.IncidentId,
                                EquipmentId: ret.id,
                                StationId: ret.stationId,
                                StationName: ret.stationName,
                            }
                            ApiService.callCustom(ApiType.POST, variable.addDeleteEquipmentToTable, dat, (response) => {
                                // if response true: Save successfull
                                if (response.data) {
                                    let datum = {
                                        id: response.id,
                                        incidentId: this.IncidentId,
                                        equipmentId: ret.id,
                                        name: ret.name,
                                        stationId: ret.stationId,
                                        stationName: ret.stationName
                                    }
                                    this.equipments.push(datum)
                                    DatatableService.updateDataGrid('EquipmentTable', this.equipments);

                                }
                                // if response false: Error
                                else {
                                    toastr.error("An Error Occurred. Please contact Admin")
                                }
                            })
                        }
                    }
                }
            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })

    }
    //Add Equipment To Datatable
    addEquipmentToTable() {
        if ($("#select-equipment").val() != '') {
            var isExist = this.equipments.filter(x => x.equipmentId == parseInt($("#select-equipment").val()));
            if (isExist.length == 0) {
                let data = {
                    Id: 0,
                    IncidentId: this.IncidentId,
                    EquipmentId: parseInt($("#select-equipment").val()),
                    StationId: parseInt($("#select-vehicle-StationId").val()),
                    StationName: $("#select-vehicle-StationId option:selected").text()
                }
                ApiService.callCustom(ApiType.POST, variable.addDeleteEquipmentToTable, data, (response) => {
                    // if response true: Save successfull
                    if (response.data) {
                        let datum = {
                            id: response.id,
                            incidentId: this.IncidentId,
                            equipmentId: parseInt($("#select-equipment").val()),
                            name: $("#select-equipment option:selected").text(),
                            stationId: parseInt($("#select-equipment-StationId").val()),
                            stationName: $("#select-equipment-StationId option:selected").text(),
                        }
                        this.equipments.push(datum)
                        DatatableService.updateDataGrid('EquipmentTable', this.equipments);
                        let dat = {
                            id: 0,
                            incidentId: this.IncidentId,
                            otherStationId: $("#select-equipment-StationId").val(),
                            name: $("#select-equipment-StationId option:selected").text(),
                        }
                        this.setStationList(TransactionType.Add, dat)

                        $("#select-equipment-StationId").val("").trigger("change");
                        $("#select-equipment-Category").val("").trigger("change");
                        $("#select-equipment").val("").trigger("change");
                    }
                    // if response false: Error
                    else {
                        toastr.error("An Error Occurred. Please contact Admin")
                    }
                })
            } else {
                toastr.warning("Equipment Already exist in table. Please choose another equipment.")
            }

        } else {
            toastr.warning("Please Select an Equipment")
        }
    }

    //delete From Equipment Datatable
    deleteEquipment(meta) {
        let equipment = this.equipments[meta];
        let data = {
            Id: equipment.id,
            IncidentId: equipment.incidentId,
            EquipmentId: equipment.equipmentId
        }
        ApiService.callCustom(ApiType.POST, variable.addDeleteEquipmentToTable, data, (response) => {
            // if response true: Save successfull
            if (response.data) {
                this.equipments.splice(meta, 1);
                DatatableService.updateDataGrid("EquipmentTable", this.equipments);
            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }

    //Add Resource To datatable
    addResourceToTable() {
        if ($("#select-resource").val() != '') {
            var isExist = this.resources.filter(x => x.resourceId == parseInt($("#select-resource").val()));
            if (isExist.length == 0) {
                let data = {
                    Id: 0,
                    IncidentId: this.IncidentId,
                    ResourceId: parseInt($("#select-resource").val()),
                    IsCrewCommander: $("#ResourceCrewCommander").prop("checked"),
                    StationId: parseInt($("#select-vehicle-StationId").val()),
                    StationName: $("#select-vehicle-StationId option:selected").text()
                }
                ApiService.callCustom(ApiType.POST, variable.addDeleteResourceToTable, data, (response) => {
                    // if response true: Save successfull
                    if (response.data) {
                        let datum = {
                            id: response.id,
                            incidentId: this.IncidentId,
                            resourceId: parseInt($("#select-resource").val()),
                            name: $("#select-resource option:selected").text(),
                            isCrewCommander: $("#ResourceCrewCommander").prop("checked") ? "Yes" : "No",
                            stationId: parseInt($("#select-resource-StationId").val()),
                            stationName: $("#select-resource-StationId option:selected").text(),
                        }
                        this.resources.push(datum)
                        DatatableService.updateDataGrid('ResourceTable', this.resources);
                        let dat = {
                            id: 0,
                            incidentId: this.IncidentId,
                            otherStationId: $("#select-resource-StationId").val(),
                            name: $("#select-resource-StationId option:selected").text(),
                        }
                        this.setStationList(TransactionType.Add, dat)

                        $("#select-resource-StationId").val("").trigger("change");
                        $("#select-resource-Category").val("").trigger("change");
                        $("#select-resource-Rank").val("").trigger("change");
                        $("#select-resource").val("").trigger("change");
                        $("#ResourceCrewCommander").prop("checked", false)

                        let AssignmentData = {
                            AssignmentType: 1,
                            Id: datum.resourceId
                        }
                        this.getEquipmentListByAssignment(AssignmentData)
                    }
                    // if response false: Error
                    else {
                        toastr.error("An Error Occurred. Please contact Admin")
                    }
                })
            } else {
                toastr.warning("Resource Already exist in table. Please choose another resource.")
            }

        } else {
            toastr.warning("Please Select an Resource")
        }
    }

    //delete From Resource Datatable
    deleteResource(meta) {
        let resource = this.resources[meta];
        let data = {
            Id: resource.id,
            IncidentId: resource.incidentId,
            ResourceId: resource.resourceId
        }
        ApiService.callCustom(ApiType.POST, variable.addDeleteResourceToTable, data, (response) => {
            // if response true: Save successfull
            if (response.data) {
                this.resources.splice(meta, 1);
                DatatableService.updateDataGrid("ResourceTable", this.resources);
            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }

    //Add External Agency To datatable
    addExternalToTable() {
        if ($("#select-external-agency").val() != '') {
            let data = {
                Id: 0,
                IncidentId: this.IncidentId,
                ExternalAgencyId: parseInt($("#select-external-agency").val()),
                Remarks: $("#ExternalRemarks").val(),
            }
            ApiService.callCustom(ApiType.POST, variable.addDeleteExternalToTable, data, (response) => {
                // if response true: Save successfull
                if (response.data) {
                    ApiService.callCustom(ApiType.POST, variable.getByIdDetailsExternalAgency, data.ExternalAgencyId, (dat) => {
                        // if response true: Save successfull
                        if (dat.data) {

                            let datum = {
                                id: response.id,
                                incidentId: this.IncidentId,
                                externalAgencyId: parseInt($("#select-external-agency").val()),
                                remarks: $("#ExternalRemarks").val(),
                                name: $("#select-external-agency option:selected").text(),
                                contactNumber: dat.data["contactNumber"],
                            }
                            this.externalAgencies.push(datum)
                            DatatableService.updateDataGrid('ExternalAgencyTable', this.externalAgencies);

                            $("#select-external-agency").val("").trigger("change");
                            $("#ExternalRemarks").val("");
                        }
                    })

                }
                // if response false: Error
                else {
                    toastr.error("An Error Occurred. Please contact Admin")
                }
            })

        } else {
            toastr.warning("Please Select an External Agency")
        }
    }

    //delete External Agency from table
    deleteExternalAgency(meta) {
        let externalAgency = this.externalAgencies[meta];
        let data = {
            Id: externalAgency.id,
            IncidentId: externalAgency.incidentId,
            ExternalAgencyId: externalAgency.externalAgencyId,
            Remarks: externalAgency.remarks
        }
        ApiService.callCustom(ApiType.POST, variable.addDeleteExternalToTable, data, (response) => {
            // if response true: Save successfull
            if (response.data) {
                this.externalAgencies.splice(meta, 1);
                DatatableService.updateDataGrid("ExternalAgencyTable", this.externalAgencies);
            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }

    //Add Casualty To Datatable
    addCasualtyToTable() {
        if ($("#select-casualty-Gender").val() != '' && $("#select-casualty-Type").val()) {
            let data = {
                Id: 0,
                IncidentId: this.IncidentId,
                Name: $("#CasualtyName").val(),
                Address: $("#CasualtyAddress").val(),
                Age: $("#CasualtyAge").val() == "" ? 0 : parseInt($("#CasualtyAge").val()),
                Remarks: $("#CasualtyRemarks").val(),
                Gender: $("#select-casualty-Gender").val(),
                CasualtyType: $("#select-casualty-Type").val()
            }
            ApiService.callCustom(ApiType.POST, variable.addDeleteCasualtyToTable, data, (response) => {
                // if response true: Save successfull
                if (response.data) {
                    let datum = {
                        id: response.id,
                        incidentId: this.IncidentId,
                        name: $("#CasualtyName").val(),
                        address: $("#CasualtyAddress").val(),
                        age: $("#CasualtyAge").val() == "" ? 0 : parseInt($("#CasualtyAge").val()),
                        remarks: $("#CasualtyRemarks").val(),
                        gender: $("#select-casualty-Gender").val(),
                        casualtyType: $("#select-casualty-Type").val(),
                        genderName: $("#select-casualty-Gender option:selected").text(),
                        casualtyTypeName: $("#select-casualty-Type option:selected").text(),
                    }
                    this.casualties.push(datum)
                    DatatableService.updateDataGrid('CasualtyTable', this.casualties);
                    $("#select-casualty-Type").val("").trigger("change");
                    $("#select-casualty-Gender").val("").trigger("change");
                    $("#CasualtyAge").val("");
                    $("#CasualtyName").val("");
                    $("#CasualtyAddress").val("");
                    $("#CasualtyRemarks").val("");
                }
                // if response false: Error
                else {
                    toastr.error("An Error Occurred. Please contact Admin")
                }
            })

        } else {
            toastr.warning("Please Select a Gender and CasualTy Type")
        }
    }

    //delete casualties from table
    deleteCasualty(meta) {
        let casualty = this.casualties[meta];
        let data = {
            Id: casualty.id,
            IncidentId: casualty.incidentId,
            Name: casualty.name,
            Address: casualty.address,
            Age: casualty.age,
            Remarks: casualty.remarks,
            Gender: casualty.gender,
            CasualtyType: casualty.casualtyType
        }
        ApiService.callCustom(ApiType.POST, variable.addDeleteCasualtyToTable, data, (response) => {
            // if response true: Save successfull
            if (response.data) {
                this.casualties.splice(meta, 1);
                DatatableService.updateDataGrid("CasualtyTable", this.casualties);
            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }

    //Add Fatality To Datatable
    addFatalityToTable() {
        if ($("#select-fatality-Gender").val() != '' && $("#select-fatality-Type").val()) {
            let data = {
                Id: 0,
                IncidentId: this.IncidentId,
                Name: $("#FatalityName").val(),
                Address: $("#FatalityAddress").val(),
                Age: $("#FatalityAge").val() == "" ? 0 : parseInt($("#FatalityAge").val()),
                Remarks: $("#FatalityRemarks").val(),
                Gender: $("#select-fatality-Gender").val(),
                FatalityType: $("#select-fatality-Type").val()
            }
            ApiService.callCustom(ApiType.POST, variable.addDeleteFatalityToTable, data, (response) => {
                // if response true: Save successfull
                if (response.data) {
                    let datum = {
                        id: response.id,
                        incidentId: this.IncidentId,
                        name: $("#FatalityName").val(),
                        address: $("#FatalityAddress").val(),
                        age: $("#FatalityAge").val() == "" ? 0 : parseInt($("#FatalityAge").val()),
                        remarks: $("#FatalityRemarks").val(),
                        gender: $("#select-fatality-Type").val(),
                        fatalityType: $("#select-fatality-Gender").val(),
                        genderName: $("#select-fatality-Type option:selected").text(),
                        fatalityTypeName: $("#select-fatality-Gender option:selected").text(),
                    }
                    this.fatalities.push(datum)
                    DatatableService.updateDataGrid('FatalityTable', this.fatalities);

                    $("#select-fatality-Type").val("").trigger("change");
                    $("#select-fatality-Gender").val("").trigger("change");
                    $("#FatalityAge").val("");
                    $("#FatalityName").val("");
                    $("#FatalityAddress").val("");
                    $("#FatalityRemarks").val("");
                }
                // if response false: Error
                else {
                    toastr.error("An Error Occurred. Please contact Admin")
                }
            })

        } else {
            toastr.warning("Please Select a Gender and Casualty Type")
        }
    }

    //delet fatality from table
    deleteFatality(meta) {
        let fatality = this.fatalities[meta];
        let data = {
            Id: fatality.id,
            IncidentId: fatality.incidentId,
            Name: fatality.name,
            Address: fatality.address,
            Age: fatality.age,
            Remarks: fatality.remarks,
            Gender: fatality.gender,
            FatalityType: fatality.fatalityType
        }
        ApiService.callCustom(ApiType.POST, variable.addDeleteFatalityToTable, data, (response) => {
            // if response true: Save successfull
            if (response.data) {
                this.fatalities.splice(meta, 1);
                DatatableService.updateDataGrid("FatalityTable", this.fatalities);
            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }

    getSalvage() {
        let EstimatedValue = $("#EstimatedValue").val() == "" ? 0.0 : parseFloat($("#EstimatedValue").val());
        let Cost = $("#DamageCost").val() == "" ? 0.0 : parseFloat($("#DamageCost").val());
        let Salvage = parseFloat(EstimatedValue - Cost).toFixed(2);
        $("#Salvage").val(Salvage)
    }

    //Add Damage To Datatable
    addDamageToTable() {
        if ($("#DamageDetails").val() != '') {
            let data = {
                Id: 0,
                IncidentId: this.IncidentId,
                Details: $("#DamageDetails").val(),
                EstimatedValue: $("#EstimatedValue").val() == "" ? 0.0 : parseFloat($("#EstimatedValue").val()).toFixed(2),
                Cost: $("#DamageCost").val() == "" ? 0.0 : parseFloat($("#DamageCost").val()).toFixed(2),
                Salvage: $("#Salvage").val()
            }
            ApiService.callCustom(ApiType.POST, variable.addDeleteDamageToTable, data, (response) => {
                // if response true: Save successfull
                if (response.data) {
                    let datum = {
                        id: response.id,
                        incidentId: this.IncidentId,
                        details: $("#DamageDetails").val(),
                        estimatedValue: $("#EstimatedValue").val() == "" ? 0.0 : parseFloat($("#EstimatedValue").val()),
                        cost: $("#DamageCost").val() == "" ? 0.0 : parseFloat($("#DamageCost").val()),
                        salvage: $("#Salvage").val()
                    }
                    this.damages.push(datum)
                    DatatableService.updateDataGrid('DamageTable', this.damages);

                    $("#DamageDetails").val("");
                    $("#EstimatedValue").val("");
                    $("#DamageCost").val("");
                    $("#Salvage").val("");
                }
                // if response false: Error
                else {
                    toastr.error("An Error Occurred. Please contact Admin")
                }
            })

        } else {
            toastr.warning("Please input the damage details")
        }
    }

    //delete damage from table 
    deleteDamage(meta) {
        let damage = this.damages[meta];
        let data = {
            Id: damage.id,
            IncidentId: damage.incidentId,
            Details: damage.details,
            EstimatedValue: damage.estimatedValue,
            Cost: damage.cost,
            Salvage: damage.Salvage,
        }
        ApiService.callCustom(ApiType.POST, variable.addDeleteDamageToTable, data, (response) => {
            // if response true: Save successfull
            if (response.data) {
                this.damages.splice(meta, 1);
                DatatableService.updateDataGrid("DamageTable", this.damages);
            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }

    //add file To Datatable
    addFileToTable() {
        const fileInput = document.querySelector('#FileFile');
        let data = {
            id: 0,
            incidentId: this.IncidentId,
            file: fileInput.files[0],
            fileName: fileInput.files[0].name
        }

        const formData = new FormData();
        formData.append("data.id", data.id);
        formData.append("data.incidentId", data.incidentId);
        formData.append("data.file", data.file);
        formData.append("data.fileName", data.fileName);
        let requestOptions = {
            method: "POST",
            redirect: 'follow',
            body: formData
        };

        try {
            fetch(variable.addFileToIncidentTable, requestOptions).then(response => response.json()).then((responseJSON) => {
                if (responseJSON.data) {
                    let datum = {
                        id: responseJSON.id,
                        fileName: fileInput.files[0].name
                    }
                    this.files.push(datum)
                    DatatableService.updateDataGrid('FileTable', this.files);
                    $("#FileFile").val("")
                } else {
                    toastr.error("An Error Occurred. Please contact Admin")
                }
            });

        } catch (error) {
            toastr.error("An Error Occurred. Please contact Admin")
        }
    }

    //delete file from table
    deleteFile(meta) {
        let file = this.files[meta];
        ApiService.callCustom(ApiType.POST, variable.deleteFileToIncidentTable, file.id, (response) => {
            // if response true: Save successfull
            if (response.data) {
                this.files.splice(meta, 1);
                DatatableService.updateDataGrid("FileTable", this.files);
            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }

    //Send Chat Message
    sendMessage() {
        let data = {
            IncidentId: this.IncidentId,
            Text: $("#chat-message").val()
        }
        ApiService.callCustom(ApiType.POST, variable.saveChatIncident, data, (response) => {
            // if response true: Save successfull
            if (response.data) {
                this.getRefreshedDataChat()
                $("#chat-message").val("")
            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }

    //download file
    downloadFile(id) {
        window.location.href = "/IncidentHandling/DownloadIncidentFile?Id=" + id;
    }

    //update Values
    updateValue(e, sectionId, DTOName, IdName = null) {
        if (!this.InitialRender) {
            let data = DefaultService.resultObject(DTOName, document.getElementById(sectionId));
            data["IncidentId"] = this.IncidentId;
            if (DTOName == "Incident") {
                data["IncidentReportRemarks"] = $("#IncidentReportRemarks").val();
            }
            if (IdName != null) {
                let val = $("#" + IdName).val();
                if (val == "") {
                    data["Id"] = 0;
                } else {
                    data["Id"] = $("#" + IdName).val();
                }
            }

            let Datum = {
                [DTOName]: data
            }
            if (this.IncidentId > 0) {
                Datum["Id"] = this.IncidentId
            }
            ApiService.callCustom(ApiType.POST, variable.saveCallHandling, Datum, (response) => {
                // if response true: Save successfull
                if (response.data) {
                    if (response.id != 0 && IdName != null) {
                        $("#" + IdName).val(response.id)
                    }
                    //toastr.success("Saved Successfully")
                }
                // if response false: Error
                else {
                    toastr.error("An Error Occurred. Please contact Admin")
                }
            })
        }
    }

    //Request To CR Checbox
    requestCheck(id) {
        let datum = {
            Id: id,
            Completed: $("#" + id).prop("checked")
        }
        ApiService.callCustom(ApiType.POST, variable.mobileRequestSaveIncident, datum, (response) => {
            // if response true: Save successfull
            if (response.data) {

            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["Address", "select-IncidentType", "select-StationId", "select-NatureOfIncidentId", "InitialIncidentType"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }
        valid = valid;

        //return boolean id valid or not
        return valid
    }

    //save action button
    save() {
        //Show loader 
        Loader.showLoader("Saving Data...");
        let data = {
            Id: this.IncidentId,
            Status: 3
        }
        //check validation and data 
        if (this.valid() && data != false) {

            //call api to save 
            ApiService.callCustom(ApiType.POST, variable.saveCallHandling, data, (response) => {
                // if response true: Save successfull
                if (response.data) {
                    toastr.success("Saved Successfully")
                    this.back();
                }
                // if response false: Error
                else {
                    toastr.error("An Error Occurred. Please contact Admin")
                }
            })
        }
        //hide loader
        Loader.hideLoader();
    }

    //assign action button
    assign() {
        //Show loader 
        Loader.showLoader("Saving Data...");
        let data = this.resources.filter(x => x["isCrewCommander"] == "Yes");
        //check validation and data 
        if (data.length > 0) {
            let datum = [];
            data.forEach((x) => {
                let dat = {
                    Id: 0,
                    IncidentId: this.IncidentId,
                    ResourceId: x.resourceId
                }
                datum.push(dat)
            });
            //call api to save 
            ApiService.callCustom(ApiType.POST, variable.assignCallHandling, datum, (response) => {
                // if response true: Save successfull
                if (response.data) {
                    toastr.success("Saved Successfully")
                    this.back();
                }
                // if response false: Error
                else {
                    toastr.error("An Error Occurred. Please contact Admin")
                }
            })
        } else {
            toastr.error("No Resource Crew Commander found.")
        }
        //hide loader
        Loader.hideLoader();
    }

    //Acknowledge
    acknowledge() {
        ApiService.callCustom(ApiType.POST, variable.saveAcknowledgeIncident, this.IncidentId, (response) => {
            // if response true: Save successfull
            if (response.data) {
                toastr.success("Acknowledgement sent successfully")
                $("#acknowledge-btn").addClass("d-none")
            }
            // if response false: Error
            else {
                toastr.error("An Error Occurred. Please contact Admin")
            }
        })
    }

    //Completed
    complete() {
        if ($("#select-IncidentSubType").val() == "" || $("#select-IncidentCode").val() == "") {
            toastr.error("Please select a Sub type and Code to complete theis incident")
        }else{
            var data = {
                IncidentId: this.IncidentId,
                Reason: "",
                Status: TransactionStatus.Completed
            }
            AlertBoxService.alertBoxWithMessageBox("Are you sure you want to complete this Incident Call?", "Reason :", "textarea", "Complete", variable.saveAbortCompletedIncident, "/IncidentHandling/List#", data)
        }
    }

    //Abort
    abort() {
        var data = {
            IncidentId: this.IncidentId,
            Reason: "",
            Status: TransactionStatus.Abort
        }
        AlertBoxService.alertBoxWithMessageBox("Are you sure you want to abort this Incident Call?", "Reason :", "textarea", "Abort", variable.saveAbortCompletedIncident, "/IncidentHandling/List#", data)
    }

    //Back button
    back() {
        //show loader when refirecting to listing
        Loader.showLoader("Redirecting To Listing...");
        location.href = "/IncidentHandling/List#"
        Loader.hideLoader();
    }

    //Enlarge / Minimize map
    enlargeMinimizeMap() {
        var buttonCustom = document.getElementById("enlarge-minimize");
        var isMinimized = buttonCustom.innerHTML.includes('Enlarge')
        if (isMinimized) {
            buttonCustom.innerHTML = "Minimize Map";
            var content = document.getElementById("map-container-callhandling");
            content.style.maxHeight = 'calc(100vh - 7vh - 8vh)';
            var map = document.getElementById("map");
            map.style.height = '70vh';
            map.style.width = '60vw';
            map.style.marginLeft = '-10vw';
            var button = document.getElementById("enlarge-map-btn");
            button.style.setProperty('margin-left', '-9vw', 'important');
            button.style.transitionDuration = "2.5s";
        } else {
            buttonCustom.innerHTML = "Enlarge Map";
            var content = document.getElementById("map-container-callhandling");
            content.style.maxHeight = 'calc(100vh - 7vh - 30vh)';
            var map = document.getElementById("map");
            map.style.height = '40vh';
            map.style.width = '100%';
            map.style.marginLeft = 'auto';
            var button = document.getElementById("enlarge-map-btn");
            button.style.setProperty('margin', '-35px 0 0 10px', 'important');
        }
    }

    minimizeMap() {
        var buttonCustom = document.getElementById("enlarge-minimize");
        if (buttonCustom != null) {
            buttonCustom.innerHTML = "Enlarge Map";
            var content = document.getElementById("zone-content");
            content.style.maxHeight = 'calc(100vh - 7vh - 30vh)';
            var map = document.getElementById("map");
            map.style.height = '40vh';
            map.style.width = '100%';
            map.style.marginLeft = 'auto';
            var button = document.getElementById("enlarge-map-btn");
            button.style.setProperty('margin', '-35px 0 0 10px', 'important');
        }
    }
}
window.CallHandling = new CallHandling();