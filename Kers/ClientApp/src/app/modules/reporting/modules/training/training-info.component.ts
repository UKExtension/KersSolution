import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { Observable } from 'rxjs';
import { Training, TrainingEnrollment } from './training';
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

  constructor(
    private route: ActivatedRoute,
    private service: TrainingService,
    private userService: UserService
  ) {
    
   }

  ngOnInit() {
    this.training$ = this.route.paramMap.pipe(
      switchMap((params: ParamMap) =>
        this.service.getTraining(+params.get('id')))
    );
    this.userService.current().subscribe(
      res => {
        this.user = res;
      }
    )
    
    
  }

  registerCutOfDate(training:Training):Date{
    var start = new Date(training.start);
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
  attendieemails(training:Training):string{
    var emails = training.enrollment.map( a => a.attendie.rprtngProfile.email).join(";");
/* 
    for( let attendie of training.attendees){
      emails += attendie.rprtngProfile.email + ";"
    }
     */
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
