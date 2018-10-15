import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { DataService } from './data.service';
import { Observable, Subject } from 'rxjs';
import { AuthService } from './auth.service';
import { User } from './entities/User';
import { tap, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class GlobalsResolverService implements Resolve<boolean> {

  // This service ensures that the current user object is retrieved and stored
  // in a global place accessible to all components, components use the
  // information in the current user (e.g. the role) to modify their behavior

  private _currentUser: User;

  public get currentUser(): User {
    return this._currentUser;
  }

  constructor(private data: DataService, private auth: AuthService) {

    // Clear the current user after signing-out
    auth.signedOut$.subscribe(() => {
      this._currentUser = null;
    });
  }

  resolve(): Observable<boolean> | boolean {
    if (!!this._currentUser) {
      return true;
    } else {
      return this.data.users.getCurrent(new Subject<void>()).pipe(
        tap(user => this._currentUser = user),
        map(() => true)
      );
    }
  }
}
