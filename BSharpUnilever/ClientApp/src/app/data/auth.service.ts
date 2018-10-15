import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, of, Subject, throwError, timer } from 'rxjs';
import { catchError, filter, map, mergeMap, retryWhen, switchMap, takeUntil, tap } from 'rxjs/operators';
import { friendly } from '../misc/util';
import { AuthTokenResponse } from './entities/auth/AuthTokenResponse';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  // Refresh the user session every 1 hour
  private REFRESH_DURATION = 1000 * 60 * 60;
  private REFRESH_FAIL_DURATION = 1000 * 2;
  private LOCAL_STORAGE_KEY = 'idToken';

  public returnUrl: string;
  public creatingToken: boolean;
  public signedOut$ = new Subject<void>();
  public localStorageTokenChange$ =
    new BehaviorSubject<AuthTokenResponse>(this.localStorageToken);

  constructor(private router: Router, private http: HttpClient) {

    // Listen to changes on the local storage token from the
    // current window/tab
    this.localStorageTokenChange$.pipe(
      // set a new timer which signs the user out automatically when
      // the new token expires, while canceling any previous timer
      switchMap(newToken => {
        if (!newToken) {
          return of();
        }
        return timer(new Date(newToken.Expiration)).pipe(
          tap(() => this.signOutAndChallengeUser())
        );
      })
    ).subscribe();

    // If the user signs out or signs in from another window
    // we automatically sign out/in in this window in order to
    // achieve a consistent experience
    addEventListener('storage', (e: StorageEvent) => {
      if (e.key === this.LOCAL_STORAGE_KEY) {

        if (!e.newValue && !!e.oldValue) {
          // Signed out from another window/tab
          this.signOutAndChallengeUser();

        } else if (!e.oldValue && !!e.newValue) {
          // Signed in from another window/tab
          this.signInCallback(this.localStorageToken);
        }

        // Notify this window to refresh the expiry timer
        if (e.oldValue !== e.newValue) {
          this.localStorageTokenChange$.next(this.localStorageToken);
        }
      }
    }, false);

    // Refresh the token every 1 hour to keep the user session
    // alive. In case of disconnection, keep trying every 2 seconds
    // until it succeeds
    timer(0, this.REFRESH_DURATION).pipe(
      filter(() => this.isAuthenticated),
      switchMap(() => this.refreshToken()), // TODO: Make it such that only one window is refreshing
      retryWhen(attempts => attempts.pipe(mergeMap(() => timer(this.REFRESH_FAIL_DURATION)))),
      tap(tokenResponse => {
        if (!!tokenResponse) {
          this.localStorageToken = tokenResponse;
        }
      }),
    ).subscribe();
  }

  public createToken(email: string, password: string): Observable<AuthTokenResponse> {
    // Calls the create-token endpoint
    return this.http.post<AuthTokenResponse>(`api/auth/create-token`,
      { Email: email, Password: password }, {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' })
      }).pipe(
        tap((tokenResponse) => this.signInCallback(tokenResponse)),
        catchError((error) => {
          const friendlyError = friendly(error);
          return throwError(friendlyError);
        })
      );
  }

  private refreshToken(): Observable<AuthTokenResponse> {
    // Calls the refresh-token endpoint
    return this.http.get<AuthTokenResponse>(`api/auth/refresh-token`).pipe(
      catchError((error) => {
        const friendlyError = friendly(error);
        return throwError(friendlyError);
      }),
      takeUntil(this.signedOut$)
    );
  }

  public forgotPassword(email: string): Observable<boolean> {
    const url = `api/auth/forgot-password`;

    return this.http.post<AuthTokenResponse>(url, { Email: email }, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      map(() => true),
      catchError((error) => {
        const friendlyError = friendly(error);
        return throwError(friendlyError);
      }),
      takeUntil(this.signedOut$)
    );
  }

  public confirmEmail(userId: string, emailConfirmationToken: string): Observable<boolean> {
    const url = `api/auth/confirm-email`;

    const obs$ = this.http.post(url, { UserId: userId, EmailConfirmationToken: emailConfirmationToken }, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      map(() => true),
      catchError((error) => {
        const friendlyError = friendly(error);
        return throwError(friendlyError);
      }),
      takeUntil(this.signedOut$)
    );

    return obs$;
  }

  public resetPassword(userId: string, passwordResetToken: string,
    newPassword: string): Observable<AuthTokenResponse> {
    const url = `api/auth/reset-password`;

    const obs$ = this.http.post<AuthTokenResponse>(url,
      { UserId: userId, PasswordResetToken: passwordResetToken, NewPassword: newPassword },
      { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) }
    ).pipe(
      tap((tokenResponse) => this.signInCallback(tokenResponse)),
      catchError((error) => {
        const friendlyError = friendly(error);
        return throwError(friendlyError);
      }),
      takeUntil(this.signedOut$)
    );

    return obs$;
  }

  private signInCallback(tokenResponse: AuthTokenResponse) {
    // Store the freshly obtained ID token in localstorage if it has a later expiry date
    const currentToken = this.localStorageToken;
    if (!currentToken || currentToken.Expiration < tokenResponse.Expiration) {
      this.localStorageToken = tokenResponse;
    }

    // Navigate away from the sign-in page
    this.router.navigateByUrl(this.returnUrl || '');

    // This field has served its purpose and can be deleted
    this.returnUrl = null;
  }

  public signOutAndChallengeUser(returnUrl: string = null) {
    this.signOut();
    this.challengeUser(returnUrl);
  }

  public signOut() {
    // Clear the token from local storage
    if (!!this.localStorageToken) {
      this.localStorageToken = null;
    }

    // notify everyone to clear any data and cancel any requests
    this.signedOut$.next();
  }

  private challengeUser(returnUrl: string = null) {
    // Sends the user to the sign-in screen
    this.returnUrl = returnUrl;
    this.router.navigate(['sign-in']);
  }

  private get localStorageToken(): AuthTokenResponse {
    const stringToken = localStorage.getItem(this.LOCAL_STORAGE_KEY);
    if (!!stringToken) {
      return <AuthTokenResponse>JSON.parse(stringToken);
    } else {
      return null;
    }
  }

  private set localStorageToken(val: AuthTokenResponse) {
    // Set the new item
    if (!!val) {
      localStorage.setItem(this.LOCAL_STORAGE_KEY, JSON.stringify(val));
    } else {
      localStorage.removeItem(this.LOCAL_STORAGE_KEY);
    }

    // Notify listeners
    this.localStorageTokenChange$.next(val);
  }

  public get idToken(): string {
    const tokenResponse = this.localStorageToken;
    return !!tokenResponse ? tokenResponse.Token : null;
  }

  public get isAuthenticated() {
    const token = this.localStorageToken;
    return !!token && new Date(token.Expiration) > new Date();
  }
}
