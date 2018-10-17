import { Component } from '@angular/core';
import { GlobalsResolverService } from '../../data/globals-resolver.service';

@Component({
  selector: 'b-stores',
  templateUrl: './stores.component.html',
})
export class StoresComponent {

  constructor(private globals: GlobalsResolverService) { }

  canCreate = () => {
    return this.globals.currentUser.Role === 'Administrator';
  }
}
