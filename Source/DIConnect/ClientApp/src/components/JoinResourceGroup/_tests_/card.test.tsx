// <copyright file="card.test.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import * as React from "react";
import { Provider } from "@fluentui/react-northstar";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import Card from '../card';
import pretty from "pretty";
import { IDiscoverResourceGroupsPost } from "../discoverResourceGroups";

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
    }
}));

let container: any = null;
const groups: Array<IDiscoverResourceGroupsPost> = [
    {
        groupId: "1234", groupType: "teams", groupName: "group1", groupDescription: "test desc1", imageLink: "link1", groupLink: "group link1", tags: ["test1", "test2"], location: "pune"
    }
];
beforeEach(() => {
    // setup a DOM element as a render target
    container = document.createElement("div");
    // container *must* be attached to document so events work correctly.
    document.body.appendChild(container);
    act(() => {
        render(<Provider><Card
            resourceGroupDetails={groups[0]}
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

describe('Card', () => {
    it('renders snapshots', () => {
        expect(pretty(container.innerHTML)).toMatchSnapshot();
    });

    it("checks card title text", async () => {
        let groupName = document.querySelector(
            "[data-testid=group-name]"
        );
        if (groupName != null) {
            expect(groupName.innerHTML).toBe(groups[0].groupName);
        }
    });

    it("checks image src content for card", async () => {
        let groupImage = document.querySelector("[data-testid=group-img]");
        if (groupImage != null) {
            expect(groupImage.getAttribute("src")).toBe(
                groups[0].imageLink
            );
        }
    });

    it("checks card description", async () => {
        let groupDescription = document.querySelector(
            "[data-testid=group-desc]"
        );
        if (groupDescription != null) {
            expect(groupDescription.innerHTML).toBe(groups[0].groupDescription);
        }
    });

    it("checks card tags", async () => {
        let tags: Array<Element> = [];
        for (let i = 0; i < 3; i++) {
            tags.push(document.querySelector(
                "[data-testid=group-tag" + i + "]"
            )!);
        }
        expect(tags.length).toBe(3);
        if (tags[0].firstElementChild != null)
            expect(tags[0].firstElementChild.innerHTML).toBe("test1");
    });
});