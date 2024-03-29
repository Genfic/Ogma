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
 * @interface MarkChapterAsUnreadResponse
 */
export interface MarkChapterAsUnreadResponse {
    /**
     * 
     * @type {Array<number>}
     * @memberof MarkChapterAsUnreadResponse
     */
    read?: Array<number> | null;
}

export function MarkChapterAsUnreadResponseFromJSON(json: any): MarkChapterAsUnreadResponse {
    return MarkChapterAsUnreadResponseFromJSONTyped(json, false);
}

export function MarkChapterAsUnreadResponseFromJSONTyped(json: any, ignoreDiscriminator: boolean): MarkChapterAsUnreadResponse {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'read': !exists(json, 'read') ? undefined : json['read'],
    };
}

export function MarkChapterAsUnreadResponseToJSON(value?: MarkChapterAsUnreadResponse | null): any {
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

