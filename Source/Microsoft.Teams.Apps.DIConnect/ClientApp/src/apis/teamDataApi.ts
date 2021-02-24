// <copyright file="teamDataApi.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import axios from './axiosJWTDecorator';
import { getBaseUrl } from '../configVariables';

let baseAxiosUrl = getBaseUrl() + '/api';

/**
* GET team details
* @param id Teams unique id.
*/
export const getTeamDetails = async (id: string): Promise<any> => {
    let url = baseAxiosUrl + "/teamdata/search?groupId=" + id;
    return await axios.get(url, false);
}

/**
* Method to get resource group owner details for a given teamId.
* @param groupId Teams unique Azure Active Directory group id.
*/
export const verifyAccessPermission = async (groupId: string): Promise<any> => {
    let url = baseAxiosUrl + "/teamdata/owner?groupId=" + groupId;
    return await axios.get(url);
}