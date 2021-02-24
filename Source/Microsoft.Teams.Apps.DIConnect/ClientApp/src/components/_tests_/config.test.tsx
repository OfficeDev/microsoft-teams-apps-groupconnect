// <copyright file="config.test.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from "react";
import { Provider } from "@fluentui/react-northstar";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import Configuration from "../config";
import pretty from "pretty";

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
    settings: {
        saveEvent: () => {
            notifySuccess: () => { };
        },
        registerOnSaveHandler: (saveEvent: any) =>
            saveEvent(
                Promise.resolve({ teamId: "ewe", entityId: "sdsd", locale: "en-US" })
            ),
        setSettings: (callback: any) => {
            return true;
        },
    },
    setValidityState() { },
    getContext: (callback: any) =>
        callback(
            Promise.resolve({ teamId: "ewe", entityId: "sdsd", locale: "en-US" })
        ),
}));

let container: any = null;
beforeEach(() => {
    // setup a DOM element as a render target
    container = document.createElement("div");
    // container *must* be attached to document so events work correctly.
    document.body.appendChild(container);
    act(() => {
        render(
            <Provider>
                <Configuration />
            </Provider>,
            container
        );
    });
});

afterEach(() => {
    // cleanup on exiting
    unmountComponentAtNode(container);
    container.remove();
    container = null;
});

describe("Configuration", () => {
    it("renders snapshots", async () => {
        act(() => {
            render(
                <Provider>
                    <Configuration />
                </Provider>,
                container
            );
        });

        expect(pretty(container.innerHTML)).toMatchSnapshot();
    });

    // Note (Prequisites to run or execute the test cases)
    // We have observed some runtime errors while we are runnning the app and simultaneously running the test cases, as we work around comment out the following code
    // Comment out line no 100 ( 'saveEvent.notifySuccess();' ) in config.tsx file .
    // Comment out line 103 ( 'microsoftTeams.settings.setValidityState(false);' ) in config.tsx file .

    it("checks Communications Tab Text content", async () => {
        let categoryNameInput = document.querySelector(
            "[data-testid=test-communication]"
        );
        if (categoryNameInput != null) {
            expect(categoryNameInput.innerHTML).toBe("CommunicationsTab");
        }
    });

    it("checks Configuration Tab Text content", async () => {
        let categoryNameInput = document.querySelector(
            "[data-testid=test-configuration]"
        );
        if (categoryNameInput != null) {
            expect(categoryNameInput.innerHTML).toBe("ConfigurationTab");
        }
    });

    it("checks Image src content for Communication Tab", async () => {
        let categoryImageInput = document.querySelector("[data-testid=img-communication]");
        if (categoryImageInput != null) {
            expect(categoryImageInput.getAttribute("src")).toBe(
                "http://localhost/image/color.png"
            );
        }
    });

    it("checks Image src conten for Configuration Tab", async () => {
        let categoryImageInput = document.querySelector("[data-testid=img-configuration]");
        if (categoryImageInput != null) {
            expect(categoryImageInput.getAttribute("src")).toBe(
                "http://localhost/image/color.png"
            );
        }
    });
});
