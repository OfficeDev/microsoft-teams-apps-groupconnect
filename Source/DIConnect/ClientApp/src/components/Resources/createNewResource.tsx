// <copyright file="createNewResource.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

import * as React from "react";
import { WithTranslation, withTranslation } from "react-i18next";
import * as microsoftTeams from "@microsoft/teams-js";
import { Text, Flex, Input, Loader, Button, Image, TextArea } from "@fluentui/react-northstar";
import { TFunction } from "i18next";
import { getBaseUrl } from "../../configVariables";
import './group.scss';
import Constants from "../../constants/constants";
import { ResourceEntity } from "../../models/resource";
import { createResource } from "../../apis/resourceApi";

interface IState {
    loading: boolean,
    theme: string;
    type: string;
    title: string;
    description: string;
    redirectionUrl: string;
    imageLink: string;
    isTypePresent: boolean;
    isTitlePresent: boolean;
    isDescriptionPresent: boolean;
    isRedirectionUrlPresent: boolean;
    isRedirectionUrlValid: boolean;
    isImageLinkPresent: boolean;
    isImageLinkValid: boolean;
    errorMessage: string;
    submitLoading: boolean;
    isResourceCreatedSuccessfully: boolean;
}

class createNewResource extends React.Component<WithTranslation, IState> {
    localize: TFunction;
    
    constructor(props: Readonly<WithTranslation>) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            loading: true,
            theme: "",
            type: "",
            title: "",
            description: "",
            redirectionUrl: "",
            imageLink: "",
            isTypePresent: true,
            isTitlePresent: true,
            isDescriptionPresent: true,
            isRedirectionUrlPresent: true,
            isRedirectionUrlValid: true,
            isImageLinkPresent: true,
            isImageLinkValid: true,
            errorMessage: "",
            submitLoading: false,
            isResourceCreatedSuccessfully: false,
        }
    }

    public componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.setState({
                theme: context.theme!,
                loading: false
            });
        });
    }

    /**
    *Submit new resource details
    */
    private handleSubmit = () => {
        if (this.checkIfSubmitAllowed()) {
            this.setState({ submitLoading: true });
            let resourceDetails: ResourceEntity = {
                resourceTitle: this.state.title,
                resourceDescription: this.state.description,
                imageLink: this.state.imageLink,
                redirectionUrl: this.state.redirectionUrl,
                resourceType: this.state.type,
                resourceId: ""
            }

            // Post resource details
            this.createNewResource(resourceDetails);
        }
    }

    /**
   *Create new group from API
   */
    private async createNewResource(resourceDetails: any) {
        try {
            let response = await createResource(resourceDetails);
            if (response && response.status === 201 && response.data) {
                this.setState({ submitLoading: false, errorMessage: "", isResourceCreatedSuccessfully: true });
            }

            // Add delay for 2 seconds to hold task module for displaying success message
            await this.timeout(2000);
            microsoftTeams.tasks.submitTask();
        }
        catch (error) {
            if (error && error.response.status === 400 || error.response.status === 403) {
                this.setState({ submitLoading: false, errorMessage: error.response.data.value });
            }
            else {
                this.setState({ submitLoading: false, errorMessage: this.localize('GeneralErrorMessage') });
            }
        }
    }

    /**
    *Delay time in milli seconds
    */
    private timeout(delay: number) {
        return new Promise(res => setTimeout(res, delay));
    }

    /**
    *Validate input parameters
    */
    private checkIfSubmitAllowed = () => {
        if (this.isNullOrWhiteSpace(this.state.title)) {
            this.setState({ isTitlePresent: false });
            return false;
        }

        if (this.isNullOrWhiteSpace(this.state.description)) {
            this.setState({ isDescriptionPresent: false });
            return false;
        }

        if (this.isNullOrWhiteSpace(this.state.imageLink)) {
            this.setState({ isImageLinkPresent: false });
            return false;
        }

        if (!this.state.isImageLinkValid) {
            return false;
        }

        if (this.isNullOrWhiteSpace(this.state.redirectionUrl)) {
            this.setState({ isRedirectionUrlPresent: false });
            return false;
        }

        if (this.isNullOrWhiteSpace(this.state.type)) {
            this.setState({ isTypePresent: false });
            return false;
        }

        return true;
    }

    /**
    *Checks for null or white space
    */
    private isNullOrWhiteSpace = (input: string): boolean => {
        return !input || !input.trim();
    }

    /**
   *Sets group name state.
   *@param title Title string
   */
    private onTypeChange = (value: string) => {
        this.setState({ type: value, isTypePresent: true });
    }

    /**
	*Sets title state.
	*@param title Title string
	*/
    private onTitleChange = (value: string) => {
        this.setState({ title: value, isTitlePresent: true });
    }

    /**
   *Sets description state.
   *@param description Description string
   */
    private onDescriptionChange = (description: string) => {
        this.setState({ description: description, isDescriptionPresent: true });
    }

    /**
    *Sets redirection url state.
    *@param event object
    */
    private onRedirectionUrlChange = (event: any) => {
        let url = event.target.value.toLowerCase();
        if (!((url === "") || (url.startsWith("https://")))) {
            this.setState({
                isRedirectionUrlValid: false, isRedirectionUrlPresent: true
            });
        }
        else {
            this.setState({
                redirectionUrl: event.target.value, isRedirectionUrlValid: true, isRedirectionUrlPresent:true
            });
        }
    }

    /**
    *Sets image link state.
    *@param event object
    */
    private onImageLinkChange = (event: any) => {
        let url = event.target.value.toLowerCase();
        this.setState({ imageLink: url, isImageLinkPresent: true });
        if (!((url === "") || (url.startsWith("https://") || (url.startsWith("data:image/png;base64,")) || (url.startsWith("data:image/jpeg;base64,")) || (url.startsWith("data:image/gif;base64,"))))) {
            this.setState({
                isImageLinkValid: false, isImageLinkPresent: true
            });
        }
        else {
            this.setState({
                imageLink: event.target.value, isImageLinkValid: true, isImageLinkPresent: true
            });
        }
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

    /**
   *Returns text component containing error message for failed name field validation
   *@param {boolean} isValuePresent Indicates whether value is present
   */
    private getInvalidUrlError = (isValuePresent: boolean) => {
        if (!isValuePresent) {
            return (<Text content={this.localize('InvalidUrlMessage')} error size="small" />);
        }

        return (<></>);
    }

    /**
     * Renders the component.
    */
    public render(): JSX.Element {
        if (!this.state.loading && !this.state.isResourceCreatedSuccessfully) {
            return (
                <div className={this.state.theme === "default" ? "backgroundcolor" : "dark"} >
                    <Flex className="tab-container" column>
                        <Flex className="top-padding">
                            <Text size="small" content={this.localize("ResourceTitle")} />
                            <Flex.Item push>
                                {this.getRequiredFieldError(this.state.isTitlePresent)}
                            </Flex.Item>
                        </Flex>
                        <Input
                            className="between-space"
                            maxLength={Constants.maxLengthName}
                            fluid
                            placeholder={this.localize("TitlePlaceholder")}
                            value={this.state.title}
                            onChange={(event: any) => this.onTitleChange(event.target.value)}
                        />
                        <Flex className="top-padding">
                            <Text size="small" content={this.localize("Description")} />
                            <Flex.Item push>
                                {this.getRequiredFieldError(this.state.isDescriptionPresent)}
                            </Flex.Item>
                        </Flex>
                        <Flex className="between-space">
                            <TextArea
                                className="description-input-space"
                                maxLength={Constants.maxLengthResourceDescription}
                                fluid
                                placeholder={this.localize("DescriptionPlaceholder")}
                                value={this.state.description}
                                onChange={(event: any) => this.onDescriptionChange(event.target.value)}
                             />
                        </Flex>
                        <Flex className="top-padding">
                            <Text size="small" content={this.localize("ImageLink")} />
                            <Flex.Item push>
                                {this.getRequiredFieldError(this.state.isImageLinkPresent)}
                            </Flex.Item>
                            <Flex.Item push>
                                {this.getInvalidUrlError(this.state.isImageLinkValid)}
                            </Flex.Item>
                        </Flex>
                        <Input
                            className="between-space"
                            fluid
                            placeholder={this.localize("ResourceImageLinkPlaceholder")}
                            value={this.state.imageLink}
                            onChange={this.onImageLinkChange}
                            data-testid="image_url"
                        />
                        <Flex className="top-padding">
                            <Text size="small" content={this.localize("RedirectionURL")} />
                            <Flex.Item push>
                                {this.getRequiredFieldError(this.state.isRedirectionUrlPresent)}
                            </Flex.Item>
                            <Flex.Item push>
                                {this.getInvalidUrlError(this.state.isRedirectionUrlValid)}
                            </Flex.Item>
                        </Flex>
                        <Input
                            className="between-space"
                            fluid
                            placeholder={this.localize("RedirectionURLPlaceholder")}
                            value={this.state.redirectionUrl}
                            onChange={this.onRedirectionUrlChange}
                        />
                        <Flex className="top-padding">
                            <Text size="small" content={this.localize("Type")} />
                            <Flex.Item push>
                                {this.getRequiredFieldError(this.state.isTypePresent)}
                            </Flex.Item>
                        </Flex>
                        <Input
                            className="between-space"
                            maxLength={Constants.maxLengthType}
                            fluid
                            placeholder={this.localize("TypePlaceholder")}
                            value={this.state.type}
                            onChange={(event: any) => this.onTypeChange(event.target.value)}
                        />
                    </Flex>
                    <Flex className="tab-footer" hAlign="end" >
                        <Flex.Item push>
                            <Text className="error-info" content={this.state.errorMessage} error size="small" />
                        </Flex.Item>
                        <Button primary content={this.localize("SubmitText")}
                            onClick={this.handleSubmit}
                            disabled={this.state.submitLoading}
                            loading={this.state.submitLoading} />
                    </Flex>
                </div>
            )
        }
        else if (this.state.isResourceCreatedSuccessfully) {
            return (
                <div className={this.state.theme === "default" ? "backgroundcolor" : ""} >
                    <div className="submit-group-success-message-container">
                        <Flex column gap="gap.small">
                            <Flex hAlign="center" className="margin-top"><Image className="preview-image-icon" fluid src={`${getBaseUrl()}/Images/successIcon.png`} /></Flex>
                            <Flex hAlign="center" className="space" column>
                                <Text weight="bold"
                                    content={this.localize("ResourceCreatedSuccessMessage")}
                                    size="medium"
                                />
                            </Flex>
                        </Flex>
                    </div>
                </div>)
        }
        else {
            return <Loader />
        }
    }
}

export default withTranslation()(createNewResource)