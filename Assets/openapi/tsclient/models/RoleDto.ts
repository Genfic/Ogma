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
 * @interface RoleDto
 */
export interface RoleDto {
    /**
     * 
     * @type {number}
     * @memberof RoleDto
     */
    id?: number;
    /**
     * 
     * @type {string}
     * @memberof RoleDto
     */
    name?: string;
    /**
     * 
     * @type {string}
     * @memberof RoleDto
     */
    color?: string | null;
    /**
     * 
     * @type {boolean}
     * @memberof RoleDto
     */
    isStaff?: boolean;
    /**
     * 
     * @type {number}
     * @memberof RoleDto
     */
    order?: number;
}

export function RoleDtoFromJSON(json: any): RoleDto {
    return RoleDtoFromJSONTyped(json, false);
}

export function RoleDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): RoleDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'name': !exists(json, 'name') ? undefined : json['name'],
        'color': !exists(json, 'color') ? undefined : json['color'],
        'isStaff': !exists(json, 'isStaff') ? undefined : json['isStaff'],
        'order': !exists(json, 'order') ? undefined : json['order'],
    };
}

export function RoleDtoToJSON(value?: RoleDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'name': value.name,
        'color': value.color,
        'isStaff': value.isStaff,
        'order': value.order,
    };
}

