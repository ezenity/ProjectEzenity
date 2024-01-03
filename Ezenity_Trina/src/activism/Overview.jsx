import React from 'react';

function Overview() {

    return (
    <div className="p-1">
      <div className="container">
        <h2>Fight Against Animal Cruelty</h2>

        <div className="text-decoration-none text-align-center">
          Each day millions of animals are being slaughtered! We must fight together to stop this atrocity and give each sentient being the life they deserve.
          <div className="row text-align-center mt-4">
            <div className="col">
              <h4>Resources and Documentaries</h4>
              <ul className="list-unstyled">
                <li className="my-2">
                  <a className="btn btn-primary w-50" rel="noreferrer" target="_blank" href="/dominionMovement" >
                    Dominion Movement
                  </a>
                </li>
                <li className="my-2">
                  <a className="btn btn-primary w-50" rel="noreferrer" target="_blank" href="/whatTheHealthFilm" >
                    What The Health Film
                  </a>
                </li>
                <li className="my-2">
                  <a className="btn btn-primary w-50" rel="noreferrer" target="_blank" href="/fastAgainstSlaughter" >
                    Fast Against Slaughter
                  </a>
                </li>
                <li className="my-2">
                  <a className="btn btn-primary w-50" rel="noreferrer" target="_blank" href="/millionDollarVegan" >
                    Million Dollar Vegan
                  </a>
                </li>
                <li className="my-2">
                  <a className="btn btn-primary w-50" rel="noreferrer" target="_blank" href="/veganAction" >
                    Vegan Action
                  </a>
                </li>
                <li className="my-2">
                  <a className="btn btn-primary w-50" rel="noreferrer" target="_blank" href="/animalCrueltyExposureFund" >
                    Animal Cruelty Exposure Fund
                  </a>
                </li>
              </ul>
            </div>
            <div className="col">
              <h4>Find Local Action</h4>
              
              <ul className="list-unstyled">
                <li className="my-2">
                  <a className="btn btn-primary w-50" rel="noreferrer" target="_blank" href="/centralFloridaAnimalRights" >
                    Central Florida Animal Rights
                  </a>
                </li>
                <li className="my-2">
                  <a className="btn btn-primary w-50" rel="noreferrer" target="_blank" href="/directActionEverywhere" >
                    Direct Action Everywhere
                  </a>
                </li>
                <li className="my-2">
                  <a className="btn btn-primary w-50" rel="noreferrer" target="_blank" href="/animalRightsFlorida" >
                    Animal Rights Florida
                  </a>
                </li>
                <li className="my-2">
                  <a className="btn btn-primary w-50" rel="noreferrer" target="_blank" href="/animalActivismMentorship" >
                    Animal Activism Mentorship
                  </a>
                </li>
              </ul>
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}

export { Overview };