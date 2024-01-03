import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

import { skillService, accountService, ecService, phService } from '@/_services';
import { Role } from '@/_helpers';

function Overview({ match }) {

  return(
    <div className="p-1">
      <div className="container career-overview">
        <div className="row">
          <div className="col-md-4">
            {Skills({ match })}
          </div>

          <div className="col-md-8 border-start">

            {ExpertiseCompentencies({ match })}
            
            {ProjectHistory({ match })}
            
            {WorkHistory()}
            
            {AwardsCertifications()}

          </div>

        </div>

      </div>
    </div>
  );
}

function Skills({ match }) {
  const { path } = match;
  const user = accountService.userValue;
  const [skills, setSkills] = useState(null);

  useEffect(() => {
    skillService.getAll().then(x => setSkills(x));
  }, []);

  function deleteSkill(id) {
    setSkills(skills.map(x => {
      if (x.id === id) { x.isDeleting = true; }
      return x;
    }));

    skillService.delete(id).then(() => {
      setSkills(skills => skills.filter(x => x.id !== id));
    });
  }

  return(
    <>
      <h2 className="mb-4">
        <svg className="material-icons me-2 mb-1" xmlns="http://www.w3.org/2000/svg" width="30" height="30" viewBox="0 0 24 24">
            <path d="M5 13.18v4L12 21l7-3.82v-4L12 17l-7-3.82zM12 3L1 9l11 6 9-4.91V17h2V9L12 3z"/>
        </svg>

        Skill Progress
        { user !== null && user.role === Role.Admin ?

            <div className="navbar-light float-end">
              <button className="navbar-toggler" type="button" id="dropdownMenuButton-SkillHeader" data-bs-toggle="offcanvas" data-bs-target="#offcanvasNavbar" aria-controls="offcanvasNavbar">
                <span className="navbar-toggler-icon"></span>
              </button>
              <div className="offcanvas offcanvas-end" tabIndex="-1" id="offcanvasNavbar" aria-labelledby="offcanvasNavbarLabel">
                <div className="offcanvas-header">
                  <h5 className="offcanvas-title" id="offcanvasNavbarlabel">Skill Progress</h5>
                  <button type="button" className="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
                </div>
                <div className="offcanvas-body">
                  <ul className="navbar-nav justify-content-end flex-grow-1 pe-3">
                    <li className="nav-item">
                      <Link to={`${path}/add-skill`} className="nav-link">Add Skill</Link>
                    </li>
                  </ul>
                </div>
              </div>
            </div>
          :
            null
        }
      </h2>

      {skills && skills.map(skill =>
        <div key={skill.id} className="mb-4">
          { user !== null && user.role === Role.Admin ?
              <h5 className="dropdown">
                <span className="dropdown-toggle" href="#" role="button" id="DropDownMenu-skillTitle" data-bs-toggle="dropdown" aria-expanded="false">
                  {skill.title}
                </span>
                <ul className="dropdown-menu" aria-labelledby="dropdownMenuButton-skillTitle">
                  <li>
                    <Link to={`${path}/edit-skill/${skill.id}`} className="dropdown-item">Edit</Link>
                  </li>
                  <li>
                    <Link to={`${path}`} onClick={() => deleteSkill(skill.id)} disabled={skill.isDeleting} className="dropdown-item">
                      {skill.isDeleting ? <span className="spinner-border spinner-border-sm"></span> : <span>Delete</span>}
                    </Link>
                  </li>
                </ul>
              </h5>
            :
              <h5>{skill.title}</h5>
          }
          <div className="progress mb-2">
            <div
                className="progress-bar progress-bar-striped progress-bar-animated bg-success"
                role="progressbar" aria-valuenow={skill.percentage}
                aria-valuemin="0"
                aria-valuemax="100"
                style={{ width: (skill.percentage) + '%' }} >
            </div>
          </div>
        </div>
      )}
    </>
  );
}

function Section({ match }) {
  const { path } = match;
  const user = accountService.userValue;
  const [sec, setSec] = useState(null);

  useEffect(() => {
    
  })
}


function ExpertiseCompentencies({ match }) {
  const { path } = match;
  const user = accountService.userValue;
  const [ecs, setECs] = useState(null);

  useEffect(() => {
    ecService.getAll().then(x => setECs(x));
  }, []);

  function deleteEC(id) {
    setECs(ecs.map(x => {
      if (x.id === id) { x.isDeleting = true; }
      return x;
    }));

    ecService.delete(id).then(() => {
      setECs(ecs => ecs.filter(x => x.id !== id));
    });
  }

  return(
    <div className="border-2">
      <h2 className="mb-4">
        Expertise and Compentencies
        
        { user !== null && user.role === Role.Admin ?

            <div className="navbar-light float-end">
              <button className="navbar-toggler" type="button" id="dropdownMenuButton-SkillHeader" data-bs-toggle="offcanvas" data-bs-target="#offcanvasNavbar-expertise-compentencies" aria-controls="offcanvasNavbar-expertise-compentencies">
                <span className="navbar-toggler-icon"></span>
              </button>
              <div className="offcanvas offcanvas-end" tabIndex="-1" id="offcanvasNavbar-expertise-compentencies" aria-labelledby="offcanvasNavbarLabel-expertise-compentencies">
                <div className="offcanvas-header">
                  <h5 className="offcanvas-title" id="offcanvasNavbarlabel-expertise-compentencies">Expertise and Compentencies</h5>
                  <button type="button" className="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
                </div>
                <div className="offcanvas-body">
                  <ul className="navbar-nav justify-content-end flex-grow-1 pe-3">
                    <li className="nav-item">
                      <Link to={`${path}/add-ec`} className="nav-link">Add New</Link>
                    </li>
                  </ul>
                </div>
              </div>
            </div>
          :
            null
        }
      </h2>

      {ecs && ecs.map(ec =>
        <dl key={ec.id} className="row m-0">
          { user !== null && user.role === Role.Admin ?
            <dt className="col-sm-3 dropdown">
              <span className="dropdown-toggle" href="#" role="button" id="DropDownMenu-ecTitle" data-bs-toggle="dropdown" aria-expanded="false">
                {ec.title}
              </span>
              <ul className="dropdown-menu" aria-labelledby="dropdownMenuButton-ecTitle">
                <li>
                  <Link to={`${path}/edit-ec/${ec.id}`} className="dropdown-item">Edit</Link>
                </li>
                <li>
                  <Link to={`${path}`} onClick={() => deleteEC(ec.id)} disabled={ec.isDeleting} className="dropdown-item">
                    {ec.isDeleting ? <span className="spinner-border spinner-border-sm"></span> : <span>Delete</span>}
                  </Link>
                </li>
              </ul>
            </dt>
          :
            <dt className="col-sm-3">{ec.title}</dt>
          }
          <dd className="col-sm-9">{ec.content}</dd>
        </dl>
      )}
    </div>
  );
}

function ProjectHistory({ match }) {
  const { path } = match;
  const user = accountService.userValue;
  const [projects, setProjects] = useState(null);

  useEffect(() => {
    phService.getAll().then(x => setProjects(x));
  }, []);

  function deleteProject(id) {
    setprojects(projects.map(x => {
      if (x.id === id) { x.isDeleting = true; }
      return x;
    }));

    phService.delete(id).then(() => {
      setProjects(projects => projects.filter(x => x.id !== id));
    });
  }
  return(
    <div>
      <h2 className="mb-2">
        Project History
        { user !== null && user.role === Role.Admin ?

            <div className="navbar-light float-end">
              <button className="navbar-toggler" type="button" id="dropdownMenuButton-ProjectHistoryHeader" data-bs-toggle="offcanvas" data-bs-target="#offcanvasNavbar-ProjectHistory" aria-controls="offcanvasNavbar">
                <span className="navbar-toggler-icon"></span>
              </button>
              <div className="offcanvas offcanvas-end" tabIndex="-1" id="offcanvasNavbar-ProjectHistory" aria-labelledby="offcanvasNavbarLabel">
                <div className="offcanvas-header">
                  <h5 className="offcanvas-title" id="offcanvasNavbarlabel">Project History</h5>
                  <button type="button" className="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
                </div>
                <div className="offcanvas-body">
                  <ul className="navbar-nav justify-content-end flex-grow-1 pe-3">
                    <li className="nav-item">
                      <Link to={`${path}/add-project-history`} className="nav-link">Add New Project</Link>
                    </li>
                  </ul>
                </div>
              </div>
            </div>
          :
            null
        }
      </h2>

      <div className="row">

        <dt className="col-sm-3 border-top">2021-02 / 2021-12</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              Fortify (.NET, C#/VB.NET, ASP.NET/MVC, SQL)
            </li>
            <li>Confidential - Sanford, FL</li>
            <li>
              <ul>
                <li>Performed security assessments for the client-facing</li>
                <li>
                  Reviewed security vulnerability reports for applications and databases, analyzed and worked extensively with development teams for the
                  implementation of mitigating controls
                </li>
                <li>Providing fixes and filtering false findings for the vulnerabilities reported in the scan reports</li>
                <li>
                  Documented security findings, recommendations and presented to the business users, executive
                  committee, and compliance departments
                </li>
              </ul>
            </li>
          </ul>
        </dd>


      </div>
      
    </div>
  );
}

function WorkHistory() {
  return (
    <div>
      <h2>Work History</h2>

      <div className="row">

        <dt className="col-sm-3 border-top">2010-12 / 2020-02</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              Software Developer
            </li>
            <li>Self Employed - Philadelphia / Croydon, PA</li>
            <li>
              <ul>
                <li>Tested websites and performed troublesgooting prior to deployment</li>
                <li>Built, evaluated and deployed scalable, highly available and modular software products</li>
                <li>Designed, implemented and monitored web pages for continuous improvements</li>
                <li>Administered, supported and monitored databases by proactively resolving database issues while maintaining servers</li>
                <li>Debugged and modified 5+ Java software components</li>
              </ul>
            </li>
          </ul>
        </dd>

        <dt className="col-sm-3 border-top">2018-10 / 2019-07</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              Customer Service Representative
            </li>
            <li>Sykes Enterprises Inc. - Philadelphia, PA</li>
            <li>
              <ul>
                <li>Provided primary customer support to internal and external customers in challenging environment</li>
                <li>Answered constant flow of customer calls with up to 5 calss in queue per minute</li>
                <li>Recommended Brand Products or Serices to customers, thoroughly explaining details</li>
                <li>Educated customers on promotions to enhance sales</li>
                <li>Achieved and consistently exceeded revenue quota through product and service promotion during routine calls</li>
                <li>Assisted customers with setting appointments, shipping and special-order requests</li>
                <li>Assisted customers with arranding merchandise pick-ups at other locations</li>
              </ul>
            </li>
          </ul>
        </dd>



      </div>

    </div>
  );
}

function AwardsCertifications() {
  return(
    <div>
      <h2>Awards and Certifications</h2>

      <div className="row">

        <dt className="col-sm-3 border-top">2022-02</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              Connect your Services with Microsoft Azure Service Bus
            </li>
            <li>By Microsoft on Coursera</li>
            <li>
              <ul>
                <li>Credential ID: 7U8FXRNPLPYM</li>
              </ul>
            </li>
          </ul>
        </dd>

        <dt className="col-sm-3 border-top">2022-02</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              Create Serverless Applications
            </li>
            <li>By Microsoft on Coursera</li>
            <li>
              <ul>
                <li>Credential ID: 7H7X2B2VYXMJ</li>
              </ul>
            </li>
          </ul>
        </dd>

        <dt className="col-sm-3 border-top">2020-03</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              Open-Source Tools for Data Science
            </li>
            <li>By IBM on Coursera</li>
            <li>
              <ul>
                <li>Credential ID: XJM68JSM6KZZ</li>
              </ul>
            </li>
          </ul>
        </dd>

        <dt className="col-sm-3 border-top">2020-03</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              Java Fundamentals
            </li>
            <li>By Mosh Hamedani on Teachable</li>
            <li>
              <ul>
                <li>Certificate ID: 51r8tw6s</li>
              </ul>
            </li>
          </ul>
        </dd>

        <dt className="col-sm-3 border-top">2020-03</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              Java Object-oriented Programming
            </li>
            <li>By Mosh Hamedani on Teachable</li>
            <li>
              <ul>
                <li>Certificate ID: t2ckw188</li>
              </ul>
            </li>
          </ul>
        </dd>

        <dt className="col-sm-3 border-top">2020-02</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              What is Data Science?
            </li>
            <li>By IBM on Coursera</li>
            <li>
              <ul>
                <li>Credential ID: NCNMQ2JPR2N6</li>
              </ul>
            </li>
          </ul>
        </dd>



      </div>

    </div>
  );
}

export { Overview };