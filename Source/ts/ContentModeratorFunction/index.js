"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = y[op[0] & 2 ? "return" : op[0] ? "throw" : "next"]) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [0, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
Object.defineProperty(exports, "__esModule", { value: true });
var axios_1 = require("axios");
var CONTENT_MODERATOR_API_URL = "https://westeurope.api.cognitive.microsoft.com/contentmoderator/moderate/v1.0/ProcessText/Screen?language=eng";
var COMPUTER_VISION_API_URL = "https://westeurope.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Description,Tags&language=en";
function run(context, queueInput, image, inputDocumentIn) {
    return __awaiter(this, void 0, void 0, function () {
        var passesText, imageInformation;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, passesTextModeratorAsync(inputDocumentIn)];
                case 1:
                    passesText = _a.sent();
                    context.bindings.inputDocumentOut = inputDocumentIn;
                    return [4 /*yield*/, analyzeImage(context.bindings.image)];
                case 2:
                    imageInformation = _a.sent();
                    context.bindings.inputDocumentOut.IsApproved = imageInformation[1] && passesText;
                    context.bindings.inputDocumentOut.Caption = imageInformation[0];
                    return [2 /*return*/];
            }
        });
    });
}
exports.run = run;
;
function passesTextModeratorAsync(document) {
    return __awaiter(this, void 0, void 0, function () {
        var config, content, result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (document.ReviewText == null) {
                        return [2 /*return*/, true];
                    }
                    config = {
                        headers: {
                            "Ocp-Apim-Subscription-Key": process.env["ContentModerationApiKey"],
                            "Content-Type": "text/plain"
                        }
                    };
                    content = document.ReviewText;
                    return [4 /*yield*/, axios_1.default.post(CONTENT_MODERATOR_API_URL, content, config)];
                case 1:
                    result = _a.sent();
                    // If we have Terms in result it failed the moderation (Terms will have the bad terms)
                    return [2 /*return*/, result.data.Terms == null];
            }
        });
    });
}
function analyzeImage(image) {
    return __awaiter(this, void 0, void 0, function () {
        var config, result, caption, containsCat;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    config = {
                        processData: false,
                        headers: {
                            "Ocp-Apim-Subscription-Key": process.env["MicrosoftVisionApiKey"],
                            "Content-Type": "application/octet-stream"
                        }
                    };
                    return [4 /*yield*/, axios_1.default.post(COMPUTER_VISION_API_URL, image, config)];
                case 1:
                    result = _a.sent();
                    caption = result.data.description.captions[0].text;
                    containsCat = result.data.tags.some(function (item) {
                        return item.name.indexOf("cat") !== -1;
                    });
                    return [2 /*return*/, [caption, containsCat]];
            }
        });
    });
}
//# sourceMappingURL=index.js.map