// CreateVeilingKlokDTO.ts
export interface CreateVeilingKlokDTO {
	scheduledAt: string;
	veilingDurationSeconds: number;
	products: Record<string, number>;
}
