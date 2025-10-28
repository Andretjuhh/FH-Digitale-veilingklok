import React, { useMemo, useState } from "react";
import "../styles/login.css"; // reuse the same base styles
import "../styles/Dashboard.css"; // specific dashboard styles

function KlantDashboard() {
  const [activeTab, setActiveTab] = useState("overzicht");

  const stats = useMemo(
    () => [
      { label: "Openstaande bestellingen", value: 3 },
      { label: "Lopende biedingen", value: 7 },
      { label: "Tegoed (EUR)", value: "€ 1.245,00" },
      { label: "Favoriete aanvoerders", value: 5 },
    ],
    []
  );

  const recentOrders = [
    { id: "#10234", product: "Rosa Avalanche+®", qty: 40, price: "€ 0,45", status: "Verzonden" },
    { id: "#10221", product: "Phalaenopsis Mix", qty: 12, price: "€ 5,60", status: "Bevestigd" },
    { id: "#10188", product: "Tulipa Paars", qty: 50, price: "€ 0,18", status: "Geannuleerd" },
  ];

  const recentBids = [
    { lot: "RFH-24-00098", product: "Gerbera Mix", myBid: "€ 0,12", result: "Gewonnen" },
    { lot: "RFH-24-00091", product: "Ranonkel Wit", myBid: "€ 0,22", result: "Overboden" },
    { lot: "RFH-24-00087", product: "Alstroemeria", myBid: "€ 0,09", result: "Open" },
  ];

  const upcomingAuctions = [
    { time: "Vandaag 14:30", grower: "GreenBloom BV", product: "Rosa Avalanche+®", qty: 120 },
    { time: "Vandaag 15:10", grower: "OrchidHouse", product: "Phalaenopsis 12cm", qty: 80 },
    { time: "Morgen 09:00", grower: "TulipKing", product: "Tulipa Mix 40cm", qty: 500 },
  ];

  return (
    <div className="dashboard-page">
      {/* Top bar */}
      <header className="dashboard-topbar">
        <div className="topbar-left">
          <div className="brand-mark">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="28"
              height="28"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <circle cx="12" cy="12" r="9" />
              <path d="M8 12h8M12 8v8" />
            </svg>
          </div>
          <div className="brand-copy">
            <h1 className="app-title">Klant Dashboard</h1>
            <span className="app-subtitle">Welkom terug — beheer je veilingen en bestellingen</span>
          </div>
        </div>

        <div className="topbar-right">
          <button className="btn-quiet" aria-label="Zoeken">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>
          </button>
          <button className="btn-quiet has-dot" aria-label="Meldingen">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M18 8a6 6 0 0 0-12 0c0 7-3 9-3 9h18s-3-2-3-9"/><path d="M13.73 21a2 2 0 0 1-3.46 0"/></svg>
          </button>
          <div className="avatar">
            <span>KL</span>
          </div>
        </div>
      </header>

      {/* Tabs */}
      <nav className="dashboard-tabs" role="tablist" aria-label="Dashboard tabs">
        {[
          { id: "overzicht", label: "Overzicht" },
          { id: "bestellingen", label: "Bestellingen" },
          { id: "biedingen", label: "Biedingen" },
          { id: "profiel", label: "Profiel" },
        ].map((t) => (
          <button
            key={t.id}
            role="tab"
            aria-selected={activeTab === t.id}
            className={"tab-btn " + (activeTab === t.id ? "is-active" : "")}
            onClick={() => setActiveTab(t.id)}
          >
            {t.label}
          </button>
        ))}
      </nav>

      {/* Content */}
      <main className="dashboard-content">
        {activeTab === "overzicht" && (
          <section className="grid grid-12 gap-16">
            {/* Stats cards */}
            {stats.map((s) => (
              <div key={s.label} className="card stat-card col-span-12 sm:col-span-6 lg:col-span-3">
                <p className="stat-label">{s.label}</p>
                <p className="stat-value">{s.value}</p>
              </div>
            ))}

            {/* Recent Orders */}
            <div className="card col-span-12 lg:col-span-7">
              <div className="card-header">
                <h3 className="card-title">Recente bestellingen</h3>
                <button className="btn-link">Alles bekijken</button>
              </div>
              <div className="table-wrap">
                <table className="table">
                  <thead>
                    <tr>
                      <th>Order</th>
                      <th>Product</th>
                      <th>Aantal</th>
                      <th>Prijs / st.</th>
                      <th>Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {recentOrders.map((o) => (
                      <tr key={o.id}>
                        <td>{o.id}</td>
                        <td>{o.product}</td>
                        <td>{o.qty}</td>
                        <td>{o.price}</td>
                        <td>
                          <span className={`badge ${badgeClass(o.status)}`}>{o.status}</span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>

            {/* Upcoming Auctions */}
            <div className="card col-span-12 lg:col-span-5">
              <div className="card-header">
                <h3 className="card-title">Aankomende veilingen</h3>
                <button className="btn-link">Schema</button>
              </div>
              <ul className="list relaxed">
                {upcomingAuctions.map((a, idx) => (
                  <li key={idx} className="list-row">
                    <div className="list-leading">
                      <div className="pill pill-muted">{a.time}</div>
                    </div>
                    <div className="list-content">
                      <div className="list-title">{a.product}</div>
                      <div className="list-subtitle">
                        {a.grower} • {a.qty} stuks
                      </div>
                    </div>
                    <div className="list-trailing">
                      <button className="btn-secondary">Herinner mij</button>
                    </div>
                  </li>
                ))}
              </ul>
            </div>

            {/* Recent Bids */}
            <div className="card col-span-12">
              <div className="card-header">
                <h3 className="card-title">Mijn recente biedingen</h3>
                <div className="actions">
                  <button className="btn-secondary">Exporteren</button>
                </div>
              </div>
              <div className="table-wrap">
                <table className="table">
                  <thead>
                    <tr>
                      <th>Lot</th>
                      <th>Product</th>
                      <th>Mijn bod</th>
                      <th>Resultaat</th>
                    </tr>
                  </thead>
                  <tbody>
                    {recentBids.map((b, i) => (
                      <tr key={i}>
                        <td>{b.lot}</td>
                        <td>{b.product}</td>
                        <td>{b.myBid}</td>
                        <td>
                          <span className={`badge ${badgeClass(b.result)}`}>{b.result}</span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </section>
        )}

        {activeTab === "bestellingen" && (
          <EmptyState
            title="Nog geen filter toegepast"
            subtitle="Zoek of filter om specifieke bestellingen te vinden."
            cta="Open zoekbalk"
          />
        )}

        {activeTab === "biedingen" && (
          <EmptyState
            title="Geen actieve biedingen"
            subtitle="Zodra je biedt op een veiling, verschijnen ze hier."
            cta="Bekijk veilingen"
          />
        )}

        {activeTab === "profiel" && (
          <section className="grid grid-12 gap-16">
            <div className="card col-span-12 lg:col-span-6">
              <div className="card-header">
                <h3 className="card-title">Accountgegevens</h3>
              </div>
              <form className="form-grid">
                <label>
                  <span>Naam</span>
                  <input type="text" className="input-field" defaultValue="Klant Lando" />
                </label>
                <label>
                  <span>E-mail</span>
                  <input type="email" className="input-field" defaultValue="klant@example.com" />
                </label>
                <label>
                  <span>Telefoon</span>
                  <input type="tel" className="input-field" placeholder="+31 6 1234 5678" />
                </label>
                <div className="form-actions">
                  <button className="btn-primary" type="button">Opslaan</button>
                  <button className="btn-secondary" type="button">Wachtwoord wijzigen</button>
                </div>
              </form>
            </div>

            <div className="card col-span-12 lg:col-span-6">
              <div className="card-header">
                <h3 className="card-title">Meldingen</h3>
              </div>
              <ul className="list compact">
                <li className="list-row">
                  <div className="list-content">
                    <div className="list-title">Biedupdates</div>
                    <div className="list-subtitle">Push en e-mail</div>
                  </div>
                  <div className="list-trailing">
                    <Toggle defaultChecked />
                  </div>
                </li>
                <li className="list-row">
                  <div className="list-content">
                    <div className="list-title">Orderstatus</div>
                    <div className="list-subtitle">Alleen e-mail</div>
                  </div>
                  <div className="list-trailing">
                    <Toggle />
                  </div>
                </li>
                <li className="list-row">
                  <div className="list-content">
                    <div className="list-title">Nieuws & updates</div>
                    <div className="list-subtitle">Maandelijks</div>
                  </div>
                  <div className="list-trailing">
                    <Toggle />
                  </div>
                </li>
              </ul>
            </div>
          </section>
        )}
      </main>

      <footer className="dashboard-footer">
        <span>© {new Date().getFullYear()} — Jouw Veilingplatform</span>
        <div className="footer-actions">
          <button className="btn-link">Support</button>
          <button className="btn-link">Privacy</button>
          <button className="btn-link">Voorwaarden</button>
        </div>
      </footer>
    </div>
  );
}

/* ---------- Helpers & small UI atoms (pure CSS-driven) ---------- */
function badgeClass(state: string) {
  const s = (state ?? "").toString().toLowerCase();
  if (s.includes("verzonden") || s.includes("gewonnen") || s.includes("bevestigd")) return "badge-success";
  if (s.includes("open")) return "badge-warning";
  if (s.includes("overboden") || s.includes("geannuleerd")) return "badge-danger";
  return "badge-muted";
}

function EmptyState(
  { title, subtitle, cta }: { title: string; subtitle: string; cta: string }
): React.ReactElement {
  return (
    <div className="empty-state">
      <div className="empty-emoji" aria-hidden={true}>🌿</div>
      <h3 className="empty-title">{title}</h3>
      <p className="empty-subtitle">{subtitle}</p>
      <button className="btn-primary">{cta}</button>
    </div>
  );
}


function Toggle({ defaultChecked = false }) {
  const [on, setOn] = useState(defaultChecked);
  return (
    <button
      type="button"
      role="switch"
      aria-checked={on}
      className={"toggle " + (on ? "is-on" : "")}
      onClick={() => setOn(!on)}
    >
      <span className="thumb" />
    </button>
  );
}

export default KlantDashboard;
