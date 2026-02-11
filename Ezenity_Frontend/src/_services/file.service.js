import config from "config";
import { fetchWrapper } from "@/_helpers";

const baseUrl = config.api.url("/files"); // v1

export const fileService = {
    list,
    upload,
    remove,
    urlFor,
};

function list(scope) {
    const qs = scope ? `?scope=${encodeURIComponent(scope)}` : "";
    return fetchWrapper.get(`${baseUrl}/list${qs}`);
}

// key: "file" must match controller
function upload(file, { scope } = {}) {
    const form = new FormData();
    form.append("file", file);
    if (scope) form.append("scope", scope);

    return fetchWrapper.post(`${baseUrl}/upload`, form);
}

function remove(id) {
    return fetchWrapper.delete(`${baseUrl}/${id}`);
}

function urlFor(id) {
    return `${baseUrl}/${id}`;
}
