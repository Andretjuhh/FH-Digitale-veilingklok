// CreateVeilingKlokDTO.ts
export interface CreateVeilingKlokDTO {
  scheduledAt: string;
  veilingDurationMinutes: number;
  products: Record<string, number>;
}
