import React from "react";
import { ImgWithFallback } from "../_helpers/fallback-image";
// Standard Images
import birthdayDinnerOneFallback from "../images/family/30th_birthday_dinner.png";
import birthdayDinnerTwoFallback from "../images/family/birthday_dinner_a.png";
import birthdayDinnerThreeFallback from "../images/family/Birthday_dinner.png";
import birthdayDinnerFourFallback from "../images/family/wife_and-I.png";
import christmas2021LakeMaryFallback from "../images/family/christmas_2021_lake_mary.jpg";
import christmas2021KissimmeeFamilyPhotoFallback from "../images/family/christmas_2021.png";
import christmas2021kissimmeeWifeAndIFallback from "../images/family/christmas_2021_001.jpg";
import christmas2021GayLordHotelFallback from "../images/family/christmas_2021_002.jpg";
import christmas2020Fallback from "../images/family/christmas_2020.jpg";
import christmas2019Fallback from "../images/family/santa_picture_2019.png";
import newYearsEve2019Fallback from "../images/family/new_years_eve_2019.png";
import nightOutDinnerDateFallback from "../images/family/Dinner_date.png";
import wifesBDayManateesFallback from "../images/family/wifes_bday_manatees.jpg";
import disneyTripFallback from "../images/family/disney_trip.jpg";
import disneyTripOneFallback from "../images/family/disney_trip_001.jpg";
import disneyTripTwoFallback from "../images/family/disney_trip_002.jpg";
import disneyTripThreeFallback from "../images/family/disney_trip_003.jpg";
import familyNatureWalkFallback from "../images/family/nature_walk.jpg";
import waterParkOneFallback from "../images/family/water_park_001.jpg";
import waterParkTwoFallback from "../images/family/water_park_002.jpg";
import waterParkThreeFallback from "../images/family/water_park_003.jpg";
import stiFrontFallback from "../images/sti/FrontDriverSide.jpg";
import stiSideFallback from "../images/sti/SideDriverSide.jpg";
// WEBP Images
import birthdayDinnerOne from "../images/family/30th_birthday_dinner.webp";
import birthdayDinnerTwo from "../images/family/birthday_dinner_a.webp";
import birthdayDinnerThree from "../images/family/Birthday_dinner.webp";
import birthdayDinnerFour from "../images/family/wife_and-I.webp";
import christmas2021LakeMary from "../images/family/christmas_2021_lake_mary.webp";
import christmas2021KissimmeeFamilyPhoto from "../images/family/christmas_2021.webp";
import christmas2021kissimmeeWifeAndI from "../images/family/christmas_2021_001.webp";
import christmas2021GayLordHotel from "../images/family/christmas_2021_002.webp";
import christmas2020 from "../images/family/christmas_2020.webp";
import christmas2019 from "../images/family/santa_picture_2019.webp";
import newYearsEve2019 from "../images/family/new_years_eve_2019.webp";
import nightOutDinnerDate from "../images/family/Dinner_date.webp";
import wifesBDayManatees from "../images/family/wifes_bday_manatees.webp";
import disneyTrip from "../images/family/disney_trip.webp";
import disneyTripOne from "../images/family/disney_trip_001.webp";
import disneyTripTwo from "../images/family/disney_trip_002.webp";
import disneyTripThree from "../images/family/disney_trip_003.webp";
import familyNatureWalk from "../images/family/nature_walk.webp";
import waterParkOne from "../images/family/water_park_001.webp";
import waterParkTwo from "../images/family/water_park_002.webp";
import waterParkThree from "../images/family/water_park_003.webp";
import stiFront from "../images/sti/FrontDriverSide.webp";
import stiSide from "../images/sti/SideDriverSide.webp";

function Gallery() {
  return(
    <div className="p-1">
      <div className="container">
        <h4>Gallery Albums</h4>

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

      {/* 30th Birthday Images */}
      <a className="" data-bs-toggle="modal" data-bs-target="#staticBackdropBirthdayDinner">
        {/* <img src={birthdayDinnerThree} width="100px" className="img-fluid img-thumbnail float-left" /> */}
        {/* <img src={birthdayDinnerThree} className="rounded mb-4 shadow img-fluid border border-success" /> */}
        <ImgWithFallback src={birthdayDinnerThree} fallback={birthdayDinnerThreeFallback} alt="" className="rounded mb-4 shadow img-fluid border border-success" />
      </a>

      {/* Popup window */}
      <div className="modal fade" id="staticBackdropBirthdayDinner" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="staticBackdropLabel">30th Birthday Dinner</h5>
              <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body text-center">
              
              {/* SlideShow (Carousel) START */}
              <div id="carouselBirthdayDinnerIndicators" className="carousel slide carousel-fade" data-bs-ride="carousel">
                <div className="carousel-indicators">
                  <button type="button" data-bs-target="#carouselBirthdayDinnerIndicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
                  <button type="button" data-bs-target="#carouselBirthdayDinnerIndicators" data-bs-slide-to="1" aria-label="Slide 2"></button>
                  <button type="button" data-bs-target="#carouselBirthdayDinnerIndicators" data-bs-slide-to="2" aria-label="Slide 3"></button>
                  <button type="button" data-bs-target="#carouselBirthdayDinnerIndicators" data-bs-slide-to="3" aria-label="Slide 4"></button>
                </div>
                <div className="carousel-inner">
                  <div className="carousel-item active" data-bs-interval="4000">
                    {/* <img src={birthdayDinnerThree} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={birthdayDinnerThree} fallback={birthdayDinnerThreeFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    {/* <img src={birthdayDinnerTwo} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={birthdayDinnerTwo} fallback={birthdayDinnerTwoFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    {/* <img src={birthdayDinnerFour} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={birthdayDinnerFour} fallback={birthdayDinnerFourFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item">
                    {/* <img src={birthdayDinnerOne} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={birthdayDinnerOne} fallback={birthdayDinnerOneFallback} className="img-fluid img-thumbnail" />
                  </div>
                </div>
                <button className="carousel-control-prev" type="button" data-bs-target="#carouselBirthdayDinnerIndicators" data-bs-slide="prev">
                  <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Previous</span>
                </button>
                <button className="carousel-control-next" type="button" data-bs-target="#carouselBirthdayDinnerIndicators" data-bs-slide="next">
                  <span className="carousel-control-next-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Next</span>
                </button>
              </div>
              {/* SlideShow (Carousel) END */}

            </div>
            <div className="modal-footer">
              {/* TODO */}
            </div>
          </div>
        </div>
      </div>

      {/* Christmas Images */}
      <a className="" data-bs-toggle="modal" data-bs-target="#staticBackdropChristmas">
        {/* <img src={christmas2021KissimmeeFamilyPhoto} width="100px" className="img-fluid img-thumbnail float-left" /> */}
        {/* <img src={christmas2021KissimmeeFamilyPhoto} className="rounded mb-4 shadow img-fluid border border-success" /> */}
        <ImgWithFallback src={christmas2021KissimmeeFamilyPhoto} fallback={christmas2021KissimmeeFamilyPhotoFallback} className="rounded mb-4 shadow img-fluid border border-success" />
      </a>

      {/* Popup window */}
      <div className="modal fade" id="staticBackdropChristmas" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="staticBackdropLabel">Christmas Collage</h5>
              <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body text-center">
              
              {/* SlideShow (Carousel) START */}
              <div id="carouselChristmasIndicators" className="carousel slide carousel-fade" data-bs-ride="carousel">
                <div className="carousel-indicators">
                  <button type="button" data-bs-target="#carouselChristmasIndicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
                  <button type="button" data-bs-target="#carouselChristmasIndicators" data-bs-slide-to="1" aria-label="Slide 2"></button>
                  <button type="button" data-bs-target="#carouselChristmasIndicators" data-bs-slide-to="2" aria-label="Slide 3"></button>
                  <button type="button" data-bs-target="#carouselChristmasIndicators" data-bs-slide-to="3" aria-label="Slide 4"></button>
                  <button type="button" data-bs-target="#carouselChristmasIndicators" data-bs-slide-to="4" aria-label="Slide 5"></button>
                  <button type="button" data-bs-target="#carouselChristmasIndicators" data-bs-slide-to="5" aria-label="Slide 6"></button>
                  <button type="button" data-bs-target="#carouselChristmasIndicators" data-bs-slide-to="5" aria-label="Slide 7"></button>
                </div>
                <div className="carousel-inner">
                  <div className="carousel-item active" data-bs-interval="4000">
                    {/* <img src={christmas2021KissimmeeFamilyPhoto} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={christmas2021KissimmeeFamilyPhoto} fallback={christmas2021KissimmeeFamilyPhotoFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    {/* <img src={christmas2021kissimmeeWifeAndI} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={christmas2021kissimmeeWifeAndI} fallback={christmas2021kissimmeeWifeAndIFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    {/* <img src={christmas2021LakeMary} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={christmas2021LakeMary} fallback={christmas2021LakeMaryFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    {/* <img src={christmas2021GayLordHotel} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={christmas2021GayLordHotel} fallback={christmas2021GayLordHotelFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    {/* <img src={christmas2020} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={christmas2020} fallback={christmas2020Fallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    {/* <img src={christmas2019} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={christmas2019} fallback={christmas2019Fallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item">
                    {/* <img src={newYearsEve2019} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={newYearsEve2019} fallback={newYearsEve2019Fallback} className="img-fluid img-thumbnail" />
                  </div>
                </div>
                <button className="carousel-control-prev" type="button" data-bs-target="#carouselChristmasIndicators" data-bs-slide="prev">
                  <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Previous</span>
                </button>
                <button className="carousel-control-next" type="button" data-bs-target="#carouselChristmasIndicators" data-bs-slide="next">
                  <span className="carousel-control-next-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Next</span>
                </button>
              </div>
              {/* SlideShow (Carousel) END */}

            </div>
            <div className="modal-footer">
              {/* TODO */}
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
      
      {/* sti front image */}
      <a className="" data-bs-toggle="modal" data-bs-target="#staticBackdrop_STI">
        <ImgWithFallback src={stiFront} fallback={stiFrontFallback} className="rounded mb-4 shadow img-fluid border border-success" />
      </a>

      {/* Popup Window */}
      <div className="modal fade" id="staticBackdrop_STI" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="staticBackdropLabel">2005 Subaru Impreza WRX STI</h5>
              <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body text-center">
              
              {/* SlideShow (Carousel) START */}
              <div id="carouselSTIIndicators" className="carousel slide carousel-fade" data-bs-ride="carousel">
                <div className="carousel-indicators">
                  <button type="button" data-bs-target="#carouselSTIIndicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
                  <button type="button" data-bs-target="#carouselSTIIndicators" data-bs-slide-to="1" aria-label="Slide 2"></button>
                </div>
                <div className="carousel-inner">
                  <div className="carousel-item active" data-bs-interval="4000">
                    <ImgWithFallback src={stiFront} fallback={stiFrontFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item">
                    <ImgWithFallback src={stiSide} fallback={stiSideFallback} className="img-fluid img-thumbnail" />
                  </div>
                </div>
                <button className="carousel-control-prev" type="button" data-bs-target="#carouselSTIIndicators" data-bs-slide="prev">
                  <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Previous</span>
                </button>
                <button className="carousel-control-next" type="button" data-bs-target="#carouselSTIIndicators" data-bs-slide="next">
                  <span className="carousel-control-next-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Next</span>
                </button>
              </div>
              {/* SlideShow (Carousel) END */}

            </div>
            <div className="card-footer">
              <ul className="list-unstyled">
                {/* <li>High Boost:
                  <ul>
                    <li>Engine HP: 549.3 HP</li>
                    <li>Boost: 26.9 PSI</li>
                    <li>477.7 HP @ 6050 RPM</li>`
                    <li>447.5 Ib-ft @ 5050 RPM</li>
                  </ul>
                </li> */}

                {/* <li>Low Boost:
                  <ul>
                    <li>Engine HP: 434.4 HP</li>
                    <li>Boost: 18.8 PSI</li>
                    <li>371.4 HP @ 5700 RPM</li>
                    <li>363.8 Ib-ft @ 5200 RPM</li>
                  </ul>
                </li> */}

                {/* <li>Engine:
                  <ul>
                    <li>Street fighter 2 block
                      <ul>
                        <li>703 STI Case Halves Casting</li>
                        <li>ARP Head stud kit 260-4701</li>
                        <li>Cometic head gasket</li>
                        <li>Bored and Plateau hone with deck plates</li>
                        <li>H-Beam 4340 Rods</li>
                        <li>Competition Bearings</li>
                        <li>Forged & Coated Pistons</li>
                      </ul>
                    </li>
                    <li>Crawford Air Oil Separator w/ FMIC</li>
                    <li>Perrin Equal Length Header</li>
                    <li>Omni power 3bar MAP sensor conversion with re-tune</li>
                    <li>STI oil cooler</li>
                  </ul>
                </li> */}
                
                {/* <li>Exhaust:
                  <ul>
                    <li>Perrin Performance equal length header</li>
                    <li>HKS Hi-Power Knock Off exhaust (Turbo Back)</li>
                  </ul>
                </li> */}
                
                {/* <li>Clutch:
                  <ul>
                    <li>Clutch Masters FX500 Stage 5 Rigid 4-Puck Clutch</li>
                    <li>Motul Rbf 660 Racing Brake Fluid-Motocross Brake Fluid</li>
                  </ul>
                </li> */}
                
                {/* <li>Turbo:
                  <ul>
                    <li>Perrin Rotated Mount GT3582R .63 AR Turbocharger</li>
                    <li>WRX/STI Perrin Intercooler Core & Bumper Beam</li>
                    <li>TurboXS Rotated Mount Intercoller Pipe Kit</li>
                    <li>HKS SSQV</li>
                    <li>Tial 60mm Wastegate</li>
                  </ul>
                </li> */}

                {/* <li>Electronics:
                  <ul>
                    <li>APEXi Subaru STI AVC-R Boost Controller</li>
                    <li>Greddy Black FATT Turbo Timer II</li>
                    <li>Clarion NX409 6.5-Inch 2-DIN Multimedia Station W/ Touch Panel</li>
                    <li>SMC Methanol Injection Kit</li>
                    <li>AutoMeter 3604 Sport Comp II SeriesBoost Gauge 0-35 PSI</li>
                    <li>AutoMeter 3327 Sport Comp 2-1/16" Oil Pressure Gauge</li>
                    <li>AutoMeter 3344 2-1/16" Sport Comp Pyrometer Gauge Kit</li>
                    <li>AutoMeter 52mm Full Triple Pillar Pod Subaru WRX/STI</li>
                    <li>Cobb Access Port V2</li>
                  </ul>
                </li> */}

                {/* <li>Suspension:
                  <ul>
                    <li>Cusco Bushing Rear Differential Mount Collar GRB GVB STI</li>
                    <li>Springs - Tein SKS60-AUB00 1.5 inch Front, 1.7 Rear</li>
                  </ul>
                </li> */}

                {/* <li>Wheels:
                  <ul>
                    <li>BBS Type-M 18 and 225 / 45-18</li>
                  </ul>
                </li> */}

                {/* <li>Brakes:
                  <ul>
                    <li>DBA 5000 Front Drilled & Slotted 2-Piece Rotor W/ Gold Hat</li>
                    <li>STI DBA 4000 series Drilled & Slotted Rear</li>
                    <li>Stoptech Stainless Steel Brake Lines Front & Rear</li>
                    <li>Motul Rbf 660 Racing Brake Fluid-Motorcross Brake Fluid</li>
                    <li>05 WRX STI Hawk HB453V.585 HT Brake Pads Front</li>
                    <li>04 WRX STI Hawk DTC-60 Brake Pads Rear</li>
                  </ul>
                </li> */}

              </ul>
            </div>
          </div>
        </div>
      </div>

      {/* Water Park Images */}
      <a className="" data-bs-toggle="modal" data-bs-target="#staticBackdropWaterPark">
        {/* <img src={waterParkOne} width="100px" className="img-fluid img-thumbnail float-left" /> */}
        {/* <img src={waterParkOne} className="rounded mb-4 shadow img-fluid border border-success"  /> */}
        <ImgWithFallback src={waterParkOne} fallback={waterParkOneFallback} className="rounded mb-4 shadow img-fluid border border-success"  />
      </a>

      {/* Popup window */}
      <div className="modal fade" id="staticBackdropWaterPark" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="staticBackdropLabel">Water Park Fun</h5>
              <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body text-center">
              
              {/* SlideShow (Carousel) START */}
              <div id="carouselWaterParkIndicators" className="carousel slide carousel-fade" data-bs-ride="carousel">
                <div className="carousel-indicators">
                  <button type="button" data-bs-target="#carouselWaterParkIndicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
                  <button type="button" data-bs-target="#carouselWaterParkIndicators" data-bs-slide-to="1" aria-label="Slide 2"></button>
                  <button type="button" data-bs-target="#carouselWaterParkIndicators" data-bs-slide-to="2" aria-label="Slide 3"></button>
                </div>
                <div className="carousel-inner">
                  <div className="carousel-item active" data-bs-interval="4000">
                    {/* <img src={waterParkOne} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={waterParkOne} fallback={waterParkOneFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    {/* <img src={waterParkTwo} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={waterParkTwo} fallback={waterParkTwoFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item">
                    {/* <img src={waterParkThree} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={waterParkThree} fallback={waterParkThreeFallback} className="img-fluid img-thumbnail" />
                  </div>
                </div>
                <button className="carousel-control-prev" type="button" data-bs-target="#carouselWaterParkIndicators" data-bs-slide="prev">
                  <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Previous</span>
                </button>
                <button className="carousel-control-next" type="button" data-bs-target="#carouselWaterParkIndicators" data-bs-slide="next">
                  <span className="carousel-control-next-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Next</span>
                </button>
              </div>
              {/* SlideShow (Carousel) END */}

            </div>
            <div className="modal-footer">
              {/* TODO */}
            </div>
          </div>
        </div>
      </div>

      {/* Nature Images */}
      <a className="" data-bs-toggle="modal" data-bs-target="#staticBackdropNature">
        {/* <img src={familyNatureWalk} width="100px" className="img-fluid img-thumbnail float-left" /> */}
        {/* <img src={familyNatureWalk} className="rounded mb-4 shadow img-fluid border border-success" /> */}
        <ImgWithFallback src={familyNatureWalk} fallback={familyNatureWalkFallback} className="rounded mb-4 shadow img-fluid border border-success" />
      </a>

      {/* Popup window */}
      <div className="modal fade" id="staticBackdropNature" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="staticBackdropLabel">Nature Adventures</h5>
              <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body text-center">
              
              {/* SlideShow (Carousel) START */}
              <div id="carouselNatureIndicators" className="carousel slide carousel-fade" data-bs-ride="carousel">
                <div className="carousel-indicators">
                  <button type="button" data-bs-target="#carouselNatureIndicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
                  <button type="button" data-bs-target="#carouselNatureIndicators" data-bs-slide-to="1" aria-label="Slide 2"></button>
                </div>
                <div className="carousel-inner">
                  <div className="carousel-item active" data-bs-interval="4000">
                    {/* <img src={familyNatureWalk} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={familyNatureWalk} fallback={familyNatureWalkFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item">
                    {/* <img src={wifesBDayManatees} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={wifesBDayManatees} fallback={wifesBDayManateesFallback} className="img-fluid img-thumbnail" />
                  </div>
                </div>
                <button className="carousel-control-prev" type="button" data-bs-target="#carouselNatureIndicators" data-bs-slide="prev">
                  <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Previous</span>
                </button>
                <button className="carousel-control-next" type="button" data-bs-target="#carouselNatureIndicators" data-bs-slide="next">
                  <span className="carousel-control-next-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Next</span>
                </button>
              </div>
              {/* SlideShow (Carousel) END */}

            </div>
            <div className="modal-footer">
              {/* TODO */}
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
      {/* Disney 2021 Images */}
      <a className="" data-bs-toggle="modal" data-bs-target="#staticBackdropDisney2021">
        {/* <img src={disneyTripThree} width="100px" className="img-fluid img-thumbnail float-left" /> */}
        {/* <img src={disneyTripThree} className="rounded mb-4 shadow img-fluid border border-success" /> */}
        <ImgWithFallback src={disneyTripThree} fallback={disneyTripThreeFallback} className="rounded mb-4 shadow img-fluid border border-success" />
      </a>

      {/* Popup window */}
      <div className="modal fade" id="staticBackdropDisney2021" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="staticBackdropLabel">Disney Trip 2021</h5>
              <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body text-center">
              
              {/* SlideShow (Carousel) START */}
              <div id="carouselDisney2021Indicators" className="carousel slide carousel-fade" data-bs-ride="carousel">
                <div className="carousel-indicators">
                  <button type="button" data-bs-target="#carouselDisney2021Indicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
                  <button type="button" data-bs-target="#carouselDisney2021Indicators" data-bs-slide-to="1" aria-label="Slide 2"></button>
                  <button type="button" data-bs-target="#carouselDisney2021Indicators" data-bs-slide-to="2" aria-label="Slide 3"></button>
                  <button type="button" data-bs-target="#carouselDisney2021Indicators" data-bs-slide-to="3" aria-label="Slide 4"></button>
                </div>
                <div className="carousel-inner">
                  <div className="carousel-item active" data-bs-interval="4000">
                    {/* <img src={disneyTripThree} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={disneyTripThree} fallback={disneyTripThreeFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    {/* <img src={disneyTripOne} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={disneyTripOne} fallback={disneyTripOneFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    {/* <img src={disneyTrip} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={disneyTrip} fallback={disneyTripFallback} className="img-fluid img-thumbnail" />
                  </div>
                  <div className="carousel-item" data-bs-interval="4000">
                    {/* <img src={disneyTripTwo} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={disneyTripTwo} fallback={disneyTripTwoFallback} className="img-fluid img-thumbnail" />
                  </div>
                </div>
                <button className="carousel-control-prev" type="button" data-bs-target="#carouselDisney2021Indicators" data-bs-slide="prev">
                  <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Previous</span>
                </button>
                <button className="carousel-control-next" type="button" data-bs-target="#carouselDisney2021Indicators" data-bs-slide="next">
                  <span className="carousel-control-next-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Next</span>
                </button>
              </div>
              {/* SlideShow (Carousel) END */}

            </div>
            <div className="modal-footer">
              {/* TODO */}
            </div>
          </div>
        </div>
      </div>


      
      {/* Date Night Images */}
      <a className="" data-bs-toggle="modal" data-bs-target="#staticBackdropDateNight">
        {/* <img src={nightOutDinnerDate} className="rounded mb-4 shadow img-fluid border border-success" /> */}
        <ImgWithFallback src={nightOutDinnerDate} fallback={nightOutDinnerDateFallback} className="rounded mb-4 shadow img-fluid border border-success" />
      </a>

      {/* Popup window */}
      <div className="modal fade" id="staticBackdropDateNight" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex="-1" aria-labelledby="staticBackdropLabel" aria-hidden="true">
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="staticBackdropLabel">Date Nights</h5>
              <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body text-center">
              
              {/* SlideShow (Carousel) START */}
              <div id="carouselDisney2021Indicators" className="carousel slide carousel-fade" data-bs-ride="carousel">
                <div className="carousel-indicators">
                  <button type="button" data-bs-target="#carouselDateNightIndicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
                  {/* <button type="button" data-bs-target="#carouselDateNightIndicators" data-bs-slide-to="1" aria-label="Slide 2"></button> */}
                </div>
                <div className="carousel-inner">
                  <div className="carousel-item active" data-bs-interval="4000">
                    {/* <img src={nightOutDinnerDate} className="img-fluid img-thumbnail" /> */}
                    <ImgWithFallback src={nightOutDinnerDate} fallback={nightOutDinnerDateFallback} className="img-fluid img-thumbnail" />
                  </div>
                  {/* <div className="carousel-item" data-bs-interval="4000">
                    <img src={disneyTripOne} className="img-fluid img-thumbnail" />
                  </div> */}
                </div>
                <button className="carousel-control-prev" type="button" data-bs-target="#carouselDateNightIndicators" data-bs-slide="prev">
                  <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Previous</span>
                </button>
                <button className="carousel-control-next" type="button" data-bs-target="#carouselDateNightIndicators" data-bs-slide="next">
                  <span className="carousel-control-next-icon" aria-hidden="true"></span>
                  <span className="visually-hidden">Next</span>
                </button>
              </div>
              {/* SlideShow (Carousel) END */}

            </div>
            <div className="modal-footer">
              {/* TODO */}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}


export { Gallery };