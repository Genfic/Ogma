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
 * @interface CreateQuoteCommand
 */
export interface CreateQuoteCommand {
    /**
     * 
     * @type {string}
     * @memberof CreateQuoteCommand
     */
    body?: string | null;
    /**
     * 
     * @type {string}
     * @memberof CreateQuoteCommand
     */
    author?: string | null;
}

export function CreateQuoteCommandFromJSON(json: any): CreateQuoteCommand {
    return CreateQuoteCommandFromJSONTyped(json, false);
}

export function CreateQuoteCommandFromJSONTyped(json: any, ignoreDiscriminator: boolean): CreateQuoteCommand {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'body': !exists(json, 'body') ? undefined : json['body'],
        'author': !exists(json, 'author') ? undefined : json['author'],
    };
}

export function CreateQuoteCommandToJSON(value?: CreateQuoteCommand | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'body': value.body,
        'author': value.author,
    };
}

