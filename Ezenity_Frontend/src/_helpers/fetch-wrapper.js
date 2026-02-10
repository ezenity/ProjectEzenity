import config from "config";
import { accountService } from "@/_services";

export const fetchWrapper = { get, post, put, delete: _delete };

function get(url) {
    return request(url, { method: "GET" });
}

function post(url, body) {
    return request(url, {
        method: "POST",
        body: body !== undefined ? JSON.stringify(body) : undefined,
    });
}

function put(url, body) {
    return request(url, {
        method: "PUT",
        body: body !== undefined ? JSON.stringify(body) : undefined,
    });
}

function _delete(url) {
    return request(url, { method: "DELETE" });
}

function request(url, options) {
    const isApi = isApiUrl(url);

    const headers = {
        Accept: "application/json",
        ...(options.body ? { "Content-Type": "application/json" } : {}),
        ...(isApi ? authHeader() : {}),
        ...(options.headers || {}),
    };

    const requestOptions = {
        ...options,
        headers,
        // If your API sets/uses cookies (refresh token), you need this on ALL calls
        ...(isApi ? { credentials: "include" } : {}),
    };

    return fetch(url, requestOptions).then(handleResponse);
}

function authHeader() {
    const user = accountService.userValue;
    const token = user?.jwtToken || user?.token || user?.accessToken; // tolerate different backend shapes
    return token ? { Authorization: `Bearer ${token}` } : {};
}

function isApiUrl(url) {
    // Works with absolute or relative URLs
    const apiBase = new URL(config.apiUrl, window.location.origin);
    const target = new URL(url, window.location.origin);

    // Same origin AND path starts with api base path
    return (
        target.origin === apiBase.origin &&
        target.pathname.startsWith(apiBase.pathname.replace(/\/+$/, "") + "/")
    );
}

function handleResponse(response) {
    // 204 No Content: don’t try to parse a body
    if (response.status === 204) return Promise.resolve(null);

    const contentType = response.headers.get("content-type") || "";

    return response.text().then((text) => {
        const isJson =
            contentType.includes("application/json") ||
            contentType.includes("application/vnd.api+json");

        let data = text;
        if (isJson && text) {
            try {
                data = JSON.parse(text);
            } catch {
                // keep as text if server returned invalid JSON
                data = text;
            }
        }

        if (!response.ok) {
            if ([401, 403].includes(response.status) && accountService.userValue) {
                accountService.logout();
            }

            const msg =
                (isJson && data && (data.message || data.error || data.title)) ||
                response.statusText ||
                (typeof data === "string" ? data : "Request failed");

            return Promise.reject(msg);
        }

        return data;
    });
}
