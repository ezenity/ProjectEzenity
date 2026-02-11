import config from "config";
import { fetchWrapper } from "@/_helpers";

const baseUrl = config.api.url("/vault");

const unwrap = (res) => (res && typeof res === "object" && "data" in res ? res.data : res);

export const vaultService = {
    getMissions,
    submitMission,
};

function getMissions() {
    return fetchWrapper.get(`${baseUrl}/missions`).then(unwrap);
}

function submitMission(missionId, payload) {
    return fetchWrapper.post(`${baseUrl}/missions/${missionId}/submissions`, payload).then(unwrap);
}
