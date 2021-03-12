// <copyright file="constants.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

export default class Constants {
    public static readonly stateTagMaxLength: number = 20;
    public static readonly stateTagMaxCount: number = 3;
    public static readonly stateLocationMaxLength: number = 100;
    public static readonly maxLengthName: number = 100;
    public static readonly maxLengthDescription: number = 300;
    public static readonly maxLengthKnowledgeBaseId: number = 50;
    public static readonly groupCreatedBotCommand: string = "Group created";
    public static readonly editTaskModuleHeight: number = 800;
    public static readonly editTaskModuleWidth: number = 600;
    public static readonly faqSettingsTaskModuleHeight: number = 200;
    public static readonly faqSettingsTaskModuleWidth: number = 400;

    public static readonly screenWidth: number = 700;
    public static readonly screenWidthMax: number = 1100;
    public static readonly screenWidthDefault: number = 875;
    public static readonly screenWidthMin: number = 650;

    //Constant for entity Id
    public static readonly entityId: string = "DI_connect_app";
    public static readonly channelTabEntityId: string = "DI_Connect_Channel_App";   
}