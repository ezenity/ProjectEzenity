// src/config/index.js

const API_BASE = "/api";          // keep proxy rules simple: proxy "/api" -> backend
const DEFAULT_API_VERSION = "v1"; // change this once when you bump defaults

function joinUrl(...parts) {
    return parts
        .filter(Boolean)
        .map((p) => String(p).replace(/^\/+|\/+$/g, "")) // trim slashes
        .join("/")
        .replace(/^/, "/"); // ensure leading slash
}

const config = {
    apiBase: API_BASE,
    apiDefaultVersion: DEFAULT_API_VERSION,

    // legacy compatibility: existing code using config.apiUrl will still work
    apiUrl: joinUrl(API_BASE, DEFAULT_API_VERSION),

    // preferred: version-aware URL builder
    api: {
        url(path = "", version = DEFAULT_API_VERSION) {
            return joinUrl(API_BASE, version, path);
        },
    },
};

export default config;
