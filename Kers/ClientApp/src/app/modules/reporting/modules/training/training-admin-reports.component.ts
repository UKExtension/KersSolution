import { Component, OnInit } from '@angular/core';
import { Training } from './training';

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

  constructor() { 
    var now = new Date();
    this.thisYear = now.getFullYear();
    for( var year = 2017; year <= this.thisYear; year++){
      this.years.push(year);
    }
  }

  ngOnInit() {
  }

  switchStatus(status:string){
    this.status = status;
  }

}
