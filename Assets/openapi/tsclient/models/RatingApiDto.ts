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
 * @interface RatingApiDto
 */
export interface RatingApiDto {
    /**
     * 
     * @type {number}
     * @memberof RatingApiDto
     */
    id?: number;
    /**
     * 
     * @type {string}
     * @memberof RatingApiDto
     */
    name?: string | null;
    /**
     * 
     * @type {string}
     * @memberof RatingApiDto
     */
    description?: string | null;
    /**
     * 
     * @type {number}
     * @memberof RatingApiDto
     */
    order?: number;
    /**
     * 
     * @type {string}
     * @memberof RatingApiDto
     */
    icon?: string | null;
    /**
     * 
     * @type {boolean}
     * @memberof RatingApiDto
     */
    blacklistedByDefault?: boolean;
}

export function RatingApiDtoFromJSON(json: any): RatingApiDto {
    return RatingApiDtoFromJSONTyped(json, false);
}

export function RatingApiDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): RatingApiDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'name': !exists(json, 'name') ? undefined : json['name'],
        'description': !exists(json, 'description') ? undefined : json['description'],
        'order': !exists(json, 'order') ? undefined : json['order'],
        'icon': !exists(json, 'icon') ? undefined : json['icon'],
        'blacklistedByDefault': !exists(json, 'blacklistedByDefault') ? undefined : json['blacklistedByDefault'],
    };
}

export function RatingApiDtoToJSON(value?: RatingApiDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'name': value.name,
        'description': value.description,
        'order': value.order,
        'icon': value.icon,
        'blacklistedByDefault': value.blacklistedByDefault,
    };
}
