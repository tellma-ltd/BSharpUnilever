"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var http_1 = require("@angular/common/http");
function friendly(error) {
    // Translates HttpClient's errors into human-friendly errors
    var result = '';
    if (error instanceof http_1.HttpErrorResponse) {
        var response = error;
        switch (response.status) {
            case 0: { // Offline
                result = "Unable to reach the server, please check the connection of your device";
                break;
            }
            case 400: { // Bad Request
                if (error.error instanceof Blob) {
                    // Need a better solution to handle blobs
                    result = 'Unknown error';
                }
                else {
                    result = error.error;
                }
                break;
            }
            case 401: { // Unauthorized
                result = "Your login session has expired, please login again";
                break;
            }
            case 403: { // Forbidden
                result = "Sorry, your account does not have sufficient permissions";
                break;
            }
            case 404: { // Not found
                result = "Sorry, the requested resource was not found";
                break;
            }
            case 500: { // Internal Server Error
                result = "An unhandled exception occurred on the server, please contact your IT department";
                break;
            }
            default: { // Any other HTTP error
                result = "An unknown error has occurred while retrieving the record, please contact your IT department";
                break;
            }
        }
    }
    else {
        console.error(error);
        result = "Unknown error";
    }
    return result;
}
exports.friendly = friendly;
function cloneModel(model) {
    // This technique is simple and effective for cloning model objects, these objects
    // have to be JSON friendly anyways since they travel from/to the server
    return JSON.parse(JSON.stringify(model));
}
exports.cloneModel = cloneModel;
function downloadBlob(blob, fileName) {
    // Helper function to download a blob from memory to the user's computer,
    // Without having to open a new window first
    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        // To support IE and Edge
        window.navigator.msSaveOrOpenBlob(blob, fileName);
    }
    else {
        // Create an in memory url for the blob, further reading:
        // https://developer.mozilla.org/en-US/docs/Web/API/URL/createObjectURL
        var url = window.URL.createObjectURL(blob);
        // Below is a trick for downloading files without opening
        // a new window. This is a more elegant user experience
        var a = document.createElement('a');
        document.body.appendChild(a);
        a.setAttribute('style', 'display: none');
        a.href = url;
        a.download = fileName;
        a.click();
        a.remove();
        // Best practice to prevent a memory leak, especially in a SPA like bSharp
        window.URL.revokeObjectURL(url);
    }
}
exports.downloadBlob = downloadBlob;
//# sourceMappingURL=util.js.map