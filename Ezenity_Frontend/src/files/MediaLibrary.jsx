import React, { useEffect, useState } from "react";
import { fileService } from "@/_services/file.service";

export function MediaLibrary({ scope = "vault" }) {
    const [items, setItems] = useState([]);
    const [busy, setBusy] = useState(false);
    const [err, setErr] = useState("");

    async function load() {
        setErr("");
        const res = await fileService.list(scope);
        const data = res?.data ?? res; // supports your ApiResponse envelope
        setItems(Array.isArray(data) ? data : []);
    }

    useEffect(() => {
        load().catch((e) => setErr(String(e)));
    }, [scope]);

    async function onPick(e) {
        const file = e.target.files?.[0];
        if (!file) return;

        setBusy(true);
        setErr("");
        try {
            const res = await fileService.upload(file, { scope });
            const uploaded = (res?.data ?? res)?.file ?? (res?.data ?? res);
            // reload for canonical list/order
            await load();
        } catch (e) {
            setErr(String(e));
        } finally {
            setBusy(false);
            e.target.value = ""; // allow re-pick same file
        }
    }

    return (
        <div className="container p-3">
            <h3 className="mb-2">Media Library ({scope})</h3>

            <div className="mb-3">
                <input type="file" onChange={onPick} disabled={busy} />
                {busy && <div className="small mt-2">Uploading…</div>}
                {err && <div className="alert alert-danger mt-2">{err}</div>}
            </div>

            <div className="row">
                {items.map((x) => (
                    <div className="col-6 col-md-4 col-lg-3 mb-3" key={x.id}>
                        <div className="card">
                            {x.contentType?.startsWith("video/") ? (
                                <video className="card-img-top" controls src={x.url} />
                            ) : (
                                <img className="card-img-top" alt={x.originalName} src={x.url} />
                            )}

                            <div className="card-body">
                                <div className="small text-truncate" title={x.originalName}>
                                    {x.originalName}
                                </div>

                                <button
                                    className="btn btn-sm btn-outline-danger mt-2"
                                    onClick={async () => {
                                        await fileService.remove(x.id);
                                        await load();
                                    }}
                                >
                                    Delete
                                </button>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}
