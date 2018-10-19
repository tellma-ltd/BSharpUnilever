export class User {
  Id: number;
  FullName: string;
  Email: string;
  Role: 'KAE' | 'Manager' | 'Administrator' | 'Inactive';
  EmailConfirmed: boolean;
}


export enum SupportRequestState {
  KAE = 'KAE',
  Manager = 'Manager',
  Administrator = 'Administrator',
  Inactive = 'Inactive',
}
