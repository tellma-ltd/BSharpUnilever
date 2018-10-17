import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable()
export class HttpRequestInterceptor implements HttpInterceptor {

  constructor(private auth: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // This handy feature provided by Angular allows us to intercept and
    // modify all HTTP calls issued with the HttpClient, here we use it
    // to get the bearer token from the injected authentication service
    // and add it as a header as per the standard specification, this allows
    // the API to authenticate the request
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
