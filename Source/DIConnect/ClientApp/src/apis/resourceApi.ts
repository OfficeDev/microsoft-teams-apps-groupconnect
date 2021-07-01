// <copyright file="resourceApi.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

import axios from './axiosJWTDecorator';
import { getBaseUrl } from '../configVariables';

let baseAxiosUrl = getBaseUrl() + '/api';

/**
* GET resource details based on id
* @param unique resource group id.
*/
export const getResource = async (id: string): Promise<any> => {
    let url = baseAxiosUrl + "/resource/" + id;
    return await axios.get(url);
}

/**
* GET all resources
*/
export const getAllResources = async (): Promise<any> => {
    let url = baseAxiosUrl + "/resource";
    return await axios.get(url);
}

/**
* PATCH resource details
*/
export const updateResource = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/resource";
    return await axios.patch(url, payload, false);
}

/**
* POST Create new resource
* @param payload new resource object to be added in storage.
*/
export const createResource = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/resource";
    return await axios.post(url, payload, false);
}

/**
* DELETE resource details based on id
* @param unique resource group id.
*/
export const deleteResource = async (id: string): Promise<any> => {
    let url = baseAxiosUrl + "/resource/" + id;
    return await axios.delete(url);
}