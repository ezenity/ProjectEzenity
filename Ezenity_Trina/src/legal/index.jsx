import React from "react";
import { Route, Switch } from 'react-router-dom';

import { Copyright } from './Copyright';
import { PrivacyPolicy } from './PrivacyPolicy';
import { TermsConditions } from './TermsConditions';

function Legal({ match }) {
  const { path } = match;

  return (
    <div className="p-4">
      <div className="container">
        <Switch>
          <Route path={`${path}/copyright`} component={Copyright} />
          <Route path={`${path}/privacy-policy`} component={PrivacyPolicy} />
          <Route path={`${path}/terms-conditions`} component={TermsConditions} />
        </Switch>
      </div>
    </div>
  );
}

export { Legal };