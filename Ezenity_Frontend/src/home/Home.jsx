// /src/home/Home.jsx
import React from "react";
import { Link } from "react-router-dom";
import { accountService } from "@/_services";

function FeatureCard({ title, text }) {
    return (
        <div className="col-md-4 mb-3">
            <div className="card h-100 shadow-sm">
                <div className="card-body">
                    <h5 className="card-title">{title}</h5>
                    <p className="card-text">{text}</p>
                </div>
            </div>
        </div>
    );
}

function Home() {
    const user = accountService.userValue;

    return (
        <div className="p-4">
            <div className="container">
                <div className="mb-4">
                    <h1 className="mb-2">
                        {user ? `Welcome back, ${user.firstName}!` : "Welcome to After Hours Crew"}
                    </h1>
                    <p className="text-muted mb-0">
                        Invite-only rider community focused on progression: Rep → Emblems → Coins → access.
                    </p>
                </div>

                {/* Core pillars */}
                <div className="row mb-2">
                    <FeatureCard
                        title="Earn Rep"
                        text="Build reputation through participation and consistency. Rep is your foundation and unlocks the next layers of progression."
                    />
                    <FeatureCard
                        title="Stack Emblems"
                        text="Emblems represent milestones and achievements. They’re your visible proof of progress inside the community."
                    />
                    <FeatureCard
                        title="Earn Coins"
                        text="Coins are earned through progression and activity. Certain areas and perks are gated behind coin/emblem levels."
                    />
                </div>

                {/* Invite-only + Vault */}
                <div className="card shadow-sm mb-4">
                    <div className="card-body">
                        <h4 className="card-title mb-2">Invite Only • Verified Riders • The Vault</h4>

                        <p className="mb-2">
                            Access is invite-only. Once you’re in, progression starts immediately with an entry-level
                            onboarding path (your “newcomer” stage).
                        </p>

                        <p className="mb-2">
                            <strong>The Vault</strong> is gated content and access. To enter, you must be a{" "}
                            <strong>Verified Rider</strong>. The verification checklist and rules are provided
                            inside your onboarding once you’re eligible to begin verification.
                        </p>

                        {!user ? (
                            <div className="d-flex flex-wrap gap-2">
                                <Link className="btn btn-primary mr-2" to="/account/login">
                                    Login
                                </Link>
                                <Link className="btn btn-outline-secondary" to="/about">
                                    What is this?
                                </Link>
                            </div>
                        ) : (
                            <div className="d-flex flex-wrap gap-2">
                                <Link className="btn btn-primary mr-2" to="/profile">
                                    Go to Profile
                                </Link>
                                <Link className="btn btn-outline-secondary" to="/about">
                                    Learn More
                                </Link>
                            </div>
                        )}
                    </div>
                </div>

                {/* Next steps */}
                <div className="row">
                    <div className="col-md-6 mb-3">
                        <div className="card h-100 shadow-sm">
                            <div className="card-body">
                                <h5 className="card-title">Newcomer Stage</h5>
                                <p className="card-text">
                                    Your entry stage explains how Rep, Emblems, and Coins work and what actions count
                                    toward progression. It also introduces the path to becoming Verified.
                                </p>
                            </div>
                        </div>
                    </div>

                    <div className="col-md-6 mb-3">
                        <div className="card h-100 shadow-sm">
                            <div className="card-body">
                                <h5 className="card-title">Verified Rider Path</h5>
                                <p className="card-text">
                                    Verification is required for Vault access. When you’re eligible, the app will show
                                    the checklist and rules you must complete to become Verified.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    );
}

export { Home };
