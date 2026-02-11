// Phase A: static missions (you can move this to backend later)
const missions = [
    {
        id: "ridge-run-001",
        title: "Ridge Run",
        objective: "Complete the route and submit proof.",
        description:
            "A clean, controlled run focused on line discipline and safe pacing. This mission is used as an entry challenge.",
        rewards: {
            rep: 150,
            coins: 25,
            emblems: [{ id: "newcomer", name: "Newcomer Emblem" }],
        },
        proof: {
            allowUploads: true,
            allowLinks: true,
            maxImages: 2,
            maxVideo: 1,
        },
    },
    {
        id: "night-shift-002",
        title: "Night Shift",
        objective: "Ride the night route and submit proof + notes.",
        description:
            "Night-focused mission. Requires visible lighting and safe riding behavior.",
        rewards: {
            rep: 250,
            coins: 50,
            emblems: [{ id: "night-rider", name: "Night Rider Emblem" }],
        },
        proof: {
            allowUploads: true,
            allowLinks: true,
            maxImages: 2,
            maxVideo: 1,
        },
    },
];

export const vaultService = {
    getMissions,
    getMissionById,
};

function getMissions() {
    return Promise.resolve(missions);
}

function getMissionById(id) {
    return Promise.resolve(missions.find((m) => m.id === id) || null);
}
