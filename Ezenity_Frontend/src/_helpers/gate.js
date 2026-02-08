/**
 * gate.js
 * -------
 * Shared helpers for the client-side gate UX.
 * (Not real security—just UI/UX gating.)
 */

export const UNLOCK_KEY = "ez_gate_unlocked";

export function isGateUnlocked() {
    return sessionStorage.getItem(UNLOCK_KEY) === "1";
}

export function setGateUnlocked() {
    sessionStorage.setItem(UNLOCK_KEY, "1");
}

export function clearGateUnlocked() {
    sessionStorage.removeItem(UNLOCK_KEY);
}

export { UNLOCK_KEY };