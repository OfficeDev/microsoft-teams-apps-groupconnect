// <copyright file="popupMenuCheckboxesContent.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from "react";
import { Flex, Button, Provider, Divider } from "@fluentui/react-northstar";
import { CloseIcon } from "@fluentui/react-icons-northstar";
import CheckboxWrapper from "../checkboxWrapper";
import { useTranslation } from 'react-i18next';
import { ICheckBoxItem } from "../FilterBar/filterBar";
import "./popupMenu.scss";

interface IPopupMenuCheckboxesContentProps {
    disableClear: boolean,
    content: any,
    onCheckboxStateChange: (checkboxState: Array<ICheckBoxItem>) => void
}

const PopupMenuCheckboxesContent: React.FunctionComponent<IPopupMenuCheckboxesContentProps> = props => {
    const localize = useTranslation().t;
    const [data] = React.useState({ checkboxes: props.content.checkboxes });
    const [filteredCheckboxes, setFilteredCheckboxes] = React.useState(props.content.checkboxes);
    let [disableClear, setDisableClear] = React.useState(true);
    let [checkBoxClicked, setCheckBoxClicked] = React.useState(false);

    /**
    *Updates particular checkbox's isChecked state and passes changed state back to parent component.
    *@param key Unique key for checkbox which needs to be updated
    *@param checked Boolean indicating checkbox current value
    */
    const onCheckboxValueChange = (key: number, checked: boolean) => {
        let checkCount = 0;
        setCheckBoxClicked(true);
        let checkboxList = data.checkboxes.map((checkbox: ICheckBoxItem) => {
            if (checkbox.key === key) {
                checkbox.isChecked = checked;
            }
            return checkbox;
        });

        checkboxList.map((checkbox: ICheckBoxItem) => {
            if (checkbox.isChecked) {
                checkCount = checkCount + 1;
            }
        })

        if (checkCount > 0) {
            setDisableClear(false);
        }
        else {
            setDisableClear(true);
        }

        props.onCheckboxStateChange(checkboxList);
    }

    /**
    *Sets all checkbox's isChecked to false to unselect all and passes changed state back to parent component.
    */
    const deSelectAll = () => {
        let checkboxList = filteredCheckboxes.map((checkbox: ICheckBoxItem) => {
            checkbox.isChecked = false
            return checkbox;
        });
        setFilteredCheckboxes(checkboxList);
        props.onCheckboxStateChange(checkboxList);
        setDisableClear(true);
        setCheckBoxClicked(true);
    }

    return (
        <Provider>
            <div className="content-items-wrapper">
                <div className="content-items-body">
                    <Flex gap="gap.small" vAlign="center" hAlign="end">
                        <Button disabled={checkBoxClicked ? disableClear : props.disableClear} className="clear-button" icon={<CloseIcon />} size="small" text onClick={() => deSelectAll()} content={localize("ClearAll")} />
                    </Flex>
                    <Divider className="filter-popup-menu-divider" />
                    {
                        filteredCheckboxes.map((checkbox: ICheckBoxItem) => {
                            if (checkbox.title.trim().length) {
                                return (
                                    <CheckboxWrapper title={checkbox.checkboxLabel} isChecked={checkbox.isChecked} index={checkbox.key} onChange={(key, isChecked) => onCheckboxValueChange(key, isChecked)} />
                                );
                            }
                        })
                    }
                </div>
            </div>
        </Provider>
    );
}

export default PopupMenuCheckboxesContent;