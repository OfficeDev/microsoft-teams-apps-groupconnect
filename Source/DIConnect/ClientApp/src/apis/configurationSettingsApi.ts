// <copyright file="configurationSettingsApi.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

import axios from './axiosJWTDecorator';
import { getBaseUrl } from '../configVariables';

let baseAxiosUrl = getBaseUrl() + '/api';

/**
* GET FAQ configuration settings
*/
export const getFAQConfiguration = async (): Promise<any> => {
    let url = baseAxiosUrl + "/configurationSettings/faqconfiguration";
    return await axios.get(url, false);
}

/**
* GET ERG configuration
*/
export const getERGConfiguration = async (): Promise<any> => {
    let url = baseAxiosUrl + "/configurationSettings";
    return await axios.get(url, false);
}

/**
* PATCH ERG and FAQ configuration settings
*/
export const updateConfiguration = async (payload: {}): Promise<any> => {
    let url = baseAxiosUrl + "/configurationSettings";
    return await axios.patch(url, payload);
}