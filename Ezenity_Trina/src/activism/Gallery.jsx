import React from "react";
import { ImgWithFallback } from "../_helpers/fallback-image";
// Standard Images
import exampleFallback from "../images/example/exampleFallback.jpg";
import atlantaProtestFallback from "../images/activism/AlantaProtestFallback.png";
import circusProtestFallback from "../images/activism/CircusProtestFallback.png";
import seaworldProtestFallback from "../images/activism/SeaworldProtestFallback.jpg";
// WEBP Images
import exampleOne from "../images/example/exampleOne.webp";
import atlantaProtest from "../images/activism/AlantaProtest.webp";
import circusProtest from "../images/activism/CircusProtest.webp";
import seaworldProtest from "../images/activism/SeaworldProtest.webp";

function Gallery() {
  return(
    <div className="p-1">
      <div className="container">
        <h4>Activism</h4>

        <div className="row">
          {columnOne()}
          {columnTwo()}
          {columnThree()}
        </div>

      </div>
    </div>
  );
}


function columnOne() {
  return (
    <div className="col-lg-4 col-md-12 mb-4 mb-lg-0">

      {/* Example Images */}
      <a className="" data-bs-toggle="modal" data-bs-target="#staticBackdrop_Column_1_Row_1">
        <ImgWithFallback src={atlantaProtest} fallback={atlantaProtestFallback} alt="" className="rounded mb-4 shadow img-fluid border border-success activism-max-height" />
      </a>

      {/* Popup window */}
      <div className="modal fade" id="staticBackdrop_Column_1_Row_1" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="staticBackdropLabel">AAM Assembly (Alanta, GA)</h5>
              <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body text-center">
              
              {/* SlideShow (Carousel) START */}
              <div id="carousel_Column_1_Row_1_Indicators" className="carousel slide carousel-fade" data-bs-ride="carousel">
                <div className="carousel-indicators">
                  <button type="button" data-bs-target="#carousel_Column_1_Row_1_Indicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
                  {/* <button type="button" data-bs-target="#carousel_Column_1_Row_1_Indicators" data-bs-slide-to="1" aria-label="Slide 2"></button>
                  <button type="button" data-bs-target="#carousel_Column_1_Row_1_Indicators" data-bs-slide-to="2" aria-label="Slide 3"></button>
                  <button type="button" data-bs-target="#carousel_Column_1_Row_1_Indicators" data-bs-slide-to="3" aria-label="Slide 4"></button> */}
                </div>
                <div className="carousel-inner">
                  <div className="carousel-item active" data-bs-interval="4000">
                    <ImgWithFallback src={atlantaProtest} fallback={atlantaProtestFallback} className="img-fluid img-thumbnail" />
                  </div>
                  {/* <div className="carousel-item" data-bs-interval="4000">
                    <ImgWithFallback src={exampleOne} fallback={exampleFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    <ImgWithFallback src={exampleOne} fallback={exampleFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item">
                    <ImgWithFallback src={exampleOne} fallback={exampleFallback} className="img-fluid img-thumbnail" />
                  </div> */}
                </div>
                <button className="carousel-control-prev" type="button" data-bs-target="#carousel_Column_1_Row_1_Indicators" data-bs-slide="prev">
                  <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Previous</span>
                </button>
                <button className="carousel-control-next" type="button" data-bs-target="#carousel_Column_1_Row_1_Indicators" data-bs-slide="next">
                  <span className="carousel-control-next-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Next</span>
                </button>
              </div>
              {/* SlideShow (Carousel) END */}

            </div>
            <div className="modal-footer">
              My daughter & I attended the 7 day AAM Assembly. We could only stay 2 days, but in that short amount of time it was the best experience being surrounded by many other BADASS, compassionate vegans making changes for the 🌍. Great presentations & actions for the animals (including protesting Zoo Atlanta, Anthroplogie, Seaquest & horse drawn carriages). For those interested in help going vegan or how to make the switch, this is an awesome group & I'm happy to share resources, as well. It all boils down to #humanfreedom #animalrights ✊📢✌
            </div>
          </div>
        </div>
      </div>

    </div>
  );
}

function columnTwo() {
  return (
    <div className="col-lg-4 col-md-12 mb-4 mb-lg-0">
      
      {/* Example Image */}
      <a className="" data-bs-toggle="modal" data-bs-target="#staticBackdrop_Column_2_Row_1">
        <ImgWithFallback src={circusProtest} fallback={circusProtestFallback} alt="" className="rounded mb-4 shadow img-fluid border border-success activism-max-height" />
      </a>

      {/* Popup Window */}
      <div className="modal fade" id="staticBackdrop_Column_2_Row_1" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="staticBackdropLabel">Circus Protest (Orlando, FL)</h5>
              <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body text-center">
              
              {/* SlideShow (Carousel) START */}
              <div id="carousel_Column_2_Row_1_Indicators" className="carousel slide carousel-fade" data-bs-ride="carousel">
                <div className="carousel-indicators">
                  <button type="button" data-bs-target="#carousel_Column_2_Row_1_Indicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
                  {/* <button type="button" data-bs-target="#carousel_Column_2_Row_1_Indicators" data-bs-slide-to="1" aria-label="Slide 2"></button> */}
                </div>
                <div className="carousel-inner">
                  <div className="carousel-item active" data-bs-interval="4000">
                  <ImgWithFallback src={circusProtest} fallback={circusProtestFallback} className="img-fluid img-thumbnail" />
                  </div>
                  {/* <div className="carousel-item">
                  <ImgWithFallback src={exampleOne} fallback={exampleFallback} className="img-fluid img-thumbnail" />
                  </div> */}
                </div>
                <button className="carousel-control-prev" type="button" data-bs-target="#carousel_Column_2_Row_1_Indicators" data-bs-slide="prev">
                  <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Previous</span>
                </button>
                <button className="carousel-control-next" type="button" data-bs-target="#carousel_Column_2_Row_1_Indicators" data-bs-slide="next">
                  <span className="carousel-control-next-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Next</span>
                </button>
              </div>
              {/* SlideShow (Carousel) END */}

            </div>
            <div className="card-footer">
              <ul className="list-unstyled">
                <li>
                  Abuse & stressful living conditions (small cages, etc) are common in the circus. Cheers of the crowd & the dizzying lights all disorientate/cause stress to wild animals. This also teaches our children animals are here for entertainment & we can treat them any way we please...regardless of their emotions. A circus can still be awesome WITHOUT animals.
                  <br />
                  <br />
                  • <br />
                  •• <br />
                  ••• <br />
                  #animallovers #instadaily #animalrightsofig #activist #peta
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>


    </div>
  );
}

function columnThree() {
  return (
    <div className="col-lg-4 col-md-12 mb-4 mb-lg-0">
      
      {/* Examples Images */}
      <a className="" data-bs-toggle="modal" data-bs-target="#staticBackdrop_Column_3_Row_1">
        <ImgWithFallback src={seaworldProtest} fallback={seaworldProtestFallback} alt="" className="rounded mb-4 shadow img-fluid border border-success activism-max-height" />
      </a>

      {/* Popup window */}
      <div className="modal fade" id="staticBackdrop_Column_3_Row_1" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="staticBackdropLabel">Seaworld Protest (Orlando, FL)</h5>
              <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body text-center">
              
              {/* SlideShow (Carousel) START */}
              <div id="carousel_Column_3_Row_1_Indicators" className="carousel slide carousel-fade" data-bs-ride="carousel">
                <div className="carousel-indicators">
                  <button type="button" data-bs-target="#carousel_Column_3_Row_1_Indicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
                  {/* <button type="button" data-bs-target="#carousel_Column_3_Row_1_Indicators" data-bs-slide-to="1" aria-label="Slide 2"></button>
                  <button type="button" data-bs-target="#carousel_Column_3_Row_1_Indicators" data-bs-slide-to="2" aria-label="Slide 3"></button>
                  <button type="button" data-bs-target="#carousel_Column_3_Row_1_Indicators" data-bs-slide-to="3" aria-label="Slide 4"></button> */}
                </div>
                <div className="carousel-inner">
                  <div className="carousel-item active" data-bs-interval="4000">
                    <ImgWithFallback src={seaworldProtest} fallback={seaworldProtestFallback} className="img-fluid img-thumbnail" />
                  </div>
                  {/* <div className="carousel-item" data-bs-interval="4000">
                    <ImgWithFallback src={exampleOne} fallback={exampleFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    <ImgWithFallback src={exampleOne} fallback={exampleFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    <ImgWithFallback src={exampleOne} fallback={exampleFallback} className="img-fluid img-thumbnail" />
                  </div> */}
                </div>
                <button className="carousel-control-prev" type="button" data-bs-target="#carousel_Column_3_Row_1_Indicators" data-bs-slide="prev">
                  <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Previous</span>
                </button>
                <button className="carousel-control-next" type="button" data-bs-target="#carousel_Column_3_Row_1_Indicators" data-bs-slide="next">
                  <span className="carousel-control-next-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Next</span>
                </button>
              </div>
              {/* SlideShow (Carousel) END */}

            </div>
            <div className="modal-footer">
              Today's #boycottseaworldday 🐳🐋 People across the world are taking action for the orcas, dolphins, and other animals trapped at SeaWorld. There's nothing fun or entertaining about captivity. There's plenty of other ways to see wildlife in their natural habitats rather than stuck in these small enclosures.
                <br />
                <br />
              If you agree with this & want to get involved, @animalrightsfl holds monthly protests at SeaWorld you can join. #captivitykills ✌
                <br />
                <br />
              #vegancommunity #seaworld #orlandoflorida #wildlife #entertainment #familyfun #oceanlife #dolphins #animallovers #govegan #vegankids #peta
            </div>
          </div>
        </div>
      </div>



    </div>
  );
}


export { Gallery };