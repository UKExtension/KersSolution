import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { Training } from './training';
import { TrainingService } from './training.service';
import { User } from '../user/user.service';

@Component({
  selector: 'training-post-attendance',
  templateUrl: './training-post-attendance.component.html',
  styles: []
})
export class TrainingPostAttendanceComponent implements OnInit {

  trainings:Training[];
  @Input() user:User;

  years:number[] = [];
  thisYear:number = 0;
  loading = false;

  start:Date = new Date();
  end:Date = new Date();

  constructor(
    private service:TrainingService
  ) { 
/* 
    var now = new Date();
    this.thisYear = now.getFullYear();
    for( var year = 2011; year <= this.thisYear; year++){
      this.years.push(year);
    }
 */
    this.start.setMonth( this.start.getMonth() -3);

  }

  ngOnInit() {
    //this.loadData();
  }

  loadData(){
    this.loading = true;
    this.service.proposedByUser(0, this.thisYear).subscribe(
        res => {
          this.trainings = res;
          this. loading = false;
        }
      );
  }

  onYearChange(year:number){
    this.thisYear = year;
    this.loadData();
  }

}

/* 

<div class="row">
  <div class="col-sm-6 col-xs-12" class="form-inline" style="margin-top: 3px;">
    
    <div class="form-group col-sm-6 col-xs-12" style="margin-bottom: 8px;">
      <label for="exampleInputName2">Select Calendar Year: &nbsp;</label>
      <select class="form-control" (change)="onYearChange($event.target.value)">
        <option *ngFor="let year of years" [value]="year" [selected]="year == thisYear">{{year}}</option>
      </select>
    </div>
    
    
      
  </div>
  <div class="col-sm-6 col-xs-12 text-right" style="margin-top: 3px;">
  </div>

</div>



<div class="table-responsive">
  <loading *ngIf="loading"></loading>
  <table  class="table table-bordered table-striped" *ngIf="!loading">
    <thead>
        <tr>
            <th>Training Date(s)</th>
            <th>Hours</th>
            <th>Title</th>
            <th>&nbsp;</th>
        </tr>
    </thead>
    <tr *ngFor="let training of trainings" [training-post-attendance-detail]="training"></tr>
  </table>
</div> */