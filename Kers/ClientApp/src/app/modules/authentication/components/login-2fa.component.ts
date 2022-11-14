import { Component, Inject, OnInit, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../authentication.service';
import {FormBuilder, Validators }   from '@angular/forms';
import {Location} from '@angular/common';

@Component({
  selector: 'login',
  templateUrl: 'login-2fa.component.html'
})
export class Login2faComponent implements OnInit { 

  message: string = "Username or Password Missmatch";
  public ukLogoSrc: string;
  loginError = false;
  loginForm = null;

  loading = false;

  @Output() onNewUser = new EventEmitter<object>();

  constructor(
                public authService: AuthenticationService, 
                public router: Router,
                private fb: FormBuilder,
                private location: Location
              ) 
  {
    this.ukLogoSrc = location.prepareExternalUrl('/assets/images/UK_gray.svg');
    //this.setMessage();
    this.loginForm = fb.group(
            {
              name: ['', Validators.required], 
              password: ['', Validators.required]
            })
    
  }

  ngOnInit(){
    	
    if(this.authService.isLoggedIn()){
      this.router.navigate(['/reporting']);
    }
  }

  private getCookie(name: string) {
        let ca: Array<string> = document.cookie.split(';');
        let caLen: number = ca.length;
        let cookieName = name + "=";
        let c: string;

        

        for (let i: number = 0; i < caLen; i += 1) {
            c = ca[i].replace(/^\s\+/g, "");
            if (c.indexOf(cookieName) == 0) {
                return c.substring(cookieName.length, c.length);
            }
        }
        return "";
    }

  performLogin(e){
   
    e.preventDefault();
    var username = this.loginForm.value.name;
    var password = this.loginForm.value.password;
    this.loading = true;
    this.authService.login(username, password) 
      .subscribe( 
        (data) => {
            this.loading = false;
            if(data["error"] != null){
              this.loginError = true;
              this.message = data["error"];
            }else{
              this.loginError = false;
              if(data["newUser"] != null){
                this.onNewUser.emit(data);
              }else{
                var auth = this.authService.getAuth();
                if(this.authService.redirectUrl == undefined){
                  this.router.navigate(['/reporting']);
                }else{
                  this.router.navigate([this.authService.redirectUrl]);
                }
              }
            }
            
          },
            _ => {
              this.loading = false;
              this.loginError = true;
            }
      );
     
  }
  
  

}