import React from "react";
import { Route, Switch } from "react-router-dom";

import { Overview } from './Overview';
// import { Gallery } from './Gallery';

function Store({ match }) {
  const { path } = match;

  return (
    <div className="p-4">
      <div className="container">
          <Switch>
              <Route exact path={path} component={Overview} />
              {/* <Route path={`${path}/gallery`} component={Gallery} /> */}
          </Switch>
      </div>
    </div>
  );
}

export { Store };