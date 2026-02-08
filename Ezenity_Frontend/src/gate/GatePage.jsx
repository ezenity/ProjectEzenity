import React, { useEffect, useMemo, useRef, useState } from "react";
import { Link, useHistory } from "react-router-dom";
import "./GatePage.less";

/**
 * GatePage
 * --------
 * Full-screen animated landing "gate" with a click-pattern unlock.
 *
 * Behavior:
 * 1) Plays a one-time intro background animation (green -> red -> black/silver -> gold)
 * 2) Shows skulls that pulse forever
 * 3) User must click skulls in a specific pattern to "unlock"
 * 4) After unlock, shows Login/Register actions (and sets a session flag)
 *
 * Notes:
 * - This is NOT real security (client-side logic can be bypassed).
 * - If you want to require the gate before accessing routes, use GateGuardRoute.
 */
export default function GatePage() {
    const history = useHistory();

    // You can change this pattern any time:
    // Allowed IDs: "left", "center", "right"
    const requiredPattern = useMemo(() => ["left", "right", "center", "center", "left"], []);

    // Timing
    const INTRO_MS = 5200;        // how long before skulls become active
    const RESET_AFTER_MS = 3000;  // if user pauses too long, reset progress

    const [introDone, setIntroDone] = useState(false);
    const [armed, setArmed] = useState(false);
    const [progress, setProgress] = useState(0);
    const [unlocked, setUnlocked] = useState(false);

    // UI feedback (shake on wrong input)
    const [shake, setShake] = useState(false);

    const resetTimerRef = useRef(null);

    useEffect(() => {
        const t = setTimeout(() => {
            setIntroDone(true);
            setArmed(true);
        }, INTRO_MS);

        return () => clearTimeout(t);
    }, []);

    function resetProgress() {
        setProgress(0);
        if (resetTimerRef.current) {
            clearTimeout(resetTimerRef.current);
            resetTimerRef.current = null;
        }
    }

    function armResetTimer() {
        if (resetTimerRef.current) clearTimeout(resetTimerRef.current);
        resetTimerRef.current = setTimeout(() => {
            resetProgress();
        }, RESET_AFTER_MS);
    }

    function onSkullTap(id) {
        if (!armed || unlocked) return;

        const expected = requiredPattern[progress];

        if (id === expected) {
            const next = progress + 1;
            setProgress(next);
            armResetTimer();

            if (next >= requiredPattern.length) {
                // unlocked
                setUnlocked(true);
                setArmed(false);
                resetProgress();

                // Session flag so you can gate routes if desired
                sessionStorage.setItem("ez_gate_unlocked", "1");

                // Optional: auto-navigate directly after unlock:
                // history.push("/account/login");
            }
            return;
        }

        // Wrong input -> reset + shake
        setShake(true);
        resetProgress();
        window.setTimeout(() => setShake(false), 520);
    }

    function clearGate() {
        sessionStorage.removeItem("ez_gate_unlocked");
        history.push("/");
        window.location.reload();
    }

    return (
        <div className={`gateRoot ${introDone ? "introDone" : "introPlaying"} ${shake ? "shake" : ""}`}>
            {/* BACKGROUND LAYERS */}
            <div className="bgLayer bgGreen" />
            <div className="bgLayer bgRed" />
            <div className="bgLayer bgSteel" />
            <div className="bgLayer bgGold" />
            <div className="bgLayer lava" />
            <div className="bgLayer stars" />

            {/* VIGNETTE */}
            <div className="vignette" />

            {/* CONTENT */}
            <div className="content">
                <div className="brandRow">
                    <div className="brandName">Ezenity</div>
                    <div className="brandTag">Underground • Invite-only • Ride smart</div>
                </div>

                {/* SKULLS */}
                <div className={`skullStage ${introDone ? "show" : ""}`}>
                    <SkullRow
                        disabled={!armed || unlocked}
                        onTap={onSkullTap}
                    />
                </div>

                {/* SUBTLE PROGRESS (optional but helpful) */}
                {!unlocked && (
                    <div className="hint">
                        <span className="hintDotRow" aria-hidden="true">
                            {requiredPattern.map((_, i) => (
                                <span key={i} className={`hintDot ${i < progress ? "on" : ""}`} />
                            ))}
                        </span>
                    </div>
                )}

                {/* UNLOCK PANEL */}
                <div className={`panel ${unlocked ? "open" : ""}`}>
                    <div className="panelTitle">Access granted</div>
                    <div className="panelText">
                        Welcome in. Choose where you’re headed.
                    </div>

                    <div className="panelActions">
                        <Link className="btn primary" to="/account/login">
                            Login
                        </Link>
                        <Link className="btn" to="/account/register">
                            Register
                        </Link>
                    </div>

                    {/* DEV/ADMIN convenience (remove if you don’t want it) */}
                    <button className="linkBtn" onClick={clearGate} type="button">
                        Reset gate (local)
                    </button>
                </div>

                <div className="footerLine">
                    <span>© {new Date().getFullYear()} Ezenity</span>
                    <span className="dot">•</span>
                    <span>Automated message, please do not reply</span>
                </div>
            </div>
        </div>
    );
}

/**
 * SkullRow
 * --------
 * Three skulls: big center + two smaller.
 * IDs used for the pattern: left, center, right
 */
function SkullRow({ disabled, onTap }) {
    return (
        <div className="skullRow">
            <button
                className="skullBtn small"
                type="button"
                onClick={() => onTap("left")}
                disabled={disabled}
                aria-label="Left skull"
            >
                <SkullIcon />
            </button>

            <button
                className="skullBtn big"
                type="button"
                onClick={() => onTap("center")}
                disabled={disabled}
                aria-label="Center skull"
            >
                <SkullIcon />
            </button>

            <button
                className="skullBtn small"
                type="button"
                onClick={() => onTap("right")}
                disabled={disabled}
                aria-label="Right skull"
            >
                <SkullIcon />
            </button>
        </div>
    );
}

/**
 * SkullIcon
 * ---------
 * Simple inline SVG skull (placeholder).
 * Replace with your own pirate-skull image anytime:
 * - import skullPng from "@/images/pirate-skull.png";
 * - return <img src={skullPng} alt="" />
 */
function SkullIcon() {
    return (
        <svg
            className="skullSvg"
            viewBox="0 0 128 128"
            role="img"
            aria-hidden="true"
        >
            {/* head */}
            <path
                d="M64 10c-24 0-42 18-42 42 0 18 8 28 18 34v16c0 6 6 10 12 10h24c6 0 12-4 12-10V86c10-6 18-16 18-34 0-24-18-42-42-42z"
                fill="currentColor"
                opacity="0.92"
            />
            {/* eyes */}
            <circle cx="46" cy="52" r="10" fill="#0b0f18" opacity="0.95" />
            <circle cx="82" cy="52" r="10" fill="#0b0f18" opacity="0.95" />
            {/* nose */}
            <path
                d="M64 62c-6 0-9 7-9 12 0 6 4 10 9 10s9-4 9-10c0-5-3-12-9-12z"
                fill="#0b0f18"
                opacity="0.9"
            />
            {/* teeth */}
            <path
                d="M44 92h40v14c0 4-4 6-8 6H52c-4 0-8-2-8-6V92z"
                fill="currentColor"
                opacity="0.85"
            />
            <path d="M56 92v20" stroke="#0b0f18" strokeWidth="3" opacity="0.7" />
            <path d="M64 92v20" stroke="#0b0f18" strokeWidth="3" opacity="0.7" />
            <path d="M72 92v20" stroke="#0b0f18" strokeWidth="3" opacity="0.7" />
        </svg>
    );
}
