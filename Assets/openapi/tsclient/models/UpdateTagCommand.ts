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
    ETagNamespace,
    ETagNamespaceFromJSON,
    ETagNamespaceFromJSONTyped,
    ETagNamespaceToJSON,
} from './ETagNamespace';

/**
 * 
 * @export
 * @interface UpdateTagCommand
 */
export interface UpdateTagCommand {
    /**
     * 
     * @type {number}
     * @memberof UpdateTagCommand
     */
    id?: number;
    /**
     * 
     * @type {string}
     * @memberof UpdateTagCommand
     */
    name?: string | null;
    /**
     * 
     * @type {string}
     * @memberof UpdateTagCommand
     */
    description?: string | null;
    /**
     * 
     * @type {ETagNamespace}
     * @memberof UpdateTagCommand
     */
    namespace?: ETagNamespace | null;
}

export function UpdateTagCommandFromJSON(json: any): UpdateTagCommand {
    return UpdateTagCommandFromJSONTyped(json, false);
}

export function UpdateTagCommandFromJSONTyped(json: any, ignoreDiscriminator: boolean): UpdateTagCommand {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'name': !exists(json, 'name') ? undefined : json['name'],
        'description': !exists(json, 'description') ? undefined : json['description'],
        'namespace': !exists(json, 'namespace') ? undefined : ETagNamespaceFromJSON(json['namespace']),
    };
}

export function UpdateTagCommandToJSON(value?: UpdateTagCommand | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'name': value.name,
        'description': value.description,
        'namespace': ETagNamespaceToJSON(value.namespace),
    };
}

