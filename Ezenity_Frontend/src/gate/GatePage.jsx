import React, { useEffect, useMemo, useRef, useState } from "react";
import { useHistory } from "react-router-dom";
import { accountService } from "@/_services";
import { isGateUnlocked, setGateUnlocked } from "@/_helpers/gate";
import "./GatePage.less";

import coin_Main from "@/images/skulls/skull-coin-3.png";
import emblem_One from "@/images/skulls/skull-emblem-1.png";
import emblem_Three from "@/images/skulls/skull-emblem-3.png";

const SECRET_PATTERN = ["coinMain", "emblemOne", "emblemThree", "coinMain"];
const INTRO_MS = 900;
const RESET_AFTER_MS = 2500;
const ENTER_DELAY_MS = 520;

export default function GatePage() {
    const history = useHistory();

    const [phase, setPhase] = useState("intro"); // intro -> ready -> entering
    const [input, setInput] = useState([]);
    const [shake, setShake] = useState(false);
    const [lastHit, setLastHit] = useState(null);
    const [status, setStatus] = useState("Tap the sigil.");

    const timers = useRef([]);
    const resetTimer = useRef(null);
    const enteringRef = useRef(false);

    const coinConfig = useMemo(
        () => [
            { id: "emblemOne", img: emblem_One, size: "sm" },
            { id: "emblemThree", img: emblem_Three, size: "sm" },
            { id: "coinMain", img: coin_Main, size: "lg" },
        ],
        []
    );

    useEffect(() => {
        // If already unlocked this session, go straight in
        if (isGateUnlocked()) {
            const current = accountService.userValue;
            history.replace(current ? "/profile" : "/account/login");
            return;
        }

        const t = setTimeout(() => {
            setPhase("ready");
            setStatus("Tap the sigil.");
        }, INTRO_MS);

        timers.current.push(t);

        return () => {
            timers.current.forEach(clearTimeout);
            timers.current = [];
            if (resetTimer.current) clearTimeout(resetTimer.current);
        };
    }, [history]);

    function armResetTimeout() {
        if (resetTimer.current) clearTimeout(resetTimer.current);
        resetTimer.current = setTimeout(() => {
            setInput([]);
            setStatus("Tap the sigil.");
        }, RESET_AFTER_MS);
    }

    function triggerShake() {
        setShake(true);
        const t = setTimeout(() => setShake(false), 520);
        timers.current.push(t);
    }

    function deny() {
        setStatus("Denied.");
        triggerShake();
        setInput([]);
        const t = setTimeout(() => setStatus("Tap the sigil."), 700);
        timers.current.push(t);
    }

    function enter() {
        if (enteringRef.current) return;
        enteringRef.current = true;

        setPhase("entering");
        setStatus("Entering…");

        // set session flag used by App + GateGuardRoute
        setGateUnlocked();

        const dest = accountService.userValue ? "/profile" : "/account/login";

        // SPA navigation first
        const t = setTimeout(() => {
            try {
                history.replace(dest);
            } catch {
                window.location.assign(dest);
                return;
            }

            // fallback: if something prevents SPA render, force navigation
            const t2 = setTimeout(() => {
                if (window.location.pathname !== dest) {
                    window.location.assign(dest);
                }
            }, 150);

            timers.current.push(t2);
        }, ENTER_DELAY_MS);

        timers.current.push(t);
    }

    function onCoinClick(id) {
        if (phase !== "ready") return;

        // click feedback
        setLastHit(id);
        const hitT = setTimeout(() => setLastHit(null), 240);
        timers.current.push(hitT);

        const next = [...input, id];
        setInput(next);
        armResetTimeout();

        // prefix validation
        for (let i = 0; i < next.length; i++) {
            if (SECRET_PATTERN[i] !== next[i]) {
                deny();
                return;
            }
        }

        // completed
        if (next.length === SECRET_PATTERN.length) {
            enter();
        } else {
            // keep it subtle — no passcode/progress indicator
            setStatus("…");
            const t = setTimeout(() => setStatus("Tap the sigil."), 350);
            timers.current.push(t);
        }
    }

    return (
        <div className={`gateRoot2 ${shake ? "shake" : ""} phase-${phase}`}>
            <div className="bg2 base" />
            <div className="bg2 neon" />
            <div className="bg2 smoke" />
            <div className="bg2 haze" />
            <div className="bg2 embers" />
            <div className="bg2 streaks" />
            <div className="vignette2" />

            {/* HUD ELEMENTS */}
            <div className="hud hudLeft" aria-hidden="true">
                <div className="hudTitle">MIDNIGHT RIDERS</div>
                <div className="hudLine" />
                <div className="hudText">Earn rep • coins • emblems</div>
            </div>

            <div className="hud hudRight" aria-hidden="true">
                <div className="hudTitle">NIGHT OPS</div>
                <div className="hudLine" />
                <div className="hudText">Invite-only access</div>
            </div>

            {/* CONTENT */}
            <div className="content2">
                <div className="brand2">
                    <div className="brandName2">Ezenity</div>
                    <div className="brandTag2">Underground ♦ Invite-only ♦ Ride smart</div>
                </div>

                <div className="sigil2" aria-label="Gate sigil">
                    {coinConfig.map((c) => (
                        <button
                            key={c.id}
                            type="button"
                            className={`coinBtn ${c.size} ${lastHit === c.id ? "is-hit" : ""}`}
                            data-id={c.id}
                            onClick={() => onCoinClick(c.id)}
                            disabled={phase !== "ready"}
                            aria-label={`sigil-${c.id}`}
                        >
                            <span className="coinRing" aria-hidden="true" />
                            <span className="coinGlint" aria-hidden="true" />
                            <img className="coinImg" src={c.img} alt="" draggable="false" />
                            <span className="coinBurst" aria-hidden="true" />
                        </button>
                    ))}
                </div>

                <div className={`status2 ${phase === "entering" ? "statusEnter" : ""}`} aria-live="polite">
                    <span>{phase === "intro" ? "Loading…" : status}</span>
                </div>

                <div className="footer2">
                    <span>© {new Date().getFullYear()} Ezenity</span>
                    <span className="dot">•</span>
                    <span>Automated message, please do not reply</span>
                </div>
            </div>
        </div>
    );
}
