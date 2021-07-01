// <copyright file="resourceCard.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

import { Card, Flex, Text, Image } from '@fluentui/react-northstar';
import * as React from 'react';
import { WithTranslation, withTranslation } from 'react-i18next';
import { TFunction } from 'i18next';
import './resourceCard.scss';
import { ResourceEntity } from '../../models/resource';

// Interface for all resources tab.
interface IResourceCardProps extends WithTranslation {
    tiles: ResourceEntity;
}

// Component for rendering tile for all resource tab.
class ResourceCard extends React.Component<IResourceCardProps> {
    localize: TFunction;
    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
    }

    // Renders the component
    render() {
        const card =
            <>
                <div className="card-grid-tile">
                    <Card className="card">
                        <Card.Body className="all-resource-card-body">
                            <Flex column gap="gap.small">
                                <Image className="card-image" src={this.props.tiles.imageLink} />
                            </Flex>
                        </Card.Body>
                        <Card.Footer>
                            <Flex>
                                <Text
                                    className="card-type"
                                    content={this.props.tiles.resourceType}
                                />
                            </Flex>
                            <Flex>
                                <a href={this.props.tiles.redirectionUrl} target="_blank" style={{ textDecoration: "none", color: "inherit" }}>
                                    <Text
                                        className="card-title"
                                        content={this.props.tiles.resourceTitle}
                                        weight="bold"
                                     />
                                </a>
                            </Flex>
                            <Flex gap="gap.small" column>
                                <Text className="card-content" content={this.props.tiles.resourceDescription} />
                            </Flex>
                        </Card.Footer>
                    </Card>
                </div>
            </>
        return card;
    }
}

export default withTranslation()(ResourceCard);