// CreateVeilingKlokDTO.ts
export interface CreateVeilingKlokDTO {
	scheduledAt: string; // ISO string
	veilingDurationMinutes: number;
	products: Record<string, number>; // Dictionary<Guid, decimal>
}
