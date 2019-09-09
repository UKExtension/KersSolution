import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { Observable } from 'rxjs';
import { Training, TrainingEnrollment, TrainingSearchCriteria } from './training';
import { TrainingService } from './training.service';
import { switchMap } from 'rxjs/operators';
import { UserService, User, UserSpecialty } from '../user/user.service';

@Component({
  selector: 'training-info',
  templateUrl: './training-info.component.html',
  styleUrls: ['./training-info.component.css']
})
export class TrainingInfoComponent implements OnInit {
  
  training$: Observable<Training>;
  user:User;
  loading = false;
  criteria:TrainingSearchCriteria;

  constructor(
    private route: ActivatedRoute,
    private service: TrainingService,
    private userService: UserService
  ) {
    
   }

  ngOnInit() {
    this.training$ = this.route.paramMap.pipe(
      switchMap((params: ParamMap) =>{
          this.criteria = <TrainingSearchCriteria>{
            start: <string> params.get("start"),
            end: <string> params.get("end"),
            search: <string> params.get("search"),
            contacts: <string> params.get("contacts"),
            day: params.get("day") == "null" ? null : +params.get("day"),
            withseats: <boolean> <unknown>params.get("withseats"),
            order: <string> params.get("order")
          }
          return this.service.getTraining(+params.get('id'));
        }
      )
    );
    this.userService.current().subscribe(
      res => {
        this.user = res;
      }
    )
    
    
  }

  registerCutOfDate(training:Training):Date{
    var start = new Date(training.start);
    start.setHours(3, 0, 0);
    if( training.registerCutoffDays == null) return start;
    start.setDate( start.getDate() - +training.registerCutoffDays.registerDaysVal );
    return start;
  }

  cancelCutOfDate(training:Training):Date{
    var start = new Date(training.start);
    if( training.cancelCutoffDays == null) return start;
    start.setDate( start.getDate() - training.cancelCutoffDays.cancelDaysVal );
    return start;
  }

  isEnrolled(training:Training):boolean{
    if( this.user != undefined ){
      var isUserThere = training.enrollment.filter( e => e.attendieId == this.user.id);
      if( isUserThere.length > 0 ) return true;
    }
    
    return false;
  }

  isFull(training:Training):boolean{
    if(training.seatLimit){
      if(training.enrollment.length >= training.seatLimit) return true;
    }
    return false;
  }

  isItInsideTheCancellationWindow(training:Training):boolean{
    if(training.cancelCutoffDays){
      var cutof = new Date(training.start);
      cutof.setHours(3, 0, 0);
      cutof.setDate(cutof.getDate() - training.cancelCutoffDays.cancelDaysVal);
      var today = new Date();
      today.setHours(3, 0, 0)
      if( cutof <= today ) return true;
    }
    return false;
  }
  isItInsideRegistrationWindow(training:Training):boolean{
    if(training.registerCutoffDays){
      var cutof = this.registerCutOfDate(training);
      var today = new Date();
      today.setHours(3, 0, 0);
      if( cutof <= today ) return false;
    }
    return true;
  }
  isItPast(training:Training):boolean{
    var start = new Date(training.start);
    start.setHours(3, 0, 0);
    var today = new Date();
    today.setHours(3, 0, 0)
    if( start <= today ) return true;
    return false;
  }


  attendieemails(training:Training):string{
    var emails = training.enrollment.map( a => a.attendie.rprtngProfile.email).join(";");
    return emails;
  }

  specialty(area:UserSpecialty):string{
    var nm = area.specialty.code;
    if( nm.substr(0, 4) == "prog") nm = nm.substr( 4, nm.length);
    return nm;
  }

  enroll(training:Training){
    this.loading = true;
    this.service.enroll(training).subscribe(
      res => {
        training.enrollment.push( <TrainingEnrollment>{enrolledDate: new Date(), attendieId:this.user.id, attendie: this.user})
        this.loading = false;
      }
    )
  }
  unenroll(training:Training){
    this.loading = true;
    this.service.unenroll(training).subscribe(
      _ => {
        var isUserThere = training.enrollment.filter( e => e.attendieId == this.user.id);
        if( isUserThere.length > 0 ){
          let index: number = training.enrollment.indexOf(isUserThere[0]);
          if (index !== -1) {
            training.enrollment.splice(index, 1);
          }
        }
        this.loading = false;
      }
    )
  }

}
/* 

Handle adding to the waiting list

if (HiddenField6.Value == "1")  // Training is full.
            {
                if (isOnWaitingList(HiddenField0.Value))
                {
                    Label1.Text = "*** TRAINING IS FULL - YOU ARE ON THE WAITING LIST *** <br /><br />In the event of a cancellation, those on the waiting list will be automatically enrolled, then sent a notification via email.";
                    Label1.Visible = true;
                    Button1.Text = "";
                    Button1.Visible = false;
                }
                else
                {
                    if (HiddenField4.Value == "Y")  // Outside of cancellation window. Allow user the option to be added to the waiting list.
                    {
                        Label1.Text = "*** TRAINING IS FULL ***<br /><br />If you would like to be added to the waiting list for this training, please click the submit button below.&nbsp;&nbsp;In the event of a cancellation, those on the waiting list will be automatically enrolled, then sent a notification via email.";
                        Label1.Visible = true;
                        Button1.Text = "Add me to the waiting list";
                        Button1.Visible = true;
                    }
                    else  // Inside the cancellation window. Do not allow user the option to be added to the waiting list.
                    {
                        Label1.Text = "*** TRAINING IS FULL ***<br /><br />";
                        Label1.Visible = true;
                        Button1.Text = "";
                        Button1.Visible = false;
                    }
                }
            }
            else  // Training is not full or no enrollment limit.
            {
                if (HiddenField5.Value == "upcoming")
                {
                    Button1.Text = "Enroll for this training event";
                    Button1.Visible = true;
                    Label1.Text = "Clicking the Enroll button below will immediately enroll you in this training event.";
                    Label1.Visible = true;
                }

                if (HiddenField5.Value == "past")
                {
                    Button1.Text = "Enroll for this PAST training event";
                    Button1.Visible = true;
                    Label1.Text = "If you attended this training session but did not previously enroll, click the Enroll button below and you will be enrolled.&nbsp;&nbsp;You must contact the instructor and request they return to the roster to confirm your attendance.&nbsp;&nbsp;Clicking the Enroll button below will immediately enroll you in this training event.";
                    Label1.Visible = true;
                }
            }

   */