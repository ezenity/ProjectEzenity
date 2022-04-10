import React from "react";
import { accountService } from "@/_services";

function Home() {
  const user = accountService.userValue;

  if (user == null)
    return (
    <div className="p-4">
        <div className="container">
            <h1>Hi Guest!</h1>

        </div>
    </div>
    );

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
