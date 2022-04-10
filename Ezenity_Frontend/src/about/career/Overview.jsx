import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

import { skillService, accountService } from '@/_services';
import { Role } from '@/_helpers';

function Overview({ match }) {

  return(
    <div className="p-1">
      <div className="container">
        <div className="row">
          <div className="col-md-4">
            {Skills({ match })}
          </div>

          <div className="col-md-8">

            {ExpertiseCompentencies()}
            
            {Projects()}
            
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
    
    <h2 className="my-4">
      <svg className="material-icons me-2 mb-1" xmlns="http://www.w3.org/2000/svg" width="30" height="30" viewBox="0 0 24 24">
          <path d="M5 13.18v4L12 21l7-3.82v-4L12 17l-7-3.82zM12 3L1 9l11 6 9-4.91V17h2V9L12 3z"/>
      </svg>

      Skill Progress
      {
        user !== null && user.role === Role.Admin ?

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
        <h5>
          {skill.title}
          {
            user !== null && user.role === Role.Admin ?
              <>
                <button onClick={() => deleteSkill(skill.id)} className="btn btn-sm btn-danger float-end" style={{ width: '60px' }} disabled={skill.isDeleting}>
                    {skill.isDeleting 
                        ? <span className="spinner-border spinner-border-sm"></span>
                        : <span>Delete</span>
                    }
                </button>
                <Link to={`${path}/edit-skill/${skill.id}`} className="btn btn-sm btn-primary me-1 float-end">Edit</Link>
              </>
            :
              null
          }
        </h5>
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

function ExpertiseCompentencies() {
  return(
    <div className="border-2">
      <h2 className="mb-2">Expertise and Compentencies</h2>

      <dl className="row">

        <dt className="col-sm-3">Technical Skills</dt>
        <dd className="col-sm-9">
          C#, VB.NET, ASP.NET, ADO.NET, ASP, xPath, XQuery, XSLT, REST/SOAP, jQuery, Java, Maven,
          Git, Linux (Debian/ARM), XML, HTML, CSS, SQL/MySQL, Groovy, TypeScript, Bash, Python, 
          Angular, React, Mockito, SLF4j, Docker
        </dd>

        <dt className="col-sm-3">Soft Skills</dt>
        <dd className="col-sm-9">
          Communication Skills, Problem Solving, Teamwork/Collaboration, Research, Creativity
        </dd>

        <dt className="col-sm-3">Tools</dt>
        <dd className="col-sm-9">
          Azure DevOps, HP Fortify (SCA), Visual Studios, SoapUI, IIS Manager, MS SQL Server, IntelliJ,
          STS, Microsoft Office, GitHub, BitBucket, Jenkins, PhpMyAdmin, Netbeans, Junit, RStudio IDE,
          Zeppelin Notbooks, Autosys, SQL Developer, Toad (Oracle), IBM Algo Collateral, Powershell
        </dd>

      </dl>

    </div>
  );
}

function Projects() {
  return(
    <div>
      <h2 className="mb-2">Project History</h2>

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

        <dt className="col-sm-3 border-top">2022-02 / 2021-01</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              Reach (Java, Junit, Maven, Jenkins, Groovy, Git)
            </li>
            <li>Project Ezenity - Croydon, PA</li>
            <li>
              <ul>
                <li>Rebuild of UrExtras Project with modern java implementation</li>
                <li>Improved speed performace and upload intial loading by 25%</li>
              </ul>
            </li>
          </ul>
        </dd>

        <dt className="col-sm-3 border-top">2020-02 / 2022-01</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              Ezenity (Angular, TypeScript, HTML/CSS, Jenkins, Groovy, Linux (ARM), Bash, Docker)
            </li>
            <li>Project Ezenity - Croydon, PA</li>
            <li>
              <ul>
                <li>Developed and Administered Angular based website</li>
                <li>Utilized ES6/ES2015 and gained experience with HTML5, CSS3, TypeScript and Twitter Bootstrap</li>
                <li>Developed, maintain and supported continuous integration framework based on Jenkins</li>
              </ul>
            </li>
          </ul>
        </dd>

        <dt className="col-sm-3 border-top">2019-01 / 2020-01</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              UrExtras (Java, Maven, Git)
            </li>
            <li>Project Ezenity - Croydon, PA</li>
            <li>
              <ul>
                <li>Manipulating the core mechanics of objects within a gaming software</li>
              </ul>
            </li>
          </ul>
        </dd>

        <dt className="col-sm-3 border-top">2018-08 / 2020-01</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              Pl3xCraft (Java, Maven, Jenkins Git)
            </li>
            <li>Pl3x Gaming - Croydon, PA</li>
            <li>
              <ul>
                <li>Handled vector calculations to determine visual particle locations being spawneed to the end user</li>
                <li>Applied modern Java implementations to the core</li>
              </ul>
            </li>
          </ul>
        </dd>

        <dt className="col-sm-3 border-top">2018-07 / 2019-01</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              Mystical Items (Java, Gradle, JSON, Git)
            </li>
            <li>Forge - Croydon, PA</li>
            <li>
              <ul>
                <li>Created 32x32 pixeled objects with gimp to be applied as custom objects</li>
                <li>Applied custom abilities to current and new objects</li>
                <li>Exposed to JSON, Gradle and image manipulation</li>
              </ul>
            </li>
          </ul>
        </dd>

        <dt className="col-sm-3 border-top">2015-10 / 2020-01</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              DwDCmd (Java, MySQL, Linux, Git)
            </li>
            <li>DwD Gaming - Croydon, PA</li>
            <li>
              <ul>
                <li>Exposed to packet handling, metadata and various ways of sotring data into a file</li>
                <li>Added additional moderation commands to apply disciplinary actions to the end user</li>
                <li>Configured and stored end user data to a MySQL database using JDBC queries</li>
                <li>Configured LDAP server for varies applications</li>
              </ul>
            </li>
          </ul>
        </dd>

        <dt className="col-sm-3 border-top">2010-12 / 2018-01</dt>
        <dd className="col-sm-9 border-top">
          <ul className="list-unstyled">
            <li className="lead">
              DwD/Pl3x Gaming (Linux, Communication, Data Analysis, Project Management)
            </li>
            <li>Gaming Community - Croydon, PA</li>
            <li>
              <ul>
                <li>Monitored projects to ensure back-end developers met their deadlines</li>
                <li>Motivated end users to participate into organized events</li>
                <li>Analyzed submitted projects to minimize the risk of poor community engagement</li>
                <li>Actively listened to constructive feedback to resolve conflicts (Community and Individually)</li>
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