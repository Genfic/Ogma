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
    BaseModel,
    BaseModelFromJSON,
    BaseModelFromJSONTyped,
    BaseModelToJSON,
} from './BaseModel';
import {
    OgmaUser,
    OgmaUserFromJSON,
    OgmaUserFromJSONTyped,
    OgmaUserToJSON,
} from './OgmaUser';
import {
    VoteAllOf,
    VoteAllOfFromJSON,
    VoteAllOfFromJSONTyped,
    VoteAllOfToJSON,
} from './VoteAllOf';

/**
 * 
 * @export
 * @interface Vote
 */
export interface Vote {
    /**
     * 
     * @type {number}
     * @memberof Vote
     */
    id: number;
    /**
     * 
     * @type {OgmaUser}
     * @memberof Vote
     */
    user?: OgmaUser | null;
    /**
     * 
     * @type {number}
     * @memberof Vote
     */
    userId?: number;
    /**
     * 
     * @type {number}
     * @memberof Vote
     */
    storyId?: number;
}

export function VoteFromJSON(json: any): Vote {
    return VoteFromJSONTyped(json, false);
}

export function VoteFromJSONTyped(json: any, ignoreDiscriminator: boolean): Vote {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': json['id'],
        'user': !exists(json, 'user') ? undefined : OgmaUserFromJSON(json['user']),
        'userId': !exists(json, 'userId') ? undefined : json['userId'],
        'storyId': !exists(json, 'storyId') ? undefined : json['storyId'],
    };
}

export function VoteToJSON(value?: Vote | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'user': OgmaUserToJSON(value.user),
        'userId': value.userId,
        'storyId': value.storyId,
    };
}
