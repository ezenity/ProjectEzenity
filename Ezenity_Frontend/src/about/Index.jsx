import React from "react";
import { Route, Switch, Redirect, NavLink } from "react-router-dom";

import { Overview } from "./Overview";
import { Gallery } from "./Gallery";
import { MediaLibrary } from "../files/MediaLibrary";

function About({ match }) {
    const { path, url } = match;

    return (
        <div className="p-4">
            <div className="container">
                <div className="d-flex align-items-center justify-content-between mb-3">
                    <div>
                        <h2 className="m-0">About</h2>
                        <small className="text-muted">
                            After Hours Crew • Invite-only rider network
                        </small>
                    </div>
                </div>

                <div className="row">
                    {/* Left menu */}
                    <div className="col-md-3 mb-3">
                        <div className="list-group">
                            <NavLink
                                exact
                                to={`${url}`}
                                className="list-group-item list-group-item-action"
                                activeClassName="active"
                            >
                                Overview
                            </NavLink>

                            <NavLink
                                to={`${url}/media`}
                                className="list-group-item list-group-item-action"
                                activeClassName="active"
                            >
                                Media Library
                            </NavLink>
                        </div>

                        <div className="mt-3 small text-muted">
                            <div className="mb-2">
                                <strong>Note:</strong> This section is being rebuilt from older content.
                            </div>
                            <div>
                                Media Library is a placeholder — later it will connect to profile + Vault uploads.
                            </div>
                        </div>
                    </div>

                    {/* Right content */}
                    <div className="col-md-9">
                        <div className="card">
                            <div className="card-body">
                                <Switch>
                                    <Route exact path={path} component={Overview} />
                                    <Route path={`${path}/media`} component={MediaLibrary} />

                                    {/* Fallback */}
                                    <Redirect to={url} />
                                </Switch>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export { About };
