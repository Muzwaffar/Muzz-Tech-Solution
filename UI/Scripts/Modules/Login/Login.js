import { ApiService, ApiType } from "../../Common/apiService.js";
import { DefaultService } from "../../Common/DefaultService.js";
import { LocalStorageValues } from "../../Common/Enums.js";
import { Variable } from "../../Common/Variable.js";
import { AlertBoxService } from "../../Common/AlertBoxService.js";
import { Loader } from "../../Common/Loader/Loader.js";

export class Login {//class that handles the login screen
    constructor() {
        $(document).ready(() => {
            Loader.hideLoader(); //hide the loader
        });
    }

    togglePasswordVisibility() {//change the type of the input and the icon for fa-eye
        let elem = $("#password-show-hide")[0];
        let input = $("#passwordContainer")[0];
        if (elem.classList.contains("fa-eye")) {//active
            input.type = "text";
        } else {//inactive
            input.type = "password";
        }
        elem.classList.toggle("fa-eye");
        elem.classList.toggle("fa-eye-slash");
    }

    login() {//call api to login
        
        let res = DefaultService.result(document.getElementById("login-panel"));
        if (res) {//call api
            Loader.showLoader();
            ApiService.callCustom(ApiType.POST, new Variable().login, res, (data) => {

                Loader.hideLoader();
                if (data.data) {
                    localStorage.setItem(LocalStorageValues.USERTOKEN, data.data.userToken);
                    localStorage.setItem(LocalStorageValues.ONETIMETOKEN, data.oneTimeToken);
                    localStorage.setItem(LocalStorageValues.USERNAME, data.data.userName);
                    localStorage.setItem(LocalStorageValues.EMAIL, data.data.email);
                    localStorage.setItem(LocalStorageValues.LOGOUT, data.data.email);
                    localStorage.setItem(LocalStorageValues.FULLUSERNAME, data.data.fullName);
                }
                localStorage.setItem("RoleId", data.roleId);

                if (data.data["status"] == "AL") {
                    toastr.warning(data.additionalData);
                }
                else if (data.data["status"] == "AC") {
                    setTimeout(
                        function () {
                            window.location.href = "/Incident/List";
                        }, 500);
                    toastr.success("Login Successful");
                }
                else if (data.data["status"] == "PE") {
                    AlertBoxService.showPasswordExpired();
                }
                else if (data.data["status"] == "CP") {
                    AlertBoxService.showChangePassword();
                }
                else if (data.qryResult == "failed") {
                    toastr.error(data.additionalData);
                }

            }, () => {
                Loader.hideLoader();

            });
        } else {
            toastr.error("Please enter Email/Password");
        }
    }

    forgotPassword() {//call api to forgotPassword

        let res = DefaultService.result(document.getElementById("forgotpassword-panel"));
        if (res) {
            Loader.showLoader();

            ApiService.callCustom(ApiType.POST, new Variable().forgotPassword, res.email, (response) => {
                Loader.hideLoader();

                if (response.data == true) {
                    toastr.success("An email has been sent to you!");
                    setTimeout(
                        function () {
                            window.location.href = "/Login/Login";
                        },500);
                } else {
                    toastr.error(response.additionalData)
                }
            }, () => {
                Loader.hideLoader();
            });
        } else {


        }
    }


    back() {

        window.location.href = "/Login/Login";
    }

    loginInputChange() {//capture keypresses and detect if enter
        let e = window.event;
        if (e.keyCode == 13 && e.target.value.length > 0) {//login if input valid
            this.login();
        }
    }
}
window.Login = new Login();