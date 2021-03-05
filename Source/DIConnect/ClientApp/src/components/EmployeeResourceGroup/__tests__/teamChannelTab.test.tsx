// <copyright file="teamChannelTab.test.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Provider } from "@fluentui/react-northstar";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import TeamChannelTab from "../teamChannelTab";
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
jest.mock("../../../apis/employeeResourceGroupApi");
jest.mock("../../../apis/teamDataApi");
jest.mock("@microsoft/teams-js", () => ({
  initialize: () => {
    return true;
  },
  getContext: (callback: any) =>
    callback(
      Promise.resolve({
        teamId: "00:0000000000000000000000000000000@abc.tacv0",
        entityId: "3763ada3-4a19-4dd6-b083-511c2df6e625",
        locale: "en-US",
      })
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
        <TeamChannelTab />
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

describe("TeamChannelTab", () => {
  it("renders snapshots", () => {
    expect(pretty(container.innerHTML)).toMatchSnapshot();
  });
});
