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
    EReportableContentTypes,
    EReportableContentTypesFromJSON,
    EReportableContentTypesFromJSONTyped,
    EReportableContentTypesToJSON,
} from './EReportableContentTypes';

/**
 * 
 * @export
 * @interface ReportContentCommand
 */
export interface ReportContentCommand {
    /**
     * 
     * @type {number}
     * @memberof ReportContentCommand
     */
    itemId?: number;
    /**
     * 
     * @type {string}
     * @memberof ReportContentCommand
     */
    reason?: string | null;
    /**
     * 
     * @type {EReportableContentTypes}
     * @memberof ReportContentCommand
     */
    itemType?: EReportableContentTypes;
}

export function ReportContentCommandFromJSON(json: any): ReportContentCommand {
    return ReportContentCommandFromJSONTyped(json, false);
}

export function ReportContentCommandFromJSONTyped(json: any, ignoreDiscriminator: boolean): ReportContentCommand {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'itemId': !exists(json, 'itemId') ? undefined : json['itemId'],
        'reason': !exists(json, 'reason') ? undefined : json['reason'],
        'itemType': !exists(json, 'itemType') ? undefined : EReportableContentTypesFromJSON(json['itemType']),
    };
}

export function ReportContentCommandToJSON(value?: ReportContentCommand | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'itemId': value.itemId,
        'reason': value.reason,
        'itemType': EReportableContentTypesToJSON(value.itemType),
    };
}
