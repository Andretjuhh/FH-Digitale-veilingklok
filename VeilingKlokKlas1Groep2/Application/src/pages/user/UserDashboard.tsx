// External imports
import React, { useEffect, useState } from 'react';

// Internal imports
import Page from "../../components/screens/Page";
import Button from "../../components/buttons/Button";

function UserDashboard() {
  const [price, setPrice] = useState<number>(1);
  // Probeer lokale foto 'roses.jpg' uit public/pictures; val terug op SVG en dan op bestaande afbeelding
  const [imgSrc, setImgSrc] = useState<string>("/pictures/roses.jpg");

  // Laat de prijs geleidelijk dalen tot 0
  useEffect(() => {
    const id = window.setInterval(() => {
      setPrice(p => Math.max(0, +(p - 0.01).toFixed(2)));
    }, 1000);
    return () => clearInterval(id);
  }, []);

  return (
    <Page enableHeader className="user-dashboard">
      <section className="user-hero">
        <h1 className="user-hero-title">Mijn Dashboard</h1>
        <p className="user-hero-sub">Overzicht van je profiel en acties</p>
      </section>

      <section className="user-card-wrap">
        <div className="user-card">
          {/* Left media block */}
          <img
            className="user-card-media"
            src={imgSrc}
            onError={() => setImgSrc(prev => prev.endsWith('.jpg') ? '/pictures/plant 4.png' : '/pictures/kweker.png')}
            alt="Rozen"
          />

          {/* Center profile area */}
          <div className="user-card-center">
            <div className="user-avatar">
              <span className="user-price">{price > 0 ? `€ ${price.toFixed(2)}` : 'Gratis'}</span>
            </div>
            <div className="user-name" />
            <div className="user-meta" />

            <div className="user-actions">
              <Button className="user-action-btn !bg-primary-main" label="Actie 1" />
              <Button className="user-action-btn" label="Actie 2" />
            </div>
          </div>

          {/* Right side badge/indicator */}
          <div className="user-card-aside" aria-hidden />
        </div>
      </section>

      <footer className="user-footer">
        <div className="user-footer-col">
          <div className="user-footer-title" />
          <div className="user-footer-line" />
          <div className="user-footer-line" />
        </div>
        <div className="user-footer-col">
          <div className="user-footer-list">
            <span />
            <span />
            <span />
          </div>
        </div>
        <div className="user-footer-col">
          <div className="user-footer-list">
            <span />
            <span />
            <span />
          </div>
        </div>
        <div className="user-footer-col">
          <div className="user-footer-list">
            <span />
            <span />
            <span />
          </div>
        </div>
      </footer>
    </Page>
  );
}

export default UserDashboard;
