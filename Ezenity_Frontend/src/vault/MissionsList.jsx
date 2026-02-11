import React, { useEffect, useState } from "react";
import { Link, useRouteMatch } from "react-router-dom";
import { vaultService } from "@/_services/vault.service";

function MissionsList() {
    const { url } = useRouteMatch();
    const [missions, setMissions] = useState([]);

    useEffect(() => {
        vaultService.getMissions().then(setMissions);
    }, []);

    return (
        <div className="row">
            {missions.map((m) => (
                <div key={m.id} className="col-md-6 col-lg-4 mb-3">
                    <div className="card h-100 shadow-sm">
                        <div className="card-body">
                            <h5 className="card-title">{m.title}</h5>
                            <p className="card-text text-muted">{m.objective}</p>

                            <div className="d-flex flex-wrap gap-2">
                                <span className="badge bg-dark">+{m.rewards.rep} Rep</span>
                                <span className="badge bg-secondary">+{m.rewards.coins} Coins</span>
                                {m.rewards.emblems.map((e) => (
                                    <span key={e.id} className="badge bg-success">
                                        Emblem: {e.name}
                                    </span>
                                ))}
                            </div>
                        </div>

                        <div className="card-footer bg-transparent">
                            <Link to={`${url}/${m.id}`} className="btn btn-outline-primary w-100">
                                View Mission
                            </Link>
                        </div>
                    </div>
                </div>
            ))}
        </div>
    );
}

export { MissionsList };
