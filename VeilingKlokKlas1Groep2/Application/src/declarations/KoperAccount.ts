export class NewKoperAccount {
	constructor(
		public name: string,
		public email: string,
		public password: string,

		public address: string,
		public postcode: string,
		public regio: string,
		public createdAt: Date
	) {}
}
