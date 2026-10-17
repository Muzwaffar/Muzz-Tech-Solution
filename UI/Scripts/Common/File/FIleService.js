import { DatatableService } from "/Scripts/Common/DatatableService.js";
export class FileService {//handles all file requests

    static addFileForUpload(FieldId, KeywordId, TableIndex) {
        const fileInput = document.querySelector('#' + FieldId);
        let dat = {
            id: 0,
            tableId: TableIndex,
            file: fileInput.files[0],
            fileName: fileInput.files[0].name,
            keywords: $("#" + KeywordId).val()
        }
        $("#" + KeywordId).val('');
        $("#" + FieldId).val(null);
        return dat;
    }

    static addSingleFileForUpload(FieldId) {
        const fileInput = document.querySelector('#' + FieldId);
        let dat = {
            file: fileInput.files[0],
            fileName: fileInput.files[0].name,
        };
        return dat;
    }

    static uploadFile(data, ParentModule, Module, SubModule, RecordId) {
        var success = true;
        const formData = new FormData();
        formData.append('data.RecordId', RecordId);
        formData.append('data.ParentModule', ParentModule);
        formData.append('data.Module', Module);
        formData.append('data.SubModule', SubModule);

        for (var [i, datum] of data.entries()) {
            formData.append('data.FilesList[' + i + '].Id', datum.id);
            formData.append('data.FilesList[' + i + '].TableId', datum.tableId);
            formData.append('data.FilesList[' + i + '].Keywords', datum.keywords);
            formData.append('data.FilesList[' + i + '].File', datum.file);
        }
        let requestOptions = {
            method: "POST",
            redirect: 'follow',
            body: formData
        };

        const url = variable.uploadFiles;

        try {
            fetch(url, requestOptions).then(response => response.json())
            return true;

        } catch (error) {
            success = false;
        }
        return success; 
    }

    static uploadSingleFile(data) {
        var success = true;
        const formData = new FormData();
        formData.append("data.file", data.file);
        let requestOptions = {
            method: "POST",
            redirect: 'follow',
            body: formData
        };

        const url = variable.uploadSingleFile;

        try {
            fetch(url, requestOptions).then(response => response.json())
            return true;

        } catch (error) {
            success = false;
        }
        return success;
    }

    static initialiseFileTable(TableId, deleteFuction = null, deleteAction = true, isBase64 =false) {
        //Datatble column for Upload
        var FileColumnList = [
            { data: 'id', title: 'Id', visible: false },
            { data: 'fileName', title: 'File Name' },
            { data: 'keywords', title: 'Keywords' },

        ];
        // Init File Table 
        // Parameters: Table Id, list of columns, data, has delete action? , delete action function
        DatatableService.initializeDataGridFile(TableId, FileColumnList, [], true, deleteAction, deleteFuction, isBase64)
    }
}