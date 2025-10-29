// External imports
import React, { useEffect, useMemo, useState } from 'react';

// Internal imports
import Page from "../../components/screens/Page";
import Button from "../../components/buttons/Button";
import AuctionClock from "../../components/elements/AuctionClock";

function UserDashboard() {
  const CLOCK_SECONDS = 4;
  const [price, setPrice] = useState<number>(0.65);
  // Productenlijst (dummy data)
  type Product = {
    supplier: string;
    avr: string;
    name: string;
    land: string;
    mps: string;
    brief: string;
    kwa: string;
    qi: string;
    minStemLen: string;
    stemsPerBundle: string;
    ripeness: string;
    image: string;
  };

  const products = useMemo<Product[]>(() => [
    {
      supplier: 'Kees van Os',
      avr: '4177',
      name: 'R Gr Red Naomi!',
      land: 'NL',
      mps: 'A',
      brief: '32214a',
      kwa: 'A1',
      qi: 'A',
      minStemLen: '50 cm',
      stemsPerBundle: '10',
      ripeness: '3-3',
      image: '/pictures/plant 4.png',
    },
    {
      supplier: 'Flora BV',
      avr: '5032',
      name: 'Tulipa Yellow King',
      land: 'NL',
      mps: 'A',
      brief: '11802b',
      kwa: 'A2',
      qi: 'B',
      minStemLen: '40 cm',
      stemsPerBundle: '20',
      ripeness: '2-3',
      image: '/pictures/plant 2.png',
    },
    {
      supplier: 'BloomCo',
      avr: '6120',
      name: 'Gerbera Mix',
      land: 'NL',
      mps: 'B',
      brief: '90211c',
      kwa: 'A1',
      qi: 'A',
      minStemLen: '45 cm',
      stemsPerBundle: '15',
      ripeness: '2-2',
      image: '/pictures/plant 1.png',
    },
    {
      supplier: 'GreenFields',
      avr: '7102',
      name: 'Chrysanthemum White',
      land: 'DE',
      mps: 'A',
      brief: '55231a',
      kwa: 'A2',
      qi: 'A',
      minStemLen: '55 cm',
      stemsPerBundle: '12',
      ripeness: '3-4',
      image: '/pictures/plant 3.png',
    },
  ], []);

  const [productIndex, setProductIndex] = useState<number>(0);
  const current = products[productIndex];
  // afbeelding bron per product
  const [imgSrc, setImgSrc] = useState<string>(current.image);
  useEffect(() => { setImgSrc(current.image); }, [current]);

  const upcoming = useMemo(() => {
    const after = products.slice(productIndex + 1);
    const before = products.slice(0, productIndex);
    return [...after, ...before];
  }, [products, productIndex]);
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
              <div className="prod-row"><span className="prod-label">Aanvoerder</span><span className="prod-val">{current.supplier}</span><span className="prod-label">avr nr</span><span className="prod-val">{current.avr}</span></div>
              <div className="prod-row"><span className="prod-label">Product</span><span className="prod-val prod-val--wide">{current.name}</span><span className="prod-label">land</span><span className="prod-val">{current.land}</span><span className="prod-label">mps</span><span className="prod-val">{current.mps}</span></div>
              <div className="prod-row"><span className="prod-label">brief</span><span className="prod-val">{current.brief}</span><span className="prod-label">kwa</span><span className="prod-val">{current.kwa}</span><span className="prod-label">qi</span><span className="prod-val">{current.qi}</span></div>
              <div className="prod-row"><span className="prod-label">minimum steellengte</span><span className="prod-val prod-val--wide">{current.minStemLen}</span></div>
              <div className="prod-row"><span className="prod-label">aantal stelen per bos</span><span className="prod-val prod-val--wide">{current.stemsPerBundle}</span></div>
              <div className="prod-row"><span className="prod-label">rijpheidstadium</span><span className="prod-val prod-val--wide">{current.ripeness}</span></div>
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
              <Button
                className="user-action-btn"
                label="Ander product"
                onClick={() => {
                  setProductIndex((i) => (i + 1) % products.length);
                  // reset price/clock for new product
                  setPaused(true);
                  setTimeout(() => {
                    setResetToken((v) => v + 1);
                    setPrice(0.65);
                    setPaused(false);
                  }, 300);
                }}
              />
            </div>
          </div>

          {/* Right side badge/indicator */}
          <div className="user-card-aside" aria-hidden />
        </div>
      </section>

      {/* Upcoming queue under the clock */}
      <section className="upcoming">
        <h3 className="upcoming-title">Volgende producten</h3>
        <ul className="upcoming-list">
          {upcoming.map((p, i) => (
            <li className="upcoming-item" key={i}>
              <img
                className="upcoming-thumb"
                src={p.image}
                alt={p.name}
                onError={(e) => { (e.currentTarget as HTMLImageElement).src = '/pictures/kweker.png'; }}
              />
              <div className="upcoming-info">
                <div className="upcoming-name">{p.name}</div>
                <div className="upcoming-meta">Aanvoerder: {p.supplier} · Lengte: {p.minStemLen} · Bos: {p.stemsPerBundle}</div>
              </div>
              <div className="upcoming-badge">{p.kwa}</div>
            </li>
          ))}
        </ul>
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



