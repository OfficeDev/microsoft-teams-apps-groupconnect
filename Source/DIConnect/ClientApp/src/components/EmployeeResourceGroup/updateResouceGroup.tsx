// <copyright file="updateResouceGroup.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from "react";
import { RouteComponentProps } from "react-router-dom";
import { WithTranslation, withTranslation } from "react-i18next";
import * as microsoftTeams from "@microsoft/teams-js";
import { Text, Flex, Label, Input, Checkbox, Loader, Button, Dropdown, CloseIcon, InfoIcon } from "@fluentui/react-northstar";
import { TFunction } from "i18next";
import { EmployeeResourceGroupUpdate } from "../../models/employeeResourceGroup";
import { getEmployeeResourceGroup, updateEmployeeResourceGroup } from "../../apis/employeeResourceGroupApi";
import { getTeamDetails } from "../../apis/teamDataApi";
import { GroupType, MatchingFrequencyType } from "../../constants/groupType";
import Constants from "../../constants/constants";
import './group.scss';

interface IState {
    loading: boolean,
    theme: string;
    tagValidation: ITagValidationParameters;
    tagsList: Array<string>;
    tag: string;
    groupId: string;
    groupType: string;
    selectedGroupType: number;
    groupName: string;
    groupDescription: string;
    groupLink: string;
    teamName: string;
    teamDescription: string;
    teamLink: string;
    imageLink: string;
    location: string;
    searchEnabled: boolean;
    isGroupTypePresent: boolean;
    isGroupNamePresent: boolean;
    isGroupDescriptionPresent: boolean;
    isGroupLinkPresent: boolean;
    isGroupLinkValid: boolean;
    isTeamNamePresent: boolean;
    isTeamDescriptionPresent: boolean;
    isTeamLinkPresent: boolean;
    isImageLinkPresent: boolean;
    isLocationPresent: boolean;
    isTeamLinkValid: boolean;
    isImageLinkValid: boolean;
    isExternalSelected: boolean;
    isTeamsSelected: boolean;
    errorMessage: string;
    submitLoading: boolean;
    profileMatchFrequency: any;
    frequency: string;
    isProfileMatchingEnabled: boolean;
    isFrequencyTypeMonthly: boolean;
}

export interface ITagValidationParameters {
    isEmpty: boolean;
    isExisting: boolean;
    isLengthValid: boolean;
    isTagsCountValid: boolean;
    containsSemicolon: boolean;
}

export interface IUpdateGroupProps extends RouteComponentProps, WithTranslation {
    updateGroupProps?: any;
}

class UpdateResouceGroup extends React.Component<IUpdateGroupProps, IState> {
    localize: TFunction;
    userObjectId: string = "";
    teamsAadGroupId: string = "";

    constructor(props: IUpdateGroupProps) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            loading: true,
            theme: "",
            tagsList: [],
            tag: "",
            tagValidation: { isEmpty: false, isExisting: false, isLengthValid: true, isTagsCountValid: true, containsSemicolon: false },
            groupId: "",
            groupType: "",
            selectedGroupType: 0,
            groupName: "",
            groupDescription: "",
            groupLink: "",
            teamName: "",
            teamDescription: "",
            teamLink: "",
            imageLink: "",
            location: "",
            searchEnabled: false,
            isGroupTypePresent: true,
            isGroupNamePresent: true,
            isGroupDescriptionPresent: true,
            isGroupLinkPresent: true,
            isGroupLinkValid: true,
            isTeamNamePresent: true,
            isTeamDescriptionPresent: true,
            isTeamLinkPresent: true,
            isImageLinkPresent: true,
            isLocationPresent: true,
            isTeamLinkValid: true,
            isImageLinkValid: true,
            isExternalSelected: false,
            isTeamsSelected: false,
            errorMessage: "",
            submitLoading: false,
            profileMatchFrequency: "",
            frequency: "",
            isProfileMatchingEnabled: true,
            isFrequencyTypeMonthly: true,
        }
    }

    public async componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.userObjectId = context.userObjectId!;
            this.teamsAadGroupId = context.groupId!;
            this.setState({
                theme: context.theme!,
            });
        });

        let params = this.props.match.params;
        if ('id' in params) {
            let id = params['id'];
            await this.getResourceGroup(id);
            this.setState({
                loading: false,
                groupId: id
            });
        }
    }

    /**
   * Method to get employee resource group details.
   */
    private getResourceGroup = async (id: string) => {
        const response = await getEmployeeResourceGroup(id);
        if (response.status === 200 && response.data) {
            this.setState({
                groupType: response.data.groupType === GroupType.teams ? this.localize("Teams") : this.localize("External"),
                selectedGroupType: response.data.groupType,
                isTeamsSelected: response.data.groupType === GroupType.teams,
                isExternalSelected: response.data.groupType !== GroupType.teams,
                imageLink: response.data.imageLink,
                tagsList: response.data.tags,
                location: response.data.location,
                searchEnabled: response.data.includeInSearchResults,
                profileMatchFrequency: response.data.profileMatchFrequency,
                isProfileMatchingEnabled: response.data.isProfileMatchingEnabled,
                isFrequencyTypeMonthly: response.data.matchingFrequency === MatchingFrequencyType.monthly,
            });
            if (response.data.groupType !== GroupType.teams) {
                this.setState({
                    groupName: response.data.groupName,
                    groupDescription: response.data.groupDescription,
                    groupLink: response.data.groupLink
                });
            }
            else {
                this.setState({
                    teamName: response.data.groupName,
                    teamDescription: response.data.groupDescription,
                    teamLink: response.data.groupLink
                });
            }

            if (this.state.isFrequencyTypeMonthly) {
                this.setState({
                    frequency: this.localize("EveryMonth")
                })
            }
            else {
                this.setState({
                    frequency: this.localize("EveryWeek")
                })
            }
        }
    }

    /**
    *Submit new employee resource details
    */
    private handleSubmit = () => {
        if (this.checkIfSubmitAllowed()) {
            this.setState({ submitLoading: true });
            let groupDetails: EmployeeResourceGroupUpdate = {
                groupId: this.state.groupId,
                groupType: this.state.selectedGroupType,
                groupName: this.state.selectedGroupType === GroupType.external ? this.state.groupName : this.state.teamName,
                groupDescription: this.state.selectedGroupType === GroupType.external ? this.state.groupDescription : this.state.teamDescription,
                groupLink: this.state.selectedGroupType === GroupType.external ? this.state.groupLink : this.state.teamLink,
                imageLink: this.state.imageLink,
                location: this.state.location,
                includeInSearchResults: this.state.searchEnabled,
                tags: JSON.stringify(this.state.tagsList),
                isProfileMatchingEnabled: this.state.isProfileMatchingEnabled,
                matchingFrequency: this.state.isFrequencyTypeMonthly ? MatchingFrequencyType.monthly : MatchingFrequencyType.weekly,
            }

            // Post group details
            this.updateEmployeeResourceGroup(groupDetails);
        }
    }

    /**
    *Update employee resource group from API
    */
    private async updateEmployeeResourceGroup(groupDetails: any) {
        try {
            let response = await updateEmployeeResourceGroup(this.state.groupId, groupDetails, this.teamsAadGroupId);
            if (response.status === 200 && response.data) {
                this.setState({ submitLoading: false, errorMessage: "" });
                groupDetails.includeInSearchResults = this.state.searchEnabled;
                microsoftTeams.tasks.submitTask();
            }
        }
        catch (error) {
            if (error.response.status === 400 || error.response.status === 403 || error.response.status === 404) {
                this.setState({ submitLoading: false, errorMessage: error.response.data.value });
            }
            else {
                this.setState({ submitLoading: false, errorMessage: this.localize('GeneralErrorMessage') });
            }
        }
    }

    /**
    *Validate input parameters
    */
    private checkIfSubmitAllowed = () => {
        if (this.isNullorWhiteSpace(this.state.groupType)) {
            this.setState({ isGroupTypePresent: false });
            return false;
        }

        if (this.state.selectedGroupType === GroupType.external && this.isNullorWhiteSpace(this.state.groupName)) {
            this.setState({ isGroupNamePresent: false });
            return false;
        }

        if (this.state.selectedGroupType === GroupType.external && this.isNullorWhiteSpace(this.state.groupDescription)) {
            this.setState({ isGroupDescriptionPresent: false });
            return false;
        }

        if (this.state.selectedGroupType === GroupType.external && this.isNullorWhiteSpace(this.state.groupLink)) {
            this.setState({ isGroupLinkPresent: false });
            return false;
        }

        if (this.state.selectedGroupType === GroupType.teams && this.isNullorWhiteSpace(this.state.teamLink)) {
            this.setState({ isTeamLinkPresent: false });
            return false;
        }

        if (this.state.selectedGroupType === GroupType.teams && this.isNullorWhiteSpace(this.state.teamName)) {
            this.setState({ isTeamNamePresent: false });
            return false;
        }

        if (this.state.selectedGroupType === GroupType.teams && this.isNullorWhiteSpace(this.state.teamDescription)) {
            this.setState({ isTeamDescriptionPresent: false });
            return false;
        }

        if (this.isNullorWhiteSpace(this.state.imageLink)) {
            this.setState({ isImageLinkPresent: false });
            return false;
        }

        if (this.isNullorWhiteSpace(this.state.location)) {
            this.setState({ isLocationPresent: false });
            return false;
        }

        return true;
    }

    /**
    *Checks for null or white space
    */
    private isNullorWhiteSpace = (input: string): boolean => {
        return !input || !input.trim();
    }

    /**
	*Check if tag is valid
	*/
    private checkIfTagIsValid = () => {
        let validationParams: ITagValidationParameters = { isEmpty: false, isLengthValid: true, isExisting: false, isTagsCountValid: false, containsSemicolon: false };
        if (this.state.tag.trim() === "") {
            validationParams.isEmpty = true;
        }

        if (this.state.tag.length > Constants.stateTagMaxLength) {
            validationParams.isLengthValid = false;
        }

        let tags = this.state.tagsList;
        let isTagExist = tags.find((tag: string) => {
            if (tag.toLowerCase() === this.state.tag.toLowerCase()) {
                return tag;
            }
        });

        if (this.state.tag.split(";").length > 1 || this.state.tag.split(",").length > 1) {
            validationParams.containsSemicolon = true;
        }

        if (isTagExist) {
            validationParams.isExisting = true;
        }

        if (this.state.tagsList.length < Constants.stateTagMaxCount) {
            validationParams.isTagsCountValid = true;
        }

        this.setState({ tagValidation: validationParams });

        if (!validationParams.isEmpty && !validationParams.isExisting && validationParams.isLengthValid && validationParams.isTagsCountValid && !validationParams.containsSemicolon) {
            return true;
        }
        return false;
    }

    /**
   *Sets state of tagsList by removing tag using its index.
   *@param index Index of tag to be deleted.
   */
    private onTagRemoveClick = (index: number) => {
        let tags = this.state.tagsList.filter((val, i) => i !== index)
        this.setState({
            tagsList: tags
        });
    }

    /**
    *Returns text component containing error message for empty tag input field
    */
    private getTagError = () => {
        if (this.state.tagValidation.isEmpty) {
            return (<Text content={this.localize("EmptyTagError")} error size="small" />);
        }
        else if (!this.state.tagValidation.isLengthValid) {
            return (<Text content={this.localize("TagLengthError")} error size="small" />);
        }
        else if (this.state.tagValidation.isExisting) {
            return (<Text content={this.localize("SameTagExistsError")} error size="small" />);
        }
        else if (!this.state.tagValidation.isTagsCountValid) {
            return (<Text content={this.localize("TagsCountError")} error size="small" />);
        }
        else if (this.state.tagValidation.containsSemicolon) {
            return (<Text content={this.localize("SemicolonTagError")} error size="small" />);
        }
        return (<></>);
    }

    /**
	*Sets state of tagsList by adding new tag.
	*/
    private onTagAddClick = () => {
        if (this.checkIfTagIsValid()) {
            let tags = [...this.state.tagsList, this.state.tag];
            this.setState({
                tagsList: tags,
                tag: ""
            });
        }
    }

    /**
	* Adds tag when enter key is pressed
	* @param event Object containing event details
	*/
    private onTagKeyUp = (event: any) => {
        if (event.key === 'Enter') {
            this.onTagAddClick();
        }
    }

	/**
	*Sets tag state.
	*@param tag Tag string
	*/
    private onTagChange = (tag: string) => {
        this.setState({ tag: tag })
    }

    /**
    *Sets group type state.
    *@param groupType groupType string
    */
    private onGroupTypeChange = (event: any, itemsData: any) => {
        if (itemsData.value === this.localize("Teams")) {
            this.setState({ selectedGroupType: GroupType.teams, groupType: itemsData.value, isGroupTypePresent: true, isTeamsSelected: true, isExternalSelected: false });
        }
        else {
            this.setState({ selectedGroupType: GroupType.external, groupType: itemsData.value, isGroupTypePresent: true, isExternalSelected: true, isTeamsSelected: false });
        }
    }

    /**
    *Sets group type state.
    *@param groupType groupType string
    */
    private onFrequencyTypeChange = (event: any, itemsData: any) => {
        if (itemsData.value === this.localize("EveryWeek")) {
            this.setState({ isFrequencyTypeMonthly: false, frequency: this.localize("EveryWeek") });
        }
        else {
            this.setState({ isFrequencyTypeMonthly: true, frequency: this.localize("EveryMonth") });
        }
    }

    /**
	*Sets group name state.
	*@param title Title string
	*/
    private onGroupNameChange = (value: string) => {
        this.setState({ groupName: value, isGroupNamePresent: true });
    }

    /**
   *Sets group description state.
   *@param description Description string
   */
    private onGroupDescriptionChange = (description: string) => {
        this.setState({ groupDescription: description, isGroupDescriptionPresent: true });
    }

    /**
    *Sets group link state.
    *@param event object
    */
    private onGroupLinkChange = (event: any) => {
        let url = event.target.value.toLowerCase();
        if (!((url === "") || (url.startsWith("https://")))) {
            this.setState({
                isGroupLinkValid: false, isGroupLinkPresent: true
            });
        }
        else {
            this.setState({
                groupLink: event.target.value, isGroupLinkValid: true
            });
        }
    }

    /**
	*Sets team name state.
	*@param title Title string
	*/
    private onTeamNameChange = (value: string) => {
        this.setState({ teamName: value, isTeamNamePresent: true });
    }

    /**
   *Sets team description state.
   *@param description Description string
   */
    private onTeamDescriptionChange = (description: string) => {
        this.setState({ teamDescription: description, isTeamDescriptionPresent: true });
    }

    /**
    *Sets team link state.
    *@param event object
    */
    private onTeamLinkChange = async (event: any) => {
        let url = event.target.value.toLowerCase();
        if (!((url === "") || (url.startsWith("https://teams.microsoft.com/l/team")))) {
            this.setState({
                isTeamLinkValid: false, isTeamLinkPresent: true
            });
        }
        else {
            this.setState({
                teamLink: event.target.value, isTeamLinkValid: true
            });

            // Get groupId from team link
            try {
                var params = url.split("?")[1];
                let groupId = params.split("&")[0];
                let response = await getTeamDetails(groupId.split("=")[1]);
                if (response.status === 200 && response.data) {
                    this.setState({
                        teamName: response.data.name, teamDescription: response.data.description, errorMessage: ""
                    });
                }
                else {
                    this.setState({ submitLoading: false, errorMessage: this.localize('TeamNotExists') });
                }
            }
            catch (error) {
                if (error.response.status === 400 || error.response.status === 404) {
                    this.setState({ submitLoading: false, errorMessage: this.localize('TeamNotExists') });
                }
                else if (error.response.status === 403) {
                    this.setState({ submitLoading: false, errorMessage: this.localize('ForbiddenSubmitGroupErrorMessage') });
                }
                else {
                    this.setState({ submitLoading: false, errorMessage: this.localize('GeneralErrorMessage') });
                }
            }
        }
    }

    /**
    *Sets image link state.
    *@param event object
    */
    private onImageLinkChange = (event: any) => {
        let url = event.target.value.toLowerCase();
        if (!((url === "") || (url.startsWith("https://") || (url.startsWith("data:image/png;base64,")) || (url.startsWith("data:image/jpeg;base64,")) || (url.startsWith("data:image/gif;base64,"))))) {
            this.setState({
                isImageLinkValid: false, isImageLinkPresent: true
            });
        }
        else {
            this.setState({
                imageLink: event.target.value, isImageLinkValid: true
            });
        }
    }

    /**
	*Sets location state.
	*@param title Title string
	*/
    private onLocationChange = (value: string) => {
        this.setState({ location: value, isLocationPresent: true });
    }

    /**
     * Handling check box change event.
     * @param isChecked | boolean value.
     */
    private onSearchEnableChange = (isChecked: boolean): void => {
        this.setState({ searchEnabled: !isChecked });
    }

    /**
     * Handling check box change event.
     * @param isChecked | boolean value.
     */
    private onProfileMatchEnableChange = (isChecked: boolean): void => {
        this.setState({ isProfileMatchingEnabled: !isChecked });
    }

    /**
   *Returns text component containing error message for failed name field validation
   *@param {boolean} isValuePresent Indicates whether value is present
   */
    private getRequiredFieldError = (isValuePresent: boolean) => {
        if (!isValuePresent) {
            return (<Text data-testid="empty_validation" content={this.localize('RequiredFieldMessage')} error size="small" />);
        }

        return (<></>);
    }

    /**
   *Returns text component containing error message for failed name field validation
   *@param {boolean} isValuePresent Indicates whether value is present
   */
    private getInvalidUrlError = (isValuePresent: boolean) => {
        if (!isValuePresent) {
            return (<Text data-testid="url_validation" content={this.localize('InvalidUrlMessage')} error size="small" />);
        }

        return (<></>);
    }

    /**
     * Renders the component.
    */
    public render(): JSX.Element {
        if (!this.state.loading) {
            return (
                <div className={this.state.theme === "default" ? "backgroundcolor" : "dark"} >
                    <Flex className="tab-container" column>
                        <Flex className="top-padding">
                            <Text data-testid="group_type_field" size="small" content={this.localize("GroupType")} />
                            <Flex.Item push>
                                {this.getRequiredFieldError(this.state.isGroupTypePresent)}
                            </Flex.Item>
                        </Flex>
                        <Dropdown
                            className="between-space"
                            fluid
                            placeholder={this.localize("GroupTypePlaceholder")}
                            items={[
                                this.localize("Teams"),
                                this.localize("External"),
                            ]}
                            value={this.state.groupType}
                            onChange={this.onGroupTypeChange}
                            data-testid="group_type_dropdown"
                        />
                        {this.state.isExternalSelected && <div>
                            < Flex className="top-padding">
                                <Text data-testid="group_name_field" size="small" content={this.localize("GroupName")} />
                                <Flex.Item push>
                                    {this.getRequiredFieldError(this.state.isGroupNamePresent)}
                                </Flex.Item>
                            </Flex>
                            <Input
                                className="between-space"
                                maxLength={Constants.maxLengthName}
                                fluid
                                placeholder={this.localize("GroupNamePlaceholder")}
                                value={this.state.groupName}
                                onChange={(event: any) => this.onGroupNameChange(event.target.value)}
                            />
                            <Flex className="top-padding">
                                <Text data-testid="group_description_field" size="small" content={this.localize("GroupDescription")} />
                                <Flex.Item push>
                                    {this.getRequiredFieldError(this.state.isGroupDescriptionPresent)}
                                </Flex.Item>
                            </Flex>
                            <Input
                                className="between-space"
                                maxLength={Constants.maxLengthDescription}
                                fluid
                                placeholder={this.localize("GroupDescriptionPlaceholder")}
                                value={this.state.groupDescription}
                                onChange={(event: any) => this.onGroupDescriptionChange(event.target.value)}
                            />
                            <Flex className="top-padding">
                                <Text data-testid="group_link_field" size="small" content={this.localize("GroupLink")} />
                                <Flex.Item push>
                                    {this.getRequiredFieldError(this.state.isGroupLinkPresent)}
                                </Flex.Item>
                                <Flex.Item push>
                                    {this.getInvalidUrlError(this.state.isGroupLinkValid)}
                                </Flex.Item>
                            </Flex>
                            <Input
                                className="between-space"
                                fluid
                                placeholder={this.localize("GroupLinkPlaceholder")}
                                value={this.state.groupLink}
                                onChange={this.onGroupLinkChange}
                            />
                        </div>}
                        {this.state.isTeamsSelected && <div>
                            <Flex className="top-padding">
                                <Text data-testid="team_link_field" size="small" content={this.localize("TeamLink")} />
                                <Flex.Item push>
                                    {this.getRequiredFieldError(this.state.isTeamLinkPresent)}
                                </Flex.Item>
                                <Flex.Item push>
                                    {this.getInvalidUrlError(this.state.isTeamLinkValid)}
                                </Flex.Item>
                            </Flex>
                            <Input
                                className="between-space"
                                fluid
                                placeholder={this.localize("TeamLinkPlaceholder")}
                                value={this.state.teamLink}
                                onChange={this.onTeamLinkChange}
                            />
                            < Flex className="top-padding">
                                <Text data-testid="team_name_field" size="small" content={this.localize("TeamName")} />
                                <Flex.Item push>
                                    {this.getRequiredFieldError(this.state.isTeamNamePresent)}
                                </Flex.Item>
                            </Flex>
                            <Input
                                className="between-space"
                                maxLength={Constants.maxLengthName}
                                fluid
                                placeholder={this.localize("TeamNamePlaceholder") + "..."}
                                value={this.state.teamName}
                                onChange={(event: any) => this.onTeamNameChange(event.target.value)}
                            />
                            <Flex className="top-padding">
                                <Text data-testid="team_description_field" size="small" content={this.localize("TeamDescription")} />
                                <Flex.Item push>
                                    {this.getRequiredFieldError(this.state.isTeamDescriptionPresent)}
                                </Flex.Item>
                            </Flex>
                            <Input
                                className="between-space"
                                maxLength={Constants.maxLengthDescription}
                                fluid
                                placeholder={this.localize("TeamDescriptionPlaceholder") + "..."}
                                value={this.state.teamDescription}
                                onChange={(event: any) => this.onTeamDescriptionChange(event.target.value)}
                            />
                        </div>}
                        <Flex className="top-padding">
                            <Text data-testid="image_link_field" size="small" content={this.localize("ImageLink")} />
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
                            placeholder={this.localize("ImageLinkPlaceholder")}
                            value={this.state.imageLink}
                            onChange={this.onImageLinkChange}
                            data-testid="image_url"
                        />
                        <div>
                            <Flex className="top-padding">
                                <Text data-testid="tags_field" size="small" content={this.localize("Tags")} />
                                <Flex.Item push>
                                    <div>
                                        {this.getTagError()}
                                    </div>
                                </Flex.Item>
                            </Flex>
                            <Input
                                className="between-space"
                                placeholder={this.localize("TagsPlaceholder")}
                                fluid
                                value={this.state.tag}
                                onKeyDown={this.onTagKeyUp}
                                onChange={(event: any) => this.onTagChange(event.target.value)}
                            />
                            <Flex>
                                <div>
                                    {
                                        this.state.tagsList!.map((value: string, index) => {
                                            if (value.trim().length > 0) {
                                                return (
                                                    <Label
                                                        circular
                                                        className={this.state.theme === "default" ? "tags-label-wrapper" : "tags-label-wrapper-dark"}
                                                        content={<Text content={value.trim()} title={value.trim()} size="medium" />}
                                                        icon={<CloseIcon outline key={index} onClick={() => this.onTagRemoveClick(index)} />}
                                                    />
                                                )
                                            }
                                        })
                                    }
                                </div>
                            </Flex>
                        </div>
                        <Flex className="top-padding">
                            <Text data-testid="location_field" size="small" content={this.localize("Location")} />
                            <Flex.Item push>
                                {this.getRequiredFieldError(this.state.isLocationPresent)}
                            </Flex.Item>
                        </Flex>
                        <Input
                            className="between-space"
                            maxLength={Constants.stateLocationMaxLength}
                            fluid
                            placeholder={this.localize("LocationPlaceholder")}
                            value={this.state.location}
                            onChange={(event: any) => this.onLocationChange(event.target.value)}
                        />
                        <Flex className="top-padding">
                            <Text data-testid="searchenabled_field" className="margin-space" content={this.localize("SearchEnabled")} />
                            <InfoIcon outline xSpacing="after" title={this.localize("ProfileMatchingInfo")} size="small" />
                            <Checkbox toggle checked={this.state.searchEnabled} onChange={() => this.onSearchEnableChange(this.state.searchEnabled)} />
                        </Flex>
                        {this.state.selectedGroupType === GroupType.teams && <>< Flex className="top-padding">
                            <Text data-testid="searchenabled_field" className="margin-space" content={this.localize("ProfileMatching")} />
                            <InfoIcon outline xSpacing="after" title={this.localize("TagInfo")} size="small" />
                            <Checkbox toggle checked={this.state.isProfileMatchingEnabled} onChange={() => this.onProfileMatchEnableChange(this.state.isProfileMatchingEnabled)} />
                        </Flex>
                            <Flex className="top-padding">
                                <Text data-testid="searchenabled_field" className="margin-space" content={this.localize("MatchFrequency")} />
                                <Dropdown aria-readonly={true} disabled={!this.state.isProfileMatchingEnabled}
                                    items={[
                                        this.localize("EveryWeek"),
                                        this.localize("EveryMonth"),
                                    ]}
                                    value={this.state.frequency}
                                    onChange={this.onFrequencyTypeChange}
                                    fluid
                                />
                            </Flex>
                        </>}
                    </Flex>

                    <Flex className="tab-footer" hAlign="end" >
                        <Flex.Item push>
                            <Text className="error-info" content={this.state.errorMessage} error size="small" />
                        </Flex.Item>
                        <Button primary content={this.localize("UpdateButtonText")}
                            onClick={this.handleSubmit}
                            disabled={this.state.submitLoading}
                            loading={this.state.submitLoading} data-testid="submit_button" />
                    </Flex>
                </div>
            )
        }
        else {
            return <Loader />
        }
    }
}

export default withTranslation()(UpdateResouceGroup)