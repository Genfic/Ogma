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
 * @interface CommandOfChapter
 */
export interface CommandOfChapter {
    /**
     * 
     * @type {number}
     * @memberof CommandOfChapter
     */
    objectId?: number;
    /**
     * 
     * @type {string}
     * @memberof CommandOfChapter
     */
    reason?: string | null;
}

export function CommandOfChapterFromJSON(json: any): CommandOfChapter {
    return CommandOfChapterFromJSONTyped(json, false);
}

export function CommandOfChapterFromJSONTyped(json: any, ignoreDiscriminator: boolean): CommandOfChapter {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'objectId': !exists(json, 'objectId') ? undefined : json['objectId'],
        'reason': !exists(json, 'reason') ? undefined : json['reason'],
    };
}

export function CommandOfChapterToJSON(value?: CommandOfChapter | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'objectId': value.objectId,
        'reason': value.reason,
    };
}
