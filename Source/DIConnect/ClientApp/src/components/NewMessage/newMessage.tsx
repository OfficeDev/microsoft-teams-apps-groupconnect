// <copyright file="newMessage.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { withTranslation, WithTranslation } from "react-i18next";
import { TextArea, Radiobutton, RadiobuttonGroup } from 'msteams-ui-components-react';
import { initializeIcons } from 'office-ui-fabric-react';
import * as AdaptiveCards from "adaptivecards";
import { Input, Button, Loader, Dropdown, Text, Flex, FlexItem, ChevronStartIcon, Checkbox, Datepicker } from '@fluentui/react-northstar';
import * as microsoftTeams from "@microsoft/teams-js";
import Resizer from 'react-image-file-resizer';
import './newMessage.scss';
import './teamTheme.scss';
import { getDraftNotification, getTeams, createDraftNotification, updateDraftNotification, searchGroups, getGroups, verifyGroupAccess } from '../../apis/messageListApi';
import {
    getInitAdaptiveCard, setCardTitle, setCardImageLink, setCardSummary,
    setCardAuthor, setCardBtn
} from '../AdaptiveCard/adaptiveCard';
import { getBaseUrl } from '../../configVariables';
import { ImageUtil } from '../../utility/imageutility';
import { TFunction } from "i18next";

// hours that will be presented in the combo box for the scheduled messages option
const hours = ["00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11",
    "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23",
];

// minutes that will be presented in the combo box for the scheduled messages option
const minutes = ["00", "05", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55",
];

// coefficient to calculate scheduling times
const coeff = 1000 * 60 * 5;

type dropdownItem = {
    key: string,
    header: string,
    content: string,
    image: string,
    team: {
        id: string,
    },
}

export interface IDraftMessage {
    id?: string,
    title: string,
    imageLink?: string,
    summary?: string,
    author: string,
    buttonTitle?: string,
    buttonLink?: string,
    teams: any[],
    rosters: any[],
    groups: any[],
    allUsers: boolean,
    isScheduled: boolean, //property to flag if the message is scheduled
    ScheduledDate: Date //message scheduled date
}

export interface formState {
    title: string,
    summary?: string,
    btnLink?: string,
    imageLink?: string,
    btnTitle?: string,
    author: string,
    card?: any,
    page: string,
    teamsOptionSelected: boolean,
    rostersOptionSelected: boolean,
    allUsersOptionSelected: boolean,
    groupsOptionSelected: boolean,
    teams?: any[],
    groups?: any[],
    exists?: boolean,
    messageId: string,
    loader: boolean,
    groupAccess: boolean,
    loading: boolean,
    disableButton: boolean,
    noResultMessage: string,
    unstablePinned?: boolean,
    selectedTeamsNum: number,
    selectedRostersNum: number,
    selectedGroupsNum: number,
    selectedRadioBtn: string,
    selectedTeams: dropdownItem[],
    selectedRosters: dropdownItem[],
    selectedGroups: dropdownItem[],
    errorImageUrlMessage: string,
    errorButtonUrlMessage: string,
    selectedSchedule: boolean, //checkbox status for the scheduled message
    scheduledDate: string, //scheduled date in string format for the page state
    DMY: Date, //scheduled daste in date format for the page state
    DMYHour: string, //hour selected in the combo
    DMYMins: string, //mins selected in the combo
    futuredate: boolean, //flag is the date is in the future 
    disableImageUrl: boolean
}

export interface INewMessageProps extends RouteComponentProps, WithTranslation {
    getDraftMessagesList?: any;
}

class NewMessage extends React.Component<INewMessageProps, formState> {
    readonly localize: TFunction;
    private card: any;
    history: any;
    fileInput: any;

    constructor(props: INewMessageProps) {
        super(props);
        initializeIcons();
        this.localize = this.props.t;
        this.card = getInitAdaptiveCard(this.localize);
        this.setDefaultCard(this.card);
        var TempDate = this.getRoundedDate(5, this.getDateObject()); //get the current date

        this.state = {
            title: "",
            summary: "",
            author: "",
            btnLink: "",
            imageLink: "",
            btnTitle: "",
            card: this.card,
            page: "CardCreation",
            teamsOptionSelected: true,
            rostersOptionSelected: false,
            allUsersOptionSelected: false,
            groupsOptionSelected: false,
            messageId: "",
            loader: true,
            groupAccess: false,
            loading: false,
            disableButton: false,
            noResultMessage: "",
            unstablePinned: true,
            selectedTeamsNum: 0,
            selectedRostersNum: 0,
            selectedGroupsNum: 0,
            selectedRadioBtn: "teams",
            selectedTeams: [],
            selectedRosters: [],
            selectedGroups: [],
            errorImageUrlMessage: "",
            errorButtonUrlMessage: "",
            selectedSchedule: false, //by default the message is not scheduled
            scheduledDate: TempDate.toUTCString(), //current date in UTC string format
            DMY: TempDate, //current date in date format
            DMYHour: this.getDateHour(TempDate.toUTCString()), //initialize with the current hour (rounded up)
            DMYMins: this.getDateMins(TempDate.toUTCString()), //initialize with the current minute (rounded up)
            futuredate: false, //by default it is not a future date
            disableImageUrl: false 
        }
        this.fileInput = React.createRef();
        this.handleImageSelection = this.handleImageSelection.bind(this);
        this.history = props.history;
    }

    public async componentDidMount() {
        microsoftTeams.initialize();
        //- Handle the Esc key
        document.addEventListener("keydown", this.escFunction, false);
        let params = this.props.match.params;
        this.setGroupAccess();
        this.getTeamList().then(() => {
            if ('id' in params) {
                let id = params['id'];
                this.getItem(id).then(() => {
                    const selectedTeams = this.makeDropdownItemList(this.state.selectedTeams, this.state.teams);
                    const selectedRosters = this.makeDropdownItemList(this.state.selectedRosters, this.state.teams);
                    this.setState({
                        exists: true,
                        messageId: id,
                        selectedTeams: selectedTeams,
                        selectedRosters: selectedRosters,
                        selectedSchedule: this.state.selectedSchedule,
                        scheduledDate: this.state.scheduledDate,
                        DMY: this.getDateObject(this.state.scheduledDate),
                        DMYHour: this.getDateHour(this.state.scheduledDate),
                        DMYMins: this.getDateMins(this.state.scheduledDate)
                    })
                });
                this.getGroupData(id).then(() => {
                    const selectedGroups = this.makeDropdownItems(this.state.groups);
                    this.setState({
                        selectedGroups: selectedGroups
                    })
                });
            } else {
                this.setState({
                    exists: false,
                    loader: false
                }, () => {
                    let adaptiveCard = new AdaptiveCards.AdaptiveCard();
                    adaptiveCard.parse(this.state.card);
                    let renderedCard = adaptiveCard.render();
                    document.getElementsByClassName('adaptiveCardContainer')[0].appendChild(renderedCard);
                    if (this.state.btnLink) {
                        let link = this.state.btnLink;
                        adaptiveCard.onExecuteAction = function (action) { window.open(link, '_blank'); };
                    }
                })
            }
        });
    }

    //function to handle the selection of the OS file upload box
    private handleImageSelection() {
        //get the first file selected
        const file = this.fileInput.current.files[0];
        if (file) { //if we have a file
            //resize the image to fit in the adaptivecard
            Resizer.imageFileResizer(file, 400, 400, 'JPEG', 80, 0,
                uri => {
                    if (uri.toString().length < 32768) {
                        //everything is ok with the image, lets set it on the card and update
                        setCardImageLink(this.card, uri.toString());
                        this.updateCard();
                        //lets set the state with the image value
                        this.setState({
                            imageLink: uri.toString()
                        }
                        );
                    } else {
                        //images bigger than 32K cannot be saved, set the error message to be presented
                        this.setState({
                            errorImageUrlMessage: this.localize("ErrorImageTooBig")
                        });
                    }

                },
                'base64'); //we need the image in base64
        }
    }

    //Function calling a click event on a hidden file input
    private handleUploadClick = (event: any) => {
        //reset the error message and the image link as the upload will reset them potentially
        this.setState({
            errorImageUrlMessage: "",
            imageLink: ""
        });
        //fire the fileinput click event and run the handleimageselection function
        this.fileInput.current.click();
    };

    private makeDropdownItems = (items: any[] | undefined) => {
        const resultedTeams: dropdownItem[] = [];
        if (items) {
            items.map((element) => {
                resultedTeams.push({
                    key: element.id,
                    header: element.name,
                    content: element.mail,
                    image: ImageUtil.makeInitialImage(element.name),
                    team: {
                        id: element.id
                    },

                });
            });
        }
        return resultedTeams;
    }

    private makeDropdownItemList = (items: any[], fromItems: any[] | undefined) => {
        const dropdownItemList: dropdownItem[] = [];
        items.map(element =>
            dropdownItemList.push(
                typeof element !== "string" ? element : {
                    key: fromItems!.find(x => x.id === element).id,
                    header: fromItems!.find(x => x.id === element).name,
                    image: ImageUtil.makeInitialImage(fromItems!.find(x => x.id === element).name),
                    team: {
                        id: element
                    }
                })
        );
        return dropdownItemList;
    }

    public setDefaultCard = (card: any) => {
        const titleAsString = this.localize("TitleText");
        const summaryAsString = this.localize("Summary");
        const authorAsString = this.localize("Author1");
        const buttonTitleAsString = this.localize("ButtonTitle");

        setCardTitle(card, titleAsString);
        let imgUrl = getBaseUrl() + "/image/imagePlaceholder.png";
        setCardImageLink(card, imgUrl);
        setCardSummary(card, summaryAsString);
        setCardAuthor(card, authorAsString);
        setCardBtn(card, buttonTitleAsString, "https://adaptivecards.io");
    }

    private getTeamList = async () => {
        try {
            const response = await getTeams();
            this.setState({
                teams: response.data
            });
        } catch (error) {
            return error;
        }
    }

    private getGroupItems() {
        if (this.state.groups) {
            return this.makeDropdownItems(this.state.groups);
        }
        const dropdownItems: dropdownItem[] = [];
        return dropdownItems;
    }

    private setGroupAccess = async () => {
        await verifyGroupAccess().then(() => {
            this.setState({
                groupAccess: true
            });
        }).catch((error) => {
            const errorStatus = error.response.status;
            if (errorStatus === 403) {
                this.setState({
                    groupAccess: false
                });
            }
            else {
                throw error;
            }
        });
    }

    private getGroupData = async (id: number) => {
        try {
            const response = await getGroups(id);
            this.setState({
                groups: response.data
            });
        }
        catch (error) {
            return error;
        }
    }

    private getItem = async (id: number) => {
        try {
            const response = await getDraftNotification(id);
            const draftMessageDetail = response.data;
            let selectedRadioButton = "teams";
            if (draftMessageDetail.rosters.length > 0) {
                selectedRadioButton = "rosters";
            }
            else if (draftMessageDetail.groups.length > 0) {
                selectedRadioButton = "groups";
            }
            else if (draftMessageDetail.allUsers) {
                selectedRadioButton = "allUsers";
            }
            this.setState({
                teamsOptionSelected: draftMessageDetail.teams.length > 0,
                selectedTeamsNum: draftMessageDetail.teams.length,
                rostersOptionSelected: draftMessageDetail.rosters.length > 0,
                selectedRostersNum: draftMessageDetail.rosters.length,
                groupsOptionSelected: draftMessageDetail.groups.length > 0,
                selectedGroupsNum: draftMessageDetail.groups.length,
                selectedRadioBtn: selectedRadioButton,
                selectedTeams: draftMessageDetail.teams,
                selectedRosters: draftMessageDetail.rosters,
                selectedGroups: draftMessageDetail.groups,
                selectedSchedule: draftMessageDetail.isScheduled,
                scheduledDate: draftMessageDetail.scheduledDate
            });

            setCardTitle(this.card, draftMessageDetail.title);
            setCardImageLink(this.card, draftMessageDetail.imageLink);
            setCardSummary(this.card, draftMessageDetail.summary);
            setCardAuthor(this.card, draftMessageDetail.author);
            setCardBtn(this.card, draftMessageDetail.buttonTitle, draftMessageDetail.buttonLink);

            this.setState({
                title: draftMessageDetail.title,
                summary: draftMessageDetail.summary,
                btnLink: draftMessageDetail.buttonLink,
                imageLink: draftMessageDetail.imageLink,
                btnTitle: draftMessageDetail.buttonTitle,
                author: draftMessageDetail.author,
                allUsersOptionSelected: draftMessageDetail.allUsers,
                loader: false
            }, () => {
                this.updateCard();
            });
        } catch (error) {
            return error;
        }
    }

    public componentWillUnmount() {
        document.removeEventListener("keydown", this.escFunction, false);
    }

    public render(): JSX.Element {
        if (this.state.loader) {
            return (
                <div className="Loader">
                    <Loader />
                </div>
            );
        } else {
            if (this.state.page === "CardCreation") {
                return (
                    <div className="taskModule">
                        <div className="formContainer">
                            <div className="formContentContainer" >
                                <Flex>
                                <Input
                                    className="inputField"
                                    value={this.state.title}
                                    label={this.localize("TitleText")}
                                    placeholder={this.localize("PlaceHolderTitle")}
                                    onChange={this.onTitleChanged}
                                    autoComplete="off"
                                    required
                                    fluid
                                    />
                                </Flex>
                                <Flex gap="gap.smaller" vAlign="end" className="inputField">
                                    <Input
                                        label={this.localize("ImageURL")}
                                        placeholder={this.localize("ImageLinkPlaceholder")}
                                        value={this.state.imageLink}
                                        onChange={this.onImageLinkChanged}
                                        autoComplete="off"
                                        data-testid="image_url"
                                        error={this.state.errorImageUrlMessage !== ""}
                                        fluid
                                        />
                                    <Flex.Item push>
                                        <Button onClick={this.handleUploadClick}
                                            text
                                            size="small"
                                            content={this.localize("UploadImage")}
                                        />
                                    </Flex.Item>
                                    <input type="file" accept="image/"
                                        style={{ display: 'none' }}
                                        onChange={this.handleImageSelection}
                                        ref={this.fileInput} />
                                </Flex>
                                <Flex className="inputField">
                                <Text className={(this.state.errorImageUrlMessage === "") ? "hide" : "show"} error size="small" content={this.state.errorImageUrlMessage} />
                                </Flex>
                                <TextArea
                                    className="inputField textArea"
                                    autoFocus
                                    placeholder={this.localize("Summary")}
                                    label={this.localize("SummaryPlaceHolder")}
                                    value={this.state.summary}
                                    onChange={this.onSummaryChanged}
                                />
                                <Flex>
                                <Input
                                    className="inputField"
                                    value={this.state.author}
                                    label={this.localize("Author")}
                                    placeholder={this.localize("AuthorPlaceHolder")}
                                    onChange={this.onAuthorChanged}
                                        autoComplete="off"
                                        fluid
                                />
                                </Flex>
                                <Flex>
                                <Input
                                    className="inputField"
                                    value={this.state.btnTitle}
                                    label={this.localize("ButtonTitle")}
                                    placeholder={this.localize("ButtonTitlePlaceHolder")}
                                    onChange={this.onBtnTitleChanged}
                                    autoComplete="off"
                                    fluid
                                />
                                </Flex>
                                <Flex>
                                <Input
                                    className="inputField"
                                    value={this.state.btnLink}
                                    label={this.localize("ButtonURL")}
                                    placeholder={this.localize("ButtonURLPlaceHolder")}
                                    onChange={this.onBtnLinkChanged}
                                    error={this.state.errorButtonUrlMessage !== ""}
                                    fluid
                                    />
                                </Flex>
                            </div>
                            <div className="adaptiveCardContainer">
                            </div>
                        </div>

                        <div className="footerContainer">
                            <div className="buttonContainer">
                                <Button content={this.localize("Next")} disabled={this.isNextBtnDisabled()} id="saveBtn" onClick={this.onNext} primary />
                            </div>
                        </div>
                    </div>
                );
            }
            else if (this.state.page === "AudienceSelection") {
                return (
                    <div className="taskModule">
                        <div className="formContainer">
                            <div className="formContentContainer" >
                                <h3>{this.localize("SendHeadingText")}</h3>
                                <RadiobuttonGroup
                                    className="radioBtns"
                                    value={this.state.selectedRadioBtn}
                                    onSelected={this.onGroupSelected}
                                >
                                    <Radiobutton className="bottom-space" name="grouped" value="rosters" label={this.localize("SendToRosters")} />
                                    {this.state.rostersOptionSelected &&
                                        <Dropdown
                                            className="bottom-space"
                                            placeholder={this.localize("SendToRostersPlaceHolder")}
                                            search
                                            multiple
                                            items={this.getItems()}
                                            value={this.state.selectedRosters}
                                            onChange={this.onRostersChange}
                                            unstable_pinned={this.state.unstablePinned}
                                            noResultsMessage={this.localize("NoMatchMessage")}
                                        />
                                    }
                                    <Radiobutton className="bottom-space" name="grouped" value="groups" label={this.localize("SendToGroups")} />
                                    <div className={this.state.groupsOptionSelected && !this.state.groupAccess ? "" : "hide"}>
                                        <div className="noteText">
                                            <Text error content={this.localize("SendToGroupsPermissionNote")} />
                                        </div>
                                    </div>
                                    {(this.state.groupsOptionSelected && this.state.groupAccess) &&
                                        <Dropdown
                                            className="hideToggle"
                                            placeholder={this.localize("SendToGroupsPlaceHolder")}
                                            search={this.onGroupSearch}
                                            multiple
                                            loading={this.state.loading}
                                            loadingMessage={this.localize("LoadingText")}
                                            items={this.getGroupItems()}
                                            value={this.state.selectedGroups}
                                            onSearchQueryChange={this.onGroupSearchQueryChange}
                                            onChange={this.onGroupsChange}
                                            noResultsMessage={this.state.noResultMessage}
                                            unstable_pinned={this.state.unstablePinned}
                                        />
                                    }
                                    <div className={this.state.groupsOptionSelected && this.state.groupAccess ? "" : "hide"}>
                                        <div className="noteText">
                                            <Text error content={this.localize("SendToGroupsNote")} />
                                        </div>
                                    </div>
                                </RadiobuttonGroup>
                                <h3>
                                    <Checkbox
                                        labelPosition="start"
                                        onClick={this.onScheduleSelected}
                                        label={this.localize("ScheduledSend")}
                                        checked={this.state.selectedSchedule}
                                        toggle
                                    />
                                </h3>
                                <div className="DateTimeSelector">
                                    <div className="noteText">
                                        <Text size="small" content={this.localize("ScheduledSendDescription")} />
                                    </div>
                                </div>
                                <Flex gap="gap.smaller" className="DateTimeSelector">
                                    <Datepicker
                                        disabled={!this.state.selectedSchedule}
                                        defaultSelectedDate={this.getDateObject(this.state.scheduledDate)}
                                        minDate={new Date()}
                                        inputOnly
                                        onDateChange={this.handleDateChange}
                                    />
                                    <FlexItem shrink={true} size="1%">
                                        <Dropdown
                                            placeholder="hour"
                                            disabled={!this.state.selectedSchedule}
                                            fluid={true}
                                            items={hours}
                                            defaultValue={this.getDateHour(this.state.scheduledDate)}
                                            onChange={this.handleHourChange}
                                        />
                                    </FlexItem>
                                    <FlexItem shrink={true} size="1%">
                                        <Dropdown
                                            placeholder="mins"
                                            disabled={!this.state.selectedSchedule}
                                            fluid={true}
                                            items={minutes}
                                            defaultValue={this.getDateMins(this.state.scheduledDate)}
                                            onChange={this.handleMinsChange}
                                        />
                                    </FlexItem>
                                </Flex>
                                <div className={this.state.futuredate && this.state.selectedSchedule ? "ErrorMessage" : "hide"}>
                                    <div className="noteText">
                                        <Text error content={this.localize('FutureDateError')} />
                                    </div>
                                </div>
                            </div>
                            <div className="adaptiveCardContainer">
                            </div>
                        </div>

                        <div className="footerContainer">
                            <Flex space="between">
                                <Button icon={<ChevronStartIcon xSpacing="before" />} content={this.localize("Back")} text onClick={this.onBack} />
                                <Flex className="buttonContainer">
                                    <Button
                                        content="Schedule"
                                        disabled={!this.state.selectedSchedule || this.state.disableButton || this.isBtnDisabled()}
                                        onClick={this.onSchedule}
                                        primary={this.state.selectedSchedule} />
                                    <Button
                                        content={this.localize("SaveAsDraft")}
                                        disabled={this.isBtnDisabled() || this.state.disableButton || this.state.selectedSchedule}
                                        id="saveBtn"
                                        onClick={this.onSave}
                                        secondary />
                                    <Button
                                        content={this.localize("PublishButtonText")}
                                        disabled={this.isBtnDisabled() || this.state.disableButton || this.state.selectedSchedule}
                                        id="saveBtn"
                                        onClick={() => this.onPublish()}
                                        primary={!this.state.selectedSchedule} />
                                </Flex>
                            </Flex>
                        </div>
                    </div>
                );
            } else {
                return (<div>Error</div>);
            }
        }
    }

    //get the next rounded up (ceil) date in minutes
    private getRoundedDate = (minutes: number, d = new Date()) => {

        let ms = 1000 * 60 * minutes; // convert minutes to ms
        let roundedDate = new Date(Math.ceil(d.getTime() / ms) * ms);

        return roundedDate
    }

    //get date object based on the string parameter
    private getDateObject = (datestring?: string) => {
        if (!datestring) {
            var TempDate = new Date(); //get current date
            TempDate.setTime(TempDate.getTime() + 86400000);
            return TempDate; //if date string is not provided, then return tomorrow rounded up next 5 minutes
        }
        return new Date(datestring); //if date string is provided, return current date object
    }

    private getDateHour = (datestring: string) => {
        if (!datestring) return "00";
        var thour = new Date(datestring).getHours().toString();
        return thour.padStart(2, "0");
    }

    private getDateMins = (datestring: string) => {
        if (!datestring) return "00";
        var tmins = new Date(datestring).getMinutes().toString();
        return tmins.padStart(2, "0");
    }

    //handles click on DatePicker to change the schedule date
    private handleDateChange = (e: any, v: any) => {
        var TempDate = v.value; //set the tempdate var with the value selected by the user
        TempDate.setMinutes(parseInt(this.state.DMYMins)); //set the minutes selected on minutes drop down 
        TempDate.setHours(parseInt(this.state.DMYHour)); //set the hour selected on hour drop down
        //set the state variables
        this.setState({
            scheduledDate: TempDate.toUTCString(), //updates the state string representation
            DMY: TempDate, //updates the date on the state
        });
    }

    //handles selection on the hour combo
    private handleHourChange = (e: any, v: any) => {
        var TempDate = this.state.DMY; //get the tempdate from the state
        TempDate.setHours(parseInt(v.value)); //set hour with the value select on the hour drop down
        //set state variables
        this.setState({
            scheduledDate: TempDate.toUTCString(), //updates the string representation 
            DMY: TempDate, //updates DMY
            DMYHour: v.value, //set the new hour value on the state
        });
    }

    //handles selction on the minute combo
    private handleMinsChange = (e: any, v: any) => {
        var TempDate = this.state.DMY;
        TempDate.setMinutes(parseInt(v.value));
        this.setState({
            scheduledDate: TempDate.toUTCString(),
            DMY: TempDate,
            DMYMins: v.value,
        });
    }

    private onGroupSelected = (value: any) => {
        this.setState({
            selectedRadioBtn: value,
            teamsOptionSelected: value === 'teams',
            rostersOptionSelected: value === 'rosters',
            groupsOptionSelected: value === 'groups',
            allUsersOptionSelected: value === 'allUsers',
            selectedTeams: value === 'teams' ? this.state.selectedTeams : [],
            selectedTeamsNum: value === 'teams' ? this.state.selectedTeamsNum : 0,
            selectedRosters: value === 'rosters' ? this.state.selectedRosters : [],
            selectedRostersNum: value === 'rosters' ? this.state.selectedRostersNum : 0,
            selectedGroups: value === 'groups' ? this.state.selectedGroups : [],
            selectedGroupsNum: value === 'groups' ? this.state.selectedGroupsNum : 0,
        });
    }

    //handler for the Schedule Send checkbox
    private onScheduleSelected = () => {
        var TempDate = this.getRoundedDate(5, this.getDateObject()); //get the next day date rounded to the nearest hour/minute
        //set the state
        this.setState({
            selectedSchedule: !this.state.selectedSchedule,
            scheduledDate: TempDate.toUTCString(),
            DMY: TempDate
        });
    }

    private isBtnDisabled = () => {
        const teamsSelectionIsValid = (this.state.teamsOptionSelected && (this.state.selectedTeamsNum !== 0)) || (!this.state.teamsOptionSelected);
        const rostersSelectionIsValid = (this.state.rostersOptionSelected && (this.state.selectedRostersNum !== 0)) || (!this.state.rostersOptionSelected);
        const groupsSelectionIsValid = (this.state.groupsOptionSelected && (this.state.selectedGroupsNum !== 0)) || (!this.state.groupsOptionSelected);
        const nothingSelected = (!this.state.teamsOptionSelected) && (!this.state.rostersOptionSelected) && (!this.state.groupsOptionSelected) && (!this.state.allUsersOptionSelected);
        return (!teamsSelectionIsValid || !rostersSelectionIsValid || !groupsSelectionIsValid || nothingSelected)
    }

    private isNextBtnDisabled = () => {
        const title = this.state.title;
        const btnTitle = this.state.btnTitle;
        const btnLink = this.state.btnLink;
        return !(title && ((btnTitle && btnLink) || (!btnTitle && !btnLink)) && (this.state.errorImageUrlMessage === "") && (this.state.errorButtonUrlMessage === ""));
    }

    private getItems = () => {
        const resultedTeams: dropdownItem[] = [];
        if (this.state.teams) {
            let remainingUserTeams = this.state.teams;
            if (this.state.selectedRadioBtn !== "allUsers") {
                if (this.state.selectedRadioBtn === "teams") {
                    this.state.teams.filter(x => this.state.selectedTeams.findIndex(y => y.team.id === x.id) < 0);
                }
                else if (this.state.selectedRadioBtn === "rosters") {
                    this.state.teams.filter(x => this.state.selectedRosters.findIndex(y => y.team.id === x.id) < 0);
                }
            }
            remainingUserTeams.map((element) => {
                resultedTeams.push({
                    key: element.id,
                    header: element.name,
                    content: element.mail,
                    image: ImageUtil.makeInitialImage(element.name),
                    team: {
                        id: element.id
                    }
                });
            });
        }
        return resultedTeams;
    }

    private static MAX_SELECTED_TEAMS_NUM: number = 20;

    private onTeamsChange = (event: any, itemsData: any) => {
        if (itemsData.value.length > NewMessage.MAX_SELECTED_TEAMS_NUM) return;
        this.setState({
            selectedTeams: itemsData.value,
            selectedTeamsNum: itemsData.value.length,
            selectedRosters: [],
            selectedRostersNum: 0,
            selectedGroups: [],
            selectedGroupsNum: 0
        })
    }

    private onRostersChange = (event: any, itemsData: any) => {
        if (itemsData.value.length > NewMessage.MAX_SELECTED_TEAMS_NUM) return;
        this.setState({
            selectedRosters: itemsData.value,
            selectedRostersNum: itemsData.value.length,
            selectedTeams: [],
            selectedTeamsNum: 0,
            selectedGroups: [],
            selectedGroupsNum: 0
        })
    }

    private onGroupsChange = (event: any, itemsData: any) => {
        this.setState({
            selectedGroups: itemsData.value,
            selectedGroupsNum: itemsData.value.length,
            groups: [],
            selectedTeams: [],
            selectedTeamsNum: 0,
            selectedRosters: [],
            selectedRostersNum: 0
        })
    }

    private onGroupSearch = (itemList: any, searchQuery: string) => {
        const result = itemList.filter(
            (item: { header: string; content: string; }) => (item.header && item.header.toLowerCase().indexOf(searchQuery.toLowerCase()) !== -1) ||
                (item.content && item.content.toLowerCase().indexOf(searchQuery.toLowerCase()) !== -1),
        )
        return result;
    }

    private onGroupSearchQueryChange = async (event: any, itemsData: any) => {

        if (!itemsData.searchQuery) {
            this.setState({
                groups: [],
                noResultMessage: "",
            });
        }
        else if (itemsData.searchQuery && itemsData.searchQuery.length <= 2) {
            this.setState({
                loading: false,
                noResultMessage: this.localize("NoResultMessage"),
            });
        }
        else if (itemsData.searchQuery && itemsData.searchQuery.length > 2) {
            // handle event trigger on item select.
            const result = itemsData.items && itemsData.items.find(
                (item: { header: string; }) => item.header.toLowerCase() === itemsData.searchQuery.toLowerCase()
            )
            if (result) {
                return;
            }

            this.setState({
                loading: true,
                noResultMessage: "",
            });

            try {
                const query = encodeURIComponent(itemsData.searchQuery);
                const response = await searchGroups(query);
                this.setState({
                    groups: response.data,
                    loading: false,
                    noResultMessage: this.localize("NoResultMessage")
                });
            }
            catch (error) {
                return error;
            }
        }
    }

    private async onPublish() {
        this.setState({ disableButton: true });
        const selectedTeams: string[] = [];
        const selctedRosters: string[] = [];
        const selectedGroups: string[] = [];
        this.state.selectedTeams.map(x => selectedTeams.push(x.team.id));
        this.state.selectedRosters.map(x => selctedRosters.push(x.team.id));
        this.state.selectedGroups.map(x => selectedGroups.push(x.team.id));

        const draftMessage: IDraftMessage = {
            id: this.state.messageId,
            title: this.state.title,
            imageLink: this.state.imageLink,
            summary: this.state.summary,
            author: this.state.author,
            buttonTitle: this.state.btnTitle,
            buttonLink: this.state.btnLink,
            teams: selectedTeams,
            rosters: selctedRosters,
            groups: selectedGroups,
            allUsers: this.state.allUsersOptionSelected,
            isScheduled: false,
            ScheduledDate: new Date(this.state.scheduledDate)
        };

        let messageId;
        if (this.state.exists) {
            this.editDraftMessage(draftMessage);
            messageId = this.state.messageId;
        } else {
            messageId = await this.postDraftMessage(draftMessage);
        }

        this.history.push('/sendconfirmation/' + messageId);
    }

    // handler for the schedule button
    private onSchedule = () => {
        var Today = new Date();
        var Scheduled = new Date(this.state.scheduledDate);

        if (Scheduled.getTime() > Today.getTime() + 1800000) { this.onSave() }
        else {
            this.setState({
                futuredate: true
            })
        }
    }


    private onSave = () => {
        this.setState({ disableButton: true });
        const selectedTeams: string[] = [];
        const selctedRosters: string[] = [];
        const selectedGroups: string[] = [];
        this.state.selectedTeams.map(x => selectedTeams.push(x.team.id));
        this.state.selectedRosters.map(x => selctedRosters.push(x.team.id));
        this.state.selectedGroups.map(x => selectedGroups.push(x.team.id));

        const draftMessage: IDraftMessage = {
            id: this.state.messageId,
            title: this.state.title,
            imageLink: this.state.imageLink,
            summary: this.state.summary,
            author: this.state.author,
            buttonTitle: this.state.btnTitle,
            buttonLink: this.state.btnLink,
            teams: selectedTeams,
            rosters: selctedRosters,
            groups: selectedGroups,
            allUsers: this.state.allUsersOptionSelected,
            isScheduled: this.state.selectedSchedule,
            ScheduledDate: new Date(this.state.scheduledDate)
        };

        if (this.state.exists) {
            this.editDraftMessage(draftMessage).then(() => {
                microsoftTeams.tasks.submitTask();
            });
        } else {
            this.postDraftMessage(draftMessage).then(() => {
                microsoftTeams.tasks.submitTask();
            });
        }
    }

    private editDraftMessage = async (draftMessage: IDraftMessage) => {
        try {
            await updateDraftNotification(draftMessage);
        } catch (error) {
            return error;
        }
    }

    private postDraftMessage = async (draftMessage: IDraftMessage) => {
        try {
            let response = await createDraftNotification(draftMessage);
            return response.data;
        } catch (error) {
            throw error;
        }
    }

    public escFunction(event: any) {
        if (event.keyCode === 27 || (event.key === "Escape")) {
            microsoftTeams.tasks.submitTask();
        }
    }

    private onNext = (event: any) => {
        this.setState({
            page: "AudienceSelection"
        }, () => {
            this.updateCard();
        });
    }

    private onBack = (event: any) => {
        this.setState({
            page: "CardCreation"
        }, () => {
            this.updateCard();
        });
    }

    private onTitleChanged = (event: any) => {
        let showDefaultCard = (!event.target.value && !this.state.imageLink && !this.state.summary && !this.state.author && !this.state.btnTitle && !this.state.btnLink);
        setCardTitle(this.card, event.target.value);
        setCardImageLink(this.card, this.state.imageLink);
        setCardSummary(this.card, this.state.summary);
        setCardAuthor(this.card, this.state.author);
        setCardBtn(this.card, this.state.btnTitle, this.state.btnLink);
        this.setState({
            title: event.target.value,
            card: this.card
        }, () => {
            if (showDefaultCard) {
                this.setDefaultCard(this.card);
            }
            this.updateCard();
        });
    }

    private onImageLinkChanged = (event: any) => {
        let url = event.target.value.toLowerCase();
        if (!((url === "") || (url.startsWith("https://") || (url.startsWith("data:image/png;base64,")) || (url.startsWith("data:image/jpeg;base64,")) || (url.startsWith("data:image/gif;base64,"))))) {
            this.setState({
                errorImageUrlMessage: this.localize("ErrorUrlMessage")
            });
        } else {
            this.setState({
                errorImageUrlMessage: ""
            });
        }

        let showDefaultCard = (!this.state.title && !event.target.value && !this.state.summary && !this.state.author && !this.state.btnTitle && !this.state.btnLink);
        setCardTitle(this.card, this.state.title);
        setCardImageLink(this.card, event.target.value);
        setCardSummary(this.card, this.state.summary);
        setCardAuthor(this.card, this.state.author);
        setCardBtn(this.card, this.state.btnTitle, this.state.btnLink);
        this.setState({
            imageLink: event.target.value,
            card: this.card
        }, () => {
            if (showDefaultCard) {
                this.setDefaultCard(this.card);
            }
            this.updateCard();
        });
    }

    private onSummaryChanged = (event: any) => {
        let showDefaultCard = (!this.state.title && !this.state.imageLink && !event.target.value && !this.state.author && !this.state.btnTitle && !this.state.btnLink);
        setCardTitle(this.card, this.state.title);
        setCardImageLink(this.card, this.state.imageLink);
        setCardSummary(this.card, event.target.value);
        setCardAuthor(this.card, this.state.author);
        setCardBtn(this.card, this.state.btnTitle, this.state.btnLink);
        this.setState({
            summary: event.target.value,
            card: this.card
        }, () => {
            if (showDefaultCard) {
                this.setDefaultCard(this.card);
            }
            this.updateCard();
        });
    }

    private onAuthorChanged = (event: any) => {
        let showDefaultCard = (!this.state.title && !this.state.imageLink && !this.state.summary && !event.target.value && !this.state.btnTitle && !this.state.btnLink);
        setCardTitle(this.card, this.state.title);
        setCardImageLink(this.card, this.state.imageLink);
        setCardSummary(this.card, this.state.summary);
        setCardAuthor(this.card, event.target.value);
        setCardBtn(this.card, this.state.btnTitle, this.state.btnLink);
        this.setState({
            author: event.target.value,
            card: this.card
        }, () => {
            if (showDefaultCard) {
                this.setDefaultCard(this.card);
            }
            this.updateCard();
        });
    }

    private onBtnTitleChanged = (event: any) => {
        const showDefaultCard = (!this.state.title && !this.state.imageLink && !this.state.summary && !this.state.author && !event.target.value && !this.state.btnLink);
        setCardTitle(this.card, this.state.title);
        setCardImageLink(this.card, this.state.imageLink);
        setCardSummary(this.card, this.state.summary);
        setCardAuthor(this.card, this.state.author);
        if (event.target.value && this.state.btnLink) {
            setCardBtn(this.card, event.target.value, this.state.btnLink);
            this.setState({
                btnTitle: event.target.value,
                card: this.card
            }, () => {
                if (showDefaultCard) {
                    this.setDefaultCard(this.card);
                }
                this.updateCard();
            });
        } else {
            delete this.card.actions;
            this.setState({
                btnTitle: event.target.value,
            }, () => {
                if (showDefaultCard) {
                    this.setDefaultCard(this.card);
                }
                this.updateCard();
            });
        }
    }

    private onBtnLinkChanged = (event: any) => {
        if (!(event.target.value === "" || event.target.value.toLowerCase().startsWith("https://"))) {
            this.setState({
                errorButtonUrlMessage: this.localize("ErrorUrlMessage")
            });
        } else {
            this.setState({
                errorButtonUrlMessage: ""
            });
        }

        const showDefaultCard = (!this.state.title && !this.state.imageLink && !this.state.summary && !this.state.author && !this.state.btnTitle && !event.target.value);
        setCardTitle(this.card, this.state.title);
        setCardSummary(this.card, this.state.summary);
        setCardAuthor(this.card, this.state.author);
        setCardImageLink(this.card, this.state.imageLink);
        if (this.state.btnTitle && event.target.value) {
            setCardBtn(this.card, this.state.btnTitle, event.target.value);
            this.setState({
                btnLink: event.target.value,
                card: this.card
            }, () => {
                if (showDefaultCard) {
                    this.setDefaultCard(this.card);
                }
                this.updateCard();
            });
        } else {
            delete this.card.actions;
            this.setState({
                btnLink: event.target.value
            }, () => {
                if (showDefaultCard) {
                    this.setDefaultCard(this.card);
                }
                this.updateCard();
            });
        }
    }

    private updateCard = () => {
        const adaptiveCard = new AdaptiveCards.AdaptiveCard();
        adaptiveCard.parse(this.state.card);
        const renderedCard = adaptiveCard.render();
        const container = document.getElementsByClassName('adaptiveCardContainer')[0].firstChild;
        if (container != null) {
            container.replaceWith(renderedCard);
        } else {
            document.getElementsByClassName('adaptiveCardContainer')[0].appendChild(renderedCard);
        }
        const link = this.state.btnLink;
        adaptiveCard.onExecuteAction = function (action) { window.open(link, '_blank'); }
    }
}

const newMessageWithTranslation = withTranslation()(NewMessage);
export default newMessageWithTranslation;
