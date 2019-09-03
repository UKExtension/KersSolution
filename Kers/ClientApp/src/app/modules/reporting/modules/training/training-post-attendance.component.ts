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
