import { Component, ViewEncapsulation } from '@angular/core';
import { UserService, User, ReportingProfile, PersonalProfile } from '../../reporting/modules/user/user.service';
import { Observable, of } from "rxjs";
import { Router } from '@angular/router';
import { AuthenticationService } from '../authentication.service';


@Component({
  templateUrl: 'login-home.component.html',
  styles: [`
  body{
      background-color: #F7F7F7 !important;
  }
  
  

  ` ],
  encapsulation: ViewEncapsulation.None
})
export class LoginHomeComponent { 
    
    
    public newUser:boolean = false;

    private auth;

    public userObservable:Observable<User>;

    constructor( 
        public authService: AuthenticationService, 
        private router: Router
    )   
    {
        
    }

    ngOnInit(){
        
        
    }

    onNewUser(data){
        this.auth = data;
        this.newUser= true;
        var reportng = new ReportingProfile(0, data.newUser.userid,data.newUser.personID, data.newUser.name, data.newUser.userid + "@uky.edu","", true,1, null, undefined, null, 1,null);
        var prsnl = new PersonalProfile(0,data.newUser.fname,data.newUser.lname, "", "", "", "", "", [], [],"", null);
        var user = new User( 0, 0, [], reportng, prsnl, undefined, null, null, null,null, null);
        this.userObservable = of(user);
    }
    reportingFormCancel(){
        this.newUser= false;
    }
    reportingFormSubmit(user){
        delete this.auth.newUser;
        this.authService.setAuth(this.auth);
        this.router.navigate(['/reporting/user/personal'])
    }
}