import React, { useMemo, useState } from "react";
// Zet dit boven je component die ze gebruikt (bijv. boven Dashboard_Veiling)

// Kleine helpercomponenten
const Stat: React.FC<{ label: string; value: string | number }> = ({
  label,
  value,
}) => (
  <div style={{ display: "flex", flexDirection: "column", gap: 2 }}>
    <span style={{ fontSize: 12, color: "#6b7280" }}>{label}</span>
    <span style={{ fontSize: 14, fontWeight: 600 }}>{value}</span>
  </div>
);

const PlaceholderImg: React.FC = () => (
  <div
    style={{
      width: 80,
      height: 80,
      borderRadius: 12,
      background: "#f3f4f6",
      border: "1px solid #e5e7eb",
      display: "grid",
      placeContent: "center",
      fontSize: 20,
    }}
  >
    {/* simpele icoon/vervanger */}
    <span role="img" aria-label="gavel">
      🔨
    </span>
  </div>
);

// --- Common prop helpers ---
type DivProps = React.PropsWithChildren<React.HTMLAttributes<HTMLDivElement>>;

type ButtonProps = React.PropsWithChildren<
  React.ButtonHTMLAttributes<HTMLButtonElement>
> & {
  variant?: "default" | "outline" | "secondary" | "destructive";
  size?: "sm" | "md" | "icon";
};

type InputProps = React.InputHTMLAttributes<HTMLInputElement>;

type BadgeProps = React.PropsWithChildren<{
  tone?: "default" | "secondary";
  style?: React.CSSProperties;
}>;

// ------------------------------
// Mini UI Componenten (met children getypt)
// ------------------------------
export function Card({ children, style, ...p }: DivProps) {
  return (
    <div
      {...p}
      style={{
        border: "1px solid #e5e7eb",
        borderRadius: 12,
        background: "#fff",
        boxShadow: "0 1px 2px rgba(0,0,0,0.04)",
        ...style,
      }}
    >
      {children}
    </div>
  );
}

export function CardHeader({ children, style, ...p }: DivProps) {
  return (
    <div
      {...p}
      style={{ padding: 16, borderBottom: "1px solid #f1f5f9", ...style }}
    >
      {children}
    </div>
  );
}

export function CardContent({ children, style, ...p }: DivProps) {
  return (
    <div {...p} style={{ padding: 16, ...style }}>
      {children}
    </div>
  );
}

export function CardTitle({ children, style, ...p }: DivProps) {
  return (
    <h2 {...p} style={{ margin: 0, fontSize: 18, fontWeight: 600, ...style }}>
      {children}
    </h2>
  );
}

export function CardDescription({ children, style, ...p }: DivProps) {
  return (
    <p {...p} style={{ margin: 0, color: "#6b7280", fontSize: 14, ...style }}>
      {children}
    </p>
  );
}

export function Button({
  children,
  variant = "default",
  size = "md",
  style,
  ...p
}: ButtonProps) {
  const base: React.CSSProperties = {
    display: "inline-flex",
    alignItems: "center",
    justifyContent: "center",
    gap: 8,
    borderRadius: 8,
    border: "1px solid transparent",
    cursor: "pointer",
    transition: "all .15s ease",
    fontWeight: 500,
    whiteSpace: "nowrap",
  };
  const sizes: Record<string, React.CSSProperties> = {
    sm: { padding: "6px 10px", fontSize: 14 },
    md: { padding: "8px 14px", fontSize: 14 },
    icon: { padding: 8, width: 40, height: 36, fontSize: 14 },
  };
  const variants: Record<string, React.CSSProperties> = {
    default: { background: "#2563eb", color: "#fff" },
    outline: { background: "#fff", borderColor: "#e5e7eb", color: "#111827" },
    secondary: { background: "#f3f4f6", color: "#111827" },
    destructive: { background: "#dc2626", color: "#fff" },
  };
  return (
    <button
      {...p}
      style={{ ...base, ...sizes[size], ...variants[variant], ...style }}
      onMouseEnter={(e) => (e.currentTarget.style.opacity = "0.92")}
      onMouseLeave={(e) => (e.currentTarget.style.opacity = "1")}
    >
      {children}
    </button>
  );
}

export function Input({ style, ...p }: InputProps) {
  return (
    <input
      {...p}
      style={{
        width: "100%",
        padding: "8px 10px",
        borderRadius: 8,
        border: "1px solid #e5e7eb",
        outline: "none",
        fontSize: 14,
        ...style,
      }}
    />
  );
}

export function Badge({ children, tone = "default", style }: BadgeProps) {
  return (
    <span
      style={{
        display: "inline-flex",
        alignItems: "center",
        gap: 6,
        padding: "2px 8px",
        borderRadius: 999,
        fontSize: 12,
        border: "1px solid",
        borderColor: tone === "default" ? "#dbeafe" : "#e5e7eb",
        background: tone === "default" ? "#eff6ff" : "#f9fafb",
        color: "#111827",
        ...style,
      }}
    >
      {children}
    </span>
  );
}

export function Avatar({ children, style, ...p }: DivProps) {
  return (
    <div
      {...p}
      style={{
        width: 32,
        height: 32,
        borderRadius: "50%",
        background: "#e5e7eb",
        display: "grid",
        placeContent: "center",
        fontSize: 12,
        fontWeight: 600,
        ...style,
      }}
    >
      {children}
    </div>
  );
}

export function Progress({
  value,
  style,
}: {
  value: number;
  style?: React.CSSProperties;
}) {
  return (
    <div
      style={{
        height: 8,
        width: "100%",
        background: "#e5e7eb",
        borderRadius: 999,
        ...style,
      }}
    >
      <div
        style={{
          height: 8,
          width: `${Math.max(0, Math.min(100, value))}%`,
          background: "#2563eb",
          borderRadius: 999,
          transition: "width .25s ease",
        }}
      />
    </div>
  );
}

export function HStack({ children, style, ...p }: DivProps) {
  return (
    <div
      {...p}
      style={{ display: "flex", alignItems: "center", gap: 8, ...style }}
    >
      {children}
    </div>
  );
}

export function VStack({ children, style, ...p }: DivProps) {
  return (
    <div
      {...p}
      style={{ display: "flex", flexDirection: "column", gap: 8, ...style }}
    >
      {children}
    </div>
  );
}

// ------------------------------
// Emoji-Icons (geen extra dependencies)
// ------------------------------
const Icon = {
  Bell: () => <span>🔔</span>,
  Search: () => <span>🔎</span>,
  Gavel: () => <span>🔨</span>,
  Timer: () => <span>⏱️</span>,
  Up: () => <span>▲</span>,
  Down: () => <span>▼</span>,
  Play: () => <span>▶︎</span>,
  Stop: () => <span>■</span>,
  Plus: () => <span>＋</span>,
  Minus: () => <span>−</span>,
  Users: () => <span>👥</span>,
  Settings: () => <span>⚙️</span>,
};

// ------------------------------
// Mock data
// ------------------------------
const MOCK_LOTS = [
  {
    id: "K-1024",
    title: "Rozen – ‘Avalanche+’ Premium (20 stuks)",
    seller: "Bloemenhof BV",
    startPrice: 6.5,
    highestBid: 9.2,
    bids: 14,
    status: "actief" as const,
  },
  {
    id: "K-1025",
    title: "Tulpen – Mix Voorjaar (50 stuks)",
    seller: "Van der Meer",
    startPrice: 4.0,
    highestBid: 0,
    bids: 0,
    status: "komend" as const,
  },
  {
    id: "K-1011",
    title: "Lelies – Orientals (10 stuks)",
    seller: "Lilium Co.",
    startPrice: 7.1,
    highestBid: 12.0,
    bids: 22,
    status: "verkocht" as const,
  },
];

// ------------------------------
// Hoofdcomponent
// ------------------------------
export default function Dashboard_Veiling() {
  const [query, setQuery] = useState("");
  const [activeTab, setActiveTab] = useState<"actief" | "komend" | "verkocht">(
    "actief"
  );
  const [currentPrice, setCurrentPrice] = useState(8.5);
  const [clock, setClock] = useState(75); // 0..100
  const [isLive, setIsLive] = useState(true);

  const filteredLots = useMemo(
    () =>
      MOCK_LOTS.filter(
        (l) =>
          l.status === activeTab &&
          (l.title.toLowerCase().includes(query.toLowerCase()) ||
            l.id.toLowerCase().includes(query.toLowerCase()))
      ),
    [activeTab, query]
  );

  return (
    <div
      style={{ minHeight: "100vh", background: "#f8fafc", color: "#0f172a" }}
    >
      {/* Topbar */}
      <header
        style={{
          position: "sticky",
          top: 0,
          zIndex: 10,
          borderBottom: "1px solid #e5e7eb",
          background: "rgba(255,255,255,.9)",
          backdropFilter: "blur(6px)",
        }}
      >
        <div style={{ maxWidth: 1120, margin: "0 auto", padding: "0 16px" }}>
          <HStack style={{ height: 56, justifyContent: "space-between" }}>
            <HStack>
              <div
                style={{
                  width: 32,
                  height: 32,
                  borderRadius: 8,
                  background: "#dbeafe",
                  display: "grid",
                  placeContent: "center",
                  color: "#2563eb",
                }}
              >
                <Icon.Gavel />
              </div>
              <span style={{ fontWeight: 600 }}>Veilingklok Pro</span>
            </HStack>

            <HStack style={{ gap: 8 }}>
              <Input
                value={query}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                  setQuery(e.target.value)
                }
                placeholder="Zoeken…"
                style={{ width: 200 }}
              />
              <Button variant="outline" size="icon" aria-label="Meldingen">
                <Icon.Bell />
              </Button>
              <Avatar>VM</Avatar>
            </HStack>
          </HStack>
        </div>
      </header>

      {/* Heading */}
      <section
        style={{ maxWidth: 1120, margin: "0 auto", padding: "24px 16px" }}
      >
        <h1 style={{ fontSize: 22, fontWeight: 600 }}>
          Veilingmeester Dashboard
        </h1>
        <p style={{ color: "#6b7280" }}>
          Beheer live kavels, biedingen en aankomende rondes.
        </p>
      </section>

      <main
        style={{
          maxWidth: 1120,
          margin: "0 auto",
          padding: "0 16px 48px",
          display: "grid",
          gap: 16,
        }}
      >
        {/* Live Auction */}
        <Card>
          <CardHeader
            style={{ display: "flex", justifyContent: "space-between" }}
          >
            <div>
              <CardTitle>Live kavel</CardTitle>
              <CardDescription>
                Kavel K-1024 • Rozen ‘Avalanche+’ Premium
              </CardDescription>
            </div>
            <HStack>
              <Badge tone={isLive ? "default" : "secondary"}>
                <span
                  style={{
                    width: 8,
                    height: 8,
                    borderRadius: "50%",
                    background: "#22c55e",
                    display: "inline-block",
                  }}
                />
                {isLive ? "Live" : "Gepauzeerd"}
              </Badge>
              <Button
                variant={isLive ? "destructive" : "default"}
                size="sm"
                onClick={() => setIsLive((v) => !v)}
              >
                {isLive ? (
                  <>
                    <Icon.Stop /> Stop
                  </>
                ) : (
                  <>
                    <Icon.Play /> Start
                  </>
                )}
              </Button>
            </HStack>
          </CardHeader>

          <CardContent>
            <HStack
              style={{
                justifyContent: "space-between",
                flexWrap: "wrap",
                gap: 24,
              }}
            >
              {/* Verkoper + stats */}
              <HStack style={{ alignItems: "flex-start" }}>
                <PlaceholderImg />
                <VStack>
                  <div style={{ color: "#6b7280" }}>Verkoper</div>
                  <div style={{ fontWeight: 600 }}>Bloemenhof BV</div>
                  <HStack>
                    <Stat label="Startprijs" value="€ 6,50" />
                    <Stat label="Biedingen" value={28} />
                    <Stat label="Voorraad" value="120 bossen" />
                  </HStack>
                </VStack>
              </HStack>

              {/* Klok + balk */}
              <VStack>
                <div style={{ color: "#6b7280" }}>Veilingklok</div>
                <HStack>
                  <div
                    style={{
                      position: "relative",
                      display: "grid",
                      placeContent: "center",
                      width: 96,
                      height: 96,
                      borderRadius: "50%",
                      border: "1px solid #e5e7eb",
                      background: "#f9fafb",
                      textAlign: "center",
                    }}
                  >
                    <div style={{ fontSize: 12, color: "#6b7280" }}>
                      Resterend
                    </div>
                    <div style={{ fontSize: 22, fontWeight: 700 }}>
                      {Math.max(0, Math.round((clock / 100) * 60))}s
                    </div>
                    <div
                      style={{
                        position: "absolute",
                        inset: 0,
                        borderRadius: "50%",
                        boxShadow: "inset 0 0 0 6px #2563eb",
                        clipPath: `inset(${100 - clock}% 0 0 0)`,
                        transition: "clip-path .3s ease",
                      }}
                    />
                  </div>

                  <VStack style={{ width: 200 }}>
                    <Progress value={clock} />
                    <HStack>
                      <Button
                        variant="outline"
                        size="icon"
                        onClick={() => setClock((v) => Math.max(0, v - 10))}
                      >
                        <Icon.Down />
                      </Button>
                      <Button
                        variant="outline"
                        size="icon"
                        onClick={() => setClock((v) => Math.min(100, v + 10))}
                      >
                        <Icon.Up />
                      </Button>
                    </HStack>
                  </VStack>
                </HStack>
              </VStack>

              {/* Bied controls */}
              <VStack style={{ alignItems: "flex-end" }}>
                <div style={{ color: "#6b7280" }}>Huidig bod</div>
                <div style={{ fontSize: 34, fontWeight: 800 }}>
                  € {currentPrice.toFixed(2)}
                </div>
                <HStack>
                  <Button
                    variant="outline"
                    size="icon"
                    onClick={() =>
                      setCurrentPrice((p) => Math.max(0, +(p - 0.1).toFixed(2)))
                    }
                  >
                    <Icon.Minus />
                  </Button>
                  <Button
                    size="icon"
                    onClick={() =>
                      setCurrentPrice((p) => +(p + 0.1).toFixed(2))
                    }
                  >
                    <Icon.Plus />
                  </Button>
                  <Button>
                    <Icon.Gavel /> Plaats bod
                  </Button>
                </HStack>
                <HStack style={{ gap: 24, fontSize: 12, color: "#6b7280" }}>
                  <span>
                    <Icon.Users /> 86 kijkers
                  </span>
                  <span>
                    <Icon.Timer /> ronde 3
                  </span>
                </HStack>
              </VStack>
            </HStack>
          </CardContent>
        </Card>

        {/* Kavellijst (vereenvoudigde tabs) */}
        <Card>
          <CardHeader>
            <HStack style={{ justifyContent: "space-between" }}>
              <div>
                <CardTitle>Kavellijst</CardTitle>
                <CardDescription>
                  Overzicht van actieve, komende en verkochte kavels.
                </CardDescription>
              </div>
              <HStack>
                <Button variant="outline" size="sm">
                  <Icon.Settings /> Kolommen
                </Button>
                <Button size="sm">
                  <Icon.Play /> Start ronde
                </Button>
              </HStack>
            </HStack>
          </CardHeader>

          <CardContent>
            <HStack style={{ marginBottom: 12 }}>
              {(["actief", "komend", "verkocht"] as const).map((tab) => (
                <Button
                  key={tab}
                  variant={activeTab === tab ? "default" : "outline"}
                  size="sm"
                  onClick={() => setActiveTab(tab)}
                >
                  {tab[0].toUpperCase() + tab.slice(1)}
                </Button>
              ))}
            </HStack>

            <div style={{ border: "1px solid #e5e7eb", borderRadius: 12 }}>
              {filteredLots.length === 0 && (
                <div style={{ padding: 24, fontSize: 14, color: "#6b7280" }}>
                  Geen kavels gevonden.
                </div>
              )}
              {filteredLots.map((lot) => (
                <div
                  key={lot.id}
                  style={{
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "space-between",
                    gap: 16,
                    padding: 16,
                    borderTop: "1px solid #f1f5f9",
                  }}
                >
                  <HStack style={{ alignItems: "flex-start", gap: 16 }}>
                    <div
                      style={{
                        width: 80,
                        height: 80,
                        borderRadius: 12,
                        background: "#f3f4f6",
                        border: "1px solid #e5e7eb",
                        display: "grid",
                        placeContent: "center",
                        fontSize: 20,
                      }}
                    >
                      <Icon.Gavel />
                    </div>
                    <div>
                      <div style={{ fontSize: 14, color: "#6b7280" }}>
                        {lot.id} • {lot.seller}
                      </div>
                      <div style={{ fontWeight: 600 }}>{lot.title}</div>
                    </div>
                  </HStack>

                  <div
                    style={{
                      display: "grid",
                      gridTemplateColumns: "repeat(3,minmax(0,1fr))",
                      gap: 24,
                      minWidth: 320,
                    }}
                  >
                    <Stat
                      label="Start"
                      value={`€ ${lot.startPrice.toFixed(2)}`}
                    />
                    <Stat
                      label="Hoogste bod"
                      value={
                        lot.highestBid ? `€ ${lot.highestBid.toFixed(2)}` : "—"
                      }
                    />
                    <Stat label="Biedingen" value={lot.bids} />
                  </div>

                  <HStack>
                    {lot.status === "komend" && (
                      <Button size="sm">
                        <Icon.Play /> Start
                      </Button>
                    )}
                    {lot.status === "actief" && (
                      <Button variant="secondary" size="sm">
                        Openen
                      </Button>
                    )}
                    {lot.status === "verkocht" && (
                      <Button variant="outline" size="sm">
                        Details
                      </Button>
                    )}
                  </HStack>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </main>
    </div>
  );
}
