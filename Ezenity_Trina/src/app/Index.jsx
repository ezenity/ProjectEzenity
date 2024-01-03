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
import { Activism } from "@/activism";
// import { Store } from "@/store";
import { FAQ } from "@/faq";

function App() {
  const { pathname } = useLocation();
  const [user, setUser] = useState({});

  useEffect(() => {
    const subscription = accountService.user.subscribe((x) => setUser(x));
    return subscription.unsubscribe;
  }, []);

  // console.log(pathname);

  return (
    <div className="app-container" >
      <Nav />
      <Alert />
      <Switch>
        <Redirect from="/:url*(/+)" to={pathname.slice(0, -1)} />

        {/* Publicly accessable routes */}
        <Route exact path="/" component={Home} />
        <Route path="/activism" component={Activism} />
        {/* <Route path="/store" component={Store} /> */}
        <Route path="/store" component={() => (window.location="https://store.changemakercreations.com/")} />
        <Route path="/faq" component={FAQ} />
        <Route path="/account" component={Account} />
        <Route path="/legal" component={Legal} />
        <Route path="/footer" component={Footer} />
        {/* Social Media Accounts */}
        <Route path="/instagram" component={() => (window.location="https://www.instagram.com/changemakercreations/" )} />
        {/* <Route path="/facebook" component={() => (window.location="https://www.facebook.com/trina.faith.7/" )} /> */}
        {/* <Route path="/linkedin" component={() => (window.location="https://www.linkedin.com/in/katrina-macallister-masi-425511105/" )} /> */}
        {/* Activism Resources and Documentaries */}
        <Route path="/dominionMovement" component={() => (window.location="https://www.dominionmovement.com/")} />
        <Route path="/whatTheHealthFilm" component={() => (window.location="https://www.whatthehealthfilm.com/")} />
        <Route path="/fastAgainstSlaughter" component={() => (window.location="https://www.fastagainstslaughter.org/")} />
        <Route path="/millionDollarVegan" component={() => (window.location="https://www.milliondollarvegan.com/")} />
        <Route path="/veganAction" component={() => (window.location="https://vegan.org/")} />
        <Route path="/animalCrueltyExposureFund" component={() => (window.location="https://www.animalcrueltyexposurefund.org/")} />
        {/* Activism Local Actions */}
        <Route path="/centralFloridaAnimalRights" component={() => (window.location="https://www.facebook.com/groups/3346233518750427/")} />
        <Route path="/directActionEverywhere" component={() => (window.location="https://m.facebook.com/1434183549932112")} />
        <Route path="/animalRightsFlorida" component={() => (window.location="https://m.facebook.com/147627221972661")} />
        <Route path="/animalActivismMentorship" component={() => (window.location="https://www.facebook.com/AnimalActivismMentorship?mibextid=ZbWKwL")} />
        {/* ONLINE STORE LINKS */}
        <Route path="/customTumblerCups" component={() => (window.location="https://checkout.square.site/merchant/MLYSH5RZ572JS/checkout/5K53TIXB6PWHSTUWYVMUUCEK?src=embed")} />

        


        {/* <Route path="" component={() => (window.location="")} />
        <Route path="" component={() => (window.location="")} />
        <Route path="" component={() => (window.location="")} />
        <Route path="" component={() => (window.location="")} /> */}
        
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
