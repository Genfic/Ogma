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
 * @interface UpdateRolesCommand
 */
export interface UpdateRolesCommand {
    /**
     * 
     * @type {number}
     * @memberof UpdateRolesCommand
     */
    userId?: number;
    /**
     * 
     * @type {Array<number>}
     * @memberof UpdateRolesCommand
     */
    roles?: Array<number>;
}

export function UpdateRolesCommandFromJSON(json: any): UpdateRolesCommand {
    return UpdateRolesCommandFromJSONTyped(json, false);
}

export function UpdateRolesCommandFromJSONTyped(json: any, ignoreDiscriminator: boolean): UpdateRolesCommand {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'userId': !exists(json, 'userId') ? undefined : json['userId'],
        'roles': !exists(json, 'roles') ? undefined : json['roles'],
    };
}

export function UpdateRolesCommandToJSON(value?: UpdateRolesCommand | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'userId': value.userId,
        'roles': value.roles,
    };
}
