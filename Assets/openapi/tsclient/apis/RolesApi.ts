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
    CreateRoleCommand,
    CreateRoleCommandFromJSON,
    CreateRoleCommandToJSON,
    ProblemDetails,
    ProblemDetailsFromJSON,
    ProblemDetailsToJSON,
    RoleDto,
    RoleDtoFromJSON,
    RoleDtoToJSON,
    UpdateRoleCommand,
    UpdateRoleCommandFromJSON,
    UpdateRoleCommandToJSON,
} from '../models';

export interface RolesDeleteRoleRequest {
    id: number;
}

export interface RolesGetRoleRequest {
    id: number;
}

export interface RolesPostRoleRequest {
    createRoleCommand: CreateRoleCommand;
}

export interface RolesPutRoleRequest {
    updateRoleCommand: UpdateRoleCommand;
}

/**
 * 
 */
export class RolesApi extends runtime.BaseAPI {

    /**
     */
    async rolesDeleteRoleRaw(requestParameters: RolesDeleteRoleRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        if (requestParameters.id === null || requestParameters.id === undefined) {
            throw new runtime.RequiredError('id','Required parameter requestParameters.id was null or undefined when calling rolesDeleteRole.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/roles/{id}`.replace(`{${"id"}}`, encodeURIComponent(String(requestParameters.id))),
            method: 'DELETE',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async rolesDeleteRole(requestParameters: RolesDeleteRoleRequest, initOverrides?: RequestInit): Promise<void> {
        await this.rolesDeleteRoleRaw(requestParameters, initOverrides);
    }

    /**
     */
    async rolesGetRoleRaw(requestParameters: RolesGetRoleRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<RoleDto>> {
        if (requestParameters.id === null || requestParameters.id === undefined) {
            throw new runtime.RequiredError('id','Required parameter requestParameters.id was null or undefined when calling rolesGetRole.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/roles/{id}`.replace(`{${"id"}}`, encodeURIComponent(String(requestParameters.id))),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => RoleDtoFromJSON(jsonValue));
    }

    /**
     */
    async rolesGetRole(requestParameters: RolesGetRoleRequest, initOverrides?: RequestInit): Promise<RoleDto> {
        const response = await this.rolesGetRoleRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async rolesGetRolesRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<RoleDto>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/roles`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(RoleDtoFromJSON));
    }

    /**
     */
    async rolesGetRoles(initOverrides?: RequestInit): Promise<Array<RoleDto>> {
        const response = await this.rolesGetRolesRaw(initOverrides);
        return await response.value();
    }

    /**
     */
    async rolesPostRoleRaw(requestParameters: RolesPostRoleRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<RoleDto>> {
        if (requestParameters.createRoleCommand === null || requestParameters.createRoleCommand === undefined) {
            throw new runtime.RequiredError('createRoleCommand','Required parameter requestParameters.createRoleCommand was null or undefined when calling rolesPostRole.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        const response = await this.request({
            path: `/api/roles`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: CreateRoleCommandToJSON(requestParameters.createRoleCommand),
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => RoleDtoFromJSON(jsonValue));
    }

    /**
     */
    async rolesPostRole(requestParameters: RolesPostRoleRequest, initOverrides?: RequestInit): Promise<RoleDto> {
        const response = await this.rolesPostRoleRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async rolesPutRoleRaw(requestParameters: RolesPutRoleRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<RoleDto>> {
        if (requestParameters.updateRoleCommand === null || requestParameters.updateRoleCommand === undefined) {
            throw new runtime.RequiredError('updateRoleCommand','Required parameter requestParameters.updateRoleCommand was null or undefined when calling rolesPutRole.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        const response = await this.request({
            path: `/api/roles`,
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: UpdateRoleCommandToJSON(requestParameters.updateRoleCommand),
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => RoleDtoFromJSON(jsonValue));
    }

    /**
     */
    async rolesPutRole(requestParameters: RolesPutRoleRequest, initOverrides?: RequestInit): Promise<RoleDto> {
        const response = await this.rolesPutRoleRaw(requestParameters, initOverrides);
        return await response.value();
    }

}