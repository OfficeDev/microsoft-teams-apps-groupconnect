// <copyright file="discoverResourceGroups.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { Loader, Text, Grid, gridBehavior } from "@fluentui/react-northstar";
import { EyeIcon } from "@fluentui/react-icons-northstar";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import Card from "./card";
import FilterBar from "../FilterBar/filterBar";
import { ICheckBoxItem } from "../FilterBar/filterBar";
import { getSearchableResourceGroupsAsync } from '../../apis/employeeResourceGroupApi';
import './card.scss';
import Constants from '../../constants/constants';
import { GroupType } from '../../constants/groupType';

export interface IDiscoverResourceGroupsPost {
    groupType: GroupType;
    groupName: string;
    groupDescription: string;
    imageLink: string;
    groupLink: string;
    tags: Array<string>;
    location: string;
    groupId: string;
}

export interface IDiscoverResourceGroupsState {
    loader: boolean;
    windowWidth: number;
    resourceGroups: Array<IDiscoverResourceGroupsPost>;
    tagsList: Array<string>;
    locationsList: Array<string>;
    selectedLocationsList: Array<string>;
    selectedTagsList: Array<string>;
    searchText: string;
}

class DiscoverResourceGroups extends React.Component<WithTranslation, IDiscoverResourceGroupsState> {
    readonly localize: TFunction;
    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            loader: true,
            windowWidth: window.innerWidth,
            resourceGroups: [],
            tagsList: [],
            locationsList: [],
            selectedLocationsList: [],
            selectedTagsList: [],
            searchText: "",
        }
    }
    public componentDidMount() {
        microsoftTeams.initialize();
        window.addEventListener("resize", this.setWindowWidth);
        this.discoverResourceGroupsAsync();
    }

    public componentWillUnmount() {
        window.removeEventListener('resize', this.setWindowWidth);
    }

    /**
    * Method to get searchable employee resource groups.
    */
    private discoverResourceGroupsAsync = async () => {
        const response = await getSearchableResourceGroupsAsync();
        if (response.status === 200 && response.data) {
            this.setState({
                resourceGroups: response.data
            }
            );
        }
        this.setFilterBarValues();
        this.setState({
            loader: false
        });
    }

    /**
    * Get window width real time
    */
    private setWindowWidth = () => {
        if (window.innerWidth !== this.state.windowWidth) {
            this.setState({ windowWidth: window.innerWidth });
        }
    };

    /**
    * Method to set unique filter bar values.
    */
    public setFilterBarValues = () => {
        const locations = new Array<string>();
        const tags = new Array<string>();
        if (this.state.resourceGroups.length > 0) {
            this.state.resourceGroups!.forEach((value: IDiscoverResourceGroupsPost, index) => {
                if (!locations.find(item => item.toUpperCase() === value.location.toUpperCase())) {
                    locations.push(value.location);
                }
                value.tags!.forEach((tag: string) => {
                    if (!tags.find(item => item.toUpperCase() === tag.toUpperCase())) {
                        tags.push(tag);
                    }
                });
            });

            this.setState({
                locationsList: locations.sort(),
                tagsList: tags.sort(),
            }
            );
        }
    }

    /**
    *Filter resource groups based on 'location' checkbox selection.
    *@param selectedCheckboxes User selected checkbox array
    */
    onLocationCheckboxStateChange = async (selectedCheckboxes: Array<ICheckBoxItem>) => {
        let selectedLocations = selectedCheckboxes.filter(value => value.isChecked).map((postType: ICheckBoxItem) => { return postType.id.toString().toUpperCase().trim() });
        this.setState({
            selectedLocationsList: selectedLocations
        });
    }

    /**
    *Filter resource groups based on 'tags' checkbox selection.
    *@param selectedCheckboxes User selected checkbox array
    */
    onTagsCheckboxStateChange = async (selectedCheckboxes: Array<ICheckBoxItem>) => {
        let selectedTags = selectedCheckboxes.filter(value => value.isChecked).map((postType: ICheckBoxItem) => { return postType.id.toString().toUpperCase().trim() });
        await this.setState({
            selectedTagsList: selectedTags
        });
    }

    /**
    * Method to set search text given in the search box.
    */
    public handleSearchInputChange = async (searchText: string) => {
        await this.setState({
            searchText: searchText
        })
    }

    public render(): JSX.Element {
        if (this.state.loader) {
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
            let resourceGroups = this.state.selectedLocationsList.length > 0
                ? this.state.resourceGroups.filter(group => this.state.selectedLocationsList.includes(group.location.toUpperCase()))
                : this.state.resourceGroups;

            if (this.state.selectedTagsList.length > 0) {
                resourceGroups = resourceGroups.filter(group => {
                    return group.tags!.some((tag: string) => {
                        return this.state.selectedTagsList.includes(tag.toUpperCase());
                    })
                });
            }

            if (this.state.searchText !== "" || this.state.searchText !== null) {
                resourceGroups = resourceGroups.filter(group => ((group.groupName.toUpperCase().indexOf(this.state.searchText.toUpperCase()) !== -1)
                    || (group.groupDescription.toUpperCase().indexOf(this.state.searchText.toUpperCase()) !== -1)))
            }
            const cards = new Array<any>();
            const tiles = resourceGroups.map((value: IDiscoverResourceGroupsPost) => (
                <Card resourceGroupDetails={value} />
            ));

            // Cards component array to be rendered in grid.
            let columns = (this.state.windowWidth > Constants.screenWidthMax) ? 4
                : (this.state.windowWidth >= Constants.screenWidthDefault && this.state.windowWidth < Constants.screenWidthMax) ? 3
                    : (this.state.windowWidth >= Constants.screenWidthMin && this.state.windowWidth < Constants.screenWidthDefault) ? 2
                        : 1;
            cards.push(<Grid columns={columns}
                accessibility={gridBehavior}
                className="tile-render"
                content={tiles}>
            </Grid>)

            let scrollViewStyle = { height: "92vh" };
            return (
                <div className="site-div">
                    <div className="container-subdiv-cardview">
                        <FilterBar
                            tagsList={this.state.tagsList}
                            locationsList={this.state.locationsList}
                            commandBarSearchText={this.state.searchText}
                            onSearchInputChange={this.handleSearchInputChange}
                            onLocationCheckboxStateChange={this.onLocationCheckboxStateChange}
                            onTagsCheckboxStateChange={this.onTagsCheckboxStateChange}
                        />
                        <div className="scroll-view" style={scrollViewStyle}>
                            {
                                tiles.length > 0 ? cards : <div className="no-post-added-container">
                                    <div className="app-logo">
                                        <EyeIcon size="largest" />
                                    </div>
                                    <div className="no-data-preview">
                                        <Text content={this.localize("NoResultsText")} />
                                    </div>
                                </div>
                            }
                        </div>
                    </div>
                </div>
            );
        }
    }
}

export default withTranslation()(DiscoverResourceGroups)