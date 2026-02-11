import React, { useState, useEffect } from "react";
import { Route, Switch, Redirect, useLocation } from "react-router-dom";

import { Role } from "@/_helpers";
import { isGateUnlocked } from "@/_helpers/gate";
import { accountService } from "@/_services";
import { Nav, PrivateRoute, Alert, GateGuardRoute } from "@/_components";

import { Home } from "@/home";
import { Profile } from "@/profile";
import { Admin } from "@/admin";
import { Account } from "@/account";
import { Legal } from "@/legal";
import { Footer } from "@/footer";
import { About } from "@/about";
import { GatePage } from "@/gate";
import { Vault } from "@/vault";

function App() {
    const { pathname } = useLocation();
    const [user, setUser] = useState(null);

    //useEffect(() => {
    //    const subscription = accountService.user.subscribe((x) => setUser(x));
    //    return subscription.unsubscribe;
    //}, []);

    useEffect(() => {
        let isMounted = true;

        // 1) Try to refresh session on app start.
        // If there is no cookie yet, ignore the error (that’s normal for guests).
        accountService
            .refreshToken()
            .catch(() => null);

        // 2) Subscribe to user updates
        const subscription = accountService.user.subscribe((x) => {
            if (isMounted) setUser(x);
        });

        return () => {
            isMounted = false;
            subscription.unsubscribe();
        };
    }, []);

    // Gate is only “active” on the root route, until unlocked
    const gateActive = pathname === "/" && !isGateUnlocked();

    return (
        <div className="app-container">
            {!gateActive && <Nav />}
            {!gateActive && <Alert />}

            <Switch>
                <Redirect from="/:url*(/+)" to={pathname.slice(0, -1)} />

                {/* Root: Gate until unlocked, then show Home */}
                <Route
                    exact
                    path="/"
                    render={() => (isGateUnlocked() ? <Home /> : <GatePage />)}
                />

                {/* Public routes you still want accessible */}
                <Route path="/about" component={About} />
                <Route path="/legal" component={Legal} />
                <Route path="/footer" component={Footer} />
                <Route path="/github" component={() => (window.location = "https://www.github.com/ezenity/")} />
                <Route path="/instagram" component={() => (window.location = "https://www.instagram.com/midnight_gd/")} />
                <Route path="/facebook" component={() => (window.location = "https://www.facebook.com/project.ezenity/")} />
                <Route path="/linkedin" component={() => (window.location = "https://www.linkedin.com/in/anthonymmacallister/")} />
                <Route path="/jenkins" component={() => (window.location = "https://www.ezenity.com/jenkins/")} />

                {/* Gate-protected account routes (login/register/forgot-password/etc.) */}
                <GateGuardRoute path="/account" component={Account} />

                {/* Auth-required routes */}
                <PrivateRoute path="/profile" component={Profile} />
                <PrivateRoute path="/vault" component={Vault} />
                <PrivateRoute path="/admin" roles={[Role.Admin]} component={Admin} />

                <Redirect from="*" to="/" />
            </Switch>
        </div>
    );
}

export { App };
