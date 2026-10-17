import { LocalStorageValues } from "../Common/Enums.js";
import { Loader } from "../Common/Loader/Loader.js";

export class ApiType{
    static POST = "POST";
    static GET = "GET";
    static PUT = "PUT";
    static DELETE = "DELETE";
}

export class ApiService {//handles all api calls

    static callCustom(type, url, data, success = null, showLoader = true, LoaderText = "Loading...") {// make a post request to server using jquery $.ajax
        Loader.showLoader(LoaderText);
        $.ajax({
            type,
            contentType: "application/json",
            url,
            data: this._parse(data),
            success: function (response) {
                success(response);
                Loader.hideLoader();
                return;
            }, error: function () {
                Loader.hideLoader();
                return;
            }
        });
    }


    static call(type, url, data, success = null, error = null, raw = false, showLoader = false) {// make a post request to server using jquery $.ajax
        $.ajax({
            type,
            contentType: "application/json",
            url,
            data: this._addAuth(data),
            success: function (response) {
                try {
                   response.data['oneTimeToken'] = response.oneTimeToken;//make it accessible to callback
                   
                } catch (e) { console.log(e); }
                
                if (raw) {
                    success(response);
                    return;
                }

                if (ApiService._parseResponse(response)) {
                    if (typeof success == "function") success(response.data, response.msg);
                } else {
                    if (typeof error == "function") error();
                }
            }, error: function () {
                if (raw) {
                    success(response);
                    return;
                }
            }
        });
    }

    static _addAuth(data) {//add usertoken and onetimetoken to query
        if (data == null) {//no data has been provided
            data = {}
        }

        var  resp = { data };

        resp[LocalStorageValues.USERTOKEN] = localStorage.getItem(LocalStorageValues.USERTOKEN);
        resp[LocalStorageValues.ONETIMETOKEN] = localStorage.getItem(LocalStorageValues.ONETIMETOKEN);
        return JSON.stringify(resp);
    }

    static _parseResponse(response) {// add usertoken and onetimetoken to query
        if (response.qryResult == "success") {
            return true;
        } else if (response.qryResult == "failed") {
            return false;
        }
    }

    static _parse(data) {//add usertoken and onetimetoken to query
        if (data == null) {//no data has been provided
            data = {}
        }
        var resp = { data };
        resp[LocalStorageValues.USERTOKEN] = localStorage.getItem(LocalStorageValues.USERTOKEN);
        resp[LocalStorageValues.ONETIMETOKEN] = localStorage.getItem(LocalStorageValues.ONETIMETOKEN);
        resp[LocalStorageValues.MODULEID] = sessionStorage.getItem(LocalStorageValues.MODULEID);
        return JSON.stringify(resp);
    }

    

}