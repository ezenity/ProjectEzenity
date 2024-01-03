import React from 'react';

function Overview() {

    return (
    <div className="p-1">
      <div className="container">
        <h4>STORE - COMING SOON!</h4>

        <div className="tab-pane fade show active" id="v-pills-welcome" role="tabpanel" aria-labelledby="v-pills-welcome-tab">
          
          {/* TODO - CREATE STORE PAGE */}

          <div className="col">
            <h4></h4>
            <ul className="list-unstyled">
              <li className="my-2">
                <div className="customTumblerCups_Container">
                  <img className="customTumblerCups_Img" src="https://items-images-production.s3.us-west-2.amazonaws.com/files/907d24c28215bab40adaac468c72e5d6bf00cf41/original.jpeg" alt="Custom Tumbler Cups"/ >
                  <div className="p-2">
                    <p className="customTumblerCups_Title">Custom Tumbler Cups</p>
                    <p className="customTumblerCups_Price">$40.00 - $46.00</p>
                    <a className="customTumblerCups_Button btn btn-primary" target="_blank" href="/customTumblerCups">Buy now</a>
                  </div>
                </div>
              </li>
            </ul>
          </div>

        </div>
      </div>
    </div>
  );
}

export { Overview };