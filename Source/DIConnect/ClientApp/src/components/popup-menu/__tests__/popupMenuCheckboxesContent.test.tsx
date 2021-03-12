// <copyright file="popupMenuCheckboxesContent.test.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Provider } from "@fluentui/react-northstar";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import pretty from "pretty";
import PopupMenuCheckboxesContent from "../popupMenuCheckboxesContent";
import { ICheckBoxItem } from "../../FilterBar/filterBar";

jest.mock("react-i18next", () => ({
    useTranslation: () => ({
        t: (key: any) => key,
        i18n: { changeLanguage: jest.fn() },
    }),
    withTranslation: () => (Component: any) => {
        Component.defaultProps = {
            ...Component.defaultProps,
            t: (key: any) => key,
        };
        return Component;
    },
}));
jest.mock("@microsoft/teams-js", () => ({
    initialize: () => {
        return true;
    },
    getContext: (callback: any) =>
        callback(
            Promise.resolve({ teamId: "id", locale: "en-US" })
        ),
}));

let container: any = null;

const getCheckboxLabel = (): JSX.Element => {
    return (
        <div className="container-div">
            test
        </div>
    );
}

const checkboxItemList: Array<ICheckBoxItem> = [
    {
        id: "1",
        key: 1,
        title: "checkbox 1",
        checkboxLabel: getCheckboxLabel(),
        isChecked: false
    },
    {
        id: "1",
        key: 1,
        title: "checkbox 1",
        checkboxLabel: getCheckboxLabel(),
        isChecked: false
    }
];

let a: any

beforeEach(() => {
    // setup a DOM element as a render target
    container = document.createElement("div");
    // container *must* be attached to document so events work correctly.
    document.body.appendChild(container);
    act(() => {
        render(<Provider>
            <PopupMenuCheckboxesContent
                disableClear={false}
                content={{ checkboxes: checkboxItemList, title: "test"}}
                onCheckboxStateChange={(typeState: Array<any>) => { }}
            />
        </Provider>, container);
    });
});

afterEach(() => {
    // cleanup on exiting
    unmountComponentAtNode(container);
    container.remove();
    container = null;
});

describe('PopupMenuCheckboxesContent', () => {
    it('renders snapshots', () => {
        expect(pretty(container.innerHTML)).toMatchSnapshot();
    });
});