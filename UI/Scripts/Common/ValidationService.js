
import { DefaultService } from "../Common/DefaultService.js";
export class Validation {//handles field validations

    //Email Validation using regex
    email(data) {
        let email = data[0].value;
        if (email.match(/^[a-z|0-9|A-Z]*([_][a-z|0-9|A-Z]+)*([.][a-z|0-9|A-Z]+)*([.][a-z|0-9|A-Z]+)*(([_][a-z|0-9|A-Z]+)*)?@[a-z][a-z|0-9|A-Z]*\.([a-z][a-z|0-9|A-Z]*(\.[a-z][a-z|0-9|A-Z]*)?)$/)) {
            return (true)
        } else {
            toastr.error("Invalid Email Format. Please Re-enter.")
            data[0].value = "";
            return (false)
        }
    }

    //Password validation using regex
    password(data) {
        let password = data[0].value;
        if (password.match(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$!%*?&])[A-Za-z\d@$#!%*?&.]{8,}$/)) {
            return (true)
        } else {
            toastr.error("Invalid Password Format. Please Re-enter.");
            data[0].value = "";
            return (false)
        }
    }

    //Currency Validation using regex
    currency(data) {
        let currency = data[0].value;
        if (currency != "") {
            if (!currency.includes(".")) {
                currency += ".00"
                data[0].value = currency;
            }
            if (currency.match(/^\d+(?:\.\d{0,2})$/)) {
                return (true)
            } else {
                toastr.error("Invalid Currency Value Format. Please Re-enter.");
                data[0].value = "";
                return (false)
            }

        }
       
    }

    static checkNIC(NIC) {
        const regex = /[ `!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/;
        if (regex.test(NIC)) {
            return "Invalid NIC/Password Format";
        }
        else if (NIC.length == 14) {
            return "Valid";
        }
        else if (NIC.length == 6 || NIC.length == 7 || NIC.length == 8 || NIC.length == 9) {
            return "Valid";
        }
        else if (NIC.toLowerCase() == "na") {
            return "NA";
        }
        else {
            return "Please enter a valid NIC/Passport";
        }

    }

    // Check Whether value has been selected in dropdown or not
    // true : if value has been selected
    // false : if value has NOT been selected
    static checkDropdownSelected(data) {
        if (data == "00000000-0000-0000-0000-000000000000") {
            return false;
        } else {
            return true;
        }
    }

    // Check Whether value has been selected in dropdown or not
    // true : if value has been selected
    // false : if value has NOT been selected
    static checkDropdownSelectedID(data) {
        if (data == 0) {
            return false;
        } else {
            return true;
        }
    }

    //check if field is null, empty or undefined
    static isNullOrUndefined(data) {
        if (data == null || data == undefined || data == "") {
            return true
        } else { 
            return false
        }
    }

    //form Validation
    static formValidation(ids = []) {
        var result = true;
        $(".validation-text").remove();
        ids.forEach(id => {
            let value = $("#" + id).val();
            if (value == null || value == undefined || value == "" || value == 0 || value.toString() == '00:00') {
                result = false;
                let span = DefaultService.createElement("span", { class: "span-smaller text-danger validation-text" }, [], { innerHTML: "<i class='fa-solid fa-circle-exclamation me-2'> </i>This Field is required" })
                $('#' + id).parent().append(span);
            } 
           
        })
        return result
    }


    //start date & end date Validation
    static isStartEndDateValid(start, end) {
        var result = true;
        result = $("#" + end).val() >= $("#" + start).val();
        if (!result) {
            let span = DefaultService.createElement("span", { class: "span-smaller text-danger validation-text" }, [], { innerHTML: "<i class='fa-solid fa-circle-exclamation me-2'> </i>Please change date ( End Date Time should be After Start Date Time)" })
            setTimeout(e => {
                $('#' + start).parent().append(span);
                $('#' + end).parent().append(span);
            },25)
            
        }
        
        return result
    }

    //Clear Values
    static clearValues(ids = []) {
        var result = true;
        ids.forEach(id => {
            if (id.includes("select-")) {
                $("#" + id).val(null).trigger('change')
            } else {
                $("#" + id).val("");
            }
            
        })
    }

    //Prevnt entry of future Dates
    static futureDateCheck(id, date) {
        var now = Validation.stringToDate(moment(moment()).format("DD/MM/yyyy"));
        var selectedDate = Validation.stringToDate(moment(date).format("DD/MM/yyyy"));
        var result = true;
        result = now => selectedDate;
        if (!result) {
            let span = DefaultService.createElement("span", { class: "span-smaller text-danger validation-text" }, [], { innerHTML: "<i class='fa-solid fa-circle-exclamation me-2'> </i>Please change date ( Date cannot exceeds today)" })
            setTimeout(e => {
                $('#' + id).parent().append(span);
            }, 25)

        }
        return result
    }

    //Input Type Field - Upload Attachment
    static validateFileType(Id, AttachmentType) {
        var selectedFile = document.getElementById(Id).files[0];
        var allowedTypes = [];
        if (AttachmentType == 1) {
            allowedTypes = ['image/jpeg', 'image/png', 'image/bmp'];

            if (!allowedTypes.includes(selectedFile.type)) {
                toastr.error('Invalid file type. Please upload a valid file format');
                document.getElementById(Id).value = '';
            }
        }
        else if (AttachmentType == 2) {
            allowedTypes = ['application/msword', 'text/csv', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'application/pdf', 'application/rtf', 'text/plain', 'application/zip', 'application/gzip', 'application/vnd.rar', 'application/vnd.ms-excel', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'application/vnd.ms-powerpoint', 'application/vnd.openxmlformats-officedocument.presentationml.presentation'];

            if (!allowedTypes.includes(selectedFile.type)) {
                toastr.error('Invalid file type. Please upload a valid file format');
                document.getElementById(Id).value = '';
            }
        }
        else if(AttachmentType == 3) {
            allowedTypes = ['application/msword', 'text/csv', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'application/pdf', 'application/rtf', 'text/plain', 'application/zip', 'application/gzip', 'application/vnd.rar', 'application/vnd.ms-excel', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'application/vnd.ms-powerpoint', 'application/vnd.openxmlformats-officedocument.presentationml.presentation', 'image/jpeg', 'image/png', 'image/bmp'];

            if (!allowedTypes.includes(selectedFile.type)) {
                toastr.error('Invalid file type. Please upload a valid file format');
                document.getElementById(Id).value = '';
            }
        }
        else if (AttachmentType == 4) {
            allowedTypes = ['application/pdf'];

            if (!allowedTypes.includes(selectedFile.type)) {
                toastr.error('Invalid file type. Please upload a valid file format');
                document.getElementById(Id).value = '';
                return false;
            }
            return true;
        }
    }

    //
    static stringToDate(str) {
        var [dd, mm, yyyy] = str.split('/');
        return new Date(yyyy, mm - 1, dd);
    }

    //Validation to set default val
    static formValidationToDefault(ids = []) {
        var result = true;
        ids.forEach(id => {
            let value = $("#" + id).val();
            let e = $("#" + id);
            if (value == null || value == undefined || value == "" || value == 0) {
                if (e[0].localName == "select") {
                    $('#' + id).val(-1);
                } else {
                    $("#" + id).val("-1");
                }
                result = false
            }
        })
        return result
    }

    //Validation to set default val
    static setDisabledToListOfFields(ids = []) {
        var result = true;
        ids.forEach(id => {

            $("#" + id).prop("disabled", true);
            // $("#" + id).addClass("disabled");
        })
        return result
    }
}
window.Validation = new Validation();