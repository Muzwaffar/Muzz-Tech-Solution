import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class AlertBoxService {//handles delete from list 

    static deleteAlertBox(apiVariable, apiData, name) {
        let text = "Are you sure you want to delete this record?"
        if (name != null) {
            text = 'Are you sure you want to delete this ' + name + '?'
        }
        Swal.fire({
            title: text,
            text: "You won't be able to revert this action!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#2870ea',
            cancelButtonColor: '#FF0046',
            confirmButtonText: 'Yes',
            cancelButtonText: "No",
            customClass: {
                confirmButton: 'form-control btn-c-info',
                cancelButton: 'form-control btn-c-error'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                Loader.showLoader();
                //call API to delete record
                ApiService.callCustom(ApiType.POST, apiVariable, apiData, (data) => {
                    if (data.data) {
                        Swal.fire({
                            title: 'Deleted!',
                            text: name + " has been deleted.",
                            icon: 'success',
                            confirmButtonColor: '#2870ea',
                            confirmButtonText: 'Ok',
                            customClass: {
                                confirmButton: 'form-control btn-c-info'
                            }
                        }).then((result) => {
                            Loader.hideLoader();
                            location.reload();
                        })
                    }
                    //response false
                    else if (!data.data) {
                        Swal.fire({
                            title: 'Error',
                            text: data.additionalData,
                            icon: 'error',
                            confirmButtonColor: '#2870ea',
                            confirmButtonText: 'Ok',
                            customClass: {
                                confirmButton: 'form-control btn-c-info'
                            }
                        }).then((result) => {
                            Loader.hideLoader();
                            location.reload();
                        })
                    }
                    //failed
                    else {
                        toastr.error("Action Failed. Please contact Administrator");
                        return false;
                    }

                })
            }
        })
    }

    static deleteMasterAlertBox(apiVariable, apiData, name) {
        let text = "Record may be currently in use and changes to it may affect other processes. Are you sure you wants to proceed?"

        Swal.fire({
            title: text,
            text: "You won't be able to revert this action!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#2870ea',
            cancelButtonColor: '#FF0046',
            confirmButtonText: 'Yes',
            cancelButtonText: "No",
            customClass: {
                confirmButton: 'form-control btn-c-info',
                cancelButton: 'form-control btn-c-error'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                AlertBoxService.deleteAlertBox(apiVariable, apiData, name);
            }
        })
    }
    static saveBackAlertBox(url) {
        Swal.fire({
            title: "Go Back Without Saving?",
            text: "Are you sure you want to return to listing without saving?",
            icon: 'warning',
            showCancelButton: true,
            allowOutsideClick: false,
            confirmButtonColor: '#2870ea',
            cancelButtonColor: '#FF0046',
            confirmButtonText: 'Back to list',
            cancelButtonText: "Save as draft",
            customClass: {
                confirmButton: 'form-control btn-c-info',
                cancelButton: 'form-control btn-c-error'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                location.href = url;
            }
            else {
                $("#value-draft-btn").click();
            }
        })
    }
    static resetPassword(url, id, name) {
        Swal.fire({
            title: "Reset Password To Default.",
            text: "You won't be able to revert this action!",
            icon: 'info',
            confirmButtonColor: '#2870ea',
            confirmButtonText: '<a id="link" href="#partialPasswordLogin" role="button">OK</a>',
            customClass: {
                confirmButton: 'form-control btn-c-info nomargin'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                ApiService.callCustom(ApiType.POST, url, id, (data) => {
                    if (data) {
                        toastr.success("Password Reset Successful");
                    }
                })
            }
        })
    }

    static showChangePassword() {
        Swal.fire({
            title: "Login Successful.",
            text: "Please change your password",
            icon: 'info',
            confirmButtonColor: '#2870ea',
            confirmButtonText: '<a id="link" href="#partialPasswordLogin" role="button">OK</a>',
            customClass: {
                confirmButton: 'form-control btn-c-info nomargin'
            }
        }).then((result) => {
            if (result.isConfirmed) {

                $(".contentPassword").html("")

                ApiService.callCustom(ApiType.Get, new Variable().changePasswordPartialView, "", (data) => {
                    if (data) {
                        //set partial view content
                        $(".contentPassword").html(data)
                    }
                })
            }
        })
    }

    static showPasswordExpired() {
        Swal.fire({
            title: "Password Expired.",
            text: "Please change your password",
            icon: 'warning',
            confirmButtonColor: '#2870ea',
            confirmButtonText: '<a id="link" href="#partialPasswordLogin" role="button">OK</a>',
            customClass: {
                confirmButton: 'form-control btn-c-info nomargin'
            }
        }).then((result) => {
            if (result.isConfirmed) {

                $(".contentPassword").html("")

                ApiService.callCustom(ApiType.Get, new Variable().changePasswordPartialView, "", (data) => {
                    if (data) {
                        //set partial view content
                        $(".contentPassword").html(data)
                    }
                })
            }
        })
    }

    static declineStockRequestAlertBox(apiVariable, apiData, name) {
        let text = "Are you sure you want to decline this record?"
        if (name != null) {
            text = 'Are you sure you want to decline this ' + name + '?'
        }
        Swal.fire({
            title: text,
            text: "You won't be able to revert this action!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#2870ea',
            cancelButtonColor: '#FF0046',
            confirmButtonText: 'Yes',
            cancelButtonText: "No",
            customClass: {
                confirmButton: 'form-control btn-c-info',
                cancelButton: 'form-control btn-c-error'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                Loader.showLoader();
                //call API to delete record
                ApiService.callCustom(ApiType.POST, apiVariable, apiData, (data) => {
                    if (data.data) {
                        Swal.fire({
                            title: 'Decline!',
                            text: name + " has been decline.",
                            icon: 'success',
                            confirmButtonColor: '#2870ea',
                            confirmButtonText: 'Ok',
                            customClass: {
                                confirmButton: 'form-control btn-c-info'
                            }
                        }).then((result) => {
                            Loader.hideLoader();
                            location.reload();
                        })
                    }
                    //response false
                    else if (!data.data) {
                        Swal.fire({
                            title: 'Error',
                            text: data.additionalData,
                            icon: 'error',
                            confirmButtonColor: '#2870ea',
                            confirmButtonText: 'Ok',
                            customClass: {
                                confirmButton: 'form-control btn-c-info'
                            }
                        }).then((result) => {
                            Loader.hideLoader();
                            location.reload();
                        })
                    }
                    //failed
                    else {
                        toastr.error("Action Failed. Please contact Administrator");
                        return false;
                    }

                })
            }
        })
    }

    static approveStockRequestAlertBox(apiVariable, apiData, name) {
        let text = "Are you sure you want to aprrove this record?"
        if (name != null) {
            text = 'Are you sure you want to aprrove this ' + name + '?'
        }
        Swal.fire({
            title: text,
            text: "You won't be able to revert this action!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#2870ea',
            cancelButtonColor: '#FF0046',
            confirmButtonText: 'Yes',
            cancelButtonText: "No",
            customClass: {
                confirmButton: 'form-control btn-c-info',
                cancelButton: 'form-control btn-c-error'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                Loader.showLoader();
                //call API to delete record
                ApiService.callCustom(ApiType.POST, apiVariable, apiData, (data) => {
                    if (data.data) {
                        Swal.fire({
                            title: 'Approved!',
                            text: name + " has been aprroved.",
                            icon: 'success',
                            confirmButtonColor: '#2870ea',
                            confirmButtonText: 'Ok',
                            customClass: {
                                confirmButton: 'form-control btn-c-info'
                            }
                        }).then((result) => {
                            Loader.hideLoader();
                            location.reload();
                        })
                    }
                    //response false
                    else if (!data.data) {
                        Swal.fire({
                            title: 'Error',
                            text: data.additionalData,
                            icon: 'error',
                            confirmButtonColor: '#2870ea',
                            confirmButtonText: 'Ok',
                            customClass: {
                                confirmButton: 'form-control btn-c-info'
                            }
                        }).then((result) => {
                            Loader.hideLoader();
                            location.reload();
                        })
                    }
                    //failed
                    else {
                        toastr.error("Action Failed. Please contact Administrator");
                        return false;
                    }

                })
            }
        })
    }

    static alertBoxCustom(message, type) {
        Swal.fire({
            text: message,
            icon: type,
            showCancelButton: false,
            confirmButtonColor: '#2870ea',
            confirmButtonText: 'OK',
            customClass: {
                confirmButton: 'form-control btn-c-info',
            }
        }).then((result) => {
            Loader.hideLoader();
            location.reload();
        })
    }

    static alertBoxCustomWithAPI(title, message, url, data, reload = false) {
        Swal.fire({
            title: title,
            text: message,
            icon: 'info',
            showCancelButton: true,
            confirmButtonColor: '#2870ea',
            cancelButtonColor: '#FF0046',
            confirmButtonText: 'Yes',
            cancelButtonText: "No",
            customClass: {
                confirmButton: 'form-control btn-c-info',
                cancelButton: 'form-control btn-c-error'
            }
        }).then((result) => {
            if (result.isConfirmed) {

                ApiService.callCustom(ApiType.POST, url, data, (response) => {
                    if (response.data) {
                        if (reload) {

                            location.reload();
                        }
                        Loader.hideLoader();
                    } else {
                        toastr.error("An Error Occurred. Please contact Admin")
                    }
                })
            }
        })
    }

    static alertBoxWithMessageBox(title, text,inputType, confirmBtnText, url, backURL, data, icon) {
        Swal.fire({
            title: title,
            text: text,
            input: inputType,
            inputAttributes: {
                autocapitalize: "off"
            },
            icon: "info",
            showCancelButton: true,
            confirmButtonColor: '#322eaf',
            cancelButtonColor: '#FF0046',
            confirmButtonText: confirmBtnText,
            cancelButtonText: "Cancel",
            customClass: {
                confirmButton: 'form-control btn-c-info',
                cancelButton: 'form-control btn-c-error'
            },
            showLoaderOnConfirm: true,
            preConfirm: async (message) => {
                if (message == "") {
                    toastr.error("Please input a reason.");
                    return false;
                } else {
                    data["Reason"] = message;
                    ApiService.callCustom(ApiType.POST, url, data, (response) => {
                        if (response.data) {
                            Swal.fire({
                                text: "Request Sent Successfully",
                                icon: "success",
                                showCancelButton: false,
                                confirmButtonColor: '#2870ea',
                                confirmButtonText: 'OK',
                                customClass: {
                                    confirmButton: 'form-control btn-c-info',
                                }
                            }).then((result) => {
                                location.href = backURL;
                            })
                        } else {

                            toastr.error("An Error Occurred. Please contact Admin")
                        }
                    })
                }

            },
            allowOutsideClick: () => !Swal.isLoading()
        })
    }

}