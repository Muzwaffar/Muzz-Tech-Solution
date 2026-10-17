import { Validation } from "/Scripts/Common/ValidationService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";

export class SectionHelper {

    static SaveUpdateFormatting() {
        //get all element values
        let elem = DefaultService.resultObject("elem", document.getElementById("scrollspy-element"));
        //get element type values
        let InputTypeDetails = DefaultService.resultObject("InputTypeDetails", document.getElementById("scrollspy-element"));
        InputTypeDetails.Name = $("#select-element-type :selected").text();
        //get lookup values
        let LookUpValue = DefaultService.resultObject("LookUpValue", document.getElementById("scrollspy-element"));
        LookUpValue.Name = $("#select-uom :selected").text();
        //get source values
        let source = DefaultService.resultObject("source", document.getElementById("scrollspy-element"));
        source.Name = $("#select-source :selected").text();
        //get parameter 1 values
        let ParameterList1 = DefaultService.resultObject("ParameterList1", document.getElementById("scrollspy-element"));
        ParameterList1.Name = $("#select-param-1 :selected").text();
        let Parameter1 = DefaultService.resultObject("Parameter1", document.getElementById("scrollspy-element"));

        let IsListing = $("#IsListing").prop("checked");
        let URL = $("#URL").val();
        let Order = $("#Order").val();
        let ListingName = $("#ListingName").val();
        
        
        //set relation to send to back
        let relationToSend = {
            Name: elem.Name,
            Notification: false,
            IsMobileChecklist: false,
            InputTypeDetails: Validation.checkDropdownSelectedID(InputTypeDetails.Id) ? InputTypeDetails : null,
            LookUpValue: Validation.checkDropdownSelectedID(LookUpValue.Id) ? LookUpValue : null,
            Source: Validation.checkDropdownSelectedID(source.Id) ? source : null,
            Parameters: [],
            IsListing: IsListing,
            ListingName: ListingName,
            Order: Order,
            URL: URL
        }
        if (InputTypeDetails.Name == "Attachment") {
            let dat = { ...relationToSend }
            relationToSend.InnerElemList = [dat]
        }

        //set relation to set in datatable
        let relation = {
            ElementName: relationToSend.Name,
            ElementType: InputTypeDetails.Name,
            Order: Order,
            URL : URL,
            UOM: LookUpValue.Name,
            SourceName: source.Name,
            SourceParam1: ParameterList1.Name,
            Param1: Parameter1.Value,
            SourceParam2: "",
            Param2: 0,
            Notification: false,
            IsMobileChecklist: false,
            IsListing: IsListing,
            ListingName: ListingName
        }

        //set parameter 1 values in relation to send
        if (Validation.checkDropdownSelectedID(ParameterList1.Id)) {
            var param1 = {};
            param1.Name = "Parameter 1";
            param1.Value = Parameter1.Value;
            ParameterList1.Name = $("#select-param-1 :selected").text();
            param1.ParameterList = ParameterList1;
            relationToSend.Parameters.push(param1);
        }
        var validURL = false;
        var validLN = false;
        var validUOM = false;
        if (!$("#URLDIV").hasClass("d-none")) {
            if (!Validation.formValidation(["URL"])) {
                toastr.error("Please fill in URL field");
            } else {
                validURL = true;
            }
        } else {
            validURL = true
        }
        if (!$("#ListingNameDIV").hasClass("d-none")) {
            if (!Validation.formValidation(["ListingName"])) {
                toastr.error("Please fill in Listing Name field");
            } else {
                validLN = true
            }
        } else {
            validLN = true
        }
        if (!$("#select-uom-container").hasClass("d-none")) {
            if (!Validation.formValidation(["select-uom"])) {
                toastr.error("Please chose a unit of measure type");
            } else {
                validUOM = true
            }
        } else {
            validUOM = true
        }
        if (!Validation.formValidation(["ElemName", "select-element-type", "Order"])) {
            toastr.error("Please fill in required field(s)");
        } else {
            if (validURL && validLN && validUOM) {
                Validation.clearValues(["ElemName", "select-element-type", "select-uom", "select-source", "select-param-1", "Value1", "Order", "ListingName"])
                //return relation to save and relation to display in datagrid
                return { relationToSend: relationToSend, relation: relation };
            }
        }
    }
    
    static SaveUpdateFormattingDT(index, id = null) {
        //get all element values
        let elem = DefaultService.resultObject("elem", document.getElementById("scrollspy-datatable"));
        //get element type values
        let InputTypeDetails = DefaultService.resultObject("InputTypeDetails", document.getElementById("scrollspy-datatable"));
        InputTypeDetails.Name = $("#select-element-typeDT :selected").text();
        //get lookup values
        let LookUpValue = DefaultService.resultObject("LookUpValue", document.getElementById("scrollspy-datatable"));
        LookUpValue.Name = null;
        //get source values
        let source = DefaultService.resultObject("source", document.getElementById("scrollspy-datatable"));
        source.Name = $("#select-sourceDT :selected").text();
        //get parameter 1 values
        let ParameterList1 = DefaultService.resultObject("ParameterList1", document.getElementById("scrollspy-datatable"));
        ParameterList1.Name = $("#select-param-1DT :selected").text();
        let Parameter1 = DefaultService.resultObject("Parameter1", document.getElementById("scrollspy-datatable"));
        //get parameter 2 values
        let ParameterList2 = DefaultService.resultObject("ParameterList2", document.getElementById("scrollspy-datatable"));
        ParameterList2.Name = $("#select-param-2DT :selected").text();
        let Parameter2 = DefaultService.resultObject("Parameter2", document.getElementById("scrollspy-datatable"));

        //set relation to send to back
        let relationToSend = {
            Name: elem.Name,
            Notification: elem.Notification == undefined ? false : elem.Notification,
            IsMobileChecklist: elem.IsMobileChecklist == undefined ? false : elem.IsMobileChecklist,
            InputTypeDetails: Validation.checkDropdownSelectedID(InputTypeDetails.Id) ? InputTypeDetails : null,
            LookUpValue: Validation.checkDropdownSelectedID(LookUpValue.Id) ? LookUpValue : null,
            Source: Validation.checkDropdownSelectedID(source.Id) ? source : null,
            Parameters: [],
            Order: index,
            ParentId: id
        }

        //set relation to set in datatable
        let relation = {
            ElementName: relationToSend.Name,
            ElementType: InputTypeDetails.Name,
            UOM: LookUpValue.Name,
            SourceName: source.Name,
            SourceParam1: ParameterList1.Name,
            Param1: Parameter1.Value,
            SourceParam2: "",
            Param2: 0,
            Notification: false,
            IsMobileChecklist: false,
            Order: index,
            ParentId: id
        }

        //set parameter 1 values in relation to send
        if (Validation.checkDropdownSelectedID(ParameterList1.Id)) {
            var param1 = {};
            param1.Name = "Parameter 1";
            param1.Value = Parameter1.Value;
            ParameterList1.Name = $("#select-param-1DT :selected").text();
            param1.ParameterList = ParameterList1;
            relationToSend.Parameters.push(param1);
        }

        if (!Validation.formValidation(["ElemNameDT", "select-element-typeDT"])) {
            toastr.error("Please fill in required field(s)");
        } else {
            Validation.clearValues(["ElemNameDT", "select-element-typeDT", "select-uomDT", "select-sourceDT", "select-param-1DT", "Value1DT"])
            //return relation to save and relation to display in datagrid
            return { relationToSend: relationToSend, relation: relation };
        }
    }

    static GetElementByIdTableFormat(data) {
        var relation = [];
        $.each(data, (index, value) => {
            let x = {
                ElementName: value.name,
                ElementType: value.inputTypeDetails.name,
                Order: value.Order == 0 ? "" : value.order,
                URL: value.url,
                UOM: value.lookUpValue == null ? "" : value.lookUpValue.name,
                SourceName: value.source == null ? "" : value.source.name,
                SourceParam1: value.parameters.length == 0 ? "" : value.parameters[0].parameterList == null ? "" : value.parameters[0].parameterList.name,
                Param1: value.parameters.length == 0 ? "" : value.parameters[0].value,
                SourceParam2: "",
                Param2: 0,
                Notification: value.notification,
                IsMobileChecklist: value.isMobileChecklist,
                IsListing: value.isListing,
                ListingName: value.listingName,
            }
            relation.push(x)
        })

        return relation
    }

    static GetElementByIdTableFormatDT(data) {
        var relation = [];
        $.each(data, (index, value) => {
            let x = {
                ElementName: value.name,
                ElementType: value.inputTypeDetails.name,
                UOM: value.lookUpValue == null ? "" : value.lookUpValue.description,
                SourceName: value.source == null ? "" : value.source.name,
                SourceParam1: value.parameters.length == 0 ? "" : value.parameters[0].parameterList == null ? "" : value.parameters[0].parameterList.name,
                Param1: value.parameters.length == 0 ? "" : value.parameters[0].value,
                SourceParam2: "",
                Param2: 0,
                Notification: value.notification,
                IsMobileChecklist: value.isMobileChecklist,
                Order: value.order,
                InnerElemList: value.innerElemList,
                ParentId: value.parentId
            }
            relation.push(x)
        })

        return relation
    }
}
window.SectionHelper = new SectionHelper();