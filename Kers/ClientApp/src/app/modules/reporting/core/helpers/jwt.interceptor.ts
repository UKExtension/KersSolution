import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';


@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    authKey = "auth";
    constructor() { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // add authorization header with jwt token if available

        if (typeof window !== 'undefined') {
            var i = localStorage.getItem(this.authKey);
            if( i != null ){
                var auth = JSON.parse(i);

                request = request.clone({
                    setHeaders: {
                        Authorization: `Bearer ${auth.access_token}`,
                        'Content-Type': 'application/json; charset=utf-8',
                        'Cache-Control': 'no-cache',
                        'Pragma': 'no-cache',
                        'Expires': 'Sat, 01 Jan 2000 00:00:00 GMT'
                    }
                });

                
            }
        }

        return next.handle(request);
    }
}