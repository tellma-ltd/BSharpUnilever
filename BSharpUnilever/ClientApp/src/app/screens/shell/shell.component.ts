import { Component } from '@angular/core';
import { AuthService } from '../../data/auth.service';
import { GlobalsResolverService } from '../../data/globals-resolver.service';

@Component({
  selector: 'b-shell',
  templateUrl: './shell.component.html',
  styles: []
})
export class ShellComponent {

  // For the menu
  public isCollapsed = true;

  constructor(private auth: AuthService, private globals: GlobalsResolverService) {

  }

  onToggleCollapse() {
    this.isCollapsed = !this.isCollapsed;
  }

  onCollapse() {
    this.isCollapsed = true;
  }

  onSignOut() {
    this.auth.signOutAndChallengeUser();
  }

  get currentUserFullName() {
    return this.globals.currentUser.FullName;
  }

  get currentUserRole() {
    return this.globals.currentUser.Role;
  }
}
