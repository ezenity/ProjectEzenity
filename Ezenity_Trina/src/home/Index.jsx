import React from "react";
import { accountService } from "@/_services";

import { ImgWithFallback } from "../_helpers/fallback-image";
// Standard Images
import Cups_001_Fallback from "../images/cups/001_Cups_Display.jpg";
// WEBP Images
import Cups_001_One from "../images/cups/001_Cups_Display.webp";

function Home() {
  const user = accountService.userValue;

  if (user == null)
    return (
    <div className="p-4 home">
        <div className="container text-center">
          <span className="d-block">
            <h1 className="text-uppercase fw-bold">Change Maker Creations</h1>
          </span>
            
            <p>Thanks for joining our cause for animal liberation</p>
            <p>Providing custom products for all your desired needs!</p>

            <a className="" data-bs-toggle="modal" data-bs-target="#staticBackdrop_Column_3_Row_1">
              <ImgWithFallback src={Cups_001_One} fallback={Cups_001_Fallback} alt="" className="rounded mb-4 shadow img-fluid border border-success" />
            </a>
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
