import React from "react";
import { Route, Switch, useRouteMatch } from "react-router-dom";
import { MissionsList } from "./MissionsList";
import { MissionDetail } from "./MissionDetail";

function Vault() {
    const { path } = useRouteMatch();

    return (
        <div className="p-4">
            <div className="container">
                <h2 className="mb-3">THE VAULT</h2>
                <p className="text-muted">
                    Missions, milestones, and objectives. Complete runs, submit proof, earn rep,
                    stack emblems, and unlock deeper access.
                </p>

                <Switch>
                    <Route exact path={path} component={MissionsList} />
                    <Route path={`${path}/missions/:id`} component={MissionDetail} />
                </Switch>
            </div>
        </div>
    );
}

export { Vault };
