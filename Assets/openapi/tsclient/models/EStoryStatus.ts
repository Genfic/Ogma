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
export enum EStoryStatus {
    InProgress = 'InProgress',
    Completed = 'Completed',
    OnHiatus = 'OnHiatus',
    Cancelled = 'Cancelled'
}

export function EStoryStatusFromJSON(json: any): EStoryStatus {
    return EStoryStatusFromJSONTyped(json, false);
}

export function EStoryStatusFromJSONTyped(json: any, ignoreDiscriminator: boolean): EStoryStatus {
    return json as EStoryStatus;
}

export function EStoryStatusToJSON(value?: EStoryStatus | null): any {
    return value as any;
}
