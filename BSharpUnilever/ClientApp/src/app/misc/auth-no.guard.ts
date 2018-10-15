import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../data/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthNoGuard implements CanActivate {

  // This guard prevents a user who is already authenticated from accessing
  // certain screens that allow them to sign in again (potentially as a different
  // user) such as the sign -in screen, this is to avoid anomalous behavior

  constructor(private auth: AuthService, private router: Router) { }

  canActivate(): boolean {
    if (this.auth.isAuthenticated) {
      this.router.navigate(['']);
      return false;
    } else {
      return true;
    }
  }
}
