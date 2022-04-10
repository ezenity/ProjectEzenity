import React, { useState, useEffect } from "react";
import { Route, Switch, Redirect, useLocation } from "react-router-dom";

import { Role } from "@/_helpers";
import { accountService } from "@/_services";
import { Nav, PrivateRoute, Alert } from "@/_components";
import { Home } from "@/home";
import { Profile } from "@/profile";
import { Admin } from "@/admin";
import { Account } from "@/account";
import { Legal } from "@/legal";
import { Footer } from "@/footer";
import { About } from "@/about";

function App() {
  const { pathname } = useLocation();
  const [user, setUser] = useState({});

  useEffect(() => {
    const subscription = accountService.user.subscribe((x) => setUser(x));
    return subscription.unsubscribe;
  }, []);

  // console.log(pathname);



  return (
    // <div className={'app-container' + (user && " bg-light")}>
    <div className="app-container" >
      <Nav />
      <Alert />
      <Switch>
        <Redirect from="/:url*(/+)" to={pathname.slice(0, -1)} />

        {/* Publicly accessable routes */}
        <Route exact path="/" component={Home} />
        <Route path="/about" component={About} />
        <Route path="/account" component={Account} />
        <Route path="/legal" component={Legal} />
        <Route path="/footer" component={Footer} />
        <Route path="/github" component={() => (window.location="https://www.github.com/ezenity/" )} />
        <Route path="/instagram" component={() => (window.location="https://www.instagram.com/midnight_gd/" )} />
        <Route path="/facebook" component={() => (window.location="https://www.facebook.com/project.ezenity/" )} />
        <Route path="/linkedin" component={() => (window.location="https://www.linkedin.com/in/anthonymmacallister/" )} />
        <Route path="/jenkins" component={() => (window.location="https://www.ezenity.com/jenkins/" )} />
      
        {/* Routes requiring authentication */}
        {/* <PrivateRoute exact path="/" component={Home} /> */}
        <PrivateRoute path="/profile" component={Profile} />
        <PrivateRoute path="/admin" roles={[Role.Admin]} component={Admin} />

        <Redirect from="*" to="/" />
      </Switch>
    </div>
  );
}

export { App };
