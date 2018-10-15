"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var SupportRequest = /** @class */ (function () {
    function SupportRequest() {
    }
    return SupportRequest;
}());
exports.SupportRequest = SupportRequest;
var SupportRequestState;
(function (SupportRequestState) {
    SupportRequestState["Draft"] = "Draft";
    SupportRequestState["Submitted"] = "Submitted";
    SupportRequestState["Approved"] = "Approved";
    SupportRequestState["Posted"] = "Posted";
    SupportRequestState["Canceled"] = "Canceled";
    SupportRequestState["Rejected"] = "Rejected";
})(SupportRequestState = exports.SupportRequestState || (exports.SupportRequestState = {}));
var SupportRequestReason;
(function (SupportRequestReason) {
    SupportRequestReason["DC"] = "Display Contract";
    SupportRequestReason["PS"] = "Premium Support";
    SupportRequestReason["PR"] = "Price Reduction";
    SupportRequestReason["FB"] = "From Balance";
})(SupportRequestReason = exports.SupportRequestReason || (exports.SupportRequestReason = {}));
var SupportRequestLineItem = /** @class */ (function () {
    function SupportRequestLineItem() {
    }
    return SupportRequestLineItem;
}());
exports.SupportRequestLineItem = SupportRequestLineItem;
var StateChange = /** @class */ (function () {
    function StateChange() {
    }
    return StateChange;
}());
exports.StateChange = StateChange;
var GeneratedDocument = /** @class */ (function () {
    function GeneratedDocument() {
    }
    return GeneratedDocument;
}());
exports.GeneratedDocument = GeneratedDocument;
//# sourceMappingURL=SupportRequest.js.map