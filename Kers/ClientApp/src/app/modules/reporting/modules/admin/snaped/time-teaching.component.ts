import { Component, OnInit } from '@angular/core';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import { Observable, Subject } from 'rxjs';
import { flatMap, startWith, tap } from 'rxjs/operators';
import { ServicelogService, SnapDirectSessionLength, SnapDirectSessionType } from '../../servicelog/servicelog.service';
import { CongressionalDistrict, ExtensionArea, ExtensionRegion, StateService } from '../../state/state.service';
import { PlanningUnit } from '../../user/user.service';
import { SnapedAdminService, SnapedSearchCriteria, SnapSeearchResultsWithCount } from './snaped-admin.service';

@Component({
  selector: 'app-time-teaching',
  template: `
    
  

  <h3>Time Spent Teaching</h3><br>
  <div class="row">
    <div class="col-xs-12 text-right" style="margin-top: 3px;">
      <div class="input-group" style="width:250px; float:right;">
              
        <input type="text" class="form-control input-box" placeholder="Click to select a date" 
        angular-mydatepicker name="mydate" (click)="dp.toggleCalendar()" 
        [(ngModel)]="model" [options]="myDateRangePickerOptions" 
        #dp="angular-mydatepicker" (dateChanged)="dateCnanged($event)">
        <span class="input-group-addon" id="basic-addon1" (click)="dp.toggleCalendar()" style="cursor: pointer;"><i class="fa fa-calendar"></i></span>
      </div>
    </div>
  
  </div>
  
  
  <a (click)="condition = !condition" style="cursor: pointer;"><i class="fa fa-minus-square" *ngIf="condition"></i>
    <i class="fa fa-plus-square" *ngIf="!condition"></i> more search options
  </a>
  <div class="row">
    <form *ngIf="condition" class="form-horizontal form-label-left">
      <div class="col-sm-offset-3 col-sm-9">
        <h2>Refine Search</h2>
      </div>
      <div class="form-group" >
          <label class="control-label col-md-3 col-sm-3 col-xs-12">Name</label>
          <div class="col-md-6 col-sm-6 col-xs-12">
              <input class="form-control" name="contact" (keyup)="onSearch($event)" />
          </div>
      </div>
      <div class="form-group" >
        <label class="control-label col-md-3 col-sm-3 col-xs-12">Congressional District</label>
        <div class="col-md-6 col-sm-6 col-xs-12">
          <select class="form-control" (change)="onCongressionalChange($event)" name="CongressionalDistrict">
            <option value="null">-- Select --</option>
            <option *ngFor="let congressional of congressional$ | async" value="{{congressional.id}}">{{congressional.name}}</option>
          </select>
        </div>
      </div>
      <div class="form-group" >
        <label class="control-label col-md-3 col-sm-3 col-xs-12">Region</label>
        <div class="col-md-6 col-sm-6 col-xs-12">
          <select class="form-control" (change)="onRegionChange($event)" name="Region">
            <option value="null">-- Select --</option>
            <option *ngFor="let region of regions$ | async" value="{{region.id}}">{{region.name}}</option>
          </select>
        </div>
      </div>
      <div class="form-group" *ngIf="criteria.regionId != null && criteria.regionId != 0">
        <label class="control-label col-md-3 col-sm-3 col-xs-12">Area</label>
        <div class="col-md-6 col-sm-6 col-xs-12">
          <select class="form-control" (change)="onAreaChange($event)" name="Region">
            <option value="null">-- Select --</option>
            <option *ngFor="let area of areas$ | async" value="{{area.id}}">{{area.name}}</option>
          </select>
        </div>
      </div>
      <div class="form-group" >
        <label class="control-label col-md-3 col-sm-3 col-xs-12">County</label>
        <div class="col-md-6 col-sm-6 col-xs-12">
          <select class="form-control" (change)="onCountyChange($event)" name="Region">
            <option value="null">-- Select --</option>
            <option *ngFor="let county of counties$ | async" value="{{county.id}}">{{county.name}}</option>
          </select>
        </div>
      </div>
      <div class="form-group" >
          <label class="control-label col-md-3 col-sm-3 col-xs-12">Order by</label>
          <div class="col-md-9 col-sm-9 col-xs-12">
              <div class="btn-group" data-toggle="buttons">
                  <label class="btn btn-default" [class.active]="order=='dsc'">
                  <input type="radio" name="type" id="option2" (click)="switchOrder('dsc')"> Date Descending
                  </label>
                  <label class="btn btn-default" [class.active]="order=='asc'">
                    <input type="radio" name="type" id="option3" (click)="switchOrder('asc')"> Date Ascending
                  </label>
                  <label class="btn btn-default" [class.active]="order=='alph'">
                      <input type="radio" name="type" id="option4" (click)="switchOrder('alph')"> Alphabetically
                    </label>
              </div>
          </div>
      </div>  
      
      
      
    </form>
  </div><br>

  <div class="table-responsive" *ngIf="revisions$ | async as revisions">

    <table class="table">
      <thead>
        <tr>
          <th>SNAP-Ed Sessions</th>
          <th>Number delivered</th>
          <th *ngFor="let length of sessionLengths$ | async"># of Sessions {{length.name}} Minutes Teaching</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let type of (sessionTypes$ | async); let i = index">
          <td>{{type.name}}</td>
          <td>{{revisions[i][0]}}</td>
          <td *ngFor="let length of (sessionLengths$ | async); let j = index">{{revisions[i][j+1]}}</td>
        </tr>
      <tbody>
    </table>

  <loading *ngIf="loading"></loading>



</div>





  `,
  styles: [
  ]
})
export class TimeTeachingComponent implements OnInit {


  condition = false;


  criteria:SnapedSearchCriteria;
  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  revisions$:Observable<number[][]>;
  congressional$:Observable<CongressionalDistrict[]>;
  regions$:Observable<ExtensionRegion[]>;
  areas$:Observable<ExtensionArea[]>;
  counties$:Observable<PlanningUnit[]>
  type="direct";
  order = "dsc";

  sessionTypes$:Observable<SnapDirectSessionType[]>;
  sessionLengths$:Observable<SnapDirectSessionLength[]>;


  myDateRangePickerOptions: IAngularMyDpOptions = {
    dateRange: true,
    dateFormat: 'mmm dd, yyyy'
  };
  model: IMyDateModel = null;





  constructor(
    private service:SnapedAdminService,
    private stateService:StateService,
    private servicelogService: ServicelogService
  ) { }

  ngOnInit(): void {
    this.congressional$ = this.stateService.congressional();
    this.regions$ = this.stateService.regions();
    this.counties$ = this.stateService.counties();

    this.sessionTypes$ = this.servicelogService.sessiontypes();
    this.sessionLengths$ = this.servicelogService.sessionlengths();

    var startDate = new Date();
    startDate.setMonth( startDate.getMonth() - 1);
    var endDate = new Date();


    this.model = {
      isRange: true, 
      singleDate: null, 
      dateRange: {
        beginDate: {
          year: startDate.getFullYear(), month: startDate.getMonth() + 1, day: startDate.getDate()
        },
        endDate: {
          year: endDate.getFullYear(), month: endDate.getMonth() + 1, day: endDate.getDate()
        }
      }
    };


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
            flatMap(_ => this.service.getTimeTeaching(this.criteria)), // Get some items
            tap(_ => this.loading = false) // Turn off the spinner
          );
  }





  onSearch(event){
    this.criteria["search"] = event.target.value;
    this.onRefresh();
  }

  dateCnanged(event: IMyDateModel){
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



  getCount(count:number){
    return count;
  }


  loadMore(){
    this.criteria.take += 20;
    this.onRefresh();

  }











}
