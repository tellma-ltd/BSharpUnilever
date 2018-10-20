export class Product {
  Id: number;
  Description: string;
  Barcode: string;
  SapCode: string;
  Type: string;
  IsPromo: boolean;
  IsActive = true;
}

const choicesProductType: { [key: string]: string; } = {
  'HC': 'Home Care',
  'PC': 'Personal Care',
  'F&R': 'Food & Refreshments',
  'O': 'Other',
};

export { choicesProductType };


