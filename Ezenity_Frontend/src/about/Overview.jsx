import React from "react";
import { Link } from "react-router-dom";
import { accountService } from "@/_services";

function Overview() {
    const user = accountService.userValue;

    const steps = [
        {
            title: "Get Invited",
            desc: "Project Ezenity is invite-only. Your invite is your entry point to the community.",
        },
        {
            title: "Earn Your Newcomer Emblem",
            desc: "Complete the newcomer checklist (basic setup + community intro). This unlocks your first emblem/coin tier.",
        },
        {
            title: "Build Reputation (Rep)",
            desc: "Rep is earned through consistency, community contribution, and verified participation — not follower count.",
        },
        {
            title: "Become a Verified Rider",
            desc: "Verified Rider status is the requirement for accessing THE VAULT. Verification rules are explained after onboarding.",
        },
        {
            title: "Access THE VAULT",
            desc: "THE VAULT is the members-only zone: drops, challenges, events, and community-only unlocks.",
        },
    ];

    const pillars = [
        {
            title: "Earn Rep",
            desc:
                "Rep is your trust score. It grows when you show up, contribute, and follow the code. " +
                "It’s designed to reward real community energy.",
            bullets: [
                "Participate in community missions + events",
                "Help other riders (support, guidance, meetups)",
                "Consistency over time (not spam)",
            ],
        },
        {
            title: "Stack Emblems",
            desc:
                "Emblems are achievements, milestones, and seasonal drops. " +
                "They show what you’ve done — and what you’ve earned access to.",
            bullets: [
                "Newcomer Emblem → Verified Rider path",
                "Seasonal drops + limited emblems",
                "Skill / contribution-based unlocks",
            ],
        },
        {
            title: "Earn Coins",
            desc:
                "Coins are an in-app reward currency used for community unlocks. " +
                "They are not a real-world currency and aren’t meant for resale.",
            bullets: [
                "Earned through missions + milestones",
                "Used for Vault unlocks + perks",
                "Tied to behavior + participation",
            ],
        },
    ];

    return (
        <div>
            <div className="mb-3">
                <h3 className="mb-1">Project Ezenity</h3>
                <div className="text-muted">
                    A private rider network built around reputation, progression, and real community.
                </div>
            </div>

            <div className="alert alert-secondary d-flex align-items-center justify-content-between flex-wrap">
                <div className="mb-2 mb-md-0">
                    <strong>Status:</strong>{" "}
                    {user ? (
                        <span>
                            Signed in as <strong>{user.firstName}</strong>
                        </span>
                    ) : (
                        <span>Guest (limited access)</span>
                    )}
                </div>

                <div className="d-flex">
                    {!user ? (
                        <>
                            <Link className="btn btn-sm btn-primary mr-2" to="/account/login">
                                Login
                            </Link>
                            <Link className="btn btn-sm btn-outline-primary" to="/account/register">
                                Register
                            </Link>
                        </>
                    ) : (
                        <Link className="btn btn-sm btn-outline-dark" to="/profile">
                            Go to Profile
                        </Link>
                    )}
                </div>
            </div>

            <hr />

            <h4 className="mb-3">What you can do here</h4>
            <div className="row">
                {pillars.map((p) => (
                    <div className="col-md-4 mb-3" key={p.title}>
                        <div className="card h-100">
                            <div className="card-body">
                                <h5 className="card-title">{p.title}</h5>
                                <p className="card-text text-muted">{p.desc}</p>
                                <ul className="mb-0">
                                    {p.bullets.map((b) => (
                                        <li key={b}>{b}</li>
                                    ))}
                                </ul>
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            <hr />

            <div className="d-flex align-items-center justify-content-between flex-wrap mb-2">
                <h4 className="mb-0">Invite-only + THE VAULT</h4>
                <small className="text-muted">Progression-based access</small>
            </div>

            <p className="text-muted">
                This is not an open-public community. The goal is quality over noise.
                Access to <strong>THE VAULT</strong> requires <strong>Verified Rider</strong> status,
                which is earned through a defined onboarding path.
            </p>

            <div className="card">
                <div className="card-body">
                    <h5 className="card-title">How it works (high level)</h5>
                    <div className="list-group">
                        {steps.map((s, idx) => (
                            <div className="list-group-item" key={s.title}>
                                <div className="d-flex align-items-center justify-content-between">
                                    <strong>
                                        {idx + 1}. {s.title}
                                    </strong>
                                </div>
                                <div className="text-muted">{s.desc}</div>
                            </div>
                        ))}
                    </div>

                    <div className="mt-3 small text-muted">
                        Verification rules (what qualifies you as a Verified Rider) are revealed after onboarding,
                        so people can’t game the system.
                    </div>
                </div>
            </div>

            <hr />

            <h4 className="mb-2">Why this exists</h4>
            <p className="text-muted mb-0">
                Project Ezenity is built for riders who want a real network: progression, accountability,
                and community momentum — not random follows. The platform rewards participation and respect.
            </p>
        </div>
    );
}

export { Overview };
