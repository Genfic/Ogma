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
import {
    EDeletedBy,
    EDeletedByFromJSON,
    EDeletedByFromJSONTyped,
    EDeletedByToJSON,
} from './EDeletedBy';
import {
    UserSimpleDto,
    UserSimpleDtoFromJSON,
    UserSimpleDtoFromJSONTyped,
    UserSimpleDtoToJSON,
} from './UserSimpleDto';

/**
 * 
 * @export
 * @interface CommentDto
 */
export interface CommentDto {
    /**
     * 
     * @type {number}
     * @memberof CommentDto
     */
    id?: number;
    /**
     * 
     * @type {UserSimpleDto}
     * @memberof CommentDto
     */
    author?: UserSimpleDto;
    /**
     * 
     * @type {Date}
     * @memberof CommentDto
     */
    dateTime?: Date;
    /**
     * 
     * @type {Date}
     * @memberof CommentDto
     */
    lastEdit?: Date | null;
    /**
     * 
     * @type {number}
     * @memberof CommentDto
     */
    editCount?: number;
    /**
     * 
     * @type {boolean}
     * @memberof CommentDto
     */
    owned?: boolean;
    /**
     * 
     * @type {string}
     * @memberof CommentDto
     */
    body?: string;
    /**
     * 
     * @type {EDeletedBy}
     * @memberof CommentDto
     */
    deletedBy?: EDeletedBy | null;
    /**
     * 
     * @type {boolean}
     * @memberof CommentDto
     */
    isBlocked?: boolean;
}

export function CommentDtoFromJSON(json: any): CommentDto {
    return CommentDtoFromJSONTyped(json, false);
}

export function CommentDtoFromJSONTyped(json: any, ignoreDiscriminator: boolean): CommentDto {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'author': !exists(json, 'author') ? undefined : UserSimpleDtoFromJSON(json['author']),
        'dateTime': !exists(json, 'dateTime') ? undefined : (new Date(json['dateTime'])),
        'lastEdit': !exists(json, 'lastEdit') ? undefined : (json['lastEdit'] === null ? null : new Date(json['lastEdit'])),
        'editCount': !exists(json, 'editCount') ? undefined : json['editCount'],
        'owned': !exists(json, 'owned') ? undefined : json['owned'],
        'body': !exists(json, 'body') ? undefined : json['body'],
        'deletedBy': !exists(json, 'deletedBy') ? undefined : EDeletedByFromJSON(json['deletedBy']),
        'isBlocked': !exists(json, 'isBlocked') ? undefined : json['isBlocked'],
    };
}

export function CommentDtoToJSON(value?: CommentDto | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'author': UserSimpleDtoToJSON(value.author),
        'dateTime': value.dateTime === undefined ? undefined : (value.dateTime.toISOString()),
        'lastEdit': value.lastEdit === undefined ? undefined : (value.lastEdit === null ? null : value.lastEdit.toISOString()),
        'editCount': value.editCount,
        'owned': value.owned,
        'body': value.body,
        'deletedBy': EDeletedByToJSON(value.deletedBy),
        'isBlocked': value.isBlocked,
    };
}
