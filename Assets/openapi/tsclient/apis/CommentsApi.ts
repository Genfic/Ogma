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
    CommentDto,
    CommentDtoFromJSON,
    CommentDtoToJSON,
    CommentsControllerPostData,
    CommentsControllerPostDataFromJSON,
    CommentsControllerPostDataToJSON,
    GetRevisionResult,
    GetRevisionResultFromJSON,
    GetRevisionResultToJSON,
    PaginationResultOfCommentDto,
    PaginationResultOfCommentDtoFromJSON,
    PaginationResultOfCommentDtoToJSON,
    UpdateCommentCommand,
    UpdateCommentCommandFromJSON,
    UpdateCommentCommandToJSON,
} from '../models';

export interface CommentsDeleteCommentRequest {
    id: number;
}

export interface CommentsGetCommentRequest {
    id: number;
}

export interface CommentsGetCommentsRequest {
    thread?: number;
    page?: number | null;
    highlight?: number | null;
}

export interface CommentsGetMarkdownRequest {
    id?: number;
}

export interface CommentsGetRevisionsRequest {
    id: number;
}

export interface CommentsPostCommentsRequest {
    commentsControllerPostData: CommentsControllerPostData;
}

export interface CommentsPutCommentRequest {
    updateCommentCommand: UpdateCommentCommand;
}

/**
 * 
 */
export class CommentsApi extends runtime.BaseAPI {

    /**
     */
    async commentsDeleteCommentRaw(requestParameters: CommentsDeleteCommentRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>> {
        if (requestParameters.id === null || requestParameters.id === undefined) {
            throw new runtime.RequiredError('id','Required parameter requestParameters.id was null or undefined when calling commentsDeleteComment.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/comments/{id}`.replace(`{${"id"}}`, encodeURIComponent(String(requestParameters.id))),
            method: 'DELETE',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.TextApiResponse(response) as any;
    }

    /**
     */
    async commentsDeleteComment(requestParameters: CommentsDeleteCommentRequest, initOverrides?: RequestInit): Promise<number> {
        const response = await this.commentsDeleteCommentRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async commentsGetCommentRaw(requestParameters: CommentsGetCommentRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<CommentDto>> {
        if (requestParameters.id === null || requestParameters.id === undefined) {
            throw new runtime.RequiredError('id','Required parameter requestParameters.id was null or undefined when calling commentsGetComment.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/comments/{id}`.replace(`{${"id"}}`, encodeURIComponent(String(requestParameters.id))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => CommentDtoFromJSON(jsonValue));
    }

    /**
     */
    async commentsGetComment(requestParameters: CommentsGetCommentRequest, initOverrides?: RequestInit): Promise<CommentDto> {
        const response = await this.commentsGetCommentRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async commentsGetCommentsRaw(requestParameters: CommentsGetCommentsRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<PaginationResultOfCommentDto>> {
        const queryParameters: any = {};

        if (requestParameters.thread !== undefined) {
            queryParameters['Thread'] = requestParameters.thread;
        }

        if (requestParameters.page !== undefined) {
            queryParameters['Page'] = requestParameters.page;
        }

        if (requestParameters.highlight !== undefined) {
            queryParameters['Highlight'] = requestParameters.highlight;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/comments`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => PaginationResultOfCommentDtoFromJSON(jsonValue));
    }

    /**
     */
    async commentsGetComments(requestParameters: CommentsGetCommentsRequest = {}, initOverrides?: RequestInit): Promise<PaginationResultOfCommentDto> {
        const response = await this.commentsGetCommentsRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async commentsGetMarkdownRaw(requestParameters: CommentsGetMarkdownRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<string>> {
        const queryParameters: any = {};

        if (requestParameters.id !== undefined) {
            queryParameters['Id'] = requestParameters.id;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/comments/md`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.TextApiResponse(response) as any;
    }

    /**
     */
    async commentsGetMarkdown(requestParameters: CommentsGetMarkdownRequest = {}, initOverrides?: RequestInit): Promise<string> {
        const response = await this.commentsGetMarkdownRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async commentsGetRevisionsRaw(requestParameters: CommentsGetRevisionsRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<GetRevisionResult>>> {
        if (requestParameters.id === null || requestParameters.id === undefined) {
            throw new runtime.RequiredError('id','Required parameter requestParameters.id was null or undefined when calling commentsGetRevisions.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/comments/revisions/{id}`.replace(`{${"id"}}`, encodeURIComponent(String(requestParameters.id))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(GetRevisionResultFromJSON));
    }

    /**
     */
    async commentsGetRevisions(requestParameters: CommentsGetRevisionsRequest, initOverrides?: RequestInit): Promise<Array<GetRevisionResult>> {
        const response = await this.commentsGetRevisionsRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async commentsPostCommentsRaw(requestParameters: CommentsPostCommentsRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<CommentDto>> {
        if (requestParameters.commentsControllerPostData === null || requestParameters.commentsControllerPostData === undefined) {
            throw new runtime.RequiredError('commentsControllerPostData','Required parameter requestParameters.commentsControllerPostData was null or undefined when calling commentsPostComments.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        const response = await this.request({
            path: `/api/comments`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: CommentsControllerPostDataToJSON(requestParameters.commentsControllerPostData),
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => CommentDtoFromJSON(jsonValue));
    }

    /**
     */
    async commentsPostComments(requestParameters: CommentsPostCommentsRequest, initOverrides?: RequestInit): Promise<CommentDto> {
        const response = await this.commentsPostCommentsRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async commentsPutCommentRaw(requestParameters: CommentsPutCommentRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<CommentDto>> {
        if (requestParameters.updateCommentCommand === null || requestParameters.updateCommentCommand === undefined) {
            throw new runtime.RequiredError('updateCommentCommand','Required parameter requestParameters.updateCommentCommand was null or undefined when calling commentsPutComment.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        const response = await this.request({
            path: `/api/comments`,
            method: 'PATCH',
            headers: headerParameters,
            query: queryParameters,
            body: UpdateCommentCommandToJSON(requestParameters.updateCommentCommand),
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => CommentDtoFromJSON(jsonValue));
    }

    /**
     */
    async commentsPutComment(requestParameters: CommentsPutCommentRequest, initOverrides?: RequestInit): Promise<CommentDto> {
        const response = await this.commentsPutCommentRaw(requestParameters, initOverrides);
        return await response.value();
    }

}
