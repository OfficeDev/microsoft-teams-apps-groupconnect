// <copyright file="configurationTab.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { withTranslation, WithTranslation } from "react-i18next";
import * as microsoftTeams from "@microsoft/teams-js";
import { Loader, List, Text, Flex, Checkbox, MoreIcon, MenuButton, Button, EditIcon, TrashCanIcon, Dialog, CloseIcon, AddIcon, Accordion } from '@fluentui/react-northstar';
import { TFunction } from "i18next";
import { EmployeeResourceGroupResponse, EmployeeResourceGroupUpdate } from '../../models/employeeResourceGroup';
import { ResourceEntity } from '../../models/resource';
import { getAllEmployeeResourceGroups, updateEmployeeResourceGroup, deleteEmployeeResourceGroup } from '../../apis/employeeResourceGroupApi';
import { getAllResources, deleteResource } from '../../apis/resourceApi';
import { getERGConfiguration } from '../../apis/configurationSettingsApi';
import { getBaseUrl } from '../../configVariables';
import './configurationTab.scss';
import Constants from '../../constants/constants';

interface IState {
    loader: boolean;
    menuOpen: boolean;
    groupId: string;
    groupName: string;
    deleteResource: boolean;
    openDeleteDialog: boolean;
    theme: string;
    isERGEnabledForTeam: boolean;
    ergDisplayButtonText: string;
    groups: EmployeeResourceGroupResponse[];
    resources: ResourceEntity[];
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
            deleteResource: false,
            openDeleteDialog: false,
            theme: "",
            ergDisplayButtonText: this.localize('RegisterNewERGDefaultButtonText'),
            isERGEnabledForTeam: false,
            groups: [],
            resources:[]
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
            
        await this.getERGDetails();
        await this.getResourceGroups();
        await this.getAllResourcesDetails();
    }

    /**
    * Method to get all employee resource groups.
    */
    private getResourceGroups = async () => {
        const response = await getAllEmployeeResourceGroups();
        if (response && response.status === 200 && response.data) {
            this.setState({
                groups: response.data
            });
        }
        this.setState({
            loader: false
        });
    }

    /**
    * Method to get all resource details.
    */
    private getAllResourcesDetails = async () => {
        const response = await getAllResources();
        if (response && response.status === 200 && response.data) {
            this.setState({
                resources: response.data
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
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '18%' }} grow={1} >
                        <Text
                            className="max-width-title"
                            content={this.localize("GroupName")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '55%' }} shrink={false}>
                        <Text
                            className="max-width-description"
                            truncated
                            content={this.localize("Description")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '5%' }}>
                        <Text></Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '20%' }}>
                        <Text
                            content={this.localize("SearchEnabled")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item >
                        <Text></Text>
                    </Flex.Item>
                </Flex>
            ),
            styles: { margin: '0.2rem 0.2rem 0 0' },
        }];
        return out;
    }

    private resourceLabels = () => {
        const out = [{
            key: "labels",
            content: (
                <Flex vAlign="center" fill gap="gap.small">
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '18%' }} grow={1} >
                        <Text
                            className="max-width-title"
                            content={this.localize("ResourceNameHeaderText")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '55%' }} shrink={false}>
                        <Text
                            className="max-width-description"
                            truncated
                            content={this.localize("Description")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '5%' }}>
                        <Text></Text>
                    </Flex.Item>
                    <Flex.Item size="size.quarter" variables={{ 'size.quarter': '20%' }}>
                        <Text
                            content={this.localize("ResourceTypeHeaderText")}
                        >
                        </Text>
                    </Flex.Item>
                    <Flex.Item >
                        <Text></Text>
                    </Flex.Item>
                </Flex>
            ),
            styles: { margin: '0.2rem 0.2rem 0 0' },
        }];
        return out;
    }

    private configurationContentForERG = (group: any) => {
        return (
            <Flex className="listContainer" vAlign="center" fill gap="gap.small">
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '20%' }} grow={1}>
                    <Text size="medium"
                        className="max-width-title"
                        truncated
                        content={group.groupName}
                        title={group.groupName}
                        onClick={() => this.openEditTaskModule(group.groupId)}
                    >
                    </Text>
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '55%' }}>
                    <Text
                        className="max-width-description"
                        truncated
                        title={group.groupDescription}
                        content={group.groupDescription}
                    >
                    </Text>
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '5%' }}>
                    <Text></Text>
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '15%' }}>
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
                                        deleteResource: false,
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

    private configurationContentForResource = (resource: any) => {
        return (
            <Flex className="listContainer" vAlign="center" fill gap="gap.small">
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '20%' }} grow={1}>
                    <Text size="medium"
                        className="max-width-title"
                        truncated
                        content={resource.resourceTitle}
                        title={resource.resourceTitle}
                        onClick={() => this.openEditResourceTaskModule(resource.resourceId)}
                    >
                    </Text>
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '55%' }}>
                    <Text
                        className="max-width-description"
                        truncated
                        title={resource.resourceDescription}
                        content={resource.resourceDescription}
                    >
                    </Text>
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '5%' }}>
                    <Text></Text>
                </Flex.Item>
                <Flex.Item size="size.quarter" variables={{ 'size.quarter': '15%' }}>
                    <Text size="medium"
                        className="max-width-type"
                        truncated
                        title={resource.resourceType}
                        content={resource.resourceType}
                    ></Text>
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
                                    this.openEditResourceTaskModule(resource.resourceId);
                                }
                            },
                            {
                                icon: <TrashCanIcon size="medium" />,
                                key: 'Delete',
                                content: this.localize("Delete"),
                                onClick: () => {
                                    this.setState({
                                        menuOpen: false,
                                        groupId: resource.resourceId,
                                        groupName: resource.resourceTitle,
                                        deleteResource: true,
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
        if (this.state.deleteResource) {
            let response = await deleteResource(this.state.groupId);
            if (response && response.status === 200 && !response.data) {
                this.setState({ openDeleteDialog: false })
            }

            this.getAllResourcesDetails();
        }
        else {
            let response = await deleteEmployeeResourceGroup(this.state.groupId);
            if (response && response.status === 200 && !response.data) {
                this.setState({ openDeleteDialog: false })
            }

            this.getResourceGroups();
        }
    }

    private openEditTaskModule = (groupId: string) => {
        microsoftTeams.tasks.startTask({
            title: this.localize('UpdateERGHeader'),
            height: Constants.editTaskModuleHeight,
            width: Constants.editTaskModuleWidth,
            url: getBaseUrl() + "/updateerg/" + groupId,
        }, this.submitEditHandler);
    }

    private openEditResourceTaskModule = (resourceId: string) => {
        microsoftTeams.tasks.startTask({
            title: this.localize('UpdateResourceHeader'),
            height: Constants.resourceTaskModuleHeight,
            width: Constants.resourceTaskModuleWidth,
            url: getBaseUrl() + "/updateResource/" + resourceId,
        }, this.submitResourceHandler);
    }

    submitEditHandler = async () => {
        await this.getResourceGroups();
    };

    submitResourceHandler = async () => {
        await this.getAllResourcesDetails();
    };

    /**
   *Open the task with knowledge base data.
   */
    private onFaqSettingButtonClick = () => {
        microsoftTeams.tasks.startTask({
            title: this.localize('ConfigureHeaderText'),
            height: Constants.faqSettingsTaskModuleHeight,
            width: Constants.faqSettingsTaskModuleWidth,
            url: getBaseUrl() + "/updateConfiguration",
        }, this.submitFaqHandler);
    }

    /**
    * Method to get ERG details.
    */
    private getERGDetails = async () => {
        try {
            const response = await getERGConfiguration();
            if (response.status === 200 && response.data) {
                this.setState({
                    ergDisplayButtonText: response.data.value,
                    isERGEnabledForTeam: response.data.isEnabled,
                    loader: false
                });
            }
        } catch (error) {
            // For first run experience we are limiting the create/request new group to global team. Handling 404 error to provide input from user.
            if (error.response.status === 404) {
                this.setState({
                    loader: false
                });
            }
            else {
                throw error;
            }
        }
    }

    /**
   * Method to handle create erg submit request
   */
    handleAddClick = () => {
        microsoftTeams.tasks.startTask({
            title: this.localize("NewERGHeader"),
            height: Constants.editTaskModuleHeight,
            width: Constants.editTaskModuleWidth,
            url: `${getBaseUrl()}/createNewGroup`,
        }, this.submitHandler);
    }

    submitHandler = async () => {
       this.getResourceGroups();
    };

    submitFaqHandler = async () => {
        this.getERGDetails();
    };

    /**
    * Method to handle create new resource submit request
    */
    handleAddResourceClick = () => {
        microsoftTeams.tasks.startTask({
            title: this.localize("CreateNewResourceHeader"),
            height: Constants.resourceTaskModuleHeight,
            width: Constants.resourceTaskModuleWidth,
            url: `${getBaseUrl()}/createNewResource`,
        }, this.submitResourceHandler);
    }

    public render(): JSX.Element {
        let keyCount = 0;
        const processItem = (group: any) => {
            keyCount++;
            const out = {
                key: keyCount,
                content: this.configurationContentForERG(group),
                styles: { margin: '0.2rem 0.2rem 0 0' },
            };
            return out;
        };

        const processItemOfResource = (resource: any) => {
            let resourceKeyCount = 0;
            resourceKeyCount++;
            const out = {
                key: resourceKeyCount,
                content: this.configurationContentForResource(resource),
                styles: { margin: '0.2rem 0.2rem 0 0' },
            };
            return out;
        };

        const label = this.processLabels();
        const resourceLabel = this.resourceLabels();
        const outList = this.state.groups.map(processItem);
        const resourceOutList = this.state.resources.map(processItemOfResource);
        const allGroups = [...label, ...outList];
        const allResources = [...resourceLabel, ...resourceOutList];

        const panels = [
            {
                key: 'ErgGroupsKey',
                title: <Text content={this.localize("ErgGroupsHeaderText")} weight="semibold" />,
                content: <List items={allGroups} className="list" />
            },
            {
                key: 'Resourcekey',
                title: <Text content={this.localize("ResourcesHeaderText")} weight="semibold" />,
                content: <List items={allResources} className="list" />
            },
        ]

        if (this.state.loader) {
            return (
                <Loader />
            );
        }
        else {
            return (
                <div>
                    <Flex className="title-container">
                        {this.state.isERGEnabledForTeam && <Button icon={<AddIcon xSpacing="before" size="smaller" />}
                            content={this.state.ergDisplayButtonText} onClick={this.handleAddClick} text />}
                        <Button icon={<AddIcon xSpacing="before" size="smaller" />}
                            content={this.localize("NewResourceButtonText")} onClick={this.handleAddResourceClick} text />
                        <Flex.Item push>
                            <Flex>
                                <Button primary className="faq-setting-button" content={this.localize("FaqSettingsText")} onClick={() => this.onFaqSettingButtonClick()} />
                            </Flex>
                        </Flex.Item>
                    </Flex>
                    <Accordion panels={panels} defaultActiveIndex={[0, 1]} />
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
