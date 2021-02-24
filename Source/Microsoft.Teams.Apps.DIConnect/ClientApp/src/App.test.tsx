// <copyright file="App.test.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft.
// Licensed under the MIT License.
// </copyright>

import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';

it('renders without crashing', () => {
  const div = document.createElement('div');
  ReactDOM.render(<App />, div);
  ReactDOM.unmountComponentAtNode(div);
});