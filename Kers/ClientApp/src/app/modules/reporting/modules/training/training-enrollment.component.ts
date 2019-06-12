import { Component, OnInit } from '@angular/core';
import { User } from '../user/user.service';
import { Observable } from 'rxjs';
import { TrainingService } from './training.service';
import { Training } from './training';

@Component({
  selector: 'training-enrollment',
  templateUrl: './training-enrollment.component.html',
  styles: []
})
export class TrainingEnrollmentComponent implements OnInit {

  users:Observable<User[]>;
  trainings:Training[];
  years:number[] = [];
  thisYear:number;
  loading = false;

  totalHours = 0;

  selectedUserId:number;

  constructor(
    private service:TrainingService
  ) {

    var now = new Date();
    this.thisYear = now.getFullYear();
    for( var year = 2017; year <= this.thisYear; year++){
      this.years.push(year);
    }
    this.loadUsers();
   }

  ngOnInit() {
  }

  loadUsers(){
    this.users = this.service.usersWithTrainings(this.thisYear);
  }


  onYearChange(year:number){
    this.thisYear = year;
    this.trainings = null;
    this.loadUsers();
  }

  onUserChange(userId:number){
    this.trainings = null;
    this.loading = true;
    this.selectedUserId = userId;
    this.service.enrolledByUser(userId, this.thisYear).subscribe(
      res =>{
        this.trainings = res;
        this.totalHours = this.getTotalHours(this.trainings);
        this.loading = false;
      } 
    );
  }
  attendance(training:Training){

    var attended = "NO"
    if( this.isAttended( training )){
        attended = "YES"; 
    }
    return attended;
  }
  getTotalHours(trainings:Training[]):number{
    var totalHours = 0;
    for( let training of trainings){
      if( this.isAttended( training) && training.iHour ){
        totalHours += training.iHour.iHourValue;
      }
    }
    return totalHours;
  }
  isAttended( training:Training ):boolean{
    var enrollment = training.enrollment.filter( e => e.attendieId == this.selectedUserId);
    if( enrollment.length != 0){
      if( enrollment[0].attended == true ){
        return true;
      }
    }
    return false;
  }

}
