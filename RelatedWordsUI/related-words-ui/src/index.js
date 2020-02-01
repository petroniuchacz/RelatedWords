import React from "react";
import ReactDOM from "react-dom";
import { createStore, applyMiddleware } from "redux";
import "./index.css";
import "./stylesheets/NavBar.css";
import "./stylesheets/App.css";
import "./stylesheets/Notifications.css";
import App from "./App";
import { Provider } from "react-redux";
import thunk from "redux-thunk";
import rootReducer from './reducers/rootReducer';
import { BrowserRouter as Router, Route } from 'react-router-dom';
import * as serviceWorker from "./serviceWorker";
// import { devToolsEnhancer } from 'redux-devtools-extension';
import { composeWithDevTools } from "redux-devtools-extension";

// const store = createStore(tasks, devToolsEnhancer())

const store = createStore(
  rootReducer, 
  composeWithDevTools(applyMiddleware(thunk))
  );

if (module.hot) {
  module.hot.accept("./App", () => {
    const NextApp = require("./App").default;
    ReactDOM.render(
      <Provider store={store}>
        <Router>
          <Route path="/" component={NextApp}/>
        </Router>
      </Provider>,
      document.getElementById("root"),
    );
  });
  module.hot.accept("./reducers/rootReducer", () => {
    const nextRootReducer = require("./reducers/rootReducer").default;
    store.replaceReducer(nextRootReducer);
  });
}

ReactDOM.render(
  <Provider store={store}>
    <Router>
      <Route path="/" component={App} />
    </Router>
  </Provider>,
  document.getElementById("root"),
);


// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
