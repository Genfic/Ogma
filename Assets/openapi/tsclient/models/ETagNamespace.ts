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
export enum ETagNamespace {
    ContentWarning = 'ContentWarning',
    Genre = 'Genre',
    Franchise = 'Franchise'
}

export function ETagNamespaceFromJSON(json: any): ETagNamespace {
    return ETagNamespaceFromJSONTyped(json, false);
}

export function ETagNamespaceFromJSONTyped(json: any, ignoreDiscriminator: boolean): ETagNamespace {
    return json as ETagNamespace;
}

export function ETagNamespaceToJSON(value?: ETagNamespace | null): any {
    return value as any;
}
