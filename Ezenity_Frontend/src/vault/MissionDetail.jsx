import React, { useEffect, useMemo, useState } from "react";
import { useParams } from "react-router-dom";
import { vaultService } from "@/_services/vault.service";
import { fetchWrapper } from "@/_helpers";

function MissionDetail() {
    const { missionId } = useParams();
    const [mission, setMission] = useState(null);

    useEffect(() => {
        vaultService.getMissionById(missionId).then(setMission);
    }, [missionId]);

    const [links, setLinks] = useState({ youtube: "", instagram: "", tiktok: "", facebook: "" });
    const [images, setImages] = useState([]);
    const [video, setVideo] = useState(null);
    const canSubmit = useMemo(() => mission?.proof, [mission]);

    if (!mission) return <div className="text-muted">Mission not found.</div>;

    async function uploadOne(file) {
        const fd = new FormData();
        fd.append("file", file);
        // uses your updated fetchWrapper FormData support
        return fetchWrapper.post("/api/v1/files/upload", fd);
    }

    async function handleSubmitProof() {
        // Phase B will store submission records. For now just prove uploads work.
        const uploaded = [];

        for (const img of images) uploaded.push(await uploadOne(img));
        if (video) uploaded.push(await uploadOne(video));

        console.log("Uploaded proof:", uploaded, "Links:", links);
        alert("Proof uploaded (check console). Next: save a Submission record.");
    }

    return (
        <div className="card shadow-sm">
            <div className="card-body">
                <h3 className="mb-2">{mission.title}</h3>
                <p className="text-muted mb-3">{mission.description}</p>

                <h6>Objective</h6>
                <p>{mission.objective}</p>

                <h6>Rewards</h6>
                <div className="d-flex flex-wrap gap-2 mb-4">
                    <span className="badge bg-dark">+{mission.rewards.rep} Rep</span>
                    <span className="badge bg-secondary">+{mission.rewards.coins} Coins</span>
                    {mission.rewards.emblems.map((e) => (
                        <span key={e.id} className="badge bg-success">
                            {e.name}
                        </span>
                    ))}
                </div>

                <hr />

                <h5 className="mb-2">Submit Proof</h5>
                <p className="text-muted">
                    Upload media or provide social links. This will be used for approval and completion.
                </p>

                {canSubmit?.allowLinks && (
                    <div className="row">
                        {["youtube", "instagram", "tiktok", "facebook"].map((k) => (
                            <div className="col-md-6 mb-2" key={k}>
                                <label className="form-label text-capitalize">{k} link</label>
                                <input
                                    className="form-control"
                                    value={links[k]}
                                    onChange={(e) => setLinks((p) => ({ ...p, [k]: e.target.value }))}
                                    placeholder={`Paste ${k} URL`}
                                />
                            </div>
                        ))}
                    </div>
                )}

                {canSubmit?.allowUploads && (
                    <div className="row mt-2">
                        <div className="col-md-6 mb-3">
                            <label className="form-label">Images (max {mission.proof.maxImages})</label>
                            <input
                                type="file"
                                className="form-control"
                                accept="image/*"
                                multiple
                                onChange={(e) => setImages(Array.from(e.target.files || []).slice(0, mission.proof.maxImages))}
                            />
                        </div>

                        <div className="col-md-6 mb-3">
                            <label className="form-label">Video (max {mission.proof.maxVideo})</label>
                            <input
                                type="file"
                                className="form-control"
                                accept="video/*"
                                onChange={(e) => setVideo((e.target.files || [])[0] || null)}
                            />
                        </div>
                    </div>
                )}

                <button className="btn btn-primary" onClick={handleSubmitProof}>
                    Submit Proof
                </button>

                <hr className="my-4" />

                <h5>Comments / Posts (Phase C)</h5>
                <p className="text-muted mb-0">
                    Next we’ll add a submission thread here that shows user icon + username,
                    and lets admins approve completion.
                </p>
            </div>
        </div>
    );
}

export { MissionDetail };
