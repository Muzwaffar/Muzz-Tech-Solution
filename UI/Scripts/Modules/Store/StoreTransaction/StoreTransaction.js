import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { Validation } from "/Scripts/Common/ValidationService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApprovalStatus } from "/Scripts/Common/Enums.js";
import { AlertBoxService } from "/Scripts/Common/AlertBoxService.js";
import { Variable } from "/Scripts/Common/variable.js";
export class StoreTransaction {//class that handles the Control Book screen
    storeItemData = [];
    constructor() {
        this.getList();
        //Get All Dropdown Values
        this.getDropdowns();
        $(document).ready(function () {
            //Datatble column for all tables
            var StockRequestList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'name', title: 'Name' },
                { data: 'requestDate', title: 'Request Date' },
                { data: 'requestBy', title: 'Request By' },
                { data: 'itemSource', title: 'ItemSource' },
                { data: 'quantity', title: 'Quantity' }
            ]
            //Init Dtatable
            DatatableService.initializeDataGridDynamic("stock-request-Table", StockRequestList, "StoreTransaction", null, "", true)

            var StoreTransactionList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'name', title: 'Name' },
                { data: 'storeLocation', title: 'Store Location' },
                { data: 'quantity', title: 'Quantity' },
                { data: 'date', title: 'Date' },
                { data: 'type', title: 'Type' }
            ]
            //Init Dtatable
            DatatableService.initializeDataGridDynamic("store-transaction-Table", StoreTransactionList, "StoreTransaction", null, "", false)

            var StoreItemList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'name', title: 'Name' },
                { data: 'group', title: 'Group' },
                { data: 'category', title: 'Category' },
                { data: 'quantity', title: 'Quantity' }
            ]
            //Init Dtatable
            DatatableService.initializeDataGridDynamic("store-item-Table", StoreItemList, "StoreTransaction", null, "", true)

            var StoreTransactionInList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'name', title: 'Name' },
                { data: 'storeLocation', title: 'Store Location' },
                { data: 'quantity', title: 'Quantity' },
                { data: 'date', title: 'Date' },
                { data: 'type', title: 'Type' }
            ]
            //Init Dtatable
            DatatableService.initializeDataGridDynamic("in-Table", StoreTransactionInList, "StoreTransaction", null, "", false)

            var StoreTransactionOutList = [
                { data: 'id', title: 'Id', visible: false },
                { data: 'name', title: 'Name' },
                { data: 'storeLocation', title: 'Store Location' },
                { data: 'quantity', title: 'Quantity' },
                { data: 'date', title: 'Date' },
                { data: 'type', title: 'Type' }
            ]
            //Init Dtatable
            DatatableService.initializeDataGridDynamic("out-Table", StoreTransactionOutList, "StoreTransaction", null, "", false)

        })

        var RequestList = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'name', title: 'Name' },
            { data: 'requestDate', title: 'Request Date' },
            { data: 'requestBy', title: 'Request By' },
            { data: 'itemSource', title: 'ItemSource' },
            { data: 'quantity', title: 'Quantity' }
        ]
        //Init Dtatable
        DatatableService.initializeDataGridDynamic("request-Table", RequestList, "StoreTransaction", null, "", false)
    }
    //set Milk Record Book 
    getList() {
        ApiService.callCustom(ApiType.POST, variable.getStockRequestList, "", (data) => {
            var aprroveDecline = data.data.filter(p => p.approvalStatus != ApprovalStatus.None );
            var request = data.data.filter(p => p.approvalStatus == ApprovalStatus.None );
            if (data) {
                DatatableService.updateDataGrid('stock-request-Table', request);
                DatatableService.updateDataGrid('request-Table', aprroveDecline);
            }
        });

        ApiService.callCustom(ApiType.POST, variable.getStoreTransactionList, "", (data) => {
            if (data) {
                var storeIn = data.data.filter(p => p.type == 'In');
                var storeOut = data.data.filter(p => p.type == 'Out');
                DatatableService.updateDataGrid('store-transaction-Table', data.data);
                DatatableService.updateDataGrid('in-Table', storeIn);
                DatatableService.updateDataGrid('out-Table', storeOut);

            }
        });

        ApiService.callCustom(ApiType.POST, variable.getStoreItemList, "", (data) => {
            if (data) {
                this.storeItemData = data.data;
                DatatableService.updateDataGrid('store-item-Table', this.storeItemData);
            }
        });
    }

    //get all dropdown values
    getDropdowns() {
        var dropdown = null;
        ApiService.callCustom(ApiType.POST, variable.getAllDropdownValuesStoreTransaction, "", (data) => {
            if (data) {
                dropdown = data.data;
                if (dropdown != null) {
                    // Set Category list of values
                    DropdownService.initDropdown("select-store-location", dropdown, "StoreLocation");

                }
            }
        });
    }

    setValues(Id) {
        var name = this.storeItemData.find(x => x.id == Id).name;
        var quantity = this.storeItemData.find(x => x.id == Id).quantity;
        $('#StoreId').val(Id);
        $('#Name').val(name);
        $('#AvailableQuantity').val(quantity);
    }

    //check Valid
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["CostPrice", "DateRecieved", "select-store-location", "Quantity"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        //return boolean id valid or not
        return valid
    }

    // save action button
    save() {
        //Show loader 
        Loader.showLoader("Saving Data...");
        // Set data to send to api
        let data = DefaultService.result(document.getElementById("Store-AddEdit-Panel"));
        //check validation and data 
        if (this.valid() && data != false) {
            if (data["ExpiryDate"] == "") { data["ExpiryDate"] = null }
            // call api to save 
            ApiService.callCustom(ApiType.POST, variable.saveStoreTransaction, data, (response) => {
                // if response true: Save successfull
                if (response.data) {
                    toastr.success("Saved Successfully!")
                    window.location.reload();
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

    aprrove(id) {
        let dataToSend = {
            Id: id,
            ApprovalStatus: ApprovalStatus.Approved
        }
        AlertBoxService.approveStockRequestAlertBox(new Variable().getApproveDeclineStockRequest, dataToSend, "Stock Request");
    }

    decline(id) {
        let dataToSend = {
            Id: id,
            ApprovalStatus: ApprovalStatus.Declined
        }
        AlertBoxService.declineStockRequestAlertBox(new Variable().getApproveDeclineStockRequest, dataToSend, "Stock Request");
    }

}
window.StoreTransaction = new StoreTransaction();