import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { Validation } from "/Scripts/Common/ValidationService.js";

export class GetNAABYNIC {//class that handles the NAA NIC

    //call NAAResource API
    getNaaByNic(data) {
        var NIC = data[0].value;
        var validation = Validation.checkNIC(NIC);
        if (validation == "Valid") {
            ApiService.callCustom(ApiType.POST, new Variable().getNAAByNic, NIC, (data) => {
                if (data) {
                    if (data.data.id != 0) {
                        var toShow = data.data;
                        $("#FirstName").val(toShow.firstName);
                        $("#MiddleName").val(toShow.middleName);
                        $("#LastName").val(toShow.lastName);
                        $("#DOB").val(toShow.dob.split("T")[0]);
                        $("#TelNo").val(toShow.telNo);
                        $("#MobileNo").val(toShow.mobileNo);
                        $("#Email").val(toShow.email);
                        $("#Address").val(toShow.address);
                        $("#Street").val(toShow.street);
                        $("#Locality").val(toShow.locality);
                        $("#VillageTown").val(toShow.villageTown);
                        $("#select-VillageTown-id").val(toShow.villageTownId);

                        $("#NAACode").val(toShow.code);
                        $("#NAAId").val(toShow.id);
                        $("#select-title-id").val(toShow.title);
                        $("#select-gender-id").val(toShow.gender);
                    }
                    else {
                        $("#NAAId").val(0);
                    }
                }
            })
        }
        else if (validation == "Please enter a valid NIC/Passport" || validation != "NA" && validation != "Valid") {
            toastr.error(validation);
            $("#NICPassport").val("");
        }

        ////set default value - do not call api 
        //if (validation == "NA") {
        //    $("#NAAId").val("00000000-0000-0000-0000-000000000000");
        //}
    }

    //call NAAResource API
    getNaaByMobileNo(data) {
        var MobileNo = data[0].value;
        ApiService.callCustom(ApiType.POST, new Variable().getNAAByMobileNo, MobileNo, (data) => {
            if (data) {
                if (data.data.id != 0) {
                    var toShow = data.data;
                    $("#FirstName").val(toShow.firstName);
                    $("#LastName").val(toShow.lastName);

                    $("#Address").val(toShow.address);
                    $("#NICPassport").val(toShow.identifier);
                    $("#Street").val(toShow.street);
                    $("#Locality").val(toShow.locality);
                    $("#VillageTown").val(toShow.villageTown);
                    $("#select-VillageTown-id").val(toShow.villageTownId);

                    $("#NAACode").val(toShow.code);
                    $("#NAAId").val(toShow.id);
                }
                else {
                    $("#NAAId").val(0);
                }
            }
        })
    }

    //call NAAResource API
    static getCallerDetailsByMobileNo(datum) {
        if (datum != '') {
            ApiService.callCustom(ApiType.POST, new Variable().getCallerByMobile, datum, (data) => {
                if (data.data != null) {
                    var caller = data.data;
                    if (caller.id != 0) {
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
                    }
                }
            })
        }
      
    }
}
window.GetNAABYNIC = new GetNAABYNIC();