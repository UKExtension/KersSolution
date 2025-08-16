import { HttpBackend, HttpClient, HttpEvent, HttpHeaders } from '@angular/common/http';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import {Location} from '@angular/common';
import { KersUser } from '../../reporting/modules/admin/users/users.service';
import { HandleError, HttpErrorHandler } from '../../reporting/core/services/http-error-handler.service';
import { AuthenticationService } from '../authentication.service';
import { MessageService } from '../../reporting/core/services/message.service';
import { PersonalProfile, ReportingProfile, User } from '../../reporting/modules/user/user.service';

@Component({
  selector: 'login-jwt-get',
  template: `
  <div class="row" *ngIf="newUserExists" >
    <div class="row">
        <div class="col-sm-2"></div>
        <div class="col-md-10"><br><br>Please, edit information bellow for creating your KERS reporting profile.<br><br></div>
    </div>
    <div class="col-xs-11">
        <user-reporting-form [userObservable]="userObservable" (onFormCancel)="reportingFormCancel()" (onFormSubmit)="reportingFormSubmit($event)"></user-reporting-form>
    </div>
  </div>
  `,
  styles: [`
  body{
      background-color: #F7F7F7 !important;
  }
  
  

  `],
  encapsulation: ViewEncapsulation.None
})
export class LoginJwtGetComponent implements OnInit {
  token:string;
  newUser:SapUser;
  newUserExists:boolean = false;
  redirectURL:string;
  private handleError: HandleError;
  private httpClient: HttpClient;
  userObservable:Observable<User>;
  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private http: HttpClient,
    private handler: HttpBackend,
    private location:Location,
    private httpErrorHandler: HttpErrorHandler,
    private authService: AuthenticationService, 
    private messageService: MessageService
    
  ) {
    this.handleError = httpErrorHandler.createHandleError('Checktoken');
    this.httpClient = new HttpClient(handler);
   }

  ngOnInit(): void {
    if(this.authService.isLoggedIn()){
      this.router.navigate(['/reporting']);
    }
    this.route.queryParams.subscribe(
      params => {
        this.token =  <string> params["access_token"];
        this.newUser = <SapUser> JSON.parse(params["newUser"]);
        this.redirectURL = <string> params["relayState"];
        this.checkToken().subscribe(
          res => {
            if(this.newUser != null){
              this.checkNewUser().subscribe(
                res => {
                  if(res){
                    var reportng = new ReportingProfile(0, this.newUser.Userid,this.newUser.PersonId, this.newUser.Name, this.newUser.Userid + "@uky.edu","", true,1, null, undefined, null, 1,null);
                    var prsnl = new PersonalProfile(0,this.newUser.Fname,this.newUser.Lname, "", "", "", "", "", [], [],"", null);
                    var usr = new User( 0, 0, [], reportng, prsnl, undefined, null, null, null,null, null);
                    this.userObservable = of(usr);
                    this.newUserExists = true;
                  }else{
                    var errorMessage = "Not UK Extension employee status.";
                    this.messageService.add(errorMessage);
                    this.router.navigate(['/login2fa']);
                  }
                }
              )
            }else{
              this.authService.setAuth( { newUser: this.newUser, access_token: this.token });
              if(this.redirectURL != "" && this.redirectURL != null){
                if(this.redirectURL.includes('/core/reports')){
                  window.location.href = this.redirectURL;
                }else{
                  this.router.navigate([this.redirectURL]);
                }
              }else{
                this.router.navigate(['/reporting']);
              }
            }
          },
          err => {
            var errorMessage = "System errror occured in your login attempt.";
            this.messageService.add(errorMessage);
            this.router.navigate(['/login2fa']);
          }
        );
    });
  }

  checkToken():Observable<KersUser | HttpEvent<Observable<KersUser>>>{
    var opts:any = {};
    opts.headers = new HttpHeaders().set("Authorization", `Bearer ${this.token}`);
    var url = this.location.prepareExternalUrl('/api/user/checktoken');
    var data = this.newUser;
    return this.httpClient.post<Observable<KersUser>>(url, data, opts);
  }

  checkNewUser():Observable<boolean  | HttpEvent<Observable<boolean>>>{
    var opts:any = {};
    opts.headers = new HttpHeaders().set("Authorization", `Bearer ${this.token}`);
    var url = this.location.prepareExternalUrl('/api/user/checknewuser');
    var data = this.newUser;
    return this.httpClient.post<Observable<boolean>>(url, data, opts);
  }

  reportingFormCancel(){
    this.newUserExists= false;
    var errorMessage = "Canceled new user registration screen.";
    this.messageService.add(errorMessage);
    this.router.navigate(['/login2fa']);
  }
  reportingFormSubmit(user){
      delete this.newUser;
      this.authService.setAuth( { newUser: this.newUser, access_token: this.token });
      this.router.navigate(['/reporting/user/personal'])
  }



}
export class SapUser{
  public Userid;
  public rId;
  public PersonId;
  public Name;
  public Lname;
  public Fname;
}