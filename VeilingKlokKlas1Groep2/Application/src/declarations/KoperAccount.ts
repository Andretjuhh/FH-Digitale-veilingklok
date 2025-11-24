export class NewKoperAccount {
	constructor(
		public name: string,
		public email: string,
		public password: string,
		public telephone: string,

		public address: string,
		public postcode: string,
		public regio: string,
		public kvkNumber: string,
		public createdAt: Date
	) {}
}
