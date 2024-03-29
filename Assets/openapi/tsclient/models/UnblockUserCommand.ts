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
 * @interface UnblockUserCommand
 */
export interface UnblockUserCommand {
    /**
     * 
     * @type {string}
     * @memberof UnblockUserCommand
     */
    name?: string | null;
}

export function UnblockUserCommandFromJSON(json: any): UnblockUserCommand {
    return UnblockUserCommandFromJSONTyped(json, false);
}

export function UnblockUserCommandFromJSONTyped(json: any, ignoreDiscriminator: boolean): UnblockUserCommand {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'name': !exists(json, 'name') ? undefined : json['name'],
    };
}

export function UnblockUserCommandToJSON(value?: UnblockUserCommand | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'name': value.name,
    };
}

