import React from 'react';

const ProgressBar = ({ percentage }) => {
  return (
    <div className="progress mb-2">
      <div
        className="progress-bar progress-bar-striped progress-bar-animated bg-success"
        role="progressbar"
        aria-valuenow={percentage}
        aria-valuemin="0"
        aria-valuemax="100"
        style={{ width: percentage + '%' }}
      ></div>
    </div>
  );
};

const TwoColumnDisplay = ({ title, content }) => {
  return (
    <dl className="row m-0">
      <dt className="col-sm-3">{title}</dt>
      <dd className="col-sm-9">{content}</dd>
    </dl>
  );
};

const Section = ({ title, layout = 'default', children }) => {
  return (
    <div className="section">
      {layout === 'sidebar' ? (
        <div className="sidebar">{children}</div>
      ) : (
        <div className="section-content">{children}</div>
      )}
    </div>
  );
};

export default Section;