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
 * @interface BanUserCommand
 */
export interface BanUserCommand {
    /**
     * 
     * @type {number}
     * @memberof BanUserCommand
     */
    userId?: number;
    /**
     * 
     * @type {number}
     * @memberof BanUserCommand
     */
    clubId?: number;
    /**
     * 
     * @type {string}
     * @memberof BanUserCommand
     */
    reason?: string | null;
}

export function BanUserCommandFromJSON(json: any): BanUserCommand {
    return BanUserCommandFromJSONTyped(json, false);
}

export function BanUserCommandFromJSONTyped(json: any, ignoreDiscriminator: boolean): BanUserCommand {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'userId': !exists(json, 'userId') ? undefined : json['userId'],
        'clubId': !exists(json, 'clubId') ? undefined : json['clubId'],
        'reason': !exists(json, 'reason') ? undefined : json['reason'],
    };
}

export function BanUserCommandToJSON(value?: BanUserCommand | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'userId': value.userId,
        'clubId': value.clubId,
        'reason': value.reason,
    };
}

