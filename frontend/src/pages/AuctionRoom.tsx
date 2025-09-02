import { useState } from "react";

// Simpele type-definitie voor een bod
type Bid = {
  id: number;
  bidder: string;
  amount: number;
};

export default function AuctionRoom() {
  const [lot] = useState({
    id: "LOT-001",
    product: "Rozenboeket",
    quantity: 100,
    grower: "Bloemenkwekerij Janssen",
  });

  const [bids] = useState<Bid[]>([
    { id: 1, bidder: "Koper A", amount: 12.5 },
    { id: 2, bidder: "Koper B", amount: 13.0 },
    { id: 3, bidder: "Koper C", amount: 13.2 },
  ]);

  return (
    <div style={{ padding: "2rem" }}>
      <h1>Digitale Veilingklok</h1>

      <section style={{ marginBottom: "1.5rem" }}>
        <h2>Actueel product</h2>
        <p><strong>{lot.product}</strong></p>
        <p>Hoeveelheid: {lot.quantity}</p>
        <p>Teler: {lot.grower}</p>
      </section>

      <section>
        <h2>Biedingen</h2>
        <ul>
          {bids.map((bid) => (
            <li key={bid.id}>
              💰 {bid.bidder} bood € {bid.amount.toFixed(2)}
            </li>
          ))}
        </ul>
      </section>
    </div>
  );
}
