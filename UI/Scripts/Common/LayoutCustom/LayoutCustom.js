import { ApiService, ApiType } from "../../Common/apiService.js";
import { LocalStorageValues } from "../../Common/Enums.js";
import { Variable } from "../../Common/Variable.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";
import { Loader } from "../../Common/Loader/Loader.js";

export class Layout {
    ListIds = [];
    constructor() {
        $(function () {
            //Log out
            $(document).idleTimer({
                timeout: 2400000,
                idle: true
            });
            //set idle timer 
            $(document).on("idle.idleTimer", function (event, elem, obj) {
                toastr.info("Session Idle for 40 minutes. Logging out.");
                ApiService.callCustom(ApiType.POST, new Variable().logout, "", (result) => {
                    if (result.data == 'success') {
                        window.location.href = new Variable()._baseInternalApi;
                    }
                })
            });
            //timer activated
            $(document).on("active.idleTimer", function (event, elem, obj, triggerevent) {

            });
        });
        $(document).ready(() => {
            //Hide Popup when reloading
            if (!window.location.href.includes("IncidentHandling/AddEdit")) {
                window.location.href = "#";
            }
           
            // Hide the settings container
            $("#settingspopup").hide();

            // Hide the settings container when click aside
            $(document).mouseup(function (e) {
                var container = $("#settingspopup");

                // if the target of the click isn't the container nor a descendant of the container
                if (!container.is(e.target) && container.has(e.target).length === 0) {
                    container.hide();
                }

            })

            // Show the settings container upon icon-click
            $("#settings").click(function () {
                if ($("#settingspopup").is(":hidden")) {
                    $("#settingspopup").show();
                }
            });

            // API Call for signout
            $("#signout").click(function () {
                ApiService.callCustom(ApiType.POST, new Variable().logout, "", (result) => {
                    if (result.data == 'success') {
                        window.location.href = new Variable()._baseInternalApi;
                    }
                })
            });

            //get username to set in user placeholder
            $("#username-main").html(localStorage.getItem(LocalStorageValues.USERNAME));

            //open change password partial view
            $("#changePassword").click(function () {
                $(".contentPassword").html("")

                ApiService.callCustom(ApiType.Get, new Variable().changePasswordPartialViewLayout, "", (data) => {
                    if (data) {
                        //set partial view content
                        $(".contentPassword").html(data)
                    }
                })
            });
            //Get Notification list
            $("#event-widget,#today-tab").click((e) => {
                this.eventCenter(true);
            });

            //Get Notification list - last 7 days
            $("#last-7-days-tab").click((e) => {
                this.eventCenter(false);
            });

            setInterval(this.heartBeat, 5000); // call heartBeat every 5 seconds 

        });
    }

    heartBeat() {
        // code to execute every 10 seconds

        //new httpRequest
        var request = new XMLHttpRequest();
        //httpRequest : open a post request , set url, is async is set to true
        request.open("POST", variable.heartBeat, true);
        //header is sent in json format
        request.setRequestHeader("Content-Type", "application/json");
        //response
        request.onreadystatechange = function () {
            if (request.readyState == 4) {
                if (request.status == 200) {
                    var data = JSON.parse(request.responseText);
                    if (data.data.showBadge) {
                        $("#event-badge").removeClass("d-none");
                    }
                    else {
                        $("#event-badge").addClass("d-none");
                    }

                    if (data.data.notify) {
                        var notiffSound = $("#notif-sound-container")[0];
                        notiffSound.play();
                    }

                    if (data.data.popEvents.length > 0) {
                        let popupAlertContainer = $("#popup-alert-container");

                        for (let alert of data.data.popEvents) {
                            let event = DefaultService.createElement("div", { class: "toast popup-toast popup-toast-red fade", id: "popup-" + alert.id,role: "alert", 'aria-live': "assertive", 'aria-atomic': "true", 'data-bs-delay': "15000", 'data-bs-autohide':"true"}, [
                                DefaultService.createElement("div", { class: "toast-header naveo-color-red" }, [
                                        DefaultService.createElement("i", { class: "fa-solid fa-bell fa-2x me-2" }, [], {}),
                                        DefaultService.createElement("strong", { class: "mx-2" }, [], { innerHTML: alert.title }),
                                        DefaultService.createElement("button", { class: "btn-close right-12" ,'data-bs-dismiss':"toast", 'aria-label':"Close"}, [], {})
                                ]),
                                DefaultService.createElement("div", { class: "toast-body" }, [
                                    DefaultService.createElement("span", { class: "" }, [], { innerHTML: alert.detail }),
                                    DefaultService.createElement("button", { class: "form-control btn-c-secondary mt-3 w-100", onclick: "Layout.redirect('" + alert.target + "')" }, [
                                        DefaultService.createElement("i", { class: "fa fa-eye me-2" }, [], {  }),
                                    ], {  innerHTML: "View" })
                                ]),
                            ]);
                            popupAlertContainer.append(
                                    event
                            );
                            $('#popup-' + alert.id).toast("show");

                        }

                    }
                }
            }
        };
        //set data to send
        var userToken = localStorage.getItem("userToken");
        var data = JSON.stringify({ "data": userToken });
        //send httpRequest
        request.send(data);
    }

    //redirect
    redirect(url) {
        window.location.href = url;
    }

    //Open event center
    eventCenter(today) {
        let data = {
            From: moment().startOf('day').toISOString(),
            To: moment().endOf('day').toISOString()
        };
        if (!today) {
            data.From = moment().subtract(7, 'd').startOf('day').toISOString();
            data.To = moment().endOf('day').toISOString();
        }
        $("#event-badge").addClass("d-none");
        ApiService.callCustom(ApiType.POST, new Variable().getNotificationList, data, (data) => {
            if (data) {

                if (today) {
                    let menu_unread = $("#list-events-unread");
                    menu_unread[0].innerHTML = "";

                    let menu_read = $("#list-events-read");
                    menu_read[0].innerHTML = "";

                    if (data.data.length > 0) {
                        for (let notification of data.data) {
                            let event = DefaultService.createElement("li", { class: "list-group-item" }, [

                                DefaultService.createElement("div", { class: "not-title" }, [
                                    DefaultService.createElement("span", { class: "dot" }, [], { innerHTML: "" }),
                                    DefaultService.createElement("span", { class: "not-title-text f-weight-bold" }, [], { innerHTML: notification.detail })
                                ]),
                                DefaultService.createElement("span", { class: "not-de scription" }, [], { innerHTML: notification.title }),
                                DefaultService.createElement("div", { class: "not-actions" }, [
                                    DefaultService.createElement("div", { class: "not-action-left" }, [
                                        DefaultService.createElement("button", { class: notification.target == "" ? "btn-s-info d-none" : "btn-s-info", onclick: "location.href='" + notification.target + "'" }, [], { innerHTML: "View" }),
                                    ]),
                                    DefaultService.createElement("div", { class: "not-action-right" }, []),
                                ]),
                                DefaultService.createElement("div", { class: "not-footer" }, [
                                    DefaultService.createElement("span", { class: "span-smaller" }, [], { innerHTML: moment(notification.actionDate).format("DD/MM/yyyy") + " " + moment(notification.actionDate).format("HH:mm") }),
                                    DefaultService.createElement("span", { class: "dot-small span-smaller" }, [], {}),
                                    DefaultService.createElement("span", { class: "span-smaller" }, [], { innerHTML: notification.addDetail }),
                                ]),
                            ]);

                            if (notification.isRead) {
                                menu_read.append(
                                    event
                                );
                            } else {
                                menu_unread.append(
                                    event
                                );
                            }

                        }

                        let eventNoNotification = DefaultService.createElement("span", { class: "d-flex justify-content-center mb-3" }, [], { innerHTML: "No Notification" });
                        if (data.data.filter(x => x.isRead == false).length == 0) {
                            menu_unread.append(
                                eventNoNotification
                            );
                        }
                        if (data.data.filter(x => x.isRead == true).length == 0) {
                            menu_read.append(
                                eventNoNotification
                            );
                        }
                    }
                } else {
                    let menu_last = $("#list-events-last-7-days");
                    menu_last[0].innerHTML = "";

                    if (data.data.length > 0) {
                        for (let notification of data.data) {
                            let event = DefaultService.createElement("li", { class: "list-group-item" }, [

                                DefaultService.createElement("div", { class: "not-title" }, [
                                    DefaultService.createElement("span", { class: "dot" }, [], { innerHTML: "" }),
                                    DefaultService.createElement("span", { class: "not-title-text f-weight-bold" }, [], { innerHTML: notification.detail })
                                ]),
                                DefaultService.createElement("span", { class: "not-de scription" }, [], { innerHTML: notification.title }),
                                DefaultService.createElement("div", { class: "not-actions" }, [
                                    DefaultService.createElement("div", { class: "not-action-left" }, [
                                        DefaultService.createElement("button", { class: notification.target == "" ? "btn-s-info d-none" : "btn-s-info", onclick: "location.href='" + notification.target + "'" }, [], { innerHTML: "View" }),
                                    ]),
                                    DefaultService.createElement("div", { class: "not-action-right" }, []),
                                ]),
                                DefaultService.createElement("div", { class: "not-footer" }, [
                                    DefaultService.createElement("span", { class: "span-smaller" }, [], { innerHTML: moment(notification.actionDate).format("DD/MM/yyyy") + " " + moment(notification.actionDate).format("HH:mm") }),
                                    DefaultService.createElement("span", { class: "dot-small span-smaller" }, [], {}),
                                    DefaultService.createElement("span", { class: "span-smaller" }, [], { innerHTML: notification.addDetail }),
                                ]),
                            ]);

                            menu_last.append(
                                event
                            );

                        }
                    } else {
                        let eventNoNotification = DefaultService.createElement("span", { class: "d-flex justify-content-center mb-3" }, [], { innerHTML: "No Notification" });
                        menu_last.append(
                            eventNoNotification
                        );
                    }
                }
                


            }
        })
    }

    //change passowrd
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

    //eye icon when toggling password visibility
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

window.Layout = new Layout();
