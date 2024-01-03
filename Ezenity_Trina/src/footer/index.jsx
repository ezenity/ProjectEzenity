import React from "react";

function Footer() {

  return (

    <footer className="footer">
      <div className="border-top border-success pt-4">
        <div className="container">
          <div className="row">
            <div className="clearfix">

              <div className="col float-start">
                <div className="text-muted">
                  &copy; Change Maker Creations 2022, All Rights Reserved.
                </div>
              </div>

              <div className="col-6 float-end">
                <ul className="nav justify-content-end list-unstyled d-flex">
                  {/* Instagram */}
                  <li className="ms-3">
                    <a className="text-decoration-none" rel="noreferrer" target="_blank" href="/instagram" >
                      <svg className="material-icons" xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 300 300" >
                        <g id="XMLID_504_">
                          <path id="XMLID_505_" d="M38.52,0.012h222.978C282.682,0.012,300,17.336,300,38.52v222.978c0,21.178-17.318,38.49-38.502,38.49 H38.52c-21.184,0-38.52-17.313-38.52-38.49V38.52C0,17.336,17.336,0.012,38.52,0.012z M218.546,33.329 c-7.438,0-13.505,6.091-13.505,13.525v32.314c0,7.437,6.067,13.514,13.505,13.514h33.903c7.426,0,13.506-6.077,13.506-13.514 V46.854c0-7.434-6.08-13.525-13.506-13.525H218.546z M266.084,126.868h-26.396c2.503,8.175,3.86,16.796,3.86,25.759 c0,49.882-41.766,90.34-93.266,90.34c-51.487,0-93.254-40.458-93.254-90.34c0-8.963,1.37-17.584,3.861-25.759H33.35v126.732 c0,6.563,5.359,11.902,11.916,11.902h208.907c6.563,0,11.911-5.339,11.911-11.902V126.868z M150.283,90.978 c-33.26,0-60.24,26.128-60.24,58.388c0,32.227,26.98,58.375,60.24,58.375c33.278,0,60.259-26.148,60.259-58.375 C210.542,117.105,183.561,90.978,150.283,90.978z"/>
                        </g>
                      </svg>
                    </a>
                  </li>
                  {/* Facebook */}
                  {/* <li className="ms-3">
                    <a className="text-decoration-none" rel="noreferrer" target="_blank" href="/facebook" >
                      <svg className="material-icons" xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 34.499 34.499">
                        <g>
                          <path d="M28.018,0H6.483C2.902,0,0,2.902,0,6.483v21.533c0,3.582,2.902,6.483,6.483,6.483h21.533c3.582,0,6.483-2.901,6.483-6.483 V6.483C34.5,2.902,31.598,0,28.018,0z M28.992,8.088c-0.51-0.158-1.139-0.275-1.924-0.275c-1.963,0-2.785,1.53-2.785,3.414v0.98 h3.846v4.395h-3.807v14.794h-6.004V16.603h-2.511v-4.395h2.511v-0.746c0-2.314,0.707-4.867,2.434-6.435 c1.49-1.413,3.57-1.923,5.297-1.923c1.334,0,2.354,0.158,3.18,0.393L28.992,8.088z"/>
                        </g>
                      </svg>
                    </a>
                  </li> */}
                </ul>
              </div>
            </div>
          </div>
        </div>
      </div>
    </footer>
  );
}

export {Footer};