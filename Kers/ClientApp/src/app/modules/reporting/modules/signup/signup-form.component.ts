import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, Validators} from "@angular/forms";
import { Observable } from 'rxjs';
import { Activity, Ethnicity } from '../activity/activity.service';
import { Race, ServicelogService } from '../servicelog/servicelog.service';
import { ActivitySignUpEntry, SignupService } from './signup.service';

@Component({
  selector: 'signup-form',
  template: `
    
<br><br>
  <div *ngIf="confirmMessage" class="red text-center" style="width:100%;font-weight:bold;">
   <br><br><br>
   Your information is recorded. <br>
   Thanks for your submission.
   <br><br>
  </div>
  <loading *ngIf="loading && !confirmMessage"></loading>
  <div class="row" *ngIf="!loading && !confirmMessage">
      <form class="form-horizontal form-label-left" novalidate (ngSubmit)="onSubmit()" [formGroup]="signupForm">
          <div class="form-group">
              <label for="name" class="control-label col-md-2 col-sm-2 col-xs-12">Name:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" formControlName="name" class="form-control col-xs-12" />
              </div>
          </div>
          <div class="form-group">
              <label class="control-label col-md-2 col-sm-2 col-xs-12" for="address">Address: </label>
              <div class="col-md-9 col-sm-9 col-xs-12">
                <input type="text" name="address" formControlName="address" id="address" class="form-control col-xs-12" />
              </div>
          </div>
          <div class="form-group">
              <label class="control-label col-md-2 col-sm-2 col-xs-12" for="email">Email: </label>
              <div class="col-md-9 col-sm-9 col-xs-12">
                <input type="text" name="email" formControlName="email" id="email" class="form-control col-xs-12" />
              </div>
          </div>
          <div class="form-group">
              <label class="control-label col-md-2 col-sm-2 col-xs-12" for="raceId">Race: </label>
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <select name="race" id="raceId" formControlName="raceId" class="form-control col-md-7 col-xs-12" >	
                      <option value="">--- select ---</option>
                      <option *ngFor="let race of races | async"  [value]="race.id">{{race.name}}</option>
                  </select>
              </div>
          </div>
          <div class="form-group">
              <label class="control-label col-md-2 col-sm-2 col-xs-12" for="ethnicityId">Ethnicity: </label>
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <select name="race" id="ethnicityId" formControlName="ethnicityId" class="form-control col-md-7 col-xs-12" >	
                      <option value="">--- select ---</option>
                      <option *ngFor="let ethnicity of ethnicities | async"  [value]="ethnicity.id">{{ethnicity.name}}</option>
                  </select>
              </div>
          </div>
          <div class="form-group">
              <label class="control-label col-md-2 col-sm-2 col-xs-12" for="gender">Gender: </label>
              <div class="col-md-9 col-sm-9 col-xs-12">
                <label class="radio-inline">
                  <input type="radio" name="gender" id="inlineRadio1" value="1" formControlName="gender"> Male
                </label>
                <label class="radio-inline">
                  <input type="radio" name="gender" id="inlineRadio2" value="2" formControlName="gender"> Female
                </label>
                <label class="radio-inline">
                  <input type="radio" name="gender" id="inlineRadio3" value="0" formControlName="gender"> Choose not to Identify
                </label>
              </div>
          </div>
          <div class="ln_solid"></div>
          <div class="form-group">
              <div class="col-md-6 col-sm-6 col-xs-12 col-sm-offset-2">
                  <button type="submit" [disabled]="signupForm.invalid"  class="btn btn-success">Submit</button>
              </div>
          </div>
          
      </form>
  </div>
  
  `,
  styles: [
  ]
})
export class SignupFormComponent implements OnInit {

  @Input() activity:Activity;
  @Input() dalayConfirm:boolean = true;
  @Output() Submit = new EventEmitter<ActivitySignUpEntry>();
  races:Observable<Race[]>;
  ethnicities: Observable<Ethnicity[]>;
  loading = false;
  confirmMessage = false;
  signupForm:any;

  constructor(
    private fb: FormBuilder,
    private service:ServicelogService,
    private signupservice: SignupService
  ) { 
    this.signupForm = this.fb.group(
      {
          name: ["", Validators.required],
          address: [""],
          email: ["", Validators.email],
          raceId: [""],
          gender: [""],
          ethnicityId: [""]
      }
  );

  }

  ngOnInit(): void {
    this.races = this.service.races();
    this.ethnicities = this.service.ethnicities();
  }

  onSubmit(){
    var val = <ActivitySignUpEntry>this.signupForm.value;
    val.activityId = this.activity.id;
    this.loading = true;
    this.signupservice.add( val ).subscribe(
      res => {
        this.signupForm.reset();
        this.Submit.emit( res );
        var thisClass = this;
        this.confirmMessage = true;
        if(this.dalayConfirm){
          setTimeout(() => {
            this.confirmMessage = false;
            this.loading=false;
          }, 2000);
        }else{
          this.loading=false;
        }
        
        
        
        
      }
    )
    
  }

}
