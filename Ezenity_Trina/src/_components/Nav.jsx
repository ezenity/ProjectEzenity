import React, { useState, useEffect } from 'react';
import { NavLink, Route } from 'react-router-dom';

import { Role } from '@/_helpers';
import { accountService } from '@/_services';

function Nav() {
  const [user, setUser] = useState({});

  useEffect(() => {
    const subscription = accountService.user.subscribe(x => setUser(x));
    return subscription.unsubscribe;
  }, []);

  // Guest Users
  if (!user)
    return (
      <div>
        <nav className="navbar navbar-expand navbar-dark bg-dark">
          <div className="container">

            <Route path="/" component={LeftNav} />

            <div className="spacer"></div>

            <div className="navbar-nav">
              <NavLink to="/account/login" className="nav-item nav-link">
                Login
              </NavLink>
            </div>
          </div>
        </nav>
        <Route path="/activism" component={ActivismNav} />
      </div>
    );

  // Logged in users
  return (
    <div>
      <nav className="navbar navbar-expand navbar-dark bg-dark">
        <div className="container">

          <Route path="/" component={LeftNav} />

          <div className="spacer"></div>

          <div className="navbar-nav">
            <NavLink to="/profile" className="nav-item nav-link">
              Profile
            </NavLink>

            {user.role === Role.Admin &&
              <NavLink to="/admin" className="nav-item nav-link">
                Admin
              </NavLink>
            }

            <a onClick={accountService.logout} className="nav-item nav-link">
              Logout
            </a>
          </div>
        </div>
      </nav>
      <Route path="/activism" component={ActivismNav} />
      <Route path="/admin" component={AdminNav} />
    </div>
  );
}

function LeftNav() {
  return (
    <div className="navbar-nav">
      <NavLink exact to="/" className="nav-item nav-link">
        Home
      </NavLink>

      <NavLink exact to="/activism" className="nav-item nav-link">
        Activism
      </NavLink>

      <NavLink exact to="/store" className="nav-item nav-link">
        Store
      </NavLink>

      <NavLink exact to="/faq" className="nav-item nav-link">
        FAQ
      </NavLink>
    </div>
  );
}

function AdminNav({ match }) {
  const { path } = match;

  return (
    <nav className="admin-nav navbar navbar-expand navbar-light">
      <div className="container">
        <div className="navbar-nav">
          <NavLink to={`${path}/users`} className="nav-item nav-link">
            Users
          </NavLink>
        </div>
      </div>
    </nav>
  );
}

function ActivismNav({match}) {
  const { path } = match;

  return (
    <nav className="admin-nav navbar navbar-expand navbar-light">
      <div className="container">
        <div className="navbar-nav">
          <NavLink exact to={`${path}`} className="nav-item nav-link">
            Welcome
          </NavLink>
          <NavLink to={`${path}/gallery`} className="nav-item nav-link">
            Gallery
          </NavLink>
        </div>
      </div>
    </nav>
  );
}

export { Nav };
