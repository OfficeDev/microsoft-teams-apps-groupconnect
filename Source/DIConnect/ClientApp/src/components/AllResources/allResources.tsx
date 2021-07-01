// <copyright file="allResources.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

import * as React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import ResourceCard from "./resourceCard";
import './resourceCard.scss';
import { EyeIcon, Grid, Loader, Text, gridBehavior, Input, SearchIcon, Flex } from '@fluentui/react-northstar';
import Constants from '../../constants/constants';
import { getAllResources } from '../../apis/resourceApi';
import { ResourceEntity } from '../../models/resource';

export interface IAllResourceState {
    loader: boolean;
    windowWidth: number;
    resources: ResourceEntity[];
    searchText: string;
    screenWidth: number;
}

class AllResource extends React.Component<WithTranslation, IAllResourceState> {
    readonly localize: TFunction;
    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            loader: true,
            windowWidth: window.innerWidth,
            resources: [],
            searchText: "",
            screenWidth: Constants.screenWidth,
        }
    }

    public componentDidMount() {
        microsoftTeams.initialize();
        window.addEventListener("resize", this.resize.bind(this));
        this.resize();
        this.getAllResourcesDetailAsync();
    }

    public componentWillUnmount() {
        window.removeEventListener('resize', this.resize.bind(this));
    }

    /**
    * Method to get all resources.
    */
    private getAllResourcesDetailAsync = async () => {
        const response = await getAllResources();
        if (response && response.status === 200 && response.data) {
            this.setState({
                resources: response.data
            }
            );
        }
        this.setState({
            loader: false
        });
    }

    /**
    * Invokes when screen is resized
    */
    resize = () => {
        if (window.innerWidth !== this.state.screenWidth) {
            this.setState({ screenWidth: window.innerWidth });
        }
    }

    /**
    * Method to set search text given in the search box.
    */
    public handleSearchInputChange = async (searchText: any) => {
        this.setState({
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
            let scrollViewStyle = { height: "92vh" };
            let resources = this.state.resources;

            if (this.state.searchText !== "" || this.state.searchText !== null) {
                resources = resources.filter(group => ((group.resourceTitle?.toUpperCase().indexOf(this.state.searchText.toUpperCase()) !== -1)
                    || (group.resourceType?.toUpperCase().indexOf(this.state.searchText.toUpperCase()) !== -1)
                    || (group.resourceDescription?.toUpperCase().indexOf(this.state.searchText.toUpperCase()) !== -1)))
            }

            const cards = new Array<any>();
            const tiles = resources.map((value: ResourceEntity, index) => {
                return <ResourceCard tiles={value} key={index} />
            });

            const TilesCount = { "tileFour": 4, "tileThree": 3, "tileTwo": 2, "tileOne": 1 };

            // Cards component array to be rendered in grid.
            let columns = (this.state.windowWidth > Constants.screenWidthMax) ? TilesCount.tileFour
                : (this.state.windowWidth >= Constants.screenWidthDefault && this.state.windowWidth < Constants.screenWidthMax) ? TilesCount.tileThree
                    : (this.state.windowWidth >= Constants.screenWidthMin && this.state.windowWidth < Constants.screenWidthDefault) ? TilesCount.tileTwo
                        : TilesCount.tileOne;
            cards.push(<Grid columns={columns}
                accessibility={gridBehavior}
                className="tile-render"
                content={tiles}>
            </Grid>)

            return (
                <div>
                    {
                        <div className="site-div">
                            <div className="container-subdiv-cardview">
                                {
                                    this.state.screenWidth <= 700 && <Flex.Item grow>
                                        <Flex column gap="gap.small" vAlign="stretch">
                                            <Flex className="mobile-filterbar-title-wrapper">
                                                <div className="searchbar-wrapper-mobile">
                                                    <Input inverted placeholder={this.localize("ResourceSearchPlaceholder")} className="mobile-view-search-box" onChange={(event: any) => this.handleSearchInputChange(event.target.value)} />
                                                    <SearchIcon key="search" className="discover-search-icon-mobile" />
                                                </div>
                                            </Flex>
                                        </Flex>
                                    </Flex.Item>
                                }
                                {
                                    this.state.screenWidth >= 700 && <Flex.Item grow>
                                        <Flex column gap="gap.small" vAlign="stretch">
                                            <Flex className="mobile-filterbar-title-wrapper">
                                                <div className="searchbar-wrapper-mobile">
                                                    <Input inverted placeholder={this.localize("ResourceSearchPlaceholder")} className="desktop-search-box" onChange={(event: any) => this.handleSearchInputChange(event.target.value)} />
                                                    <SearchIcon key="search" className="discover-search-icon" />
                                                </div></Flex>
                                        </Flex>
                                    </Flex.Item>
                                }

                                <div className="scroll-bar" style={scrollViewStyle}>
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
                    }
                </div>
            );
        }
    }
}

export default withTranslation()(AllResource)