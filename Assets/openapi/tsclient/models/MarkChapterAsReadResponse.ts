/* tslint:disable */
/* eslint-disable */
/**
 * My Title
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */

import { exists, mapValues } from '../runtime';
/**
 * 
 * @export
 * @interface MarkChapterAsReadResponse
 */
export interface MarkChapterAsReadResponse {
    /**
     * 
     * @type {Array<number>}
     * @memberof MarkChapterAsReadResponse
     */
    read?: Array<number> | null;
}

export function MarkChapterAsReadResponseFromJSON(json: any): MarkChapterAsReadResponse {
    return MarkChapterAsReadResponseFromJSONTyped(json, false);
}

export function MarkChapterAsReadResponseFromJSONTyped(json: any, ignoreDiscriminator: boolean): MarkChapterAsReadResponse {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'read': !exists(json, 'read') ? undefined : json['read'],
    };
}

export function MarkChapterAsReadResponseToJSON(value?: MarkChapterAsReadResponse | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'read': value.read,
    };
}
