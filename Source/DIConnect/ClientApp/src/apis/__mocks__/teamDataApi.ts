// <copyright file="teamDataApi.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

/** Indicates the response status codes */
export enum ResponseStatus {
    OK = 200
}

export const getTeamDetails = async (): Promise<any> => {

    return Promise.resolve({
        data: { teamDisplayName: "TestTeam", teamDescription: "TestTeamDescription" },
        status: ResponseStatus.OK
    });
}

export const verifyAccessPermission = async (): Promise<any> => {

    return Promise.resolve({
        data: true,
        status: ResponseStatus.OK
    });
}