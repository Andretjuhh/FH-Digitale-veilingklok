export class NewVeilingmeesterAccount {
	constructor(
		public email: string,
		public password: string,
		public regio: string,
		public createdAt: Date,
        public authorisatieCode: number,
	) {}
}
