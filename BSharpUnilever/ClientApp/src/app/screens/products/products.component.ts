import { Component } from '@angular/core';
import { choicesProductType } from '../../data/entities/Product';

@Component({
  selector: 'b-products',
  templateUrl: './products.component.html',
  styles: []
})
export class ProductsComponent {

  getDisplay(key: string): string {
    return choicesProductType[key];
  }
}
