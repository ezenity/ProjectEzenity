import React from 'react';
import { Route, Switch } from 'react-router-dom';

import { Overview } from './Overview';
import { AddSkill } from './AddSkill';
import { EditSkill } from './EditSkill';

function Career({ match }) {
    const { path } = match;
    
    return (
        <Switch>
            <Route exact path={path} component={Overview} />
            <Route path={`${path}/add-skill`} component={AddSkill} />
            <Route path={`${path}/edit-skill/:id`} component={EditSkill} />
        </Switch>
    );
}

export { Career };