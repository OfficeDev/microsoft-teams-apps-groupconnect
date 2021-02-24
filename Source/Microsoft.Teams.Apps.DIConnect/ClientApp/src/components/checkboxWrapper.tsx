// <copyright file="checkboxWrapper.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from "react";
import { Flex, Checkbox } from "@fluentui/react-northstar";
import "../components/popup-menu/popupMenu.scss";

interface ICheckboxProps {
    title: JSX.Element;
    index: number;
    isChecked: boolean;
    onChange: (key: number, isChecked: boolean) => void
}

const CheckboxWrapper: React.FunctionComponent<ICheckboxProps> = props => {
    return (
        <div>
            <Flex gap="gap.small">
                <Checkbox className="checkbox-wrapper" label={props.title} key={props.index} checked={props.isChecked} onChange={(event, data: any) => props.onChange(props.index, data.checked)} />
            </Flex>
        </div>
    );
}

export default CheckboxWrapper;