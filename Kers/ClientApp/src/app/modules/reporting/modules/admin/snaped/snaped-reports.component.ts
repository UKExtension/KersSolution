import { Component, OnInit, Input } from '@angular/core';
import { SnapedSearchCriteria, SnapedAdminService, SnapSearchResult, SnapSeearchResultsWithCount } from './snaped-admin.service';
import { IMyDrpOptions, IMyDateRangeModel } from 'mydaterangepicker';
import { Observable, Subject } from 'rxjs';
import { startWith, flatMap, tap } from 'rxjs/operators';
import { StateService, CongressionalDistrict, ExtensionArea, ExtensionRegion } from '../../state/state.service';
import { PlanningUnit } from "../../user/user.service";
import { saveAs } from 'file-saver';

@Component({
  selector: 'snaped-reports',
  templateUrl: './snaped-reports.component.html',
  styles: [`
  .download-overlay{
    background-color:rgba(220,239,230, 0.8);
    border: 3px solid rgba(120,139,130, 0.2);
    position: fixed;
    top:0;
    bottom:0;
    left:0;
    right:0;
    z-index: 100;
    padding: 10px;
  }
  `]
})
export class SnapedReportsComponent implements OnInit {
  condition = false;

  @Input() criteria:SnapedSearchCriteria;
  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  revisions$:Observable<SnapSeearchResultsWithCount>;
  congressional$:Observable<CongressionalDistrict[]>;
  regions$:Observable<ExtensionRegion[]>;
  areas$:Observable<ExtensionArea[]>;
  counties$:Observable<PlanningUnit[]>
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


  csvData = [];
  csvCriteria:SnapedSearchCriteria;
  csvInitiated = false;
  csvChunkSize = 3;
  csvTotalBatches:number;
  csvBatchesCompleted = 0;
  csvResultsCount = 0;
  csvAverageBatchTime = 0;

  constructor(
    private service:SnapedAdminService,
    private stateService:StateService
  ) { }

  ngOnInit() {
    this.congressional$ = this.stateService.congressional();
    this.regions$ = this.stateService.regions();
    this.counties$ = this.stateService.counties();

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
      congressionalDistrictId: null,
      regionId: null,
      areaId: null,
      unitId: null,
      type: this.type,
      skip: 0,
      take: 20
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
  onCongressionalChange(event){
    if(event.target.value == "null"){
      this.criteria.congressionalDistrictId = undefined;
    }else{
      this.criteria.congressionalDistrictId = <number>event.target.value;
    }
    this.onRefresh();
  }
  onRegionChange(event){
    this.criteria.areaId = undefined;
    if(event.target.value == "null"){
      this.criteria.regionId = undefined;
    }else{
      this.criteria.regionId = <number>event.target.value;
      this.areas$ = this.stateService.areas(this.criteria.regionId);
    }
    this.onRefresh();
  }

  onAreaChange(event){
    if(event.target.value == "null"){
      this.criteria.areaId = undefined;
    }else{
      this.criteria.areaId = <number>event.target.value;
    } 
    this.onRefresh();
  }
  onCountyChange(event){
    if(event.target.value == "null"){
      this.criteria.unitId = undefined;
    }else{
      this.criteria["unitId"] = event.target.value;
    }
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
    this.csvInitiated=true;
    this.csvCriteria = Object.assign({}, this.criteria);
    this.csvCriteria.skip = 0;
    this.csvCriteria.take = this.csvChunkSize;
    this.csvTotalBatches = Math.ceil( this.csvResultsCount/this.csvChunkSize );
    this.csvBatchesCompleted = 0;
    this.service.GetCustomDataHeader().subscribe(
      res => {
        this.csvData=[res];
        this.getCsvData();
      }
    )
     
  }

  getCsvData(){
    if(this.csvCriteria.skip < this.csvResultsCount && this.csvInitiated){
      var startDate = new Date();
      this.service.getCustomData(this.csvCriteria).subscribe(
        res => {
          this.csvData = this.csvData.concat(res);
          this.csvBatchesCompleted++;
          this.csvCriteria.skip = this.csvCriteria.skip + this.csvChunkSize;
          var endDate   = new Date();
          var seconds = (endDate.getTime() - startDate.getTime()) / 1000;
          this.csvAverageBatchTime = (this.csvAverageBatchTime == 0 ? seconds : (this.csvAverageBatchTime + seconds) /2)
          this.getCsvData();
        }
      )
    }else{
      this.downloadFile(this.csvData);
      this.csvAverageBatchTime = 0;
      this.csvInitiated=false;
    }
     
  }
  downloadFile(data: any) {
    const replacer = (key, value) => value === null ? '' : value; // specify how you want to handle null values here
    const header = Object.keys(data[0]);
    let csv = data.map(row => header.map(fieldName => JSON.stringify(row[fieldName], replacer)).join(','));
    //csv.unshift(header.join(','));
    let csvArray = csv.join('\r\n');

    var blob = new Blob([csvArray], {type: 'text/csv' })
    saveAs(blob, "KERS_ServiceLogData.csv");
  }

  getTimeRemaining():number{
    return (this.csvTotalBatches - this.csvBatchesCompleted ) * this.csvAverageBatchTime / 60;

  }


  getCount(count:number){
    this.csvResultsCount = count;
    return count;
  }


  loadMore(){
    this.criteria.take += 20;
    this.onRefresh();

  }







}
