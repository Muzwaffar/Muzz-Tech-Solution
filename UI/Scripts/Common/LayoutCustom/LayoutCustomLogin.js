import { ApiService, ApiType } from "../../Common/apiService.js";
import { LocalStorageValues } from "../../Common/Enums.js";
import { Variable } from "../../Common/Variable.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { Loader } from "../../Common/Loader/Loader.js";

export class LayoutCustomLogin {
    constructor() {

        $(function () {
            //Log out
            $(document).idleTimer({
                timeout: 2400000,
                idle: true
            });

            $(document).on("idle.idleTimer", function (event, elem, obj) {
                toastr.info("Session Idle for 40 minutes. Loggin out.");
                $.ajax({
                    url: new Variable().logout,
                    type: 'POST',
                    data: "",
                    success: function (result) {
                        if (result.data == 'success') {
                            window.location.href = new Variable()._baseInternalApi;
                        }
                    }
                });
            });

            $(document).on("active.idleTimer", function (event, elem, obj, triggerevent) {

            });
        });
        $(document).ready(() => {
           
        });
    }

    changePassword() {
        // get all values
        let data = DefaultService.result(document.getElementById("ChangePassword-Panel"));
        //check if required fields are input
        if (data == false || $("#NewPassword").val() == '' || $("#confirmpassword").val() == '') {
            //show error if required fields not input
            toastr.error("Please Fill In Mandatory Fields")
        } else {
            if (data["NewPassword"] != data["ConfirmPassword"]) {
                toastr.error("The password you entered do not match. Please re-enter your password");
                return;
            }
            //call create or update API
            ApiService.callCustom(ApiType.POST, variable.changePassword, data, (response) => {
                //Response true
                if (response) {
                    toastr.success("Password Changed Sucessfully!")
                    setTimeout(
                        function () {
                            window.location.href = "/Login/Login";
                        },
                        500);
                }
                //response false
                else if (response) {
                    toastr.error(response.additionalData);
                }
                //failed
                else {
                    toastr.error("Could not open the record");
                    return false;
                }
            }, () => {
                Loader.hideLoader();
            })
        }
    }

    togglePasswordVisibility(x, inputId) {//change the type of the input and the icon for fa-eye
        let elem = $("#" + x[0].id)[0];
        let input = $("#" + inputId)[0];
        if (elem.classList.contains("fa-eye")) {//active
            input.type = "text";
        } else {//inactive
            input.type = "password";
        }

        elem.classList.toggle("fa-eye");
        elem.classList.toggle("fa-eye-slash");
    }

}

window.LayoutCustomLogin = new LayoutCustomLogin();
