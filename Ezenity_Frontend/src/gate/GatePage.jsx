import React, { useEffect, useMemo, useRef, useState } from "react";
import { useHistory } from "react-router-dom";
import { accountService } from "@/_services";
import { isGateUnlocked, unlockGate } from "@/_helpers/gate";
import "./GatePage.less";

import coin_Main from "@/images/skulls/skull-coin-3.png";
import emblem_One from "@/images/skulls/skull-emblem-1.png";
import emblem_Three from "@/images/skulls/skull-emblem-3.png";

const SECRET_PATTERN = ["coinMain", "emblemOne", "emblemThree", "coinMain"];

export default function GatePage() {
    const history = useHistory();

    const [phase, setPhase] = useState("intro"); // intro -> ready -> entering
    const [input, setInput] = useState([]);
    const [shake, setShake] = useState(false);
    const [lastHit, setLastHit] = useState(null);

    const timers = useRef([]);
    const resetTimer = useRef(null);

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

        const t = setTimeout(() => setPhase("ready"), 900);
        timers.current.push(t);

        return () => {
            timers.current.forEach(clearTimeout);
            timers.current = [];
            if (resetTimer.current) clearTimeout(resetTimer.current);
        };
    }, [history]);

    function armResetTimeout() {
        if (resetTimer.current) clearTimeout(resetTimer.current);
        resetTimer.current = setTimeout(() => setInput([]), 2500);
    }

    function triggerShake() {
        setShake(true);
        const t = setTimeout(() => setShake(false), 520);
        timers.current.push(t);
    }

    function onCoinClick(id) {
        if (phase !== "ready") return;

        setLastHit(id);
        const hitT = setTimeout(() => setLastHit(null), 220);
        timers.current.push(hitT);

        const next = [...input, id];
        setInput(next);
        armResetTimeout();

        // prefix validation
        for (let i = 0; i < next.length; i++) {
            if (SECRET_PATTERN[i] !== next[i]) {
                triggerShake();
                setInput([]);
                return;
            }
        }

        // completed
        if (next.length === SECRET_PATTERN.length) {
            setPhase("entering");
            unlockGate(); // <--- IMPORTANT: matches GateGuardRoute + App

            const t = setTimeout(() => {
                const current = accountService.userValue;
                history.push(current ? "/profile" : "/account/login");
            }, 450);

            timers.current.push(t);
        }
    }

    return (
        <div className={`gateRoot2 ${shake ? "shake" : ""} phase-${phase}`}>
            <div className="bg2 base" />
            <div className="bg2 haze" />
            <div className="bg2 embers" />
            <div className="bg2 streaks" />
            <div className="vignette2" />

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
                            <img className="coinImg" src={c.img} alt="" draggable="false" />
                            <span className="coinBurst" aria-hidden="true" />
                        </button>
                    ))}
                </div>

                <div className="status2">
                    {phase === "intro" && <span>Loading…</span>}
                    {phase === "ready" && <span>Tap the sigil.</span>}
                    {phase === "entering" && <span>Entering…</span>}
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
