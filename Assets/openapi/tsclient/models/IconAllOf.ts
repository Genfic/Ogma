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
 * @interface IconAllOf
 */
export interface IconAllOf {
    /**
     * 
     * @type {string}
     * @memberof IconAllOf
     */
    name?: string | null;
}

export function IconAllOfFromJSON(json: any): IconAllOf {
    return IconAllOfFromJSONTyped(json, false);
}

export function IconAllOfFromJSONTyped(json: any, ignoreDiscriminator: boolean): IconAllOf {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'name': !exists(json, 'name') ? undefined : json['name'],
    };
}

export function IconAllOfToJSON(value?: IconAllOf | null): any {
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

