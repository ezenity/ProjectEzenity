import React from 'react';

function Overview() {

    return (
    <div className="p-1">
      <div className="container">
        <h4>Frequently Asked Questions</h4>

        <div className="tab-pane fade show active" id="v-pills-welcome" role="tabpanel" aria-labelledby="v-pills-welcome-tab">
          <ol>
            <li>
              What type of products do you provide?
              <ul>
                <li>Custom Tumbler Cups (Available In Store)</li>
                <li>Custom Coffee Mugs (Coming Soon)</li>
                <li>Custom Starbucks Cups (Coming Soon)</li>
                <li>Custom Shirts (Coming Soon)</li>
              </ul>
            </li>
            <li>
              Where can I buy your products?
                <ul>
                  <li>
                    <a className="text-decoration-none" rel="noreferrer" target="_blank" href="/store" >
                      Online Shop (Store)
                    </a>
                  </li>
                  <li>Popup Locations (Various locations in Florida)</li>
                </ul>
            </li>
            <li>
              Can I request a custom Cup design?
                <ul>
                  <li>
                    Yes (Form Coming Soon)
                  </li>
                </ul>
            </li>
          </ol>
        </div>
      </div>
    </div>
  );
}

export { Overview };