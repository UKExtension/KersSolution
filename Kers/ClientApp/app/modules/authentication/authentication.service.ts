import { Injectable, EventEmitter } from '@angular/core';
import {Location} from '@angular/common';
import {Http, Headers, Response, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import { JwtHelper } from './jwt.helper';


@Injectable()
export class AuthenticationService {
  //isLoggedIn: boolean = false;
  authKey = "auth";
  // store the URL so we can redirect after logging in
  redirectUrl: string = '/reporting';


  constructor(private http:Http, private location:Location){}


  login(username: string, password: string):any{
    var url = "/api/token";
    var data = {
      username: username,
      password: password, 
      client_id: "KersReporting", 
      grant_type: "password",
      scope: "offline_access profile email"
    }
    return this.http.post(
        this.location.prepareExternalUrl(url),
        JSON.stringify(data),
        new RequestOptions({ headers: new Headers(
                {
                  "Content-Type": "application/json"
                }
              )
            }
          )
    ).map( response => {
          var auth = response.json();
          if(auth.newUser == null){
            this.setAuth(auth);
          }
          return auth;
      }
    );
   
}

private extractData(res: Response) {
    let body = res.json();
    return body.data || { };
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
