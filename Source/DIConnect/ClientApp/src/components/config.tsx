// <copyright file="config.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { Text, Image, List } from '@fluentui/react-northstar'
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import './config.scss';
import { getBaseUrl } from '../configVariables';
import Constants from "../constants/constants";

export interface IConfigState {
    url: string;
    tabName: string;
}

class Configuration extends React.Component<WithTranslation, IConfigState> {
    localize: TFunction;
    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            url: "",
            tabName: "",
        }
    }

    /**     
     * Method to set selected tab name and url based on user's selection.
     */
    updateTabSettings = (newProps: any) => {
        if (newProps.selectedIndex === 0) {
            this.setState({
                tabName: this.localize("CommunicationsTabName"),
                url: getBaseUrl() + "/messages?locale={locale}"
            })
            microsoftTeams.settings.setValidityState(true);
        }
        else if (newProps.selectedIndex === 1) {
            this.setState({
                tabName: this.localize("ConfigurationTabName"),
                url: getBaseUrl() + "/configurationTab?locale={locale}"
            })
            microsoftTeams.settings.setValidityState(true);
        }
    }

    public componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.settings.registerOnSaveHandler((saveEvent) => {
            microsoftTeams.settings.setSettings({
                entityId: Constants.entityId,
                contentUrl: this.state.url,
                suggestedDisplayName: this.state.tabName,
            });
            saveEvent.notifySuccess();
        });
        microsoftTeams.settings.setValidityState(false);
    }

    public render(): JSX.Element {
        return (
            <div className="config-container">
                <List
                    selectable
                    onSelectedIndexChange={(e, newProps: any) => {
                        this.updateTabSettings(newProps)
                    }}
                    items={[
                        {
                            key: 'communication',
                            className:"config-card-container",
                            media: <Image className="img-container" src={getBaseUrl() + "/Artifacts/CommunicationsTab.png"} />,
                            content: <Text className="text-container" content={this.localize("CommunicationsTabOption")} />,
                        },
                        {
                            key: 'configuration',
                            className: "config-card-container",
                            media: <Image className="img-container" src={getBaseUrl() + "/Artifacts/ConfigurationTab.png"} />,
                            content: <Text className="text-container" content={this.localize("ConfigurationTabOption")} />,
                        },
                    ]}
                />
            </div>
        );
    }
}

export default withTranslation()(Configuration)