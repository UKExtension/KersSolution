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

  trainings:Observable<Training[]>;
  @Input() user:User;

  years:number[] = [];
  thisYear:number = 0;

  constructor(
    private service:TrainingService
  ) { 
    var now = new Date();
    this.thisYear = now.getFullYear();
    for( var year = 2011; year <= this.thisYear; year++){
      this.years.push(year);
    }
  }

  ngOnInit() {
    this.loadData();
  }

  loadData(){
    this.trainings = this.service.proposedByUser(0, this.thisYear);
  }

  onYearChange(year:number){
    this.thisYear = year;
    this.trainings = null;
    this.loadData();
  }

}
