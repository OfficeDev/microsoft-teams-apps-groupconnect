// <copyright file="filterBar.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from "react";
import { Flex, Input, Text, Button, AddIcon } from "@fluentui/react-northstar";
import * as microsoftTeams from "@microsoft/teams-js";
import { initializeIcons } from "@uifabric/icons";
import PopupMenuWrapper from "../../components/popup-menu/popupMenuWrapper";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import Constants from "../../constants/constants";
import { SearchIcon } from "@fluentui/react-icons-northstar";
import { Icon } from '@fluentui/react';
import { getAppId } from "../../apis/appSettingsApi";
import "./filterbar.scss";
import { getBaseUrl } from "../../configVariables";

export interface ICheckBoxItem {
    id: string;
    key: number;
    title: string;
    checkboxLabel: JSX.Element,
    isChecked: boolean;
}

interface IFilterBarProps extends WithTranslation {
    tagsList: Array<string>;
    locationsList: Array<string>;
    commandBarSearchText: string;
    onSearchInputChange: (searchString: string) => void;
    onLocationCheckboxStateChange: (currentValues: Array<ICheckBoxItem>) => void;
    onTagsCheckboxStateChange: (currentValues: Array<ICheckBoxItem>) => void;
}

interface IFilterBarState {
    tagsList: Array<ICheckBoxItem>;
    locationsList: Array<ICheckBoxItem>;
    screenWidth: number;
    isFilterEnabled: boolean;
    hideFilter: boolean;
}

class FilterBar extends React.Component<IFilterBarProps, IFilterBarState> {
    localize: TFunction;
    botId: string;
    constructor(props: IFilterBarProps) {
        super(props);
        this.botId = "";
        initializeIcons();
        this.localize = this.props.t;
        this.state = {
            tagsList: this.props.tagsList.map((value: string, index: number) => {
                return { id: value, isChecked: false, key: index, title: value, checkboxLabel: <Text content={value} /> };
            }),
            locationsList: this.props.locationsList.map((value: string, index: number) => {
                return { id: value, isChecked: false, key: index, title: value, checkboxLabel: <Text content={value} /> };
            }),
            screenWidth: Constants.screenWidth,
            isFilterEnabled: false,
            hideFilter: true
        }
    }

    componentDidMount() {
        window.addEventListener("resize", this.resize.bind(this));
        this.getAppSetting();
        this.resize();
    }

    /**
    *Get bot id from API
    */
    private getAppSetting = async () => {
        let response = await getAppId();
        if (response.data) {
            this.botId = response.data.appId;
        }
    }

    /**
    * Invokes when screen is resized
    */
    resize = () => {
        if (window.innerWidth !== this.state.screenWidth) {
            this.setState({ screenWidth: window.innerWidth });
        }
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.resize.bind(this));
    }

    /**
   *Sets state of 'Location' filter item when checkbox value changes.
   *@param locationValues Array of 'location' checkboxes with updated user selection
   */
    private onLocationCheckboxStateChange = (locationValues: Array<ICheckBoxItem>) => {
        this.setState({ locationsList: locationValues });
        this.props.onLocationCheckboxStateChange(locationValues);
    }

    /**
   *Sets state of 'Tags' filter item when checkbox value changes.
   *@param tagsValues Array of 'tags' checkboxes with updated user selection
   */
    private onTagsCheckboxStateChange = (tagsValues: Array<ICheckBoxItem>) => {
        this.setState({ tagsList: tagsValues });
        this.props.onTagsCheckboxStateChange(tagsValues);
    }

    /**
    * Method to show or hide filter bar when user clicks filter icon
    */
    private onFilterButtonClick = () => {
        if (this.state.isFilterEnabled === false) {
            this.setState({
                isFilterEnabled: true,
                hideFilter: false
            })
        }
        else if (this.state.isFilterEnabled === true) {
            this.setState({
                isFilterEnabled: false,
                hideFilter: true
            })
        }
    }

    /**
    * Method to handle create erg submit request
    */
    handleAddClick = () => {
        microsoftTeams.tasks.startTask({
            completionBotId: this.botId,
            title: this.localize("NewERGHeader"),
            height: Constants.editTaskModuleHeight,
            width: Constants.editTaskModuleWidth,
            url: `${getBaseUrl()}/createNewGroup`,
        }, this.submitHandler);
    }

    submitHandler = async () => {
    };

    /**
    * Renders the component
    */
    public render(): JSX.Element {
        return (
            <div>
                {this.state.screenWidth > Constants.screenWidth &&
                    <div className="filter-bar">
                        <div>
                            <Flex gap="gap.small">
                                <div className="filter-bar-title-container">
                                <Button icon={<AddIcon xSpacing="before" size="smaller" />}
                                    content={this.localize("NewERGTitleText")} onClick={this.handleAddClick} text />
                                </div>
                                <div className="filter-bar-item-container" hidden={this.state.hideFilter}>
                                    <PopupMenuWrapper title={this.localize("Location")} checkboxes={this.state.locationsList} onCheckboxStateChange={this.onLocationCheckboxStateChange} />
                                    <PopupMenuWrapper title={this.localize("Tags")} checkboxes={this.state.tagsList} onCheckboxStateChange={this.onTagsCheckboxStateChange} />
                                </div>
                                <Flex.Item push>
                                    <Flex>
                                        <Button className="filter-button" content={<Text content={this.localize("Filter")} />} icon={this.state.isFilterEnabled ? <Icon iconName="FilterSolid" className="filter-icon-filled" /> : <Icon iconName="Filter" className="filter-icon" />} text onClick={this.onFilterButtonClick} />
                                        <Input inverted fluid placeholder={this.localize("SearchPlaceholder")} onChange={(event: any) => this.props.onSearchInputChange(event.target.value)} value={this.props.commandBarSearchText} data-testid="search-input" />
                                        <SearchIcon key="search" className="discover-search-icon" />
                                    </Flex>
                                </Flex.Item>
                            </Flex>
                        </div>
                    </div>}
                {this.state.screenWidth <= Constants.screenWidth && <Flex gap="gap.small" vAlign="start" className="filter-bar-wrapper">
                    <Flex.Item grow>
                        <Flex column gap="gap.small" vAlign="stretch">
                            <Flex className="mobile-filterbar-title-wrapper">
                                <div className="title-container-mobile">
                                    <Text content={this.localize("NewERGTitleText")} weight="semibold" onClick={this.handleAddClick} />
                                </div>
                                <Button className="filter-button" content={<Text content={this.localize("Filter")} />} icon={this.state.isFilterEnabled ? <Icon iconName="FilterSolid" className="filter-icon-filled" /> : <Icon iconName="Filter" className="filter-icon" />} text onClick={this.onFilterButtonClick} />
                                <div className="searchbar-wrapper-mobile">
                                    <Input className="filter-button-mobile" inverted fluid placeholder={this.localize("SearchPlaceholder")} onChange={(event: any) => this.props.onSearchInputChange(event.target.value)} value={this.props.commandBarSearchText} />
                                    <SearchIcon key="search" className="discover-search-icon-mobile" />
                                </div>
                            </Flex>
                            <Flex className="mobile-filterbar-wrapper">
                                {!this.state.hideFilter && <Flex>
                                    <PopupMenuWrapper title={this.localize("Location")} checkboxes={this.state.locationsList} onCheckboxStateChange={this.onLocationCheckboxStateChange} />
                                    <PopupMenuWrapper title={this.localize("Tags")} checkboxes={this.state.tagsList} onCheckboxStateChange={this.onTagsCheckboxStateChange} />
                                </Flex>}
                            </Flex>
                        </Flex>
                    </Flex.Item>
                </Flex>}
            </div>
        );
    }
}

export default withTranslation()(FilterBar)