import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { JwtHelper } from './jwt.helper';
import { User } from '../reporting/modules/user/user.service';
import { HttpErrorHandler, HandleError } from '../reporting/core/services/http-error-handler.service';
import { catchError, tap } from 'rxjs/operators';



@Injectable()
export class AuthenticationService {
  //isLoggedIn: boolean = false;
  authKey = "auth";
  // store the URL so we can redirect after logging in
  redirectUrl: string = '/reporting';
  private handleError: HandleError;

  constructor( 
      private http: HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
      ) {
          this.handleError = httpErrorHandler.createHandleError('HelpService');
      }


  login(username: string, password: string):any{
    var url = "/api/token";
    var data = {
      username: username,
      password: password, 
      client_id: "KersReporting", 
      grant_type: "password",
      scope: "offline_access profile email"
    }
    return this.http.post( this.location.prepareExternalUrl(url), data )
      .pipe(
          tap( auth => {
                  if(auth["newUser"] == null && auth["error"] == null){
                    this.setAuth(auth);
                  }
                }
              ),
          catchError(this.handleError('GetAuthToken'))
      );
    
    
    
}

private extractData(res: Response) {
    let body = res.json();
    return body["data"] || { };
}

logout():boolean{
  this.setAuth(null);
  return false;
}

toUrlEncodedString(data:any){
  var body = "";
  for(var key in data){
    if( body.length ){
      body += '&';
    }
    body += key + "=";
    body += encodeURIComponent(data[key]);
  }
  return body;
}

setAuth(auth: any):boolean{
  if(auth){
    localStorage.setItem(this.authKey, JSON.stringify(auth));
  }else{
    localStorage.removeItem(this.authKey);
  }
  return true;
}

getAuth():any{
  var i = localStorage.getItem(this.authKey);
  if(i){
    return JSON.parse(i);
  }else{
    return null;
  }
}

isLoggedIn():boolean{
  if (typeof window !== 'undefined') {
    var token = localStorage.getItem(this.authKey);
    var helper = new JwtHelper;
    if(token != null){
      if(helper.isTokenExpired(token)){
        localStorage.removeItem(this.authKey);
        return false;
      }
      return true;
    }
  }
  return false;
}
/*

  login(): Observable<boolean> {
    return Observable.of(true).delay(1000).do(val => this.isLoggedIn = true);
  }

  logout(): void {
    this.isLoggedIn = false;
  }
  */
}
