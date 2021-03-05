// <copyright file="teamChannelConfig.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { getBaseUrl } from '../configVariables';
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import Constants from '../constants/constants';

export interface IConfigState {
    url: string;
}

class TeamOwnerConfiguration extends React.Component<WithTranslation, IConfigState> {
    localize: TFunction;
    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            url: getBaseUrl() + "/ergChannelTab"
        }
    }

    public componentDidMount() {
        microsoftTeams.initialize();

        microsoftTeams.settings.registerOnSaveHandler((saveEvent) => {
            microsoftTeams.settings.setSettings({
                entityId: Constants.channelTabEntityId,
                contentUrl: this.state.url,
                suggestedDisplayName: this.localize("ResourceGroupChannelTabName"),
            });
            saveEvent.notifySuccess();
        });

        microsoftTeams.settings.setValidityState(true);
    }

    public render(): JSX.Element {
        return (
            <div className="configContainer">
                <h3>{this.localize("ConfigureTabMessage")}</h3>
            </div>
        );
    }
}

export default withTranslation()(TeamOwnerConfiguration);