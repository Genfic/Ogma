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
 * @interface DeleteVoteResult
 */
export interface DeleteVoteResult {
    /**
     * 
     * @type {boolean}
     * @memberof DeleteVoteResult
     */
    didVote?: boolean;
    /**
     * 
     * @type {number}
     * @memberof DeleteVoteResult
     */
    count?: number | null;
}

export function DeleteVoteResultFromJSON(json: any): DeleteVoteResult {
    return DeleteVoteResultFromJSONTyped(json, false);
}

export function DeleteVoteResultFromJSONTyped(json: any, ignoreDiscriminator: boolean): DeleteVoteResult {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'didVote': !exists(json, 'didVote') ? undefined : json['didVote'],
        'count': !exists(json, 'count') ? undefined : json['count'],
    };
}

export function DeleteVoteResultToJSON(value?: DeleteVoteResult | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'didVote': value.didVote,
        'count': value.count,
    };
}
