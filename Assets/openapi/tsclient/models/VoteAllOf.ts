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
import {
    OgmaUser,
    OgmaUserFromJSON,
    OgmaUserFromJSONTyped,
    OgmaUserToJSON,
} from './OgmaUser';

/**
 * 
 * @export
 * @interface VoteAllOf
 */
export interface VoteAllOf {
    /**
     * 
     * @type {OgmaUser}
     * @memberof VoteAllOf
     */
    user?: OgmaUser | null;
    /**
     * 
     * @type {number}
     * @memberof VoteAllOf
     */
    userId?: number;
    /**
     * 
     * @type {number}
     * @memberof VoteAllOf
     */
    storyId?: number;
}

export function VoteAllOfFromJSON(json: any): VoteAllOf {
    return VoteAllOfFromJSONTyped(json, false);
}

export function VoteAllOfFromJSONTyped(json: any, ignoreDiscriminator: boolean): VoteAllOf {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'user': !exists(json, 'user') ? undefined : OgmaUserFromJSON(json['user']),
        'userId': !exists(json, 'userId') ? undefined : json['userId'],
        'storyId': !exists(json, 'storyId') ? undefined : json['storyId'],
    };
}

export function VoteAllOfToJSON(value?: VoteAllOf | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'user': OgmaUserToJSON(value.user),
        'userId': value.userId,
        'storyId': value.storyId,
    };
}
