// <copyright file="appSettingsApi.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

export const getAppId = async () => {
    let appid = "bc7c2e05-ebbc-42a8-b813-90c30b0dfd69";
    let httpStatusCode = "200";

    return Promise.resolve({
        data: appid,
        status: httpStatusCode
    });
}