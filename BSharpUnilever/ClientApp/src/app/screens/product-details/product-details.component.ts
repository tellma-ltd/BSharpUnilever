import { Component, ViewChild } from '@angular/core';
import { choicesProductType, Product } from '../../data/entities/Product';
import { GlobalsResolverService } from '../../data/globals-resolver.service';
import { DetailsComponent } from '../../layouts/details/details.component';
import { ICanDeactivate } from '../../misc/deactivate.guard';

@Component({
  selector: 'b-product-details',
  templateUrl: './product-details.component.html',
  styles: []
})
export class ProductDetailsComponent implements ICanDeactivate {

  @ViewChild(DetailsComponent)
  details: DetailsComponent;

  createNew = () => {
    const result = new Product();
    result.IsActive = true;
    return result;
  };

  constructor(private globals: GlobalsResolverService) { }

  // It might make sense to move these to a base class for
  // all details components, instead of repeating ourselves
  canDeactivate(): boolean {
    return this.details.canDeactivate();
  }

  get types() {
    return Object.keys(choicesProductType).map(key =>
      ({ value: key, display: choicesProductType[key] }));
  }

  typeDisplay(key: string): string {
    return !!key ? choicesProductType[key] : '';
  }

  canUpdate = () => {
    return this.globals.currentUser.Role === 'Administrator';
  }
}
