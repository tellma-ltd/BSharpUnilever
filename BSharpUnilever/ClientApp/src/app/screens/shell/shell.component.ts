import { Component } from '@angular/core';
import { AuthService } from '../../data/auth.service';

@Component({
  selector: 'b-shell',
  templateUrl: './shell.component.html',
  styles: []
})
export class ShellComponent {

  // For the menu
  public isCollapsed = true;

  constructor(private auth: AuthService) {

  }

  onToggleCollapse() {
    this.isCollapsed = !this.isCollapsed;
  }

  onSignOut() {
    this.auth.signOutAndChallengeUser();
  }
}
