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
 * @interface RatingAllOf
 */
export interface RatingAllOf {
    /**
     * 
     * @type {string}
     * @memberof RatingAllOf
     */
    name?: string | null;
    /**
     * 
     * @type {string}
     * @memberof RatingAllOf
     */
    description?: string | null;
    /**
     * 
     * @type {number}
     * @memberof RatingAllOf
     */
    order?: number;
    /**
     * 
     * @type {string}
     * @memberof RatingAllOf
     */
    icon?: string | null;
    /**
     * 
     * @type {string}
     * @memberof RatingAllOf
     */
    iconId?: string | null;
    /**
     * 
     * @type {boolean}
     * @memberof RatingAllOf
     */
    blacklistedByDefault?: boolean;
}

export function RatingAllOfFromJSON(json: any): RatingAllOf {
    return RatingAllOfFromJSONTyped(json, false);
}

export function RatingAllOfFromJSONTyped(json: any, ignoreDiscriminator: boolean): RatingAllOf {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'name': !exists(json, 'name') ? undefined : json['name'],
        'description': !exists(json, 'description') ? undefined : json['description'],
        'order': !exists(json, 'order') ? undefined : json['order'],
        'icon': !exists(json, 'icon') ? undefined : json['icon'],
        'iconId': !exists(json, 'iconId') ? undefined : json['iconId'],
        'blacklistedByDefault': !exists(json, 'blacklistedByDefault') ? undefined : json['blacklistedByDefault'],
    };
}

export function RatingAllOfToJSON(value?: RatingAllOf | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'name': value.name,
        'description': value.description,
        'order': value.order,
        'icon': value.icon,
        'iconId': value.iconId,
        'blacklistedByDefault': value.blacklistedByDefault,
    };
}
