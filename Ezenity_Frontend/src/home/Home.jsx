// /src/home/Home.jsx
import React from "react";
import { accountService } from "@/_services";

/**
 * Home
 * ----
 * Simple landing page once the user is authenticated (or for guests if you allow it).
 */
function Home() {
    const user = accountService.userValue;

    if (!user) {
        return (
            <div className="p-4">
                <div className="container">
                    <h1>Hi Guest!</h1>
                </div>
            </div>
        );
    }

    return (
        <div className="p-4">
            <div className="container">
                <h1>Hi {user.firstName}!</h1>
                <p>This is the Test Homepage!</p>
            </div>
        </div>
    );
}

export { Home };
