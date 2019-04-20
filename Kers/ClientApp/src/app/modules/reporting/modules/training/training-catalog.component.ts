import { Component, OnInit, Input } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { Training } from './training';
import { TrainingService } from './training.service';
import { IMyDrpOptions, IMyDateRangeModel } from "mydaterangepicker";
import { startWith, flatMap, delay, map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-training-catalog',
  templateUrl: './training-catalog.component.html',
  styles: []
})
export class TrainingCatalogComponent implements OnInit {
  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  trainings$:Observable<Training[]>;


  @Input() admin:boolean = false;
  @Input() startDate:Date;
  @Input() endDate:Date;


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
    if( this.startDate == null){
      this.startDate = new Date();
    }
    if( this.endDate == null ){
      this.endDate = new Date();
      this.endDate.setMonth( this.endDate.getMonth() + 5);
    }
    this.model.beginDate = {year: this.startDate.getFullYear(), month: this.startDate.getMonth() + 1, day: this.startDate.getDate()};
    this.model.endDate = {year: this.endDate.getFullYear(), month: this.endDate.getMonth() + 1, day: this.endDate.getDate()};
    
    this.refresh = new Subject();

    this.trainings$ = this.refresh.asObservable()
      .pipe(
        startWith('onInit'), // Emit value to force load on page load; actual value does not matter
        flatMap(_ => this.service.perPeriod(this.startDate, this.endDate)), // Get some items
        //delay(1000), // Delay to let our spinner shine
        //map(data => data.results), // Map data
        tap(_ => this.loading = false) // Turn off the spinner
      );
    
    
    
    //this.trainings$ = this.service.perPeriod(this.startDate, this.endDate);
  }

  dateCnanged(event: IMyDateRangeModel){
    this.startDate = event.beginJsDate;
    this.endDate = event.endJsDate;
    this.onRefresh();
    //this.trainings$ = this.service.perPeriod(event.beginJsDate, event.endJsDate);
  }

  onRefresh() {
    this.loading = true; // Turn on the spinner.
    this.refresh.next('onRefresh'); // Emit value to force reload; actual value does not matter
  }

}
