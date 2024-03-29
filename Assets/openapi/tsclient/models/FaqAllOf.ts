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
 * @interface FaqAllOf
 */
export interface FaqAllOf {
    /**
     * 
     * @type {string}
     * @memberof FaqAllOf
     */
    question?: string | null;
    /**
     * 
     * @type {string}
     * @memberof FaqAllOf
     */
    answer?: string | null;
    /**
     * 
     * @type {string}
     * @memberof FaqAllOf
     */
    answerRendered?: string | null;
}

export function FaqAllOfFromJSON(json: any): FaqAllOf {
    return FaqAllOfFromJSONTyped(json, false);
}

export function FaqAllOfFromJSONTyped(json: any, ignoreDiscriminator: boolean): FaqAllOf {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'question': !exists(json, 'question') ? undefined : json['question'],
        'answer': !exists(json, 'answer') ? undefined : json['answer'],
        'answerRendered': !exists(json, 'answerRendered') ? undefined : json['answerRendered'],
    };
}

export function FaqAllOfToJSON(value?: FaqAllOf | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'question': value.question,
        'answer': value.answer,
        'answerRendered': value.answerRendered,
    };
}

