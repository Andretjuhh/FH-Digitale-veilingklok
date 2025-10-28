import React, { useMemo, useState } from "react";
import {
  Bell,
  Search,
  Gavel,
  Timer,
  ChevronUp,
  ChevronDown,
  Play,
  Square,
  Plus,
  Minus,
  Users,
  Settings,
} from "lucide-react";
import {
  Card,
  CardHeader,
  CardContent,
  CardTitle,
  CardDescription,
} from "../components/ui/card";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Badge } from "../components/ui/badge";
import { Avatar, AvatarFallback } from "../components/ui/avatar";
import {
  Tabs,
  TabsList,
  TabsTrigger,
  TabsContent,
} from "../components/ui/tabs";
import { Progress } from "../components/ui/progress";

// --- Helper components ---
function Stat({ label, value }: { label: string; value: string | number }) {
  return (
    <div className="flex flex-col">
      <span className="text-xs text-muted-foreground">{label}</span>
      <span className="text-sm font-medium">{value}</span>
    </div>
  );
}

function PlaceholderImg() {
  return (
    <div className="aspect-square w-20 rounded-xl bg-muted/60 ring-1 ring-border grid place-content-center">
      <Gavel className="h-6 w-6" />
    </div>
  );
}

// --- Mock data ---
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

export default function Dashboard_Veiling() {
  const [query, setQuery] = useState("");
  const [activeTab, setActiveTab] = useState<"actief" | "komend" | "verkocht">("actief");

  // Live lot state
  const [currentPrice, setCurrentPrice] = useState(8.5);
  const [clock, setClock] = useState(75); // 0-100 progress
  const [isLive, setIsLive] = useState(true);

  const filteredLots = useMemo(() => {
    return MOCK_LOTS.filter(
      (l) =>
        l.status === activeTab &&
        (l.title.toLowerCase().includes(query.toLowerCase()) ||
          l.id.toLowerCase().includes(query.toLowerCase()))
    );
  }, [activeTab, query]);

  return (
    <div className="min-h-screen bg-background text-foreground">
      {/* Top Navigation */}
      <header className="sticky top-0 z-30 w-full border-b bg-background/80 backdrop-blur supports-[backdrop-filter]:bg-background/60">
        <div className="mx-auto max-w-7xl px-4">
          <div className="flex h-14 items-center justify-between gap-4">
            <div className="flex items-center gap-3">
              <div className="h-8 w-8 rounded-lg bg-primary/10 grid place-content-center">
                <Gavel className="h-4 w-4 text-primary" />
              </div>
              <span className="font-semibold tracking-tight">Veilingklok Pro</span>
            </div>

            <nav className="hidden md:flex items-center gap-6 text-sm text-muted-foreground">
              <a className="hover:text-foreground" href="#">Dashboard</a>
              <a className="hover:text-foreground" href="#">Kavels</a>
              <a className="hover:text-foreground" href="#">Kopers</a>
              <a className="hover:text-foreground" href="#">Rapporten</a>
              <a className="hover:text-foreground" href="#">Instellingen</a>
            </nav>

            <div className="flex items-center gap-2">
              <div className="relative hidden sm:block">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  value={query}
                  onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                    setQuery(e.target.value)
                  }
                  placeholder="Zoeken…"
                  className="pl-9 w-56"
                />
              </div>
              <Button variant="ghost" size="icon" aria-label="Meldingen">
                <Bell className="h-5 w-5" />
              </Button>
              <Avatar className="h-8 w-8">
                <AvatarFallback>VM</AvatarFallback>
              </Avatar>
            </div>
          </div>
        </div>
      </header>

      {/* Page heading */}
      <section className="mx-auto max-w-7xl px-4 py-6">
        <h1 className="text-2xl font-semibold tracking-tight">Veilingmeester Dashboard</h1>
        <p className="text-muted-foreground">Beheer live kavels, biedingen en aankomende rondes.</p>
      </section>

      <main className="mx-auto max-w-7xl px-4 pb-24 grid gap-6">
        {/* Live Auction Card */}
        <Card className="overflow-hidden">
          <CardHeader className="flex flex-row items-center justify-between">
            <div>
              <CardTitle>Live kavel</CardTitle>
              <CardDescription>Kavel K-1024 • Rozen ‘Avalanche+’ Premium</CardDescription>
            </div>
            <div className="flex items-center gap-2">
              <Badge variant={isLive ? "default" : "secondary"} className="gap-1">
                <span className="h-2 w-2 rounded-full bg-green-500" />
                {isLive ? "Live" : "Gepauzeerd"}
              </Badge>
              <Button
                variant={isLive ? "destructive" : "default"}
                size="sm"
                onClick={() => setIsLive((v) => !v)}
              >
                {isLive ? (
                  <span className="inline-flex items-center gap-2">
                    <Square className="h-4 w-4" /> Stop
                  </span>
                ) : (
                  <span className="inline-flex items-center gap-2">
                    <Play className="h-4 w-4" /> Start
                  </span>
                )}
              </Button>
            </div>
          </CardHeader>

          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-[auto_1fr_auto] items-center gap-6">
              <div className="flex items-start gap-4">
                <PlaceholderImg />
                <div className="space-y-2">
                  <div className="text-sm text-muted-foreground">Verkoper</div>
                  <div className="font-medium">Bloemenhof BV</div>
                  <div className="grid grid-cols-3 gap-4 pt-2">
                    <Stat label="Startprijs" value="€ 6,50" />
                    <Stat label="Biedingen" value={28} />
                    <Stat label="Voorraad" value="120 bossen" />
                  </div>
                </div>
              </div>

              {/* Clock + progress (no framer-motion) */}
              <div className="space-y-3">
                <div className="text-sm text-muted-foreground">Veilingklok</div>
                <div className="flex items-center gap-4">
                  <div className="relative grid place-content-center h-24 w-24 rounded-full border bg-muted/40">
                    <div className="text-xs text-muted-foreground">Resterend</div>
                    <div className="text-2xl font-semibold">
                      {Math.max(0, Math.round((clock / 100) * 60))}s
                    </div>
                    <div
                      className="absolute inset-0 rounded-full"
                      style={{
                        boxShadow: "inset 0 0 0 6px hsl(var(--primary))",
                        clipPath: `inset(${100 - clock}% 0 0 0)`,
                        transition: "clip-path 0.3s ease",
                      }}
                    />
                  </div>
                  <div className="w-64 max-w-full">
                    <Progress value={clock} />
                    <div className="mt-2 flex gap-2">
                      <Button
                        variant="outline"
                        size="icon"
                        onClick={() => setClock((v) => Math.max(0, v - 10))}
                      >
                        <ChevronDown className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="outline"
                        size="icon"
                        onClick={() => setClock((v) => Math.min(100, v + 10))}
                      >
                        <ChevronUp className="h-4 w-4" />
                      </Button>
                    </div>
                  </div>
                </div>
              </div>

              {/* Bid controls */}
              <div className="flex flex-col items-end gap-3">
                <div className="text-sm text-muted-foreground">Huidig bod</div>
                <div className="text-4xl font-bold tracking-tight">€ {currentPrice.toFixed(2)}</div>
                <div className="flex items-center gap-2">
                  <Button
                    variant="outline"
                    size="icon"
                    onClick={() =>
                      setCurrentPrice((p) => Math.max(0, +(p - 0.1).toFixed(2)))
                    }
                  >
                    <Minus className="h-4 w-4" />
                  </Button>
                  <Button
                    onClick={() => setCurr
