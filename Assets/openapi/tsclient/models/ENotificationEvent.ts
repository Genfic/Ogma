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

/**
 * 
 * @export
 * @enum {string}
 */
export enum ENotificationEvent {
    System = 'System',
    WatchedStoryUpdated = 'WatchedStoryUpdated',
    WatchedThreadNewComment = 'WatchedThreadNewComment',
    FollowedAuthorNewBlogpost = 'FollowedAuthorNewBlogpost',
    FollowedAuthorNewStory = 'FollowedAuthorNewStory',
    CommentReply = 'CommentReply'
}

export function ENotificationEventFromJSON(json: any): ENotificationEvent {
    return ENotificationEventFromJSONTyped(json, false);
}

export function ENotificationEventFromJSONTyped(json: any, ignoreDiscriminator: boolean): ENotificationEvent {
    return json as ENotificationEvent;
}

export function ENotificationEventToJSON(value?: ENotificationEvent | null): any {
    return value as any;
}

