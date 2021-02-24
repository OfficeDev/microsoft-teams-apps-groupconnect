// <copyright file="card.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { Flex, Text, Image, Label } from '@fluentui/react-northstar'
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import { IDiscoverResourceGroupsPost } from "./discoverResourceGroups";
import './card.scss';

interface ICardProps extends WithTranslation {
    resourceGroupDetails: IDiscoverResourceGroupsPost;
}

class Card extends React.Component<ICardProps> {
    readonly localize: TFunction;
    constructor(props: any) {
        super(props);
        this.localize = this.props.t;        
    }

    public componentDidMount() {
        microsoftTeams.initialize();
    }

    public render(): JSX.Element {
        return (
            <div className="card-bg">
                <Flex gap="gap.smaller" vAlign="center">
                    <Image className="card-img" src={this.props.resourceGroupDetails.imageLink} data-testid="group-img" />
                </Flex>
                <div className="card-body">
                    <Flex gap="gap.smaller" column vAlign="start">
                        <Flex gap="gap.smaller" className="title-flex">
                            <a href={this.props.resourceGroupDetails.groupLink} target="_blank" style={{ textDecoration: "none", color: "inherit" }}>
                                <Text className="card-title-text" size="large" content={this.props.resourceGroupDetails.groupName} weight="bold" data-testid="group-name" />
                            </a>
                        </Flex>
                        <div className="footer-flex">
                            <Flex gap="gap.smaller" className="tags-flex">
                                {
                                    this.props.resourceGroupDetails.tags!.map((value: string, index: number) => {
                                        if (value.trim().length > 0) {
                                            return < Label className="tags-label-wrapper" circular content={value.trim()} data-testid={"group-tag" + index} />
                                        }
                                    })
                                }
                            </Flex>
                            <Flex className="card-content-flex" gap="gap.small">
                                <Text size="small" className="card-content-text" content={this.props.resourceGroupDetails.groupDescription} data-testid="group-desc" />
                            </Flex>
                        </div>
                    </Flex>
                </div>
            </div>
        );
    }
}

export default withTranslation()(Card)