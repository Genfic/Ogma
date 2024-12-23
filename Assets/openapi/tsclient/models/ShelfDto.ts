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
 * @interface ShelfDto
 */
export interface ShelfDto {
    /**
     * 
     * @type {number}
     * @memberof ShelfDto
     */
    id?: number;
    /**
     * 
     * @type {string}
     * @memberof ShelfDto
     */
    name?: string;
    /**
     * 
     * @type {string}
     * @memberof ShelfDto
     */
    description?: string;
    /**
     * 
     * @type {boolean}
     * @memberof ShelfDto
     */
    isDefault?: boolean;
    /**
     * 
     * @type {boolean}
     * @memberof ShelfDto
     */
    isPublic?: boolean;
    /**
     * 
     * @type {boolean}
     * @memberof ShelfDto
     */
    isQuickAdd?: boolean;
    /**
     * 
     * @type {boolean}
     * @memberof ShelfDto
     */
    trackUpdates?: boolean;
    /**
     * 
     * @type {string}
     * @memberof ShelfDto
     */
    color?: string;
    /**
     * 
     * @type {number}
     * @memberof ShelfDto
     */
    storiesCount?: number;
    /**
     * 
     * @type {string}
     * @memberof ShelfDto
     */
    iconName?: string | null;
    /**
     * 
     * @type {number}
     * @memberof ShelfDto
     */
    iconId?: number | null;
}

export function ShelfDtoFromJSON(json: any): ShelfDto {
    return ShelfDtoFromJSONTyped(json, false);
}

export function ShelfDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): ShelfDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'name': !exists(json, 'name') ? undefined : json['name'],
        'description': !exists(json, 'description') ? undefined : json['description'],
        'isDefault': !exists(json, 'isDefault') ? undefined : json['isDefault'],
        'isPublic': !exists(json, 'isPublic') ? undefined : json['isPublic'],
        'isQuickAdd': !exists(json, 'isQuickAdd') ? undefined : json['isQuickAdd'],
        'trackUpdates': !exists(json, 'trackUpdates') ? undefined : json['trackUpdates'],
        'color': !exists(json, 'color') ? undefined : json['color'],
        'storiesCount': !exists(json, 'storiesCount') ? undefined : json['storiesCount'],
        'iconName': !exists(json, 'iconName') ? undefined : json['iconName'],
        'iconId': !exists(json, 'iconId') ? undefined : json['iconId'],
    };
}

export function ShelfDtoToJSON(value?: ShelfDto | null): any {
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
        'isDefault': value.isDefault,
        'isPublic': value.isPublic,
        'isQuickAdd': value.isQuickAdd,
        'trackUpdates': value.trackUpdates,
        'color': value.color,
        'storiesCount': value.storiesCount,
        'iconName': value.iconName,
        'iconId': value.iconId,
    };
}

