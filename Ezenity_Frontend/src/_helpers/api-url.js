import config from "config";

export function apiUrl(path, version = config.apiDefaultVersion) {
    const base = config.apiBaseUrl.replace(/\/+$/, "");
    const v = String(version || "").replace(/^\/+|\/+$/g, "");
    const p = String(path || "").startsWith("/") ? path : `/${path}`;
    return `${base}/${v}${p}`;
}
