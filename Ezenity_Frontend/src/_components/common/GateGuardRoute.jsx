import React from "react";
import { Route, Redirect } from "react-router-dom";
import { isGateUnlocked } from "@/_helpers/gate";

/**
 * GateGuardRoute
 * --------------
 * Client-side gating for routes you want hidden behind the skull pattern.
 * If gate isn't unlocked -> redirect to "/".
 *
 * IMPORTANT:
 * This is only UX gating — not real security.
 */
function GateGuardRoute({ component: Component, ...rest }) {
    return (
        <Route
            {...rest}
            render={(props) => {
                if (!isGateUnlocked()) {
                    return <Redirect to={{ pathname: "/", state: { from: props.location } }} />;
                }
                return <Component {...props} />;
            }}
        />
    );
}

export { GateGuardRoute };
