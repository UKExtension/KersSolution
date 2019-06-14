import { Component, OnInit } from '@angular/core';
import { Training } from './training';
import { TrainingService } from './training.service';

@Component({
  selector: 'training-admin-reports',
  templateUrl: './training-admin-reports.component.html',
  styles: []
})
export class TrainingAdminReportsComponent implements OnInit {

  trainings:Training[];
  years:number[] = [];
  thisYear:number;
  loading = false;

  status='act';

  totalHours = 0;
  totalEnrolled = 0;
  totalAttended = 0;

  constructor(
    private service:TrainingService
  ) { 
    var now = new Date();
    this.thisYear = now.getFullYear();
    for( var year = 2017; year <= this.thisYear; year++){
      this.years.push(year);
    }
  }

  ngOnInit() {
    this.loadTrainings();
  }

  loadTrainings(){
    this.loading = true;
    var stts = "A";
    if(this.status == "pnd"){
      stts = "P";
    }else if(this.status == "cnc"){
      stts = "C";
    }
    this.service.trainingsbystatus(this.thisYear, stts).subscribe(
      res => {
        this.trainings = res;
        this.loading = false;
        this.calculate();
      }
    );
  }
  attendance( training:Training):number{
    var attended = 0;
    var filtered = training.enrollment.filter( e => e.attended );
    if( filtered != undefined) attended = filtered.length;
    return attended;
  }

  calculate(){
    this.calculateTotalHours();
    this.calculateTotalEnrolled();
    this.calculateTotalAttended();
  }
  calculateTotalHours(){
    this.totalHours = this.trainings.reduce( function(a, b){ 
      return a + (b.iHour != undefined ? b.iHour.iHourValue : 0); 
    }, 0);
  }
  calculateTotalEnrolled(){
    this.totalEnrolled = this.trainings.reduce( function(a, b){ return a + b.enrollment.length; }, 0);
  }
  calculateTotalAttended(){
    
    this.totalAttended = this.trainings.reduce( 
                function(a, b){ 
                  var attended = b.enrollment.filter( e => e.attended);
                  return a + attended.length; 
                }, 0);
  }

  onYearChange(year:number){
    this.thisYear = year;
    this.loadTrainings();
  }

  switchStatus(status:string){
    this.status = status;
    this.loadTrainings();
  }

}
