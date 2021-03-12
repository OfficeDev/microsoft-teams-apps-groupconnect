// <copyright file="updateKnowledgeBaseId.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { TFunction } from 'i18next';
import { withTranslation, WithTranslation } from "react-i18next";
import { RouteComponentProps } from 'react-router-dom';
import { Flex, Text, Input, Button, Loader, InfoIcon } from '@fluentui/react-northstar';
import { getKnowledgeBaseId, updateKnowledgeBaseId } from "../../apis/knowledgeBaseSettingsApi";
import Constants from '../../constants/constants';
import './configurationTab.scss';

export interface IUpdateKnowledgeBaseState {
    loading: boolean,
    theme: string;
    knowledgeBaseId: string;
    isKnowledgeBaseIdPresent: boolean;
    submitLoading: boolean;
}

export interface UpdateKnowledgeBaseProps extends RouteComponentProps, WithTranslation {
}

class UpdateKnowledgeBase extends React.Component<UpdateKnowledgeBaseProps, IUpdateKnowledgeBaseState> {
    readonly localize: TFunction;

    constructor(props: UpdateKnowledgeBaseProps) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            loading: true,
            theme:"",
            knowledgeBaseId: "",
            isKnowledgeBaseIdPresent: true,
            submitLoading: false,
        }
    }

    public async componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.setState({
                theme: context.theme!,
            });
        });

        await this.getKnowledgeBaseDetails();
    }

    /**
    * Method to get knowledge base details.
    */
    private getKnowledgeBaseDetails = async () => {
        try {
            const response = await getKnowledgeBaseId();
            if (response.status === 200 && response.data) {
                this.setState({
                    knowledgeBaseId: response.data,
                    loading: false,
                });
            }
        } catch (error) {
            // Knowledge base id will be empty for first run experience. Handling 404 error to provide input from user.
            if (error.response.status === 404) {
                this.setState({ loading: false });
            }
            else {
                throw error;
            }
        }
    }

    /**
    *Submit knowledge base details
    */
    private handleSubmit = async () => {
        if (!this.state.knowledgeBaseId) {
            this.setState({ isKnowledgeBaseIdPresent: false});
            return;
        }

        this.setState({ submitLoading: true });
        let knowledgeBaseData: object = { id: this.state.knowledgeBaseId };
        await updateKnowledgeBaseId(knowledgeBaseData);
        microsoftTeams.tasks.submitTask();
    }

    /**
	*Sets knowledge base id state.
	*@param value Knowledge base id string
	*/
    private onKnowledgeBaseInputChange = (value: string) => {
        this.setState({ knowledgeBaseId: value, isKnowledgeBaseIdPresent: true });
    }

    /**
    *Returns text component containing error message for failed name field validation
    *@param {boolean} isValuePresent Indicates whether value is present
    */
    private getRequiredFieldError = (isValuePresent: boolean) => {
        if (!isValuePresent) {
            return (<Text content={this.localize('RequiredFieldMessage')} error size="small" />);
        }

        return (<></>);
    }

    public render(): JSX.Element {
        if (!this.state.loading) {
            return (
                <div className={this.state.theme === "default" ? "backgroundcolor" : ""} >
                    <Flex className="module-container" column>
                        <Flex className="top-padding">
                            <Text size="small" content={this.localize('QnAMakerTitleText')} className="margin-space" />
                            <InfoIcon outline xSpacing="after" title={this.localize("FaQInfoIconText")} size="small" />
                            <Flex.Item push>
                                {this.getRequiredFieldError(this.state.isKnowledgeBaseIdPresent)}
                            </Flex.Item>
                        </Flex>
                        <Flex>
                            <Input
                                className="between-space"
                                maxLength={Constants.maxLengthKnowledgeBaseId}
                                fluid
                                value={this.state.knowledgeBaseId}
                                placeholder={this.localize('QnAMakerInputPlaceholderText')}
                                onChange={(event: any) => this.onKnowledgeBaseInputChange(event.target.value)}
                            />
                        </Flex>
                        <Flex.Item push>
                            <Flex className="knowledge-base-footer" hAlign="end" >
                                <Button primary content={this.localize("SaveText")}
                                    onClick={this.handleSubmit} disabled={this.state.submitLoading}
                                    loading={this.state.submitLoading} />
                            </Flex>
                        </Flex.Item>
                    </Flex>
                </div>
            )
        }
        else {
            return <Loader />
        }
    }
}

const updateKnowledgeBaseWithTranslation = withTranslation()(UpdateKnowledgeBase);
export default updateKnowledgeBaseWithTranslation;