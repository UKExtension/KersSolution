import { Component, Inject, OnInit, Output, EventEmitter, ViewEncapsulation } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '../authentication.service';
import {FormBuilder, Validators }   from '@angular/forms';
import {Location} from '@angular/common';
import { MessageService } from '../../reporting/core/services/message.service';
import { environment } from '../../../../environments/environment';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'login2fa',
  templateUrl: 'login-2fa.component.html',
  styles: [`
  body{
      background-color: #F7F7F7 !important;
  }
  
  

  ` ],
  encapsulation: ViewEncapsulation.None
})
export class Login2faComponent implements OnInit { 

  public ukLogoSrc: string;
  message: string = "Username or Password Missmatch";
  loginError = false;
  loginUrl:string;
  loading = false;
  isProduction:boolean;
  loginForm = null;

  @Output() onNewUser = new EventEmitter<object>();

  constructor(
                public authService: AuthenticationService, 
                public router: Router,
                private route: ActivatedRoute,
                public messageService: MessageService,
                private fb: FormBuilder,
                private location: Location
              ) 
  {
    this.ukLogoSrc = location.prepareExternalUrl('/assets/images/UK_gray.svg');
    this.isProduction = false;
    //this.isProduction = environment.production;
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
    var loginPath = '/loginsso';
    if(this.authService.redirectUrl != null){
      let httpParams = new HttpParams().set('rurl', this.authService.redirectUrl);
      loginPath += "?" + httpParams.toString();
    }
    this.loginUrl = this.location.prepareExternalUrl(loginPath);
    console.log(window.location.href );
    this.route.queryParams
      .subscribe(params => {
        if(params.errormessage != undefined){
          
          this.messageService.add(params.errormessage);
        }
      }
    );


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