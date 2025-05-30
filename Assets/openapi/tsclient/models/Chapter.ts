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
    ChapterAllOf,
    ChapterAllOfFromJSON,
    ChapterAllOfFromJSONTyped,
    ChapterAllOfToJSON,
} from './ChapterAllOf';
import {
    CommentsThread,
    CommentsThreadFromJSON,
    CommentsThreadFromJSONTyped,
    CommentsThreadToJSON,
} from './CommentsThread';
import {
    ContentBlock,
    ContentBlockFromJSON,
    ContentBlockFromJSONTyped,
    ContentBlockToJSON,
} from './ContentBlock';
import {
    Report,
    ReportFromJSON,
    ReportFromJSONTyped,
    ReportToJSON,
} from './Report';
import {
    Story,
    StoryFromJSON,
    StoryFromJSONTyped,
    StoryToJSON,
} from './Story';

/**
 * 
 * @export
 * @interface Chapter
 */
export interface Chapter {
    /**
     * 
     * @type {number}
     * @memberof Chapter
     */
    id: number;
    /**
     * 
     * @type {number}
     * @memberof Chapter
     */
    order?: number;
    /**
     * 
     * @type {Date}
     * @memberof Chapter
     */
    creationDate?: Date;
    /**
     * 
     * @type {Date}
     * @memberof Chapter
     */
    publicationDate?: Date | null;
    /**
     * 
     * @type {string}
     * @memberof Chapter
     */
    title?: string;
    /**
     * 
     * @type {string}
     * @memberof Chapter
     */
    slug?: string;
    /**
     * 
     * @type {string}
     * @memberof Chapter
     */
    body?: string;
    /**
     * 
     * @type {string}
     * @memberof Chapter
     */
    startNotes?: string | null;
    /**
     * 
     * @type {string}
     * @memberof Chapter
     */
    endNotes?: string | null;
    /**
     * 
     * @type {number}
     * @memberof Chapter
     */
    wordCount?: number;
    /**
     * 
     * @type {CommentsThread}
     * @memberof Chapter
     */
    commentsThread?: CommentsThread;
    /**
     * 
     * @type {Story}
     * @memberof Chapter
     */
    story?: Story;
    /**
     * 
     * @type {number}
     * @memberof Chapter
     */
    storyId?: number;
    /**
     * 
     * @type {ContentBlock}
     * @memberof Chapter
     */
    contentBlock?: ContentBlock | null;
    /**
     * 
     * @type {number}
     * @memberof Chapter
     */
    contentBlockId?: number | null;
    /**
     * 
     * @type {Array<Report>}
     * @memberof Chapter
     */
    reports?: Array<Report>;
}

export function ChapterFromJSON(json: any): Chapter {
    return ChapterFromJSONTyped(json, false);
}

export function ChapterFromJSONTyped(json: any, ignoreDiscriminator: boolean): Chapter {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': json['id'],
        'order': !exists(json, 'order') ? undefined : json['order'],
        'creationDate': !exists(json, 'creationDate') ? undefined : (new Date(json['creationDate'])),
        'publicationDate': !exists(json, 'publicationDate') ? undefined : (json['publicationDate'] === null ? null : new Date(json['publicationDate'])),
        'title': !exists(json, 'title') ? undefined : json['title'],
        'slug': !exists(json, 'slug') ? undefined : json['slug'],
        'body': !exists(json, 'body') ? undefined : json['body'],
        'startNotes': !exists(json, 'startNotes') ? undefined : json['startNotes'],
        'endNotes': !exists(json, 'endNotes') ? undefined : json['endNotes'],
        'wordCount': !exists(json, 'wordCount') ? undefined : json['wordCount'],
        'commentsThread': !exists(json, 'commentsThread') ? undefined : CommentsThreadFromJSON(json['commentsThread']),
        'story': !exists(json, 'story') ? undefined : StoryFromJSON(json['story']),
        'storyId': !exists(json, 'storyId') ? undefined : json['storyId'],
        'contentBlock': !exists(json, 'contentBlock') ? undefined : ContentBlockFromJSON(json['contentBlock']),
        'contentBlockId': !exists(json, 'contentBlockId') ? undefined : json['contentBlockId'],
        'reports': !exists(json, 'reports') ? undefined : ((json['reports'] as Array<any>).map(ReportFromJSON)),
    };
}

export function ChapterToJSON(value?: Chapter | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'order': value.order,
        'creationDate': value.creationDate === undefined ? undefined : (value.creationDate.toISOString()),
        'publicationDate': value.publicationDate === undefined ? undefined : (value.publicationDate === null ? null : value.publicationDate.toISOString()),
        'title': value.title,
        'slug': value.slug,
        'body': value.body,
        'startNotes': value.startNotes,
        'endNotes': value.endNotes,
        'wordCount': value.wordCount,
        'commentsThread': CommentsThreadToJSON(value.commentsThread),
        'story': StoryToJSON(value.story),
        'storyId': value.storyId,
        'contentBlock': ContentBlockToJSON(value.contentBlock),
        'contentBlockId': value.contentBlockId,
        'reports': value.reports === undefined ? undefined : ((value.reports as Array<any>).map(ReportToJSON)),
    };
}

