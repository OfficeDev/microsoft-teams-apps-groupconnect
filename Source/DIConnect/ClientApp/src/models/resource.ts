// <copyright file="resource.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

/**
* Resource request object schema
*/
export class ResourceEntity {
    resourceId: string | undefined;
    resourceTitle: string | undefined;
    resourceDescription: string | undefined;
    redirectionUrl: string | undefined;
    imageLink: string | undefined;
    resourceType: string | undefined;
}