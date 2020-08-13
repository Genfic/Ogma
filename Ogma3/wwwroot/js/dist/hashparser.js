"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.HashParser = void 0;
var HashParser = /** @class */ (function () {
    function HashParser(hash) {
        this.hash = hash;
    }
    HashParser.prototype.parse = function () {
        var out = new Map();
        var params = this.hash.substr(1).split('+');
        for (var _i = 0, params_1 = params; _i < params_1.length; _i++) {
            var param = params_1[_i];
            var split = param.split('.');
            out.set(split[0], split[1]);
        }
        return out;
    };
    return HashParser;
}());
exports.HashParser = HashParser;
