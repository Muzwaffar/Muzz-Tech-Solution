export class Variable {

    _isLocal = true;

    //UI APIs
    _baseExternalApi = "https://localhost:57936/api/";
    _baseInternalApi = "https://localhost:57936/";

    externalURL = "https://localhost:7248/"

    _baseApi;
    _baseLiveApi;

    /*------------------------------------------------------
     * Geocoding and Reverse Geocoding
    -------------------------------------------------------*/

    LocationIQURL = "https://us1.locationiq.com/v1/";
    LocationIQAccessToken = "pk.149c65be158279fdf0bc5b6d349541f1";
    LReverseGeooding = "/reverse?key=" + this.LocationIQAccessToken;
    LForwardGeocoding = "/search?key=" + this.LocationIQAccessToken;
    LAutocomplete = "/autocomplete?key=" + this.LocationIQAccessToken;
    LRouting = "/directions/driving/";

    /*APIs VARIABLES*/

    /*------------------------------------------------------
     * Authentication - Authorisation
    -------------------------------------------------------*/

    //Account
    login;
    logout;
    forgotPassword
    changePasswordPartialView;
    changePasswordPartialViewLayout;
    changePassword;
    resetPassword;

    /*------------------------------------------------------
     * Menu
    -------------------------------------------------------*/
    getMenu;
    getListMenu;
    getByIdMenu;
    updateMenu;

    /*------------------------------------------------------
    * Notification
   -------------------------------------------------------*/
    getNotificationList;
    heartBeat;

    /*------------------------------------------------------
     * Activities
    -------------------------------------------------------*/
    //Graph
    activitiesData;
    //GetById
    getByIdActivities;
    getByIdDetailsActivities;
    getByIdConfigActivities;
    //Dropdowns
    getDropDownActivities;
    getDrpdownValuesByLabelActivities;
    //Create - Update - Listing 
    createUpdateActivities;
    getAllActivies;

    //Station Activities
    getByIdActivitiesStation;

    /*------------------------------------------------------
    * Schedule
   -------------------------------------------------------*/

    //Schedule Calendar
    getCalendar;
    getCalendarById;
    getCalendarAPI;
    getCalendarGrid;
    getCalendarFiles;
    downloadCalendarFile;

    //ScheduleList
    getScheduleListById;
    getScheduleListByIdApi;
    getServiceDropdownSL;
    getDropdownsSL;
    getDropdownByDate;
    getHelpersById;
    createUpdateSchedule;
    updateScheduleStatus;
    deleteSchedule;
    getServicesByServiceId;
    downloadScheduleReport;
    getListScheduleByRange;
    getScheduleItem;
    GetItemsByFormId;

    /*------------------------------------------------------
     * Administration
    -------------------------------------------------------*/

    //LookupType
    getByIdLKtype;
    createUpdateLKtype;
    getListLKtype;
    deleteLKtype;
    getLKtypeDropdown;


    //LookupValue
    getByIdLKvalue;
    createUpdateLKvalue;
    getListLKvalue;
    deleteLKvalue;


    //Uom Header
    getByIdUomHeader;
    createUpdateUomHeader;
    getListUomHeadere;
    deleteUomHeader;

    //Uom Details
    getByIdUomDetails;
    createUpdateUomDetails;
    getListUomDetails;
    deleteUomDetails;
    getDropDownUomDetails;

    //NAA
    getByIdNAA;
    createUpdateNAA;
    getListNAA;
    deleteNAA;
    getNAADropdown;
    getNAAByNic;
    getNAADetailsById;
    getNAAByMobileNo;;

    //Resource Type
    getListResourceType;
    getByIdResourceType;
    deleteResourceType;
    createUpdateResourceType;

    //Resource
    getByIdResource;
    createUpdateResource;
    getListResource;
    deleteResource;
    getResourceDropdown;

    //Division
    getListDivision;
    getByIdDivision;
    deleteDivision;
    getDeptDropdown;
    createUpdateDivision;

    //Department
    getByIdDepartment;
    createUpdateDepartment;
    getListDepartment;
    deleteDepartment;

    //Zone Type
    getByIdZoneType;
    createUpdateZoneType;
    getListZoneType;
    deleteZoneType;

    //Zone 
    getByIdZone;
    createUpdateZone;
    getListZone;
    deleteZone;

     //External Agency
    getByIdExternalAgency;
    createUpdateExternalAgency;
    getListExternalAgency;
    deleteExternalAgency;
    getByIdDetailsExternalAgency;

    //Incident Type
    getByIdIncidentType;
    createUpdateIncidentType;
    getListIncidentType;
    deleteIncidentType;

    //Incident Sub Type 
    getByIdIncidentSubType;
    createUpdateIncidentSubType;
    getListIncidentSubType;
    deleteIncidentSubType;

    //Station Type
    getByIdStationType;
    createUpdateStationType;
    getListStationType;
    deleteStationType;

    //Station
    getListStation;
    getByIdStation;
    createUpdateStation;
    deleteStation;
    getStationDropdown;

    //User Type
    getByIdUserType;
    createUpdateUserType;
    getListUserType;
    deleteUserType;

    //Nature of Incident
    getByIdNatureofIncident;
    createUpdateNatureofIncident;
    getListNatureofIncident;
    deleteNatureofIncident;

    //Incident Code
    getByIdIncidentCode;
    createUpdateIncidentCode;
    getListIncidentCode;
    deleteIncidentCode;
    getDropDownIncidentSubType;
    getIncidentType;

    //Asset Make
    getByIdAssetMake;
    createUpdateAssetMake;
    getListAssetMake;
    deleteAssetMake;

    //Rank
    getByIdRank;
    createUpdateRank;
    getListRank;
    deleteRank;
    getResourceCategoryDropDown;

    //Asset Model
    getByIdAssetModel;
    createUpdateAssetModel;
    getListAssetModel;
    deleteAssetModel;
    getAssetModelDropdown;

    //Shift 
    getByIdShift;
    createUpdateShift;
    getListShift;
    deleteShift;

    //Roster
    addEditRoster;
    createUpdateRoster;
    getRosterList;
    deleteRoster;
    getDropDownRoster;
    getRosterDetailsById;
    getResourceByTeamId;
    getDaySelectionByRosterId;
    getIndividualShifts;
    getShifts;
    filterDatatableByRosterPeriod;

    //Equipment
    getListEquipment;
    getByIdEquipment;
    getAllDropdownValuesEquipment;
    saveAsyncEquipment;
    updateAsyncEquipment;
    deleteEquipment;
    getAllDropdownValuesModelE;
    EquipmentGetAllDropdownValuesDependent;
    getEquipmentByAssignment;

    //Vehicle
    getByIdVehicle;
    createUpdateVehicle;
    getListVehicle;
    deleteVehicle;
    getVehicleDropdown;
    getAllDropdownValuesModelV;


    //Equipment Model
    getByIdEquipmentModel;
    createUpdateEquipmentModel;
    getListEquipmentModel;
    deleteEquipmentModel;
    getEquipmentModelDropdown;

    //Equipment Make
    getByIdEquipmentMake;
    createUpdateEquipmentMake;
    getListEquipmentMake;
    deleteEquipmentMake;

    //Caller
    getByIdCaller;
    createUpdateCaller;
    getListCaller;
    deleteCaller;
    getDropDownCaller;
    getCallerByMobile;
    getCallerById;

    //PDA
    addEditPDA;
    createUpdatePDA;
    getPDAList;
    deletePDA;
    getDropDownPDA;
    getTableDetails;

    //Priority
    getListPriority;
    getByIdPriority;
    deletePriority;
    createUpdatePriority;

    /*------------------------------------------------------
     * Form
    -------------------------------------------------------*/

    //Form
    getFormList;
    createUpdateForm;
    deleteForm;
    getByIdForm;
    getFormDropdown;
    getAllSectionForm;

    //Form Type
    getFormTypeList;
    createUpdateFormType;
    deleteFormType;
    getByIdFormType;

    //Section
    getSectionList;
    getSectionDropdown;
    getSectionParameters;
    getSectionById;
    getSectionElement;
    addEditSection;
    deleteSection;

    //WorkflowMapping
    getWorkflowMappingList;
    getWFMAllDropdown;
    getURLName;
    getWFMSaveUpdate;
    deleteWorkflowMapping;
    getWFDTable;

    /* ------- Form Config ------ */
    //FormPagePDFMapping
    getFormPagePDFMappingList;
    getFPMAllDropdown;
    getSectionByIdDropdown;
    getElementByIdDropdown;
    getTemplateTextByIdDropdown;
    getFPPDFMSaveUpdate;
    getPDFElementTable;
    deleteFormPagePDFMapping;

    //Form Module Mapping Sequence
    getFormModuleMappingSequenceList;
    getFormModuleMappingSequenceDropdown;
    getByIdFormModuleMappingSequence;
    createUpdateFormModuleMappingSequence;
    deleteFormModuleMappingSequence;

    //Element Form Configuration
    getAllDropDownValuesEFC;
    getAllDropDownValuesMenuEFC;
    getElementFormConfig;
    createUpdateElementFormConfig;

    //Form Section And Element Module Mapping
    getFormSectionAndElementModuleMappingList;
    getFPSEAllDropdown;
    getSEByIdDropdown;
    getFPSEMSaveUpdate;
    getSEElementTable;
    deleteFormSectionAndElementModuleMapping;

    /*------------------------------------------------------
    * Form Page
   -------------------------------------------------------*/

    //Form page
    getFormPageList;
    addEditFormPageView;
    getByIdApiFormPage;
    getByIdFormPage;
    createUpdateFormPage;
    deleteFormPage;
    getAllServiceListingFormPage;
    getSelectedServicesFormPage;
    getAllDropDownValuesFormPage;
    getSectionElementsByForm;
    getSourceValueByElementId;
    getCustomDetailsByElementId;

    /*------------------------------------------------------
     * Access
    -------------------------------------------------------*/

    //Access Rights
    getARList;
    addEditAR;
    createUpdateAR;
    deleteAR;
    GetARById;
    GetModules;
    GetARByRoleId;
    GetRowAccess;

    //User
    getUserList;
    addEditUser;
    deleteUser;
    createUpdateUser;
    getUserDropDown;
    getUserMatrix;

    //Matrix
    matrixView;
    getmatrix;
    savematrix;

    //OfficeZones
    getByIdOfficeZones;
    createUpdateOfficeZones;
    getListOfficeZones;
    deleteOfficeZones;
    getOfficeZonesDropdown;

    /*------------------------------------------------------
    * Common
   -------------------------------------------------------*/
    //Approval
    getCommonApprovalView;
    saveCommonApproval;
    getCommonAllDropdownApprovals;
    getCommonUserDropdownApproval;
    saveCommonApprovalStatus;
    getAssignToMe;

    //Export
    exportService;
    //WorkflowApproval
    WorkflowSave;
    getWorkflowSeq;

    
    /*------------------------------------------------------
     * File Manager
     -------------------------------------------------------*/
    uploadFiles;
    uploadSingleFile;
    getAttachementList;
    downloadFile;

    /*------------------------------------------------------
    * Store Management
    -------------------------------------------------------*/
    //Store
    getListStore;
    getAllDropdownValuesStore;
    saveAsyncStore;
    updateAsyncStore;
    deleteStore;

    //Store Item
    getListStoreItem;
    getAllDropdownValuesStoreItem;
    getAllDropDownValuesDependantStoreItem;
    getAllDropdownValuesUOMStoreItem;
    saveAsyncStoreItem;
    updateAsyncStoreItem;
    deleteStoreItem;

    //Store Transaction
    getStockRequestList;
    getStoreTransactionList;
    getStoreItemList;
    getAllDropdownValuesStoreTransaction;
    saveStoreTransaction;
    getApproveDeclineStockRequest;

    /*------------------------------------------------------
   * Call Handling
   -------------------------------------------------------*/
    getCallHandlingList;
    getCallhandlingByIdDetails;
    getCallhandlingByIdRefreshed;
    getCallHandlingDropdown;
    getCallHandlingDelete;
    getCallhandlingIncidentSubType;
    getCallhandlingIncidentCode;
    getCallerLastTransactions;
    getVehicleByCategory;
    getEquipmentByCategory;
    getResourceByRank;
    getResourceByCategory;
    initialiseCallHandling;
    saveCallHandling;
    addDeleteVehicleToTable;
    addDeleteEquipmentToTable;
    addDeleteResourceToTable;
    addDeleteExternalToTable;
    addDeleteCasualtyToTable;
    addDeleteFatalityToTable;
    addDeleteOtherStationToTable;
    addDeleteDamageToTable;
    addFileToIncidentTable;
    deleteFileToIncidentTable;
    downloadIncidentFile;
    getChatlistIncident;
    saveChatIncident;
    saveAbortCompletedIncident
    saveAcknowledgeIncident;
    getIncidentPDA;

    /*------------------------------------------------------
      * External API
     -------------------------------------------------------*/

    fleetParam;

    /*------------------------------------------------------
     * Incident
    -------------------------------------------------------*/
    incidentList;
    getIncidentByTypeName;

    constructor() {
        //set base api
        this._baseApi = (this._isLocal ? this._baseInternalApi : this._baseExternalApi);

        //initialise all api links
        this._initialiseApi();
    }

    _initialiseApi() {//set values for all apis

        /*------------------------------------------------------
         * Authentication - Authorisation
        -------------------------------------------------------*/
        //Account
        this.login = this._baseApi + "Login/Login";
        this.logout = this._baseApi + "Users/Logout"
        this.forgotPassword = this._baseApi + "Login/ForgotPassword"
        this.changePasswordPartialViewLayout = this._baseApi + "Users/ChangePasswordLT";
        this.changePasswordPartialView = this._baseApi + "Users/ChangePasswordPV";
        this.changePassword = this._baseApi + "Users/ChangePassword";
        this.resetPassword = this._baseApi + "Users/ResetPassword";

        /*------------------------------------------------------
         * Menu
        -------------------------------------------------------*/
        this.getMenu = this._baseApi + "AccessRights/GetMenu";

        this.getListMenu = this._baseApi + "Menu/GetList";
        this.getByIdMenu = this._baseApi + "Menu/GetById";
        this.updateMenu = this._baseApi + "Menu/Update";

        /*------------------------------------------------------
         * Notification
         -------------------------------------------------------*/
        this.getNotificationList = this._baseApi + "Event/GetEventList";
        this.heartBeat = this._baseApi + "Event/HeartBeat";

        /*------------------------------------------------------
        * Activities
        -------------------------------------------------------*/
        //Graph
        this.activitiesData = this._baseApi + "Activities/ActivitiesData";
        //GetById
        this.getByIdActivities = this._baseApi + "Activities/GetById";
        this.getByIdDetailsActivities = this._baseApi + "Activities/GetByIdDetails";
        this.getByIdConfigActivities = this._baseApi + "Activities/GetByIdConfig";
        //Dropdowns
        this.getDropDownActivities = this._baseApi + "Activities/GetAllDropDownValues";
        this.getDrpdownValuesByLabelActivities = this._baseApi + "Activities/GetDrpdownValuesByLabel";
        //Create - Update - Listing 
        this.createUpdateActivities = this._baseApi + "Activities/CreateUpdate";
        this.getAllActivies = this._baseApi + "Activities/FilterDatatableByPeriod";
        //station
        this.getByIdActivitiesStation = this._baseApi + "Activities/GetByIdStation";

        /*------------------------------------------------------
         * Schedule
        -------------------------------------------------------*/

        //Schedule Calendar
        this.getCalendar = this._baseApi + "Schedule/GetCalenderView";
        this.getCalendarById = this._baseApi + "Schedule/GetCalendarById";
        this.getCalendarAPI = this._baseApi + "Schedule/GetCalendarAPI";
        this.getCalendarGrid = this._baseApi + "Schedule/GetCalendarGrid";
        this.getCalendarFiles = this._baseApi + "Schedule/GetCalendarFiles";
        this.downloadCalendarFile = this._baseApi + "Schedule/DownloadFile";

        //Schedule List
        this.getScheduleListById = this._baseApi + "Schedule/AddEditView";
        this.getScheduleListByIdApi = this._baseApi + "Schedule/GetByIdApi";
        this.getServiceDropdownSL = this._baseApi + "Schedule/GetSchDropDownValues";
        this.getDropdownsSL = this._baseApi + "Schedule/GetSRlDropDownValues";
        this.getDropdownByDate = this._baseApi + "Schedule/GetDropDownByDate";
        this.getHelpersById = this._baseApi + "Schedule/GetHelpersTabeleById";
        this.createUpdateSchedule = this._baseApi + "Schedule/CreateUpdate";
        this.updateScheduleStatus = this._baseApi + "Schedule/StatusUpdate";
        this.deleteSchedule = this._baseApi + "Schedule/DeleteSchedule";
        this.getServicesByServiceId = this._baseApi + "Schedule/GetServicesByServiceId";
        this.downloadScheduleReport = this._baseApi + "Schedule/ExportExcelSchedule";
        this.getListScheduleByRange = this._baseApi + "Schedule/FilterDatatableByPeriod";
        this.getScheduleItem = this._baseApi + "Schedule/GetScheduleItem";
        this.GetItemsByFormId = this._baseApi + "Schedule/GetItemsByFormId";

        /*------------------------------------------------------
         * Administration
        -------------------------------------------------------*/

        //LookupType
        this.getByIdLKtype = this._baseApi + "LookUpType/GetById";
        this.createUpdateLKtype = this._baseApi + "LookUpType/CreateUpdate";
        this.getListLKtype = this._baseApi + "LookUpType/GetList";
        this.deleteLKtype = this._baseApi + "LookUpType/Delete";

        //LookupValue
        this.getByIdLKvalue = this._baseApi + "LookUpValue/GetById";
        this.createUpdateLKvalue = this._baseApi + "LookUpValue/CreateUpdate";
        this.getListLKvalue = this._baseApi + "LookUpValue/GetList";
        this.deleteLKvalue = this._baseApi + "LookUpValue/Delete";
        this.getDropDownLKvalue = this._baseApi + "LookUpValue/GetAllDropDownValues";

        //NAA
        this.getByIdNAA = this._baseApi + "NAA/GetById";
        this.createUpdateNAA = this._baseApi + "NAA/CreateUpdate";
        this.getListNAA = this._baseApi + "NAA/GetList";
        this.deleteNAA = this._baseApi + "NAA/Delete";
        this.getNAADropdown = this._baseApi + "NAA/GetAllDropdownValues";
        this.getNAAByNic = this._baseApi + "NAA/GetByNic";
        this.getNAADetailsById = this._baseApi + "NAA/GetDetailsById";
        this.getNAAByMobileNo = this._baseApi + "NAA/GetByMobileNo";

        //UOM Header
        this.getByIdUomHeader = this._baseApi + "UomHeader/GetById";
        this.createUpdateUomHeader = this._baseApi + "UomHeader/CreateUpdate";
        this.getListUomHeader = this._baseApi + "UomHeader/GetList";
        this.deleteUomHeader = this._baseApi + "UomHeader/Delete";

        //UOM Details
        this.getByIdUomDetails = this._baseApi + "UomDetails/GetById";
        this.createUpdateUomDetailse = this._baseApi + "UomDetails/CreateUpdate";
        this.getListUomDetailse = this._baseApi + "UomDetails/GetList";
        this.deleteUomDetails = this._baseApi + "UomDetails/Delete";
        this.getDropDownUomDetails = this._baseApi + "UomDetails/GetAllDropDownValues";

        //Resource Type
        this.getListResourceType = this._baseApi + "ResourceType/GetList";
        this.getByIdResourceType = this._baseApi + "ResourceType/GetById";
        this.deleteResourceType = this._baseApi + "ResourceType/Delete";
        this.createUpdateResourceType = this._baseApi + "ResourceType/CreateUpdate";

        //Resource
        this.getByIdResource = this._baseApi + "Resource/GetById";
        this.createUpdateResource = this._baseApi + "Resource/CreateUpdate";
        this.getListResource = this._baseApi + "Resource/GetList";
        this.deleteResource = this._baseApi + "Resource/Delete";
        this.getResourceDropdown = this._baseApi + "Resource/GetAllDropdownValues";

        //Station
        this.getListStation = this._baseApi + "Station/GetList";
        this.getByIdStation = this._baseApi + "Station/GetById";
        this.createUpdateStation = this._baseApi + "Station/CreateUpdate";
        this.deleteStation = this._baseApi + "Station/Delete";
        this.getStationDropdown = this._baseApi + "Station/GetAllDropdownValues";

        //Division
        this.getListDivision = this._baseApi + "Division/GetList";
        this.getByIdDivision = this._baseApi + "Division/GetById";
        this.deleteDivision = this._baseApi + "Division/Delete";
        this.getDeptDropdown = this._baseApi + "Division/GetAllDropdownValues";
        this.createUpdateDivision = this._baseApi + "Division/CreateUpdate";

        //Department
        this.getByIdDepartment = this._baseApi + "Department/GetById";
        this.createUpdateDepartment = this._baseApi + "Department/CreateUpdate";
        this.getListDepartment = this._baseApi + "Department/GetList";
        this.deleteDepartment = this._baseApi + "Department/Delete";

        //GlobalParam
        this.getListGlobalParam = this._baseApi + "GlobalParam/GetList";
        this.getByIdGlobalParam = this._baseApi + "GlobalParam/GetById";
        this.createUpdateGlobalParam = this._baseApi + "GlobalParam/CreateUpdate";

        //Zone Type
        this.getByIdZoneType = this._baseApi + "ZoneType/GetById";
        this.createUpdateZoneType = this._baseApi + "ZoneType/CreateUpdate";
        this.getListZoneType = this._baseApi + "ZoneType/GetList";
        this.deleteZoneType = this._baseApi + "ZoneType/Delete";

        //Zone 
        this.getByIdZone = this._baseApi + "Zone/GetById";
        this.createUpdateZone = this._baseApi + "Zone/CreateUpdate";
        this.getListZone = this._baseApi + "Zone/GetList";
        this.deleteZone = this._baseApi + "Zone/Delete";
        this.getDropDownZone = this._baseApi + "Zone/GetAllDropDownValues";

        //Shift 
        this.getByIdShift = this._baseApi + "Shift/GetById";
        this.createUpdateShift = this._baseApi + "Shift/CreateUpdate";
        this.getListShift = this._baseApi + "Shift/GetList";
        this.deleteShift = this._baseApi + "Shift/Delete";

        //External Agency
        this.getByIdExternalAgency = this._baseApi + "ExternalAgencies/GetById";
        this.createUpdateExternalAgency = this._baseApi + "ExternalAgencies/CreateUpdate";
        this.getListExternalAgency = this._baseApi + "ExternalAgencies/GetList";
        this.deleteExternalAgency = this._baseApi + "ExternalAgencies/Delete";
        this.getByIdDetailsExternalAgency = this._baseApi + "ExternalAgencies/GetByIdDetails";

        //Incident Type
        this.getByIdIncidentType = this._baseApi + "IncidentType/GetById";
        this.createUpdateIncidentType = this._baseApi + "IncidentType/CreateUpdate";
        this.getListIncidentType = this._baseApi + "IncidentType/GetList";
        this.deleteIncidentType = this._baseApi + "IncidentType/Delete";

        //Station Type
        this.getByIdStationType = this._baseApi + "StationType/GetById";
        this.createUpdateStationType = this._baseApi + "StationType/CreateUpdate";
        this.getListStationType = this._baseApi + "StationType/GetList";
        this.deleteStationType = this._baseApi + "StationType/Delete";

        //Station
        this.getListStation = this._baseApi + "Station/GetList";
        this.getByIdStation = this._baseApi + "Station/GetById";
        this.createUpdateStation = this._baseApi + "Station/CreateUpdate";
        this.deleteStation = this._baseApi + "Station/Delete";
        this.getStationDropdown = this._baseApi + "Station/GetAllDropdownValues";

        //Incident Sub Type
        this.getByIdIncidentSubType = this._baseApi + "IncidentSubType/GetById";
        this.createUpdateIncidentSubType = this._baseApi + "IncidentSubType/CreateUpdate";
        this.getListIncidentSubType = this._baseApi + "IncidentSubType/GetList";
        this.deleteIncidentSubType = this._baseApi + "IncidentSubType/Delete";
        this.getDropDownIncidentType = this._baseApi + "IncidentSubType/GetAllDropDownValues";

        //Roster
        this.addEditRoster = this._baseApi + "Roster/GetById";
        this.createUpdateRoster = this._baseApi + "Roster/CreateUpdate";
        this.getRosterList = this._baseApi + "Roster/GetList";
        this.deleteRoster = this._baseApi + "Roster/Delete";
        this.getDropDownRoster = this._baseApi + "Roster/GetAllDropDownValues";
        this.getDaySelectionByRosterId = this._baseApi + "Roster/GetDaySelectionByRosterId";
        this.getRosterDetailsById = this._baseApi + "Roster/GetDetailsById";
        this.getResourceByTeamId = this._baseApi + "Roster/GetResourceByTeamId";
        this.getShifts = this._baseApi + "Roster/GetShifts";
        this.getIndividualShifts = this._baseApi + "Roster/GetIndividualShifts";
        this.filterDatatableByRosterPeriod = this._baseApi + "Roster/FilterDatatableByRosterPeriod";
        //User Type
        this.getByIdUserType = this._baseApi + "UserType/GetById";
        this.createUpdateUserType = this._baseApi + "UserType/CreateUpdate";
        this.getListUserType = this._baseApi + "UserType/GetList";
        this.deleteUserType = this._baseApi + "UserType/Delete";

        //Nature of Incident
        this.getByIdNatureofIncident = this._baseApi + "NatureofIncident/GetById";
        this.createUpdateNatureofIncident = this._baseApi + "NatureofIncident/CreateUpdate";
        this.getListNatureofIncident = this._baseApi + "NatureofIncident/GetList";
        this.deleteNatureofIncident = this._baseApi + "NatureofIncident/Delete";

        //Asset Make
        this.getByIdAssetMake = this._baseApi + "AssetMake/GetById";
        this.createUpdateAssetMake = this._baseApi + "AssetMake/CreateUpdate";
        this.getListAssetMake = this._baseApi + "AssetMake/GetList";
        this.deleteAssetMake = this._baseApi + "AssetMake/Delete";

        //Incident Code
        this.getByIdIncidentCode = this._baseApi + "IncidentCode/GetById";
        this.createUpdateIncidentCode = this._baseApi + "IncidentCode/CreateUpdate";
        this.getListIncidentCode = this._baseApi + "IncidentCode/GetList";
        this.deleteIncidentCode = this._baseApi + "IncidentCode/Delete";
        this.getDropDownIncidentSubType = this._baseApi + "IncidentCode/GetAllDropDownValues";
        this.getIncidentType = this._baseApi + "IncidentCode/GetIncidentType";

        //Rank
        this.getByIdRank = this._baseApi + "Rank/GetById";
        this.createUpdateRank = this._baseApi + "Rank/CreateUpdate";
        this.getListRank = this._baseApi + "Rank/GetList";
        this.deleteRank = this._baseApi + "Rank/Delete";
        this.getResourceCategoryDropDown = this._baseApi + "Rank/GetAllDropDownValues";

        //Asset Model
        this.getByIdAssetModel = this._baseApi + "AssetModel/GetById";
        this.createUpdateAssetModel = this._baseApi + "AssetModel/CreateUpdate";
        this.getListAssetModel = this._baseApi + "AssetModel/GetList";
        this.deleteAssetModel = this._baseApi + "AssetModel/Delete";
        this.getAssetModelDropdown = this._baseApi + "AssetModel/GetAllDropDownValues";

        //Equipment
        this.getListEquipment = this._baseApi + "Equipment/GetList";
        this.getByIdEquipment = this._baseApi + "Equipment/GetById";
        this.getAllDropdownValuesEquipment = this._baseApi + "Equipment/GetAllDropdownValues";
        this.saveAsyncEquipment = this._baseApi + "Equipment/SaveUpdateEquipment";
        this.deleteEquipment = this._baseApi + "Equipment/Delete";
        this.getAllDropdownValuesModelE = this._baseApi + "Equipment/GetAllDropdownValuesModel";
        this.EquipmentGetAllDropdownValuesDependent = this._baseApi + "Equipment/GetAllDropdownValuesDependent";
        this.getEquipmentByAssignment = this._baseApi + "Equipment/GetEquipmentByAssignment";

        //Vehicle
        this.getByIdVehicle = this._baseApi + "Vehicle/GetById";
        this.createUpdateVehicle = this._baseApi + "Vehicle/CreateUpdate";
        this.getListVehicle = this._baseApi + "Vehicle/GetList";
        this.deleteVehicle = this._baseApi + "Vehicle/Delete";
        this.getVehicleDropdown = this._baseApi + "Vehicle/GetAllDropDownValues";
        this.getAllDropdownValuesModelV = this._baseApi + "Vehicle/GetAllDropdownValuesModel";

     
        //PDA
        this.addEditPDA = this._baseApi + "PDA/GetById";
        this.createUpdatePDA = this._baseApi + "PDA/CreateUpdate";
        this.getPDAList = this._baseApi + "PDA/GetList";
        this.deletePDA = this._baseApi + "PDA/Delete";
        this.getDropDownPDA = this._baseApi + "PDA/GetAllDropDownValues";
        this.getTableDetails = this._baseApi + "PDA/getTableDetails";

        //Caller
        this.getByIdCaller = this._baseApi + "Caller/GetById";
        this.createUpdateCaller = this._baseApi + "Caller/CreateUpdate";
        this.getListCaller = this._baseApi + "Caller/GetList";
        this.deleteCaller = this._baseApi + "Caller/Delete";
        this.getDropDownCaller = this._baseApi + "Caller/GetAllDropDownValues";
        this.getCallerByMobile = this._baseApi + "Caller/GetCallerByMobile";
        this.getCallerById = this._baseApi + "Caller/GetCallerById";

        //Equipment Model
        this.getByIdEquipmentModel = this._baseApi + "EquipmentModel/GetById";
        this.createUpdateEquipmentModel = this._baseApi + "EquipmentModel/CreateUpdate";
        this.getListEquipmentModel = this._baseApi + "EquipmentModel/GetList";
        this.deleteEquipmentModel = this._baseApi + "EquipmentModel/Delete";
        this.getEquipmentModelDropdown = this._baseApi + "EquipmentModel/GetAllDropDownValues";

        //Equipment Make
        this.getByIdEquipmentMake = this._baseApi + "EquipmentMake/GetById";
        this.createUpdateEquipmentMake = this._baseApi + "EquipmentMake/CreateUpdate";
        this.getListEquipmentMake = this._baseApi + "EquipmentMake/GetList";
        this.deleteEquipmentMake = this._baseApi + "EquipmentMake/Delete";

        //Priority
        this.getListPriority = this._baseApi + "Priority/GetList";
        this.getByIdPriority = this._baseApi + "Priority/GetById";
        this.deletePriority = this._baseApi + "Priority/Delete";
        this.createUpdatePriority = this._baseApi + "Priority/CreateUpdate";

        /*------------------------------------------------------
        * Form
        -------------------------------------------------------*/
        //Form
        this.getFormList = this._baseApi + "Form/GetList";
        this.getByIdForm = this._baseApi + "Form/GetById";
        this.createUpdateForm = this._baseApi + "Form/CreateUpdate";
        this.deleteForm = this._baseApi + "Form/Delete";
        this.getFormDropdown = this._baseApi + "Form/GetAllDropdownValues";
        this.getAllSectionForm = this._baseApi + "Form/GetAllSection";

        //Form Type
        this.getFormTypeList = this._baseApi + "FormType/GetList";
        this.getByIdFormType = this._baseApi + "FormType/GetById";
        this.createUpdateFormType = this._baseApi + "FormType/CreateUpdate";
        this.deleteFormType = this._baseApi + "FormType/Delete";

        //Section
        this.getSectionList = this._baseApi + "Section/GetList";
        this.getSectionDropdown = this._baseApi + "Section/GetAllDropDownValues";
        this.getSectionParameters = this._baseApi + "Section/GetAllParameters";
        this.getSectionById = this._baseApi + "Section/GetById";
        this.getSectionElement = this._baseApi + "Section/GetElementById";
        this.addEditSection = this._baseApi + "Section/CreateUpdate";
        this.deleteSection = this._baseApi + "Section/Delete";

        //WorkflowMapping
        this.getWorkflowMappingList = this._baseApi + "WorkflowMapping/GetList";
        this.getWFMAllDropdown = this._baseApi + "WorkflowMapping/GetAllDropDownValues";
        this.getURLName = this._baseApi + "WorkflowMapping/GetURLName";
        this.getWFMSaveUpdate = this._baseApi + "WorkflowMapping/CreateUpdate";
        this.deleteWorkflowMapping = this._baseApi + "WorkflowMapping/Delete";
        this.getWFDTable = this._baseApi + "WorkflowMapping/GetWFDTable";

        /* ------- Form Config ------ */
        //FormPagePDFMapping
        this.getFormPagePDFMappingList = this._baseApi + "FormPagePDFMapping/GetList";
        this.getFPMAllDropdown = this._baseApi + "FormPagePDFMapping/GetAllDropdownValues";
        this.getSectionByIdDropdown = this._baseApi + "FormPagePDFMapping/GetSectionById";
        this.getElementByIdDropdown = this._baseApi + "FormPagePDFMapping/GetElementById";
        this.getTemplateTextByIdDropdown = this._baseApi + "FormPagePDFMapping/GetTemplateTextById";
        this.getFPPDFMSaveUpdate = this._baseApi + "FormPagePDFMapping/CreateUpdate";
        this.getPDFElementTable = this._baseApi + "FormPagePDFMapping/GetPDFElementTable";
        this.deleteFormPagePDFMapping = this._baseApi + "FormPagePDFMapping/Delete";

        //Form Module Mapping Sequence
        this.getFormModuleMappingSequenceList = this._baseApi + "FormModuleMappingSequence/GetList";
        this.getFormModuleMappingSequenceDropdown = this._baseApi + "FormModuleMappingSequence/GetAllDropdownValues";
        this.getByIdFormModuleMappingSequence = this._baseApi + "FormModuleMappingSequence/GetById";
        this.createUpdateFormModuleMappingSequence = this._baseApi + "FormModuleMappingSequence/CreateUpdate";
        this.deleteFormModuleMappingSequence = this._baseApi + "FormModuleMappingSequence/Delete";

        //Element Form Configuration
        this.getAllDropDownValuesEFC = this._baseApi + "ElementFormConfig/GetAllDropdownValues";
        this.getAllDropDownValuesMenuEFC = this._baseApi + "ElementFormConfig/GetAllDropdownValuesMenu";
        this.getElementFormConfig = this._baseApi + "ElementFormConfig/GetElementFormConfig";
        this.createUpdateElementFormConfig = this._baseApi + "ElementFormConfig/CreateUpdate";

        //Form Section And Element Module Mapping
        this.getFormSectionAndElementModuleMappingList = this._baseApi + "FormSectionAndElementModuleMapping/GetList";
        this.getFPSEAllDropdown = this._baseApi + "FormSectionAndElementModuleMapping/GetAllDropdownValues";
        this.getSEByIdDropdown = this._baseApi + "FormSectionAndElementModuleMapping/GetElementById";
        this.getFPSEMSaveUpdate = this._baseApi + "FormSectionAndElementModuleMapping/CreateUpdate";
        this.getSEElementTable = this._baseApi + "FormSectionAndElementModuleMapping/GetSEElementTable";
        this.deleteFormSectionAndElementModuleMapping = this._baseApi + "FormSectionAndElementModuleMapping/Delete";

        /*------------------------------------------------------
        * Form Page
        -------------------------------------------------------*/

        //Form page
        this.getFormPageList = this._baseApi + "FormPage/GetList";
        this.getByIdApiFormPage = this._baseApi + "FormPage/GetByIdApi";
        this.getByIdFormPage = this._baseApi + "FormPage/GetById";
        this.createUpdateFormPage = this._baseApi + "FormPage/CreateUpdate";
        this.deleteFormPage = this._baseApi + "FormPage/Delete";
        this.getAllServiceListingFormPage = this._baseApi + "FormPage/GetAllServiceListing";
        this.getSelectedServicesFormPage = this._baseApi + "FormPage/GetSelectedServices";
        this.getAllDropDownValuesFormPage = this._baseApi + "FormPage/GetAllDropDownValues";
        this.getSectionElementsByForm = this._baseApi + "FormPage/GetSectionElementsByForm";
        this.getSourceValueByElementId = this._baseApi + "FormPage/GetSourceValueByElementId";
        this.getCustomDetailsByElementId = this._baseApi + "FormPage/GetCustomDetailsByElementId";

        /*------------------------------------------------------
         * Access
        -------------------------------------------------------*/
        //User
        this.getUserList = this._baseApi + "Users/GetUserList";
        this.addEditUser = this._baseApi + "Users/GetUserPartialView";
        this.createUpdateUser = this._baseApi + "Users/CreateUpdate";
        this.deleteUser = this._baseApi + "Users/Delete";
        this.getUserDropDown = this._baseApi + "Users/getUserDropDown";
        this.getUserMatrix = this._baseApi + "Users/GetTree";


        //Access Rights
        this.getARList = this._baseApi + "AccessRights/GetList";
        this.GetARById = this._baseApi + "AccessRights/GetById";
        this.GetARByRoleId = this._baseApi + "AccessRights/GetARByRoleId";
        this.GetModules = this._baseApi + "AccessRights/GetModules";
        this.createUpdateAR = this._baseApi + "AccessRights/createUpdateAR";
        this.deleteAR = this._baseApi + "AccessRights/Delete";
        this.GetRowAccess = this._baseApi + "AccessRights/GetRowAccess";

        //Matrix
        this.matrixView = this._baseApi + "Matrix/Matrix";
        this.getmatrix = this._baseApi + "Matrix/GetTree";
        this.savematrix = this._baseApi + "Matrix/CRUDMatrix";

        //OfficeZones
        this.getByIdOfficeZones = this._baseApi + "OfficeZones/GetById";
        this.createUpdateOfficeZones = this._baseApi + "OfficeZones/CreateUpdate";
        this.getListOfficeZones = this._baseApi + "OfficeZones/GetList";
        this.deleteOfficeZones = this._baseApi + "OfficeZones/Delete";
        this.getOfficeZonesDropdown = this._baseApi + "OfficeZones/GetAllDropdownValues";

        /*------------------------------------------------------
         * Common
        -------------------------------------------------------*/
        //Approval
        this.getCommonApprovalView = this._baseApi + "Approval/Approval";
        this.saveCommonApproval = this._baseApi + "Approval/SendForApproval";
        this.getCommonAllDropdownApproval = this._baseApi + "Approval/GetDepartmentDropdownValues";
        this.getCommonUserDropdownApproval = this._baseApi + "Approval/GetUsersDropdownValues";
        this.saveCommonApprovalStatus = this._baseApi + "Approval/ApproveDecline";
        this.getAssignToMe = this._baseApi + "Approval/GetAssignToMe";

        //Export
        this.exportService = this._baseApi + "Export/ExportISO";

        //WorkflowApproval
        this.WorkflowSave = this._baseApi + "Workflow/WorkflowSave";
        this.getWorkflowSeq = this._baseApi + "Workflow/GetWorkflowSeq";

        /* ------- Common ------ */
        //Approval
        this.getRLApprovalView = this._baseApi + "ResearchLifecyleApproval/ResearchLifecyleApproval";
        this.saveRLApproval = this._baseApi + "ResearchLifecyleApproval/SendForApproval";
        this.getAllDropdownApproval = this._baseApi + "ResearchLifecyleApproval/GetDepartmentDropdownValues";
        this.getUserDropdownApproval = this._baseApi + "ResearchLifecyleApproval/GetUsersDropdownValues";
        this.saveApprovalStatus = this._baseApi + "ResearchLifecyleApproval/ResearchLifecyleApproveDecline";
        this.getRework = this._baseApi + "ResearchLifecyleApproval/Rework";

        /*------------------------------------------------------
        * File Manager
        -------------------------------------------------------*/
        this.uploadFiles = this._baseApi + "FileUpload/UploadFiles";
        this.uploadSingleFile = this._baseApi + "FileUpload/UploadSingleFile";
        this.getAttachementList = this._baseApi + "FileUpload/GetAttachementList";
        this.downloadFile = this._baseApi + "FileUpload/DownloadFile";

        /*------------------------------------------------------
        * Store Management
        -------------------------------------------------------*/
        //Store
        this.getListStore = this._baseApi + "Store/GetList";
        this.getAllDropdownValuesStore = this._baseApi + "Store/GetAllDropdownValues";
        this.saveAsyncStore = this._baseApi + "Store/SaveUpdateStore";
        this.deleteStore = this._baseApi + "Store/Delete";

        //Store Item
        this.getListStoreItem = this._baseApi + "StoreItem/GetList";
        this.getAllDropdownValuesStoreItem = this._baseApi + "StoreItem/GetAllDropdownValues";
        this.getAllDropdownValuesUOMStoreItem = this._baseApi + "StoreItem/GetAllDropdownValuesUOM";
        this.getAllDropDownValuesDependantStoreItem = this._baseApi + "StoreItem/GetAllDropDownValuesDependant";
        this.saveAsyncStoreItem = this._baseApi + "StoreItem/SaveUpdateStoreItem";
        this.deleteStoreItem = this._baseApi + "StoreItem/Delete";

        //Store Transaction
        this.getStockRequestList = this._baseApi + "StoreTransaction/GetStockRequestList";
        this.getStoreTransactionList = this._baseApi + "StoreTransaction/GetStoreTransactionList";
        this.getStoreItemList = this._baseApi + "StoreTransaction/GetStoreItemList";
        this.getAllDropdownValuesStoreTransaction = this._baseApi + "StoreTransaction/GetAllDropdownValues";
        this.saveStoreTransaction = this._baseApi + "StoreTransaction/Save";
        this.getApproveDeclineStockRequest = this._baseApi + "StoreTransaction/AproveDecline";

        /*------------------------------------------------------
       * Call Handling
       -------------------------------------------------------*/
        this.getCallHandlingList = this._baseApi + "IncidentHandling/GetList";
        this.getCallhandlingByIdDetails = this._baseApi + "IncidentHandling/GetByIdDetails";
        this.getCallhandlingByIdRefreshed = this._baseApi + "IncidentHandling/GetByIdRefreshed";
        this.getCallHandlingDropdown = this._baseApi + "IncidentHandling/GetAllDropdownValues";
        this.getCallHandlingDelete = this._baseApi + "IncidentHandling/Delete";
        this.getCallhandlingIncidentSubType = this._baseApi + "IncidentHandling/GetCallhandlingIncidentSubType";
        this.getCallhandlingIncidentCode = this._baseApi + "IncidentHandling/GetCallhandlingIncidentCode";
        this.getCallerLastTransactions = this._baseApi + "IncidentHandling/GetCallerLastTransactions";
        this.getVehicleByCategory = this._baseApi + "IncidentHandling/GetVehicleByCategory";
        this.getEquipmentByCategory = this._baseApi + "IncidentHandling/GetEquipmentByCategory";
        this.getResourceByRank = this._baseApi + "IncidentHandling/GetResourceByRank";
        this.getResourceByCategory = this._baseApi + "IncidentHandling/GetResourceByCategory";
        this.initialiseCallHandling = this._baseApi + "IncidentHandling/Initialise";
        this.saveCallHandling = this._baseApi + "IncidentHandling/Save";
        this.assignCallHandling = this._baseApi + "IncidentHandling/AssigneResourcecrewCommander";
        this.addDeleteVehicleToTable = this._baseApi + "IncidentHandling/AddDeleteVehicleToTable";
        this.addDeleteEquipmentToTable = this._baseApi + "IncidentHandling/AddDeleteEquipmentToTable";
        this.addDeleteResourceToTable = this._baseApi + "IncidentHandling/AddDeleteResourceToTable";
        this.addDeleteExternalToTable = this._baseApi + "IncidentHandling/AddDeleteExternalToTable";
        this.addDeleteCasualtyToTable = this._baseApi + "IncidentHandling/AddDeleteCasualtyToTable";
        this.addDeleteFatalityToTable = this._baseApi + "IncidentHandling/AddDeleteFatalityToTable";
        this.addDeleteOtherStationToTable = this._baseApi + "IncidentHandling/AddDeleteOtherStationToTable";
        this.addDeleteDamageToTable = this._baseApi + "IncidentHandling/AddDeleteDamageToTable";
        this.addFileToIncidentTable = this._baseApi + "IncidentHandling/UploadFileIncident";
        this.deleteFileToIncidentTable = this._baseApi + "IncidentHandling/DeleteFileIncident";
        this.downloadIncidentFile = this._baseApi + "IncidentHandling/DownloadIncidentFile";
        this.getChatlistIncident = this._baseApi + "IncidentHandling/Chatlist";
        this.saveChatIncident = this._baseApi + "IncidentHandling/Savechat";
        this.saveAbortCompletedIncident = this._baseApi + "IncidentHandling/AbortCompleted";
        this.saveAcknowledgeIncident = this._baseApi + "IncidentHandling/Acknowledge";
        this.mobileRequestIncident = this._baseApi + "IncidentHandling/CRRequestList";
        this.mobileRequestSaveIncident = this._baseApi + "IncidentHandling/UpdateCRRequestDTO";
        this.getIncidentPDA = this._baseApi + "IncidentHandling/GetIncidentPDA";

        /*------------------------------------------------------
          * External APIs
         -------------------------------------------------------*/
        this.fleetParam = this._baseApi + "External/GetParams";

        /*------------------------------------------------------
         * Incident
        -------------------------------------------------------*/
        //Incident
        this.incidentList = this._baseApi + "Incident/GetList";
        this.getIncidentByTypeName = this._baseApi + "Incident/GetAllByTypeName";
    }
}

window.variable = new Variable();