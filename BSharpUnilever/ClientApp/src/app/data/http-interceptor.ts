import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable()
export class HttpRequestInterceptor implements HttpInterceptor {

  constructor(private auth: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    // get the bearer token from the injected authentication service
    const idToken = this.auth.idToken;
    if (!!idToken) {
      // If there is a bearer token add it to the request headers
      req = req.clone({
        setHeaders: { Authorization: `Bearer ${idToken}` }
      });
    }

    return next.handle(req);
  }
}
