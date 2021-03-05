// <copyright file="teamChannelTab.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { Flex, Text, Image, Label, Checkbox, Dropdown, Button, Loader } from '@fluentui/react-northstar';
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import { EmployeeResourceGroupResponse } from "../../models/employeeResourceGroup";
import { getResourceGroupDetailsAsync } from '../../apis/employeeResourceGroupApi';
import { verifyAccessPermission } from '../../apis/teamDataApi';
import { getBaseUrl } from '../../configVariables';
import Constants from '../../constants/constants';
import { MatchingFrequencyType } from '../../constants/groupType';
import './teamChannelTab.scss';

export interface ITeamChannelTabState {
    loading: boolean,
    resourceGroup: EmployeeResourceGroupResponse;
}

class TeamChannelTab extends React.Component<WithTranslation, ITeamChannelTabState> {
    readonly localize: TFunction;
    teamId: string = "";
    groupId: string = "";
    isTeamOwner: boolean = false;
    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.teamId = "";
        this.groupId = "";
        this.state = {
            loading: true,
            resourceGroup: { groupType: 0, groupId: "", groupName: "", groupDescription: "", groupLink: "", imageLink: "", tags: [], location: "", includeInSearchResults: false, matchingFrequency: 0, isProfileMatchingEnabled: false },
        }
    }

    public componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext(async (context) => {
            this.teamId = context.teamId!
            this.groupId = context.groupId!;
            await this.verifyAccessPermission();
            this.getResourceGroupDetails();
        });
    }

    /**
   *Get access permissions.
   */
    private verifyAccessPermission = async () => {
        let response = await verifyAccessPermission(this.groupId);
        if (response.data) {
            this.isTeamOwner = response.data
        }
    }

    /**
    * Handle edit submit request.
    */
    private openEditTaskModule = (groupId: string) => {
        microsoftTeams.tasks.startTask({
            title: this.localize('UpdateResourceGroup'),
            height: Constants.editTaskModuleHeight,
            width: Constants.editTaskModuleWidth,
            url: getBaseUrl() + "/updateerg/" + groupId,
        }, this.submitHandler);
    }

    submitHandler = async () => {
        await this.getResourceGroupDetails();
    };

    /**
    *Get resource group details for a given team id.
    */
    private getResourceGroupDetails = async () => {
        let response = await getResourceGroupDetailsAsync(this.teamId, this.groupId);
        if (response.status === 200 && response.data) {
            this.setState({
                resourceGroup: response.data,
            })
        }
        this.setState({
            loading: false
        });
    }

    public render(): JSX.Element {
        if (this.state.loading) {
            return (
                <div className="container-div">
                    <div className="container-subdiv">
                        <div className="loader">
                            <Loader />
                        </div>
                    </div>
                </div>
            );
        }
        else {
            var frequency = this.state.resourceGroup.matchingFrequency === MatchingFrequencyType.weekly ? this.localize("EveryWeek") : this.localize("EveryMonth")
            return (
                <div>
                    <div className="grp-container">
                        <Text content={this.state.resourceGroup.groupName} weight="semibold" />
                        <Flex className="img-grp-container">
                            <Image src={this.state.resourceGroup.imageLink} fluid />
                        </Flex>
                        <Flex className="group-text-container">
                            <Text content={this.state.resourceGroup.groupDescription} />
                        </Flex>
                        <Flex className="location-container">
                            <Text className="labels" content={this.localize("Location")} weight="semibold" />
                            <Text className="location-label" content={this.state.resourceGroup.location} weight="light" />
                        </Flex>
                        <Flex className="tags-container">
                            <Text className="labels" content={this.localize("Tags")} weight="semibold" />
                            <div className="tag-label">
                                {
                                    (this.state.resourceGroup.tags)!.map((value: string) => {
                                        if (value.trim().length > 0) {
                                            return (
                                                <Label
                                                    circular
                                                    className="tags-label-wrapper"
                                                    content={<Text content={value.trim()} title={value.trim()} size="medium" />}
                                                />
                                            )
                                        }
                                    })
                                }
                            </div>
                        </Flex>
                        <Flex className="profile-container">
                            <Text className="labels" content={this.localize("ProfileMatching")} weight="semibold" />
                            <Checkbox className="profile-checkbox" disabled={true} toggle checked={this.state.resourceGroup.isProfileMatchingEnabled} />
                        </Flex>
                        <Flex>
                            <Text className="match-container" content={this.localize("MatchFrequency")} weight="semibold" />
                            <Dropdown aria-readonly={true} disabled={true}
                                className="match-dropdown"
                                items={[
                                    this.localize("EveryWeek"),
                                    this.localize("EveryMonth"),
                                ]}
                                value={frequency}
                                fluid
                            />
                        </Flex>
                    </div>
                    <div className="save-button button-footer">
                        {
                            this.isTeamOwner && <Flex vAlign="end" hAlign="end" gap="gap.medium">
                                <Button primary content={this.localize("Edit")} onClick={() => this.openEditTaskModule(this.state.resourceGroup.groupId)} />
                            </Flex>
                        }
                    </div>
                </div>
            );
        }
    }
}

export default withTranslation()(TeamChannelTab)