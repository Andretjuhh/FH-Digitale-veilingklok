import React, { useEffect, useMemo, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/Dashboard.css";
import "../styles/SiteFooter.css";
import SiteFooter from "../components/SiteFooter";




type AuctionItem = {
  id: number;
  name: string;
  lotNumber: string;
  price: number; // current price
  unit: string;
  endsAt: Date;  // countdown target
};

type Bid = {
  id: number;
  lotNumber: string;
  amount: number;
  time: string;
};

const formatTimeLeft = (target: Date) => {
  const diff = Math.max(0, target.getTime() - Date.now());
  const s = Math.floor(diff / 1000);
  const mm = String(Math.floor(s / 60)).padStart(2, "0");
  const ss = String(s % 60).padStart(2, "0");
  return `${mm}:${ss}`;
};

const KlantDashboard: React.FC = () => {
  const navigate = useNavigate();

  const [activeTab, setActiveTab] = useState<"live" | "bids" | "history">("live");
  const [timeLeft, setTimeLeft] = useState<string>("00:00");
  const [theme, setTheme] = useState<"light" | "dark">(() => {
    const stored = localStorage.getItem("theme");
    if (stored === "light" || stored === "dark") return stored;
    const prefersDark = window.matchMedia?.("(prefers-color-scheme: dark)").matches ?? false;
    return prefersDark ? "dark" : "light";
  });

  // hoeveelheid (aantal) onder de klok
  const [qty, setQty] = useState<number>(1);
  const MIN_QTY = 1;
  const MAX_QTY = 999;

  const dec = () => setQty(q => Math.max(MIN_QTY, q - 1));
  const inc = () => setQty(q => Math.min(MAX_QTY, q + 1));

  useEffect(() => {
    document.documentElement.setAttribute("data-theme", theme);
    localStorage.setItem("theme", theme);
  }, [theme]);

  const toggleTheme = () => setTheme(t => (t === "dark" ? "light" : "dark"));

  // --- demo data (replace with API) ---
  const liveItem = useMemo<AuctionItem>(() => ({
    id: 1,
    name: "Rozen – Avalanche+",
    lotNumber: "LOT-02451",
    price: 0.42,
    unit: "per steel",
    endsAt: new Date(Date.now() + 3 * 60 * 1000), // +3 min
  }), []);

  const myBids = useMemo<Bid[]>(() => ([
    { id: 1, lotNumber: "LOT-02412", amount: 0.39, time: "09:12" },
    { id: 2, lotNumber: "LOT-02398", amount: 0.41, time: "08:55" },
  ]), []);

  const history = useMemo<Bid[]>(() => ([
    { id: 11, lotNumber: "LOT-02310", amount: 0.37, time: "Gisteren 16:02" },
    { id: 12, lotNumber: "LOT-02277", amount: 0.36, time: "Gisteren 11:47" },
  ]), []);

  // countdown
  useEffect(() => {
    if (activeTab !== "live") return;
    const tick = () => setTimeLeft(formatTimeLeft(liveItem.endsAt));
    tick();
    const id = setInterval(tick, 1000);
    return () => clearInterval(id);
  }, [activeTab, liveItem.endsAt]);

  return (
    <div className="dashboard-page">
      {/* Top bar */}
      <header className="dashboard-topbar" role="banner">
        {/* LOGO → home */}
        <button
          type="button"
          className="brand-mark brand-mark--button"
          onClick={() => navigate("/")}
          aria-label="Ga naar Home"
          title="Home"
        >
          <span aria-hidden>VK</span>
        </button>

        <div className="app-title">Veilingplatform</div>

        {/* Tabs */}
        <nav className="dashboard-tabs" role="tablist" aria-label="Klant dashboard tabs">
          <button
            className={`tab-btn ${activeTab === "live" ? "is-active" : ""}`}
            role="tab"
            aria-selected={activeTab === "live"}
            onClick={() => setActiveTab("live")}
          >
            Live
          </button>
          <button
            className={`tab-btn ${activeTab === "bids" ? "is-active" : ""}`}
            role="tab"
            aria-selected={activeTab === "bids"}
            onClick={() => setActiveTab("bids")}
          >
            Mijn biedingen
          </button>
          <button
            className={`tab-btn ${activeTab === "history" ? "is-active" : ""}`}
            role="tab"
            aria-selected={activeTab === "history"}
            onClick={() => setActiveTab("history")}
          >
            Geschiedenis
          </button>
        </nav>

        {/* Rechter acties: thema + login */}
        <div className="topbar-actions">
          <button
            type="button"
            className="icon-btn"
            onClick={toggleTheme}
            aria-label={`Schakel naar ${theme === "dark" ? "dag" : "nacht"} modus`}
            title={theme === "dark" ? "Schakel naar dag modus" : "Schakel naar nacht modus"}
          >
            <span aria-hidden>{theme === "dark" ? "☀️" : "🌙"}</span>
          </button>

          <Link to="/login" className="btn-primary btn-login">
            Login
          </Link>
        </div>
      </header>

      {/* Content */}
      <main className="dashboard-content" role="main">
        {activeTab === "live" && (
          <section aria-labelledby="live-heading">
            <h2 id="live-heading" className="visually-hidden">Live veiling</h2>

            <div className="card">
              <div className="card__header">
                <div>
                  <div className="eyebrow">Loting</div>
                  <h3 className="card__title">
                    {liveItem.name} <span className="muted">({liveItem.lotNumber})</span>
                  </h3>
                </div>
                <div className="stat">
                  <div className="stat__label">Tijd resterend</div>
                  <div className="stat__value" aria-live="polite">{timeLeft}</div>
                </div>
              </div>

{/* ===== GROTE RONDE KLOK + PRODUCTPANE ===== */}
<div className="auction-layout">
  {/* Linker kolom: productafbeelding + naam */}
  <div className="product-pane">
    <img
      className="product-img"
      src={`https://picsum.photos/seed/${liveItem.id}-flower/520/520`}
      alt={liveItem.name}
      loading="lazy"
    />
    <div className="product-name">{liveItem.name}</div>
  </div>

  {/* Rechter kolom: klok + aantal knoppen */}
  <div className="clock-pane">
    <div className="clock">
      <div className="clock__face" aria-label={`Tijd resterend ${timeLeft}`} role="img">
        <div className="clock__time">{timeLeft}</div>
        <div className="clock__subtext">mm:ss</div>
      </div>

      <div className="qty-controls" aria-label="Kies hoeveelheid">
        <button
          type="button"
          className="btn-round"
          aria-label="Verlaag aantal"
          onClick={dec}
          disabled={qty <= MIN_QTY}
        >
          –
        </button>

        <div className="qty-display" aria-live="polite" aria-atomic="true">
          {qty}
        </div>

        <button
          type="button"
          className="btn-round"
          aria-label="Verhoog aantal"
          onClick={inc}
          disabled={qty >= MAX_QTY}
        >
          +
        </button>
      </div>

      <div className="qty-hint muted">Prijs wordt berekend per {liveItem.unit}</div>
    </div>
  </div>
</div>
{/* ===== EINDE KLOK + PRODUCTPANE ===== */}


              <div className="card__body">
                <div className="metric">
                  <div className="metric__label">Huidige prijs</div>
                  <div className="metric__value">€ {liveItem.price.toFixed(2)}</div>
                  <div className="metric__sublabel">/{liveItem.unit}</div>
                </div>

                <div className="actions">
                  <button
                    className="btn-primary"
                    onClick={() => alert(`Bod geplaatst voor ${qty} × ${liveItem.name}`)}
                  >
                    Plaats bod
                  </button>
                  <button className="btn-secondary" onClick={() => alert("Automatisch bieden geactiveerd")}>
                    Auto-bid
                  </button>
                  <button className="btn-quiet" onClick={() => alert("Toegevoegd aan favorieten")}>
                    Favoriet
                  </button>
                </div>
              </div>

              <div className="card__footer">
                <small className="muted">
                  Let op: Biedingen zijn bindend. Controleer hoeveelheid en prijs per eenheid.
                </small>
              </div>
            </div>
          </section>
        )}

        {activeTab === "bids" && (
          <section aria-labelledby="bids-heading">
            <h2 id="bids-heading" className="visually-hidden">Mijn biedingen</h2>

            <div className="table">
              <div className="table__row table__row--head">
                <div>Lot</div>
                <div>Bedrag</div>
                <div>Tijd</div>
              </div>
              {myBids.map(b => (
                <div className="table__row" key={b.id}>
                  <div>{b.lotNumber}</div>
                  <div>€ {b.amount.toFixed(2)}</div>
                  <div>{b.time}</div>
                </div>
              ))}
            </div>
          </section>
        )}

        {activeTab === "history" && (
          <section aria-labelledby="history-heading">
            <h2 id="history-heading" className="visually-hidden">Geschiedenis</h2>

            <div className="table">
              <div className="table__row table__row--head">
                <div>Lot</div>
                <div>Bedrag</div>
                <div>Tijd</div>
              </div>
              {history.map(h => (
                <div className="table__row" key={h.id}>
                  <div>{h.lotNumber}</div>
                  <div>€ {h.amount.toFixed(2)}</div>
                  <div>{h.time}</div>
                </div>
              ))}
            </div>
          </section>
        )}
      </main>

      {/* Footer */}
      <footer className="dashboard-footer">
        <div className="footer-actions">
          <button className="btn-secondary" onClick={() => window.location.reload()}>
            Ververs
          </button>
          <button className="btn-quiet" onClick={() => alert("Uitgelogd")}>
            Uitloggen
          </button>
        </div>
        <div className="muted">© {new Date().getFullYear()} Veilingplatform</div>
      </footer>
    </div>
  );
};


<SiteFooter />

export default KlantDashboard;
