export class User {
  Id: number;
  FullName: string;
  Email: string;
  Role: string;
  EmailConfirmed: boolean;
}


export enum SupportRequestState {
  KAE = 'KAE',
  Manager = 'Manager',
  Administrator = 'Administrator',
  Inactive = 'Inactive',
}
