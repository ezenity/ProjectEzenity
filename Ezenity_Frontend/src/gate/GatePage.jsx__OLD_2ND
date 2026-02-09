import React, { useEffect, useMemo, useRef, useState } from "react";
import { Link } from "react-router-dom";
import "./GatePage.less";

// Keep the key consistent with your helper + guard
const UNLOCK_KEY = "ez_gate_unlocked";

export default function GatePage() {
    // Pattern: matches “one big + two small” layout
    // Allowed IDs: "smallTop", "smallBottom", "big"
    const requiredPattern = useMemo(
        () => ["big", "smallTop", "smallBottom", "big"],
        []
    );

    const INTRO_MS = 2600;       // faster, punchier intro
    const RESET_AFTER_MS = 2200; // reset if user pauses

    const [introDone, setIntroDone] = useState(false);
    const [armed, setArmed] = useState(false);
    const [unlocked, setUnlocked] = useState(false);

    const [progress, setProgress] = useState(0); // internal only (no UI)
    const [shake, setShake] = useState(false);

    // per-click feedback (no progress reveal)
    const [hitId, setHitId] = useState(null);
    const hitTimer = useRef(null);
    const resetTimer = useRef(null);

    useEffect(() => {
        // if already unlocked in this tab session, just show panel immediately
        if (sessionStorage.getItem(UNLOCK_KEY) === "1") {
            setIntroDone(true);
            setArmed(false);
            setUnlocked(true);
            return;
        }

        const t = setTimeout(() => {
            setIntroDone(true);
            setArmed(true);
        }, INTRO_MS);

        return () => clearTimeout(t);
    }, []);

    function armResetTimer() {
        if (resetTimer.current) clearTimeout(resetTimer.current);
        resetTimer.current = setTimeout(() => setProgress(0), RESET_AFTER_MS);
    }

    function pulseHit(id) {
        setHitId(id);
        if (hitTimer.current) clearTimeout(hitTimer.current);
        hitTimer.current = setTimeout(() => setHitId(null), 220);
    }

    function onTap(id) {
        if (!armed || unlocked) return;

        pulseHit(id);

        const expected = requiredPattern[progress];
        if (id === expected) {
            const next = progress + 1;
            setProgress(next);
            armResetTimer();

            if (next >= requiredPattern.length) {
                // unlocked
                sessionStorage.setItem(UNLOCK_KEY, "1");
                setUnlocked(true);
                setArmed(false);
                setProgress(0);
            }
            return;
        }

        // wrong input -> shake + reset
        setShake(true);
        setProgress(0);
        setTimeout(() => setShake(false), 520);
    }

    return (
        <div className={`gateRoot ${introDone ? "introDone" : "introPlaying"} ${shake ? "shake" : ""}`}>
            {/* BACKGROUND */}
            <div className="bgLayer bgBase" />
            <div className="bgLayer bgBurst" />
            <div className="bgLayer lava" />
            <div className="bgLayer stars" />
            <div className="vignette" />

            {/* CONTENT */}
            <div className="content">
                <div className="brandRow">
                    <div className="brandName">Ezenity</div>
                    <div className="brandTag">Underground ♦ Invite-only ♦ Ride smart</div>
                </div>

                {/* SIGIL / SKULLS */}
                <div className={`sigilStage ${introDone ? "show" : ""}`}>
                    <SigilSkulls
                        disabled={!armed || unlocked}
                        onTap={onTap}
                        hitId={hitId}
                    />
                    {!unlocked && (
                        <div className="subHint">
                            Tap the sigil.
                        </div>
                    )}
                </div>

                {/* UNLOCK PANEL (only after success) */}
                <div className={`panel ${unlocked ? "open" : ""}`}>
                    <div className="panelTitle">Access granted</div>
                    <div className="panelText">Choose where you’re headed.</div>

                    <div className="panelActions">
                        <Link className="btn primary" to="/account/login">Login</Link>
                        <Link className="btn" to="/account/register">Register</Link>
                    </div>
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

function SigilSkulls({ disabled, onTap, hitId }) {
    return (
        <div className="sigil">
            <div className="sigil__ring" aria-hidden="true" />

            <button
                className={`sigil__skull sigil__skull--smallTop ${hitId === "smallTop" ? "is-hit" : ""}`}
                type="button"
                disabled={disabled}
                onClick={() => onTap("smallTop")}
                aria-label="Small skull top"
            >
                <SkullIcon />
            </button>

            <button
                className={`sigil__skull sigil__skull--smallBottom ${hitId === "smallBottom" ? "is-hit" : ""}`}
                type="button"
                disabled={disabled}
                onClick={() => onTap("smallBottom")}
                aria-label="Small skull bottom"
            >
                <SkullIcon />
            </button>

            <button
                className={`sigil__skull sigil__skull--big ${hitId === "big" ? "is-hit" : ""}`}
                type="button"
                disabled={disabled}
                onClick={() => onTap("big")}
                aria-label="Big skull"
            >
                <SkullIcon />
            </button>

            {/* Crossbones accent (non-click) */}
            <div className="sigil__bones" aria-hidden="true" />
        </div>
    );
}

function SkullIcon() {
    return (
        <svg className="skullSvg" viewBox="0 0 128 128" aria-hidden="true">
            <path
                d="M64 10c-24 0-42 18-42 42 0 18 8 28 18 34v16c0 6 6 10 12 10h24c6 0 12-4 12-10V86c10-6 18-16 18-34 0-24-18-42-42-42z"
                fill="currentColor"
                opacity="0.92"
            />
            <circle cx="46" cy="52" r="10" fill="#0b0f18" opacity="0.95" />
            <circle cx="82" cy="52" r="10" fill="#0b0f18" opacity="0.95" />
            <path
                d="M64 62c-6 0-9 7-9 12 0 6 4 10 9 10s9-4 9-10c0-5-3-12-9-12z"
                fill="#0b0f18"
                opacity="0.9"
            />
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
