export class DropdownService {//handles all dropdowns (select and multiselect)

    //Select 2 - General
    static initDropdown(SelectId, listVariableName, Title) {
        $('#' + SelectId).select2({
            allowClear: true,
            width: '100%',
            placeholder: "None Selected",
            data: listVariableName.find(y => y.title == Title) == undefined ? []: listVariableName.find(y => y.title == Title).items 
        });
    }
    // init null select 2
    static initDropdownNull(SelectId) {
        $('#' + SelectId).select2({
            allowClear: true,
            width: '100%',
            placeholder: "None Selected"
        });
    }

    //init filtered select 2
    static initDropdownFiltered(SelectId, list) {
        $('#' + SelectId).select2({
            allowClear: true,
            width: '100%',
            placeholder: "None Selected",
            data: list
        });
    }

    // destroy - empty - add new default value and insert data in select 2
    static destroyReinitDropdown(SelectId, listVariableName, Title) {
        //destroy dropdown
        $('#' + SelectId).select2("destroy");
        //empty all values
        $('#' + SelectId).empty();
        //set value to default
        $('#' + SelectId).append(new Option("", "", false, false))
        //init dropdown with values
        $('#' + SelectId).select2({
            allowClear: true,
            width: '100%',
            placeholder: "None Selected",
            data: listVariableName.find(y => y.title == Title) == undefined ? [] : listVariableName.find(y => y.title == Title).items
        });
    }

    //multiselect 
    static initMultiDropdown(SelectId, listVariableName, Title) {
        $('#' + SelectId).select2({
            multiple: true,
            closeOnSelect: false,
            width: '100%',
            placeholder: "None Selected",
            data: listVariableName.find(y => y.title == Title) == undefined ? [] : listVariableName.find(y => y.title == Title).items 
        });
    }

    //multiselect 
    static initMultiDropdownFiltered(SelectId, list) {
        $('#' + SelectId).select2({
            multiple: true,
            closeOnSelect: false,
            width: '100%',
            placeholder: "None Selected",
            data: list
        });
    }

    // init null multiselect
    static initMultiDropdownNull(SelectId) {
        $('#' + SelectId).select2({
            multiple: true,
            closeOnSelect: false,
            allowClear: true,
            width: '100%',
            placeholder: "None Selected"
        });
    }

    //multiselect 
    static initMultiDropdownWithAll(SelectId, listVariableName, Title) {
        $('#' + SelectId).select2({
            multiple: true,
            closeOnSelect: false,
            width: '100%',
            placeholder: "None Selected",
            dropdownAdapter: $.fn.select2.amd.require('select2/selectAllAdapter'),
            data: listVariableName.find(y => y.title == Title) == undefined ? [] : listVariableName.find(y => y.title == Title).items
        });
    }

    // init null multiselect
    static initMultiDropdownNullWithAll(SelectId) {
        $('#' + SelectId).select2({
            multiple: true,
            closeOnSelect: false,
            allowClear: true,
            width: '100%',
            placeholder: "None Selected",
        });
    }

    // destroy - empty - add new default value and insert data in select 2
    static destroyReinitMultiselect(SelectId, listVariableName, Title) {
        //destroy dropdown
        $('#' + SelectId).select2("destroy");
        //empty all values
        $('#' + SelectId).empty();
        //init dropdown with values
        if (listVariableName != null) {
            $('#' + SelectId).select2({
                multiple: true,
                closeOnSelect: false,
                allowClear: true,
                width: '100%',
                placeholder: "None Selected",
                data: listVariableName.find(y => y.title == Title) == undefined ? [] : listVariableName.find(y => y.title == Title).items
            });
        } else {
            $('#' + SelectId).select2({
                multiple: true,
                closeOnSelect: false,
                allowClear: true,
                width: '100%',
                placeholder: "None Selected"
            });
        }
        
    }

    // destroy - empty - add new default value and insert data in select 2
    static destroyReinitDropdownNull(SelectId, listVariableName = null, Title = null) {
        //destroy dropdown
        $('#' + SelectId).select2("destroy");
        //empty all values
        $('#' + SelectId).empty();
        //set value to default
        $('#' + SelectId).append(new Option("", "", false, false))
        //init dropdown with values
        if (listVariableName != null) {
            $('#' + SelectId).select2({
                allowClear: true,
                width: '100%',
                placeholder: "None Selected",
                data: listVariableName.find(y => y.title == Title) == undefined ? [] : listVariableName.find(y => y.title == Title).items
            });
        } else {
            $('#' + SelectId).select2({
                allowClear: true,
                width: '100%',
                placeholder: "None Selected"
            });
        }
    }

    // destroy - empty - add new default value and insert data in select 2
    static destroyReinitDropdownFiltered(SelectId, list) {
        //destroy dropdown
        $('#' + SelectId).select2("destroy");
        //empty all values
        $('#' + SelectId).empty();
        //set value to default
        $('#' + SelectId).append(new Option("", "", false, false))
        //init dropdown with values
        $('#' + SelectId).select2({
            allowClear: true,
            width: '100%',
            placeholder: "None Selected",
            data: list
        });
    }


    //Select 2 - General - with custom Search 
    static initDropdownDynamic(SelectId, URL, Title, param = null, isPreset = false) {
        if (!isPreset) {
            $('#' + SelectId).select2({
                minimumInputLength: 2,
                minimumResultsForSearch: 10,
                allowClear: true,
                width: '100%',
                placeholder: "None Selected",
                ajax: {
                    url: URL,
                    dataType: "json",
                    type: "POST",
                    data: function (params) {
                        var parameter = $("#" + param).val();
                        var queryParameters = {
                            term: params.term,
                            title: Title,
                            param: parameter
                        }

                        return queryParameters;
                    },
                    processResults: function (data) {
                        return {
                            results: data.data[0].items
                        };
                    }
                }

            });
        } else {
            $('#' + SelectId).select2({
                allowClear: true,
                width: '100%',
                placeholder: "None Selected",
                ajax: {
                    url: URL,
                    dataType: "json",
                    type: "POST",
                    data: function (params) {
                        var parameter = $("#" + param).val();
                        var queryParameters = {
                            term: params.term,
                            title: Title,
                            param: parameter
                        }

                        return queryParameters;
                    },
                    processResults: function (data) {
                        return {
                            results: data.data[0].items
                        };
                    }
                },
                escapeMarkup: function (m) { return m; }, // Let our custom formatter work

            });
        }
    }
    static search_value(SelectId, text, id) {
        // create the option and append to Select2
        var option = new Option(text, id, true, true);
        var Select = $('#' + SelectId);
        Select.append(option).trigger('change');
        // manually trigger the `select2:select` event
        Select.trigger({
            type: 'select2:select',
            params: {
                data: {
                    "id": id,
                    "text": text
                }
            }
        });
    }
}
window.DropdownService = new DropdownService();