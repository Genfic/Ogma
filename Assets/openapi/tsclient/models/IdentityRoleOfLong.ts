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
 * @interface IdentityRoleOfLong
 */
export interface IdentityRoleOfLong {
    /**
     * 
     * @type {number}
     * @memberof IdentityRoleOfLong
     */
    id?: number;
    /**
     * 
     * @type {string}
     * @memberof IdentityRoleOfLong
     */
    name?: string | null;
    /**
     * 
     * @type {string}
     * @memberof IdentityRoleOfLong
     */
    normalizedName?: string | null;
    /**
     * 
     * @type {string}
     * @memberof IdentityRoleOfLong
     */
    concurrencyStamp?: string | null;
}

export function IdentityRoleOfLongFromJSON(json: any): IdentityRoleOfLong {
    return IdentityRoleOfLongFromJSONTyped(json, false);
}

export function IdentityRoleOfLongFromJSONTyped(json: any, ignoreDiscriminator: boolean): IdentityRoleOfLong {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'name': !exists(json, 'name') ? undefined : json['name'],
        'normalizedName': !exists(json, 'normalizedName') ? undefined : json['normalizedName'],
        'concurrencyStamp': !exists(json, 'concurrencyStamp') ? undefined : json['concurrencyStamp'],
    };
}

export function IdentityRoleOfLongToJSON(value?: IdentityRoleOfLong | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'name': value.name,
        'normalizedName': value.normalizedName,
        'concurrencyStamp': value.concurrencyStamp,
    };
}
