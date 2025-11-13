export class NewKwekerAccount {
	constructor(
		public name: string,
		public email: string,
		public password: string,
		public createdAt: Date,
		public telephone: string,
		public address: string,
		public regio: string,
		public kvkNumber: string
	) {}
}
