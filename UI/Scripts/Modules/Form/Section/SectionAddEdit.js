import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { DropdownService } from "/Scripts/Common/DropdownService.js";
import { SectionHelper } from "/Scripts/Modules/Form/Section/SectionHelper.js";
import { Validation } from "/Scripts/Common/ValidationService.js";

export class SectionAddEdit {//class that handles the Add Edit screen

    //Global Variables
    Id = 0;
    relationArray = []; //save update formatting 
    relationData = [];  //grid list formatting

    relationArrayDT = []; //save update formatting 
    relationDataDT = [];  //grid list formatting

    constructor() {

        var relationColumn = [ //Relation Column List
            { data: 'ElementName', title: 'Element Name' },
            { data: 'ElementType', title: 'Element Type' },
            { data: 'Order', title: 'Order' },
            { data: 'URL', title: 'URL' },
            { data: 'UOM', title: 'Unit of Measure' },
            { data: 'SourceName', title: 'Source' },
            { data: 'SourceParam1', title: 'Source Param' },
            { data: 'Param1', title: 'Parameter' },
            { data: 'IsListing', title: 'Is Listing' },
            { data: 'ListingName', title: 'Listing Name' }
        ];

        var relationColumnDT = [ //Relation Column List
            { data: 'ElementName', title: 'Element Name' },
            { data: 'ElementType', title: 'Element Type' },
            { data: 'UOM', title: 'Unit of Measure' },
            { data: 'SourceName', title: 'Source' },
            { data: 'SourceParam1', title: 'Source Param' },
            { data: 'Param1', title: 'Parameter' },
        ];

        //Event Listener : Show Field URL 
        $('#select-element-type').on('change', () => {
            this.showOptions()
        });

        //Event Listener : Show Field Listing Name
        $('#IsListing').on('click', () => {
            this.showListingName()
        });

        //API To get Dropdown Value
        ApiService.callCustom(ApiType.POST, variable.getSectionDropdown, "", (data) => {
            if (data.data) {
                var dropdown = data.data;
                //Element Type
                DropdownService.initDropdown("select-element-type", dropdown, "InputType");
                //Unit of Measure Select
                DropdownService.initDropdown("select-uom", dropdown, "UOM");
                //Source Select
                DropdownService.initDropdown("select-source", dropdown, "Source");
                //Parameter 1 Select
                DropdownService.initDropdownNull("select-param-1");

                //Element Type
                DropdownService.initDropdown("select-element-typeDT", dropdown, "InputTypeDatatable");
                //Source Select
                DropdownService.initDropdown("select-sourceDT", dropdown, "Source");
                //Parameter 1 Select
                DropdownService.initDropdownNull("select-param-1DT");
                //Parameter 2 Select
                DropdownService.initDropdownNull("select-param-2DT");

                this.getURLparameters();
            }
        })

        //Init Relation Datatable
        DatatableService.initializeDataGridServices('relation-Element-Table', relationColumn, [], true, "SectionAddEdit.deleteRelation");
        //Init Relation Datatable
        DatatableService.initializeDataGridServices('relation-Datatable-Element-Table', relationColumnDT, [], true, "SectionAddEdit.deleteRelationDT");
    }
    //show options whrn selecting element type
    showOptions() {
        var value = $("#select-element-type :selected").text()
        if (value == "Number With UOM") {
            //Number With UOM
            $("#select-uom-container").removeClass("d-none");
            //Drpopdown
            $("#source-param-container").addClass("d-none");
            $("#select-source").val(null).trigger('change');
            $("#select-param-1").val(null).trigger('change');
            $("#Value1").val("");
            //Drpopdown - Dropdown With Add New
            $("#URLDIV").addClass("d-none");
            $("#URL").val("");
        } else if (value.toLowerCase().includes("dropdown")) {
            //Number With UOM
            $("select-uom-container").addClass("d-none");
            $("#select-uom").val(null).trigger('change');
            //Drpopdown
            $("#source-param-container").removeClass("d-none");
            //Drpopdown - Dropdown With Add New
            if (value == "Dropdown With Add New" || value == "Dropdown With Details and Add New") {
                $("#URLDIV").removeClass("d-none");
            } else {
                $("#URLDIV").addClass("d-none");
                $("#URL").val("");
            }
        } else {
            //Number With UOM
            $("#select-uom-container").addClass("d-none");
            $("#select-uom").val(null).trigger('change');
            //Drpopdown
            $("#source-param-container").addClass("d-none");
            $("#select-source").val(null).trigger('change');
            $("#select-param-1").val(null).trigger('change');
            $("#Value1").val("");
            //Drpopdown - Dropdown With Add New
            $("#URLDIV").addClass("d-none");
            $("#URL").val("");
        }
    }
    //show listing name when selecting is listing
    showListingName() {
        var check = $('#IsListing').is(':checked');
        if (check) {
            $("#ListingNameDIV").removeClass("d-none");
        } else {
            $("#ListingNameDIV").addClass("d-none");
            $("#ListingName").val("");
        }
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
            this.getRelationById(this.Id)
        };
        if (urlParams.has('View')) {
            if (parseInt(urlParams.get('View'))) {
                $(".noView").each(function (i, obj) {
                    obj.classList.add("d-none")
                });
            }
        };
    }

    //API to get relation
    getRelationById(id) {
        //set all list to empty
        this.relationArray = [];
        this.relationData = [];
        //check if page has beeen rendered and check if on update
        if (id != undefined && id != 0) {
            // call api to get all elements
            ApiService.callCustom(ApiType.POST, variable.getSectionElement, id, (data) => {
                //set data array 
                this.relationArray = data.data;
                // format data to send to datatgrid if data is found
                if (this.relationArray.length > 0) {
                    //format data for grid view
                    var relation = SectionHelper.GetElementByIdTableFormat(data.data);
                    this.relationData = relation;
                    //update grid with new data
                    DatatableService.updateDataGrid("relation-Element-Table", this.relationData);

                }

                //DATATABLE
                var dtData = data.data.find(x => x.inputTypeDetails.name.toLowerCase() == "datatable");
                // format data to send to datatgrid if data is found
                if (dtData != null) {

                    this.relationArrayDT = dtData.innerElemList;
                    var relationDT = SectionHelper.GetElementByIdTableFormatDT(dtData.innerElemList);
                    this.relationDataDT = relationDT;
                    //update grid with new data
                    DatatableService.updateDataGrid("relation-Datatable-Element-Table", this.relationDataDT);
                    $("#scrollspy-datatable ,#scroll-spy-datatable").removeClass("d-none")
                }


            })
        }
    }

    //on source selection get parameter list
    getParameterDropdown() {
        //get source id
        var id = $("#select-source").val();
        //call api if select source contains a value
        if (id != undefined) {
            //call api if parameter has been selected
            ApiService.callCustom(ApiType.POST, variable.getSectionParameters, id, (data) => {
                //if data not false
                if (data) {
                    // destroy - empty - add new default value and insert data in seleect
                    DropdownService.destroyReinitDropdown("select-param-1", data.data, "ParametersList");
                }
            })
        }
    }

    //on source selection get parameter list
    getParameterDropdownDT() {
        //get source id
        var id = $("#select-sourceDT").val();
        //call api if select source contains a value
        if (id != undefined) {
            //call api if parameter has been selected
            ApiService.callCustom(ApiType.POST, variable.getSectionParameters, id, (data) => {
                //if data not false
                if (data) {
                    // destroy - empty - add new default value and insert data in seleect
                    DropdownService.destroyReinitDropdown("select-param-1DT", data.data, "ParametersList");
                }
            })
        }
    }

    //Add Relation to table
    addRelation() {
        var inputText = $("#select-element-type :selected").text();
        var hasDatatable = null;
        hasDatatable = this.relationData.find(x => x.ElementType == "Datatable");
        if (inputText.toLowerCase().includes("datatable") && hasDatatable != null) {
            toastr.error("An Element of Type Datatable has already been added.");
        } else {
            if (this.relationData.find(x => x.Order == $('#Order').val())) {
                toastr.error("Cannot enter same Order");
            } else {
                var data = SectionHelper.SaveUpdateFormatting();
                if (data == false) {
                    toastr.error("Please enter valid Element Name and Element Type");
                } else {
                    // add data to grid
                    this.relationData.push(data.relation);
                    //add data in save listing
                    this.relationArray.push(data.relationToSend);
                    //update datagrid with new data 
                    DatatableService.updateDataGrid("relation-Element-Table", this.relationData);

                    if (inputText.toLowerCase().includes("datatable")) {
                        this.relationArrayDT = [];
                        this.relationDataDT = [];
                        DatatableService.updateDataGrid("relation-Datatable-Element-Table", this.relationDataDT);
                        $("#scrollspy-datatable ,#scroll-spy-datatable").removeClass("d-none")
                    } else {
                        $("#scrollspy-datatable,#scroll-spy-datatable").addClass("");
                    }
                }
            }
        }
    }

    //Delete relation
    deleteRelation(meta) {
        //remove data in both data in grid and data to send
        this.relationData.splice(meta, 1);
        this.relationArray.splice(meta, 1);
        //update grid with new data
        DatatableService.updateDataGrid("relation-Element-Table", this.relationData);
        if (this.relationData.find(x => x.ElementType == "Datatable") == null) {
            $("#scrollspy-datatable ,#scroll-spy-datatable").addClass("d-none")

        } else {
            $("#scrollspy-datatable ,#scroll-spy-datatable").removeClass("d-none")
        }

    }

    //Add Relation to table
    addRelationDT() {
        //data formating 
        var data = SectionHelper.SaveUpdateFormattingDT(this.relationDataDT.length + 1, this.Id);
        //check if all mandatory fields has been saved
        if (data == false) {
            toastr.error("Please enter valid Element Name and Element Type");
        } else {
            // add data to grid
            this.relationDataDT.push(data.relation);
            //add data in save listing
            this.relationArrayDT.push(data.relationToSend);
            //update datagrid with new data 
            DatatableService.updateDataGrid("relation-Datatable-Element-Table", this.relationDataDT);
        }
    }

    //Delete relation
    deleteRelationDT(meta) {
        //remove data in both data in grid and data to send
        this.relationDataDT.splice(meta, 1);
        this.relationArrayDT.splice(meta, 1);
        //update grid with new data
        DatatableService.updateDataGrid("relation-Datatable-Element-Table", this.relationDataDT);
    }

    setDataTable() {
        this.relationArray.forEach(x => {
            if (x.InputTypeDetails != undefined && x.InputTypeDetails.Name == "Datatable") {
                x.InnerElemList = this.relationArrayDT;
            } else if (x.inputTypeDetails != undefined && x.inputTypeDetails.name == "Datatable") {
                x.InnerElemList = this.relationArrayDT;
            }
        });
    }

    //Check for validity
    valid() {
        //check all valid field by id and insert invalid feedback
        var valid = Validation.formValidation(["Name"])
        if (!valid) {
            toastr.error("Please fill in required field(s)");
        }

        // check if table contains data
        if (this.relationArray.length == 0) {
            // show error message for trial datatable 
            toastr.error("Element Table should contain atleast one row.");
            valid = false;
        }

        //return boolean id valid or not
        return valid
    }

    //Button save ot update 
    save() {
        this.setDataTable();
        // get all values
        let data = {
            Sec: DefaultService.result(document.getElementById("scrollspy-details")),
            ElemList: this.relationArray
        };
        //check if required fields are input
        if (this.valid() && data != false) {
            data.Sec.Id = this.Id;
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.addEditSection, data, (response) => {
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

    //back to listing
    back() {
        window.location.href = "/Section/List#";
    }
}
//init new Edit Class
window.SectionAddEdit = new SectionAddEdit(); 