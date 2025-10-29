// External imports
import React, { useEffect, useState } from 'react';

// Internal imports
import Page from "../../components/screens/Page";
import Button from "../../components/buttons/Button";
import AuctionClock from "../../components/elements/AuctionClock";

function UserDashboard() {
  const CLOCK_SECONDS = 4;
  const [price, setPrice] = useState<number>(0.65);
  // Gebruik lokale stock-foto 'plant 4.png', val terug op roses.svg en dan kweker.png
  const [imgSrc, setImgSrc] = useState<string>("/pictures/plant 4.png");
  const [isDark, setIsDark] = useState<boolean>(() => {
    const saved = typeof window !== 'undefined' ? localStorage.getItem('theme') : null;
    return saved === 'dark';
  });
  const [paused, setPaused] = useState<boolean>(false);
  const [resetToken, setResetToken] = useState<number>(0);

  // prijs wordt gestuurd door de klok (onTick)



  return (
    <Page enableHeader className="user-dashboard">
      <section className="user-hero">
        <div className="user-hero-head">
          <div>
            <h1 className="user-hero-title">Mijn Dashboard</h1>
            <p className="user-hero-sub">Overzicht van je profiel en acties</p>
          </div>
          {/* <div className="user-hero-actions">
            <Button
              className={'app-home-s-btn'}
              label={isDark ? 'Licht thema' : 'Donker thema'}
              icon={isDark ? 'bi-sun-fill' : 'bi-moon-fill'}
              onClick={toggleTheme}
            />
          </div> */}
        </div>
      </section>

      <section className="user-card-wrap">
        <div className="user-card">
          {/* Left media block */}
          <div className="user-card-mediaBlock">
            <img
              className="user-card-media"
              src={imgSrc}
              onError={() => setImgSrc(prev => prev.endsWith('.svg') ? '/pictures/kweker.png' : '/pictures/roses.svg')}
              alt="Rozen"
            />
            <div className="product-info">
              <div className="prod-row"><span className="prod-label">Aanvoerder</span><span className="prod-val">Kees van Os</span><span className="prod-label">avr nr</span><span className="prod-val">4177</span></div>
              <div className="prod-row"><span className="prod-label">Product</span><span className="prod-val prod-val--wide">R Gr Red Naomi!</span><span className="prod-label">land</span><span className="prod-val">NL</span><span className="prod-label">mps</span><span className="prod-val">A</span></div>
              <div className="prod-row"><span className="prod-label">brief</span><span className="prod-val">32214a</span><span className="prod-label">kwa</span><span className="prod-val">A1</span><span className="prod-label">qi</span><span className="prod-val">A</span></div>
              <div className="prod-row"><span className="prod-label">minimum steellengte</span><span className="prod-val prod-val--wide">50 cm</span></div>
              <div className="prod-row"><span className="prod-label">aantal stelen per bos</span><span className="prod-val prod-val--wide">10</span></div>
              <div className="prod-row"><span className="prod-label">rijpheidstadium</span><span className="prod-val prod-val--wide">3-3</span></div>
            </div>
          </div>

          {/* Center profile area */}
          <div className="user-card-center">
            <AuctionClock
              totalSeconds={CLOCK_SECONDS}
              start
              paused={paused}
              resetToken={resetToken}
              round={1}
              coin={1}
              totalLots={16}
              amountPerLot={150}
              minAmount={1}
              price={price}
              onTick={(secs) => {
                const p = Math.max(0, (secs / CLOCK_SECONDS) * 0.65);
                setPrice(+p.toFixed(2));
              }}
            />

            <div className="user-actions">
              <Button
                className="user-action-btn !bg-primary-main"
                label="Koop product"
                onClick={() => {
                  setPaused(true);
                  setTimeout(() => {
                    // reset and resume
                    setResetToken((v) => v + 1);
                    setPrice(0.65);
                    setPaused(false);
                  }, 500);
                }}
              />
              <Button className="user-action-btn" label="Actie 2" />
            </div>
          </div>

          {/* Right side badge/indicator */}
          <div className="user-card-aside" aria-hidden />
        </div>
      </section>

      <footer className="user-footer">
        <div className="user-footer-col">
          <h4 className="user-footer-title">Over FloriClock</h4>
          <p className="user-footer-line">Digitale veiling voor bloemen en planten.</p>
          <p className="user-footer-line">Gebouwd door studenten — demo omgeving.</p>
        </div>
        <div className="user-footer-col">
          <h4 className="user-footer-title">Product</h4>
          <ul className="user-footer-list">
            <li><a href="#">Live veiling</a></li>
            <li><a href="#">Prijsgeschiedenis</a></li>
            <li><a href="#">Favorieten</a></li>
          </ul>
        </div>
        <div className="user-footer-col">
          <h4 className="user-footer-title">Resources</h4>
          <ul className="user-footer-list">
            <li><a href="#">Documentatie</a></li>
            <li><a href="#">Veelgestelde vragen</a></li>
            <li><a href="#">Status</a></li>
          </ul>
        </div>
        <div className="user-footer-col">
          <h4 className="user-footer-title">Contact</h4>
          <ul className="user-footer-list">
            <li><a href="#">Service & support</a></li>
            <li><a href="#">Contactformulier</a></li>
            <li><a href="#">Locaties</a></li>
          </ul>
        </div>
      </footer>
    </Page>
  );
}

export default UserDashboard;




