import React from "react";

function Career_Old() {
  return(
    <div className="p-1">
      <div className="container">

        <div className="row">

          {Skills()}

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

function Skills() {

  // Skills Progress Bar
  //// ARIA-VALUENOW Object
  var softwareTestingValue = '50'
  var shellScriptingValue = '25'
  var hardwareSoftwareInstallValue = '100'
  var troubleshootingDebuggingValue ='50'
  var graphicPresentationsValue = '100'
  var programmingDocumentationValue = '75'
  var problemSolvingAbilitiesValue = '50'
  var devOpsPrinciplesValue = '50'
  var interfaceDesignImplementationValue = '50'
  var agileDevelopmentMethodologiesValue = '50'
  var windowsUsageValue = '75'
  var linuxUsageValue = '85'
  var HTMLProficiencyValue = '80'
  var CSSProficiencyValue = '80'
  var cSharpProficiencyValue = '75'
  var vbNetProficiencyValue = '75'
  var aspNetProficiencyValue = '75'
  var adoNetProficiencyValue = '75'
  var javaProficiencyValue = '75'
  var mavenProficiencyValue = '80'
  var xmlProficiencyValue = '60'
  var sqlProficiencyValue = '50'
  var typeScriptProficiencyValue = '50'
  var bashProficiencyValue = '50'
  var pythonProficiencyValue = '35'
  var gitProficiencyValue = '85'
  var mockitoProficiencyValue = '45'
  var dockerProficiencyValue = '30'
  var slf4jProficiencyValue = '25'
  var angularProficiencyValue = '50'
  var reactProficiencyValue = '60'
  //// STYLE Object
  var softwareTesting = { width: softwareTestingValue + '%' }
  var shellScripting = { width: shellScriptingValue + '%' }
  var hardwareSoftwareInstall = { width: hardwareSoftwareInstallValue + '%' }
  var troubleshootingDebugging = { width: troubleshootingDebuggingValue + '%' }
  var graphicPresentations = { width: graphicPresentationsValue + '%' }
  var programmingDocumentation = { width: programmingDocumentationValue + '%' }
  var problemSolvingAbilities ={ width: problemSolvingAbilitiesValue + '%' }
  var devOpsPrinciples ={ width: devOpsPrinciplesValue + '%' }
  var interfaceDesignImplementation = { width: interfaceDesignImplementationValue + '%' }
  var agileDevelopmentMethodologies = { width: agileDevelopmentMethodologiesValue + '%' }
  var windowsUsage ={ width: windowsUsageValue + '%' }
  var linuxUsage = { width: linuxUsageValue + '%' }
  var HTMLProficiency = { width: HTMLProficiencyValue + '%' }
  var CSSProficiency = { width: CSSProficiencyValue + '%' }
  var cSharpProficiency = { width: cSharpProficiencyValue + '%' }
  var vbNetProficiency = { width: vbNetProficiencyValue + '%' }
  var aspNetProficiency = { width: aspNetProficiencyValue + '%' }
  var adoNetProficiency = { width: adoNetProficiencyValue + '%' }
  var javaProficiency = { width: javaProficiencyValue + '%' }
  var mavenProficiency = { width: mavenProficiencyValue + '%' }
  var xmlProficiency = { width: xmlProficiencyValue + '%' }
  var sqlProficiency = { width: sqlProficiencyValue + '%' }
  var typeScriptProficiency = { width: typeScriptProficiencyValue + '%' }
  var bashProficiency = { width: bashProficiencyValue + '%' }
  var pythonProficiency = { width: pythonProficiencyValue + '%' }
  var gitProficiency = { width: gitProficiencyValue + '%' }
  var mockitoProficiency = { width: mockitoProficiencyValue + '%' }
  var dockerProficiency = { width: dockerProficiencyValue + '%' }
  var slf4jProficiency = { width: slf4jProficiencyValue + '%' }
  var angularProficiency = { width: angularProficiencyValue + '%' }
  var reactProficiency = { width: reactProficiencyValue + '%' }

  return(
    <div className="col-md-4">
      <h2>
        <svg className="material-icons me-2 mb-1" xmlns="http://www.w3.org/2000/svg" width="30" height="30" viewBox="0 0 24 24">
            <path d="M5 13.18v4L12 21l7-3.82v-4L12 17l-7-3.82zM12 3L1 9l11 6 9-4.91V17h2V9L12 3z"/>
        </svg>

        Skill Progress
      </h2>

      <h5>Software Testing</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={softwareTestingValue} aria-valuemin="0" aria-valuemax="100" style={softwareTesting}></div>
      </div>

      <h5>Shell Scripting</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={shellScriptingValue} aria-valuemin="0" aria-valuemax="100" style={shellScripting}></div>
      </div>

      <h5>Hardware and Software Installation</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={hardwareSoftwareInstallValue} aria-valuemin="0" aria-valuemax="100" style={hardwareSoftwareInstall}></div>
      </div>

      <h5>Toubleshooting and Debugging</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={troubleshootingDebuggingValue} aria-valuemin="0" aria-valuemax="100" style={troubleshootingDebugging}></div>
      </div>

      <h5>Graphical Presentations</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={graphicPresentationsValue} aria-valuemin="0" aria-valuemax="100" style={graphicPresentations}></div>
      </div>

      <h5>Programming Documentation</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={programmingDocumentationValue} aria-valuemin="0" aria-valuemax="100" style={programmingDocumentation}></div>
      </div>

      <h5>Problem-Solving Abilities</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={problemSolvingAbilitiesValue} aria-valuemin="0" aria-valuemax="100" style={problemSolvingAbilities}></div>
      </div>

      <h5>DevOps Principles</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={devOpsPrinciplesValue} aria-valuemin="0" aria-valuemax="100" style={devOpsPrinciples}></div>
      </div>

      <h5>Interface Design and Implementation</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={interfaceDesignImplementationValue} aria-valuemin="0" aria-valuemax="100" style={interfaceDesignImplementation}></div>
      </div>

      <h5>Agile Development Methodologies</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={agileDevelopmentMethodologiesValue} aria-valuemin="0" aria-valuemax="100" style={agileDevelopmentMethodologies}></div>
      </div>

      <h5>Windows Knowledge and Usage</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={windowsUsageValue} aria-valuemin="0" aria-valuemax="100" style={windowsUsage}></div>
      </div>

      <h5>Linux Knowledge and Usage</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={linuxUsageValue} aria-valuemin="0" aria-valuemax="100" style={linuxUsage}></div>
      </div>

      <h5>HTML Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={HTMLProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={HTMLProficiency}></div>
      </div>

      <h5>CSS Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={CSSProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={CSSProficiency}></div>
      </div>

      <h5>C# Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={cSharpProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={cSharpProficiency}></div>
      </div>

      <h5>VB.NET Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={vbNetProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={vbNetProficiency}></div>
      </div>

      <h5>ASP.NET Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={aspNetProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={aspNetProficiency}></div>
      </div>

      <h5>ADO.NET Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={adoNetProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={adoNetProficiency}></div>
      </div>

      <h5>Java Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={javaProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={javaProficiency}></div>
      </div>

      <h5>Maven Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={mavenProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={mavenProficiency}></div>
      </div>

      <h5>XML Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={xmlProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={xmlProficiency}></div>
      </div>

      <h5>SQL Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={sqlProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={sqlProficiency}></div>
      </div>

      <h5>TypeScript Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={typeScriptProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={typeScriptProficiency}></div>
      </div>

      <h5>Bash Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={bashProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={bashProficiency}></div>
      </div>

      <h5>Python Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={pythonProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={pythonProficiency}></div>
      </div>

      <h5>Git Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={gitProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={gitProficiency}></div>
      </div>

      <h5>Mockito Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={mockitoProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={mockitoProficiency}></div>
      </div>

      <h5>Docker Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={dockerProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={dockerProficiency}></div>
      </div>

      <h5>SLF4j Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={slf4jProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={slf4jProficiency}></div>
      </div>

      <h5>Angular Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={angularProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={angularProficiency}></div>
      </div>

      <h5>React Proficiency</h5>
      <div className="progress mb-2">
        <div className="progress-bar progress-bar-striped progress-bar-animated bg-success" role="progressbar" aria-valuenow={reactProficiencyValue} aria-valuemin="0" aria-valuemax="100" style={reactProficiency}></div>
      </div>
    </div>
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

export { Career_Old };