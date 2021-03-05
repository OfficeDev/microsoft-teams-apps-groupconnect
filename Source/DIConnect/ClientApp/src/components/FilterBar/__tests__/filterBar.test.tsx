// <copyright file="filterBar.test.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Provider } from "@fluentui/react-northstar";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import pretty from "pretty";
import FilterBar from "../filterBar";
import { ICheckBoxItem } from "../filterBar";

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
jest.mock('../../../apis/appSettingsApi');
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

const tagsTest: Array<string> = ["test tag1", "test tag2", "test tag3", "test tag4"];

const locationTest: Array<string> = ["location1", "location2", "location3", "location4"];

beforeEach(() => {
    // setup a DOM element as a render target
    container = document.createElement("div");
    // container *must* be attached to document so events work correctly.
    document.body.appendChild(container);
    act(() => {
        render(<Provider>
            <FilterBar
                tagsList={tagsTest}
                locationsList={locationTest}
                commandBarSearchText="test"
                onSearchInputChange={(searchString: string) => { }}
                onLocationCheckboxStateChange={(currentValues: Array<ICheckBoxItem>) => { }}
                onTagsCheckboxStateChange={(currentValues: Array<ICheckBoxItem>) => { }}
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

describe('DiscoverResourceGroups', () => {
    it('renders snapshots', () => {
        expect(pretty(container.innerHTML)).toMatchSnapshot();
    });
});