import { Component, ViewChild } from '@angular/core';
import { User } from '../../data/entities/User';
import { GlobalsResolverService } from '../../data/globals-resolver.service';
import { DetailsComponent } from '../../layouts/details/details.component';
import { ICanDeactivate } from '../../misc/deactivate.guard';
import { Store } from '../../data/entities/Store';

@Component({
  selector: 'b-store-details',
  templateUrl: './store-details.component.html',
  styles: []
})
export class StoreDetailsComponent implements ICanDeactivate {

  @ViewChild(DetailsComponent)
  details: DetailsComponent;

  createNew = () => {
    const result = new Store();
    result.IsActive = true;
    return result;
  }

  constructor(private globals: GlobalsResolverService) { }

  canDeactivate(): boolean {
    return this.details.canDeactivate();
  }

  canUpdate = () => {
    return this.globals.currentUser.Role === 'Administrator';
  }

  userFormatter = (user: User) => user.FullName;
}
