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
  coppied = false;
  waitingList:TrainingEnrollment[];
  openWaitingList = false;

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

  canSeeWatingList():Observable<boolean>{
    return this.userService.currentUserHasAnyOfTheRoles(["CESADM"]);
  }

  cancelCutOfDate(training:Training):Date{
    var start = new Date(training.start);
    if( training.cancelCutoffDays == null) return start;
    start.setDate( start.getDate() - training.cancelCutoffDays.cancelDaysVal );
    return start;
  }

  isEnrolled(training:Training):boolean{
    if( this.user != undefined ){
      var isUserThere = training.enrollment.filter( e => e.attendieId == this.user.id && e.eStatus=="E");
      if( isUserThere.length > 0 ) return true;
    }
    return false;
  }
  isWaiting(training:Training):boolean{
    if( this.user != undefined ){
      var isUserThere = training.enrollment.filter( e => e.attendieId == this.user.id && e.eStatus=="W");
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
  public notify(payload: string) {
    this.coppied = true;
  }
  getOnlyEnrolled(training:Training):TrainingEnrollment[]{
    return training.enrollment.filter( e => e.eStatus == "E");
  }
  numberWaiting(training:Training):number{
    this.waitingList = training.enrollment.filter( e => e.eStatus == "W");
    return this.waitingList.length;
  }

  isItInsideTheCancellationWindow(training:Training):boolean{
    if(training.cancelCutoffDays){
      var cutof = new Date(training.start);
      cutof.setHours(3, 0, 0);
      cutof.setDate(cutof.getDate() - training.cancelCutoffDays.cancelDaysVal + 1);
      var today = new Date();
      today.setHours(3, 0, 0);

      if( cutof < today ) return true;
    }
    return false;
  }
  isItInsideRegistrationWindow(training:Training):boolean{
    if(training.registerCutoffDays){
      var cutof = this.registerCutOfDate(training);
      var today = new Date();
      today.setHours(2, 0, 0);
      if( cutof < today ) return false;
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
    var enrolled = training.enrollment.filter( e => e.eStatus == "E");
    var emails = enrolled.map( a => a.attendie.rprtngProfile.email).join("; ");
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
        if( !this.isWaiting(training)){
          res.attendie = this.user;
          training.enrollment.push( res )
        }else{
          var enrollment = training.enrollment.filter( e => e.attendieId == this.user.id && e.eStatus=="W");
          enrollment[0].eStatus = "E";
        }
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
