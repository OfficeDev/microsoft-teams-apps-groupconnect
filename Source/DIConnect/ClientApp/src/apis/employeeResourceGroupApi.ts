// <copyright file="employeeResourceGroupApi.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import axios from './axiosJWTDecorator';
import { getBaseUrl } from '../configVariables';

let baseAxiosUrl = getBaseUrl() + '/api';

/**
* Method to get searchable employee resource groups.
*/
export const getSearchableResourceGroupsAsync = async (): Promise<any> => {
    let url = baseAxiosUrl + "/resourcegroups/discover";
    return await axios.get(url);
}

/**
* POST Create new employee resource group
* @param payload new employee resource object to be added in storage.
*/
export const createNewGroup = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/resourcegroups";
    return await axios.post(url, payload, false);
}

/**
* GET employee resource group details based on id
* @param unique resource group id.
*/
export const getEmployeeResourceGroup = async (id: string): Promise<any> => {
    let url = baseAxiosUrl + "/resourcegroups/" + id;
    return await axios.get(url);
}

/**
* GET all employee resource group details
*/
export const getAllEmployeeResourceGroups = async (): Promise<any> => {
    let url = baseAxiosUrl + "/resourcegroups";
    return await axios.get(url);
}

/**
* PATCH update employee resource group details based on id
* @param id resource group unique id.
* @param payload updated employee resource object to be added in storage.
* @param groupId Teams unique group id.
*/
export const updateEmployeeResourceGroup = async (id: string, payload: {}, groupId: string): Promise<any> => {
    let url = baseAxiosUrl + "/resourcegroups/" + id + "?groupId=" + groupId;
    return await axios.patch(url, payload, false);
}

/**
* DELETE employee resource group details based on id
* @param unique resource group id.
*/
export const deleteEmployeeResourceGroup = async (id: string): Promise<any> => {
    let url = baseAxiosUrl + "/resourcegroups/" + id;
    return await axios.delete(url);
}

/**
* Method to get resource group details for a given teamId.
 * @param unique resource team id, 19:xxx.
 * @param unique Teams unique group id.
*/
export const getResourceGroupDetailsAsync = async (teamId: string, groupId: string): Promise<any> => {
    let url = baseAxiosUrl + "/resourcegroups/teams/" + teamId + "?groupId=" + groupId;
    return await axios.get(url);
}