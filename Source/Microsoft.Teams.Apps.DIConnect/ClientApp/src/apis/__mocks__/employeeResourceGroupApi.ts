// <copyright file="employeeResourceGroupApi.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import { IDiscoverResourceGroupsPost } from "../../components/JoinResourceGroup/discoverResourceGroups"

/** Indicates the response status codes */
export enum ResponseStatus {
    OK = 200
}

export const createNewGroup = async (payload: {}): Promise<any> => {
    return Promise.resolve({
        status: ResponseStatus.OK
    });
}

export const getTeamDetails = async (id: string): Promise<any> => {

    return Promise.resolve({
        data: { teamDisplayName: "TestTeam", teamDescription: "TestTeamDescription" },
        status: ResponseStatus.OK
    });
}

export const getERGData = async () => {
    let httpStatusCode = 200;
    let resourceGroups: Array<IDiscoverResourceGroupsPost> = [
        { groupId: "1234", groupType: "teams", groupName: "group1", groupDescription: "test desc1", imageLink: "link1", groupLink: "group link1", tags: ["test1", "test2"], location: "pune"},
        { groupId: "1235", groupType: "teams", groupName: "group2", groupDescription: "test desc2", imageLink: "link2", groupLink: "group link1", tags: ["test1", "test2"], location: "mumbai"},
        { groupId: "1236", groupType: "external", groupName: "group3", groupDescription: "test desc3", imageLink: "link3", groupLink: "group link1", tags: ["test1", "test2"], location: "mysore"}
    ]
    return Promise.resolve({
        data: resourceGroups,
        status: httpStatusCode
    });
}