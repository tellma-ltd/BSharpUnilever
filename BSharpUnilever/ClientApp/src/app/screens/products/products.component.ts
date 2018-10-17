import { Component } from '@angular/core';
import { choicesProductType } from '../../data/entities/Product';
import { GlobalsResolverService } from '../../data/globals-resolver.service';

@Component({
  selector: 'b-products',
  templateUrl: './products.component.html',
  styles: []
})
export class ProductsComponent {

  constructor(private globals: GlobalsResolverService) { }

  canCreate = () => {
    return this.globals.currentUser.Role === 'Administrator';
  }

  getDisplay(key: string): string {
    return choicesProductType[key];
  }
}
