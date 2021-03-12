// <copyright file="configurationTab.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { withTranslation, WithTranslation } from "react-i18next";
import * as microsoftTeams from "@microsoft/teams-js";
import { Loader, List, Text, Flex, Checkbox, MoreIcon, MenuButton, Button, EditIcon, TrashCanIcon, Dialog, CloseIcon } from '@fluentui/react-northstar';
import { TFunction } from "i18next";
import { EmployeeResourceGroupResponse, EmployeeResourceGroupUpdate } from '../../models/employeeResourceGroup';
import { getAllEmployeeResourceGroups, updateEmployeeResourceGroup, deleteEmployeeResourceGroup } from '../../apis/employeeResourceGroupApi';
import { getBaseUrl } from '../../configVariables';
import './configurationTab.scss';
import Constants from '../../constants/constants';

interface IState {
    loader: boolean;
    menuOpen: boolean;
    groupId: string;
    groupName: string;
    openDeleteDialog: boolean;
    theme: string;
    groups: Array<EmployeeResourceGroupResponse>[];
}

export interface ConfigurationTabProps extends RouteComponentProps, WithTranslation {
}

class ConfigurationTab extends React.Component<ConfigurationTabProps, IState> {
    readonly localize: TFunction;
    teamsAadGroupId: string = "";
    constructor(props: ConfigurationTabProps) {
        super(props);
        this.localize = this.props.t;
        this.teamsAadGroupId = "";
        this.state = {
            loader: false,
            menuOpen: true,
            groupId: "",
            groupName: "",
            openDeleteDialog: false,
            theme: "",
            groups: [],
        }
    }

    public async componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.setState({ theme: context.theme! });
            this.teamsAadGroupId = context.groupId!;
        });
        this.setState({
            loader: true
        })

        await this.getResourceGroups();
    }

    /**
    * Method to get all employee resource groups.
    */
    private getResourceGroups = async () => {
        const response = await getAllEmployeeResourceGroups();
        if (response.status === 200 && response.data) {
            this.setState({
                groups: response.data
            });
        }
        this.setState({
            loader: false
        });
    }

    private processLabels = () => {
        const out = [{
            key: "labels",
            content: (
                <Flex vAlign="center" fill gap="gap.small">
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '20%' }} >
                        <Text
                            truncated
                            content={this.localize("GroupName")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '3%' }} shrink={false}>
                        <Text></Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '50%' }} shrink={false}>
                        <Text
                            truncated
                            content={this.localize("Description")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '5%' }}>
                        <Text></Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '20%' }} >
                        <Text
                            truncated
                            content={this.localize("SearchEnabled")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item shrink={0}>
                        <Text></Text>
                    </Flex.Item>
                </Flex>
            ),
            styles: { margin: '0.2rem 0.2rem 0 0' },
        }];
        return out;
    }

    private configurationContent = (group: any) => {
        return (
            <Flex className="listContainer" vAlign="center" fill gap="gap.small">
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '15%' }} grow={1}>
                    <Text size="medium"
                        className="text-color"
                        truncated
                        content={group.groupName}
                        onClick={() => this.openEditTaskModule(group.groupId)}
                    >
                    </Text>
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '50%' }}>
                    <Text
                        className="max-width"
                        truncated
                        content={group.groupDescription}
                    >
                    </Text>
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '5%' }}>
                    <Text></Text>
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '15%' }} >
                    <Checkbox toggle checked={group.includeInSearchResults} onChange={() => this.onSearchEnableChange(group.includeInSearchResults, group, group.groupId)} />
                </Flex.Item>
                <Flex>
                    <MenuButton className="menuContainer" trigger={<Button icon={<MoreIcon />} text iconOnly />}
                        menu={[
                            {
                                icon: <EditIcon size="medium" />,
                                key: 'Edit',
                                content: this.localize("Edit"),
                                onClick: () => {
                                    this.setState({
                                        menuOpen: false,
                                    });
                                    this.openEditTaskModule(group.groupId);
                                }
                            },
                            {
                                icon: <TrashCanIcon size="medium" />,
                                key: 'Delete',
                                content: this.localize("Delete"),
                                onClick: () => {
                                    this.setState({
                                        menuOpen: false,
                                        groupId: group.groupId,
                                        groupName: group.groupName,
                                        openDeleteDialog: true
                                    });
                                }
                            },
                        ]}
                    />
                </Flex>
            </Flex>
        );
    }

    /**
     * Handling check box change event.
     * @param isChecked | boolean value.
     */
    private onSearchEnableChange = async (isChecked: boolean, group: EmployeeResourceGroupUpdate, id: string) => {
        group.includeInSearchResults = !isChecked;
        group.tags = JSON.stringify(group.tags);
        await updateEmployeeResourceGroup(id, group, this.teamsAadGroupId);
        this.getResourceGroups();
    }

    /**
     * Handling delete submit event.
     */
    private onDeleteHandleClick = async () => {
        let response = await deleteEmployeeResourceGroup(this.state.groupId);
        if (response.status === 200 && !response.data) {
            this.setState({ openDeleteDialog: false })
        }

        this.getResourceGroups();
    }

    private openEditTaskModule = (groupId: string) => {
        microsoftTeams.tasks.startTask({
            title: this.localize('UpdateERGHeader'),
            height: Constants.editTaskModuleHeight,
            width: Constants.editTaskModuleWidth,
            url: getBaseUrl() + "/updateerg/" + groupId,
        }, this.submitEditHandler);
    }

    submitEditHandler = async () => {
        await this.getResourceGroups();
    };

    /**
   *Open the task with knowledge base data.
   */
    private onFaqSettingButtonClick = () => {
        microsoftTeams.tasks.startTask({
            title: this.localize('FaqSettingsText'),
            height: Constants.faqSettingsTaskModuleHeight,
            width: Constants.faqSettingsTaskModuleWidth,
            url: getBaseUrl() + "/updateKnowledgeBaseId",
        }, this.submiFaqtHandler);
    }

    submiFaqtHandler = async () => {
    };

    public render(): JSX.Element {
        let keyCount = 0;
        const processItem = (group: any) => {
            keyCount++;
            const out = {
                key: keyCount,
                content: this.configurationContent(group),
                styles: { margin: '0.2rem 0.2rem 0 0' },
            };
            return out;
        };

        const label = this.processLabels();
        const outList = this.state.groups.map(processItem);
        const allGroups = [...label, ...outList];

        if (this.state.loader) {
            return (
                <Loader />
            );
        }
        else {
            return (
                <div>
                    <Flex className="title-container">
                        <Text content={this.localize("GroupSetting")} weight="semibold" />
                        <Flex.Item push>
                            <Flex>
                                <Button primary className="faq-setting-button" content={this.localize("FaqSettingsText")} onClick={() => this.onFaqSettingButtonClick()} />
                            </Flex>
                        </Flex.Item>
                    </Flex>
                    <List items={allGroups} className="list" />
                    <Dialog
                        open={this.state.openDeleteDialog}
                        content={<Text className="word-break" content={this.localize("DeleteContent", { "GroupName": this.state.groupName })} />}
                        onConfirm={() => this.onDeleteHandleClick()}
                        confirmButton={this.localize("Remove")}
                        header={this.localize("DeleteHeaderText")}
                        headerAction={{
                            icon: <CloseIcon />,
                            title: 'Close',
                            onClick: () => { this.setState({ openDeleteDialog: false }) },
                        }}
                        closeOnOutsideClick
                    />
                </div>
            );
        }
    }
}

const configurationTabWithTranslation = withTranslation()(ConfigurationTab);
export default configurationTabWithTranslation;