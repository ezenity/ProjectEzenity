import React from "react";
import { Route, Switch } from "react-router-dom";

import { Overview } from './Overview';
import { Gallery } from './Gallery';
import { Career_Old } from './Career_Old';
import { Career } from './career';

function About({ match }) {
  const { path } = match;

  return (
    <div className="p-4">
      <div className="container">
          <Switch>
              <Route exact path={path} component={Overview} />
              <Route path={`${path}/gallery`} component={Gallery} />
              <Route path={`${path}/career_old`} component={Career_Old} />
              <Route path={`${path}/career`} component={Career} />
          </Switch>
      </div>
    </div>
  );
}

export { About };