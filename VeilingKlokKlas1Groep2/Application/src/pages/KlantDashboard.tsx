import React, { useEffect, useMemo, useState } from "react";
import "../styles/Dashboard.css";

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
  const [activeTab, setActiveTab] = useState<"live" | "bids" | "history">("live");
  const [timeLeft, setTimeLeft] = useState<string>("00:00");

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
        <div className="brand-mark" aria-hidden>
          {/* You can replace with an <img alt="Veilingplatform" src="/logo.svg" /> */}
          <span>VK</span>
        </div>
        <div className="app-title">Veilingplatform</div>

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

              <div className="card__body">
                <div className="metric">
                  <div className="metric__label">Huidige prijs</div>
                  <div className="metric__value">€ {liveItem.price.toFixed(2)}</div>
                  <div className="metric__sublabel">/{liveItem.unit}</div>
                </div>

                <div className="actions">
                  <button className="btn-primary" onClick={() => alert("Bod geplaatst!")}>
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

export default KlantDashboard;
