/* tslint:disable */
/* eslint-disable */
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.13.2.0 (NJsonSchema v10.5.2.0 (Newtonsoft.Json v12.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------
// ReSharper disable InconsistentNaming

export const getTableInfo = (baseUrl?: string): Promise<TableInfo[]> => {

    let url_ = baseUrl + "/admin/api/telemetry/gettableinfo";
    url_ = url_.replace(/[?&]$/, "");

    let options_ = <RequestInit>{
        method: "GET",
        headers: {
            "Accept": "application/json"
        }
    };

    return fetch(url_, options_).then((_response: Response) => {

        const processGetTableInfo = (response: Response): Promise<TableInfo[]> => {
            const status = response.status;
            let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
            if (status === 200) {
                return response.json().then((_responseText) => {
                    let result200: any = null;
                    let resultData200 = _responseText
                    if (Array.isArray(resultData200)) {
                        result200 = [] as any;
                        for (let item of resultData200)
                            result200!.push(TableInfo.fromJS(item));
                    }
                    else {
                        result200 = <any>null;
                    }
                    return result200;
                });
            } else if (status !== 200 && status !== 204) {
                return response.json().then((_responseText) => {
                    return throwException("An unexpected server error occurred.", status, _responseText, _headers);
                });
            }
            return Promise.resolve<TableInfo[]>(<any>null);
        }

        return processGetTableInfo(_response);
    });
}

export const getImportantItemCounts = (baseUrl?: string): Promise<{ [key: string]: number; }> => {

    let url_ = baseUrl + "/admin/api/telemetry/getimportantitemcounts";
    url_ = url_.replace(/[?&]$/, "");

    let options_ = <RequestInit>{
        method: "GET",
        headers: {
            "Accept": "application/json"
        }
    };

    return fetch(url_, options_).then((_response: Response) => {

        const processGetImportantItemCounts = (response: Response): Promise<{ [key: string]: number; }> => {
            const status = response.status;
            let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
            if (status === 200) {
                return response.json().then((_responseText) => {
                    let result200: any = null;
                    let resultData200 = _responseText
                    if (resultData200) {
                        result200 = {} as any;
                        for (let key in resultData200) {
                            if (resultData200.hasOwnProperty(key))
                                (<any>result200)![key] = resultData200[key];
                        }
                    }
                    else {
                        result200 = <any>null;
                    }
                    return result200;
                });
            } else if (status !== 200 && status !== 204) {
                return response.json().then((_responseText) => {
                    return throwException("An unexpected server error occurred.", status, _responseText, _headers);
                });
            }
            return Promise.resolve<{ [key: string]: number; }>(<any>null);
        }

        return processGetImportantItemCounts(_response);
    });
}

export class TableInfo implements ITableInfo {
    name?: string | undefined;
    size?: number;

    constructor(data?: ITableInfo) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.name = _data["name"];
            this.size = _data["size"];
        }
    }

    static fromJS(data: any): TableInfo {
        data = typeof data === 'object' ? data : {};
        let result = new TableInfo();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["name"] = this.name;
        data["size"] = this.size;
        return data;
    }
}

export interface ITableInfo {
    name?: string | undefined;
    size?: number;
}

export class ApiException extends Error {
    message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isApiException = true;

    static isApiException(obj: any): obj is ApiException {
        return obj.isApiException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): any {
    if (result !== null && result !== undefined)
        throw result;
    else
        throw new ApiException(message, status, response, headers, null);
}