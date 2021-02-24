// <copyright file="popupMenuWrapper.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from "react";
import { Popup, Button, Text } from "@fluentui/react-northstar";
import { ChevronDownIcon } from "@fluentui/react-icons-northstar";
import PopupMenuCheckboxesContent from "./popupMenuCheckboxesContent";
import { ICheckBoxItem } from "../FilterBar/filterBar";
import "./popupMenu.scss";

interface IPopupMenuWrapperProps {
    title: string,
    checkboxes?: Array<any>,
    onCheckboxStateChange: (typeState: Array<any>) => void
}

const PopupMenuWrapper: React.FunctionComponent<IPopupMenuWrapperProps> = props => {
    const [popup, setPopup] = React.useState({ isOpen: false });
    let [disableClear, setDisableClear] = React.useState(true);
    const onFilterClick = () => {
        let checkCount = 0;
        let checkBox = props.checkboxes!;
        checkBox.map((checkbox: ICheckBoxItem) => {
            if (checkbox.isChecked) {
                checkCount = checkCount + 1;
            }
        });
        if (checkCount > 0) {
            setDisableClear(false);
        }
        else {
            setDisableClear(true);
        }
    }

    return (
        <Popup
            open={popup.isOpen}
            align="center"
            position="below"
            onOpenChange={(e, { open }: any) => setPopup({ isOpen: open })}
            trigger={<Button className={`mobile-button ${popup.isOpen ? "gray-background" : "no-background"}`} onClick={() => onFilterClick()} content={<Text content={props.title} />} iconPosition="after" icon={<ChevronDownIcon />} text />}
            content={<PopupMenuCheckboxesContent disableClear={disableClear} content={{ checkboxes: props.checkboxes, title: props.title }} onCheckboxStateChange={props.onCheckboxStateChange} />}
            trapFocus
        />
    );
}

export default PopupMenuWrapper;