import { User } from './User';
import { Store } from './Store';
import { Product } from './Product';

export class SupportRequest {
  Id: number;
  Date: string;
  SerialNumber: number;
  State: SupportRequestState;
  AccountExecutive: User;
  Manager: User;
  Reason: 'DC' | 'PS' | 'PR' | 'FB';
  Store: Store;
  Comment: string;
  LineItems: SupportRequestLineItem[];
  StateChanges: StateChange[];
  GeneratedDocuments: GeneratedDocument[];
  CreatedBy: string;
  Created: string;
  ModifiedBy: string;
  Modified: string;
}

export enum SupportRequestState {
  Draft = 'Draft',
  Submitted = 'Submitted',
  Approved = 'Approved',
  Posted = 'Posted',
  Canceled = 'Canceled',
  Rejected = 'Rejected'
}

const supportRequestReasons: { [key: string]: string; } = {
  DC: 'Display Contract',
  PS: 'Premium Support',
  PR: 'Price Reduction',
  FB: 'From Balance'
};

export { supportRequestReasons };

export class SupportRequestLineItem {
  Id: number;
  Product: Product;
  Quantity: number;
  RequestedSupport: number;
  RequestedValue: number;
  ApprovedSupport: number;
  ApprovedValue: number;
  UsedSupport: number;
  UsedValue: number;
}

export class StateChange {
  Id: number;
  FromState: SupportRequestState;
  ToState: SupportRequestState;
  Time: string;
  User: User;
}

export class GeneratedDocument {
  Id: number;
  SerialNumber: number;
  State: number;
  Date: string;
}
