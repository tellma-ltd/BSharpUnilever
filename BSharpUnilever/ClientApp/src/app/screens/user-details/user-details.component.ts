import { Component, ViewChild } from '@angular/core';
import { SupportRequestState } from '../../data/entities/User';
import { GlobalsResolverService } from '../../data/globals-resolver.service';
import { DetailsComponent } from '../../layouts/details/details.component';
import { ICanDeactivate } from '../../misc/deactivate.guard';

@Component({
  selector: 'b-user-details',
  templateUrl: './user-details.component.html',
  styles: []
})
export class UserDetailsComponent implements ICanDeactivate {

  @ViewChild(DetailsComponent)
  details: DetailsComponent;

  constructor(private globals: GlobalsResolverService) { }

  // It might make sense to move these to a base class for
  // all details components, instead of repeating ourselves
  canDeactivate(): boolean {
    return this.details.canDeactivate();
  }

  get roles() {
    return Object.keys(SupportRequestState).map(key =>
      ({ value: key, display: SupportRequestState[key] }));
  }

  canUpdate = () => {
    return this.globals.currentUser.Role === 'Administrator';
  }
}
