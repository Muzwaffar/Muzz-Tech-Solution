import { ApiService, ApiType } from "../ApiService.js";
import { DefaultService } from "../DefaultService.js";
import { LocalStorageValues } from "../../Common/Enums.js";
import { Variable } from "../Variable.js";
import { Loader } from "/Scripts/Common/Loader/Loader.js";

export class Listeners {//handles application listeners
    constructor() {
        $(document).ready(() => {
            this._initApplicationListener();
        });
    }

    // Listen for tab change
    _initApplicationListener() {
        $('button[data-bs-toggle="pill"]').on('shown.bs.tab', function (e) {
            $($.fn.dataTable.tables(true)).DataTable().columns.adjust();
        });
    }
}

window.Listeners = new Listeners();

