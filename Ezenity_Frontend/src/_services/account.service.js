import { BehaviorSubject } from "rxjs";

import config from "config";
import { fetchWrapper, history } from "@/_helpers";

const userSubject = new BehaviorSubject(null);
//const apiRoot = config.apiUrl.replace(/\/+$/, ""); // remove trailing slashes
//const baseUrl = `${apiRoot}/accounts`;
const baseUrl = config.api.url("/accounts"); // default v1

export const accountService = {
  login,
  logout,
  refreshToken,
  register,
  verifyEmail,
  forgotPassword,
  validateResetToken,
  resetPassword,
  getAll,
  getById,
  create,
  update,
  delete: _delete,
  user: userSubject.asObservable(),
  get userValue() {
    return userSubject.value;
  },
};

function login(email, password) {
  return fetchWrapper
    .post(`${baseUrl}/authenticate`, { email, password })
    .then((user) => {
      const user = unwrap(res);

      // normalize token field names (supports multiple backend shapes)
      const jwtToken = user?.jwtToken || user?.token || user?.accessToken;
      const normalized = jwtToken ? { ...user, jwtToken } : user;

      userSubject.next(normalized);

      // only start timer if token looks like a JWT
      startRefreshTokenTimerSafe();

      return normalized;
    });
}

function logout() {
  // revoke token, stop refresh timer, publish null to user subscribers and redirect to login page
  fetchWrapper.post(`${baseUrl}/revoke-token`, {});
  stopRefreshTokenTimer();
  userSubject.next(null);
  history.push("/");
}

function refreshToken() {
    return fetchWrapper
        .post(`${baseUrl}/refresh-token`, {})
        .then((user) => {
            const user = unwrap(res);

            // publish user to subscribers and start timer to refresh token
            userSubject.next(user);
            startRefreshTokenTimerSafe();
            return user;
        })
        .catch(() => {
            // If there's no cookie yet (logged out), this is normal.
            // Keep user as null and don't blow up the UI.
            userSubject.next(null);
            stopRefreshTokenTimer();
            return null;
        });;
}

function register(params) {
  return fetchWrapper.post(`${baseUrl}/register`, params);
}

function verifyEmail(token) {
  return fetchWrapper.post(`${baseUrl}/verify-email`, { token });
}

function forgotPassword(email) {
  return fetchWrapper.post(`${baseUrl}/forgot-password`, { email });
}

function validateResetToken(token) {
  return fetchWrapper.post(`${baseUrl}/validate-reset-token`, { token });
}

function resetPassword({ token, password, confirmPassword }) {
  return fetchWrapper.post(`${baseUrl}/reset-password`, {
    token,
    password,
    confirmPassword,
  });
}

function getAll() {
  return fetchWrapper.get(baseUrl);
}

function getById(id) {
  return fetchWrapper.get(`${baseUrl}/${id}`);
}

function create(params) {
  return fetchWrapper.post(baseUrl, params);
}

function update(id, params) {
  return fetchWrapper.put(`${baseUrl}/${id}`, params).then((user) => {
    // update stored user if the logged in user updated their own record
    if (user.id === userSubject.value.id) {
      // publish updated user to subscribers
      user = { ...userSubject.value, ...user };
      userSubject.next(user);
    }
    return user;
  });
}

// prefixed with underscore because 'delete' is a reserved word in javascript
function _delete(id) {
  return fetchWrapper.delete(`${baseUrl}/${id}`).then((x) => {
    // auto logout if the logged in user deleted their own record
    if (id === userSubject.value.id) {
      logout();
    }
    return x;
  });
}

// helper functions

let refreshTokenTimeout;
const unwrap = (res) => (res && typeof res === "object" && "data" in res ? res.data : res);


/** Obsolete */
function startRefreshTokenTimer() {
  // parse json object from base64 encoded jwt token
  const jwtToken = JSON.parse(atob(userSubject.value.jwtToken.split(".")[1]));
  //const jwtToken = JSON.parse( Buffer.from(userSubject.value.jwtToken.split(".")[1], 'base64') );

  // set a timeout to refresh the token a minute before it expires
  const expires = new Date(jwtToken.exp * 1000);
  const timeout = expires.getTime() - Date.now() - 60 * 1000;
  refreshTokenTimeout = setTimeout(refreshToken, timeout);
}

function startRefreshTokenTimerSafe() {
    stopRefreshTokenTimer();

    const token = userSubject.value?.jwtToken;
    if (!token) return;

    const parts = token.split(".");
    if (parts.length !== 3) return; // not a JWT, don’t crash the app

    try {
        const payload = JSON.parse(atob(parts[1]));
        const expires = new Date(payload.exp * 1000);
        const timeout = expires.getTime() - Date.now() - 60 * 1000;

        // guard: don’t schedule negative/instant refresh loops
        if (Number.isFinite(timeout) && timeout > 0) {
            refreshTokenTimeout = setTimeout(refreshToken, timeout);
        }
    } catch {
        // if payload parsing fails, skip timer rather than breaking login
    }
}

function stopRefreshTokenTimer() {
  clearTimeout(refreshTokenTimeout);
}
