/**
 * gate.js
 * -------
 * Shared helpers for the client-side gate UX.
 * (Not real security—just UI/UX gating.)
 */

const UNLOCK_KEY = "ez_gate_unlocked";

/*
 * Read
 */
export function isGateUnlocked() {
    return sessionStorage.getItem(UNLOCK_KEY) === "1";
}

/*
 * Write
 */
export function setGateUnlocked() {
    sessionStorage.setItem(UNLOCK_KEY, "1");
}

/*
 * Reset
 */
export function clearGateUnlocked() {
    sessionStorage.removeItem(UNLOCK_KEY);
}

export { UNLOCK_KEY };