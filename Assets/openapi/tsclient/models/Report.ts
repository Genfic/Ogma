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
    Blogpost,
    BlogpostFromJSON,
    BlogpostFromJSONTyped,
    BlogpostToJSON,
} from './Blogpost';
import {
    Chapter,
    ChapterFromJSON,
    ChapterFromJSONTyped,
    ChapterToJSON,
} from './Chapter';
import {
    Club,
    ClubFromJSON,
    ClubFromJSONTyped,
    ClubToJSON,
} from './Club';
import {
    Comment,
    CommentFromJSON,
    CommentFromJSONTyped,
    CommentToJSON,
} from './Comment';
import {
    OgmaUser,
    OgmaUserFromJSON,
    OgmaUserFromJSONTyped,
    OgmaUserToJSON,
} from './OgmaUser';
import {
    ReportAllOf,
    ReportAllOfFromJSON,
    ReportAllOfFromJSONTyped,
    ReportAllOfToJSON,
} from './ReportAllOf';
import {
    Story,
    StoryFromJSON,
    StoryFromJSONTyped,
    StoryToJSON,
} from './Story';

/**
 * 
 * @export
 * @interface Report
 */
export interface Report {
    /**
     * 
     * @type {number}
     * @memberof Report
     */
    id: number;
    /**
     * 
     * @type {OgmaUser}
     * @memberof Report
     */
    reporter?: OgmaUser;
    /**
     * 
     * @type {number}
     * @memberof Report
     */
    reporterId?: number;
    /**
     * 
     * @type {Date}
     * @memberof Report
     */
    reportDate?: Date;
    /**
     * 
     * @type {string}
     * @memberof Report
     */
    reason?: string;
    /**
     * 
     * @type {string}
     * @memberof Report
     */
    contentType?: string;
    /**
     * 
     * @type {Comment}
     * @memberof Report
     */
    comment?: Comment | null;
    /**
     * 
     * @type {number}
     * @memberof Report
     */
    commentId?: number | null;
    /**
     * 
     * @type {OgmaUser}
     * @memberof Report
     */
    user?: OgmaUser | null;
    /**
     * 
     * @type {number}
     * @memberof Report
     */
    userId?: number | null;
    /**
     * 
     * @type {Story}
     * @memberof Report
     */
    story?: Story | null;
    /**
     * 
     * @type {number}
     * @memberof Report
     */
    storyId?: number | null;
    /**
     * 
     * @type {Chapter}
     * @memberof Report
     */
    chapter?: Chapter | null;
    /**
     * 
     * @type {number}
     * @memberof Report
     */
    chapterId?: number | null;
    /**
     * 
     * @type {Blogpost}
     * @memberof Report
     */
    blogpost?: Blogpost | null;
    /**
     * 
     * @type {number}
     * @memberof Report
     */
    blogpostId?: number | null;
    /**
     * 
     * @type {Club}
     * @memberof Report
     */
    club?: Club | null;
    /**
     * 
     * @type {number}
     * @memberof Report
     */
    clubId?: number | null;
}

export function ReportFromJSON(json: any): Report {
    return ReportFromJSONTyped(json, false);
}

export function ReportFromJSONTyped(json: any, ignoreDiscriminator: boolean): Report {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': json['id'],
        'reporter': !exists(json, 'reporter') ? undefined : OgmaUserFromJSON(json['reporter']),
        'reporterId': !exists(json, 'reporterId') ? undefined : json['reporterId'],
        'reportDate': !exists(json, 'reportDate') ? undefined : (new Date(json['reportDate'])),
        'reason': !exists(json, 'reason') ? undefined : json['reason'],
        'contentType': !exists(json, 'contentType') ? undefined : json['contentType'],
        'comment': !exists(json, 'comment') ? undefined : CommentFromJSON(json['comment']),
        'commentId': !exists(json, 'commentId') ? undefined : json['commentId'],
        'user': !exists(json, 'user') ? undefined : OgmaUserFromJSON(json['user']),
        'userId': !exists(json, 'userId') ? undefined : json['userId'],
        'story': !exists(json, 'story') ? undefined : StoryFromJSON(json['story']),
        'storyId': !exists(json, 'storyId') ? undefined : json['storyId'],
        'chapter': !exists(json, 'chapter') ? undefined : ChapterFromJSON(json['chapter']),
        'chapterId': !exists(json, 'chapterId') ? undefined : json['chapterId'],
        'blogpost': !exists(json, 'blogpost') ? undefined : BlogpostFromJSON(json['blogpost']),
        'blogpostId': !exists(json, 'blogpostId') ? undefined : json['blogpostId'],
        'club': !exists(json, 'club') ? undefined : ClubFromJSON(json['club']),
        'clubId': !exists(json, 'clubId') ? undefined : json['clubId'],
    };
}

export function ReportToJSON(value?: Report | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'reporter': OgmaUserToJSON(value.reporter),
        'reporterId': value.reporterId,
        'reportDate': value.reportDate === undefined ? undefined : (value.reportDate.toISOString()),
        'reason': value.reason,
        'contentType': value.contentType,
        'comment': CommentToJSON(value.comment),
        'commentId': value.commentId,
        'user': OgmaUserToJSON(value.user),
        'userId': value.userId,
        'story': StoryToJSON(value.story),
        'storyId': value.storyId,
        'chapter': ChapterToJSON(value.chapter),
        'chapterId': value.chapterId,
        'blogpost': BlogpostToJSON(value.blogpost),
        'blogpostId': value.blogpostId,
        'club': ClubToJSON(value.club),
        'clubId': value.clubId,
    };
}
