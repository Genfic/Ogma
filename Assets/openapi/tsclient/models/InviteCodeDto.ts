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
 * @interface InviteCodeDto
 */
export interface InviteCodeDto {
    /**
     * 
     * @type {number}
     * @memberof InviteCodeDto
     */
    id?: number;
    /**
     * 
     * @type {string}
     * @memberof InviteCodeDto
     */
    code?: string;
    /**
     * 
     * @type {string}
     * @memberof InviteCodeDto
     */
    normalizedCode?: string;
    /**
     * 
     * @type {string}
     * @memberof InviteCodeDto
     */
    usedByUserName?: string | null;
    /**
     * 
     * @type {string}
     * @memberof InviteCodeDto
     */
    issuedByUserName?: string;
    /**
     * 
     * @type {Date}
     * @memberof InviteCodeDto
     */
    issueDate?: Date;
    /**
     * 
     * @type {Date}
     * @memberof InviteCodeDto
     */
    usedDate?: Date | null;
}

export function InviteCodeDtoFromJSON(json: any): InviteCodeDto {
    return InviteCodeDtoFromJSONTyped(json, false);
}

export function InviteCodeDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): InviteCodeDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'code': !exists(json, 'code') ? undefined : json['code'],
        'normalizedCode': !exists(json, 'normalizedCode') ? undefined : json['normalizedCode'],
        'usedByUserName': !exists(json, 'usedByUserName') ? undefined : json['usedByUserName'],
        'issuedByUserName': !exists(json, 'issuedByUserName') ? undefined : json['issuedByUserName'],
        'issueDate': !exists(json, 'issueDate') ? undefined : (new Date(json['issueDate'])),
        'usedDate': !exists(json, 'usedDate') ? undefined : (json['usedDate'] === null ? null : new Date(json['usedDate'])),
    };
}

export function InviteCodeDtoToJSON(value?: InviteCodeDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'code': value.code,
        'normalizedCode': value.normalizedCode,
        'usedByUserName': value.usedByUserName,
        'issuedByUserName': value.issuedByUserName,
        'issueDate': value.issueDate === undefined ? undefined : (value.issueDate.toISOString()),
        'usedDate': value.usedDate === undefined ? undefined : (value.usedDate === null ? null : value.usedDate.toISOString()),
    };
}

