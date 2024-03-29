import { Component, OnInit, Input } from '@angular/core';

import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import { Observable, Subject } from 'rxjs';
import { startWith, tap, mergeMap } from 'rxjs/operators';
import { StateService, CongressionalDistrict, ExtensionArea, ExtensionRegion } from '../../../state/state.service';
import { ExtensionPosition, PlanningUnit, Specialty, UserService } from "../../../user/user.service";
import { saveAs } from 'file-saver';
import { ActivitySearchCriteria, ActivitySeearchResultsWithCount, ActivityService } from '../../activity.service';
import { ActivityOption } from '../../../servicelog/servicelog.service';

@Component({
  selector: 'activity-filter',
  templateUrl: './activity-filter.component.html',
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
export class ActivityFilterComponent implements OnInit {

  @Input() userId:number | null = null;
  @Input() criteria:ActivitySearchCriteria;
  condition = false;
  isUser = false;
  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  revisions$:Observable<ActivitySeearchResultsWithCount>;
  congressional$:Observable<CongressionalDistrict[]>;
  regions$:Observable<ExtensionRegion[]>;
  areas$:Observable<ExtensionArea[]>;
  counties$:Observable<PlanningUnit[]>;
  positions$:Observable<ExtensionPosition[]>;
  programAreas$:Observable<Specialty[]>;


  type="direct";
  order = "dsc";

  myDateRangePickerOptions: IAngularMyDpOptions = {
      dateRange: true,
      dateFormat: 'mmm dd, yyyy',
      alignSelectorRight: true
  };



   model: IMyDateModel = null;
  
  optionsCheckboxes = [];
  get selectedOptions() { 
    return this.optionsCheckboxes
              .filter(opt => opt.checked)
              .map(opt => opt.value)
  }



  csvData = [];
  csvCriteria:ActivitySearchCriteria;
  csvInitiated = false;
  csvChunkSize = 8;
  csvTotalBatches:number;
  csvBatchesCompleted = 0;
  csvResultsCount = 0;
  csvAverageBatchTime = 0;

  constructor(
    private service:ActivityService,
    private stateService:StateService,
    private userService:UserService
  ) { }

  ngOnInit() {
    this.isUser = (this.userId != null);
    this.congressional$ = this.stateService.congressional();
    this.regions$ = this.stateService.regions();
    this.counties$ = this.stateService.counties();
    this.positions$ = this.userService.extensionPositions();
    this.programAreas$ = this.userService.specialties();
    this.service.options().subscribe(
      res => {
        for( let type of res){
          this.optionsCheckboxes.push(
            { name: type.name, value: type.id, checked:false }
          )
        }
      }
    );

    var end = new Date();
    var start = new Date();
    start.setMonth(end.getMonth()-1);

    this.model = {
      isRange: true, 
      singleDate: null, 
      dateRange: {
        beginDate: {
          year: start.getFullYear(), month: start.getMonth() + 1, day: start.getDate()
        },
        endDate: {
          year: end.getFullYear(), month: end.getMonth() + 1, day: end.getDate()
        }
      }
    };


    this.criteria = {
      start: start.toISOString(),
      end: end.toISOString(),
      search: "",
      order: 'dsc',
      congressionalDistrictId: null,
      regionId: null,
      areaId: null,
      unitId: null,
      positionId: null,
      specialtyId: null,
      options: [],
      skip: 0,
      take: 20
    }

    


    this.refresh = new Subject();
    
    this.revisions$ = this.refresh.asObservable()
          .pipe(
            startWith('onInit'), // Emit value to force load on page load; actual value does not matter
            mergeMap(_ => this.service.getCustom(this.criteria, this.userId)), // Get some items
            tap(_ => this.loading = false) // Turn off the spinner
          );
  }

  onSearch(event){
    this.criteria["search"] = event.target.value;
    this.onRefresh();
  }

  onDateChanged(event: IMyDateModel){
    this.criteria["start"] = event.dateRange.beginJsDate.toISOString();
    this.criteria["end"] = event.dateRange.endJsDate.toISOString();
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
  onOptionChange(){
    this.criteria.options = this.selectedOptions;
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
  onPositionChange(event){
    if(event.target.value == "null"){
      this.criteria.positionId = undefined;
    }else{
      this.criteria["positionId"] = event.target.value;
    }
    this.onRefresh();
  }
  onSpecialtyChange(event){
    if(event.target.value == "null"){
      this.criteria.specialtyId = undefined;
    }else{
      this.criteria["specialtyId"] = event.target.value;
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
      this.service.getCustomData(this.csvCriteria, this.userId).subscribe(
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
