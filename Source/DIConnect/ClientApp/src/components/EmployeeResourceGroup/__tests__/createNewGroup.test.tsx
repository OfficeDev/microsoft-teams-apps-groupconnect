// <copyright file="createNewGroup.test.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from "react";
import { Provider } from "@fluentui/react-northstar";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import pretty from "pretty";
import CreateNewGroup from "../createNewGroup";

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
jest.mock("../../../apis/employeeResourceGroupApi");
jest.mock("../../../apis/teamDataApi");
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
beforeEach(() => {
    // setup a DOM element as a render target
    container = document.createElement("div");
    // container *must* be attached to document so events work correctly.
    document.body.appendChild(container);
    act(() => {
        render(
            <Provider>
                <CreateNewGroup />
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

describe("CreateNewGroup", () => {
    it("renders snapshots", () => {
        expect(pretty(container.innerHTML)).toMatchSnapshot();
    });

    it("Check group input fields", async () => {
        const groupTypeTitle = document.querySelector(
            "[data-testid=group_type_field]"
        );
        const groupLinkTitle = document.querySelector(
            "[data-testid=group_link_field]"
        );
        const groupNameTitle = document.querySelector(
            "[data-testid=group_name_field]"
        );

        if (groupTypeTitle != null) {
            expect(groupTypeTitle.innerHTML).toBe("GroupType");
        }

        if (groupNameTitle != null) {
            expect(groupNameTitle.innerHTML).toBe("GroupName");
        }

        if (groupLinkTitle != null) {
            expect(groupLinkTitle.innerHTML).toBe("GroupLink");
        }

    });

    it("Check team input fields", async () => {
        const teamDescripionTitle = document.querySelector(
            "[data-testid=team_description_field]"
        );
        const teamLinkTitle = document.querySelector(
            "[data-testid=team_link_field]"
        );
        const teamNameTitle = document.querySelector(
            "[data-testid=group_name_field]"
        );

        if (teamDescripionTitle != null) {
            expect(teamDescripionTitle.innerHTML).toBe("TeamDescription");
        }

        if (teamLinkTitle != null) {
            expect(teamLinkTitle.innerHTML).toBe("TeamLink");
        }

        if (teamNameTitle != null) {
            expect(teamNameTitle.innerHTML).toBe("TeamName");
        }

    });

    it("Check other input fields present", async () => {
        const imageLinkTitle = document.querySelector(
            "[data-testid=image_link_field]"
        );
        const tagsTitle = document.querySelector(
            "[data-testid=tags_field]"
        );
        const locationTitle = document.querySelector(
            "[data-testid=location_field]"
        );
        const searchEnabledTitle = document.querySelector(
            "[data-testid=searchenabled_field]"
        );

        if (imageLinkTitle != null) {
            expect(imageLinkTitle.innerHTML).toBe("ImageLink");
        }

        if (tagsTitle != null) {
            expect(tagsTitle.innerHTML).toBe("Tags");
        }

        if (locationTitle != null) {
            expect(locationTitle.innerHTML).toBe("Location");
        }

        if (searchEnabledTitle != null) {
            expect(searchEnabledTitle.innerHTML).toBe("SearchEnabled");
        }

    });

    it("Submit event button", () => {
        const submitEventButton = document.querySelector(
            "[data-testid=submit_button]"
        );
       
        expect(submitEventButton).not.toBe(null);
    });
});