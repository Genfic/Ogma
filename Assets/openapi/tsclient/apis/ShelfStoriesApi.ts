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


import * as runtime from '../runtime';
import {
    AddBookToShelfResult,
    AddBookToShelfResultFromJSON,
    AddBookToShelfResultToJSON,
    GetPaginatedUserShelvesResult,
    GetPaginatedUserShelvesResultFromJSON,
    GetPaginatedUserShelvesResultToJSON,
    RemoveBookFromShelfResult,
    RemoveBookFromShelfResultFromJSON,
    RemoveBookFromShelfResultToJSON,
    ShelfDto,
    ShelfDtoFromJSON,
    ShelfDtoToJSON,
} from '../models';

export interface ShelfStoriesAddToShelfRequest {
    shelfId: number;
    storyId: number;
}

export interface ShelfStoriesGetUserQuickShelvesRequest {
    storyId: number;
}

export interface ShelfStoriesGetUserShelvesPaginatedRequest {
    storyId: number;
    page?: number;
}

export interface ShelfStoriesRemoveFromShelfRequest {
    shelfId: number;
    storyId: number;
}

/**
 * 
 */
export class ShelfStoriesApi extends runtime.BaseAPI {

    /**
     */
    async shelfStoriesAddToShelfRaw(requestParameters: ShelfStoriesAddToShelfRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<AddBookToShelfResult>> {
        if (requestParameters.shelfId === null || requestParameters.shelfId === undefined) {
            throw new runtime.RequiredError('shelfId','Required parameter requestParameters.shelfId was null or undefined when calling shelfStoriesAddToShelf.');
        }

        if (requestParameters.storyId === null || requestParameters.storyId === undefined) {
            throw new runtime.RequiredError('storyId','Required parameter requestParameters.storyId was null or undefined when calling shelfStoriesAddToShelf.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/shelfstories/{shelfId}/{storyId}`.replace(`{${"shelfId"}}`, encodeURIComponent(String(requestParameters.shelfId))).replace(`{${"storyId"}}`, encodeURIComponent(String(requestParameters.storyId))),
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => AddBookToShelfResultFromJSON(jsonValue));
    }

    /**
     */
    async shelfStoriesAddToShelf(requestParameters: ShelfStoriesAddToShelfRequest, initOverrides?: RequestInit): Promise<AddBookToShelfResult> {
        const response = await this.shelfStoriesAddToShelfRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async shelfStoriesGetUserQuickShelvesRaw(requestParameters: ShelfStoriesGetUserQuickShelvesRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<ShelfDto>>> {
        if (requestParameters.storyId === null || requestParameters.storyId === undefined) {
            throw new runtime.RequiredError('storyId','Required parameter requestParameters.storyId was null or undefined when calling shelfStoriesGetUserQuickShelves.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/shelfstories/{storyId}/quick`.replace(`{${"storyId"}}`, encodeURIComponent(String(requestParameters.storyId))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(ShelfDtoFromJSON));
    }

    /**
     */
    async shelfStoriesGetUserQuickShelves(requestParameters: ShelfStoriesGetUserQuickShelvesRequest, initOverrides?: RequestInit): Promise<Array<ShelfDto>> {
        const response = await this.shelfStoriesGetUserQuickShelvesRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async shelfStoriesGetUserShelvesPaginatedRaw(requestParameters: ShelfStoriesGetUserShelvesPaginatedRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<GetPaginatedUserShelvesResult>>> {
        if (requestParameters.storyId === null || requestParameters.storyId === undefined) {
            throw new runtime.RequiredError('storyId','Required parameter requestParameters.storyId was null or undefined when calling shelfStoriesGetUserShelvesPaginated.');
        }

        const queryParameters: any = {};

        if (requestParameters.page !== undefined) {
            queryParameters['page'] = requestParameters.page;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/shelfstories/{storyId}`.replace(`{${"storyId"}}`, encodeURIComponent(String(requestParameters.storyId))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(GetPaginatedUserShelvesResultFromJSON));
    }

    /**
     */
    async shelfStoriesGetUserShelvesPaginated(requestParameters: ShelfStoriesGetUserShelvesPaginatedRequest, initOverrides?: RequestInit): Promise<Array<GetPaginatedUserShelvesResult>> {
        const response = await this.shelfStoriesGetUserShelvesPaginatedRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async shelfStoriesRemoveFromShelfRaw(requestParameters: ShelfStoriesRemoveFromShelfRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<RemoveBookFromShelfResult>> {
        if (requestParameters.shelfId === null || requestParameters.shelfId === undefined) {
            throw new runtime.RequiredError('shelfId','Required parameter requestParameters.shelfId was null or undefined when calling shelfStoriesRemoveFromShelf.');
        }

        if (requestParameters.storyId === null || requestParameters.storyId === undefined) {
            throw new runtime.RequiredError('storyId','Required parameter requestParameters.storyId was null or undefined when calling shelfStoriesRemoveFromShelf.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/shelfstories/{shelfId}/{storyId}`.replace(`{${"shelfId"}}`, encodeURIComponent(String(requestParameters.shelfId))).replace(`{${"storyId"}}`, encodeURIComponent(String(requestParameters.storyId))),
            method: 'DELETE',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => RemoveBookFromShelfResultFromJSON(jsonValue));
    }

    /**
     */
    async shelfStoriesRemoveFromShelf(requestParameters: ShelfStoriesRemoveFromShelfRequest, initOverrides?: RequestInit): Promise<RemoveBookFromShelfResult> {
        const response = await this.shelfStoriesRemoveFromShelfRaw(requestParameters, initOverrides);
        return await response.value();
    }

}