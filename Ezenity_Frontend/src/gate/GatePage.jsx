import React, { useEffect, useMemo, useRef, useState } from "react";
import { useHistory } from "react-router-dom";
import { accountService } from "@/_services";
import { isGateUnlocked, unlockGate } from "@/_helpers/gate";
import sigilCoin from "@/images/skulls/skull-coin-3.png";
import "./GatePage.less";

/**
 * GatePage
 * - No progress dots / no "X/Y"
 * - Stronger visuals (fog/embers/noise + intro sweep)
 * - Click feedback (glow + pulse) but no length indicator
 * - Unlock => /profile if logged in, else /account/login
 *
 * NOTE: UX gate only. Not security.
 */

// Choose ANY ids you want, just keep them consistent
const SIGILS = [
    { id: "alpha", size: "xl", x: "60%", y: "52%", r: "6deg", delay: "0.0s" },  // big
    { id: "beta", size: "md", x: "46%", y: "44%", r: "-10deg", delay: "0.2s" }, // small
    { id: "gamma", size: "sm", x: "44%", y: "63%", r: "12deg", delay: "0.35s" }, // small
];

// Your secret pattern (no UI reveals)
const REQUIRED_PATTERN = ["beta", "alpha", "gamma", "alpha"];

export default function GatePage() {
    const history = useHistory();

    const requiredPattern = useMemo(() => REQUIRED_PATTERN, []);
    const [phase, setPhase] = useState("intro"); // intro | armed | unlocking
    const [seq, setSeq] = useState([]);
    const [flash, setFlash] = useState(""); // good | bad | ""
    const [lastHit, setLastHit] = useState(""); // which sigil was clicked (for glow)
    const resetTimerRef = useRef(null);

    useEffect(() => {
        // If already unlocked in this tab, bounce them forward
        if (isGateUnlocked()) {
            routeAfterUnlock();
            return;
        }

        // Intro plays briefly, then arms
        const t = setTimeout(() => setPhase("armed"), 1800);
        return () => clearTimeout(t);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    function routeAfterUnlock() {
        if (accountService.userValue) history.replace("/profile");
        else history.replace("/account/login");
    }

    function pulse(type) {
        setFlash(type);
        window.setTimeout(() => setFlash(""), 420);
    }

    function resetAttempt() {
        setSeq([]);
        if (resetTimerRef.current) {
            clearTimeout(resetTimerRef.current);
            resetTimerRef.current = null;
        }
    }

    function armResetTimer() {
        if (resetTimerRef.current) clearTimeout(resetTimerRef.current);
        resetTimerRef.current = setTimeout(() => resetAttempt(), 2200);
    }

    function onSigilClick(id) {
        // Always show "clicked" feedback even during intro (so it never feels dead)
        setLastHit(id);
        window.setTimeout(() => setLastHit(""), 180);

        if (phase !== "armed") {
            pulse("bad"); // “not ready” vibe
            return;
        }

        const next = [...seq, id];

        // Fail-fast prefix check (but DO NOT display progress length)
        for (let i = 0; i < next.length; i++) {
            if (requiredPattern[i] !== next[i]) {
                pulse("bad");
                resetAttempt();
                return;
            }
        }

        // Correct so far
        pulse("good");
        setSeq(next);
        armResetTimer();

        // Completed
        if (next.length === requiredPattern.length) {
            setPhase("unlocking");
            unlockGate();

            // dramatic exit delay
            window.setTimeout(() => routeAfterUnlock(), 700);
        }
    }

    return (
        <div className={`gateRoot gateRoot--${phase} ${flash ? `gateRoot--${flash}` : ""}`}>
            {/* BACKGROUND / FX (pointer-events disabled in CSS so clicks always work) */}
            <div className="gateLayer gateLayer--green" aria-hidden="true" />
            <div className="gateLayer gateLayer--red" aria-hidden="true" />
            <div className="gateLayer gateLayer--steel" aria-hidden="true" />
            <div className="gateLayer gateLayer--gold" aria-hidden="true" />
            <div className="gateLayer gateLayer--lava" aria-hidden="true" />
            <div className="gateFog" aria-hidden="true" />
            <div className="gateNoise" aria-hidden="true" />
            <div className="gateEmbers" aria-hidden="true" />
            <div className="gateMeteor" aria-hidden="true" />

            <div className="gateContent">
                <div className="gateBrand">
                    <div className="gateTitle">Ezenity</div>
                    <div className="gateTag">Underground ◆ Invite-only ◆ Ride smart</div>
                </div>

                {/* Sigils scattered (not just dead center) */}
                <div className="sigilField" aria-label="Gate sigils">
                    {SIGILS.map((s) => (
                        <button
                            key={s.id}
                            type="button"
                            className={[
                                "sigil",
                                `sigil--${s.size}`,
                                lastHit === s.id ? "is-hit" : "",
                                phase !== "armed" ? "is-dim" : "",
                            ].join(" ")}
                            style={{
                                "--x": s.x,
                                "--y": s.y,
                                "--r": s.r,
                                "--d": s.delay,
                            }}
                            onClick={() => onSigilClick(s.id)}
                            aria-label={`Sigil ${s.id}`}
                        >
                            <span className="sigil__ring" aria-hidden="true" />
                            <img className="sigil__img" src={sigilCoin} alt="" draggable="false" />
                            <span className="sigil__shine" aria-hidden="true" />
                        </button>
                    ))}
                </div>

                {/* No progress indicator */}
                <div className="gateHint">
                    {phase === "intro" && <span>Loading…</span>}
                    {phase === "armed" && <span>Tap the sigils.</span>}
                    {phase === "unlocking" && <span>Entering…</span>}
                </div>

                <div className="gateFooter">
                    <span>© {new Date().getFullYear()} Ezenity</span>
                    <span className="sep">•</span>
                    <span>Automated message, please do not reply</span>
                </div>
            </div>
        </div>
    );
}
