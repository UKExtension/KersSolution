import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Training } from './training';
import { TrainingService } from './training.service';
import { IMyDrpOptions, IMyDateRangeModel } from "mydaterangepicker";

@Component({
  selector: 'app-training-catalog',
  templateUrl: './training-catalog.component.html',
  styles: []
})
export class TrainingCatalogComponent implements OnInit {

  trainings:Observable<Training[]>;


  myDateRangePickerOptions: IMyDrpOptions = {
      // other options...
      dateFormat: 'mmm dd, yyyy',
      showClearBtn: false,
      showApplyBtn: false,
      showClearDateRangeBtn: false
  };
  model = {beginDate: {year: 2018, month: 10, day: 9},
                             endDate: {year: 2018, month: 10, day: 19}};

  constructor(
    private service:TrainingService
  ) { 
    //this.trainings = service.range();
  }

  ngOnInit() {
    var end = new Date();
    var start = new Date();
    start.setMonth(end.getMonth()-1)
    this.model.beginDate = {year: start.getFullYear(), month: start.getMonth(), day: start.getDate()};
    this.model.endDate = {year: end.getFullYear(), month: end.getMonth() + 1, day: end.getDate()};
  }

  dateCnanged(event: IMyDateRangeModel){
    //this.activities = this.service.perPeriod(event.beginJsDate, event.endJsDate);
    console.log(event);
    this.trainings = this.service.perPeriod(event.beginJsDate, event.endJsDate);
  }

}
