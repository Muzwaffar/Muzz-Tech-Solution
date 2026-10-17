import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DateRangeService } from "/Scripts/Common/DateRangeService.js";
import { ScreenMode, AccessOperationType, LocalStorageValues, ApprovalStatus } from "../Common/Enums.js";
import { Variable } from "/Scripts/Common/variable.js";

export class DatatableService {//handles datatables
    /*
    =======================
      Common Datatable 
    =======================
    */
    //Data table for  main listing
    static initializeDataGridPopup(tableID, partialViewId, onclickPartial, url, columnList, className, showDelete = true, showAdd = true) {
        var dataToShow = null; //init data variable
        var access = null; // init access variable
        var addBtn = '';
        ApiService.callCustom(ApiType.POST, url, "", (data) => { //Calls api to get data
            //set access levels
            if (data.access != undefined || data.access != null) {
                access = data.access
            }
            if (!showAdd) {
                addBtn = 'd-none';
            }
            dataToShow = data.data; //set data from api in data variable
            if ($('#' + tableID).length > 0) { //check if table exists
                // $('#' + tableID).DataTable().destroy();
                $('#' + tableID).DataTable({ //init table
                    dom: "Bfrtip", //button Strip
                    searching: true, //show search
                    paging: false, //no paging
                    processing: true, //processing
                    scrollY: 'auto', //fixed height
                    scrollCollapse: true,//scroll
                    order: [[0, 'desc']], //order
                    language: { // search anf info bars
                        search: '',
                        emptyTable: 'No data available',
                        searchPlaceholder: 'Search',
                        info: "Showing _TOTAL_ entries",
                        infoEmpty: "Showing 0 entries",
                        infoFiltered: "(filtered from _MAX_ entries)",
                    },
                    data: dataToShow, // data to display
                    columnDefs: [ // set action column
                        {
                            //set column order for action
                            targets: columnList.length,
                            //remove sorting on action
                            orderable: false,
                            //set width of action column
                            width: "10%",
                            render: function (data, type, full, meta) {
                                //view button
                                var view = '<a href="#' + partialViewId + '" " onclick="' + className + '.openPopupAddEdit(\'' + full.id + '\', false)" title="View"><i class="fa fa-eye text-secondary ms-1"></i></a> |';
                                //edit button
                                var edit = '<a href="#' + partialViewId + '" onclick="' + className + '.openPopupAddEdit(\'' + full.id + '\', true)" title="Edit"><i class="fa fa-edit text-secondary ms-1"></i></a> ';
                                //delete button
                                var del = ' | <a onclick="' + className + '.delete(\'' + full.id + '\')" title="Delete"><i class="fa fa-xmark text-danger"></i></a>';
                                //reset user 
                                var reset = '| <a onclick="' + className + '.reset(\'' + full.id + '\')"  title="Reset Password To Default"><i class="fa fa-arrow-left-rotate"></i></a> ';
                                //list of buttons
                                var btn = '';

                                if (access.length > 0) {
                                    if (access.find(x => x.accessType == AccessOperationType.Read) != null) {
                                        if (access.find(x => x.accessType == AccessOperationType.Read).hasAccess) {
                                            btn += view
                                        }
                                    }
                                    if (access.find(x => x.accessType == AccessOperationType.Update) != null) {
                                        if (access.find(x => x.accessType == AccessOperationType.Update).hasAccess) {
                                            btn += edit
                                        }
                                    }
                                    if (access.find(x => x.accessType == AccessOperationType.Delete) != null) {
                                        if (access.find(x => x.accessType == AccessOperationType.Delete).hasAccess) {
                                            btn += del
                                        }
                                    }

                                } else {
                                    btn += view + edit;
                                    if (showDelete) {
                                        btn += del
                                    }
                                    //TO REMOVE!! NO HARDCODING!! Please pass custom btn as param!!
                                    if (className == "User") {
                                        btn =
                                            '<div class="text-nowrap">' + view + edit + reset + del + '</div>';
                                    }
                                    if (className == "GlobalParam") {
                                        btn =
                                            '<div class="text-nowrap">' + view + edit + '</div>';
                                    }
                                }
                                data = '<div class="text-nowrap">' + btn + '</div>';
                                return data;
                            }
                        }
                    ],
                    columns: columnList, //column list

                    buttons: [ //set buttons above table
                        //Excel Export Button
                        {
                            extend: 'excel',
                            text: '<i class="fa fa-file-excel me-2"></i>Export Excel',
                            className: 'form-control btn-c-secondary d-none',
                            exportOptions: {
                                columns: ':visible:not(.sorting_disabled)'

                            }
                        },
                        //Open Add popup

                        '<a href="#' + partialViewId + '"><button class="form-control btn-c-primary d-inline ' + addBtn + '" onclick="' + onclickPartial + '"><i class="fa fa-plus me-2"></i>Add New</button></a>',
                    ],
                    drawCallback: function () {
                        // BUTTON EVENT ON THE DATA TABLE
                    }
                });
            }
        })
    }

    //Data table for staticListing
    static initializeDataGridListingStatic(tableID, addNewLink, url, columnList, className, hasDateRange = false, re = false, showDelete = false, showAdd = true, customSeqButton = " d-none", customButton = null, stationButton = "d-none", isCustomApiToGetId = "") {
        var dataToShow = null; //init data variable
        var access = []; // init access variable
        var createBtnShow = "";
        var addButtonClass = "";
        var showAddClass = "";
        if (showAdd == false) {
            showAddClass = " d-none"
        }
        var parameter = sessionStorage.getItem(LocalStorageValues.MODULEID);
        let MID = '&MId=' + parameter;
        if (hasDateRange == false) {
            addButtonClass = "ms-auto";
        }
        if (!addNewLink.includes("?")) {
            MID = "";
        }
        addNewLink = addNewLink + MID;
       
        ApiService.callCustom(ApiType.POST, new Variable().getWorkflowSeq, parameter, (dt) => { //Calls api to get WF Sequence
            if (dt.data) {
                customSeqButton = "";
            }

            ApiService.callCustom(ApiType.POST, new Variable().GetRowAccess, parameter, (AccessDt) => { //Calls api to get data
                //set access levels
                if (AccessDt.access != undefined || AccessDt.access != null) {
                    access = AccessDt.access;
                    if (access.length > 0) {

                        if (access.find(x => x.accessType == AccessOperationType.Create) != null) {
                            if (!access.find(x => x.accessType == AccessOperationType.Create).hasAccess) {
                                createBtnShow = "d-none";
                            }
                        }
                    }

                }
                ApiService.callCustom(ApiType.POST, url, parameter, (data) => { //Calls api to get data
                    dataToShow = data.data; //set data from api in data variable
                    if ($('#' + tableID).length > 0) { //check if table exists
                        this.destroyDataGrid(tableID)
                        if (isCustomApiToGetId != "") {
                            $('#' + tableID).DataTable({ //init table
                                dom: "Bfr<'customActionsdt'>tip", //button Strip
                                searching: true, //show search
                                paging: false, //no paging
                                processing: true, //processing
                                scrollY: 'auto', //fixed height
                                scrollX: 'auto',
                                order: [[0, 'desc']], //order
                                scrollCollapse: true,//scroll
                                language: { // search anf info bars
                                    search: '',
                                    emptyTable: 'No data available',
                                    searchPlaceholder: 'Search',
                                    info: "Showing _TOTAL_ entries",
                                    infoEmpty: "Showing 0 entries",
                                    infoFiltered: "(filtered from _MAX_ entries)",
                                },
                                data: dataToShow, // data to display
                                columnDefs: [ // set action column
                                    {
                                        //set column order for action
                                        targets: columnList.length,
                                        //remove sorting on action
                                        orderable: false,
                                        //set width of action column
                                        width: "50px",
                                        render: function (data, type, full, meta) {
                                            var param = "?";
                                            if (addNewLink.includes("?")) {
                                                param = "&";
                                            }

                                            let customClassInFN = "";
                                            if (tableID == "StationLivestockTableList") {
                                                if (full.deathDate != null) {
                                                    customClassInFN = "disabled"
                                                }
                                            }
                                            //view button
                                            var view = '<a href="' + addNewLink + param + 'Id=' + full.id + '&View=1" title="View"><i class="fa fa-eye text-secondary ms-1"></i></a>';
                                            var edit = '';
                                            if (tableID == "INCtable") {
                                                edit = ' | <a href="' + addNewLink + param + 'Id=' + full.id + ' &UserToken=' + localStorage.getItem(LocalStorageValues.USERTOKEN) + '" class="' + customClassInFN + '" title="Edit"><i class="fa fa-edit text-secondary ms-1"></i></a> ';
                                            } else {
                                                 edit = ' | <a href="' + addNewLink + param + 'Id=' + full.id + ' &MId=' + sessionStorage.getItem(LocalStorageValues.MODULEID) + '" class="' + customClassInFN + '" title="Edit"><i class="fa fa-edit text-secondary ms-1"></i></a> ';
                                            }
                                            //edit button
                                            
                                            //delete button
                                            var del = ' | <a onclick="' + className + '.delete(\'' + full.id + '\')" class="' + customClassInFN + '" title="Delete"><i class="fa fa-xmark text-danger ms-1"></i></a>';
                                            //Export ISO PDF button
                                            //var exp = ' | <a onclick="' + className + '.export(\'' + full.id + '\')" title="Export PDF"><i class="fa fa-file-export ms-1"></i></a>';
                                            var exp = '';
                                            //refresh button appears only when status is declined
                                            var rework = ' |<a onclick="' + className + '.rework(\'' + full.id + '\')" title="Rework"><i class="fa fa-rotate-right text-danger ms-1"></i></a>';

                                            //list of buttons
                                            var btn = '';


                                            //check access
                                            if (access.length > 0) {
                                                if (access.find(x => x.accessType == AccessOperationType.Read) != null) {
                                                    if (access.find(x => x.accessType == AccessOperationType.Read).hasAccess) {
                                                        btn += view
                                                    }
                                                }
                                                if (access.find(x => x.accessType == AccessOperationType.Update) != null) {
                                                    if (access.find(x => x.accessType == AccessOperationType.Update).hasAccess) {
                                                        btn += edit
                                                    }
                                                }
                                                if (access.find(x => x.accessType == AccessOperationType.Delete) != null) {
                                                    if (access.find(x => x.accessType == AccessOperationType.Delete).hasAccess) {
                                                        if (full.approvalStatus == "Draft") {
                                                            btn += del
                                                        } else {
                                                            if (showDelete) {
                                                                btn += del
                                                            }
                                                        }
                                                    }
                                                }
                                                if (access.find(x => x.accessType == AccessOperationType.Export) != null) {
                                                    if (access.find(x => x.accessType == AccessOperationType.Export).hasAccess) {
                                                        btn += exp
                                                    }
                                                }


                                            }

                                            if (re == true && full.approvalStatus == "Declined") {

                                                btn += rework
                                            }
                                           
                                            if (customButton != null) {
                                                var customButtonReplacement = customButton.replace('[ID]', full.id)
                                                btn += customButtonReplacement
                                            }
                                            data = '<div class="text-nowrap">' + btn + '</div>';
                                            return data;
                                        }
                                    }
                                ],
                                columns: columnList, //column list 
                                buttons: [ //set buttons above table
                                    //Excel Export Button
                                    {
                                        extend: 'excel',
                                        text: '<i class="fa fa-file-excel me-2"></i>Export Excel',
                                        className: 'form-control btn-c-secondary d-none',
                                        exportOptions: {
                                            columns: ':visible:not(.sorting_disabled)'
                                        }
                                    },
                                    //Open Add popup
                                    '<div class="d-flex ' + addButtonClass + '"><button id="list-action-button" class="form-control w-auto me-3 btn-c-secondary ' + stationButton + '" onclick="' + className + '.sendToStation()" disabled>Send To Station</button>' +
                                    '<a onclick="' + isCustomApiToGetId + '"><button class="form-control btn-c-primary d-inline ' + showAddClass + createBtnShow + customSeqButton + '"><i class="fa fa-plus me-2"></i>Add New</button></a></div>',
                                ],
                                drawCallback: function () {
                                    // BUTTON EVENT ON THE DATA TABLE
                                }
                            });
                        }else{
                            $('#' + tableID).DataTable({ //init table
                                dom: "Bfr<'customActionsdt'>tip", //button Strip
                                searching: true, //show search
                                paging: false, //no paging
                                processing: true, //processing
                                scrollY: 'auto', //fixed height
                                scrollX: 'auto',
                                order: [[0, 'desc']], //order
                                scrollCollapse: true,//scroll
                                language: { // search anf info bars
                                    search: '',
                                    emptyTable: 'No data available',
                                    searchPlaceholder: 'Search',
                                    info: "Showing _TOTAL_ entries",
                                    infoEmpty: "Showing 0 entries",
                                    infoFiltered: "(filtered from _MAX_ entries)",
                                },
                                data: dataToShow, // data to display
                                columnDefs: [ // set action column
                                    {
                                        //set column order for action
                                        targets: columnList.length,
                                        //remove sorting on action
                                        orderable: false,
                                        //set width of action column
                                        width: "50px",
                                        render: function (data, type, full, meta) {
                                            var param = "?";
                                            if (addNewLink.includes("?")) {
                                                param = "&";
                                            }

                                            let customClassInFN = "";
                                            if (tableID == "StationLivestockTableList") {
                                                if (full.deathDate != null) {
                                                    customClassInFN = "disabled"
                                                }
                                            }
                                            //view button
                                            var view = '<a href="' + addNewLink + param + 'Id=' + full.id + '&View=1" title="View"><i class="fa fa-eye text-secondary ms-1"></i></a>';
                                            //edit button
                                            var edit = ' | <a href="' + addNewLink + param + 'Id=' + full.id + ' &MId=' + sessionStorage.getItem(LocalStorageValues.MODULEID) + '" class="' + customClassInFN + '" title="Edit"><i class="fa fa-edit text-secondary ms-1"></i></a> ';
                                            //delete button
                                            var del = ' | <a onclick="' + className + '.delete(\'' + full.id + '\')" class="' + customClassInFN + '" title="Delete"><i class="fa fa-xmark text-danger ms-1"></i></a>';
                                            //Export ISO PDF button
                                            //var exp = ' | <a onclick="' + className + '.export(\'' + full.id + '\')" title="Export PDF"><i class="fa fa-file-export ms-1"></i></a>';
                                            var exp = '';
                                            //refresh button appears only when status is declined
                                            var rework = ' |<a onclick="' + className + '.rework(\'' + full.id + '\')" title="Rework"><i class="fa fa-rotate-right text-danger ms-1"></i></a>';

                                            //list of buttons
                                            var btn = '';


                                            //check access
                                            if (access.length > 0) {
                                                if (access.find(x => x.accessType == AccessOperationType.Read) != null) {
                                                    if (access.find(x => x.accessType == AccessOperationType.Read).hasAccess) {
                                                        btn += view
                                                    }
                                                }
                                                if (access.find(x => x.accessType == AccessOperationType.Update) != null) {
                                                    if (access.find(x => x.accessType == AccessOperationType.Update).hasAccess) {
                                                        btn += edit
                                                    }
                                                }
                                                if (access.find(x => x.accessType == AccessOperationType.Delete) != null) {
                                                    if (access.find(x => x.accessType == AccessOperationType.Delete).hasAccess) {
                                                        if (full.approvalStatus == "Draft") {
                                                            btn += del
                                                        } else {
                                                            if (showDelete) {
                                                                btn += del
                                                            }
                                                        }
                                                    }
                                                }
                                                if (access.find(x => x.accessType == AccessOperationType.Export) != null) {
                                                    if (access.find(x => x.accessType == AccessOperationType.Export).hasAccess) {
                                                        btn += exp
                                                    }
                                                }


                                            }

                                            if (re == true && full.approvalStatus == "Declined") {

                                                btn += rework
                                            }
                                            //else if (re == true && full.approvalStatus == "Declined") {

                                            //    btn = view + rework
                                            //} else if (full.approvalStatus == "Cancelled" || full.approvalStatus == "Approved" || full.approvalStatus == "Completed") {
                                            //    btn = view
                                            //}
                                            //else if (full.approvalStatus == "Draft") {
                                            //    btn = view + edit + del
                                            //}
                                            //else {

                                            //    btn = view + edit
                                            //    if (showDelete) {
                                            //        btn += del
                                            //    }
                                            //}
                                            if (customButton != null) {
                                                var customButtonReplacement = customButton.replace('[ID]', full.id)
                                                btn += customButtonReplacement
                                            }
                                            data = '<div class="text-nowrap">' + btn + '</div>';
                                            return data;
                                        }
                                    }
                                ],
                                columns: columnList, //column list 
                                buttons: [ //set buttons above table
                                    //Excel Export Button
                                    {
                                        extend: 'excel',
                                        text: '<i class="fa fa-file-excel me-2"></i>Export Excel',
                                        className: 'form-control btn-c-secondary d-none',
                                        exportOptions: {
                                            columns: ':visible:not(.sorting_disabled)'
                                        }
                                    },
                                    //Open Add popup
                                    '<div class="d-flex ' + addButtonClass + '"><button id="list-action-button" class="form-control w-auto me-3 btn-c-secondary ' + stationButton + '" onclick="' + className + '.sendToStation()" disabled>Send To Station</button>' +
                                    '<a href="' + addNewLink + '"><button class="form-control btn-c-primary d-inline ' + showAddClass + createBtnShow + customSeqButton + '"><i class="fa fa-plus me-2"></i>Add New</button></a></div>',
                                ],
                                drawCallback: function () {
                                    // BUTTON EVENT ON THE DATA TABLE
                                }
                            });
                        }
                        
                    }
                    //Date Range
                    if (hasDateRange == true) {
                        $('.buttons-html5').after('<div><div class="datatableDateRange"><span> <i class="fa fa-calendar me-2 span-medium naveo-color-grey"></i><span class="span-medium naveo-color-grey">Date: </span><input type="text" id="DateRangePicker" class="dateRange" onchange="' + className + '.filterDatatable()"/></span></div></div>');
                        DateRangeService.dateRangePicker();
                    }
                })
            })
        })
    }

    //Data table for services
    static initializeDataGridListing(tableID, addNewLink, url, columnList, className, hasDateRange = false, re = false, showDelete = false, customSeqButton = " d-none", customButton = null, stationButton = "d-none", showAction = true) {
        var dataToShow = null; //init data variable
        var access = []; // init access variable
        var createBtnShow = "";
        var addButtonClass = "";
        var parameter = sessionStorage.getItem(LocalStorageValues.MODULEID);
        let MID = '&MId=' + parameter;
        if (hasDateRange == false) {
            addButtonClass = "ms-auto";
        }
        if (!addNewLink.includes("?")) {
            MID = "";
        }
        if (url == "") {

            url = new Variable().GetRowAccess
        }
        ApiService.callCustom(ApiType.POST, new Variable().getWorkflowSeq, parameter, (dt) => { //Calls api to get WF Sequence
            if (dt.data) {
                customSeqButton = "";
            }

            ApiService.callCustom(ApiType.POST, url, parameter, (data) => { //Calls api to get data
                //set access levels
                if (data.access != undefined || data.access != null) {
                    access = data.access;
                    if (access.length > 0) {

                        if (access.find(x => x.accessType == AccessOperationType.Create) != null) {
                            if (!access.find(x => x.accessType == AccessOperationType.Create).hasAccess) {
                                createBtnShow = "d-none";
                            }
                        }
                    }

                }
                dataToShow = data.data; //set data from api in data variable
                if ($('#' + tableID).length > 0) { //check if table exists
                    this.destroyDataGrid(tableID)
                    $('#' + tableID).DataTable({ //init table
                        dom: "Bfr<'customActionsdt'>tip", //button Strip
                        searching: true, //show search
                        paging: false, //no paging
                        processing: true, //processing
                        scrollY: 'auto', //fixed height
                        scrollX: 'auto',
                        order: [[0, 'desc']], //order
                        scrollCollapse: true,//scroll
                        language: { // search anf info bars
                            search: '',
                            emptyTable: 'No data available',
                            searchPlaceholder: 'Search',
                            info: "Showing _TOTAL_ entries",
                            infoEmpty: "Showing 0 entries",
                            infoFiltered: "(filtered from _MAX_ entries)",
                        },
                        data: dataToShow, // data to display
                        columnDefs: [ // set action column
                            {
                                //set column order for action
                                targets: columnList.length,
                                //remove sorting on action
                                orderable: false,
                                //set width of action column
                                width: "50px",
                                visible: showAction,
                                render: function (data, type, full, meta) {
                                    var param = "?";
                                    if (addNewLink.includes("?")) {
                                        param = "&";
                                    }

                                    let customClassInFN = "";
                                    if (tableID == "StationLivestockTableList") {
                                        if (full.deathDate != null) {
                                            customClassInFN = "disabled"
                                        }
                                    }
                                    //view button
                                    var view = '<a href="' + addNewLink + param + 'Id=' + full.id + '&View=1" title="View"><i class="fa fa-eye text-secondary ms-1"></i></a>';
                                    //edit button
                                    var edit = ' | <a href="' + addNewLink + param + 'Id=' + full.id + ' &MId=' + sessionStorage.getItem(LocalStorageValues.MODULEID) + '" class="' + customClassInFN + '" title="Edit"><i class="fa fa-edit text-secondary ms-1"></i></a> ';
                                    //delete button
                                    var del = ' | <a onclick="' + className + '.delete(\'' + full.id + '\')" class="' + customClassInFN + '" title="Delete"><i class="fa fa-xmark text-danger ms-1"></i></a>';
                                    //Export ISO PDF button
                                    //var exp = ' | <a onclick="' + className + '.export(\'' + full.id + '\')" title="Export PDF"><i class="fa fa-file-export ms-1"></i></a>';
                                    var exp = '';
                                    //refresh button appears only when status is declined
                                    var rework = ' |<a onclick="' + className + '.rework(\'' + full.id + '\')" title="Rework"><i class="fa fa-rotate-right text-danger ms-1"></i></a>';

                                    //list of buttons
                                    var btn = '';


                                    //check access
                                    if (access.length > 0) {
                                        if (access.find(x => x.accessType == AccessOperationType.Read) != null) {
                                            if (access.find(x => x.accessType == AccessOperationType.Read).hasAccess) {
                                                btn += view
                                            }
                                        }
                                        if (access.find(x => x.accessType == AccessOperationType.Update) != null) {
                                            if (access.find(x => x.accessType == AccessOperationType.Update).hasAccess) {
                                                btn += edit
                                            }
                                        }
                                        if (access.find(x => x.accessType == AccessOperationType.Delete) != null) {
                                            if (access.find(x => x.accessType == AccessOperationType.Delete).hasAccess) {
                                                if (full.approvalStatus == "Draft") {
                                                    btn += del
                                                } else {
                                                    if (showDelete) {
                                                        btn += del
                                                    }
                                                }
                                            }
                                        }
                                        if (access.find(x => x.accessType == AccessOperationType.Export) != null) {
                                            if (access.find(x => x.accessType == AccessOperationType.Export).hasAccess) {
                                                btn += exp
                                            }
                                        }


                                    }

                                    if (re == true && full.approvalStatus == "Declined") {

                                        btn += rework
                                    }
                                    //else if (re == true && full.approvalStatus == "Declined") {

                                    //    btn = view + rework
                                    //} else if (full.approvalStatus == "Cancelled" || full.approvalStatus == "Approved" || full.approvalStatus == "Completed") {
                                    //    btn = view
                                    //}
                                    //else if (full.approvalStatus == "Draft") {
                                    //    btn = view + edit + del
                                    //}
                                    //else {

                                    //    btn = view + edit
                                    //    if (showDelete) {
                                    //        btn += del
                                    //    }
                                    //}
                                    if (customButton != null) {
                                        var customButtonReplacement = customButton.replace('[ID]', full.id)
                                        btn += customButtonReplacement
                                    }
                                    data = '<div class="text-nowrap">' + btn + '</div>';
                                    return data;
                                }
                            }
                        ],
                        columns: columnList, //column list 
                        buttons: [ //set buttons above table
                            //Excel Export Button
                            {
                                extend: 'excel',
                                text: '<i class="fa fa-file-excel me-2"></i>Export Excel',
                                className: 'form-control btn-c-secondary d-none',
                                exportOptions: {
                                    columns: ':visible:not(.sorting_disabled)'
                                }
                            },
                            //Open Add popup
                            '<div class="d-flex ' + addButtonClass + '"><button id="list-action-button" class="form-control w-auto me-3 btn-c-secondary ' + stationButton + '" onclick="' + className + '.sendToStation()" disabled>Send To Station</button>' +
                            '<a href="' + addNewLink + MID + '"><button class="form-control btn-c-primary d-inline ' + createBtnShow + customSeqButton + '"><i class="fa fa-plus me-2"></i>Add New</button></a></div>',
                        ],
                        drawCallback: function () {
                            // BUTTON EVENT ON THE DATA TABLE
                        }
                    });
                }
                //Date Range
                if (hasDateRange == true) {
                    $('.buttons-html5').after('<div><div class="datatableDateRange"><span> <i class="fa fa-calendar me-2 span-medium naveo-color-grey"></i><span class="span-medium naveo-color-grey">Date: </span><input type="text" id="DateRangePicker" class="dateRange" onchange="' + className + '.filterDatatable()"/></span></div></div>');
                    DateRangeService.dateRangePicker();
                }
            })
        })
    }
    //Data table for  main listing
    static initializeDataGridActivities(tableID, partialViewId, onclickPartial, columnList, className) {
        if ($('#' + tableID).length > 0) { //check if table exists
            // $('#' + tableID).DataTable().destroy();
            $('#' + tableID).DataTable({ //init table
                dom: "Bfr<'customActionsdt'>tip", //button Strip
                searching: true, //show search
                paging: false, //no paging
                processing: true, //processing
                scrollY: 'auto', //fixed height
                scrollX: 'auto', //fixed width
                scrollCollapse: true,//scroll
                language: { // search anf info bars
                    search: '',
                    emptyTable: 'No data available',
                    searchPlaceholder: 'Search',
                    info: "Showing _TOTAL_ entries",
                    infoEmpty: "Showing 0 entries",
                    infoFiltered: "(filtered from _MAX_ entries)",
                },
                //scrollX: true,
                columnDefs: [ // set action column
                    {
                        //set column order for action
                        targets: columnList.length,
                        //remove sorting on action
                        orderable: false,
                        //set width of action column
                        width: "10%",
                        render: function (data, type, full, meta) {
                            data =
                                //edit button
                                '<a href="#' + partialViewId + '" onclick="' + className + '.openPopupAddEdit(\'' + full.id + '\', ' + ScreenMode.EDIT + ')"  " title="Edit"><i class="fa fa-edit text-secondary"></i></a> '

                            return data;
                        }
                    }
                ],
                columns: columnList, //column list
                buttons: [ //set buttons above table
                    //Excel Export Button
                    {
                        extend: 'excel',
                        text: '<i class="fa fa-file-excel me-2 d-none"></i>Export Excel',
                        className: 'form-control btn-c-secondary d-none',
                        exportOptions: {
                            columns: ':visible:not(.sorting_disabled)'
                        }
                    },
                    //Open Add popup
                    '<a  href="#' + partialViewId + '"><button class="form-control btn-c-primary" onclick="' + onclickPartial + '"><i class="fa fa-plus me-2"></i>Add New</button></a>',
                ],
                drawCallback: function () {
                    // BUTTON EVENT ON THE DATA TABLE
                }
            });
        }
        //Date Range 
        $('.buttons-html5').after('<div class="datatableDateRange"><span> <i class="fa fa-calendar me-2 span-medium"></i><span class="span-medium">Date Range : </span><input type="text" id="DateRangePicker" class="dateRange" onchange="' + className + '.filterDatatable()"/></span></div>');
        DateRangeService.dateRangePicker();
    }

    //Data table for services
    static initializeDataGridServices(tableID, columnList, data, deleteAction = false, deleteFuntion) {
        if ($('#' + tableID).length > 0) { //check if table exists
            // $('#' + tableID).DataTable().destroy();
            $('#' + tableID).DataTable({ //init table
                dom: "Brtip", //button Strip
                searching: true, //show search
                paging: false, //no paging
                processing: true, //processing
                scrollY: 'auto', //fixed height
                scrollX: 'auto',
                scrollCollapse: true,//scroll
                order: [[0, 'desc']], // order
                language: { // search anf info bars
                    search: '',
                    emptyTable: 'No data available',
                    searchPlaceholder: 'Search',
                    info: "Showing _TOTAL_ entries",
                    infoEmpty: "Showing 0 entries"
                },
                data: data, // data to display
                columnDefs: [ // set action column
                    {
                        //set column order for action
                        targets: columnList.length,
                        //remove sorting on action
                        orderable: false,
                        //
                        visible: deleteAction,
                        render: function (data, type, full, meta) {
                            var data = "";
                            if (deleteAction) {
                                data += ' <a onclick="' + deleteFuntion + '(\'' + meta.row + '\')" title="Delete"><i class="fa fa-times naveo-color"></i></a> ';
                            }
                            return data;
                        }
                    }
                ],
                columns: columnList, //column list 
                buttons: [],
                drawCallback: function () {
                    // BUTTON EVENT ON THE DATA TABLE
                }
            });
        }
    }
    //Data table for Fil eUpload
    static initializeDataGridFile(tableID, columnList, data, downloadAction = true, deleteAction = true, deleteFunction = null, isBase64 = false) {
        if ($('#' + tableID).length > 0) { //check if table exists
            // $('#' + tableID).DataTable().destroy();
            $('#' + tableID).DataTable({ //init table
                dom: "Brtip", //button Strip
                searching: true, //show search
                paging: false, //no paging
                processing: true, //processing
                scrollY: 'auto', //fixed height
                scrollX: 'auto',
                scrollCollapse: true,//scroll
                order: [[0, 'desc']], // order
                language: { // search anf info bars
                    search: '',
                    emptyTable: 'No data available',
                    searchPlaceholder: 'Search',
                    info: "Showing _TOTAL_ entries",
                    infoEmpty: "Showing 0 entries"
                },
                data: data, // data to display
                columnDefs: [ // set action column
                    {
                        //set column order for action
                        targets: columnList.length,
                        //remove sorting on action
                        orderable: false,
                        //set width of action column
                        width: "50px",
                        //
                        visible: downloadAction,
                        render: function (data, type, full, meta) {
                            var data = "";
                            if (downloadAction && full.id != 0 && !isBase64) {
                                data += ' <a href="/FileUpload/DownloadFile?Id=' + full.id + '" title="Download"><i class="fa fa-download naveo-color"></i></a> ';
                            }
                            if (downloadAction && full.id != 0 && isBase64) {
                                data += ' <a href="' + full.file + '" download="' + full.fileName + '"><i class="fa fa-download naveo-color"></i></a> ';
                            }
                            if (deleteAction) {
                                data += ' <a onclick="' + deleteFunction + '(' + meta.row + ',\'' + tableID + '\')" title="Delete"><i class="fa fa-times naveo-color"></i></a> ';
                            }
                            return data;
                        }
                    }
                ],
                columns: columnList, //column list 
                buttons: [],
                drawCallback: function () {
                    // BUTTON EVENT ON THE DATA TABLE
                }
            });
        }
    }
    //Data table for services
    static initializeDataGridDynamic(tableID, columnList, className, dataToShow, classForButton = "", showDelete = true, paging = false, order = false, deleteFunction = "", customRecordButtons = null) {
        if (!$.fn.DataTable.isDataTable('#' + tableID)) {
            if (paging == false) {
                if ($('#' + tableID).length > 0) { //check if table exists
                    $('#' + tableID).DataTable({ //init table
                        searching: false, //show search
                        paging: false, //no paging
                        processing: true, //processing
                        ordering: order == true ? [[0, 'desc']] : order, // no ordering
                        data: dataToShow == null ? dataToShow : dataToShow.data, // data to display
                        info: false,
                        columnDefs: [ // set action column
                            {
                                //set column order for action
                                targets: columnList.length,
                                //remove sorting on action
                                orderable: false,
                                //set width of action column
                                width: "10%",
                                visible: showDelete,
                                render: function (data, type, full, meta) {
                                    if (tableID == "stock-request-Table") {
                                        data = '<a onclick="' + className + '.aprrove(\'' + full.id + '\')"  title="Approved"><i class="fa fa-check text-success ms-1"></i></a> ' +
                                            '| <a onclick="' + className + '.decline(\'' + full.id + '\')"  title="Declined"><i class="fa fa-xmark text-danger ms-1"></i></a>';
                                    } else if (tableID == "store-item-Table") {
                                        data = '<a onclick="' + className + '.setValues(\'' + full.id + '\')"  title="Add"><i class="fa fa-plus text-secondary"></i></a>';
                                    } else if (tableID == "equipment-Resource-R-Table") {
                                        //delete button
                                        data = ' <a class="' + classForButton + '" onclick="' + className + '.deleteRelationEquip(\'' + meta.row + '\',\'' + tableID + '\')" title="Delete"><i class="fa fa-xmark text-danger"></i></a>';

                                    } else if (tableID == "resource-Resource-R-Table") {
                                        //delete button
                                        data = ' <a class="' + classForButton + '" onclick="' + className + '.deleteRelationRank(\'' + meta.row + '\',\'' + tableID + '\')" title="Delete"><i class="fa fa-xmark text-danger"></i></a>';
                                    } else {
                                        if (deleteFunction == "") {
                                            //delete button
                                            data = ' <a class="' + classForButton + '" onclick="' + className + '.deleteRelation(\'' + meta.row + '\',\'' + tableID + '\')" title="Delete"><i class="fa fa-xmark text-danger"></i></a>';
                                        } else {
                                            //delete button
                                            data = ' <a class="' + classForButton + '" onclick="' + className + '.' + deleteFunction + '(\'' + meta.row + '\')" title="Delete"><i class="fa fa-xmark text-danger"></i></a>';
                                        }
                                       
                                    }
                                    if (customRecordButtons != null) {
                                        customRecordButtons = customRecordButtons.replace("[ID]", full.id)
                                        data += customRecordButtons;
                                    }
                                    return data;

                                }
                            }
                        ],
                        columns: columnList, //column list
                        buttons: [], //None
                        drawCallback: function () {
                            // BUTTON EVENT ON THE DATA TABLE
                        }
                    });

                }
            } else {
                if ($('#' + tableID).length > 0) { //check if table exists
                    $('#' + tableID).DataTable({ //init table
                        searching: false, //show search
                        paging: true, //paging
                        lengthMenu: [10, 20, 30, 50],
                        processing: true, //processing
                        lengthChange: true,
                        pageLength: 5,
                        ordering: false, // no ordering
                        data: dataToShow == null ? dataToShow : dataToShow.data, // data to display
                        info: false,
                        columnDefs: [ // set action column
                            {
                                //set column order for action
                                targets: columnList.length,
                                //remove sorting on action
                                orderable: false,
                                //set width of action column
                                width: "10%",
                                visible: showDelete,
                                render: function (data, type, full, meta) {
                                   if (tableID == "stock-request-Table") {
                                        data = '<a onclick="' + className + '.aprrove(\'' + full.id + '\')"  title="Approved"><i class="fa fa-check text-success ms-1"></i></a> ' +
                                            '| <a onclick="' + className + '.decline(\'' + full.id + '\')"  title="Declined"><i class="fa fa-xmark text-danger ms-1"></i></a>';
                                    } else if (tableID == "store-item-Table") {
                                        data = '<a onclick="' + className + '.setValues(\'' + full.id + '\')"  title="Add"><i class="fa fa-plus text-secondary"></i></a>';
                                    } else {
                                        data =
                                            //delete button
                                            ' <a class="' + classForButton + '" onclick="' + className + '.deleteRelation(\'' + meta.row + '\',\'' + tableID + '\')" title="Delete"><i class="fa fa-xmark text-danger"></i></a>';
                                    }
                                    return data;

                                }
                            }
                        ],
                        columns: columnList, //column list
                        buttons: [], //None
                        drawCallback: function () {
                            // BUTTON EVENT ON THE DATA TABLE
                        }
                    });

                }
            }
            
        }

    }

    /*
    =======================
       Datatable Actions
    =======================
    */
    //update datatbles
    static updateDataGrid(tableID, data) {
        if ($.fn.DataTable.isDataTable('#' + tableID) && data.length > 0) {
            $('#' + tableID).dataTable().fnClearTable();
            $('#' + tableID).dataTable().fnAddData(data);
        } else if (data.length == 0) {
            $('#' + tableID).dataTable().fnClearTable();
        }

    }
    //destroy datatble 
    static destroyDataGrid(tableID) {
        if ($.fn.DataTable.isDataTable('#' + tableID)) {
            $('#' + tableID).DataTable().destroy();
        }
    }
    //Download File
    static downloadGrid(type) {
        if (type.toLowercase() == "excel") {
            $('.buttons-excel').click();
        }
    }
    //export Excel
    static exportExcelBtn() {
        return {
            text: '<i class="fa fa-file-excel me-2"></i>Export Excel',
            className: 'form-control btn-c-secondary',
            attr: {
                title: 'Excel Export',
                id: 'excelExport'
            }
        };
    }

    /*
    =======================
       Nested Datatable 
    =======================
    */
    //Data table for  main listing
    static initializeDataGridNestedAPI(tableID, url, param = "", columnList, childColumns = null, customChild = null, search = false, exportExcel = false) {
        var dataToShow = null; //init data variable

        ApiService.callCustom(ApiType.POST, url, param, (data) => { //Calls api to get data
            dataToShow = data.data; //set data from api in data variable
            if ($('#' + tableID).length > 0) { //check if table exists
                var table = $('#' + tableID).DataTable({ //init table
                    dom: "Bfrtip", //button Strip
                    searching: search, //show search
                    paging: false, //no paging
                    processing: true, //processing
                    scrollY: 'auto', //fixed height
                    scrollX: 'auto',
                    scrollCollapse: true,//scroll
                    language: { // search anf info bars
                        search: '',
                        emptyTable: 'No data available',
                        searchPlaceholder: 'Search',
                        info: "Showing _TOTAL_ entries",
                        infoEmpty: "Showing 0 entries",
                        infoFiltered: "(filtered from _MAX_ entries)",
                    },
                    //scrollX: true,
                    data: dataToShow, // data to display
                    columns: columnList, //column list
                    order: [[1, 'asc']],
                    buttons: exportButtons, //set buttons above table,
                    drawCallback: function () {
                        // BUTTON EVENT ON THE DATA TABLE
                    }
                });

                $('#' + tableID + ' tbody').on('click', 'td.dt-control', function () {
                    var tr = $(this).closest('tr');
                    var row = table.row(tr);
                    if (row.child.isShown()) {
                        // This row is already open - close it
                        row.child.hide();
                        tr.removeClass('shown');
                    }
                    else {
                        if (childColumns == null) {
                            // Open this row
                            row.child(DatatableService.formatCustom(row.data(), customChild)).show();
                        } else {
                            // Open this row
                            row.child(DatatableService.format(row.data(), row.index())).show();
                            DatatableService.addDataToChild(row.data(), row.index(), childColumns);
                        }

                        tr.addClass('shown');
                    }

                });
            }
        })

        return dataToShow;
    }

    static addDataToChild(d, rowIndex, childColumns) {

        $('#child_' + rowIndex + '').DataTable({
            "data": d.accessRightDetailDTO,
            "bLengthChange": false,
            "bInfo": false,
            "bFilter": false,
            "bPaginate": false,
            "columns": childColumns
        });
    }
    static format(d, rowIndex) {
        // `d` is the original data object for the row
        return '<table border="0" id="child_' + rowIndex + '"></table>';

    }
    static formatCustom(row, customChild) {

        return ''
    }

    //Nested Grid Without API
    static initializeDataGridNested(tableID, columnList, childColumns, dataToShow = null, search = false, exportExcel = false) {
        var exportButtons = [];
        if (exportExcel) {
            exportButtons.push(this.exportExcelBtn)
        }
        if ($('#' + tableID).length > 0) { //check if table exists
            var table = $('#' + tableID).DataTable({ //init table
                dom: "Bfrtip", //button Strip
                searching: search, //show search
                paging: false, //no paging
                processing: true, //processing
                scrollY: 'auto', //fixed height
                scrollCollapse: true,//scroll
                language: { // search anf info bars
                    search: '',
                    emptyTable: 'No data available',
                    searchPlaceholder: 'Search',
                    info: "Showing _TOTAL_ entries",
                    infoEmpty: "Showing 0 entries",
                    infoFiltered: "(filtered from _MAX_ entries)",
                },
                //scrollX: true,
                data: dataToShow, // data to display
                columns: columnList, //column list
                buttons: exportButtons,
                drawCallback: function () {
                    // BUTTON EVENT ON THE DATA TABLE
                }
            });

            $('#' + tableID + ' tbody').on('click', 'td.dt-control', function () {
                var tr = $(this).closest('tr');
                var row = table.row(tr);
                if (row.child.isShown()) {
                    // This row is already open - close it
                    row.child.hide();
                    tr.removeClass('shown');
                }
                else {
                    // Open this row
                    row.child(DatatableService.format(row.data(), row.index())).show();
                    DatatableService.addDataToChild(row.data(), row.index(), childColumns);
                    tr.addClass('shown');
                }

            });
        }
    }


}