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
    BaseModel,
    BaseModelFromJSON,
    BaseModelFromJSONTyped,
    BaseModelToJSON,
} from './BaseModel';
import {
    Club,
    ClubFromJSON,
    ClubFromJSONTyped,
    ClubToJSON,
} from './Club';
import {
    ClubThreadAllOf,
    ClubThreadAllOfFromJSON,
    ClubThreadAllOfFromJSONTyped,
    ClubThreadAllOfToJSON,
} from './ClubThreadAllOf';
import {
    CommentsThread,
    CommentsThreadFromJSON,
    CommentsThreadFromJSONTyped,
    CommentsThreadToJSON,
} from './CommentsThread';
import {
    OgmaUser,
    OgmaUserFromJSON,
    OgmaUserFromJSONTyped,
    OgmaUserToJSON,
} from './OgmaUser';

/**
 * 
 * @export
 * @interface ClubThread
 */
export interface ClubThread {
    /**
     * 
     * @type {number}
     * @memberof ClubThread
     */
    id: number;
    /**
     * 
     * @type {string}
     * @memberof ClubThread
     */
    title?: string | null;
    /**
     * 
     * @type {string}
     * @memberof ClubThread
     */
    body?: string | null;
    /**
     * 
     * @type {OgmaUser}
     * @memberof ClubThread
     */
    author?: OgmaUser | null;
    /**
     * 
     * @type {number}
     * @memberof ClubThread
     */
    authorId?: number | null;
    /**
     * 
     * @type {Date}
     * @memberof ClubThread
     */
    creationDate?: Date;
    /**
     * 
     * @type {CommentsThread}
     * @memberof ClubThread
     */
    commentsThread?: CommentsThread | null;
    /**
     * 
     * @type {Club}
     * @memberof ClubThread
     */
    club?: Club | null;
    /**
     * 
     * @type {number}
     * @memberof ClubThread
     */
    clubId?: number;
    /**
     * 
     * @type {Date}
     * @memberof ClubThread
     */
    deletedAt?: Date | null;
    /**
     * 
     * @type {boolean}
     * @memberof ClubThread
     */
    isPinned?: boolean;
}

export function ClubThreadFromJSON(json: any): ClubThread {
    return ClubThreadFromJSONTyped(json, false);
}

export function ClubThreadFromJSONTyped(json: any, ignoreDiscriminator: boolean): ClubThread {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': json['id'],
        'title': !exists(json, 'title') ? undefined : json['title'],
        'body': !exists(json, 'body') ? undefined : json['body'],
        'author': !exists(json, 'author') ? undefined : OgmaUserFromJSON(json['author']),
        'authorId': !exists(json, 'authorId') ? undefined : json['authorId'],
        'creationDate': !exists(json, 'creationDate') ? undefined : (new Date(json['creationDate'])),
        'commentsThread': !exists(json, 'commentsThread') ? undefined : CommentsThreadFromJSON(json['commentsThread']),
        'club': !exists(json, 'club') ? undefined : ClubFromJSON(json['club']),
        'clubId': !exists(json, 'clubId') ? undefined : json['clubId'],
        'deletedAt': !exists(json, 'deletedAt') ? undefined : (json['deletedAt'] === null ? null : new Date(json['deletedAt'])),
        'isPinned': !exists(json, 'isPinned') ? undefined : json['isPinned'],
    };
}

export function ClubThreadToJSON(value?: ClubThread | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'title': value.title,
        'body': value.body,
        'author': OgmaUserToJSON(value.author),
        'authorId': value.authorId,
        'creationDate': value.creationDate === undefined ? undefined : (value.creationDate.toISOString()),
        'commentsThread': CommentsThreadToJSON(value.commentsThread),
        'club': ClubToJSON(value.club),
        'clubId': value.clubId,
        'deletedAt': value.deletedAt === undefined ? undefined : (value.deletedAt === null ? null : value.deletedAt.toISOString()),
        'isPinned': value.isPinned,
    };
}
