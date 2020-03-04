import { Component, OnInit, Input } from '@angular/core';
import { User, UserService } from '../user/user.service';
import { TrainingService } from './training.service';
import { Observable } from 'rxjs';
import { Training } from './training';

@Component({
  selector: 'training-transcript',
  templateUrl: './training-transcript.component.html',
  styles: []
})
export class TrainingTranscriptComponent implements OnInit {

  @Input() user:User;
  @Input() year:number = 0;

  years:number[] = [];
  thisYear:number;
  totalHours = 0;
  loading = false;


  trainings:Training[];

  constructor(
    private service:TrainingService,
    private userService:UserService
  ) { 
    var now = new Date();
    this.thisYear = now.getFullYear();
    for( var year = 2005; year <= this.thisYear; year++){
      this.years.push(year);
    }
  }

  ngOnInit() {
    this.loading = true;
    if( this.user == null){
      this.userService.current().subscribe(
        res =>{
          this.user = res;
          this.loadData();
        } 
      );
    }else{
      this.loadData();
    }
    
  }
  loadData(){
    
      this.service.enrolledByUser(this.user.id,this.thisYear).subscribe(
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

  survey(training:Training){
    if(this.isAttended( training )){
      var surveyTaken = training.surveyResults.filter( e => e.userId == this.user.id);
      if(surveyTaken.length == 0) return true;
    }
    return false;
  }


  isAttended( training:Training ):boolean{
    var enrollment = training.enrollment.filter( e => e.attendieId == this.user.id);
    if( enrollment.length != 0){
      if( enrollment[0].attended == true ){
        return true;
      }
    }
    return false;
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
  onYearChange(year:number){
    this.thisYear = year;
    this.loading = true;
    this.trainings = null;
    this.loadData();
  }
}
