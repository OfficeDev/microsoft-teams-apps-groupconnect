// <copyright file="employeeResourceGroupApi.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import { EmployeeResourceGroupResponse } from "../../models/employeeResourceGroup";
import { IDiscoverResourceGroupsPost } from "../../components/JoinResourceGroup/discoverResourceGroups"
import { GroupType } from "../../constants/groupType";

/** Indicates the response status codes */
export enum ResponseStatus {
    OK = 200
}

export const createNewGroup = async (payload: {}): Promise<any> => {

    return Promise.resolve({
        status: ResponseStatus.OK
    });
}

export const getSearchableResourceGroupsAsync = async () => {
    let searchableResourcegroups: Array<IDiscoverResourceGroupsPost> = [
        { groupId: "1234", groupType: GroupType.teams, groupName: "group1", groupDescription: "test desc1", imageLink: "link1", groupLink: "group link1", tags: ["test1", "test2"], location: "pune" },
        { groupId: "1235", groupType: GroupType.teams, groupName: "group2", groupDescription: "test desc2", imageLink: "link2", groupLink: "group link1", tags: ["test1", "test2"], location: "mumbai" },
        { groupId: "1236", groupType: GroupType.external, groupName: "group3", groupDescription: "test desc3", imageLink: "link3", groupLink: "group link1", tags: ["test1", "test2"], location: "mysore" }
    ]

    return Promise.resolve({
        data: searchableResourcegroups,
        status: ResponseStatus.OK
    });
}

export const getResourceGroupDetailsAsync = async () => {
    let resourceGroups: EmployeeResourceGroupResponse = { groupType: GroupType.external, groupId: "12345", groupName: "test 1", groupDescription: "test 1 desc", groupLink: "link 1", imageLink: " test img link 1", tags: ["test1", "test2"], location: "pune", includeInSearchResults: false, matchingFrequency: 0, isProfileMatchingEnabled: false }

    return Promise.resolve({
        data: resourceGroups,
        status: ResponseStatus.OK
    });
}