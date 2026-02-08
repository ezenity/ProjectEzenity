import React, { useEffect, useMemo, useRef, useState } from "react";
import { useHistory } from "react-router-dom";
import { accountService } from "@/_services";

/**
 * Gate.jsx
 * --------
 * Public landing "gate" screen for your underground bike community front.
 *
 * Behavior:
 * - Intro animation runs once, then loops subtle pulsing.
 * - User must click skulls in the correct pattern to "unlock".
 * - On unlock:
 *    - if logged in => /profile
 *    - else => /account/login
 *
 * NOTE:
 * This is NOT real security. It’s just an entrance UX.
 * Real access control must remain on the backend (and PrivateRoute already helps on frontend).
 */

const UNLOCK_KEY = "ez_gate_unlocked";

/**
 * Change this to whatever pattern you want.
 * Example pattern uses 3 skulls: left, center, right.
 *
 * Tip: Keep it short for mobile usability.
 */
const SECRET_PATTERN = ["center", "left", "right", "center"];

function Gate() {
    const history = useHistory();

    const [phase, setPhase] = useState("intro"); // "intro" -> "ready" -> "unlocking"
    const [input, setInput] = useState([]);
    const [shake, setShake] = useState(false);
    const [hint, setHint] = useState(""); // optional small status line
    const timersRef = useRef([]);

    const user = accountService.userValue;

    const isUnlocked = useMemo(() => {
        return sessionStorage.getItem(UNLOCK_KEY) === "1";
    }, []);

    useEffect(() => {
        // If already unlocked this session, send them along
        if (isUnlocked) {
            if (user) history.replace("/profile");
            else history.replace("/account/login");
            return;
        }

        // Run intro for a moment, then allow clicking
        const t = setTimeout(() => setPhase("ready"), 1600);
        timersRef.current.push(t);

        return () => {
            timersRef.current.forEach(clearTimeout);
            timersRef.current = [];
        };
    }, []);

    function resetAttempt(message = "") {
        setInput([]);
        setHint(message);
    }

    function triggerShake() {
        setShake(true);
        const t = setTimeout(() => setShake(false), 550);
        timersRef.current.push(t);
    }

    function onSkullClick(id) {
        if (phase !== "ready") return;

        const next = [...input, id];
        setInput(next);

        // Prefix validation (fail fast)
        for (let i = 0; i < next.length; i++) {
            if (SECRET_PATTERN[i] !== next[i]) {
                triggerShake();
                resetAttempt("Nope.");
                return;
            }
        }

        // Completed
        if (next.length === SECRET_PATTERN.length) {
            setPhase("unlocking");
            setHint("Unlocked.");
            sessionStorage.setItem(UNLOCK_KEY, "1");

            const t = setTimeout(() => {
                if (accountService.userValue) history.push("/profile");
                else history.push("/account/login");
            }, 650);

            timersRef.current.push(t);
            return;
        }

        // Otherwise show progress
        setHint(`${next.length}/${SECRET_PATTERN.length}`);
    }

    return (
        <div className={`gate ${shake ? "gate--shake" : ""} gate--${phase}`}>
            {/* Animated background layers */}
            <div className="gate__bg" aria-hidden="true" />
            <div className="gate__meteor" aria-hidden="true" />

            {/* Content */}
            <div className="gate__content">
                <div className="gate__brand">
                    <div className="gate__brandTitle">Ezenity</div>
                    <div className="gate__brandSub">Midnight Riders</div>
                </div>

                <div className="gate__panel">
                    <div className="gate__row">
                        <SkullButton id="left" size="sm" onClick={onSkullClick} disabled={phase !== "ready"} />
                        <SkullButton id="center" size="lg" onClick={onSkullClick} disabled={phase !== "ready"} />
                        <SkullButton id="right" size="sm" onClick={onSkullClick} disabled={phase !== "ready"} />
                    </div>

                    <div className="gate__status">
                        {phase === "intro" && <span>Loading…</span>}
                        {phase === "ready" && <span>{hint || "Tap the mark."}</span>}
                        {phase === "unlocking" && <span>Entering…</span>}
                    </div>

                    {/* Small progress dots */}
                    <div className="gate__dots" aria-hidden="true">
                        {SECRET_PATTERN.map((_, idx) => (
                            <span key={idx} className={`gate__dot ${idx < input.length ? "is-on" : ""}`} />
                        ))}
                    </div>

                    {/* Optional fallback link (you can remove this if you want it *only* pattern-based) */}
                    <div className="gate__fallback">
                        <button
                            className="gate__link"
                            type="button"
                            onClick={() => history.push("/account/login")}
                        >
                            Login
                        </button>
                        <span className="gate__sep">•</span>
                        <button
                            className="gate__link"
                            type="button"
                            onClick={() => history.push("/account/register")}
                        >
                            Register
                        </button>
                    </div>
                </div>

                <div className="gate__footer">
                    <span>© {new Date().getFullYear()} Ezenity</span>
                </div>
            </div>
        </div>
    );
}

/**
 * SkullButton
 * -----------
 * Inline SVG pirate-skull placeholder.
 * When you send your “real” skull art later, we can swap the SVG path or use an <img>.
 */
function SkullButton({ id, size, onClick, disabled }) {
    return (
        <button
            type="button"
            className={`skull skull--${size}`}
            onClick={() => onClick(id)}
            disabled={disabled}
            aria-label={`Skull ${id}`}
        >
            <span className="skull__ring" aria-hidden="true" />
            <svg className="skull__svg" viewBox="0 0 64 64" aria-hidden="true">
                {/* Simple pirate skull vibe (placeholder) */}
                <path d="M32 6c-12 0-22 9-22 21 0 7 3 13 9 17v6c0 2 2 4 4 4h18c2 0 4-2 4-4v-6c6-4 9-10 9-17C54 15 44 6 32 6z" />
                <path d="M22 30c0 3 2 6 6 6s6-3 6-6-2-6-6-6-6 3-6 6zm14 0c0 3 2 6 6 6s6-3 6-6-2-6-6-6-6 3-6 6z" />
                <path d="M28 44c2 2 6 2 8 0" fill="none" strokeWidth="3" strokeLinecap="round" />
                {/* Crossbones */}
                <path d="M12 54l14-10M12 44l14 10M52 54L38 44M52 44L38 54" fill="none" strokeWidth="4" strokeLinecap="round" />
            </svg>
        </button>
    );
}

export { Gate };
