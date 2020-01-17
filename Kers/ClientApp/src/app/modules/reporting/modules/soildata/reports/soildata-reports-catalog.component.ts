import { Component, OnInit, Input } from '@angular/core';
import { SoilReportSearchCriteria, SoilReportBundle, TypeForm } from '../soildata.report';
import { Subject, Observable } from 'rxjs';
import { IMyDrpOptions, IMyDateRangeModel } from 'mydaterangepicker';
import { SoildataService } from '../soildata.service';
import { startWith, flatMap, tap } from 'rxjs/operators';

@Component({
  selector: 'soildata-reports-catalog',
  templateUrl: 'soildata-reports-catalog.component.html'
})
export class SoildataReportsCatalogComponent implements OnInit {
  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  reports$:Observable<SoilReportBundle[]>;
  type="dsc";


  @Input() criteria:SoilReportSearchCriteria;
  @Input() startDate:Date;
  @Input() endDate:Date;



  condition = false;


  myDateRangePickerOptions: IMyDrpOptions = {
      // other options...
      dateFormat: 'mmm dd, yyyy',
      showClearBtn: false,
      showApplyBtn: false,
      showClearDateRangeBtn: false
  };
  model = {beginDate: {year: 2018, month: 10, day: 9},
                             endDate: {year: 2018, month: 10, day: 19}};


  typesCheckboxes = [];
  statusesCheckboxes = [];

  get selectedFormTypes() { 
    return this.typesCheckboxes
              .filter(opt => opt.checked)
              .map(opt => opt.value)
  }

  get selectedReportStatuses() { 
    return this.statusesCheckboxes
              .filter(opt => opt.checked)
              .map(opt => opt.value)
  }

  constructor(
    private service:SoildataService
  ) { }

  ngOnInit() {
    if( this.startDate == null){
      this.startDate = new Date();
      this.startDate.setMonth( this.startDate.getMonth() -2);
    }
    if( this.endDate == null ){
      this.endDate = new Date();
    }
    this.service.formTypes().subscribe(
      res => {
        for(let type of res){
          this.criteria.formType.push(type.id);
          this.typesCheckboxes.push({
            name:type.code, value: type.id, checked:true
          })
        }
      }
    )

    this.service.reportStatuses().subscribe(
      res => {
        for(let status of res){
          this.criteria.status.push(status.id);
          this.statusesCheckboxes.push({
            name:status.name, value: status.id, checked:true
          })
        }
      }
    )
    
    this.criteria = {
      start: this.startDate.toISOString(),
      end: this.endDate.toISOString(),
      search: "",
      order: 'dsc',
      status: [],
      formType: []
    }
    this.model.beginDate = {year: this.startDate.getFullYear(), month: this.startDate.getMonth() + 1, day: this.startDate.getDate()};
    this.model.endDate = {year: this.endDate.getFullYear(), month: this.endDate.getMonth() + 1, day: this.endDate.getDate()};
    
    this.refresh = new Subject();

    this.reports$ = this.refresh.asObservable()
      .pipe(
        startWith('onInit'), // Emit value to force load on page load; actual value does not matter
        flatMap(_ => this.service.getCustom(this.criteria)), // Get some items
        tap(_ => this.loading = false) // Turn off the spinner
      );
  }


  dateCnanged(event: IMyDateRangeModel){
    this.startDate = event.beginJsDate;
    this.endDate = event.endJsDate;
    this.criteria["start"] = event.beginJsDate.toISOString();
    this.criteria["end"] = event.endJsDate.toISOString();
    this.onRefresh();
  }

  onSearch(event){
    this.criteria["search"] = event.target.value;
    this.onRefresh();
  }
  onFormTypeChange(){
    this.criteria.formType = this.selectedFormTypes;
    this.onRefresh();
  }

  onReportStatusesChange(){
    this.criteria.status = this.selectedReportStatuses;
    this.onRefresh;
  }

  onRefresh() {
    this.loading = true; // Turn on the spinner.
    this.refresh.next('onRefresh'); // Emit value to force reload; actual value does not matter
  }
  
  switchOrder(type:string){
    this.type = type;
    this.criteria["order"] = type;
    this.onRefresh();
  }


}
