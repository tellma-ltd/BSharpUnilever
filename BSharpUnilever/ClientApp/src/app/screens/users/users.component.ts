import { Component } from '@angular/core';
import { GlobalsResolverService } from '../../data/globals-resolver.service';

@Component({
  selector: 'b-users',
  templateUrl: './users.component.html',
})
export class UsersComponent {

  constructor(private globals: GlobalsResolverService) { }

  canCreate = () => {
    return this.globals.currentUser.Role === 'Administrator';
  }
}
