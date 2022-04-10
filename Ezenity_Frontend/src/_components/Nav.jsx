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
        <Route path="/about" component={AboutNav} />
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
      <Route path="/about" component={AboutNav} />
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

      <NavLink exact to="/about" className="nav-item nav-link">
        About
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

function AboutNav({match}) {
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
          <NavLink to={`${path}/career_old`} className="nav-item nav-link">
            CareerOld
          </NavLink>
          <NavLink to={`${path}/career`} className="nav-item nav-link">
            Career
          </NavLink>
        </div>
      </div>
    </nav>
  );
}

export { Nav };
