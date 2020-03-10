import { Component, OnInit, Input } from '@angular/core';
import { SnapedSearchCriteria, SnapedAdminService, SnapSearchResult, SnapSeearchResultsWithCount } from './snaped-admin.service';
import { IMyDrpOptions, IMyDateRangeModel } from 'mydaterangepicker';
import { Observable, Subject } from 'rxjs';
import { startWith, flatMap, tap } from 'rxjs/operators';

@Component({
  selector: 'snaped-reports',
  templateUrl: './snaped-reports.component.html',
  styles: []
})
export class SnapedReportsComponent implements OnInit {
  condition = false;

  @Input() criteria:SnapedSearchCriteria;
  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  revisions$:Observable<SnapSeearchResultsWithCount>;
  type="direct";
  order = "dsc";

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
    private service:SnapedAdminService
  ) { }

  ngOnInit() {

    var startDate = new Date();
    startDate.setMonth( startDate.getMonth() - 1);
    var endDate = new Date();

    this.model.beginDate = {year: startDate.getFullYear(), month: startDate.getMonth() + 1, day: startDate.getDate()};
    this.model.endDate = {year: endDate.getFullYear(), month: endDate.getMonth() + 1, day: endDate.getDate()};
    

    this.criteria = {
      start: startDate.toISOString(),
      end: endDate.toISOString(),
      search: "",
      order: 'dsc',
      type: this.type,
      skip: 0
    }


    this.refresh = new Subject();
    
    this.revisions$ = this.refresh.asObservable()
          .pipe(
            startWith('onInit'), // Emit value to force load on page load; actual value does not matter
            flatMap(_ => this.service.getCustom(this.criteria)), // Get some items
            tap(_ => this.loading = false) // Turn off the spinner
          );
  }

  onSearch(event){
    this.criteria["search"] = event.target.value;
    this.onRefresh();
  }

  dateCnanged(event: IMyDateRangeModel){
    this.criteria["start"] = event.beginJsDate.toISOString();
    this.criteria["end"] = event.endJsDate.toISOString();
    this.onRefresh();
  }

  switchOrder(type:string){
    this.order = type;
    this.criteria["order"] = type;
    this.onRefresh();
  }

  onRefresh() {
    this.loading = true; // Turn on the spinner.
    this.refresh.next('onRefresh'); // Emit value to force reload; actual value does not matter
  }

  switchType(type:string){
    this.criteria.type = type;
    this.type = type;
    this.onRefresh();
  }
  csv(){
    
  }







}
