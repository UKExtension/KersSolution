import { Component, Inject, OnInit, Output, EventEmitter, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../authentication.service';
import {FormBuilder, Validators }   from '@angular/forms';
import {Location} from '@angular/common';
import { MessageService } from '../../reporting/core/services/message.service';

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
  loginError = false;
  
  loading = false;

  @Output() onNewUser = new EventEmitter<object>();

  constructor(
                public authService: AuthenticationService, 
                public router: Router,
                public messageService: MessageService,
                private fb: FormBuilder,
                private location: Location
              ) 
  {
    this.ukLogoSrc = location.prepareExternalUrl('/assets/images/UK_gray.svg');
  }

  ngOnInit(){
    	
    if(this.authService.isLoggedIn()){
      this.router.navigate(['/reporting']);
    }
  }
  

}