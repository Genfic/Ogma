"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.HashParser = void 0;
class HashParser {
    constructor(hash) {
        this.hash = hash;
    }
    parse() {
        let out = new Map();
        let params = this.hash.substr(1).split('+');
        for (let param of params) {
            let split = param.split('.');
            out.set(split[0], split[1]);
        }
        return out;
    }
}
exports.HashParser = HashParser;
//# sourceMappingURL=hashparser.js.map