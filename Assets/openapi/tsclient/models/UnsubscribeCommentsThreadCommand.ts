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
 * @interface UnsubscribeCommentsThreadCommand
 */
export interface UnsubscribeCommentsThreadCommand {
    /**
     * 
     * @type {number}
     * @memberof UnsubscribeCommentsThreadCommand
     */
    threadId?: number;
}

export function UnsubscribeCommentsThreadCommandFromJSON(json: any): UnsubscribeCommentsThreadCommand {
    return UnsubscribeCommentsThreadCommandFromJSONTyped(json, false);
}

export function UnsubscribeCommentsThreadCommandFromJSONTyped(json: any, ignoreDiscriminator: boolean): UnsubscribeCommentsThreadCommand {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'threadId': !exists(json, 'threadId') ? undefined : json['threadId'],
    };
}

export function UnsubscribeCommentsThreadCommandToJSON(value?: UnsubscribeCommentsThreadCommand | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'threadId': value.threadId,
    };
}
