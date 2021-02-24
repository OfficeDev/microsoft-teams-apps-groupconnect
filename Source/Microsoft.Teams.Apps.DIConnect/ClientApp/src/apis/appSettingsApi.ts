// <copyright file="appSettingsApi.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import axios from './axiosJWTDecorator';
import { getBaseUrl } from '../configVariables';

let baseAxiosUrl = getBaseUrl() + '/api';

/**
* Method to get application configuration values.
*/
export const getAppId = async (): Promise<any> => {
    let url = baseAxiosUrl + "/settings";
    return await axios.get(url);
}