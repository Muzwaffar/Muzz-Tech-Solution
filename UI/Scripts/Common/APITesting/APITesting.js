import { LocalStorageValues } from "../../Common/Enums.js";
import { Loader } from "../../Common/Loader/Loader.js";


export class APITesting {//handles all api calls
    constructor() {
        $(document).ready(() => {
            this.xhrRequestCustom("POST", "/APITesting/GetTaskDetailMobile", JSON.parse('{"data":{"id":115,"Latitude":-20.1994649,"Longitude":57.4832419},"userToken":"83f72265-f116-4b67-b9ae-a7edb5cf244a","oneTimeToken":"","RoleId":""}'));
        })
    }

    xhrRequestCustom(type, url, data = null, showloading = false) {// make a post request to server using jquery $.ajax
        if (!showloading) {
            Loader.hideLoader();
        }
        var response = { data: false };
        //new httpRequest
        var request = new XMLHttpRequest();
        return new Promise((resolve, reject) => {
            //httpRequest : open a post request , set url, is async is set to true
            request.open(type, url, true);
            //header is sent in json format
            request.setRequestHeader("Content-Type", "application/json");
            //response
            request.onreadystatechange = (e) => {
                if (request.readyState == 4) {
                    if (request.status == 200) {
                        response = JSON.parse(request.responseText);
                        resolve(response);
                    } else {
                        resolve(response);
                    }
                }
            };
            if (data != null) {
                var dataStringified = JSON.stringify(data);
                //send httpRequest
                request.send(dataStringified);
            } else {
                request.send();
            }
        });
        return response
    }

}
window.APITesting = new APITesting();