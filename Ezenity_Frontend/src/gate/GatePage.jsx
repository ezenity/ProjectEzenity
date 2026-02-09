import React, { useEffect, useMemo, useRef, useState } from "react";
import { useHistory } from "react-router-dom";
import { accountService } from "@/_services";
import "./GatePage.less";

// Your real coin assets
import coin_Main from "@/images/skulls/skull-coin-3.png";
import emblem_One from "@/images/skulls/skull-emblem-1.png";
import emblem_Three from "@/images/skulls/skull-emblem-3.png";

const UNLOCK_KEY = "ez_gate_unlocked";

// Change this pattern whenever you want (IDs must match coinConfig ids)
const SECRET_PATTERN = ["coinMain", "emblemOne", "emblemThree", "coinMain"];

export default function GatePage() {
    const history = useHistory();

    const [phase, setPhase] = useState("intro"); // intro -> ready -> entering
    const [input, setInput] = useState([]);
    const [shake, setShake] = useState(false);
    const [lastHit, setLastHit] = useState(null);

    const timers = useRef([]);
    const resetTimer = useRef(null);

    const user = accountService.userValue;

    const coinConfig = useMemo(
        () => [
            // Layout matches your “one large + two small” idea, with MORE spacing
            {
                id: "emblemOne",
                img: emblem_One,
                size: "sm",
                style: { "--x": "-455px", "--y": "-80px" }, // left/top
            },
            {
                id: "emblemThree",
                img: emblem_Three,
                size: "sm",
                style: { "--x": "485px", "--y": "-80px" }, // left/bottom
            },
            {
                id: "coinMain",
                img: coin_Main,
                size: "lg",
                style: { "--x": "0px", "--y": "-20px" }, // right/large
            },
        ],
        []
    );

    useEffect(() => {
        // If already unlocked this session, go straight in
        if (sessionStorage.getItem(UNLOCK_KEY) === "1") {
            if (user) history.replace("/profile");
            else history.replace("/account/login");
            return;
        }

        const t = setTimeout(() => setPhase("ready"), 1400);
        timers.current.push(t);

        return () => {
            timers.current.forEach(clearTimeout);
            timers.current = [];
            if (resetTimer.current) clearTimeout(resetTimer.current);
        };
    }, []);

    function armResetTimeout() {
        if (resetTimer.current) clearTimeout(resetTimer.current);
        resetTimer.current = setTimeout(() => {
            setInput([]);
        }, 3000);
    }

    function triggerShake() {
        setShake(true);
        const t = setTimeout(() => setShake(false), 520);
        timers.current.push(t);
    }

    function onCoinClick(id) {
        if (phase !== "ready") return;

        // click feedback (ring/burst)
        setLastHit(id);
        const hitT = setTimeout(() => setLastHit(null), 220);
        timers.current.push(hitT);

        const next = [...input, id];
        setInput(next);
        armResetTimeout();

        // prefix validation: fail fast
        for (let i = 0; i < next.length; i++) {
            if (SECRET_PATTERN[i] !== next[i]) {
                triggerShake();
                setInput([]);
                return;
            }
        }

        // success
        if (next.length === SECRET_PATTERN.length) {
            setPhase("entering");
            sessionStorage.setItem(UNLOCK_KEY, "1");

            // IMPORTANT: ACTUALLY NAVIGATE (fixes your “Entering…” hang)
            const t = setTimeout(() => {
                const current = accountService.userValue;
                if (current) history.push("/profile");
                else history.push("/account/login");
            }, 700);

            timers.current.push(t);
        }
    }

    return (
        <div className={`gateRoot2 ${shake ? "shake" : ""} phase-${phase}`}>
            {/* background */}
            <div className="bg2 base" />
            <div className="bg2 haze" />
            <div className="bg2 embers" />
            <div className="bg2 streaks" />
            <div className="vignette2" />

            {/* content */}
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
                            style={c.style}
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
