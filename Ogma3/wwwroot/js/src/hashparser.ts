export class HashParser {
    private hash : string;
    
    constructor(hash: string) {
        this.hash = hash;
    }
    
    public parse() : Map<string, string> 
    {
        let out : Map<string, string> = new Map();
        let params: Array<string> = this.hash.substr(1).split('+');
        for (let param of params) {
            let split: Array<string> = param.split('.');
            out.set(split[0], split[1]);
        }
        return out;
    }
}